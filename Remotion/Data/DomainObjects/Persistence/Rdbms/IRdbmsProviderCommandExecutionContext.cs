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
using System.Data;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// <see cref="IRdbmsProviderCommandExecutionContext"/> defines methods for creating and executing <see cref="IDbCommand"/> instances. These are
  /// used by RDBMS-specific implementations of <see cref="IStorageProviderCommand{T}"/> and <see cref="IDbCommandBuilder"/>.
  /// </summary>
  public interface IRdbmsProviderCommandExecutionContext
  {
    IDbCommand CreateDbCommand ();
    IDataReader ExecuteReader (IDbCommand command, CommandBehavior behavior);
    object? ExecuteScalar (IDbCommand command);
    int ExecuteNonQuery (IDbCommand command);
  }
}
