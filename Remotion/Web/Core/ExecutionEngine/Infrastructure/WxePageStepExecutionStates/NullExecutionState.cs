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
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  /// <summary>
  /// The <see cref="NullExecutionState"/> is the null-object implementation of the <see cref="IExecutionState"/> interface. This state always
  /// returns <see langword="false" /> for the <see cref="IsExecuting"/> property.
  /// </summary>
  [Serializable]
  public class NullExecutionState
      : IExecutionState,
#pragma warning disable SYSLIB0050
          IObjectReference
#pragma warning restore SYSLIB0050
  {
    public static readonly NullExecutionState Null = new NullExecutionState();

    private NullExecutionState ()
    {
    }

    public bool IsNull
    {
      get { return true; }
    }

    public IExecutionStateContext? ExecutionStateContext
    {
      get { return null; }
    }

    public IExecutionStateParameters? Parameters
    {
      get { return null; }
    }

    public bool IsExecuting
    {
      get { return false; }
    }

    public void ExecuteSubFunction (WxeContext context)
    {
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return NullExecutionState.Null;
    }
  }
}
