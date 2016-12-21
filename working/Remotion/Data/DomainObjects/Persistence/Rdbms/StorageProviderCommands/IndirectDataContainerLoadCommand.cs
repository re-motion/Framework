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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// Executes the given <see cref="IStorageProviderCommand{T, TExecutionContext}"/> to retrieve a sequence of <see cref="ObjectID"/> values, then
  /// looks up those values via the given <see cref="IStorageProviderCommandFactory{TExecutionContext}"/>. This command can be used to indirectly
  /// load <see cref="DataContainer"/> instances via two queries, where the first yields only IDs, for example for concrete table inheritance 
  /// relation lookup.
  /// </summary>
  public class IndirectDataContainerLoadCommand
      : IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IRdbmsProviderCommandExecutionContext>
  {
    private readonly IStorageProviderCommand<IEnumerable<ObjectID>, IRdbmsProviderCommandExecutionContext> _objectIDLoadCommand;
    private readonly IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderCommandFactory;

    public IndirectDataContainerLoadCommand (
        IStorageProviderCommand<IEnumerable<ObjectID>, IRdbmsProviderCommandExecutionContext> objectIDLoadCommand,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> storageProviderCommandFactory)
    {
      ArgumentUtility.CheckNotNull ("objectIDLoadCommand", objectIDLoadCommand);
      ArgumentUtility.CheckNotNull ("storageProviderCommandFactory", storageProviderCommandFactory);

      _objectIDLoadCommand = objectIDLoadCommand;
      _storageProviderCommandFactory = storageProviderCommandFactory;
    }

    public IStorageProviderCommand<IEnumerable<ObjectID>, IRdbmsProviderCommandExecutionContext> ObjectIDLoadCommand
    {
      get { return _objectIDLoadCommand; }
    }

    public IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> StorageProviderCommandFactory
    {
      get { return _storageProviderCommandFactory; }
    }

    public IEnumerable<ObjectLookupResult<DataContainer>> Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      var objectIds = _objectIDLoadCommand.Execute (executionContext);
      return _storageProviderCommandFactory.CreateForSortedMultiIDLookup (objectIds.ToArray()).Execute (executionContext);
    }
  }
}