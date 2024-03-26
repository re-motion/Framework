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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// Executes the command created by the given <see cref="IDbCommandBuilder"/> and parses the result into a sequence of objects using the specified
  /// <see cref="IObjectReader{T}"/>.
  /// </summary>
  public class MultiObjectLoadCommand<T> : IRdbmsProviderCommand<IEnumerable<T>>
  {
    private readonly Tuple<IDbCommandBuilder, IObjectReader<T>>[] _dbCommandBuildersAndReaders;

    public MultiObjectLoadCommand (IEnumerable<Tuple<IDbCommandBuilder, IObjectReader<T>>> dbCommandBuildersAndReaders)
    {
      ArgumentUtility.CheckNotNull("dbCommandBuildersAndReaders", dbCommandBuildersAndReaders);

      _dbCommandBuildersAndReaders = dbCommandBuildersAndReaders.ToArray();
    }

    public Tuple<IDbCommandBuilder, IObjectReader<T>>[] DbCommandBuildersAndReaders
    {
      get { return _dbCommandBuildersAndReaders; }
    }

    public IEnumerable<T> Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull("executionContext", executionContext);
      return _dbCommandBuildersAndReaders.SelectMany(b => LoadDataContainersFromCommandBuilder(b, executionContext));
    }

    private IEnumerable<T> LoadDataContainersFromCommandBuilder (
        Tuple<IDbCommandBuilder, IObjectReader<T>> commandBuilderTuple, IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull("commandBuilderTuple", commandBuilderTuple);

      using (var command = commandBuilderTuple.Item1.Create(executionContext))
      {
        using (var reader = executionContext.ExecuteReader(command, CommandBehavior.SingleResult))
        {
          // Use yield return to keep reader and command open while reading items
          foreach (var dataContainer in commandBuilderTuple.Item2.ReadSequence(reader))
            yield return dataContainer;
        }
      }
    }
  }
}
