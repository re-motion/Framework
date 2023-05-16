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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionCommitDataTest : ClientTransactionBaseTest
  {
    [Test]
    public void CommitPropagatesChangesToLoadedObjectsToParentTransaction ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        order.OrderNumber = 5;

        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }

        Assert.That(order.OrderNumber, Is.EqualTo(5));
      }

      Assert.That(order, Is.Not.Null);
      Assert.That(order.OrderNumber, Is.EqualTo(5));
    }

    [Test]
    public void CommitPropagatesChangesToNewObjectsToParentTransaction ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        classWithAllDataTypes.Int32Property = 7;

        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }
      }

      Assert.That(classWithAllDataTypes.Int32Property, Is.EqualTo(7));
    }

    [Test]
    public void CommitLeavesUnchangedObjectsLoadedInSub ()
    {
      Order order;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        order = DomainObjectIDs.Order1.GetObject<Order>();

        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }
        Assert.That(order.OrderNumber, Is.EqualTo(1));
      }

      Assert.That(order.OrderNumber, Is.EqualTo(1));
    }

    [Test]
    public void CommitLeavesUnchangedObjectsLoadedInRoot ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }
        Assert.That(order.OrderNumber, Is.EqualTo(1));
      }

      Assert.That(order.OrderNumber, Is.EqualTo(1));
    }

    [Test]
    public void CommitLeavesUnchangedNewObjects ()
    {
      Order order = Order.NewObject();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }
        Assert.That(order.OrderNumber, Is.EqualTo(0));
      }

      Assert.That(order.OrderNumber, Is.EqualTo(0));
    }

    [Test]
    public void CommitDeletedObject_DoesNotInfluencePreviouslyRelatedObjects_OneToManyWithDomainObjectCollection ()
    {
      var originalOrder = DomainObjectIDs.Order3.GetObject<Order>();
      Assert.That(originalOrder.OrderItems.Count, Is.EqualTo(1));

      var originalTicket = originalOrder.OrderTicket;
      var originalOfficial = originalOrder.Official;

      var orderItem = originalOrder.OrderItems[0];

      Order newOrder;
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        newOrder = DomainObjectIDs.Order4.GetObject<Order>();
        newOrder.OrderItems.Add(orderItem);
        originalTicket.Delete(); // dependent object
        originalOrder.Delete();
        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransaction.Current.Commit();
        }
        Assert.That(orderItem.Order, Is.SameAs(newOrder));
        Assert.That(orderItem.Properties.Find("Order").GetRelatedObjectID(), Is.EqualTo(newOrder.ID));
      }
      Assert.That(orderItem.Order, Is.SameAs(newOrder));
      Assert.That(orderItem.Properties.Find("Order").GetRelatedObjectID(), Is.EqualTo(newOrder.ID));

      Assert.That(newOrder.OrderItems.ContainsObject(orderItem));
      Assert.That(originalOrder.State.IsDeleted, Is.True);
      Assert.That(originalOrder.OrderItems, Is.Empty);
      Assert.That(originalOrder.OrderTicket, Is.Null);
      Assert.That(originalOrder.Official, Is.Null);

      Assert.That(originalTicket.Order, Is.Null);
      Assert.That(originalOfficial.Orders, Has.No.Member(originalOrder));
    }

    [Test]
    public void CommitDeletedObject_DoesNotInfluencePreviouslyRelatedObjects_OneToManyWithVirtualCollection ()
    {
      var originalProduct = DomainObjectIDs.Product1.GetObject<Product>();
      Assert.That(originalProduct.Reviews.Count, Is.EqualTo(3));

      var productReview = originalProduct.Reviews[0];

      Product newProduct;
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        newProduct = DomainObjectIDs.Product2.GetObject<Product>();
        newProduct.Reviews.EnsureDataComplete();
        productReview.Product = newProduct;
        originalProduct.Delete();
        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransaction.Current.Commit();
        }
        Assert.That(productReview.Product, Is.SameAs(newProduct));
        Assert.That(productReview.Properties.Find("Product").GetRelatedObjectID(), Is.EqualTo(newProduct.ID));
      }
      Assert.That(productReview.Product, Is.SameAs(newProduct));
      Assert.That(productReview.Properties.Find("Product").GetRelatedObjectID(), Is.EqualTo(newProduct.ID));

      Assert.That(newProduct.Reviews.Contains(productReview.ID), Is.True);
      Assert.That(originalProduct.State.IsDeleted, Is.True);
      Assert.That(originalProduct.Reviews, Is.Empty);
    }

    [Test]
    public void CommitDeletedObject_DoesNotInfluencePreviouslyRelatedObjects_OneToManyWithLazyLoadedVirtualCollection ()
    {
      var originalProduct = DomainObjectIDs.Product1.GetObject<Product>();

      var productReview = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
      Assert.That(productReview.Product, Is.SameAs(originalProduct));
      Assert.That(originalProduct.Reviews.IsDataComplete, Is.False);

      Product newProduct;
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        newProduct = DomainObjectIDs.Product2.GetObject<Product>();
        productReview.Product = newProduct;
        originalProduct.Delete();
        Assert.That(newProduct.Reviews.IsDataComplete, Is.False);

        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransaction.Current.Commit();
        }
        Assert.That(productReview.Product, Is.SameAs(newProduct));
        Assert.That(productReview.Properties.Find("Product").GetRelatedObjectID(), Is.EqualTo(newProduct.ID));
      }
      Assert.That(productReview.Product, Is.SameAs(newProduct));
      Assert.That(productReview.Properties.Find("Product").GetRelatedObjectID(), Is.EqualTo(newProduct.ID));

      Assert.That(newProduct.Reviews.Contains(productReview.ID), Is.True);
      Assert.That(originalProduct.State.IsDeleted, Is.True);
      Assert.That(originalProduct.Reviews, Is.Empty);
    }

    [Test]
    public void CommitDeletedObject_DoesNotInfluencePreviouslyRelatedObjects_OneToOne ()
    {
      var originalOrder = DomainObjectIDs.Order3.GetObject<Order>();
      Assert.That(originalOrder.OrderItems.Count, Is.EqualTo(1));
      var originalItem = originalOrder.OrderItems[0];
      var originalOfficial = originalOrder.Official;

      var orderTicket = originalOrder.OrderTicket;

      Order newOrder;
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        newOrder = DomainObjectIDs.Order4.GetObject<Order>();
        newOrder.OrderTicket.Delete(); // delete old ticket
        newOrder.OrderTicket = orderTicket;

        originalItem.Delete(); // delete old item
        originalOrder.Delete();
        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransaction.Current.Commit();
        }
        Assert.That(orderTicket.Order, Is.SameAs(newOrder));
        Assert.That(orderTicket.Properties.Find("Order").GetRelatedObjectID(), Is.EqualTo(newOrder.ID));
      }
      Assert.That(orderTicket.Order, Is.SameAs(newOrder));
      Assert.That(orderTicket.Properties.Find("Order").GetRelatedObjectID(), Is.EqualTo(newOrder.ID));

      Assert.That(newOrder.OrderTicket, Is.SameAs(orderTicket));
      Assert.That(originalOrder.State.IsDeleted, Is.True);
      Assert.That(originalOrder.OrderItems, Is.Empty);
      Assert.That(originalOrder.OrderTicket, Is.Null);
      Assert.That(originalOrder.Official, Is.Null);

      Assert.That(originalItem.Order, Is.Null);
      Assert.That(originalOfficial.Orders, Has.No.Member(originalOrder));
    }

    [Test]
    public void CommitSavesPropertyValuesToParentTransaction ()
    {
      Order loadedOrder = DomainObjectIDs.Order1.GetObject<Order>();
      ClassWithAllDataTypes newClassWithAllDataTypes = ClassWithAllDataTypes.NewObject();

      loadedOrder.OrderNumber = 5;
      newClassWithAllDataTypes.Int16Property = 7;

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        loadedOrder.OrderNumber = 13;
        newClassWithAllDataTypes.Int16Property = 47;

        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }

        Assert.That(loadedOrder.State.IsUnchanged, Is.True);
        Assert.That(newClassWithAllDataTypes.State.IsUnchanged, Is.True);

        Assert.That(loadedOrder.OrderNumber, Is.EqualTo(13));
        Assert.That(newClassWithAllDataTypes.Int16Property, Is.EqualTo(47));
      }

      Assert.That(loadedOrder.OrderNumber, Is.EqualTo(13));
      Assert.That(newClassWithAllDataTypes.Int16Property, Is.EqualTo(47));

      Assert.That(loadedOrder.State.IsChanged, Is.True);
      Assert.That(newClassWithAllDataTypes.State.IsNew, Is.True);
    }

    [Test]
    public void CommitSavesRelatedObjectsToParentTransaction ()
    {
      Order order = Order.NewObject();
      Official official = DomainObjectIDs.Official1.GetObject<Official>();
      order.Official = official;
      order.Customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      OrderItem orderItem = OrderItem.NewObject();
      order.OrderItems.Add(orderItem);

      Assert.That(order.Official, Is.SameAs(official));
      Assert.That(order.OrderItems.Count, Is.EqualTo(1));
      Assert.That(order.OrderItems.ContainsObject(orderItem), Is.True);
      Assert.That(order.OrderTicket, Is.Null);

      OrderItem newOrderItem;
      OrderTicket newOrderTicket;

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        newOrderItem = OrderItem.NewObject();

        orderItem.Delete();
        order.OrderItems.Add(newOrderItem);
        order.OrderItems.Add(OrderItem.NewObject());

        newOrderTicket = OrderTicket.NewObject();
        order.OrderTicket = newOrderTicket;

        Assert.That(order.Official, Is.SameAs(official));
        Assert.That(order.OrderItems.Count, Is.EqualTo(2));
        Assert.That(order.OrderItems.ContainsObject(orderItem), Is.False);
        Assert.That(order.OrderItems.ContainsObject(newOrderItem), Is.True);
        Assert.That(order.OrderTicket, Is.Not.Null);
        Assert.That(order.OrderTicket, Is.SameAs(newOrderTicket));

        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }

        Assert.That(order.State.IsUnchanged, Is.True);

        Assert.That(order.Official, Is.SameAs(official));
        Assert.That(order.OrderItems.Count, Is.EqualTo(2));
        Assert.That(order.OrderItems.ContainsObject(orderItem), Is.False);
        Assert.That(order.OrderItems.ContainsObject(newOrderItem), Is.True);
        Assert.That(order.OrderTicket, Is.Not.Null);
        Assert.That(order.OrderTicket, Is.SameAs(newOrderTicket));
      }

      Assert.That(order.Official, Is.SameAs(official));
      Assert.That(order.OrderItems.Count, Is.EqualTo(2));
      Assert.That(order.OrderItems.ContainsObject(orderItem), Is.False);
      Assert.That(order.OrderItems.ContainsObject(newOrderItem), Is.True);
      Assert.That(order.OrderTicket, Is.Not.Null);
      Assert.That(order.OrderTicket, Is.SameAs(newOrderTicket));
    }

    [Test]
    public void CommittedRelatedObjectCollectionOrder ()
    {
      Order order = Order.NewObject();
      Official official = DomainObjectIDs.Official1.GetObject<Official>();
      order.Official = official;
      order.Customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      OrderItem orderItem1 = OrderItem.NewObject();
      OrderItem orderItem2 = OrderItem.NewObject();
      OrderItem orderItem3 = OrderItem.NewObject();
      order.OrderItems.Add(orderItem1);
      order.OrderItems.Add(orderItem2);
      order.OrderItems.Add(orderItem3);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(order.OrderItems, Is.EqualTo(new object[] {orderItem1, orderItem2, orderItem3}));
        order.OrderItems.Clear();
        order.OrderItems.Add(orderItem2);
        order.OrderItems.Add(orderItem3);
        order.OrderItems.Add(orderItem1);
        Assert.That(order.OrderItems, Is.EqualTo(new object[] { orderItem2, orderItem3, orderItem1 }));
        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransaction.Current.Commit();
        }
        Assert.That(order.OrderItems, Is.EqualTo(new object[] { orderItem2, orderItem3, orderItem1 }));
      }
      Assert.That(order.OrderItems, Is.EqualTo(new object[] { orderItem2, orderItem3, orderItem1 }));
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(order.OrderItems, Is.EqualTo(new object[] {orderItem2, orderItem3, orderItem1}));
      }
    }

    [Test]
    public void CommitSavesRelatedObjectToParentTransaction ()
    {
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer>();
      Employee employee = computer.Employee;
      Location location1 = Location.NewObject();
      Location location2 = Location.NewObject();

      Client client = Client.NewObject();
      location1.Client = client;

      Employee newEmployee;
      Client newClient1 = Client.NewObject();
      Client newClient2;

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        newEmployee = Employee.NewObject();
        computer.Employee = newEmployee;

        location1.Client = newClient1;

        newClient2 = Client.NewObject();
        location2.Client = newClient2;

        Assert.That(employee.Computer, Is.Null);
        Assert.That(computer.Employee, Is.SameAs(newEmployee));
        Assert.That(location1.Client, Is.SameAs(newClient1));
        Assert.That(location2.Client, Is.SameAs(newClient2));

        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }

        Assert.That(employee.Computer, Is.Null);
        Assert.That(computer.Employee, Is.SameAs(newEmployee));
        Assert.That(location1.Client, Is.SameAs(newClient1));
        Assert.That(location2.Client, Is.SameAs(newClient2));
      }

      Assert.That(employee.Computer, Is.Null);
      Assert.That(computer.Employee, Is.SameAs(newEmployee));
      Assert.That(location1.Client, Is.SameAs(newClient1));
      Assert.That(location2.Client, Is.SameAs(newClient2));
    }

    [Test]
    public void EndPointsAreCorrectFromBothSidesForCompletelyNewObjectGraphs ()
    {
      Order order;
      OrderItem newOrderItem;
      OrderTicket newOrderTicket;
      Official newOfficial;
      Customer newCustomer;
      Ceo newCeo;

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        order = Order.NewObject();

        newOrderTicket = OrderTicket.NewObject();
        order.OrderTicket = newOrderTicket;

        newOrderItem = OrderItem.NewObject();
        order.OrderItems.Add(newOrderItem);

        newOfficial = Official.NewObject();
        order.Official = newOfficial;

        newCustomer = Customer.NewObject();
        order.Customer = newCustomer;

        newCeo = Ceo.NewObject();
        newCustomer.Ceo = newCeo;

        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }
      }

      Assert.That(newOrderTicket.Order, Is.SameAs(order));
      Assert.That(order.OrderTicket, Is.SameAs(newOrderTicket));

      Assert.That(order.OrderItems[0], Is.SameAs(newOrderItem));
      Assert.That(newOrderItem.Order, Is.SameAs(order));

      Assert.That(order.Official.Orders[0], Is.SameAs(order));
      Assert.That(order.Official, Is.SameAs(newOfficial));

      Assert.That(order.Customer.Orders[0], Is.SameAs(order));
      Assert.That(order.Customer, Is.SameAs(newCustomer));

      Assert.That(newCustomer.Ceo, Is.SameAs(newCeo));
      Assert.That(newCeo.Company, Is.SameAs(newCustomer));
    }

    [Test]
    public void CommitObjectInSubTransactionAndReloadInParent ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Order orderInSub = DomainObjectIDs.Order1.GetObject<Order>();
        Assert.That(orderInSub.OrderNumber, Is.Not.EqualTo(4711));
        orderInSub.OrderNumber = 4711;
        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }
      }

      Order orderInParent = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(orderInParent.OrderNumber, Is.EqualTo(4711));
    }

    [Test]
    public void CommitObjectInSubTransactionAndReloadInNewSub ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Order orderInSub = DomainObjectIDs.Order1.GetObject<Order>();
        Assert.That(orderInSub.OrderNumber, Is.Not.EqualTo(4711));
        orderInSub.OrderNumber = 4711;
        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransactionScope.CurrentTransaction.Commit();
        }
      }

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Order orderInSub = DomainObjectIDs.Order1.GetObject<Order>();
        Assert.That(orderInSub.OrderNumber, Is.EqualTo(4711));
      }
    }

    [Test]
    public void ObjectValuesCanBeChangedInParentAndChildSubTransactions ()
    {
      ClassWithAllDataTypes cwadt = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      cwadt.PopulateMandatoryProperties();
      Assert.That(cwadt.Int32Property, Is.Not.EqualTo(7));
      Assert.That(cwadt.Int16Property, Is.Not.EqualTo(8));

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        cwadt.Int32Property = 7;
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That(cwadt.Int32Property, Is.EqualTo(7));
          cwadt.Int16Property = 8;
          ClientTransaction.Current.Commit();
        }
        Assert.That(cwadt.Int32Property, Is.EqualTo(7));
        Assert.That(cwadt.Int16Property, Is.EqualTo(8));
        ClientTransaction.Current.Commit();
      }
      Assert.That(cwadt.Int32Property, Is.EqualTo(7));
      Assert.That(cwadt.Int16Property, Is.EqualTo(8));
      TestableClientTransaction.Commit();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var objectInThisTransaction = cwadt.GetHandle().GetObject();
        Assert.That(objectInThisTransaction.Int32Property, Is.EqualTo(7));
        Assert.That(objectInThisTransaction.Int16Property, Is.EqualTo(8));
      }
    }

    [Test]
    public void PropertyValue_HasChangedHandling_WithNestedSubTransactions ()
    {
      ClassWithAllDataTypes cwadt = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      cwadt.PopulateMandatoryProperties();

      Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
      Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
      Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
      Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
      Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original), Is.EqualTo(32767));
      Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original), Is.EqualTo(2147483647));

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        cwadt.Int32Property = 7;
        Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.True);
        Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
        Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.True);
        Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
        Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original), Is.EqualTo(32767));
        Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original), Is.EqualTo(2147483647));

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
          Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
          Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
          Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
          Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original), Is.EqualTo(32767));
          Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original), Is.EqualTo(7));

          cwadt.Int16Property = 8;

          Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
          Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.True);
          Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
          Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.True);

          ClientTransaction.Current.Commit();

          Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
          Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
          Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
          Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
          Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original), Is.EqualTo(8));
          Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original), Is.EqualTo(7));
        }

        Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.True);
        Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.True);
        Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.True);
        Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.True);
        Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original), Is.EqualTo(32767));
        Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original), Is.EqualTo(2147483647));

        ClientTransaction.Current.Commit();

        Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
        Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
        Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
        Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
        Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original), Is.EqualTo(8));
        Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original), Is.EqualTo(7));
      }

      Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.True);
      Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.True);
      Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.True);
      Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.True);
      Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original), Is.EqualTo(32767));
      Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original), Is.EqualTo(2147483647));

      TestableClientTransaction.Commit();

      Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
      Assert.That(cwadt.InternalDataContainer.HasValueChanged(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
      Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property")), Is.False);
      Assert.That(cwadt.InternalDataContainer.HasValueBeenTouched(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property")), Is.False);
      Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int16Property"), ValueAccess.Original), Is.EqualTo(8));
      Assert.That(cwadt.InternalDataContainer.GetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "Int32Property"), ValueAccess.Original), Is.EqualTo(7));
    }

    [Test]
    public void ObjectEndPoint_HasChangedHandling_WithNestedSubTransactions ()
    {
      OrderTicket orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      Order oldOrder = orderTicket.Order;

      Order newOrder = DomainObjectIDs.Order3.GetObject<Order>();
      OrderTicket oldOrderTicket = newOrder.OrderTicket;

      Order newOrder2 = DomainObjectIDs.Order4.GetObject<Order>();
      OrderTicket oldOrderTicket2 = newOrder2.OrderTicket;


      RelationEndPointID propertyID = RelationEndPointID.Create(orderTicket.ID, typeof(OrderTicket).FullName + ".Order");

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        orderTicket.Order = newOrder;
        oldOrder.OrderTicket = oldOrderTicket;
        Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
        Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(oldOrder.ID));

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That(orderTicket.Order, Is.EqualTo(newOrder));

          Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
          Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(newOrder.ID));

          orderTicket.Order = newOrder2;
          oldOrderTicket2.Order = newOrder;

          Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
          Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(newOrder.ID));

          ClientTransaction.Current.Commit();
          Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
          Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(newOrder2.ID));
        }

        Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
        Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(oldOrder.ID));

        ClientTransaction.Current.Commit();
        Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
        Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(newOrder2.ID));
      }
      Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
      Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(oldOrder.ID));

      ClientTransaction.Current.Commit();

      Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
      Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(newOrder2.ID));
    }

    [Test]
    public void VirtualObjectEndPoint_HasChangedHandling_WithNestedSubTransactions ()
    {
      OrderTicket orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      Order order1 = orderTicket1.Order;

      OrderTicket orderTicket2 = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket>();
      Order order3 = orderTicket2.Order;

      Order order4 = DomainObjectIDs.Order3.GetObject<Order>();
      OrderTicket orderTicket3 = order4.OrderTicket;

      RelationEndPointID propertyID = RelationEndPointID.Create(order4.ID, typeof(Order).FullName + ".OrderTicket");

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        order4.OrderTicket = orderTicket1;
        orderTicket3.Order = order1;

        Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
        Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(orderTicket3.ID));

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That(order4.OrderTicket, Is.EqualTo(orderTicket1));

          Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
          Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(orderTicket1.ID));

          order4.OrderTicket = orderTicket2;
          orderTicket1.Order = order3;

          Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
          Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(orderTicket1.ID));

          ClientTransaction.Current.Commit();
          Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
          Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(orderTicket2.ID));
        }

        Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
        Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(orderTicket3.ID));

        ClientTransaction.Current.Commit();
        Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
        Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(orderTicket2.ID));
      }
      Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
      Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(orderTicket3.ID));

      ClientTransaction.Current.Commit();

      Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
      Assert.That(((IObjectEndPoint)GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID)).OriginalOppositeObjectID, Is.EqualTo(orderTicket2.ID));
    }

    [Test]
    public void DomainObjectCollectionEndPoint_HasChangedHandling_WithNestedSubTransactions ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();

      OrderItem newItem = OrderItem.NewObject();
      newItem.Product = "Product";

      OrderItem firstItem = order.OrderItems[0];

      RelationEndPointID propertyID = RelationEndPointID.Create(order.ID, typeof(Order).FullName + ".OrderItems");

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        order.OrderItems.Add(newItem);

        Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
        Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(newItem), Is.False);
        Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(firstItem), Is.True);

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That(order.OrderItems.ContainsObject(newItem), Is.True);

          Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
          Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(newItem), Is.True);
          Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(firstItem), Is.True);

          order.OrderItems[0].Delete();
          Assert.That(order.OrderItems.ContainsObject(newItem), Is.True);

          Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
          Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(newItem), Is.True);
          Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(firstItem), Is.True);

          ClientTransaction.Current.Commit();

          Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
          Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(newItem), Is.True);
          Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(firstItem), Is.False);
        }

        Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
        Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(newItem), Is.False);
        Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(firstItem), Is.True);

        ClientTransaction.Current.Commit();
        Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
        Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(newItem), Is.True);
        Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(firstItem), Is.False);
      }
      Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.True);
      Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(newItem), Is.False);
      Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(firstItem), Is.True);

      ClientTransaction.Current.Commit();

      Assert.That(GetRelationEndPointWithoutLoading(ClientTransaction.Current, propertyID).HasChanged, Is.False);
      Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(newItem), Is.True);
      Assert.That(GetCollectionWithOriginalData(ClientTransaction.Current, propertyID).ContainsObject(firstItem), Is.False);
    }

    [Test]
    public void Committing_TwoDeletedNewObjects_RelatedInParentTransaction ()
    {
      var order = Order.NewObject();
      var orderTicket = OrderTicket.NewObject();

      orderTicket.Order = order;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        order.Delete();
        orderTicket.Delete();

        using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
        {
          ClientTransaction.Current.Commit();
        }
      }

      Assert.That(order.State.IsInvalid, Is.True);
      Assert.That(orderTicket.State.IsInvalid, Is.True);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }

    private DomainObjectCollection GetCollectionWithOriginalData (ClientTransaction clientTransaction, RelationEndPointID propertyID)
    {
      return ((IDomainObjectCollectionEndPoint)GetRelationEndPointWithoutLoading(clientTransaction, propertyID)).GetCollectionWithOriginalData();
    }

    private IRelationEndPoint GetRelationEndPointWithoutLoading (ClientTransaction clientTransaction, RelationEndPointID propertyID)
    {
      return GetDataManager(clientTransaction).GetRelationEndPointWithoutLoading(propertyID);
    }

    private DataManager GetDataManager (ClientTransaction transaction)
    {
      return (DataManager)PrivateInvoke.GetNonPublicProperty(transaction, "DataManager");
    }
  }
}
