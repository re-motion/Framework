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

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  /// <summary>
  /// The <see cref="IExecutionState"/> interface is defines the state-pattern for executing a sub-function within a <see cref="WxeStep"/>.
  /// </summary>
  public interface IExecutionState : INullObject
  {
    /// <summary> Gets the context of the execution. Use this member to transistion the <see cref="WxeStep"/> into the next state. </summary>
    IExecutionStateContext ExecutionStateContext { get; }

    /// <summary> Gets a set of parameters common for all execution states, such as the executing <see cref="WxeFunction"/>. </summary>
    IExecutionStateParameters Parameters { get; }

    /// <summary> Gets a flag that informs the observer whether the state is executing. This value is typically constant for a state implementation. </summary>
    bool IsExecuting { get; }

    /// <summary>
    /// Executes the behavor of the current state and uses the <see cref="ExecutionStateContext"/> to transistion the <see cref="WxeStep"/>
    /// into the next state. 
    /// </summary>
    void ExecuteSubFunction (WxeContext context);
  }
}
