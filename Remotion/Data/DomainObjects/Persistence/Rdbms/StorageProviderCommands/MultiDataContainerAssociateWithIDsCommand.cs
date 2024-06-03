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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// Executes a given <see cref="IRdbmsProviderCommandWithReadOnlySupport{T}"/> and associates the resulting <see cref="DataContainer"/> instances
  /// with a given list of <see cref="ObjectID"/> values. If any <see cref="DataContainer"/> has a non-matching <see cref="ObjectID"/>, an exception
  /// is thrown.
  /// </summary>
  public class MultiDataContainerAssociateWithIDsCommand : IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<ObjectLookupResult<DataContainer>>>
  {
    private readonly ObjectID[] _objectIDs;
    private readonly IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<DataContainer?>> _command;

    public MultiDataContainerAssociateWithIDsCommand (
        IEnumerable<ObjectID> objectIDs,
        IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<DataContainer?>> command)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);
      ArgumentUtility.CheckNotNull("command", command);

      _objectIDs = objectIDs.Select(
          (objectID, index) =>
          {
            if (objectID == null)
              throw new ArgumentNullException($"objectIDs[{index}]");
            return objectID;
          }).ToArray();

      _command = command;
    }

    public ObjectID[] ObjectIDs
    {
      get { return _objectIDs; }
    }

    public IRdbmsProviderCommand<IEnumerable<DataContainer?>> Command
    {
      get { return _command; }
    }

    public IEnumerable<ObjectLookupResult<DataContainer>> Execute (IRdbmsProviderReadWriteCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull("executionContext", executionContext);

      var dataContainers = _command.Execute(executionContext);
      return ProcessDataContainers(dataContainers);
    }

    public IEnumerable<ObjectLookupResult<DataContainer>> Execute (IRdbmsProviderReadOnlyCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull("executionContext", executionContext);

      var dataContainers = _command.Execute(executionContext);
      return ProcessDataContainers(dataContainers);
    }

    private IEnumerable<ObjectLookupResult<DataContainer>> ProcessDataContainers (IEnumerable<DataContainer?> dataContainers)
    {
      var dataContainersByID =  new Dictionary<ObjectID, DataContainer>();
      foreach (var dataContainer in dataContainers.Where(dc => dc != null))
      {
        // Duplicates overwrite the previous DataContainer
        dataContainersByID[dataContainer!.ID] = dataContainer;
      }

      var unassociatedDataContainerIDs = new HashSet<ObjectID>(dataContainersByID.Keys);
      foreach (var objectID in _objectIDs)
      {
        var dataContainer = dataContainersByID.GetValueOrDefault(objectID);
        unassociatedDataContainerIDs.Remove(objectID);
        yield return new ObjectLookupResult<DataContainer>(objectID, dataContainer);
      }

      if (unassociatedDataContainerIDs.Count > 0)
      {
        var objectIDsByValue = _objectIDs.ToLookup(id => id.Value);
        var nonMatchingIDDescriptions = unassociatedDataContainerIDs
            .Select(loadedID => new { LoadedID = loadedID, ExpectedIDs = objectIDsByValue[loadedID.Value] })
            .Select(
                t => string.Format(
                    "Loaded DataContainer ID: {0}, expected ObjectID(s): {1}",
                    t.LoadedID,
                    t.ExpectedIDs.Any() ? string.Join(", ", t.ExpectedIDs.Distinct()) : "none"));
        var message = string.Format(
            "The ObjectID of one or more loaded DataContainers does not match the expected ObjectIDs:{0}{1}",
            Environment.NewLine,
            string.Join(Environment.NewLine, nonMatchingIDDescriptions));
        throw new PersistenceException(message);
      }
    }
  }
}
