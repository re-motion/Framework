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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  public class UnloadAllCommand : IDataManagementCommand
  {
    private readonly IRelationEndPointManager _relationEndPointManager;
    private readonly DataContainerMap _dataContainerMap;
    private readonly IInvalidDomainObjectManager _invalidDomainObjectManager;
    private readonly IClientTransactionEventSink _transactionEventSink;

    private List<DataContainer> _unloadedDataContainers = new List<DataContainer>();

    public UnloadAllCommand (
        IRelationEndPointManager relationEndPointManager,
        DataContainerMap dataContainerMap,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("relationEndPointManager", relationEndPointManager);
      ArgumentUtility.CheckNotNull("dataContainerMap", dataContainerMap);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);

      _relationEndPointManager = relationEndPointManager;
      _dataContainerMap = dataContainerMap;
      _invalidDomainObjectManager = invalidDomainObjectManager;
      _transactionEventSink = transactionEventSink;
    }

    public IRelationEndPointManager RelationEndPointManager
    {
      get { return _relationEndPointManager; }
    }

    public DataContainerMap DataContainerMap
    {
      get { return _dataContainerMap; }
    }

    public IInvalidDomainObjectManager InvalidDomainObjectManager
    {
      get { return _invalidDomainObjectManager; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return Enumerable.Empty<Exception>();
    }

    public void Begin ()
    {
      RaiseRecurringBeginEvent(domainObjects => _transactionEventSink.RaiseObjectsUnloadingEvent(domainObjects));
    }

    public void Perform ()
    {
      // Reset end-point manager before resetting the DataContainers so that the VirtualEndPointUnregistering events come before the
      // DataManagerUnregistering events (although that shouldn't make any difference to users and is definitely an implementation detail)
      _relationEndPointManager.Reset();
      _unloadedDataContainers = _dataContainerMap.ToList();
      foreach (var dataContainer in _unloadedDataContainers)
      {
        _dataContainerMap.Remove(dataContainer.ID);

        var dataContainerState = dataContainer.State;
        dataContainer.Discard();
        if (dataContainerState.IsNew)
          _invalidDomainObjectManager.MarkInvalid(dataContainer.DomainObject);
      }

      Assertion.IsTrue(_dataContainerMap.Count == 0);
    }

    public void End ()
    {
      if (_unloadedDataContainers.Count > 0)
        _transactionEventSink.RaiseObjectsUnloadedEvent(_unloadedDataContainers.Select(dc => dc.DomainObject).ToList().AsReadOnly());
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand(this);
    }

    private void RaiseRecurringBeginEvent (Action<ReadOnlyCollection<IDomainObject>> eventAction)
    {
      var loadedObjectsEventRaised = new HashSet<IDomainObject>();
      ReadOnlyCollection<IDomainObject> loadedObjectsEventNotRaised;
      while ((loadedObjectsEventNotRaised = GetLoadedObjectsEventNotRaised(loadedObjectsEventRaised)).Count > 0)
      {
        eventAction(loadedObjectsEventNotRaised);
        loadedObjectsEventRaised.UnionWith(loadedObjectsEventNotRaised);
      }
    }

    private ReadOnlyCollection<IDomainObject> GetLoadedObjectsEventNotRaised (HashSet<IDomainObject> loadedObjectsEventRaised)
    {
      return _dataContainerMap
          .Select(dc => dc.DomainObject)
          .Where(obj => !loadedObjectsEventRaised.Contains(obj))
          .ToList()
          .AsReadOnly();
    }
  }
}
