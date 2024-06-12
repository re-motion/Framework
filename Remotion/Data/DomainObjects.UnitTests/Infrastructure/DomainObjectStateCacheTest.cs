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
using Remotion.Development.NUnit.UnitTesting;
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
    private OrderTicket _existingOrderTicket;
    private OrderTicket _newOrderTicket;
    private OrderViewModel _newOrderViewModel;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();
      _cachingListener = new DomainObjectStateCache(_transaction);

      _existingOrder = (Order)LifetimeService.GetObject(_transaction, DomainObjectIDs.Order1, false);
      _newOrder = (Order)LifetimeService.NewObject(_transaction, typeof(Order), ParamList.Empty);
      _notYetLoadedOrder = (Order)LifetimeService.GetObjectReference(_transaction, DomainObjectIDs.Order3);
      _existingOrderTicket = (OrderTicket)LifetimeService.GetObject(_transaction, DomainObjectIDs.OrderTicket3, false);
      _newOrderTicket = (OrderTicket)LifetimeService.NewObject(_transaction, typeof(OrderTicket), ParamList.Empty);
      _newOrderViewModel = (OrderViewModel)LifetimeService.NewObject(_transaction, typeof(OrderViewModel), ParamList.Empty);
    }

    [Test]
    public void GetState_IsInvalid ()
    {
      LifetimeService.DeleteObject(_transaction, _newOrder);

      var domainObjectState = _cachingListener.GetState(_newOrder.ID);
      Assert.That(domainObjectState.IsInvalid, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(1));
    }

    [Test]
    public void GetState_NotYetLoaded ()
    {
      var domainObjectState = _cachingListener.GetState(_notYetLoadedOrder.ID);
      Assert.That(domainObjectState.IsNotLoadedYet, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(1));
    }

    [Test]
    public void GetState_NotLoadedYet_ChangedRelation ()
    {
      _transaction.ExecuteInScope(() => _existingOrder.OrderItems.Clear());
      UnloadService.UnloadData(_transaction, _existingOrder.ID);
      var notYetLoadedOrder = _existingOrder;

      var domainObjectState = _cachingListener.GetState(notYetLoadedOrder.ID);
      Assert.That(domainObjectState.IsNotLoadedYet, Is.True);
      Assert.That(domainObjectState.IsRelationChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(2));
    }

    [Test]
    public void GetState_FromDataContainer_New ()
    {
      var domainObjectState = _cachingListener.GetState(_newOrder.ID);
      Assert.That(domainObjectState.IsNew, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(2));
    }

    [Test]
    public void GetState_FromNonPersistentDataContainer_New ()
    {
      var domainObjectState = _cachingListener.GetState(_newOrderViewModel.ID);
      Assert.That(domainObjectState.IsNew, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(2));
    }

    [Test]
    public void GetState_FromDataContainerWithPersistentProperty_New_PersistentDataChanged ()
    {
      _transaction.ExecuteInScope(() => _newOrder.OrderNumber++);

      var domainObjectState = _cachingListener.GetState(_newOrder.ID);
      Assert.That(domainObjectState.IsNew, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
      Assert.That(domainObjectState.IsDataChanged, Is.True);
      Assert.That(domainObjectState.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(4));
    }

    [Test]
    public void GetState_FromDataContainerWithNonPersistentProperty_New_NonPersistentDataChanged ()
    {
      _transaction.ExecuteInScope(() => _newOrderTicket.Int32TransactionProperty++);

      var domainObjectState = _cachingListener.GetState(_newOrderTicket.ID);
      Assert.That(domainObjectState.IsNew, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
      Assert.That(domainObjectState.IsDataChanged, Is.True);
      Assert.That(domainObjectState.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(4));
    }

    [Test]
    public void GetState_FromDataContainer_Unchanged ()
    {
      var domainObjectState = _cachingListener.GetState(_existingOrder.ID);
      Assert.That(domainObjectState.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(1));
    }

    [Test]
    public void GetState_FromDataContainerWithPersistentProperty_Changed_PersistentDataChanged ()
    {
      _transaction.ExecuteInScope(() => _existingOrder.OrderNumber++);

      var domainObjectState = _cachingListener.GetState(_existingOrder.ID);
      Assert.That(domainObjectState.IsChanged, Is.True);
      Assert.That(domainObjectState.IsDataChanged, Is.True);
      Assert.That(domainObjectState.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(3));
    }

    [Test]
    public void GetState_FromDataContainerWithNonPersistentProperty_Changed_NonPersistentDataChanged ()
    {
      _transaction.ExecuteInScope(() => _existingOrderTicket.Int32TransactionProperty++);

      var domainObjectState = _cachingListener.GetState(_existingOrderTicket.ID);
      Assert.That(domainObjectState.IsChanged, Is.True);
      Assert.That(domainObjectState.IsDataChanged, Is.True);
      Assert.That(domainObjectState.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(3));
    }

    [Test]
    public void GetState_FromNonPersistentDataContainer_New_NonPersistentDataChanged ()
    {
      _transaction.ExecuteInScope(() => _newOrderViewModel.OrderSum++);

      var domainObjectState = _cachingListener.GetState(_newOrderViewModel.ID);
      Assert.That(domainObjectState.IsNew, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
      Assert.That(domainObjectState.IsDataChanged, Is.True);
      Assert.That(domainObjectState.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(4));
    }

    [Test]
    public void GetState_FromDataContainer_DoesNotLoadRelations ()
    {
      var dataManager = ClientTransactionTestHelper.GetIDataManager(_transaction);
      Assert.That(dataManager.GetRelationEndPointWithoutLoading(RelationEndPointID.Resolve(_existingOrder, o => o.OrderTicket)), Is.Null);

      _transaction.ExecuteInScope(() => _existingOrder.OrderItems.Clear());

      Assert.That(dataManager.GetRelationEndPointWithoutLoading(RelationEndPointID.Resolve(_existingOrder, o => o.OrderTicket)), Is.Null);
    }

    [Test]
    public void GetState_FromDataContainer_ChangedRelation ()
    {
      _transaction.ExecuteInScope(() => _existingOrder.OrderItems.Clear());

      var domainObjectState = _cachingListener.GetState(_existingOrder.ID);
      Assert.That(domainObjectState.IsChanged, Is.True);
      Assert.That(domainObjectState.IsRelationChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(2));
    }

    [Test]
    public void GetState_FromDataContainer_New_IsNewInHierarchy ()
    {
      var domainObjectState = _cachingListener.GetState(_newOrder.ID);
      Assert.That(domainObjectState.IsNew, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(2));
    }

    [Test]
    public void GetState_FromDataContainer_Unchanged_IsNewInHierarchy ()
    {
      _existingOrder.GetInternalDataContainerForTransaction(_transaction).SetNewInHierarchy();

      var domainObjectState = _cachingListener.GetState(_existingOrder.ID);
      Assert.That(domainObjectState.IsUnchanged, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void GetState_FromDataContainer_Changed_IsNewInHierarchy ()
    {
      _existingOrder.GetInternalDataContainerForTransaction(_transaction).SetNewInHierarchy();
      _existingOrder.DeliveryDate = DateTime.Now;

      var domainObjectState = _cachingListener.GetState(_existingOrder.ID);
      Assert.That(domainObjectState.IsChanged, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void GetState_FromDataContainer_Deleted_IsNewInHierarchy ()
    {
      _existingOrder.GetInternalDataContainerForTransaction(_transaction).SetNewInHierarchy();
      _existingOrder.Delete();

      var domainObjectState = _cachingListener.GetState(_existingOrder.ID);
      Assert.That(domainObjectState.IsDeleted, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void GetState_FromDataContainer_ChangedRelation_IsNewInHierarchy ()
    {
      _existingOrder.GetInternalDataContainerForTransaction(_transaction).SetNewInHierarchy();
      _transaction.ExecuteInScope(() => _existingOrder.OrderItems.Clear());

      var domainObjectState = _cachingListener.GetState(_existingOrder.ID);
      Assert.That(domainObjectState.IsDataChanged, Is.False);
      Assert.That(domainObjectState.IsRelationChanged, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void GetState_FromDataContainer_NotLoadedYetInSubTransaction_NewInRootTransaction_IsNewInHierarchy ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      var subTransactionStateCache = new DomainObjectStateCache(subTransaction);

      var domainObjectState = subTransactionStateCache.GetState(_newOrder.ID);
      Assert.That(domainObjectState.IsNotLoadedYet, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(2));
    }

    [Test]
    public void GetState_FromDataContainer_NotLoadedYetInSubSubTransaction_NewInParentTransaction_IsNewInHierarchy ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      var newOrder = (Order)LifetimeService.NewObject(subTransaction, typeof(Order), ParamList.Empty);

      var subSubTransaction = subTransaction.CreateSubTransaction();
      var subSubTransactionStateCache = new DomainObjectStateCache(subSubTransaction);

      var domainObjectState = subSubTransactionStateCache.GetState(newOrder.ID);
      Assert.That(domainObjectState.IsNotLoadedYet, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(2));
    }

    [Test]
    public void GetState_FromDataContainer_NotLoadedYetInSubSubSubTransaction_NewInParentTransaction_IsNewInHierarchy ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      var newOrder = (Order)LifetimeService.NewObject(subTransaction, typeof(Order), ParamList.Empty);

      var subSubSubTransaction = subTransaction.CreateSubTransaction().CreateSubTransaction();
      var subSubSubTransactionStateCache = new DomainObjectStateCache(subSubSubTransaction);

      var domainObjectState = subSubSubTransactionStateCache.GetState(newOrder.ID);
      Assert.That(domainObjectState.IsNotLoadedYet, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(2));
    }

    [Test]
    public void GetState_FromDataContainer_NotLoadedYetInSubTransaction_UnchangedInRootTransaction_IsNewInHierarchy ()
    {
      var subSubTransaction = _transaction.CreateSubTransaction().CreateSubTransaction();
      var subSubTransactionStateCache = new DomainObjectStateCache(subSubTransaction);

      var domainObjectState = subSubTransactionStateCache.GetState(_existingOrder.ID);
      Assert.That(domainObjectState.IsNotLoadedYet, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.False);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(1));
    }

    [Test]
    public void GetState_FromDataContainer_NotLoadedYetInSubTransaction_NotLoadedInRootTransaction_IsNewInHierarchy ()
    {
      var subSubTransaction = _transaction.CreateSubTransaction().CreateSubTransaction();
      var subSubTransactionStateCache = new DomainObjectStateCache(subSubTransaction);

      var domainObjectState = subSubTransactionStateCache.GetState(_notYetLoadedOrder.ID);
      Assert.That(domainObjectState.IsNotLoadedYet, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.False);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(1));
    }

    [Test]
    public void GetState_FromDataContainer_NotLoadedYetInRootTransaction_IsNewInHierarchy ()
    {
      var transactionStateCache = new DomainObjectStateCache(_transaction);

      var domainObjectState = transactionStateCache.GetState(_notYetLoadedOrder.ID);
      Assert.That(domainObjectState.IsNotLoadedYet, Is.True);
      Assert.That(domainObjectState.IsNewInHierarchy, Is.False);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(1));
    }

    [Test]
    public void GetState_IDWithoutDomainObject ()
    {
      Assert.That(_transaction.GetEnlistedDomainObject(DomainObjectIDs.Order4), Is.Null);

      var domainObjectState = _cachingListener.GetState(DomainObjectIDs.Order4);
      Assert.That(domainObjectState.IsNotLoadedYet, Is.True);
      Assert.That(GetNumberOfSetFlags(domainObjectState), Is.EqualTo(1));
    }

    [Test]
    public void GetState_Twice ()
    {
      var existingState1 = _cachingListener.GetState(_existingOrder.ID);
      var existingState2 = _cachingListener.GetState(_existingOrder.ID);

      Assert.That(existingState1.IsUnchanged, Is.True);
      Assert.That(existingState2.IsUnchanged, Is.True);
    }

    [Test]
    public void GetState_Invalidated_AfterPropertyChange ()
    {
      var stateBeforeChange = _cachingListener.GetState(_existingOrder.ID);

      _transaction.ExecuteInScope(() => _existingOrder.OrderNumber++);
      var stateAfterChange = _cachingListener.GetState(_existingOrder.ID);

      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));

      Assert.That(stateAfterChange.IsChanged, Is.True);
      Assert.That(stateAfterChange.IsDataChanged, Is.True);
      Assert.That(stateAfterChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(3));
    }

    [Test]
    public void GetState_Invalidated_AfterRealObjectEndPointChange ()
    {
      var stateBeforeChange = _cachingListener.GetState(_existingOrder.ID);

      _transaction.ExecuteInScope(() => _existingOrder.Customer = null);
      var stateAfterChange = _cachingListener.GetState(_existingOrder.ID);

      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));

      Assert.That(stateAfterChange.IsChanged, Is.True);
      Assert.That(stateAfterChange.IsDataChanged, Is.True);
      Assert.That(stateAfterChange.IsPersistentDataChanged, Is.True);
      Assert.That(stateAfterChange.IsRelationChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(4));
    }

    [Test]
    public void GetState_Invalidated_AfterVirtualObjectEndPointChange ()
    {
      var stateBeforeChange = _cachingListener.GetState(_existingOrder.ID);

      _transaction.ExecuteInScope(() => _existingOrder.OrderTicket = null);
      var stateAfterChange = _cachingListener.GetState(_existingOrder.ID);

      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));

      Assert.That(stateAfterChange.IsChanged, Is.True);
      Assert.That(stateAfterChange.IsRelationChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(2));
    }

    [Test]
    public void GetState_Invalidated_AfterCollectionEndPointChange ()
    {
      var stateBeforeChange = _cachingListener.GetState(_existingOrder.ID);

      _transaction.ExecuteInScope(() => _existingOrder.OrderItems.RemoveAt(0));
      var stateAfterChange = _cachingListener.GetState(_existingOrder.ID);

      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));

      Assert.That(stateAfterChange.IsChanged, Is.True);
      Assert.That(stateAfterChange.IsRelationChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(2));
    }

    [Test]
    public void GetState_Invalidated_AfterUnload ()
    {
      var stateBeforeChange = _cachingListener.GetState(_existingOrder.ID);

      UnloadService.UnloadData(_transaction, _existingOrder.ID);
      var stateAfterChange = _cachingListener.GetState(_existingOrder.ID);

      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));

      Assert.That(stateAfterChange.IsNotLoadedYet, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
    }

    [Test]
    public void GetState_Invalidated_AfterReload ()
    {
      UnloadService.UnloadData(_transaction, _existingOrder.ID);
      var stateBeforeChange = _cachingListener.GetState(_existingOrder.ID);

      _transaction.EnsureDataAvailable(_existingOrder.ID);
      var stateAfterChange = _cachingListener.GetState(_existingOrder.ID);

      Assert.That(stateBeforeChange.IsNotLoadedYet, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));

      Assert.That(stateAfterChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
    }

    [Test]
    public void GetState_Invalidated_AfterDelete ()
    {
      var stateBeforeChange = _cachingListener.GetState(_existingOrder.ID);

      LifetimeService.DeleteObject(_transaction, _existingOrder);
      var stateAfterChange = _cachingListener.GetState(_existingOrder.ID);

      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));

      Assert.That(stateAfterChange.IsDeleted, Is.True);
      Assert.That(stateAfterChange.IsDataChanged, Is.True);
      Assert.That(stateAfterChange.IsPersistentDataChanged, Is.True);
      Assert.That(stateAfterChange.IsRelationChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(4));
    }

    [Test]
    public void GetState_Invalidated_AfterDiscard ()
    {
      var stateBeforeChange = _cachingListener.GetState(_newOrder.ID);

      LifetimeService.DeleteObject(_transaction, _newOrder);
      var stateAfterChange = _cachingListener.GetState(_newOrder.ID);

      Assert.That(stateBeforeChange.IsNew, Is.True);
      Assert.That(stateBeforeChange.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(2));

      Assert.That(stateAfterChange.IsInvalid, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
    }

    [Test]
    public void GetState_Invalidated_AfterRollback ()
    {
      _transaction.ExecuteInScope(() => _existingOrder.OrderNumber++);

      var stateBeforeChange = _cachingListener.GetState(_existingOrder.ID);

      _transaction.Rollback();
      var stateAfterChange = _cachingListener.GetState(_existingOrder.ID);

      Assert.That(stateBeforeChange.IsChanged, Is.True);
      Assert.That(stateBeforeChange.IsDataChanged, Is.True);
      Assert.That(stateBeforeChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(3));

      Assert.That(stateAfterChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
    }

    [Test]
    public void GetState_Invalidated_AfterCommit ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      subTransaction.EnsureDataAvailable(_existingOrder.ID);
      subTransaction.ExecuteInScope(() => _existingOrder.OrderNumber++);

      var cachingListener = new DomainObjectStateCache(subTransaction);
      var stateBeforeChange = cachingListener.GetState(_existingOrder.ID);

      subTransaction.Commit();
      var stateAfterChange = cachingListener.GetState(_existingOrder.ID);

      Assert.That(stateBeforeChange.IsChanged, Is.True);
      Assert.That(stateBeforeChange.IsDataChanged, Is.True);
      Assert.That(stateBeforeChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(3));

      Assert.That(stateAfterChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
    }

    [Test]
    public void GetState_Invalidated_AfterCommitOfSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      subTransaction.EnsureDataAvailable(_existingOrder.ID);
      subTransaction.ExecuteInScope(() => _existingOrder.OrderNumber++);

      var stateBeforeChange = _cachingListener.GetState(_existingOrder.ID);

      subTransaction.Commit();
      var stateAfterChange = _cachingListener.GetState(_existingOrder.ID);

      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));

      Assert.That(stateAfterChange.IsChanged, Is.True);
      Assert.That(stateAfterChange.IsDataChanged, Is.True);
      Assert.That(stateAfterChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(3));
    }

    [Test]
    public void GetState_Invalidated_AfterMarkInvalid ()
    {
      var stateBeforeChange = _cachingListener.GetState(_notYetLoadedOrder.ID);

      _transaction.ExecuteInScope(() => DataManagementService.GetDataManager(_transaction).MarkInvalid(_notYetLoadedOrder));
      var stateAfterChange = _cachingListener.GetState(_notYetLoadedOrder.ID);

      Assert.That(stateBeforeChange.IsNotLoadedYet, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));

      Assert.That(stateAfterChange.IsInvalid, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
    }

    [Test]
    public void GetState_Invalidated_AfterMarkNotInvalid ()
    {
      _transaction.ExecuteInScope(() => _newOrder.Delete());
      var stateBeforeChange = _cachingListener.GetState(_newOrder.ID);

      _transaction.ExecuteInScope(() => DataManagementService.GetDataManager(_transaction).MarkNotInvalid(_newOrder.ID));
      var stateAfterChange = _cachingListener.GetState(_newOrder.ID);

      Assert.That(stateBeforeChange.IsInvalid, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));

      Assert.That(stateAfterChange.IsNotLoadedYet, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
    }

    [Test]
    public void GetState_DataContainerDiscardedWhileRegisteredWithDataManager_ThrowsInvalidOperationException ()
    {
      _transaction.ExecuteInScope(() => DataManagementService.GetDataManager(_transaction).DataContainers[_newOrder.ID].Discard());
      Assert.That(
          () => _cachingListener.GetState(_newOrder.ID),
          Throws.InvalidOperationException.With.Message.EqualTo(
              $"DataContainer for object '{_newOrder.ID}' has been discarded without removing the instance from the DataManager."));
    }

    private int GetNumberOfSetFlags (DomainObjectState domainObjectState)
    {
      int count = 0;
      if (domainObjectState.IsNew)
        count++;
      if (domainObjectState.IsChanged)
        count++;
      if (domainObjectState.IsDeleted)
        count++;
      if (domainObjectState.IsInvalid)
        count++;
      if (domainObjectState.IsNotLoadedYet)
        count++;
      if (domainObjectState.IsUnchanged)
        count++;
      if (domainObjectState.IsDataChanged)
        count++;
      if (domainObjectState.IsPersistentDataChanged)
        count++;
      if (domainObjectState.IsNonPersistentDataChanged)
        count++;
      if (domainObjectState.IsRelationChanged)
        count++;
      if (domainObjectState.IsNewInHierarchy)
        count++;

      return count;
    }
  }
}
