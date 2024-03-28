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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// Creates instances of <see cref="DelegateBasedReadWriteCommand{TIn,TOut}"/> or <see cref="DelegateBasedReadOnlyCommand{TIn,TOut}"/>.
  /// Use this factory class to avoid having to pass all generic arguments to the respective constructor by hand.
  /// </summary>
  public static class DelegateBasedCommand
  {
    /// <summary>
    /// Creates instances of <see cref="DelegateBasedReadWriteCommand{TIn,TOut}"/>. Use this factory method to avoid having
    /// to pass all generic arguments to <see cref="DelegateBasedReadWriteCommand{TIn,TOut}"/>'s constructor by hand.
    /// </summary>
    public static DelegateBasedReadWriteCommand<TIn, TOut> CreateForReadWrite<TIn, TOut> (
        IRdbmsProviderReadWriteCommand<TIn> command,
        Func<TIn, TOut> operation)
    {
      return new DelegateBasedReadWriteCommand<TIn, TOut>(command, operation);
    }

    /// <summary>
    /// Creates instances of <see cref="DelegateBasedReadOnlyCommand{TIn,TOut}"/>. Use this factory method to avoid having
    /// to pass all generic arguments to <see cref="DelegateBasedReadOnlyCommand{TIn,TOut}"/>'s constructor by hand.
    /// </summary>
    public static DelegateBasedReadOnlyCommand<TIn, TOut> CreateForReadOnly<TIn, TOut> (
        IRdbmsProviderReadOnlyCommand<TIn> command,
        Func<TIn, TOut> operation)
    {
      return new DelegateBasedReadOnlyCommand<TIn, TOut>(command, operation);
    }
  }
}
