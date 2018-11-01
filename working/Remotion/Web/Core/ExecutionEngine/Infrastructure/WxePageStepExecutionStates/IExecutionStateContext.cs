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
using System.Collections.Specialized;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  /// <summary>
  /// The <see cref="IExecutionStateContext"/> interface exposes the context in which the <see cref="IExecutionState"/> implementations operate.
  /// </summary>
  public interface IExecutionStateContext
  {
    /// <summary> Gets the current <see cref="IExecutionState"/> for the <see cref="WxeStep"/>. </summary>
    IExecutionState ExecutionState { get; }

    /// <summary> Transistions the <see cref="WxeStep"/> into the next <see cref="IExecutionState"/>. </summary>
    void SetExecutionState (IExecutionState executionState);

    /// <summary> Gets the function currently executing, i.e. usually the parent of the <see cref="CurrentStep"/>. </summary>
    WxeFunction CurrentFunction { get; }

    /// <summary> Getst the <see cref="WxeStep"/> that is executing the <see cref="WxeFunction"/>. </summary>
    WxeStep CurrentStep { get; }

    //TODO: Refactor to get rid of this member
    void SetReturnState (WxeFunction returningFunction, bool isReturningPostBack, NameValueCollection previousPostBackCollection);
  }
}
