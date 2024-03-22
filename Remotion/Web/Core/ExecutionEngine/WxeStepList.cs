// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{

  /// <summary>
  ///   Performs a sequence of steps in a web application.
  /// </summary>
  [Serializable]
  public class WxeStepList : WxeStep
  {
    private List<WxeStep> _steps = new List<WxeStep>();

    private int _executingStep = -1;

    private bool _isDirtyFromExecutedSteps;

    public WxeStepList ()
    {
      InitializeSteps();
    }

    /// <summary>
    ///   Moves all steps to <paramref name="innerList"/> and makes <paramref name="innerList"/> the only step of 
    ///   this step list.
    /// </summary>
    /// <param name="innerList"> 
    ///   This list will be the only step of the <see cref="WxeStepList"/> and contain all of the 
    ///   <see cref="WxeStepList"/>'s steps. Must be empty and not executing.
    /// </param>
    protected void Encapsulate (WxeStepList innerList)
    {
      if (this.IsExecutionStarted)
        throw new InvalidOperationException("Cannot encapsulate executing list.");
      if (innerList.Count > 0)
        throw new ArgumentException("List must be empty.", "innerList");
      if (innerList.IsExecutionStarted)
        throw new ArgumentException("Cannot encapsulate into executing list.", "innerList");

      innerList._steps = this._steps;
      foreach (WxeStep step in innerList._steps)
        step.SetParentStep(innerList);

      this._steps = new List<WxeStep>(1);
      this._steps.Add(innerList);
      innerList.SetParentStep(this);
    }

    public override void Execute (WxeContext context)
    {
      if (!IsExecutionStarted)
        _executingStep = 0;

      while (_executingStep < _steps.Count)
      {
        var currentStep = _steps[_executingStep];
        if (currentStep.IsAborted)
          throw new InvalidOperationException("Step " + _executingStep + " of " + this.GetType().GetFullNameSafe() + " is aborted.");
        currentStep.Execute(context);

        _isDirtyFromExecutedSteps = _isDirtyFromExecutedSteps || currentStep.EvaluateDirtyState();

        _executingStep++;
      }
    }

    public WxeStep this[int index]
    {
      get { return _steps[index]; }
    }

    public int Count
    {
      get { return _steps.Count; }
    }

    public void Add (WxeStep step)
    {
      ArgumentUtility.CheckNotNull("step", step);

      _steps.Add(step);
      step.SetParentStep(this);
    }

    public void Add (WxeStepList target, MethodInfo method)
    {
      ArgumentUtility.CheckNotNull("target", target);
      ArgumentUtility.CheckNotNull("method", method);

      Add(new WxeMethodStep(target, method));
    }

    public void AddStepList (WxeStepList steps)
    {
      ArgumentUtility.CheckNotNull("steps", steps);

      for (int i = 0; i < steps.Count; i++)
        Add(steps[i]);
    }

    public void Insert (int index, WxeStep step)
    {
      if (_executingStep >= index)
        throw new ArgumentException("Cannot insert step only after the last executed step.", "index");
      ArgumentUtility.CheckNotNull("step", step);

      _steps.Insert(index, step);
      step.SetParentStep(this);
    }

    public override WxeStep ExecutingStep
    {
      get
      {
        if (_executingStep < _steps.Count && IsExecutionStarted)
          return _steps[_executingStep].ExecutingStep;
        else
          return this;
      }
    }

    public WxeStep? LastExecutedStep
    {
      get
      {
        if (_executingStep < _steps.Count && IsExecutionStarted)
          return _steps[_executingStep];
        else
          return null;
      }
    }

    public bool IsExecutionStarted
    {
      get { return _executingStep >= 0; }
    }

    private void InitializeSteps ()
    {
      Type type = this.GetType();
      MemberInfo[] members = NumberedMemberFinder.FindMembers(
          type,
          "Step",
          MemberTypes.Field | MemberTypes.Method | MemberTypes.NestedType,
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

      foreach (MemberInfo member in members)
      {
        if (member is FieldInfo)
        {
          FieldInfo fieldInfo = (FieldInfo)member;
          Add((WxeStep)fieldInfo.GetValue(this)!);
        }
        else if (member is MethodInfo)
        {
          MethodInfo methodInfo = (MethodInfo)member;
          Add(this, methodInfo);
        }
        else if (member is Type)
        {
          Type subtype = (Type)member;
          if (typeof(WxeStep).IsAssignableFrom(subtype))
            Add((WxeStep)Activator.CreateInstance(subtype)!);
        }
      }
    }

    protected override void AbortRecursive ()
    {
      base.AbortRecursive();
      foreach (WxeStep step in _steps)
        step.Abort();
    }

    /// <summary>
    /// Resets the dirty state for any previously executed steps recursively.
    /// </summary>
    public override void ResetDirtyStateForExecutedSteps ()
    {
      base.ResetDirtyStateForExecutedSteps();

      for (var i = 0; i < _executingStep; i++)
      {
        var step = _steps[i];
        step.ResetDirtyStateForExecutedSteps();
      }

      _isDirtyFromExecutedSteps = false;
    }

    /// <summary>
    /// Evaluates the current dirty state for this <see cref="WxeStepList"/>.
    /// </summary>
    /// <remarks>
    /// The evaluation includes the information for the currently executing <see cref="WxeStep"/>, the aggregated dirty state of previously executed steps.
    /// </remarks>
    /// <returns><see langword="true" /> if the <see cref="WxePageStep"/> represents unsaved changes.</returns>
    public override bool EvaluateDirtyState ()
    {
      if (IsDirtyStateEnabled)
      {
        if (_isDirtyFromExecutedSteps)
          return true;
      }

      // Using WxeStepList.ExecutingStep instead of LastExecutedStep would return the most-nested executing step,
      // thus skipping the recursive evaluation of the dirty state.
      var currentStepInThisStepList = LastExecutedStep;

      // Evaluating the steps for the current dirty state is relevant even when IsDirtyStateEnabled is false
      // to include information on sub-function dirty state in the result given that dirty state handling is disabled for each WxeFunction individually,
      // instead of disabling dirty state handling across the entire stack.
      if (currentStepInThisStepList?.EvaluateDirtyState() == true)
        return true;

      return base.EvaluateDirtyState();
    }
  }

}
