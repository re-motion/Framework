// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation
{
  [TestFixture]
  public class InitializationServiceTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;

    public override void SetUp ()
    {
      base.SetUp();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
    }

    [Test]
    public void TryApplyingCurrentStateAsInitialValue_WithScalarPropertyChanged_SetsOriginalValue_ReturnsObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 15;

      var customer = (Customer)LifetimeService.NewObject(_clientTransaction, typeof(Customer), ParamList.Empty);
      customer.Name = "Alice";

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.True);
      Assert.That(order.State.IsRelationChanged, Is.False);

      Assert.That(customer.State.IsNew, Is.True);
      Assert.That(customer.State.IsDataChanged, Is.True);
      Assert.That(customer.State.IsRelationChanged, Is.False);

      var domainObjects = new DomainObject[] { order, customer };
      var result = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects.Select(obj => obj.ID));

      Assert.That(result, Is.EquivalentTo(domainObjects));

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.False);
      var orderNumberPropertyAccessor = new PropertyIndexer(order)[typeof(Order), nameof(Order.OrderNumber)];
      Assert.That(orderNumberPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderNumberPropertyAccessor.GetValue<int>(), Is.EqualTo(15));
      Assert.That(orderNumberPropertyAccessor.GetOriginalValue<int>(), Is.EqualTo(15));

      Assert.That(customer.State.IsNew, Is.True);
      Assert.That(customer.State.IsDataChanged, Is.False);
      Assert.That(customer.State.IsRelationChanged, Is.False);
      var customerNamePropertyAccessor = new PropertyIndexer(customer)[typeof(Company), nameof(Company.Name)];
      Assert.That(customerNamePropertyAccessor.HasChanged, Is.False);
      Assert.That(customerNamePropertyAccessor.GetValue<string>(), Is.EqualTo("Alice"));
      Assert.That(customerNamePropertyAccessor.GetOriginalValue<string>(), Is.EqualTo("Alice"));
    }

    [Test]
    public void TryApplyingCurrentStateAsInitialValue_WithUnidirectionalRelationChanged_SetsOriginalValue_ReturnsObjects ()
    {
      var location = (Location)LifetimeService.NewObject(_clientTransaction, typeof(Location), ParamList.Empty);
      var client = DomainObjectIDs.Client1.GetObject<Client>(_clientTransaction);
      location.Client = client;

      Assert.That(location.State.IsNew, Is.True);
      Assert.That(location.State.IsDataChanged, Is.True);
      Assert.That(location.State.IsRelationChanged, Is.True);

      Assert.That(client.State.IsNew, Is.False);
      Assert.That(client.State.IsDataChanged, Is.False);
      Assert.That(client.State.IsRelationChanged, Is.False);

      var domainObjects = new DomainObject[] { location };
      var result = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects.Select(obj => obj.ID));

      Assert.That(result, Is.EquivalentTo(domainObjects));

      Assert.That(location.State.IsNew, Is.True);
      Assert.That(location.State.IsDataChanged, Is.False);
      Assert.That(location.State.IsRelationChanged, Is.False);
      var locationClientPropertyAccessor = new PropertyIndexer(location)[typeof(Location), nameof(Location.Client)];
      Assert.That(locationClientPropertyAccessor.HasChanged, Is.False);
      Assert.That(locationClientPropertyAccessor.GetValue<Client>(), Is.EqualTo(client));
      Assert.That(locationClientPropertyAccessor.GetOriginalValue<Client>(), Is.EqualTo(client));

      Assert.That(client.State.IsNew, Is.False);
      Assert.That(client.State.IsDataChanged, Is.False);
      Assert.That(client.State.IsRelationChanged, Is.False);
    }

    [Test]
    public void TryApplyingCurrentStateAsInitialValue_WithOneToOneRelationChanged_SetsOriginalValue_ReturnsObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 14;
      var orderTicket = (OrderTicket)LifetimeService.NewObject(_clientTransaction, typeof(OrderTicket), ParamList.Empty);
      orderTicket.Order = order;

      Assert.That(orderTicket.State.IsNew, Is.True);
      Assert.That(orderTicket.State.IsDataChanged, Is.True);
      Assert.That(orderTicket.State.IsRelationChanged, Is.True);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.True);
      Assert.That(order.State.IsRelationChanged, Is.True);

      var domainObjects = new DomainObject[] { order, orderTicket };
      var result = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects.Select(obj => obj.ID));

      Assert.That(result, Is.EquivalentTo(domainObjects));

      Assert.That(orderTicket.State.IsNew, Is.True);
      Assert.That(orderTicket.State.IsDataChanged, Is.False);
      Assert.That(orderTicket.State.IsRelationChanged, Is.False);
      var orderTicketOrderPropertyAccessor = new PropertyIndexer(orderTicket)[typeof(OrderTicket), nameof(OrderTicket.Order)];
      Assert.That(orderTicketOrderPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderTicketOrderPropertyAccessor.GetValue<Order>(), Is.EqualTo(order));
      Assert.That(orderTicketOrderPropertyAccessor.GetOriginalValue<Order>(), Is.EqualTo(order));
      Assert.That(orderTicketOrderPropertyAccessor.GetRelatedObjectID(), Is.EqualTo(order.ID));
      Assert.That(orderTicketOrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.EqualTo(order.ID));

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.False);
      var orderOrderTicketPropertyAccessor = new PropertyIndexer(order)[typeof(Order), nameof(Order.OrderTicket)];
      Assert.That(orderOrderTicketPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderOrderTicketPropertyAccessor.GetValue<OrderTicket>(), Is.EqualTo(orderTicket));
      Assert.That(orderOrderTicketPropertyAccessor.GetOriginalValue<OrderTicket>(), Is.EqualTo(orderTicket));
    }

    [Test]
    public void TryApplyingCurrentStateAsInitialValue_WithOneToOneRelationChangedTwice_SetsOriginalValue_ReturnsObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 14;
      var orderTicket1 = (OrderTicket)LifetimeService.NewObject(_clientTransaction, typeof(OrderTicket), ParamList.Empty);
      orderTicket1.Order = order;

      var domainObjects1 = new DomainObject[] { order, orderTicket1 };
      var result1 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects1.Select(obj => obj.ID));
      Assert.That(result1, Is.EquivalentTo(domainObjects1));

      var orderTicket2 = (OrderTicket)LifetimeService.NewObject(_clientTransaction, typeof(OrderTicket), ParamList.Empty);
      orderTicket2.Order = order;

      Assert.That(orderTicket1.State.IsNew, Is.True);
      Assert.That(orderTicket1.State.IsDataChanged, Is.True);
      Assert.That(orderTicket1.State.IsRelationChanged, Is.True);

      Assert.That(orderTicket2.State.IsNew, Is.True);
      Assert.That(orderTicket2.State.IsDataChanged, Is.True);
      Assert.That(orderTicket2.State.IsRelationChanged, Is.True);

      var domainObjects2 = new DomainObject[] { order, orderTicket1, orderTicket2 };
      var result2 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects2.Select(obj => obj.ID));
      Assert.That(result2, Is.EquivalentTo(domainObjects2));

      Assert.That(orderTicket1.State.IsNew, Is.True);
      Assert.That(orderTicket1.State.IsDataChanged, Is.False);
      Assert.That(orderTicket1.State.IsRelationChanged, Is.False);
      var orderTicket1OrderPropertyAccessor = new PropertyIndexer(orderTicket1)[typeof(OrderTicket), nameof(OrderTicket.Order)];
      Assert.That(orderTicket1OrderPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderTicket1OrderPropertyAccessor.GetValue<Order>(), Is.Null);
      Assert.That(orderTicket1OrderPropertyAccessor.GetOriginalValue<Order>(), Is.Null);
      Assert.That(orderTicket1OrderPropertyAccessor.GetRelatedObjectID(), Is.Null);
      Assert.That(orderTicket1OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.Null);

      Assert.That(orderTicket2.State.IsNew, Is.True);
      Assert.That(orderTicket2.State.IsDataChanged, Is.False);
      Assert.That(orderTicket2.State.IsRelationChanged, Is.False);
      var orderTicket2OrderPropertyAccessor = new PropertyIndexer(orderTicket2)[typeof(OrderTicket), nameof(OrderTicket.Order)];
      Assert.That(orderTicket2OrderPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderTicket2OrderPropertyAccessor.GetValue<Order>(), Is.EqualTo(order));
      Assert.That(orderTicket2OrderPropertyAccessor.GetOriginalValue<Order>(), Is.EqualTo(order));
      Assert.That(orderTicket2OrderPropertyAccessor.GetRelatedObjectID(), Is.EqualTo(order.ID));
      Assert.That(orderTicket2OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.EqualTo(order.ID));

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.False);
      var orderOrderTicketPropertyAccessor = new PropertyIndexer(order)[typeof(Order), nameof(Order.OrderTicket)];
      Assert.That(orderOrderTicketPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderOrderTicketPropertyAccessor.GetValue<OrderTicket>(), Is.EqualTo(orderTicket2));
      Assert.That(orderOrderTicketPropertyAccessor.GetOriginalValue<OrderTicket>(), Is.EqualTo(orderTicket2));
    }

    [Test]
    public void TryApplyingCurrentStateAsInitialValue_WithDomainObjectCollectionRelationChanged_SetsOriginalValue_ReturnsObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 14;
      var orderItem1 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem1.Order = order;
      var orderItem2 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem2.Order = order;

      Assert.That(orderItem1.State.IsNew, Is.True);
      Assert.That(orderItem1.State.IsDataChanged, Is.True);
      Assert.That(orderItem1.State.IsRelationChanged, Is.True);

      Assert.That(orderItem2.State.IsNew, Is.True);
      Assert.That(orderItem2.State.IsDataChanged, Is.True);
      Assert.That(orderItem2.State.IsRelationChanged, Is.True);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.True);
      Assert.That(order.State.IsRelationChanged, Is.True);

      var domainObjects = new DomainObject[] { order, orderItem1, orderItem2 };
      var result = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects.Select(obj => obj.ID));

      Assert.That(result, Is.EquivalentTo(domainObjects));

      Assert.That(orderItem1.State.IsNew, Is.True);
      Assert.That(orderItem1.State.IsDataChanged, Is.False);
      Assert.That(orderItem1.State.IsRelationChanged, Is.False);
      var orderItem1OrderPropertyAccessor = new PropertyIndexer(orderItem1)[typeof(OrderItem), nameof(OrderItem.Order)];
      Assert.That(orderItem1OrderPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderItem1OrderPropertyAccessor.GetValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem1OrderPropertyAccessor.GetOriginalValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem1OrderPropertyAccessor.GetRelatedObjectID(), Is.EqualTo(order.ID));
      Assert.That(orderItem1OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.EqualTo(order.ID));

      Assert.That(orderItem2.State.IsNew, Is.True);
      Assert.That(orderItem2.State.IsDataChanged, Is.False);
      Assert.That(orderItem2.State.IsRelationChanged, Is.False);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.False);
      var orderOrderItemsPropertyAccessor = new PropertyIndexer(order)[typeof(Order), nameof(Order.OrderItems)];
      Assert.That(orderOrderItemsPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderOrderItemsPropertyAccessor.GetValue<ObjectList<OrderItem>>(), Is.EqualTo(new[] { orderItem1, orderItem2 }));
      Assert.That(orderOrderItemsPropertyAccessor.GetOriginalValue<ObjectList<OrderItem>>(), Is.EqualTo(new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void TryApplyingCurrentStateAsInitialValue_WithVirtualCollectionRelationChanged_SetsOriginalValue_ReturnsObjects ()
    {
      var product = (Product)LifetimeService.NewObject(_clientTransaction, typeof(Product), ParamList.Empty);
      product.Name = "Paper";
      var productReview1 = (ProductReview)LifetimeService.NewObject(_clientTransaction, typeof(ProductReview), ParamList.Empty);
      productReview1.Product = product;
      var productReview2 = (ProductReview)LifetimeService.NewObject(_clientTransaction, typeof(ProductReview), ParamList.Empty);
      productReview2.Product = product;

      Assert.That(productReview1.State.IsNew, Is.True);
      Assert.That(productReview1.State.IsDataChanged, Is.True);
      Assert.That(productReview1.State.IsRelationChanged, Is.True);

      Assert.That(productReview2.State.IsNew, Is.True);
      Assert.That(productReview2.State.IsDataChanged, Is.True);
      Assert.That(productReview2.State.IsRelationChanged, Is.True);

      Assert.That(product.State.IsNew, Is.True);
      Assert.That(product.State.IsDataChanged, Is.True);
      Assert.That(product.State.IsRelationChanged, Is.True);

      var domainObjects = new DomainObject[] { product, productReview1, productReview2 };
      var result = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects.Select(obj => obj.ID));

      Assert.That(result, Is.EquivalentTo(domainObjects));

      Assert.That(productReview1.State.IsNew, Is.True);
      Assert.That(productReview1.State.IsDataChanged, Is.False);
      Assert.That(productReview1.State.IsRelationChanged, Is.False);
      var productReview1ProductPropertyAccessor = new PropertyIndexer(productReview1)[typeof(ProductReview), nameof(ProductReview.Product)];
      Assert.That(productReview1ProductPropertyAccessor.HasChanged, Is.False);
      Assert.That(productReview1ProductPropertyAccessor.GetValue<Product>(), Is.EqualTo(product));
      Assert.That(productReview1ProductPropertyAccessor.GetOriginalValue<Product>(), Is.EqualTo(product));
      Assert.That(productReview1ProductPropertyAccessor.GetRelatedObjectID(), Is.EqualTo(product.ID));
      Assert.That(productReview1ProductPropertyAccessor.GetOriginalRelatedObjectID(), Is.EqualTo(product.ID));

      Assert.That(productReview2.State.IsNew, Is.True);
      Assert.That(productReview2.State.IsDataChanged, Is.False);
      Assert.That(productReview2.State.IsRelationChanged, Is.False);

      Assert.That(product.State.IsNew, Is.True);
      Assert.That(product.State.IsDataChanged, Is.False);
      Assert.That(product.State.IsRelationChanged, Is.False);
      var productReviewsPropertyAccessor = new PropertyIndexer(product)[typeof(Product), nameof(Product.Reviews)];
      Assert.That(productReviewsPropertyAccessor.HasChanged, Is.False);
      Assert.That(productReviewsPropertyAccessor.GetValue<IObjectList<ProductReview>>(), Is.EquivalentTo(new[] { productReview1, productReview2 }));
      Assert.That(productReviewsPropertyAccessor.GetOriginalValue<IObjectList<ProductReview>>(), Is.EquivalentTo(new[] { productReview1, productReview2 }));
    }

    [Test]
    public void TryApplyingCurrentStateAsInitialValue_WithIncompleteDataSet_SkipsObjectsWithIncompleteRelationsGraphInDataSet_ReturnsOnlySuccessfullyUpdatedDomainObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 14;
      var orderItem1 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem1.Order = order;
      var orderItem2 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem2.Order = order;
      var orderTicket = (OrderTicket)LifetimeService.NewObject(_clientTransaction, typeof(OrderTicket), ParamList.Empty);
      orderTicket.Order = order;

      var customer = (Customer)LifetimeService.NewObject(_clientTransaction, typeof(Customer), ParamList.Empty);
      customer.Name = "Alice";

      var domainObjects = new DomainObject[] { order, orderItem2, orderTicket, customer };
      Assert.That(domainObjects, Has.No.Member(orderItem1));
      var result = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects.Select(obj => obj.ID));

      Assert.That(result, Is.EquivalentTo(new[] { customer }));

      Assert.That(orderItem1.State.IsNew, Is.True);
      Assert.That(orderItem1.State.IsDataChanged, Is.True);
      Assert.That(orderItem1.State.IsRelationChanged, Is.True);

      Assert.That(orderItem2.State.IsNew, Is.True);
      Assert.That(orderItem2.State.IsDataChanged, Is.True);
      Assert.That(orderItem2.State.IsRelationChanged, Is.True);

      Assert.That(orderTicket.State.IsNew, Is.True);
      Assert.That(orderTicket.State.IsDataChanged, Is.True);
      Assert.That(orderTicket.State.IsRelationChanged, Is.True);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.True);
      Assert.That(order.State.IsRelationChanged, Is.True);

      Assert.That(customer.State.IsNew, Is.True);
      Assert.That(customer.State.IsDataChanged, Is.False);
      Assert.That(customer.State.IsRelationChanged, Is.False);
    }

    [Test]
    public void
        TryApplyingCurrentStateAsInitialValue_WithOneToOneRelationChangedTwice_SkipsObjectsWithOriginalVirtualSideValueMissingInDataSet_ReturnsOnlySuccessfullyUpdatedDomainObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 14;
      var orderTicket1 = (OrderTicket)LifetimeService.NewObject(_clientTransaction, typeof(OrderTicket), ParamList.Empty);
      orderTicket1.Order = order;

      var domainObjects1 = new DomainObject[] { order, orderTicket1 };
      var result1 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects1.Select(obj => obj.ID));

      Assert.That(result1, Is.EquivalentTo(domainObjects1));

      var orderTicket2 = (OrderTicket)LifetimeService.NewObject(_clientTransaction, typeof(OrderTicket), ParamList.Empty);
      orderTicket2.Order = order;

      var domainObjects2 = new DomainObject[] { order, orderTicket2 };
      Assert.That(domainObjects2, Has.No.Member(orderTicket1));
      var result2 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects2.Select(obj => obj.ID));
      Assert.That(result2, Is.Empty);

      Assert.That(orderTicket1.State.IsNew, Is.True);
      Assert.That(orderTicket1.State.IsDataChanged, Is.True);
      Assert.That(orderTicket1.State.IsRelationChanged, Is.True);

      Assert.That(orderTicket2.State.IsNew, Is.True);
      Assert.That(orderTicket2.State.IsDataChanged, Is.True);
      Assert.That(orderTicket2.State.IsRelationChanged, Is.True);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.True);
      var orderOrderTicketPropertyAccessor = new PropertyIndexer(order)[typeof(Order), nameof(Order.OrderTicket)];
      Assert.That(orderOrderTicketPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderOrderTicketPropertyAccessor.GetValue<OrderTicket>(), Is.EqualTo(orderTicket2));
      Assert.That(orderOrderTicketPropertyAccessor.GetOriginalValue<OrderTicket>(), Is.EqualTo(orderTicket1));
    }

    [Test]
    public void
        TryApplyingCurrentStateAsInitialValue_WithOneToOneRelationChangedTwice_SkipsObjectsWithCurrentVirtualSideValueMissingInDataSet_ReturnsOnlySuccessfullyUpdatedDomainObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 14;
      var orderTicket1 = (OrderTicket)LifetimeService.NewObject(_clientTransaction, typeof(OrderTicket), ParamList.Empty);
      orderTicket1.Order = order;

      var domainObjects1 = new DomainObject[] { order, orderTicket1 };
      var result1 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects1.Select(obj => obj.ID));

      Assert.That(result1, Is.EquivalentTo(domainObjects1));

      var orderTicket2 = (OrderTicket)LifetimeService.NewObject(_clientTransaction, typeof(OrderTicket), ParamList.Empty);
      orderTicket2.Order = order;

      var domainObjects2 = new DomainObject[] { order, orderTicket1 };
      Assert.That(domainObjects2, Has.No.Member(orderTicket2));
      var result2 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects2.Select(obj => obj.ID));
      Assert.That(result2, Is.Empty);

      Assert.That(orderTicket1.State.IsNew, Is.True);
      Assert.That(orderTicket1.State.IsDataChanged, Is.True);
      Assert.That(orderTicket1.State.IsRelationChanged, Is.True);

      Assert.That(orderTicket2.State.IsNew, Is.True);
      Assert.That(orderTicket2.State.IsDataChanged, Is.True);
      Assert.That(orderTicket2.State.IsRelationChanged, Is.True);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.True);
      var orderOrderTicketPropertyAccessor = new PropertyIndexer(order)[typeof(Order), nameof(Order.OrderTicket)];
      Assert.That(orderOrderTicketPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderOrderTicketPropertyAccessor.GetValue<OrderTicket>(), Is.EqualTo(orderTicket2));
      Assert.That(orderOrderTicketPropertyAccessor.GetOriginalValue<OrderTicket>(), Is.EqualTo(orderTicket1));
    }

    [Test]
    public void
        TryApplyingCurrentStateAsInitialValue_WithOneToOneRelationChangedTwice_SkipsObjectsWithVirtualSideValueMissingInDataSet_ReturnsOnlySuccessfullyUpdatedDomainObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 14;
      var orderTicket1 = (OrderTicket)LifetimeService.NewObject(_clientTransaction, typeof(OrderTicket), ParamList.Empty);
      orderTicket1.Order = order;

      var domainObjects1 = new DomainObject[] { order, orderTicket1 };
      var result1 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects1.Select(obj => obj.ID));

      Assert.That(result1, Is.EquivalentTo(domainObjects1));

      var orderTicket2 = (OrderTicket)LifetimeService.NewObject(_clientTransaction, typeof(OrderTicket), ParamList.Empty);
      orderTicket2.Order = order;

      var domainObjects2 = new DomainObject[] { order };
      Assert.That(domainObjects2, Has.No.Member(orderTicket1));
      Assert.That(domainObjects2, Has.No.Member(orderTicket2));
      var result2 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects2.Select(obj => obj.ID));
      Assert.That(result2, Is.Empty);

      Assert.That(orderTicket1.State.IsNew, Is.True);
      Assert.That(orderTicket1.State.IsDataChanged, Is.True);
      Assert.That(orderTicket1.State.IsRelationChanged, Is.True);

      Assert.That(orderTicket2.State.IsNew, Is.True);
      Assert.That(orderTicket2.State.IsDataChanged, Is.True);
      Assert.That(orderTicket2.State.IsRelationChanged, Is.True);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.True);
      var orderOrderTicketPropertyAccessor = new PropertyIndexer(order)[typeof(Order), nameof(Order.OrderTicket)];
      Assert.That(orderOrderTicketPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderOrderTicketPropertyAccessor.GetValue<OrderTicket>(), Is.EqualTo(orderTicket2));
      Assert.That(orderOrderTicketPropertyAccessor.GetOriginalValue<OrderTicket>(), Is.EqualTo(orderTicket1));
    }


    [Test]
    public void
        TryApplyingCurrentStateAsInitialValue_WithDomainObjectCollectionRelationChangedTwice_SkipsObjectsWithOriginalVirtualSideValueMissingInDataSet_ReturnsOnlySuccessfullyUpdatedDomainObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 14;
      var orderItem1 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem1.Order = order;
      var orderItem2 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem2.Order = order;

      var domainObjects1 = new DomainObject[] { order, orderItem1, orderItem2 };
      var result1 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects1.Select(obj => obj.ID));
      Assert.That(result1, Is.EquivalentTo(domainObjects1));

      orderItem2.Order = null;
      var orderItem3 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem3.Order = order;

      var domainObjects2 = new DomainObject[] { order, orderItem1, orderItem3 };
      Assert.That(domainObjects2, Has.No.Member(orderItem2));
      var result2 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects2.Select(obj => obj.ID));
      Assert.That(result2, Is.EquivalentTo(new DomainObject[] { orderItem1 }));

      Assert.That(orderItem1.State.IsNew, Is.True);
      Assert.That(orderItem1.State.IsDataChanged, Is.False);
      Assert.That(orderItem1.State.IsRelationChanged, Is.False);
      var orderItem1OrderPropertyAccessor = new PropertyIndexer(orderItem1)[typeof(OrderItem), nameof(OrderItem.Order)];
      Assert.That(orderItem1OrderPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderItem1OrderPropertyAccessor.GetValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem1OrderPropertyAccessor.GetOriginalValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem1OrderPropertyAccessor.GetRelatedObjectID(), Is.EqualTo(order.ID));
      Assert.That(orderItem1OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.EqualTo(order.ID));

      Assert.That(orderItem2.State.IsNew, Is.True);
      Assert.That(orderItem2.State.IsDataChanged, Is.True);
      Assert.That(orderItem2.State.IsRelationChanged, Is.True);
      var orderItem2OrderPropertyAccessor = new PropertyIndexer(orderItem2)[typeof(OrderItem), nameof(OrderItem.Order)];
      Assert.That(orderItem2OrderPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderItem2OrderPropertyAccessor.GetValue<Order>(), Is.Null);
      Assert.That(orderItem2OrderPropertyAccessor.GetOriginalValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem2OrderPropertyAccessor.GetRelatedObjectID(), Is.Null);
      Assert.That(orderItem2OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.EqualTo(order.ID));

      Assert.That(orderItem3.State.IsNew, Is.True);
      Assert.That(orderItem3.State.IsDataChanged, Is.True);
      Assert.That(orderItem3.State.IsRelationChanged, Is.True);
      var orderItem3OrderPropertyAccessor = new PropertyIndexer(orderItem3)[typeof(OrderItem), nameof(OrderItem.Order)];
      Assert.That(orderItem3OrderPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderItem3OrderPropertyAccessor.GetValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem3OrderPropertyAccessor.GetOriginalValue<Order>(), Is.Null);
      Assert.That(orderItem3OrderPropertyAccessor.GetRelatedObjectID(), Is.EqualTo(order.ID));
      Assert.That(orderItem3OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.Null);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.True);
      var orderOrderItemsPropertyAccessor = new PropertyIndexer(order)[typeof(Order), nameof(Order.OrderItems)];
      Assert.That(orderOrderItemsPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderOrderItemsPropertyAccessor.GetValue<ObjectList<OrderItem>>(), Is.EqualTo(new[] { orderItem1, orderItem3 }));
      Assert.That(orderOrderItemsPropertyAccessor.GetOriginalValue<ObjectList<OrderItem>>(), Is.EqualTo(new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void
        TryApplyingCurrentStateAsInitialValue_WithDomainObjectCollectionRelationChangedTwice_SkipsObjectsWithCurrentVirtualSideValueMissingInDataSet_ReturnsOnlySuccessfullyUpdatedDomainObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 14;
      var orderItem1 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem1.Order = order;
      var orderItem2 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem2.Order = order;

      var domainObjects1 = new DomainObject[] { order, orderItem1, orderItem2 };
      var result1 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects1.Select(obj => obj.ID));
      Assert.That(result1, Is.EquivalentTo(domainObjects1));

      orderItem2.Order = null;
      var orderItem3 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem3.Order = order;

      var domainObjects2 = new DomainObject[] { order, orderItem1, orderItem2 };
      Assert.That(domainObjects2, Has.No.Member(orderItem3));
      var result2 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects2.Select(obj => obj.ID));
      Assert.That(result2, Is.EquivalentTo(new DomainObject[] { orderItem1 }));

      Assert.That(orderItem1.State.IsNew, Is.True);
      Assert.That(orderItem1.State.IsDataChanged, Is.False);
      Assert.That(orderItem1.State.IsRelationChanged, Is.False);
      var orderItem1OrderPropertyAccessor = new PropertyIndexer(orderItem1)[typeof(OrderItem), nameof(OrderItem.Order)];
      Assert.That(orderItem1OrderPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderItem1OrderPropertyAccessor.GetValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem1OrderPropertyAccessor.GetOriginalValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem1OrderPropertyAccessor.GetRelatedObjectID(), Is.EqualTo(order.ID));
      Assert.That(orderItem1OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.EqualTo(order.ID));

      Assert.That(orderItem2.State.IsNew, Is.True);
      Assert.That(orderItem2.State.IsDataChanged, Is.True);
      Assert.That(orderItem2.State.IsRelationChanged, Is.True);
      var orderItem2OrderPropertyAccessor = new PropertyIndexer(orderItem2)[typeof(OrderItem), nameof(OrderItem.Order)];
      Assert.That(orderItem2OrderPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderItem2OrderPropertyAccessor.GetValue<Order>(), Is.Null);
      Assert.That(orderItem2OrderPropertyAccessor.GetOriginalValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem2OrderPropertyAccessor.GetRelatedObjectID(), Is.Null);
      Assert.That(orderItem2OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.EqualTo(order.ID));

      Assert.That(orderItem3.State.IsNew, Is.True);
      Assert.That(orderItem3.State.IsDataChanged, Is.True);
      Assert.That(orderItem3.State.IsRelationChanged, Is.True);
      var orderItem3OrderPropertyAccessor = new PropertyIndexer(orderItem3)[typeof(OrderItem), nameof(OrderItem.Order)];
      Assert.That(orderItem3OrderPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderItem3OrderPropertyAccessor.GetValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem3OrderPropertyAccessor.GetOriginalValue<Order>(), Is.Null);
      Assert.That(orderItem3OrderPropertyAccessor.GetRelatedObjectID(), Is.EqualTo(order.ID));
      Assert.That(orderItem3OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.Null);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.True);
      var orderOrderItemsPropertyAccessor = new PropertyIndexer(order)[typeof(Order), nameof(Order.OrderItems)];
      Assert.That(orderOrderItemsPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderOrderItemsPropertyAccessor.GetValue<ObjectList<OrderItem>>(), Is.EqualTo(new[] { orderItem1, orderItem3 }));
      Assert.That(orderOrderItemsPropertyAccessor.GetOriginalValue<ObjectList<OrderItem>>(), Is.EqualTo(new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void
        TryApplyingCurrentStateAsInitialValue_WithDomainObjectCollectionRelationChangedTwice_SkipsObjectsWithVirtualSideValueMissingInDataSet_ReturnsOnlySuccessfullyUpdatedDomainObjects ()
    {
      var order = (Order)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      order.OrderNumber = 14;
      var orderItem1 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem1.Order = order;
      var orderItem2 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem2.Order = order;

      var domainObjects1 = new DomainObject[] { order, orderItem1, orderItem2 };
      var result1 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects1.Select(obj => obj.ID));
      Assert.That(result1, Is.EquivalentTo(domainObjects1));

      orderItem2.Order = null;
      var orderItem3 = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem3.Order = order;

      var domainObjects2 = new DomainObject[] { order };
      Assert.That(domainObjects2, Has.No.Member(orderItem1));
      Assert.That(domainObjects2, Has.No.Member(orderItem2));
      Assert.That(domainObjects2, Has.No.Member(orderItem3));
      var result2 = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects2.Select(obj => obj.ID));
      Assert.That(result2, Is.Empty);

      Assert.That(orderItem1.State.IsNew, Is.True);
      Assert.That(orderItem1.State.IsDataChanged, Is.False);
      Assert.That(orderItem1.State.IsRelationChanged, Is.False);
      var orderItem1OrderPropertyAccessor = new PropertyIndexer(orderItem1)[typeof(OrderItem), nameof(OrderItem.Order)];
      Assert.That(orderItem1OrderPropertyAccessor.HasChanged, Is.False);
      Assert.That(orderItem1OrderPropertyAccessor.GetValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem1OrderPropertyAccessor.GetOriginalValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem1OrderPropertyAccessor.GetRelatedObjectID(), Is.EqualTo(order.ID));
      Assert.That(orderItem1OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.EqualTo(order.ID));

      Assert.That(orderItem2.State.IsNew, Is.True);
      Assert.That(orderItem2.State.IsDataChanged, Is.True);
      Assert.That(orderItem2.State.IsRelationChanged, Is.True);
      var orderItem2OrderPropertyAccessor = new PropertyIndexer(orderItem2)[typeof(OrderItem), nameof(OrderItem.Order)];
      Assert.That(orderItem2OrderPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderItem2OrderPropertyAccessor.GetValue<Order>(), Is.Null);
      Assert.That(orderItem2OrderPropertyAccessor.GetOriginalValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem2OrderPropertyAccessor.GetRelatedObjectID(), Is.Null);
      Assert.That(orderItem2OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.EqualTo(order.ID));

      Assert.That(orderItem3.State.IsNew, Is.True);
      Assert.That(orderItem3.State.IsDataChanged, Is.True);
      Assert.That(orderItem3.State.IsRelationChanged, Is.True);
      var orderItem3OrderPropertyAccessor = new PropertyIndexer(orderItem3)[typeof(OrderItem), nameof(OrderItem.Order)];
      Assert.That(orderItem3OrderPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderItem3OrderPropertyAccessor.GetValue<Order>(), Is.EqualTo(order));
      Assert.That(orderItem3OrderPropertyAccessor.GetOriginalValue<Order>(), Is.Null);
      Assert.That(orderItem3OrderPropertyAccessor.GetRelatedObjectID(), Is.EqualTo(order.ID));
      Assert.That(orderItem3OrderPropertyAccessor.GetOriginalRelatedObjectID(), Is.Null);

      Assert.That(order.State.IsNew, Is.True);
      Assert.That(order.State.IsDataChanged, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.True);
      var orderOrderItemsPropertyAccessor = new PropertyIndexer(order)[typeof(Order), nameof(Order.OrderItems)];
      Assert.That(orderOrderItemsPropertyAccessor.HasChanged, Is.True);
      Assert.That(orderOrderItemsPropertyAccessor.GetValue<ObjectList<OrderItem>>(), Is.EqualTo(new[] { orderItem1, orderItem3 }));
      Assert.That(orderOrderItemsPropertyAccessor.GetOriginalValue<ObjectList<OrderItem>>(), Is.EqualTo(new[] { orderItem1, orderItem2 }));
    }

    [Test]
    public void TryApplyingCurrentStateAsInitialValue_WithExistingObjectInDataSet_SkipsObjectsWithIncompleteRelationsGraphInDataSet_ReturnsOnlySuccessfullyUpdatedDomainObjects ()
    {
      var order = (Order)LifetimeService.GetObject(_clientTransaction, DomainObjectIDs.Order1, false);
      order.OrderNumber = 14;
      var orderItem = (OrderItem)LifetimeService.NewObject(_clientTransaction, typeof(OrderItem), ParamList.Empty);
      orderItem.Order = order;

      Assert.That(orderItem.State.IsNew, Is.True);
      Assert.That(orderItem.State.IsDataChanged, Is.True);
      Assert.That(orderItem.State.IsRelationChanged, Is.True);

      Assert.That(order.State.IsNew, Is.False);
      Assert.That(order.State.IsDataChanged, Is.True);
      Assert.That(order.State.IsRelationChanged, Is.True);

      var domainObjects = new DomainObject[] { order, orderItem };
      var result = InitializationService.TryToApplyCurrentStateAsInitialValue(_clientTransaction, domainObjects.Select(obj => obj.ID));

      Assert.That(result, Is.Empty);

      Assert.That(orderItem.State.IsNew, Is.True);
      Assert.That(orderItem.State.IsDataChanged, Is.True);
      Assert.That(orderItem.State.IsRelationChanged, Is.True);

      Assert.That(order.State.IsNew, Is.False);
      Assert.That(order.State.IsDataChanged, Is.True);
      Assert.That(order.State.IsRelationChanged, Is.True);
    }
  }
}
