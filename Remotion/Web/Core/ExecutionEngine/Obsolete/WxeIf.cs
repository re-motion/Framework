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
using System.Reflection;

namespace Remotion.Web.ExecutionEngine.Obsolete
{
  /// <summary>
  /// If-Then-Else block.
  /// </summary>
  public abstract class WxeIf: WxeStep
  {
    WxeStepList? _stepList = null; // represents Then or Else step list, depending on result of If()

    public override void Execute (WxeContext context)
    {
      Type type = this.GetType();
      if (_stepList == null)
      {
        MethodInfo? ifMethod = type.GetMethod(
            "If", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new Type[0], null);
        if (ifMethod == null || ifMethod.ReturnType != typeof(bool))
          throw new WxeException("If-block " + type.FullName + " does not define method \"bool If()\".");

        bool result = (bool)ifMethod.Invoke(this, new object[0])!;
        if (result)
        {
          _stepList = GetResultList("Then");
          if (_stepList == null)
            throw new WxeException("If-block " + type.FullName + " does not define nested class \"Then\".");
        }
        else
        {
          _stepList = GetResultList("Else");
        }
      }

      if (_stepList != null)
      {
        _stepList.Execute(context);
      }
    }

    private WxeStepList? GetResultList (string name)
    {
      Type type = this.GetType();
      Type? stepListType = type.GetNestedType(name, BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
      if (stepListType == null)
        return null;
      if (! typeof(WxeStepList).IsAssignableFrom(stepListType))
        throw new WxeException("Type " + stepListType.FullName + " must be derived from WxeStepList.");

      WxeStepList resultList = (WxeStepList)System.Activator.CreateInstance(stepListType)!;
      resultList.SetParentStep(this);
      return resultList;
    }

    public override WxeStep ExecutingStep
    {
      get
      {
        if (_stepList == null)
          throw new WxeException("ExecutingStep must not be accessed outside of the WXE execution.");
        else
          return _stepList.ExecutingStep;
      }
    }

    protected override void AbortRecursive ()
    {
      base.AbortRecursive();
      if (_stepList != null)
        _stepList.Abort();
    }

    public override void ResetDirtyStateForExecutedSteps ()
    {
      base.ResetDirtyStateForExecutedSteps();

      _stepList?.ResetDirtyStateForExecutedSteps();
    }

    public override bool EvaluateDirtyState ()
    {
      if (_stepList != null && _stepList.EvaluateDirtyState())
        return true;

      return base.EvaluateDirtyState();
    }

    public sealed override bool IsDirtyStateEnabled => base.IsDirtyStateEnabled;
  }
}
