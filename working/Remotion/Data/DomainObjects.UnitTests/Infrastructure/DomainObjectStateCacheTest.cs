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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class DomainObjectStateCacheTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private DomainObjectStateCache _cachingListener;
    private Order _existingOrder;
    private Order _newOrder;
    private Order _notYetLoadedOrder;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction ();
      _cachingListener = new DomainObjectStateCache (_transaction);

      _existingOrder = (Order) LifetimeService.GetObject (_transaction, DomainObjectIDs.Order1, false);
      _newOrder = (Order) LifetimeService.NewObject (_transaction, typeof (Order), ParamList.Empty);
      _notYetLoadedOrder = (Order) LifetimeService.GetObjectReference (_transaction, DomainObjectIDs.Order3);
    }

    [Test]
    public void GetState_IsInvalid ()
    {
      LifetimeService.DeleteObject (_transaction, _newOrder);
      Assert.That (_cachingListener.GetState (_newOrder.ID), Is.EqualTo (StateType.Invalid));
    }

    [Test]
    public void GetState_NotYetLoaded ()
    {
      Assert.That (_cachingListener.GetState (_notYetLoadedOrder.ID), Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void GetState_FromDataContainer_New ()
    {
      Assert.That (_cachingListener.GetState (_newOrder.ID), Is.EqualTo (StateType.New));
    }

    [Test]
    public void GetState_FromDataContainer_Unchanged ()
    {
      Assert.That (_cachingListener.GetState (_existingOrder.ID), Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetState_FromDataContainer_Changed ()
    {
      _transaction.ExecuteInScope (() => _existingOrder.OrderNumber++);
      Assert.That (_cachingListener.GetState (_existingOrder.ID), Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_FromDataContainer_ChangedRelation ()
    {
      _transaction.ExecuteInScope (() => _existingOrder.OrderItems.Clear ());
      Assert.That (_cachingListener.GetState (_existingOrder.ID), Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_FromDataContainer_DoesNotLoadRelations ()
    {
      var dataManager = ClientTransactionTestHelper.GetIDataManager (_transaction);
      Assert.That (dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Resolve (_existingOrder, o => o.OrderTicket)), Is.Null);

      _transaction.ExecuteInScope (() => _existingOrder.OrderItems.Clear ());

      Assert.That (dataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Resolve (_existingOrder, o => o.OrderTicket)), Is.Null);
    }

    [Test]
    public void GetState_IDWithoutDomainObject ()
    {
      Assert.That (_transaction.GetEnlistedDomainObject (DomainObjectIDs.Order4), Is.Null);
      Assert.That (_cachingListener.GetState (DomainObjectIDs.Order4), Is.EqualTo (StateType.NotLoadedYet));
    }
    
    [Test]
    public void GetState_Twice ()
    {
      var existingState1 = _cachingListener.GetState (_existingOrder.ID);
      var existingState2 = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (existingState1, Is.EqualTo (StateType.Unchanged));
      Assert.That (existingState2, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetState_Invalidated_AfterPropertyChange ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      _transaction.ExecuteInScope (() => _existingOrder.OrderNumber++);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_Invalidated_AfterRealObjectEndPointChange ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      _transaction.ExecuteInScope (() => _existingOrder.Customer = null);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_Invalidated_AfterVirtualObjectEndPointChange ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      _transaction.ExecuteInScope (() => _existingOrder.OrderTicket = null);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_Invalidated_AfterCollectionEndPointChange ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      _transaction.ExecuteInScope (() => _existingOrder.OrderItems.RemoveAt (0));
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_Invalidated_AfterUnload ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      UnloadService.UnloadData (_transaction, _existingOrder.ID);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void GetState_Invalidated_AfterReload ()
    {
      UnloadService.UnloadData (_transaction, _existingOrder.ID);
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);
      
      _transaction.EnsureDataAvailable (_existingOrder.ID);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetState_Invalidated_AfterDelete ()
    {
      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      LifetimeService.DeleteObject (_transaction, _existingOrder);
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void GetState_Invalidated_AfterDiscard ()
    {
      var stateBeforeChange = _cachingListener.GetState (_newOrder.ID);

      LifetimeService.DeleteObject (_transaction, _newOrder);
      var stateAfterChange = _cachingListener.GetState (_newOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.New));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Invalid));
    }

    [Test]
    public void GetState_Invalidated_AfterRollback ()
    {
      _transaction.ExecuteInScope (() => _existingOrder.OrderNumber++);

      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      _transaction.Rollback ();
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Changed));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetState_Invalidated_AfterCommit ()
    {
      var subTransaction = _transaction.CreateSubTransaction ();
      subTransaction.EnsureDataAvailable (_existingOrder.ID);
      subTransaction.ExecuteInScope (() => _existingOrder.OrderNumber++);

      var cachingListener = new DomainObjectStateCache (subTransaction);
      var stateBeforeChange = cachingListener.GetState (_existingOrder.ID);

      subTransaction.Commit ();
      var stateAfterChange = cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Changed));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetState_Invalidated_AfterCommitOfSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction ();
      subTransaction.EnsureDataAvailable (_existingOrder.ID);
      subTransaction.ExecuteInScope (() => _existingOrder.OrderNumber++);

      var stateBeforeChange = _cachingListener.GetState (_existingOrder.ID);

      subTransaction.Commit ();
      var stateAfterChange = _cachingListener.GetState (_existingOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Unchanged));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void GetState_Invalidated_AfterMarkInvalid ()
    {
      var stateBeforeChange = _cachingListener.GetState (_notYetLoadedOrder.ID);

      _transaction.ExecuteInScope (() => DataManagementService.GetDataManager (_transaction).MarkInvalid (_notYetLoadedOrder));
      var stateAfterChange = _cachingListener.GetState (_notYetLoadedOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.Invalid));
    }

    [Test]
    public void GetState_Invalidated_AfterMarkNotInvalid ()
    {
      _transaction.ExecuteInScope (() => _newOrder.Delete());
      var stateBeforeChange = _cachingListener.GetState (_newOrder.ID);

      _transaction.ExecuteInScope (() => DataManagementService.GetDataManager (_transaction).MarkNotInvalid (_newOrder.ID));
      var stateAfterChange = _cachingListener.GetState (_newOrder.ID);

      Assert.That (stateBeforeChange, Is.EqualTo (StateType.Invalid));
      Assert.That (stateAfterChange, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void Serialization ()
    {
      var deserializedTuple = Serializer.SerializeAndDeserialize (Tuple.Create (_cachingListener, _transaction, _existingOrder));

      var deserializedCache = deserializedTuple.Item1;
      var deserializedTx = deserializedTuple.Item2;
      var deserializedDomainObject = deserializedTuple.Item3;

      Assert.That (deserializedCache.GetState (deserializedDomainObject.ID), Is.EqualTo (StateType.Unchanged));
      deserializedTx.ExecuteInScope (() => deserializedDomainObject.OrderNumber++);
      Assert.That (deserializedCache.GetState (deserializedDomainObject.ID), Is.EqualTo (StateType.Changed));
    }
  }
}