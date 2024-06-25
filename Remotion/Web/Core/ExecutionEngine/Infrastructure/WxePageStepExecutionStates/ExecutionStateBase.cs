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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  /// <summary>
  /// The <see cref="ExecutionStateBase{TParameters}"/> type is an abstract base class implementing the <see cref="IExecutionState"/> interface.
  /// </summary>
  /// <typeparam name="TParameters">The type of the <see cref="IExecutionStateParameters"/> required by the concrete type.</typeparam>
  /// <remarks>
  /// Derive from this type and provide an implementation for the <see cref="ExecuteSubFunction"/> method to define the behavior of this state type.
  /// </remarks>
  public abstract class ExecutionStateBase<TParameters> : IExecutionState
      where TParameters: IExecutionStateParameters
  {
    private readonly IExecutionStateContext _executionStateContext;
    private readonly TParameters _parameters;

    protected ExecutionStateBase (IExecutionStateContext executionStateContext, TParameters parameters)
    {
      ArgumentUtility.CheckNotNull("executionStateContext", executionStateContext);
      ArgumentUtility.CheckNotNull("parameters", parameters);

      _executionStateContext = executionStateContext;
      _parameters = parameters;
    }

    public abstract void ExecuteSubFunction (WxeContext context);

    public bool IsExecuting
    {
      get { return true; }
    }

    public IExecutionStateContext ExecutionStateContext
    {
      get { return _executionStateContext; }
    }

    public TParameters Parameters
    {
      get { return _parameters; }
    }

    IExecutionStateParameters IExecutionState.Parameters
    {
      get { return Parameters; }
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
