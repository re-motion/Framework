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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Delete
{
  [TestFixture]
  public class DeleteDomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void Delete ()
    {
      OrderTicket orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();

      orderTicket.Delete();

      Assert.That(orderTicket.State.IsDeleted, Is.True);
      Assert.That(orderTicket.InternalDataContainer.State.IsDeleted, Is.True);
    }

    [Test]
    public void DeleteTwice ()
    {
      OrderTicket orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();

      orderTicket.Delete();

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver(orderTicket);
      orderTicket.Delete();

      Assert.That(eventReceiver.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetObject ()
    {
      OrderTicket orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();

      orderTicket.Delete();
      Assert.That(
          () => orderTicket.ID.GetObject<OrderTicket>(),
          Throws.InstanceOf<ObjectDeletedException>());
    }

    [Test]
    public void GetObjectAndIncludeDeleted ()
    {
      OrderTicket orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();

      orderTicket.Delete();

      Assert.That(orderTicket.ID.GetObject<OrderTicket>(includeDeleted: true), Is.Not.Null);
    }

    [Test]
    public void ModifyDeletedObject ()
    {
      Order order = DomainObjectIDs.Order3.GetObject<Order>();

      var dataContainer = order.InternalDataContainer;

      order.Delete();
      Assert.That(
          () => SetPropertyValue(dataContainer, typeof(Order), "OrderNumber", 10),
          Throws.InstanceOf<ObjectDeletedException>());
    }

    [Test]
    public void AccessDeletedObject ()
    {
      Order order = DomainObjectIDs.Order3.GetObject<Order>();

      order.Delete();

      Assert.That(order.ID, Is.EqualTo(DomainObjectIDs.Order3));
      Assert.That(order.OrderNumber, Is.EqualTo(3));
      Assert.That(order.DeliveryDate, Is.EqualTo(new DateTime(2005, 3, 1)));
      Assert.That(order.InternalDataContainer.Timestamp, Is.Not.Null);
      Assert.That(GetPropertyValue(order.InternalDataContainer, typeof(Order), "OrderNumber"), Is.Not.Null);
    }

    [Test]
    public void CascadedDeleteForDomainObjectCollection ()
    {
      Employee supervisor = DomainObjectIDs.Employee1.GetObject<Employee>();
      supervisor.DeleteWithSubordinates();

      DomainObject deletedSubordinate4 = DomainObjectIDs.Employee4.GetObject<Employee>(includeDeleted: true);
      DomainObject deletedSubordinate5 = DomainObjectIDs.Employee5.GetObject<Employee>(includeDeleted: true);

      Assert.That(supervisor.State.IsDeleted, Is.True);
      Assert.That(deletedSubordinate4.State.IsDeleted, Is.True);
      Assert.That(deletedSubordinate5.State.IsDeleted, Is.True);

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      CheckIfObjectIsDeleted(DomainObjectIDs.Employee1);
      CheckIfObjectIsDeleted(DomainObjectIDs.Employee4);
      CheckIfObjectIsDeleted(DomainObjectIDs.Employee5);
    }

    [Test]
    public void CascadedDeleteForVirtualCollection ()
    {
      Product product = DomainObjectIDs.Product1.GetObject<Product>();
      product.DeleteWithProductReviews();

      DomainObject deletedProductReview1 = DomainObjectIDs.ProductReview1.GetObject<ProductReview>(includeDeleted: true);
      DomainObject deletedProductReview2 = DomainObjectIDs.ProductReview2.GetObject<ProductReview>(includeDeleted: true);

      Assert.That(product.State.IsDeleted, Is.True);
      Assert.That(deletedProductReview1.State.IsDeleted, Is.True);
      Assert.That(deletedProductReview2.State.IsDeleted, Is.True);

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      CheckIfObjectIsDeleted(DomainObjectIDs.Product1);
      CheckIfObjectIsDeleted(DomainObjectIDs.ProductReview1);
      CheckIfObjectIsDeleted(DomainObjectIDs.ProductReview2);
    }

    [Test]
    public void CascadedDeleteForNewObjectsWithDomainObjectCollection ()
    {
      Order newOrder = Order.NewObject();
      OrderTicket newOrderTicket = OrderTicket.NewObject(newOrder);
      Assert.That(newOrder.OrderTicket, Is.SameAs(newOrderTicket));
      OrderItem newOrderItem = OrderItem.NewObject(newOrder);
      Assert.That(newOrder.OrderItems, Has.Member(newOrderItem));

      newOrder.Deleted += delegate
      {
        newOrderTicket.Delete();
        newOrderItem.Delete();
      };

      newOrder.Delete();

      //Expectation: no exception

      Assert.That(newOrder.State.IsInvalid, Is.True);
      Assert.That(newOrderTicket.State.IsInvalid, Is.True);
      Assert.That(newOrderItem.State.IsInvalid, Is.True);
    }

    [Test]
    public void CascadedDeleteForNewObjectsWithVirtualCollection ()
    {
      Product newProduct = Product.NewObject();
      ProductReview newProductReview = ProductReview.NewObject();
      newProductReview.Product = newProduct;
      Assert.That(newProduct.Reviews, Has.Member(newProductReview));

      newProduct.Deleted += delegate
      {
        newProductReview.Delete();
      };

      newProduct.Delete();

      //Expectation: no exception

      Assert.That(newProduct.State.IsInvalid, Is.True);
      Assert.That(newProductReview.State.IsInvalid, Is.True);
    }

    [Test]
    public void DeleteFromSelfReferencingOneToManyRelation ()
    {
      var folder = Folder.NewObject();
      folder.ParentFolder = folder;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        folder.Delete();
        Assert.That(folder.State.IsDeleted, Is.True);
        Assert.That(folder.ParentFolder, Is.Null);
      }
    }

    [Test]
    public void DeleteFromSelfReferencingOneToManyRelation_WithOtherObjectsInvolved ()
    {
      var folder1 = Folder.NewObject();
      folder1.ParentFolder = folder1;

      var file2 = File.NewObject();
      file2.ParentFolder = folder1;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        folder1.Delete();
        Assert.That(folder1.State.IsDeleted, Is.True);
        Assert.That(file2.State.IsChanged, Is.True);
        Assert.That(file2.ParentFolder, Is.Null);
      }
    }
  }
}
