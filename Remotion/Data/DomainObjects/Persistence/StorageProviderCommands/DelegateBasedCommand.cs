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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.StorageProviderCommands
{
  /// <summary>
  /// Creates instances of <see cref="DelegateBasedCommand{TIn,TOut}"/>. Use this factory class to avoid having
  /// to pass all generic arguments to <see cref="DelegateBasedCommand{TIn,TOut}"/>'s constructor by hand.
  /// </summary>
  public static class DelegateBasedCommand
  {
    /// <summary>
    /// Creates instances of <see cref="DelegateBasedCommand{TIn,TOut}"/>. Use this factory method to avoid having
    /// to pass all generic arguments to <see cref="DelegateBasedCommand{TIn,TOut}"/>'s constructor by hand.
    /// </summary>
    public static DelegateBasedCommand<TIn, TOut> Create<TIn, TOut> (
        IStorageProviderCommand<TIn> command,
        Func<TIn, TOut> operation)
    {
      return new DelegateBasedCommand<TIn, TOut>(command, operation);
    }
  }

  /// <summary>
  /// The <see cref="DelegateBasedCommand{TIn,TOut}"/> executes an <see cref="IStorageProviderCommand{T}"/>
  /// and applies a specified operation-transformation to the result.
  /// </summary>
  public class DelegateBasedCommand<TIn, TOut> : IStorageProviderCommand<TOut>
  {
    private readonly IStorageProviderCommand<TIn> _command;
    private readonly Func<TIn, TOut> _operation;

    public DelegateBasedCommand (IStorageProviderCommand<TIn> command, Func<TIn, TOut> operation)
    {
      ArgumentUtility.CheckNotNull("command", command);
      ArgumentUtility.CheckNotNull("operation", operation);

      _command = command;
      _operation = operation;
    }

    public IStorageProviderCommand<TIn> Command
    {
      get { return _command; }
    }

    public Func<TIn, TOut> Operation
    {
      get { return _operation; }
    }

    public TOut Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      var executionResult = _command.Execute(executionContext);
      return _operation(executionResult);
    }
  }
}
