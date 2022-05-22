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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.HierarchyManagement
{
  [TestFixture]
  public class ReadOnlyClientTransactionListenerWithLoadRulesTest : StandardMappingTest
  {
    private ReadOnlyClientTransactionListenerWithLoadRules _listener;
    private ClientTransaction _transaction;

    private DomainObject _client1;
    private DomainObject _order1;
    private PropertyDefinition _orderNumberPropertyDefinition;
    private IRelationEndPointDefinition _orderTicketEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _listener = new ReadOnlyClientTransactionListenerWithLoadRules();
      _transaction = ClientTransactionObjectMother.Create();

      _client1 = (DomainObject)LifetimeService.GetObjectReference(_transaction, DomainObjectIDs.Client1);
      _order1 = (DomainObject)LifetimeService.GetObjectReference(_transaction, DomainObjectIDs.Order1);
      _orderNumberPropertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");
      _orderTicketEndPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_listener.CurrentlyLoadingObjectIDs, Is.Empty);
      Assert.That(_listener.IsInLoadMode, Is.False);
    }

    [Test]
    public void AddCurrentlyLoadingObjectIDs ()
    {
      Assert.That(_listener.CurrentlyLoadingObjectIDs, Is.Empty);
      Assert.That(_listener.IsInLoadMode, Is.False);

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Order1 });

      Assert.That(_listener.CurrentlyLoadingObjectIDs, Is.EquivalentTo(new[] { DomainObjectIDs.Order1 }));
      Assert.That(_listener.IsInLoadMode, Is.True);

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 });

      Assert.That(_listener.CurrentlyLoadingObjectIDs, Is.EquivalentTo(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 }));
      Assert.That(_listener.IsInLoadMode, Is.True);
    }

    [Test]
    public void RemoveCurrentlyLoadingObjectIDs ()
    {
      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 });

      Assert.That(_listener.CurrentlyLoadingObjectIDs, Is.EquivalentTo(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 }));
      Assert.That(_listener.IsInLoadMode, Is.True);

      _listener.RemoveCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Order1 });

      Assert.That(_listener.CurrentlyLoadingObjectIDs, Is.EquivalentTo(new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 }));
      Assert.That(_listener.IsInLoadMode, Is.True);

      _listener.RemoveCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Order3, DomainObjectIDs.Order4 });

      Assert.That(_listener.CurrentlyLoadingObjectIDs, Is.Empty);
      Assert.That(_listener.IsInLoadMode, Is.False);
    }

    [Test]
    public void NewObjectCreating_ForbiddenInLoadMode ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      CheckForbiddenOperationWithLoadMode(() => _listener.NewObjectCreating(_transaction, typeof(Order)), "An object of type 'Order' cannot be created.");
    }

    [Test]
    public void NewObjectCreating_ForbiddenWhenTransactionReadOnly ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      Assert.That(() => _listener.NewObjectCreating(_transaction, typeof(Order)), Throws.TypeOf<ClientTransactionReadOnlyException>());
    }

    [Test]
    public void NewObjectCreating_AllowedForActiveTransaction_NotInLoadMode ()
    {
      Assert.That(() => _listener.NewObjectCreating(_transaction, typeof(Order)), Throws.Nothing);
    }

    [Test]
    public void ObjectDeleting_ForbiddenInLoadMode ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      CheckForbiddenOperationWithLoadMode(
          () => _listener.ObjectDeleting(_transaction, _client1),
          "Object 'Client|1627ade8-125f-4819-8e33-ce567c42b00c|System.Guid' cannot be deleted.");
    }

    [Test]
    public void ObjectDeleting_ForbiddenWhenTransactionReadOnly ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      Assert.That(() => _listener.ObjectDeleting(_transaction, _client1), Throws.TypeOf<ClientTransactionReadOnlyException>());
    }

    [Test]
    public void ObjectDeleting_AllowedForActiveTransaction_NotInLoadMode ()
    {
      Assert.That(() => _listener.ObjectDeleting(_transaction, _client1), Throws.Nothing);
    }

    [Test]
    public void PropertyValueChanging_SomeObject_ForbiddenInLoadMode ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      CheckForbiddenOperationWithLoadMode(
          () => _listener.PropertyValueChanging(_transaction, _order1, _orderNumberPropertyDefinition, null, null),
          "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be modified. "
          + "(Modified property: 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber'.)");
    }

    [Test]
    public void PropertyValueChanging_LoadedObject_AllowedInLoadMode ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Client1 });
      Assert.That(_listener.IsInLoadMode, Is.True);

      Assert.That(
          () => _listener.PropertyValueChanging(_transaction, _order1, _orderNumberPropertyDefinition, null, null),
          Throws.InvalidOperationException);

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Order1 });

      Assert.That(
          () => _listener.PropertyValueChanging(_transaction, _order1, _orderNumberPropertyDefinition, null, null),
          Throws.Nothing);
    }

    [Test]
    public void PropertyValueChanging_LoadedObject_ForbiddenWhenDataExistsInSubTransaction ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Order1 });
      Assert.That(_listener.IsInLoadMode, Is.True);

      var fakeSubTransaction = CreateFakeSubTransaction(_transaction);

      Assert.That(() => _listener.PropertyValueChanging(_transaction, _order1, _orderNumberPropertyDefinition, null, null), Throws.Nothing);

      fakeSubTransaction.EnsureDataAvailable(_order1.ID);

      Assert.That(
          () => _listener.PropertyValueChanging(_transaction, _order1, _orderNumberPropertyDefinition, null, null),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' can no longer be modified because its data has already been loaded "
              + "into the subtransaction."));
    }

    [Test]
    public void PropertyValueChanging_AllowedForActiveTransaction_NotInLoadMode ()
    {
      Assert.That(_listener.IsInLoadMode, Is.False);
      Assert.That(() => _listener.PropertyValueChanging(_transaction, _order1, _orderNumberPropertyDefinition, null, null), Throws.Nothing);
    }

    [Test]
    public void PropertyValueChanging_ForbiddenWhenTransactionReadOnly ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);
      Assert.That(_listener.IsInLoadMode, Is.False);

      Assert.That(
          () => _listener.PropertyValueChanging(_transaction, _order1, _orderNumberPropertyDefinition, null, null),
          Throws.TypeOf<ClientTransactionReadOnlyException>());
    }

    [Test]
    public void RelationChanging_SomeObject_ForbiddenInLoadMode ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      CheckForbiddenOperationWithLoadMode(
          () => _listener.RelationChanging(_transaction, _order1, _orderTicketEndPointDefinition, null, null),
          "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be modified. "
          + "(Modified property: 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket'.)");
    }

    [Test]
    public void RelationChanging_LoadedObject_AllowedInLoadMode ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Client1 });
      Assert.That(_listener.IsInLoadMode, Is.True);

      Assert.That(
          () => _listener.RelationChanging(_transaction, _order1, _orderTicketEndPointDefinition, null, null),
          Throws.InvalidOperationException);

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Order1 });

      Assert.That(
          () => _listener.RelationChanging(_transaction, _order1, _orderTicketEndPointDefinition, null, null),
          Throws.Nothing);
    }

    [Test]
    public void RelationChanging_LoadedObject_ForbiddenWhenEndPointCompleteInSubTransaction ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Order1 });
      Assert.That(_listener.IsInLoadMode, Is.True);

      var fakeSubTransaction = CreateFakeSubTransaction(_transaction);

      var relationEndPointID = RelationEndPointID.Create(_order1.ID, _orderTicketEndPointDefinition);

      // Works if there is no matching end-point in the subtx.
      Assert.That(ClientTransactionTestHelper.GetIDataManager(fakeSubTransaction).RelationEndPoints[relationEndPointID], Is.Null);
      Assert.That(() => _listener.RelationChanging(_transaction, _order1, _orderTicketEndPointDefinition, null, null), Throws.Nothing);

      fakeSubTransaction.EnsureDataComplete(relationEndPointID);
      var relationEndPoint = (IVirtualEndPoint)ClientTransactionTestHelper.GetIDataManager(fakeSubTransaction).RelationEndPoints[relationEndPointID];

      // Still works if the matching end-point is incomplete
      relationEndPoint.MarkDataIncomplete();
      Assert.That(ClientTransactionTestHelper.GetIDataManager(fakeSubTransaction).RelationEndPoints[relationEndPointID].IsDataComplete, Is.False);
      Assert.That(() => _listener.RelationChanging(_transaction, _order1, _orderTicketEndPointDefinition, null, null), Throws.Nothing);

      // Throws if the matching end-point is complete
      relationEndPoint.EnsureDataComplete();
      Assert.That(ClientTransactionTestHelper.GetIDataManager(fakeSubTransaction).RelationEndPoints[relationEndPointID].IsDataComplete, Is.True);
      Assert.That(
          () => _listener.RelationChanging(_transaction, _order1, _orderTicketEndPointDefinition, null, null),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of object "
              + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' can no longer be modified because its "
              + "data has already been loaded into the subtransaction."));
    }

    [Test]
    public void RelationChanging_AllowedForActiveTransaction_NotInLoadMode ()
    {
      Assert.That(_listener.IsInLoadMode, Is.False);
      Assert.That(() => _listener.RelationChanging(_transaction, _order1, _orderTicketEndPointDefinition, null, null), Throws.Nothing);
    }

    [Test]
    public void RelationChanging_ForbiddenWhenTransactionReadOnly ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);
      Assert.That(_listener.IsInLoadMode, Is.False);

      Assert.That(
          () => _listener.RelationChanging(_transaction, _order1, _orderTicketEndPointDefinition, null, null),
          Throws.TypeOf<ClientTransactionReadOnlyException>());
    }

    [Test]
    public void DataContainerStateUpdated_SomeObject_ForbiddenInLoadMode ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Client1 });
      Assert.That(_listener.IsInLoadMode, Is.True);

      var someDataContainer = DataContainerObjectMother.Create(DomainObjectIDs.Client2);

      Assert.That(
          () => _listener.DataContainerStateUpdated(_transaction, someDataContainer, new DataContainerState.Builder().SetChanged().Value),
          Throws.TypeOf<ClientTransactionReadOnlyException>());
    }

    [Test]
    public void DataContainerStateUpdated_LoadedObject_AllowedInLoadMode ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Client1 });
      Assert.That(_listener.IsInLoadMode, Is.True);

      var someDataContainer = DataContainerObjectMother.Create(DomainObjectIDs.Client1);

      Assert.That(
          () => _listener.DataContainerStateUpdated(_transaction, someDataContainer, new DataContainerState.Builder().SetChanged().Value),
          Throws.Nothing);
    }

    [Test]
    public void DataContainerStateUpdated_AllowedForActiveTransaction_NotInLoadMode ()
    {
      Assert.That(_listener.IsInLoadMode, Is.False);
      var someDataContainer = DataContainerObjectMother.Create(DomainObjectIDs.Client1);

      Assert.That(
          () => _listener.DataContainerStateUpdated(_transaction, someDataContainer, new DataContainerState.Builder().SetChanged().Value),
          Throws.Nothing);
    }

    [Test]
    public void DataContainerStateUpdated_ForbiddenWhenTransactionReadOnly ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_transaction, false);
      Assert.That(_listener.IsInLoadMode, Is.False);
      var someDataContainer = DataContainerObjectMother.Create(DomainObjectIDs.Client1);

      Assert.That(
          () => _listener.DataContainerStateUpdated(_transaction, someDataContainer, new DataContainerState.Builder().SetChanged().Value),
          Throws.TypeOf<ClientTransactionReadOnlyException>());
    }

    [Test]
    public void Serializability ()
    {
      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      var deserializedInstance = Serializer.SerializeAndDeserialize(_listener);

      Assert.That(deserializedInstance.CurrentlyLoadingObjectIDs, Is.EquivalentTo(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }));
    }

    private void CheckForbiddenOperationWithLoadMode (TestDelegate forbiddenOperation, string specificMessage)
    {
      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Customer1 });

      Assert.That(
          forbiddenOperation,
          Throws.InvalidOperationException.With.Message.EqualTo(
              "While the object 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' is being loaded, only this object can be modified. "
              + specificMessage));

      _listener.AddCurrentlyLoadingObjectIDs(new[] { DomainObjectIDs.Customer2 });

      var expectedMessage = string.Format(
          "While the objects {0} are being loaded, only these object can be modified. " + specificMessage,
          "'" + _listener.CurrentlyLoadingObjectIDs.First() + "', '" + _listener.CurrentlyLoadingObjectIDs.Last() + "'");
      Assert.That(
          forbiddenOperation,
          Throws.InvalidOperationException.With.Message.EqualTo(expectedMessage));
    }

    private ClientTransaction CreateFakeSubTransaction (ClientTransaction clientTransaction)
    {
      var fakeSubTransaction = ClientTransactionObjectMother.Create();
      ClientTransactionTestHelper.SetSubTransaction(clientTransaction, fakeSubTransaction);
      return fakeSubTransaction;
    }
  }
}
