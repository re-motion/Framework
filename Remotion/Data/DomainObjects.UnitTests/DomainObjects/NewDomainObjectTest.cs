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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class NewDomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void Creation ()
    {
      Order order = Order.NewObject();

      Assert.That(order.ID, Is.Not.Null);
      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.InternalDataContainer.DomainObject, Is.SameAs(order));
    }

    [Test]
    public void GetObject_Generic ()
    {
      Order order = Order.NewObject();
      Order sameOrder = order.ID.GetObject<Order>();

      Assert.That(sameOrder, Is.SameAs(order));
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = Order.NewObject();

      Assert.That(order.OrderTicket, Is.Null);
    }

    [Test]
    public void SetRelatedObject ()
    {
      Partner partner = Partner.NewObject();
      Ceo ceo = Ceo.NewObject();

      Assert.That(partner.Ceo, Is.Null);
      Assert.That(ceo.Company, Is.Null);

      partner.Ceo = ceo;

      Assert.That(ceo.Company, Is.SameAs(partner));
      Assert.That(partner.Ceo, Is.SameAs(ceo));
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Order order = Order.NewObject();

      Assert.That(order.OrderItems, Is.Not.Null);
      Assert.That(order.OrderItems.Count, Is.EqualTo(0));
    }

    [Test]
    public void SetRelatedObjects ()
    {
      Order order = Order.NewObject();
      OrderItem orderItem = OrderItem.NewObject();

      order.OrderItems.Add(orderItem);

      Assert.That(orderItem.Order, Is.SameAs(order));
      Assert.That(order.OrderItems.Count, Is.EqualTo(1));
      Assert.That(order.OrderItems[orderItem.ID], Is.Not.Null);
    }

    [Test]
    public void StateForPropertyChange ()
    {
      Customer customer = Customer.NewObject();
      customer.Name = "Arthur Dent";

      Assert.That(customer.Name, Is.EqualTo("Arthur Dent"));
      Assert.That(customer.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name"].GetOriginalValue<string>(), Is.Null);
      Assert.That(customer.State.IsNew, Is.True);
    }

    [Test]
    public void StateForOneToOneRelationChange ()
    {
      Partner partner = Partner.NewObject();
      Ceo ceo = Ceo.NewObject();

      partner.Ceo = ceo;

      Assert.That(partner.State.IsNew, Is.True);
      Assert.That(ceo.State.IsNew, Is.True);
    }

    [Test]
    public void StateForOneToManyRelationChange ()
    {
      Order order = Order.NewObject();
      OrderItem orderItem = OrderItem.NewObject();

      order.OrderItems.Add(orderItem);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(orderItem.State.IsNew, Is.True);
    }

    [Test]
    public void Events ()
    {
      Order order = Order.NewObject();
      OrderItem orderItem = OrderItem.NewObject();

      DomainObjectEventReceiver orderEventReceiver = new DomainObjectEventReceiver(order);
      DomainObjectEventReceiver orderItemEventReceiver = new DomainObjectEventReceiver(orderItem);

      DomainObjectCollectionEventReceiver collectionEventReceiver = new DomainObjectCollectionEventReceiver(
          order.OrderItems);

      order.DeliveryDate = new DateTime(2010, 1, 1);
      order.OrderItems.Add(orderItem);

      Assert.That(orderEventReceiver.HasChangingEventBeenCalled, Is.True);
      Assert.That(orderEventReceiver.HasChangedEventBeenCalled, Is.True);
      Assert.That(orderEventReceiver.ChangingPropertyDefinition.PropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"));
      Assert.That(orderEventReceiver.ChangedPropertyDefinition.PropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"));

      Assert.That(orderEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That(orderEventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That(orderEventReceiver.ChangingRelationPropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
      Assert.That(orderEventReceiver.ChangedRelationPropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));

      Assert.That(orderItemEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That(orderItemEventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That(orderItemEventReceiver.ChangingRelationPropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"));
      Assert.That(orderItemEventReceiver.ChangedRelationPropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"));

      Assert.That(collectionEventReceiver.HasAddingEventBeenCalled, Is.True);
      Assert.That(collectionEventReceiver.HasAddedEventBeenCalled, Is.True);
      Assert.That(collectionEventReceiver.AddingDomainObject, Is.SameAs(orderItem));
      Assert.That(collectionEventReceiver.AddedDomainObject, Is.SameAs(orderItem));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Partner partner = Partner.NewObject();
      Ceo ceo = Ceo.NewObject();

      partner.Ceo = ceo;

      Assert.That(partner.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo"), Is.Null);
      Assert.That(ceo.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company"), Is.Null);
    }

    [Test]
    public void GetOriginalRelatedObjects_ForDomainObjectCollection ()
    {
      Order order = Order.NewObject();
      OrderItem orderItem = OrderItem.NewObject();

      order.OrderItems.Add(orderItem);

      DomainObjectCollection originalOrderItems = order.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

      Assert.That(originalOrderItems, Is.Not.Null);
      Assert.That(originalOrderItems.Count, Is.EqualTo(0));
      Assert.That(orderItem.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"), Is.Null);
    }

    [Test]
    public void GetOriginalRelatedObjects_ForVirtualCollection ()
    {
      Product product = Product.NewObject();
      product.Name = "The Product";
      product.Price = 1;

      var productReview = ProductReview.NewObject();
      productReview.Product = product;
      productReview.Reviewer = DomainObjectIDs.Person3.GetObject<Person>();
      productReview.Comment = "Test";
      productReview.CreatedAt = DateTime.Now;
      var originalProductReviews = product.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");

      Assert.That(originalProductReviews, Is.Not.Null);
      Assert.That(originalProductReviews.Count, Is.EqualTo(0));
      Assert.That(productReview.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"), Is.Null);
    }

    [Test]
    public void SaveNewRelatedObjects ()
    {
      Ceo ceo = Ceo.NewObject();
      Customer customer = Customer.NewObject();
      Order order = Order.NewObject();
      OrderTicket orderTicket = OrderTicket.NewObject(order);
      orderTicket.FileName = @"C:\orders\order.tkt";

      OrderItem orderItem = OrderItem.NewObject();
      orderItem.Product = "Product";

      ObjectID ceoID = ceo.ID;
      ObjectID customerID = customer.ID;
      ObjectID orderID = order.ID;
      ObjectID orderTicketID = orderTicket.ID;
      ObjectID orderItemID = orderItem.ID;

      ceo.Name = "Ford Prefect";

      customer.CustomerSince = new DateTime(2000, 1, 1);
      customer.Name = "Arthur Dent";
      customer.Ceo = ceo;

      orderItem.Position = 1;
      orderItem.Product = "Sternenkarte";

      orderTicket.FileName = @"C:\home\arthur_dent\maporder.png";

      order.OrderNumber = 42;
      order.DeliveryDate = new DateTime(2005, 2, 1);
      order.Official = DomainObjectIDs.Official1.GetObject<Official>();
      order.Customer = customer;
      order.OrderItems.Add(orderItem);

      Assert.That(ceo.InternalDataContainer.Timestamp, Is.Null);
      Assert.That(customer.InternalDataContainer.Timestamp, Is.Null);
      Assert.That(order.InternalDataContainer.Timestamp, Is.Null);
      Assert.That(orderTicket.InternalDataContainer.Timestamp, Is.Null);
      Assert.That(orderItem.InternalDataContainer.Timestamp, Is.Null);

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      ceo = ceoID.GetObject<Ceo>();
      customer = customerID.GetObject<Customer>();
      order = orderID.GetObject<Order>();
      orderTicket = orderTicketID.GetObject<OrderTicket>();
      orderItem = orderItemID.GetObject<OrderItem>();
      Official official = DomainObjectIDs.Official1.GetObject<Official>();

      Assert.That(ceo, Is.Not.Null);
      Assert.That(customer, Is.Not.Null);
      Assert.That(order, Is.Not.Null);
      Assert.That(orderTicket, Is.Not.Null);
      Assert.That(orderItem, Is.Not.Null);

      Assert.That(ceo.Company, Is.SameAs(customer));
      Assert.That(customer.Ceo, Is.SameAs(ceo));
      Assert.That(order.Customer, Is.SameAs(customer));
      Assert.That(customer.Orders.Count, Is.EqualTo(1));
      Assert.That(customer.Orders[0], Is.SameAs(order));
      Assert.That(orderTicket.Order, Is.SameAs(order));
      Assert.That(order.OrderTicket, Is.SameAs(orderTicket));
      Assert.That(orderItem.Order, Is.SameAs(order));
      Assert.That(order.OrderItems.Count, Is.EqualTo(1));
      Assert.That(order.OrderItems[0], Is.SameAs(orderItem));
      Assert.That(order.Official, Is.SameAs(official));
      Assert.That(official.Orders.Count, Is.EqualTo(6));
      Assert.That(official.Orders[orderID], Is.Not.Null);

      Assert.That(ceo.Name, Is.EqualTo("Ford Prefect"));
      Assert.That(customer.CustomerSince, Is.EqualTo(new DateTime(2000, 1, 1)));
      Assert.That(customer.Name, Is.EqualTo("Arthur Dent"));
      Assert.That(orderItem.Position, Is.EqualTo(1));
      Assert.That(orderItem.Product, Is.EqualTo("Sternenkarte"));
      Assert.That(orderTicket.FileName, Is.EqualTo(@"C:\home\arthur_dent\maporder.png"));
      Assert.That(order.OrderNumber, Is.EqualTo(42));
      Assert.That(order.DeliveryDate, Is.EqualTo(new DateTime(2005, 2, 1)));

      Assert.That(ceo.InternalDataContainer.Timestamp, Is.Not.Null);
      Assert.That(customer.InternalDataContainer.Timestamp, Is.Not.Null);
      Assert.That(order.InternalDataContainer.Timestamp, Is.Not.Null);
      Assert.That(orderTicket.InternalDataContainer.Timestamp, Is.Not.Null);
      Assert.That(orderItem.InternalDataContainer.Timestamp, Is.Not.Null);
    }

    [Test]
    public void SaveHierarchy ()
    {
      Employee supervisor = Employee.NewObject();
      Employee subordinate = Employee.NewObject();

      ObjectID supervisorID = supervisor.ID;
      ObjectID subordinateID = subordinate.ID;

      supervisor.Name = "Slartibartfast";
      subordinate.Name = "Zarniwoop";
      supervisor.Subordinates.Add(subordinate);

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      supervisor = supervisorID.GetObject<Employee>();
      subordinate = subordinateID.GetObject<Employee>();

      Assert.That(supervisor, Is.Not.Null);
      Assert.That(subordinate, Is.Not.Null);

      Assert.That(supervisor.ID, Is.EqualTo(supervisorID));
      Assert.That(subordinate.ID, Is.EqualTo(subordinateID));

      Assert.That(supervisor.Name, Is.EqualTo("Slartibartfast"));
      Assert.That(subordinate.Name, Is.EqualTo("Zarniwoop"));
    }

    [Test]
    public void ValidateMandatoryRelation ()
    {
      OrderItem orderItem = OrderItem.NewObject();
      orderItem.Product = "Product 1";
      Assert.That(
          () => TestableClientTransaction.Commit(),
          Throws.InstanceOf<MandatoryRelationNotSetException>());
    }

    [Test]
    public void SaveExistingObjectWithRelatedNew ()
    {
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer>();
      Employee newEmployee = Employee.NewObject();
      ObjectID newEmployeeID = newEmployee.ID;

      newEmployee.Computer = computer;
      newEmployee.Name = "Arthur Dent";

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      computer = DomainObjectIDs.Computer4.GetObject<Computer>();
      newEmployee = newEmployeeID.GetObject<Employee>();

      Assert.That(newEmployee, Is.Not.Null);
      Assert.That(newEmployee.Name, Is.EqualTo("Arthur Dent"));
      Assert.That(newEmployee.Computer, Is.SameAs(computer));
    }

    [Test]
    public void DataContainerStateAfterCommit ()
    {
      Computer computer = Computer.NewObject();
      computer.SerialNumber = "12345";
      TestableClientTransaction.Commit();

      Assert.That(computer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void PropertyValueHasChangedAfterCommit ()
    {
      Employee employee = Employee.NewObject();
      employee.Name = "Mr. Prosser";

      Assert.That(employee.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(Employee), "Name")), Is.True);

      TestableClientTransaction.Commit();

      Assert.That(employee.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(Employee), "Name")), Is.False);
    }

    [Test]
    public void PropertyValueHasBeenTouchedAfterCommit ()
    {
      Employee employee = Employee.NewObject();
      employee.Name = "Mr. Prosser";

      Assert.That(employee.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(Employee), "Name")), Is.True);

      TestableClientTransaction.Commit();

      Assert.That(employee.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(Employee), "Name")), Is.False);
    }

    [Test]
    public void OneToOneRelationHasChangedAfterCommit ()
    {
      Employee employee = Employee.NewObject();
      employee.Name = "Jeltz";

      Computer computer = Computer.NewObject();
      computer.SerialNumber = "42";

      employee.Computer = computer;

      Assert.That(employee.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"), Is.Null);
      Assert.That(computer.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"), Is.Null);

      TestableClientTransaction.Commit();

      Assert.That(employee.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"), Is.SameAs(computer));
      Assert.That(computer.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"), Is.SameAs(employee));
    }

    [Test]
    public void OneToManyRelationHasChangedAfterCommit_ForDomainObjectCollection ()
    {
      Employee supervisor = Employee.NewObject();
      Employee subordinate = Employee.NewObject();

      supervisor.Name = "Slartibartfast";
      subordinate.Name = "Zarniwoop";
      supervisor.Subordinates.Add(subordinate);

      Assert.That(
          supervisor.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates").Count,
          Is.EqualTo(0));
      Assert.That(subordinate.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"), Is.Null);

      TestableClientTransaction.Commit();

      DomainObjectCollection originalSubordinates =
          supervisor.GetOriginalRelatedObjectsAsDomainObjectCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates");
      Assert.That(originalSubordinates.Count, Is.EqualTo(1));
      Assert.That(originalSubordinates[subordinate.ID], Is.SameAs(subordinate));
      Assert.That(subordinate.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"), Is.SameAs(supervisor));
    }

    [Test]

    public void OneToManyRelationHasChangedAfterCommit_ForVirtualCollection ()
    {
      Product product = Product.NewObject();
      product.Name = "The Product";
      product.Price = 1;

      var productReview = ProductReview.NewObject();
      productReview.Product = product;
      productReview.Reviewer = DomainObjectIDs.Person3.GetObject<Person>();
      productReview.Comment = "Test";
      productReview.CreatedAt = DateTime.Now;

      Assert.That(
          product.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews").Count,
          Is.EqualTo(0));
      Assert.That(productReview.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"), Is.Null);

      TestableClientTransaction.Commit();

      var originalProductReviews = product.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");
      Assert.That(originalProductReviews.Count, Is.EqualTo(1));
      Assert.That(originalProductReviews[0], Is.SameAs(productReview));
      Assert.That(productReview.GetOriginalRelatedObject("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"), Is.SameAs(product));
    }
  }
}
