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
using System.ComponentModel;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Obsolete;

namespace Remotion.Web.ExecutionEngine
{

/// <summary> Performs a single operation in a web application as part of a <see cref="WxeFunction"/>. </summary>
/// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/Class/*' />
public abstract class WxeStep
{
  /// <summary> Gets the <see cref="WxeFunction"/> for the passed <see cref="WxeStep"/>. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/GetFunction/*' />
  public static WxeFunction? GetFunction (WxeStep? step)
  {
    return WxeStep.GetStepByType<WxeFunction>(step);
  }

  /// <summary>
  /// Gets the first step of the specified type <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">The type of step to get.</typeparam>
  /// <param name="step">The step from which to start searching for the given step type <typeparamref name="T"/>.</param>
  /// <returns>
  ///   The first <see cref="WxeStep"/> of the specified <typeparamref name="T"/> or <see langword="null"/> if the 
  ///   neither the <paramref name="step"/> nor it's parent steps are of a matching type. 
  /// </returns>
  protected static T? GetStepByType<T> (WxeStep? step)
      where T : WxeStep
  {
    for (;
          step != null;
          step = step.ParentStep)
    {
      T? expectedStep = step as T;
      if (expectedStep != null)
        return expectedStep;
    }
    return null;
  }

  /// <summary> Used to pass a variable by reference to a <see cref="WxeFunction"/>. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/varref/*' />
  protected static WxeVariableReference varref (string localVariable)
  {
    return new WxeVariableReference(localVariable);
  }

  private WxeStep? _parentStep = null;
  private bool _isAborted = false;
  /// <summary> 
  ///   <see langword="true"/> during the execution of <see cref="Abort"/>. Used to prevent circular aborting.
  /// </summary>
  private bool _isAborting = false;

  /// <overloads>
  /// <summary> Executes the <see cref="WxeStep"/>. </summary>
  /// <remarks> This method should only be invoked by the WXE infrastructure. </remarks>
  /// </overloads>
  /// <summary> Executes the <see cref="WxeStep"/>. </summary>
  /// <remarks> 
  ///   Invokes <see cref="M:Remotion.Web.ExecutionEngine.WxeStep.Execute(Remotion.Web.ExecutionEngine.WxeContext">WxeContext</see>,
  ///   passing the <see cref="WxeContext.Current"/> <see cref="WxeContext"/> as argument.
  ///   <note>
  ///     This method should only be invoked by the WXE infrastructure.
  ///   </note>
  /// </remarks>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public void Execute ()
  {
    Execute(WxeContext.Current!); // TODO RM-8118: not null assertion
  }

  /// <summary> Executes the <see cref="WxeStep"/>. </summary>
  /// <param name="context"> The <see cref="WxeContext"/> containing the information about the execution. </param>
  /// <remarks> 
  ///   Override this method to implement your execution logic. 
  ///   <note>
  ///     This method should only be invoked by the WXE infrastructure.
  ///   </note>
  /// </remarks>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract void Execute (WxeContext context);

  /// <summary> Gets the scope's variables collection. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/Variables/*' />
  public virtual NameObjectCollection? Variables
  {
    get { return (_parentStep == null) ? null : _parentStep.Variables; }
  }

  /// <summary> Gets the parent step of the the <see cref="WxeStep"/>. </summary>
  /// <value> The <see cref="WxeStep"/> assigned using <see cref="SetParentStep"/>. </value>
  public WxeStep? ParentStep
  {
    get { return _parentStep; }
  }

  /// <summary> Sets the parent step of this <see cref="WxeStep"/>. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/SetParentStep/*' />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public void SetParentStep (WxeStep parentStep)
  {
    ArgumentUtility.CheckNotNull("parentStep", parentStep);
    _parentStep = parentStep;
  }

  /// <summary> Gets the step currently being executed. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/ExecutingStep/*' />
  public virtual WxeStep ExecutingStep
  {
    get { return this; }
  }

  /// <summary> Gets the root <see cref="WxeFunction"/> of the execution hierarchy. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/RootFunction/*' />
  public WxeFunction? RootFunction
  {
    get
    {
      WxeStep step = this;
      while (step.ParentStep != null)
        step = step.ParentStep;
      return step as WxeFunction;
    }
  }

  /// <summary> Gets the parent <see cref="WxeFunction"/> for this <see cref="WxeStep"/>. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/ParentFunction/*' />
  public WxeFunction? ParentFunction
  {
    get { return WxeStep.GetFunction(ParentStep); }
  }

  /// <summary> 
  ///   Gets the <see cref="Exception"/> caught by the <see cref="WxeTryCatch"/> block encapsulating this 
  ///   <see cref="WxeStep"/>.
  /// </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/CurrentException/*' />
  protected Exception? CurrentException
  {
    get
    {
      for (WxeStep? step = this;
           step != null;
           step = step.ParentStep)
      {
        if (step is WxeCatchBlock)
          return ((WxeCatchBlock)step).Exception;
      }

      return null;
    }
  }

  /// <summary> Gets a flag describing whether the <see cref="WxeStep"/> has been aborted. </summary>
  /// <value> <see langword="true"/> once <see cref="AbortRecursive"/> as finished executing. </value>
  public bool IsAborted
  {
    get { return _isAborted; }
  }

  /// <summary> Aborts the <b>WxeStep</b> by calling <see cref="AbortRecursive"/>. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/Abort/*' />
  [EditorBrowsable(EditorBrowsableState.Never)]
  public void Abort ()
  {
    if (! _isAborted && ! _isAborting)
    {
      _isAborting = true;
      AbortRecursive();
      _isAborted = true;
      _isAborting = false;
    }
  }

  /// <summary> Contains the aborting logic for the <see cref="WxeStep"/>. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/AbortRecursive/*' />
  protected virtual void AbortRecursive ()
  {
  }

  /// <summary>
  /// Allows clearing the accumulated dirty state history for this <see cref="WxeStep"/>.
  /// </summary>
  public virtual void ResetDirtyStateForExecutedSteps ()
  {
  }

  /// <summary>
  /// Evaluates the current dirty state for this <see cref="WxeStep"/>.
  /// </summary>
  /// <returns><see langword="true" /> if the <see cref="WxeStep"/> represents unsaved changes.</returns>
  public virtual bool EvaluateDirtyState ()
  {
    return false;
  }

  /// <summary> Gets the flag that determines whether to include this <see cref="WxeStep"/>'s dirty state during a call to <see cref="EvaluateDirtyState"/>. </summary>
  /// <value>
  /// The value returned by this <see cref="WxeStep"/>'s <see cref="ParentFunction"/> or <see langword="true" />
  /// if this <see cref="WxeStep"/> does not have a <see cref="ParentFunction"/>.
  /// </value>
  public virtual bool IsDirtyStateEnabled
  {
    get
    {
      var parentFunction = ParentFunction;
      if (parentFunction == null)
        return true;

      return parentFunction.IsDirtyStateEnabled;
    }
  }
}

}
