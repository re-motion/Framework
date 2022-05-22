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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionGetRelationsTest : ClientTransactionBaseTest
  {
    private ClientTransactionEventReceiver _eventReceiver;

    public override void SetUp ()
    {
      base.SetUp();

      _eventReceiver = new ClientTransactionEventReceiver(TestableClientTransaction);
    }

    [Test]
    public void GetRelatedObjectForAlreadyLoadedObjects ()
    {
      IDomainObject order = TestableClientTransaction.GetObject(DomainObjectIDs.Order1, false);
      IDomainObject orderTicket = TestableClientTransaction.GetObject(DomainObjectIDs.OrderTicket1, false);

      _eventReceiver.Clear();

      Assert.That(
          TestableClientTransaction.GetRelatedObject(
              RelationEndPointID.Create(order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket")),
          Is.SameAs(orderTicket));

      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));

      Assert.That(
          TestableClientTransaction.GetRelatedObject(
              RelationEndPointID.Create(orderTicket.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order")),
          Is.SameAs(order));
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      IDomainObject orderTicket = TestableClientTransaction.GetObject(DomainObjectIDs.OrderTicket1, false);
      _eventReceiver.Clear();
      IDomainObject order =
          TestableClientTransaction.GetRelatedObject(RelationEndPointID.Create(orderTicket.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));

      Assert.That(order, Is.Not.Null);
      Assert.That(order.ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetRelatedObjectOverVirtualEndPoint ()
    {
      IDomainObject order = TestableClientTransaction.GetObject(DomainObjectIDs.Order1, false);
      _eventReceiver.Clear();

      IDomainObject orderTicket = TestableClientTransaction.GetRelatedObject(
          RelationEndPointID.Create(order.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));

      Assert.That(orderTicket, Is.Not.Null);
      Assert.That(orderTicket.ID, Is.EqualTo(DomainObjectIDs.OrderTicket1));
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(1));

      var domainObjects = _eventReceiver.LoadedDomainObjectLists[0];
      Assert.That(domainObjects.Count, Is.EqualTo(1));
      Assert.That(domainObjects[0], Is.SameAs(orderTicket));
    }

    [Test]
    public void GetOptionalRelatedObject ()
    {
      var id = new ObjectID("ClassWithValidRelations", new Guid("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      IDomainObject classWithValidRelation = TestableClientTransaction.GetObject(id, false);
      _eventReceiver.Clear();

      Assert.That(
          TestableClientTransaction.GetRelatedObject(
              RelationEndPointID.Create(
                  classWithValidRelation.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional")),
          Is.Null);

      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetOptionalRelatedObjectOverVirtualEndPoint ()
    {
      var id = new ObjectID("ClassWithGuidKey", new Guid("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      IDomainObject classWithGuidKey = TestableClientTransaction.GetObject(id, false);
      _eventReceiver.Clear();

      Assert.That(
          TestableClientTransaction.GetRelatedObject(
              RelationEndPointID.Create(
                  classWithGuidKey.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional")),
          Is.Null);

      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetOptionalRelatedObjectTwice ()
    {
      var id = new ObjectID("ClassWithValidRelations", new Guid("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      CountingObjectLoaderDecorator decorator = null;
      var clientTransaction =
          ClientTransactionObjectMother.CreateTransactionWithObjectLoaderDecorator<TestableClientTransaction>(
              loader => decorator ?? (decorator = new CountingObjectLoaderDecorator(loader)));

      IDomainObject classWithValidRelation = clientTransaction.GetObject(id, false);
      Assert.That(decorator.NumberOfCallsToLoadObject, Is.EqualTo(1));
      Assert.That(decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo(0));

      Assert.That(
          clientTransaction.GetRelatedObject(
              RelationEndPointID.Create(
                  classWithValidRelation.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional")),
          Is.Null);

      Assert.That(decorator.NumberOfCallsToLoadObject, Is.EqualTo(1));
      Assert.That(decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo(0));

      clientTransaction.GetRelatedObject(
          RelationEndPointID.Create(classWithValidRelation.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional"));

      Assert.That(decorator.NumberOfCallsToLoadObject, Is.EqualTo(1));
      Assert.That(decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo(0));
    }

    [Test]
    public void GetOptionalRelatedObjectOverVirtualEndPointTwice ()
    {
      var id = new ObjectID("ClassWithGuidKey", new Guid("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      CountingObjectLoaderDecorator decorator = null;
      var clientTransactionMock = ClientTransactionObjectMother.CreateTransactionWithObjectLoaderDecorator<TestableClientTransaction>(
        loader => decorator ?? (decorator = new CountingObjectLoaderDecorator(loader)));

      IDomainObject classWithGuidKey = clientTransactionMock.GetObject(id, false);
      Assert.That(decorator.NumberOfCallsToLoadObject, Is.EqualTo(1));
      Assert.That(decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo(0));

      Assert.That(
          clientTransactionMock.GetRelatedObject(
              RelationEndPointID.Create(
                  classWithGuidKey.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional")),
          Is.Null);

      Assert.That(decorator.NumberOfCallsToLoadObject, Is.EqualTo(1));
      Assert.That(decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo(1));

      clientTransactionMock.GetRelatedObject(
          RelationEndPointID.Create(classWithGuidKey.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional"));

      Assert.That(decorator.NumberOfCallsToLoadObject, Is.EqualTo(1));
      Assert.That(decorator.NumberOfCallsToLoadRelatedObject, Is.EqualTo(1));
    }

    [Test]
    public void GetRelatedObjectWithInheritance ()
    {
      IDomainObject expectedCeo = TestableClientTransaction.GetObject(DomainObjectIDs.Ceo6, false);
      IDomainObject partner = TestableClientTransaction.GetObject(DomainObjectIDs.Partner1, false);

      IDomainObject actualCeo = TestableClientTransaction.GetRelatedObject(RelationEndPointID.Create(partner.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo"));
      Assert.That(actualCeo, Is.SameAs(expectedCeo));
    }

    [Test]
    public void GetRelatedObjects_DomainObjectCollection ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      _eventReceiver.Clear();

      IReadOnlyList<IDomainObject> orders = TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(customer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.That(orders, Is.Not.Null);
      Assert.That(orders, Is.TypeOf<OrderCollection>());
      Assert.That(((OrderCollection)orders).Count, Is.EqualTo(2));

      var domainObjects = _eventReceiver.LoadedDomainObjectLists[0];
      Assert.That(domainObjects.Count, Is.EqualTo(2));
    }

    [Test]
    public void GetRelatedObjects_VirtualCollection ()
    {
      Product product = DomainObjectIDs.Product1.GetObject<Product>();
      _eventReceiver.Clear();

      IReadOnlyList<IDomainObject> productReviews = TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(product.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));

      Assert.That(productReviews, Is.Not.Null);
      Assert.That(productReviews, Is.TypeOf<VirtualObjectList<ProductReview>>());
      Assert.That(((IObjectList<ProductReview>)productReviews).Count, Is.EqualTo(3));

      var domainObjects = _eventReceiver.LoadedDomainObjectLists[0];
      Assert.That(domainObjects.Count, Is.EqualTo(3));
    }

    [Test]
    public void GetRelatedObjectsTwice_DomainObjectCollection ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      _eventReceiver.Clear();

      DomainObjectCollection orders1 = (DomainObjectCollection)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(customer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      DomainObjectCollection orders2 = (DomainObjectCollection)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(customer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.That(ReferenceEquals(orders1, orders2), Is.True);

      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetRelatedObjectsTwice_VirtualCollection ()
    {
      Product product = DomainObjectIDs.Product1.GetObject<Product>();
      _eventReceiver.Clear();

      IObjectList<IDomainObject> productReviews1 = (IObjectList<IDomainObject>)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(product.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));

      IObjectList<IDomainObject> productReviews2 = (IObjectList<IDomainObject>)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(product.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));

      Assert.That(ReferenceEquals(productReviews1, productReviews2), Is.True);

      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetRelatedObjectsWithAlreadyLoadedObject_DomainObjectCollection ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      _eventReceiver.Clear();

      DomainObjectCollection orders = (DomainObjectCollection)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(customer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      orders.EnsureDataComplete();
      Assert.That(orders.Count, Is.EqualTo(2));
      Assert.That(orders[DomainObjectIDs.Order1], Is.SameAs(order));
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(1));
      Assert.That(_eventReceiver.LoadedDomainObjectLists[0].Count, Is.EqualTo(1));
    }

    [Test]
    public void GetRelatedObjectsWithAlreadyLoadedObject_VirtualCollection ()
    {
      Product product = DomainObjectIDs.Product1.GetObject<Product>();
      ProductReview productReview = DomainObjectIDs.ProductReview2.GetObject<ProductReview>();
      _eventReceiver.Clear();

      IObjectList<IDomainObject> productReviews = (IObjectList<IDomainObject>)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(product.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));

      productReviews.EnsureDataComplete();
      Assert.That(productReviews.Count, Is.EqualTo(3));
      Assert.That(productReviews.GetObject(DomainObjectIDs.ProductReview2), Is.SameAs(productReview));
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(1));
      Assert.That(_eventReceiver.LoadedDomainObjectLists[0].Count, Is.EqualTo(2));
    }

    [Test]
    public void LoadedEventDoesNotFireWithEmptyDomainObjectCollection ()
    {
      Customer customer = DomainObjectIDs.Customer2.GetObject<Customer>();
      _eventReceiver.Clear();

      DomainObjectCollection orders = (DomainObjectCollection)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(customer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.That(orders, Is.Not.Null);
      Assert.That(orders, Is.Empty);
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));
    }

    [Test]
    public void LoadedEventDoesNotFireWithEmptyVirtualCollection ()
    {
      Person person = DomainObjectIDs.Person4.GetObject<Person>();
      _eventReceiver.Clear();

      IObjectList<ProductReview> productReviews = (IObjectList<ProductReview>)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(person.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Person.Reviews"));

      Assert.That(productReviews, Is.Not.Null);
      Assert.That(productReviews, Is.Empty);
      Assert.That(_eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad_DomainObjectCollection ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      DomainObjectCollection orders = (DomainObjectCollection)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(customer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Order orderFromClientTransaction = DomainObjectIDs.Order1.GetObject<Order>();
      Order orderFromCollection = (Order)orders[DomainObjectIDs.Order1];

      Assert.That(orderFromClientTransaction, Is.SameAs(orderFromCollection));
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad_VirtualCollection ()
    {
      Product product = DomainObjectIDs.Product1.GetObject<Product>();

      IObjectList<ProductReview> productReviews = (IObjectList<ProductReview>)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(product.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));

      ProductReview productReviewFromClientTransaction = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
      ProductReview productReviewFromCollection = productReviews.GetObject(DomainObjectIDs.ProductReview1);

      Assert.That(productReviewFromClientTransaction, Is.SameAs(productReviewFromCollection));
    }

    [Test]
    public void GetRelatedObjectsAndNavigateBack ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      DomainObjectCollection orders = (DomainObjectCollection)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(customer.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.That(
          TestableClientTransaction.GetRelatedObject(
              RelationEndPointID.Create(orders[0].ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer")),
          Is.SameAs(customer));
    }

    [Test]
    public void GetRelatedObjectsWithInheritance ()
    {
      IDomainObject industrialSector = TestableClientTransaction.GetObject(DomainObjectIDs.IndustrialSector2, false);
      IDomainObject expectedPartner = TestableClientTransaction.GetObject(DomainObjectIDs.Partner2, false);

      DomainObjectCollection companies = (DomainObjectCollection)TestableClientTransaction.GetRelatedObjects(
          RelationEndPointID.Create(industrialSector.ID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"));

      Assert.That(companies[DomainObjectIDs.Partner2], Is.SameAs(expectedPartner));
    }
  }
}
