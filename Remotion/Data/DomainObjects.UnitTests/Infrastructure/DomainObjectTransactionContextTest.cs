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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class DomainObjectTransactionContextTest : ClientTransactionBaseTest
  {
    private Order _loadedOrder1;
    private Order _notYetLoadedOrder2;
    private Order _newOrder;

    private DomainObjectTransactionContext _loadedOrder1Context;
    private DomainObjectTransactionContext _notYetLoadedOrder2Context;
    private DomainObjectTransactionContext _newOrderContext;

    public override void SetUp ()
    {
      base.SetUp();

      _loadedOrder1 = DomainObjectIDs.Order1.GetObject<Order>();
      _notYetLoadedOrder2 = (Order)LifetimeService.GetObjectReference(TestableClientTransaction, DomainObjectIDs.Order3);
      _newOrder = Order.NewObject();

      _loadedOrder1Context = new DomainObjectTransactionContext(new DomainObjectTransactionContextImplementation(_loadedOrder1), TestableClientTransaction);
      _notYetLoadedOrder2Context = new DomainObjectTransactionContext(new DomainObjectTransactionContextImplementation(_notYetLoadedOrder2), TestableClientTransaction);
      _newOrderContext = new DomainObjectTransactionContext(new DomainObjectTransactionContextImplementation(_newOrder), TestableClientTransaction);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_loadedOrder1Context.DomainObject, Is.SameAs(_loadedOrder1));
      Assert.That(_loadedOrder1Context.ClientTransaction, Is.SameAs(TestableClientTransaction));
    }

    [Test]
    public void Initialization_InvalidTransaction ()
    {
      Assert.That(
          () => new DomainObjectTransactionContext(new DomainObjectTransactionContextImplementation(_newOrder), ClientTransaction.CreateRootTransaction()),
          Throws.TypeOf<ClientTransactionsDifferException>());
    }

    [Test]
    public void State_IsInvalid_False ()
    {
      Assert.That(_newOrderContext.State.IsInvalid, Is.False);
    }

    [Test]
    public void State_IsInvalid_False_InvalidInCurrentTransaction ()
    {
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(_loadedOrder1.State.IsInvalid, Is.False);
        Assert.That(_loadedOrder1Context.State.IsInvalid, Is.False);

        DeleteOrder(_loadedOrder1);
        ClientTransaction.Current.Commit();

        Assert.That(_loadedOrder1.State.IsInvalid, Is.True);
        Assert.That(_loadedOrder1Context.State.IsInvalid, Is.False);
      }
    }

    [Test]
    public void State_IsInvalid_True ()
    {
      _newOrder.Delete();
      Assert.That(_newOrderContext.State.IsInvalid, Is.True);
    }

    [Test]
    public void State_NotYetLoaded ()
    {
      Assert.That(_notYetLoadedOrder2Context.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void State_FromDataContainer ()
    {
      Assert.That(_newOrderContext.State.IsNew, Is.True);
      Assert.That(_loadedOrder1Context.State.IsUnchanged, Is.True);

      _loadedOrder1.OrderNumber = 2;

      Assert.That(_loadedOrder1Context.State.IsChanged, Is.True);
    }

    [Test]
    public void State_FromDataContainer_WithChangedRelation ()
    {
      _loadedOrder1.OrderItems.Clear();

      Assert.That(_loadedOrder1Context.State.IsChanged, Is.True);
    }

    [Test]
    public void RegisterForCommit_Unchanged ()
    {
      Assert.That(_loadedOrder1Context.State.IsUnchanged, Is.True);
      _loadedOrder1Context.RegisterForCommit();
      Assert.That(_loadedOrder1Context.State.IsChanged, Is.True);
      Assert.That(GetDataContainer(_loadedOrder1Context).HasBeenMarkedChanged, Is.True);
    }

    [Test]
    public void RegisterForCommit_Changed ()
    {
      _loadedOrder1.OrderNumber = 2;

      Assert.That(_loadedOrder1Context.State.IsChanged, Is.True);
      _loadedOrder1Context.RegisterForCommit();
      Assert.That(_loadedOrder1Context.State.IsChanged, Is.True);
      Assert.That(GetDataContainer(_loadedOrder1Context).HasBeenMarkedChanged, Is.True);

      _loadedOrder1.OrderNumber = _loadedOrder1.Properties[typeof(Order), "OrderNumber"].GetOriginalValue<int>();

      Assert.That(_loadedOrder1Context.State.IsChanged, Is.True);
    }

    [Test]
    public void RegisterForCommit_New ()
    {
      Assert.That(_newOrderContext.State.IsNew, Is.True);
      _newOrderContext.RegisterForCommit();

      Assert.That(_newOrderContext.State.IsNew, Is.True);
      Assert.That(GetDataContainer(_newOrderContext).HasBeenMarkedChanged, Is.False);
    }

    [Test]
    public void RegisterForCommit_Deleted ()
    {
      DeleteOrder(_loadedOrder1);
      Assert.That(_loadedOrder1Context.State.IsDeleted, Is.True);

      Assert.That(() => _loadedOrder1Context.RegisterForCommit(), Throws.Nothing);

      Assert.That(_loadedOrder1Context.State.IsDeleted, Is.True);
      Assert.That(GetDataContainer(_loadedOrder1Context).HasBeenMarkedChanged, Is.False);
    }

    [Test]
    public void RegisterForCommit_Invalid ()
    {
      _newOrder.Delete();

      Assert.That(
          () => _newOrderContext.RegisterForCommit(),
          Throws.TypeOf<ObjectInvalidException>().With.Message.Contains(_newOrder.ID.ToString()));
      Assert.That(_newOrderContext.State.IsInvalid, Is.True);
    }

    [Test]
    public void RegisterForCommit_NotLoadedYet ()
    {
      UnloadService.UnloadData(_loadedOrder1Context.ClientTransaction, _loadedOrder1.ID);
      Assert.That(_loadedOrder1Context.State.IsNotLoadedYet, Is.True);

      _loadedOrder1Context.RegisterForCommit();

      Assert.That(_loadedOrder1Context.State.IsChanged, Is.True);
      var dataContainer = GetDataContainer(_loadedOrder1Context);
      Assert.That(dataContainer, Is.Not.Null);
      Assert.That(dataContainer.HasBeenMarkedChanged, Is.True);
    }

    [Test]
    public void Timestamp_LoadedObject ()
    {
      var timestamp = _loadedOrder1Context.Timestamp;
      Assert.That(timestamp, Is.Not.Null);
      Assert.That(timestamp, Is.SameAs(_loadedOrder1.Timestamp));
    }

    [Test]
    public void Timestamp_NewObject ()
    {
      var timestamp = _newOrderContext.Timestamp;
      Assert.That(timestamp, Is.Null);
    }

    [Test]
    public void Timestamp_Discarded ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContext.Timestamp,
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      Assert.That(TestableClientTransaction.DataManager.DataContainers[_notYetLoadedOrder2.ID], Is.Null);

      _notYetLoadedOrder2Context.EnsureDataAvailable();

      Assert.That(TestableClientTransaction.DataManager.DataContainers[_notYetLoadedOrder2.ID], Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[_notYetLoadedOrder2.ID].DomainObject, Is.SameAs(_notYetLoadedOrder2));
    }

    [Test]
    public void EnsureDataAvailable_Discarded ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContext.EnsureDataAvailable(),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void TryEnsureDataAvailable_True ()
    {
      Assert.That(TestableClientTransaction.DataManager.DataContainers[_notYetLoadedOrder2.ID], Is.Null);

      var result = _notYetLoadedOrder2Context.TryEnsureDataAvailable();

      Assert.That(result, Is.True);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[_notYetLoadedOrder2.ID], Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[_notYetLoadedOrder2.ID].DomainObject, Is.SameAs(_notYetLoadedOrder2));
    }

    [Test]
    public void TryEnsureDataAvailable_False ()
    {
      var notFoundObjectReference = DomainObjectMother.GetNotLoadedObject(TestableClientTransaction, new ObjectID(typeof(ClassWithAllDataTypes), Guid.NewGuid()));
      var notFoundContext = new DomainObjectTransactionContext(new DomainObjectTransactionContextImplementation(notFoundObjectReference), TestableClientTransaction);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[notFoundObjectReference.ID], Is.Null);

      var result = notFoundContext.TryEnsureDataAvailable();

      Assert.That(result, Is.False);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[notFoundObjectReference.ID], Is.Null);
    }

    [Test]
    public void TryEnsureDataAvailable_Discarded ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContext.TryEnsureDataAvailable(),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void ClientTransaction_WhenReferenceInitializationIsActive_ReturnsValue ()
    {
      var objectBeingInitialized = Order.NewObject();
      var context = new DomainObjectTransactionContextImplementation(objectBeingInitialized);
      context.BeginDomainObjectReferenceInitializing();
      var referenceInitializationContext = new DomainObjectTransactionContext(context, TestableClientTransaction);

      Assert.That(referenceInitializationContext.ClientTransaction, Is.SameAs(TestableClientTransaction));
    }

    private void DeleteOrder (Order order)
    {
      while (order.OrderItems.Count > 0)
        order.OrderItems[0].Delete();

      order.OrderTicket.Delete();
      order.Delete();
    }

    private DataContainer GetDataContainer (DomainObjectTransactionContext transactionContext)
    {
      return ClientTransactionTestHelper.GetIDataManager(transactionContext.ClientTransaction).DataContainers[transactionContext.DomainObject.ID];
    }
  }
}
