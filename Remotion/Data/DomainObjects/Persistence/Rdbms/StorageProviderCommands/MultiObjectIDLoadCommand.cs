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
  /// Executes the command created by the given <see cref="IDbCommandBuilder"/> and parses the result into a sequence of <see cref="ObjectID"/>
  /// instances.
  /// </summary>
  public class MultiObjectIDLoadCommand : IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<ObjectID?>>
  {
    private readonly IEnumerable<IDbCommandBuilder> _dbCommandBuilders;
    private readonly IObjectReader<ObjectID?> _objectIDReader;

    public MultiObjectIDLoadCommand (IEnumerable<IDbCommandBuilder> dbCommandBuilders, IObjectReader<ObjectID?> objectIDReader)
    {
      ArgumentUtility.CheckNotNull("dbCommandBuilders", dbCommandBuilders);
      ArgumentUtility.CheckNotNull("objectIDReader", objectIDReader);

      _dbCommandBuilders = dbCommandBuilders;
      _objectIDReader = objectIDReader;
    }

    public IEnumerable<IDbCommandBuilder> DbCommandBuilders
    {
      get { return _dbCommandBuilders; }
    }

    public IObjectReader<ObjectID?> ObjectIDReader
    {
      get { return _objectIDReader; }
    }

    public IEnumerable<ObjectID?> Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull("executionContext", executionContext);
      return _dbCommandBuilders.SelectMany(b => LoadObjectIDsFromCommandBuilder(b, executionContext));
    }

    private IEnumerable<ObjectID?> LoadObjectIDsFromCommandBuilder (
        IDbCommandBuilder commandBuilder, IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull("commandBuilder", commandBuilder);

      using (var command = commandBuilder.Create(executionContext))
      {
        using (var reader = executionContext.ExecuteReader(command, CommandBehavior.SingleResult))
        {
          // Use yield return to keep reader and command open while reading items
          foreach (var objectID in _objectIDReader.ReadSequence(reader))
            yield return objectID;
        }
      }
    }
  }
}
