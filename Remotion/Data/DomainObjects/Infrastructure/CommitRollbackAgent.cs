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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Performs the <see cref="DomainObjects.ClientTransaction.Commit"/> and <see cref="DomainObjects.ClientTransaction.Rollback"/> operations by 
  /// gathering the commit set from the  <see cref="IDataManager"/>, raising events via the <see cref="IClientTransactionEventSink"/>, and persisting 
  /// the commit set via the  <see cref="IPersistenceStrategy"/>.
  /// </summary>
  public class CommitRollbackAgent : ICommitRollbackAgent
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionEventSink _eventSink;
    private readonly IPersistenceStrategy _persistenceStrategy;
    private readonly IDataManager _dataManager;

    public CommitRollbackAgent (
        ClientTransaction clientTransaction, IClientTransactionEventSink eventSink, IPersistenceStrategy persistenceStrategy, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull("dataManager", dataManager);

      _clientTransaction = clientTransaction;
      _eventSink = eventSink;
      _persistenceStrategy = persistenceStrategy;
      _dataManager = dataManager;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IClientTransactionEventSink EventSink
    {
      get { return _eventSink; }
    }

    public IPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public IDataManager DataManager
    {
      get { return _dataManager; }
    }

    public bool HasData (Predicate<DomainObjectState> predicate)
    {
      ArgumentUtility.CheckNotNull("predicate", predicate);

      return _dataManager.GetLoadedDataByObjectState(predicate).Any();
    }

    public void CommitData ()
    {
      var persistableDataItems = BeginCommit();
      _eventSink.RaiseTransactionCommitValidateEvent(new ReadOnlyCollection<PersistableData>(persistableDataItems));

      _persistenceStrategy.PersistData(persistableDataItems);
      _dataManager.Commit();

      var changedButNotDeletedDomainObjects = persistableDataItems
          .Where(item => !item.DomainObjectState.IsDeleted)
          .Select(item => item.DomainObject)
          .ToList()
          .AsReadOnly();
      EndCommit(changedButNotDeletedDomainObjects);
    }

    public void RollbackData ()
    {
      var persistableDataItems = BeginRollback();

      _dataManager.Rollback();

      var changedButNotNewItems =
          persistableDataItems.Where(item => !item.DomainObjectState.IsNew)
              .Select(item => item.DomainObject)
              .ToList()
              .AsReadOnly();
      EndRollback(changedButNotNewItems);
    }

    private IList<PersistableData> BeginCommit ()
    {
      // Note regarding to Committing: 
      // Every object raises a Committing event even if another object's Committing event changes the first object's state back to original 
      // during its own Committing event. Because the event order of .NET is not deterministic, this behavior is desired to ensure consistency: 
      // Every object changed during a ClientTransaction raises a Committing event regardless of the Committing event order of specific objects.  

      // Note regarding to Committed: 
      // If an object is changed back to its original state during the Committing phase, no Committed event will be raised,
      // because in this case the object won't be committed to the underlying backend (e.g. database).

      var committingEventNotRaised = GetNewChangedDeletedData().ToList();
      var committingEventRaised = new HashSet<ObjectID>();

      // Repeat this until all objects in the commit set have got the event. The commit set can change while this loop is iterated.
      while (true)
      {
        var eventArgReadOnlyCollection = ListAdapter.AdaptReadOnly(committingEventNotRaised, item => item.DomainObject);
        var committingEventRegistrar = new CommittingEventRegistrar(_clientTransaction);
        _eventSink.RaiseTransactionCommittingEvent(eventArgReadOnlyCollection, committingEventRegistrar);

        // Remember which objects have got the event right now.
        committingEventRaised.UnionWith(committingEventNotRaised.Select(item => item.DomainObject.ID));

        // Remove objects registered for repeated Committing events so that they'll get the event again.
        committingEventRaised.ExceptWith(committingEventRegistrar.RegisteredObjects.Select(obj => obj.ID));

        // Reevaluate the commit set - it might have changed. Have all objects in it got the event? If yes, return the commit set.
        var changedItems = GetNewChangedDeletedData().ToList();
        committingEventNotRaised = changedItems.Where(item => !committingEventRaised.Contains(item.DomainObject.ID)).ToList();

        if (!committingEventNotRaised.Any())
          return changedItems;
      }
    }

    private void EndCommit (ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
      _eventSink.RaiseTransactionCommittedEvent(changedDomainObjects);
    }

    private IList<PersistableData> BeginRollback ()
    {
      // Note regarding to RollingBack: 
      // Every object raises a RollingBack event even if another object's RollingBack event changes the first object's state back to original 
      // during its own RollingBack event. Because the event order of .NET is not deterministic, this behavior is desired to ensure consistency: 
      // Every object changed during a ClientTransaction raises a RollingBack event regardless of the RollingBack event order of specific objects.  

      // Note regarding to RolledBack: 
      // If an object is changed back to its original state during the RollingBack phase, no RolledBack event will be raised,
      // because the object actually has never been changed from a ClientTransaction's perspective.

      var rollingBackEventNotRaised = GetNewChangedDeletedData().ToList();
      var rollingBackEventRaised = new HashSet<ObjectID>();

      // Repeat this until all objects in the rollback set have got the event. The rollback set can change while this loop is iterated.
      while (true)
      {
        var eventArgReadOnlyCollection = ListAdapter.AdaptReadOnly(rollingBackEventNotRaised, item => item.DomainObject);
        _eventSink.RaiseTransactionRollingBackEvent(eventArgReadOnlyCollection);

        // Remember which objects have got the event right now.
        rollingBackEventRaised.UnionWith(rollingBackEventNotRaised.Select(item => item.DomainObject.ID));

        // Reevaluate the rollback set - it might have changed. Have all objects in it got the event? If yes, return the rollback set.
        var changedItems = GetNewChangedDeletedData().ToList();
        rollingBackEventNotRaised = changedItems.Where(item => !rollingBackEventRaised.Contains(item.DomainObject.ID)).ToList();

        if (!rollingBackEventNotRaised.Any())
          return changedItems;
      }
    }

    private void EndRollback (ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
      _eventSink.RaiseTransactionRolledBackEvent(changedDomainObjects);
    }

    private IEnumerable<PersistableData> GetNewChangedDeletedData ()
    {
      // Place tests in order of probability to reduce number of checks required until a match for a typical usage scenario
      return _dataManager.GetLoadedDataByObjectState(state => state.IsChanged || state.IsNew || state.IsDeleted);
    }

  }
}
