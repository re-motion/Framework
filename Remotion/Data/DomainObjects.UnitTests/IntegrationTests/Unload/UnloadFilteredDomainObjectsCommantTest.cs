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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadFilteredTest: ClientTransactionBaseTest
  {
    [Test]
    public void UnloadFiltered_WithAllObjects_SingleDomainObject_WithoutChanges ()
    {
      var order = (Order)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Order1, false);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);

      UnloadFiltered(obj => true);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);

      Assert.That(order.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void UnloadFiltered_WithAllObjects_WithOneToOneRelation_WithoutChanges ()
    {
      var order = (Order)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Order1, false);
      order.OrderTicket.EnsureDataAvailable();

      var orderTicket = order.OrderTicket;

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);

      UnloadFiltered(obj => true);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);

      Assert.That(order.State.IsNotLoadedYet, Is.True);
      Assert.That(orderTicket.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void UnloadFiltered_WithAllObjects_WithOneToOneRelation_WithChanges ()
    {
      var order1 = (Order)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Order1, false);
      var order2 = (Order)LifetimeService.NewObject(TestableClientTransaction,typeof(Order), ParamList.Empty);
      order1.OrderTicket.EnsureDataAvailable();

      var orderTicket = order1.OrderTicket;
      orderTicket.Order = order2;

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);

      UnloadFiltered(obj => true);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);

      Assert.That(order1.State.IsNotLoadedYet, Is.True);
      Assert.That(order1.State.IsRelationChanged, Is.False);
      Assert.That(order2.State.IsInvalid, Is.True);
      Assert.That(order2.State.IsRelationChanged, Is.False);
      Assert.That(orderTicket.State.IsNotLoadedYet, Is.True);
      Assert.That(orderTicket.State.IsRelationChanged, Is.False);
    }

    [Test]
    public void UnloadFiltered_WithAllObjects_WithOneToOneRelation_WithChanges_IncludeOnlyForeignKeySide_ThrowsInvalidOperationException ()
    {
      var order1 = (Order)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Order1, false);
      var order2 = (Order)LifetimeService.NewObject(TestableClientTransaction,typeof(Order), ParamList.Empty);
      order1.OrderTicket.EnsureDataAvailable();

      var orderTicket = order1.OrderTicket;
      orderTicket.Order = order2;

      Assert.That(order1.State.IsChanged, Is.True);
      Assert.That(order1.State.IsRelationChanged, Is.True);
      Assert.That(order2.State.IsNew, Is.True);
      Assert.That(order2.State.IsRelationChanged, Is.True);
      Assert.That(orderTicket.State.IsChanged, Is.True);
      Assert.That(orderTicket.State.IsDataChanged, Is.True);
      Assert.That(orderTicket.State.IsRelationChanged, Is.True);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);

      Assert.That(
          () => UnloadFiltered(obj => obj == orderTicket),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot unload the following relation endpoints because the associated domain objects have not been included in the data set.\r\n"
                  + orderTicket.ID + "/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order,\r\n"
                  + order2.ID + "/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void UnloadFiltered_WithAllObjects_WithOneToOneRelation_WithChanges_IncludeVirtualSide ()
    {
      var order1 = (Order)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Order1, false);
      var order2 = (Order)LifetimeService.NewObject(TestableClientTransaction,typeof(Order), ParamList.Empty);
      order1.OrderTicket.EnsureDataAvailable();

      var orderTicket = order1.OrderTicket;
      orderTicket.Order = order2;

      Assert.That(order1.State.IsChanged, Is.True);
      Assert.That(order1.State.IsRelationChanged, Is.True);
      Assert.That(order2.State.IsNew, Is.True);
      Assert.That(order2.State.IsRelationChanged, Is.True);
      Assert.That(orderTicket.State.IsChanged, Is.True);
      Assert.That(orderTicket.State.IsDataChanged, Is.True);
      Assert.That(orderTicket.State.IsRelationChanged, Is.True);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);
      var orderTicketEndPoints = TestableClientTransaction.DataManager.RelationEndPoints.Where(ep => ep.Definition.ClassDefinition.ClassType == typeof(OrderTicket)).ToArray();

      UnloadFiltered(obj => obj == order1 || obj == order2);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.DataContainers.Select(dc => dc.DomainObject), Has.No.AnyOf(orderTicket));
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints.Select(ep => ep.Definition), Has.No.AnyOf(orderTicketEndPoints));

      Assert.That(order1.State.IsUnchanged, Is.True);
      Assert.That(order1.State.IsRelationChanged, Is.False);
      Assert.That(order2.State.IsNew, Is.True);
      Assert.That(order2.State.IsRelationChanged, Is.False);
      Assert.That(orderTicket.State.IsNotLoadedYet, Is.True);
      Assert.That(orderTicket.State.IsRelationChanged, Is.False);

      orderTicket.EnsureDataAvailable();
      Assert.That(orderTicket.Order, Is.SameAs(order1));
      Assert.That(order2.OrderTicket, Is.Null);
    }

    [Test]
    public void UnloadFiltered_WithAllObjects_WithDomainObjectCollectionRelation_WithoutChanges ()
    {
      var order = (Order)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Order1, false);
      order.OrderItems.EnsureDataComplete();

      var orderItem = order.OrderItems.First();

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);

      UnloadFiltered(obj => true);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);

      Assert.That(order.State.IsNotLoadedYet, Is.True);
      Assert.That(order.OrderItems.IsDataComplete, Is.False);
      Assert.That(orderItem.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void UnloadFiltered_WithAllObjects_WithDomainObjectCollectionRelation_WithChanges ()
    {
      var order = (Order)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Order1, false);
      order.OrderItems.EnsureDataComplete();

      var orderItem = order.OrderItems.First();
      orderItem.Order = null;

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);

      UnloadFiltered(obj => true);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);

      Assert.That(order.State.IsNotLoadedYet, Is.True);
      Assert.That(order.OrderItems.IsDataComplete, Is.False);
      Assert.That(order.State.IsRelationChanged, Is.False);
      Assert.That(orderItem.State.IsNotLoadedYet, Is.True);
      Assert.That(orderItem.State.IsRelationChanged, Is.False);
    }

    [Test]
    public void UnloadFiltered_WithAllObjects_WithVirtualCollectionRelation_WithoutChanges ()
    {
      var product = (Product)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Product1, false);
      product.Reviews.EnsureDataComplete();

      var productReview = product.Reviews.First();

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);

      UnloadFiltered(obj => true);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);

      Assert.That(product.State.IsNotLoadedYet, Is.True);
      Assert.That(product.Reviews.IsDataComplete, Is.False);
      Assert.That(productReview.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void UnloadFiltered_WithAllObjects_WithVirtualCollectionRelation_WithChanges ()
    {
      var product = (Product)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Product1, false);
      product.Reviews.EnsureDataComplete();

      var productReview = product.Reviews.First();
      productReview.Product = null;

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);

      UnloadFiltered(obj => true);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);

      Assert.That(product.State.IsNotLoadedYet, Is.True);
      Assert.That(product.Reviews.IsDataComplete, Is.False);
      Assert.That(product.State.IsRelationChanged, Is.False);
      Assert.That(productReview.State.IsNotLoadedYet, Is.True);
      Assert.That(productReview.State.IsRelationChanged, Is.False);
    }

    [Test]
    public void UnloadFiltered_WithAllObjects_WithAnonymousRelation_WithoutChanges ()
    {
      var location = (Location)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Location1, false);

      var client = location.Client;
      client.EnsureDataAvailable();

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);

      UnloadFiltered(obj => true);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);

      Assert.That(location.State.IsNotLoadedYet, Is.True);
      Assert.That(client.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void UnloadFiltered_WithAllObjects_WithAnonymousRelation_WithChanges ()
    {
      var location = (Location)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Location1, false);

      var client = location.Client;
      client.EnsureDataAvailable();
      location.Client = null;

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);

      UnloadFiltered(obj => true);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);

      Assert.That(location.State.IsNotLoadedYet, Is.True);
      Assert.That(location.State.IsRelationChanged, Is.False);
      Assert.That(client.State.IsNotLoadedYet, Is.True);
      Assert.That(client.State.IsRelationChanged, Is.False);
    }

    [Test]
    public void UnloadFiltered_WithSomeObjects_WithAnonymousRelation_WithChanges_IncludeOnlyForeignKeySide ()
    {
      var location = (Location)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Location1, false);

      var client = location.Client;
      client.EnsureDataAvailable();
      location.Client = null;

      Assert.That(location.State.IsChanged, Is.True);
      Assert.That(location.State.IsRelationChanged, Is.True);
      Assert.That(client.State.IsUnchanged, Is.True);
      Assert.That(client.State.IsRelationChanged, Is.False);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);
      var locationEndPoints = TestableClientTransaction.DataManager.RelationEndPoints.Where(ep => ep.Definition.ClassDefinition.ClassType == typeof(Location)).ToArray();

      UnloadFiltered(obj => obj == location);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.DataContainers.Select(dc => dc.DomainObject), Has.No.AnyOf(location));
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints.Select(ep => ep.Definition), Has.No.AnyOf(locationEndPoints));

      Assert.That(location.State.IsNotLoadedYet, Is.True);
      Assert.That(location.State.IsRelationChanged, Is.False);
      Assert.That(client.State.IsUnchanged, Is.True);
      Assert.That(client.State.IsRelationChanged, Is.False);

      location.EnsureDataAvailable();
      Assert.That(location.Client, Is.SameAs(client));
    }

    [Test]
    public void UnloadFiltered_WithSomeObjects_WithAnonymousRelation_WithChanges_IncludeOnlyAnonymousSide ()
    {
      var location = (Location)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Location1, false);

      var client = location.Client;
      client.EnsureDataAvailable();
      location.Client = null;

      Assert.That(location.State.IsChanged, Is.True);
      Assert.That(location.State.IsDataChanged, Is.True);
      Assert.That(location.State.IsRelationChanged, Is.True);
      Assert.That(client.State.IsUnchanged, Is.True);
      Assert.That(client.State.IsRelationChanged, Is.False);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);
      var clientEndPoints = TestableClientTransaction.DataManager.RelationEndPoints.Where(ep => ep.Definition.ClassDefinition.ClassType == typeof(Client)).ToArray();

      UnloadFiltered(obj => obj == client);

      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.DataContainers.Select(dc => dc.DomainObject), Has.No.AnyOf(client));
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Not.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints.Select(ep => ep.Definition), Has.No.AnyOf(clientEndPoints));

      Assert.That(location.State.IsChanged, Is.True);
      Assert.That(location.State.IsDataChanged, Is.True);
      Assert.That(location.State.IsRelationChanged, Is.True);
      Assert.That(client.State.IsNotLoadedYet, Is.True);
      Assert.That(client.State.IsRelationChanged, Is.False);

      var client2 = (Client)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Client2, false);
      Assert.That(client2.ID, Is.Not.EqualTo(client.ID));
      location.Client = client2;
      Assert.That(location.Client, Is.EqualTo(client2));
    }


    private void UnloadFiltered (Predicate<DomainObject> domainObjectFilter)
    {
      UnloadService.UnloadFiltered(TestableClientTransaction, domainObjectFilter);
    }
  }
}
