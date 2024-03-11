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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Delete
{
  [TestFixture]
  public class DeleteNewDomainObjectTest : ClientTransactionBaseTest
  {
    private Order _newOrder;
    private DataContainer _newOrderContainer;
    private OrderTicket _newOrderTicket;

    private PropertyDefinition _orderNumberProperty;

    public override void SetUp ()
    {
      base.SetUp();

      _newOrder = Order.NewObject();
      _newOrderContainer = _newOrder.InternalDataContainer;
      _newOrderTicket = OrderTicket.NewObject(_newOrder);

      _orderNumberProperty = GetPropertyDefinition(typeof(Order), "OrderNumber");
    }

    [Test]
    public void RelatedObject ()
    {
      Assert.That(_newOrderTicket.Order, Is.SameAs(_newOrder));
      Assert.That(_newOrder.OrderTicket, Is.SameAs(_newOrderTicket));

      _newOrder.Delete();

      Assert.That(_newOrderTicket.Order, Is.Null);

      _newOrderTicket.Delete();

      Assert.That(TestableClientTransaction.DataManager.DataContainers.Count, Is.EqualTo(0));
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints.Count, Is.EqualTo(0));
    }

    [Test]
    public void DomainObjectID ()
    {
      ObjectID oldID = _newOrder.ID;
      _newOrder.Delete();
      ObjectID newID = _newOrder.ID;
      Assert.That(newID, Is.EqualTo(oldID));
    }

    [Test]
    public void DomainObjectState ()
    {
      _newOrder.Delete();
      Assert.That(_newOrder.State.IsInvalid, Is.True);
    }

    [Test]
    public void DomainObjectDataContainer ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrder.InternalDataContainer,
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DomainObjectDelete ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrder.Delete(),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DomainObjectGetRelatedObject ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrder.GetRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DomainObjectGetRelatedObjectsForDomainObjectCollection ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrder.GetRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DomainObjectGetRelatedObjectsForVirtualCollection ()
    {
      var product = Product.NewObject();
      var productReview = ProductReview.NewObject();
      productReview.Product = product;

      product.Delete();
      Assert.That(
          () => product.GetRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DomainObjectGetOriginalRelatedObject ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrder.GetRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DomainObjectGetOriginalRelatedObjectsForDomainObjectCollection ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrder.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]

    public void DomainObjectGetOriginalRelatedObjectsForVirtualCollection ()
    {
      var product = Product.NewObject();
      var productReview = ProductReview.NewObject();
      productReview.Product = product;

      product.Delete();
      Assert.That(
          () => product.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DomainObjectSetRelatedObject ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrder.SetRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _newOrderTicket),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DomainObjectIsInvalid ()
    {
      Assert.That(_newOrder.State.IsInvalid, Is.False);

      _newOrder.Delete();

      Assert.That(_newOrder.State.IsInvalid, Is.True);
    }

    [Test]
    public void DomainObjectGetPropertyValue ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrder.Properties[typeof(Order), "OrderNumber"].GetValueWithoutTypeCheck(),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DomainObjectSetPropertyValue ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrder.Properties[typeof(Order), "OrderNumber"].SetValueWithoutTypeCheck(10),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DataContainerGetValue ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContainer.GetValue(GetPropertyDefinition(typeof(Order), "OrderNumber")),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DataContainerSetValue ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContainer.SetValue(GetPropertyDefinition(typeof(Order), "OrderNumber"), 10),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DataContainerDomainObject ()
    {
      _newOrder.Delete();
      Assert.That(_newOrderContainer.DomainObject, Is.SameAs(_newOrder));
    }

    [Test]
    public void DataContainerID ()
    {
      _newOrder.Delete();
      Assert.That(_newOrderContainer.ID, Is.SameAs(_newOrder.ID));
    }

    [Test]
    public void DataContainerClassDefinition ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContainer.ClassDefinition,
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DataContainerDomainObjectType ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContainer.DomainObjectType,
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DataContainerState ()
    {
      _newOrder.Delete();
      Assert.That(_newOrderContainer.State.IsDiscarded, Is.True);
    }

    [Test]
    public void DataContainerTimestamp ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContainer.Timestamp,
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DataContainerClientTransaction ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContainer.ClientTransaction,
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void DataContainerIsDiscarded ()
    {
      DataContainer newDataContainer = _newOrder.InternalDataContainer;
      Assert.That(newDataContainer.State.IsDiscarded, Is.False);

      _newOrder.Delete();

      Assert.That(newDataContainer.State.IsDiscarded, Is.True);
    }

    [Test]
    public void PropertyValueGetValue ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContainer.GetValue(_orderNumberProperty),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void PropertyValueSetValue ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContainer.SetValue(_orderNumberProperty, 10),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void PropertyValueOriginalValue ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContainer.GetValue(_orderNumberProperty, ValueAccess.Original),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void PropertyValueHasChanged ()
    {
      _newOrder.Delete();
      Assert.That(
          () => _newOrderContainer.HasValueChanged(_orderNumberProperty),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void PropertyValueHasBeenTouched ()
    {
      _newOrder.Delete();

      Assert.That(
          () => _newOrderContainer.HasValueBeenTouched(_orderNumberProperty),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void Events ()
    {
      var orderItemsCollection = _newOrder.OrderItems;
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver(
          new DomainObject[] { _newOrder, _newOrderTicket },
          new DomainObjectCollection[] { orderItemsCollection });

      _newOrder.Delete();

      ChangeState[] expectedStates =
          new ChangeState[]
          {
              new ObjectDeletionState(_newOrder, "1. Deleting event of order"),
              new CollectionDeletionState(orderItemsCollection, "2. Deleting of order.OrderItems"),
              new RelationChangeState(
                  _newOrderTicket,
                  "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order",
                  _newOrder,
                  null,
                  "3. Relation changing event of orderTicket"),
              new RelationChangeState(
                  _newOrderTicket,
                  "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order",
                  null,
                  null,
                  "4. Relation changed event of orderTicket"),
              new CollectionDeletionState(orderItemsCollection, "5. Deleted of order.OrderItems"),
              new ObjectDeletionState(_newOrder, "6. Deleted event of order")
          };

      eventReceiver.Check(expectedStates);
    }

    [Test]
    public void DeleteFromManyToOneRelationForDomainObjectCollection ()
    {
      Customer newCustomer = Customer.NewObject();

      _newOrder.Customer = newCustomer;

      ObjectID newOrderID = _newOrder.ID;

      _newOrder.Delete();

      Assert.That(newCustomer.Orders.Contains(newOrderID), Is.False);
    }

    [Test]
    public void DeleteFromManyToOneRelationForVirtualCollection ()
    {
      var product = Product.NewObject();
      var productReview = ProductReview.NewObject();
      productReview.Product = product;

      var productReviewID = productReview.ID;

      productReview.Delete();

      Assert.That(product.Reviews.Contains(productReviewID), Is.False);
    }

    [Test]
    public void DeleteFromOneToManyRelationForDomainObjectCollection ()
    {
      Customer newCustomer = Customer.NewObject();

      _newOrder.Customer = newCustomer;

      ObjectID newCustomerID = newCustomer.ID;

      newCustomer.Delete();

      Assert.That(_newOrder.Customer, Is.Null);
    }

    [Test]
    public void DeleteFromOneToManyRelationForVirtualCollection ()
    {
      var product = Product.NewObject();
      var productReview = ProductReview.NewObject();
      productReview.Product = product;

      product.Delete();

      Assert.That(productReview.Product, Is.Null);
    }

    [Test]
    public void DeleteFromSelfReferencingOneToManyRelation ()
    {
      var folder = Folder.NewObject();
      folder.ParentFolder = folder;

      folder.Delete();
      Assert.That(folder.State.IsInvalid, Is.True);
    }

    [Test]
    public void DeleteFromSelfReferencingOneToManyRelation_WithOtherObjectsInvolved ()
    {
      var folder1 = Folder.NewObject();
      folder1.ParentFolder = folder1;

      var file2 = File.NewObject();
      file2.ParentFolder = folder1;

      folder1.Delete();
      Assert.That(folder1.State.IsInvalid, Is.True);
      Assert.That(file2.State.IsNew, Is.True);
      Assert.That(file2.ParentFolder, Is.Null);
    }

    [Test]
    public void DeleteNewObjectsInDomainObjectsCommittingEvent ()
    {
      _newOrder.Committing += (o, args) =>
      {
        _newOrder.Delete();
        _newOrderTicket.Delete();
      };
      _newOrderTicket.Committing += (o, args) => Assert.Fail("NewOrderTicket_Committing event should not be raised.");
      TestableClientTransaction.Committing += (sender, args1) => Assert.That(args1.DomainObjects.Count, Is.EqualTo(2));

      TestableClientTransaction.Commit();
    }

    [Test]
    public void DeleteNewObjectsInClientTransactionsCommittingEvent ()
    {
      _newOrder.Committing += (sender, args) => Assert.Fail("Should not be called.");
      _newOrderTicket.Committing += (sender1, args1) => Assert.Fail("Should not be called.");
      TestableClientTransaction.Committing += (sender2, args2) =>
      {
        _newOrder.Delete();
        _newOrderTicket.Delete();
      };
      TestableClientTransaction.Commit();
    }
  }
}
