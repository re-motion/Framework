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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionRelationTest : ClientTransactionBaseTest
  {
    [Test]
    public void Parent_CanReloadRelatedObject_LoadedInSubTransaction_AndGetTheSameReference ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Order order;
      OrderTicket orderTicket;
      using (subTransaction.EnterDiscardingScope())
      {
        order = DomainObjectIDs.Order1.GetObject<Order>();
        orderTicket = order.OrderTicket;
      }
      Assert.That(DomainObjectIDs.Order1.GetObject<Order>(), Is.SameAs(order));
      Assert.That(DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>(), Is.SameAs(orderTicket));
    }

    [Test]
    public void Parent_CanReloadNullRelatedObject_LoadedInSubTransaction ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Computer computer;
      Employee employee;
      using (subTransaction.EnterDiscardingScope())
      {
        computer = DomainObjectIDs.Computer4.GetObject<Computer>();
        Assert.That(computer.Employee, Is.Null);
        employee = DomainObjectIDs.Employee1.GetObject<Employee>();
        Assert.That(employee.Computer, Is.Null);
      }
      Assert.That(DomainObjectIDs.Computer4.GetObject<Computer>().Employee, Is.Null);
      Assert.That(computer.Employee, Is.Null);
      Assert.That(DomainObjectIDs.Employee1.GetObject<Employee>().Computer, Is.Null);
      Assert.That(employee.Computer, Is.Null);
    }

    [Test]
    public void Parent_CanReloadRelatedObjectCollectionWithDomainObjectCollection_LoadedInSubTransaction_AndGetTheSameReferences ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Order order;
      var orderItems = new HashSet<OrderItem>();
      using (subTransaction.EnterDiscardingScope())
      {
        order = DomainObjectIDs.Order1.GetObject<Order>();
        orderItems.Add(order.OrderItems[0]);
        orderItems.Add(order.OrderItems[1]);
      }
      Assert.That(DomainObjectIDs.Order1.GetObject<Order>(), Is.SameAs(order));
      Assert.That(orderItems.Contains(DomainObjectIDs.OrderItem1.GetObject<OrderItem>()), Is.True);
      Assert.That(orderItems.Contains(DomainObjectIDs.OrderItem2.GetObject<OrderItem>()), Is.True);
    }

    [Test]
    public void Parent_CanReloadRelatedObjectCollectionWithVirtualCollection_LoadedInSubTransaction_AndGetTheSameReferences ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Product product;
      var productReviews = new HashSet<ProductReview>();
      using (subTransaction.EnterDiscardingScope())
      {
        product = DomainObjectIDs.Product1.GetObject<Product>();
        productReviews.Add(product.Reviews[0]);
        productReviews.Add(product.Reviews[1]);
      }
      Assert.That(DomainObjectIDs.Product1.GetObject<Product>(), Is.SameAs(product));
      Assert.That(productReviews.Contains(DomainObjectIDs.ProductReview1.GetObject<ProductReview>()), Is.True);
      Assert.That(productReviews.Contains(DomainObjectIDs.ProductReview2.GetObject<ProductReview>()), Is.True);
    }

    [Test]
    public void OverwritingUnidirectionalInSubTransactionWorks ()
    {
      Location location = Location.NewObject();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        location.Client = Client.NewObject();
      }
    }

    [Test]
    public void OverwritingDeletedNewUnidirectionalInSubTransactionWorks ()
    {
      Location location = Location.NewObject();
      location.Client = Client.NewObject();
      location.Client.Delete();

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(location.Client.State.IsInvalid, Is.True);
        location.Client = Client.NewObject();
        Assert.That(location.Client.State.IsNew, Is.True);
      }
    }

    [Test]
    public void OverwritingDeletedLoadedUnidirectionalInSubTransactionWorks ()
    {
      Location location = Location.NewObject();
      location.Client = DomainObjectIDs.Client1.GetObject<Client>();
      location.Client.Delete();

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(location.Client.State.IsInvalid, Is.True);
        location.Client = Client.NewObject();
        Assert.That(location.Client.State.IsNew, Is.True);
      }
    }

    [Test]
    public void OverwritingCollections ()
    {
      Customer location = Customer.NewObject();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        location.Orders = new OrderCollection();
      }
    }


    [Test]
    public void LoadRelatedDataContainersWithDomainObjectCollection_MakesParentWritableWhileGettingItsContainers ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();

      // cause parent tx to require reload of data containers...
      UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, order.OrderItems.AssociatedEndPointID);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var relatedObjects = order.OrderItems.ToArray();
        Assert.That(relatedObjects,
            Is.EquivalentTo(new[] { DomainObjectIDs.OrderItem1.GetObject<OrderItem>(), DomainObjectIDs.OrderItem2.GetObject<OrderItem>() }));
      }
    }

    [Test]
    public void LoadRelatedDataContainersWithVirtualCollection_MakesParentWritableWhileGettingItsContainers ()
    {
      var product = DomainObjectIDs.Product1.GetObject<Product>();

      // cause parent tx to require reload of data containers...
      UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, product.Reviews.AssociatedEndPointID);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var relatedObjects = product.Reviews.ToArray();
        Assert.That(
            relatedObjects,
            Is.EquivalentTo(
                new[]
                {
                    DomainObjectIDs.ProductReview1.GetObject<ProductReview>(),
                    DomainObjectIDs.ProductReview2.GetObject<ProductReview>(),
                    DomainObjectIDs.ProductReview3.GetObject<ProductReview>()
                }));
      }
    }

    [Test]
    public void SubTransactionHasRelatedObjectCollectionEqualToParent_WithDomainObjectCollection ()
    {
      Order loadedOrder = DomainObjectIDs.Order1.GetObject<Order>();
      ObjectList<OrderItem> loadedItems = loadedOrder.OrderItems;

      Assert.That(loadedOrder.OrderItems, Is.SameAs(loadedItems));

      Dev.Null = loadedOrder.OrderItems[0];
      OrderItem loadedItem2 = loadedOrder.OrderItems[1];
      OrderItem newItem1 = OrderItem.NewObject();
      OrderItem newItem2 = OrderItem.NewObject();
      newItem2.Product = "Baz, buy two get three for free";

      loadedOrder.OrderItems.Clear();
      loadedOrder.OrderItems.Add(loadedItem2);
      loadedOrder.OrderItems.Add(newItem1);
      loadedOrder.OrderItems.Add(newItem2);

      Order newOrder = Order.NewObject();
      OrderItem newItem3 = OrderItem.NewObject();
      newItem3.Product = "FooBar, the energy bar with extra Foo";
      newOrder.OrderItems.Add(newItem3);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(DomainObjectIDs.Order1.GetObject<Order>(), Is.SameAs(loadedOrder));
        Assert.That(loadedOrder.OrderItems, Is.Not.SameAs(loadedItems));

        Assert.That(loadedOrder.OrderItems, Is.EqualTo(new[] { loadedItem2, newItem1, newItem2 }));

        Assert.That(loadedItem2.Order, Is.SameAs(loadedOrder));
        Assert.That(newItem1.Order, Is.SameAs(loadedOrder));
        Assert.That(newItem2.Order, Is.SameAs(loadedOrder));

        Assert.That(loadedOrder.OrderItems[2].Product, Is.EqualTo("Baz, buy two get three for free"));

        Assert.That(newOrder.OrderItems, Is.EqualTo(new[] { newItem3 }));
        Assert.That(newOrder.OrderItems[0].Product, Is.EqualTo("FooBar, the energy bar with extra Foo"));
        Assert.That(newItem3.Order, Is.SameAs(newOrder));
      }
    }

    [Test]
    public void SubTransactionHasRelatedObjectCollectionEqualToParent_WithVirtualCollection ()
    {
      Product loadedProduct = DomainObjectIDs.Product1.GetObject<Product>();
      IObjectList<ProductReview> loadedItems = loadedProduct.Reviews;

      Assert.That(loadedProduct.Reviews, Is.SameAs(loadedItems));

      Dev.Null = loadedProduct.Reviews[0];
      ProductReview loadedItem2 = loadedProduct.Reviews[1];

      ProductReview newItem1 = ProductReview.NewObject();
      newItem1.CreatedAt = loadedItem2.CreatedAt.AddDays(1);

      ProductReview newItem2 = ProductReview.NewObject();
      newItem2.CreatedAt = newItem1.CreatedAt.AddDays(1);
      newItem2.Comment = "Baz, buy two get three for free";

      loadedProduct.Reviews.Last().Product = null;
      loadedProduct.Reviews.Last().Product = null;
      loadedProduct.Reviews.Last().Product = null;
      loadedItem2.Product = loadedProduct;
      newItem1.Product = loadedProduct;
      newItem2.Product = loadedProduct;

      Product newProduct = Product.NewObject();
      ProductReview newItem3 = ProductReview.NewObject();
      newItem3.Comment = "FooBar, the energy bar with extra Foo";
      newItem3.Product = newProduct;

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(DomainObjectIDs.Product1.GetObject<Product>(), Is.SameAs(loadedProduct));
        Assert.That(loadedProduct.Reviews, Is.Not.SameAs(loadedItems));

        Assert.That(loadedProduct.Reviews, Is.EqualTo(new[] { loadedItem2, newItem1, newItem2 }));

        Assert.That(loadedItem2.Product, Is.SameAs(loadedProduct));
        Assert.That(newItem1.Product, Is.SameAs(loadedProduct));
        Assert.That(newItem2.Product, Is.SameAs(loadedProduct));

        Assert.That(loadedProduct.Reviews[2].Comment, Is.EqualTo("Baz, buy two get three for free"));

        Assert.That(newProduct.Reviews, Is.EqualTo(new[] { newItem3 }));
        Assert.That(newProduct.Reviews[0].Comment, Is.EqualTo("FooBar, the energy bar with extra Foo"));
        Assert.That(newItem3.Product, Is.SameAs(newProduct));
      }
    }

    [Test]
    public void SortExpressionNotExecuted_WhenLoadingDomainObjectCollectionFromParent ()
    {
      var customer1 = DomainObjectIDs.Customer1.GetObject<Customer>();
      var orders = customer1.Orders.Reverse().ToArray();
      customer1.Orders.Clear();
      customer1.Orders.AddRange(orders);

      Assert.That(customer1.Orders, Is.EqualTo(orders));

      var sortExpression = ((DomainObjectCollectionRelationEndPointDefinition)customer1.Orders.AssociatedEndPointID.Definition).GetSortExpression();
      Assert.That(sortExpression, Is.Not.Null);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(customer1.Orders.ToArray(), Is.EqualTo(orders), "This would not be equal if the sort expression was executed.");
        Assert.That(customer1.Properties[typeof(Customer).FullName + ".Orders"].HasChanged, Is.False);
      }
    }

    [Test]
    public void SortExpressionEvaluated_WhenLoadingVirtualCollectionFromParent ()
    {
      var product = DomainObjectIDs.Product1.GetObject<Product>();
      var productReviews = product.Reviews.Reverse().ToArray();
      productReviews[0].CreatedAt = productReviews[2].CreatedAt.AddDays(1);

      Assert.That(product.Reviews, Is.EquivalentTo(productReviews));
      Assert.That(product.Reviews, Is.Not.EqualTo(productReviews));

      var sortExpression = ((VirtualCollectionRelationEndPointDefinition)product.Reviews.AssociatedEndPointID.Definition).GetSortExpression();
      Assert.That(sortExpression, Is.Not.Null);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(product.Reviews, Is.EquivalentTo(productReviews));
        Assert.That(product.Reviews, Is.Not.EqualTo(productReviews));
        Assert.That(product.Properties[typeof(Product).FullName + ".Reviews"].HasChanged, Is.False);
      }
    }

    [Test]
    public void SubTransactionCanGetRelatedObjectCollectionEvenWhenObjectsHaveBeenDiscarded_WithDomainObjectCollection ()
    {
      Order loadedOrder = DomainObjectIDs.Order1.GetObject<Order>();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        OrderItem orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
        orderItem1.Delete();
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(orderItem1.State.IsInvalid, Is.True);

        ObjectList<OrderItem> orderItems = loadedOrder.OrderItems;
        Assert.That(orderItems.Select(o => o.ID), Is.EqualTo(new[] { DomainObjectIDs.OrderItem2 }));
      }
    }

    [Test]
    public void SubTransactionCanGetRelatedObjectCollectionEvenWhenObjectsHaveBeenDiscarded_WithVirtualCollection ()
    {
      Product loadedProduct = DomainObjectIDs.Product1.GetObject<Product>();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        ProductReview productReview1 = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
        productReview1.Delete();
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(productReview1.State.IsInvalid, Is.True);

        IObjectList<ProductReview> productReviews = loadedProduct.Reviews;
        Assert.That(productReviews.Select(o => o.ID), Is.EqualTo(new[] { DomainObjectIDs.ProductReview2, DomainObjectIDs.ProductReview3 }));
      }
    }

    [Test]
    public void RelatedObjectCollectionChangesAreNotPropagatedToParent_WithDomainObjectCollection ()
    {
      Order loadedOrder = DomainObjectIDs.Order1.GetObject<Order>();

      Assert.That(loadedOrder.OrderItems.Count, Is.EqualTo(2));
      OrderItem loadedItem1 = loadedOrder.OrderItems[0];
      OrderItem loadedItem2 = loadedOrder.OrderItems[1];

      Order newOrder = Order.NewObject();

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        loadedOrder.OrderItems.Clear();
        newOrder.OrderItems.Add(OrderItem.NewObject());

        using (TestableClientTransaction.EnterNonDiscardingScope())
        {
          Assert.That(loadedOrder.OrderItems.Count, Is.EqualTo(2));
          Assert.That(loadedOrder.OrderItems[0], Is.SameAs(loadedItem1));
          Assert.That(loadedOrder.OrderItems[1], Is.SameAs(loadedItem2));
          Assert.That(newOrder.OrderItems.Count, Is.EqualTo(0));
        }
      }
    }

    [Test]
    public void RelatedObjectCollectionChangesAreNotPropagatedToParent_WithVirtualCollection ()
    {
      Product loadedOrder = DomainObjectIDs.Product1.GetObject<Product>();

      Assert.That(loadedOrder.Reviews.Count, Is.EqualTo(3));
      ProductReview loadedItem1 = loadedOrder.Reviews[0];
      ProductReview loadedItem2 = loadedOrder.Reviews[1];
      ProductReview loadedItem3 = loadedOrder.Reviews[2];

      Product newProduct = Product.NewObject();

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        loadedItem1.Product = null;
        loadedItem2.Product = null;
        loadedItem3.Product = null;
        var newItem = ProductReview.NewObject();
        newItem.Product = newProduct;

        using (TestableClientTransaction.EnterNonDiscardingScope())
        {
          Assert.That(
              loadedOrder.Reviews.Select(o => o.ID),
              Is.EqualTo(new[] { DomainObjectIDs.ProductReview1, DomainObjectIDs.ProductReview2, DomainObjectIDs.ProductReview3 }));
          Assert.That(newProduct.Reviews.Count, Is.EqualTo(0));
        }
      }
    }

    [Test]
    public void SubTransactionHasSameRelatedObjectAsParent1To1 ()
    {
      Computer loadedComputer = DomainObjectIDs.Computer1.GetObject<Computer>();
      Employee loadedEmployee = DomainObjectIDs.Employee1.GetObject<Employee>();
      Assert.That(loadedEmployee, Is.Not.SameAs(loadedComputer.Employee));
      loadedComputer.Employee = loadedEmployee;

      Assert.That(loadedComputer.Employee, Is.SameAs(loadedEmployee));
      Assert.That(loadedEmployee.Computer, Is.SameAs(loadedComputer));

      Computer newComputer = Computer.NewObject();
      Employee newEmployee = Employee.NewObject();
      newEmployee.Computer = newComputer;

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(DomainObjectIDs.Computer1.GetObject<Computer>(), Is.SameAs(loadedComputer));
        Assert.That(DomainObjectIDs.Employee1.GetObject<Employee>(), Is.SameAs(loadedEmployee));

        Assert.That(loadedComputer.Employee, Is.SameAs(loadedEmployee));
        Assert.That(loadedEmployee.Computer, Is.SameAs(loadedComputer));

        Assert.That(newEmployee.Computer, Is.SameAs(newComputer));
        Assert.That(newComputer.Employee, Is.SameAs(newEmployee));
      }
    }

    [Test]
    public void RelatedObjectChangesAreNotPropagatedToParent ()
    {
      Computer loadedComputer = DomainObjectIDs.Computer1.GetObject<Computer>();
      Employee loadedEmployee = DomainObjectIDs.Employee3.GetObject<Employee>();

      Computer newComputer = Computer.NewObject();
      Employee newEmployee = Employee.NewObject();
      newEmployee.Computer = newComputer;

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        loadedComputer.Employee = Employee.NewObject();
        loadedEmployee.Computer = Computer.NewObject();

        newComputer.Employee = Employee.NewObject();
        newEmployee.Computer = Computer.NewObject();

        using (TestableClientTransaction.EnterNonDiscardingScope())
        {
          Assert.That(loadedEmployee.Computer, Is.SameAs(loadedComputer));
          Assert.That(loadedComputer.Employee, Is.SameAs(loadedEmployee));

          Assert.That(newEmployee.Computer, Is.SameAs(newComputer));
          Assert.That(newComputer.Employee, Is.SameAs(newEmployee));
        }
      }
    }

    [Test]
    public void RelatedObjectChangesArePropagatedDuringCommit_WithVirtualCollectionLoaded_DoesNotAffectLoadState ()
    {
      Product loadedProduct = DomainObjectIDs.Product1.GetObject<Product>();
      loadedProduct.Reviews.EnsureDataComplete();
      ProductReview newProductReview;
      ProductReview referenceProductReview = loadedProduct.Reviews[0];
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(loadedProduct.Reviews.IsDataComplete, Is.False);

        newProductReview = ProductReview.NewObject();
        newProductReview.CreatedAt = referenceProductReview.CreatedAt.AddDays(1);
        newProductReview.Comment = "Baz, buy two get three for free";
        newProductReview.Product = loadedProduct;

        Assert.That(loadedProduct.Reviews.IsDataComplete, Is.False);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(loadedProduct.Reviews.IsDataComplete, Is.False);
      }

      Assert.That(loadedProduct.Reviews.IsDataComplete, Is.True);
      IObjectList<ProductReview> productReviews = loadedProduct.Reviews;
      Assert.That(
          productReviews.Select(o => o.ID),
          Is.EqualTo(new[] { DomainObjectIDs.ProductReview1, newProductReview.ID, DomainObjectIDs.ProductReview2, DomainObjectIDs.ProductReview3 }));
    }

    [Test]
    public void RelatedObjectChangesArePropagatedDuringCommit_WithVirtualCollectionNotLoaded_DoesNotAffectLoadState ()
    {
      Product loadedProduct = DomainObjectIDs.Product1.GetObject<Product>();
      ProductReview newProductReview;
      ProductReview referenceProductReview = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
      Assert.That(loadedProduct.Reviews.IsDataComplete, Is.False);
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(loadedProduct.Reviews.IsDataComplete, Is.False);

        newProductReview = ProductReview.NewObject();
        newProductReview.CreatedAt = referenceProductReview.CreatedAt.AddDays(1);
        newProductReview.Comment = "Baz, buy two get three for free";
        newProductReview.Product = loadedProduct;

        Assert.That(loadedProduct.Reviews.IsDataComplete, Is.False);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(loadedProduct.Reviews.IsDataComplete, Is.False);
      }

      Assert.That(loadedProduct.Reviews.IsDataComplete, Is.False);
      IObjectList<ProductReview> productReviews = loadedProduct.Reviews;
      Assert.That(
          productReviews.Select(o => o.ID),
          Is.EqualTo(new[] { DomainObjectIDs.ProductReview1, newProductReview.ID, DomainObjectIDs.ProductReview2, DomainObjectIDs.ProductReview3 }));
    }
  }
}
