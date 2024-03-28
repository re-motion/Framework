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
  /// Executes the given <see cref="IRdbmsProviderCommandWithReadOnlySupport{T}"/> to retrieve a sequence of <see cref="ObjectID"/> values, then
  /// looks up those values via the given <see cref="IRdbmsProviderCommandFactory"/>. This command can be used to indirectly
  /// load <see cref="DataContainer"/> instances via two queries, where the first yields only IDs, for example for concrete table inheritance 
  /// relation lookup.
  /// </summary>
  public class IndirectDataContainerLoadCommand : IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<ObjectLookupResult<DataContainer>>>
  {
    private readonly IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<ObjectID>> _objectIDLoadCommand;
    private readonly IRdbmsProviderCommandFactory _rdbmsProviderCommandFactory;

    public IndirectDataContainerLoadCommand (
        IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<ObjectID>> objectIDLoadCommand,
        IRdbmsProviderCommandFactory rdbmsProviderCommandFactory)
    {
      ArgumentUtility.CheckNotNull("objectIDLoadCommand", objectIDLoadCommand);
      ArgumentUtility.CheckNotNull("rdbmsProviderCommandFactory", rdbmsProviderCommandFactory);

      _objectIDLoadCommand = objectIDLoadCommand;
      _rdbmsProviderCommandFactory = rdbmsProviderCommandFactory;
    }

    public IRdbmsProviderCommand<IEnumerable<ObjectID>> ObjectIDLoadCommand
    {
      get { return _objectIDLoadCommand; }
    }

    public IRdbmsProviderCommandFactory RdbmsProviderCommandFactory
    {
      get { return _rdbmsProviderCommandFactory; }
    }

    public IEnumerable<ObjectLookupResult<DataContainer>> Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull("executionContext", executionContext);

      var objectIds = _objectIDLoadCommand.Execute(executionContext);
      return _rdbmsProviderCommandFactory.CreateForSortedMultiIDLookup(objectIds.ToArray()).Execute(executionContext);
    }
  }
}
