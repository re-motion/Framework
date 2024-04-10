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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// Executes the command created by the given <see cref="IDbCommandBuilder"/> and parses the result into a single object using the specified
  /// <see cref="IObjectReader{T}"/>.
  /// </summary>
  public class ScalarValueLoadCommand : IRdbmsProviderCommandWithReadOnlySupport<object?>
  {
    private readonly IDbCommandBuilder _dbCommandBuilder;

    public ScalarValueLoadCommand (IDbCommandBuilder dbCommandBuilder)
    {
      ArgumentUtility.CheckNotNull("dbCommandBuilder", dbCommandBuilder);

      _dbCommandBuilder = dbCommandBuilder;
    }

    public IDbCommandBuilder DbCommandBuilder
    {
      get { return _dbCommandBuilder; }
    }

    public object? Execute (IRdbmsProviderReadWriteCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull("executionContext", executionContext);
      return Execute<IRdbmsProviderReadWriteCommandExecutionContext>(executionContext);
    }

    public object? Execute (IRdbmsProviderReadOnlyCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull("executionContext", executionContext);
      return Execute<IRdbmsProviderReadOnlyCommandExecutionContext>(executionContext);
    }

    private object? Execute<TExecutionContext> (TExecutionContext executionContext)
        where TExecutionContext : IDbCommandFactory, IScalarCommandExecutionContext
    {
      using (var command = _dbCommandBuilder.Create(executionContext))
      {
        return executionContext.ExecuteScalar(command);
      }
    }
  }
}
