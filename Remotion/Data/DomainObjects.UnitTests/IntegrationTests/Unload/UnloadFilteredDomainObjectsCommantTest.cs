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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadFilteredTest: ClientTransactionBaseTest
  {
    [Test]
    public void UnloadAllObjects_SingleDomainObject ()
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
    public void UnloadAllObjects_WithOneToOneRelation ()
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
    public void UnloadAllObjects_WithDomainObjectCollectionRelation ()
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
    public void UnloadAllObjects_WithVirtualCollectionRelation ()
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


    private void UnloadFiltered (Predicate<DomainObject> domainObjectFilter)
    {
      var unloadCommand = CreateUnloadCommand(domainObjectFilter);
      unloadCommand.Begin();
      unloadCommand.Perform();
      unloadCommand.End();
    }

    private UnloadFilteredDomainObjectsCommand CreateUnloadCommand (Predicate<DomainObject> domainObjectFilter)
    {
      var invalidDomainObjectManager = (IInvalidDomainObjectManager)PrivateInvoke.GetNonPublicField(TestableClientTransaction.DataManager, "_invalidDomainObjectManager");
      Assertion.IsNotNull(invalidDomainObjectManager, "DataManager._invalidDomainObjectManager not found or null");

      return new UnloadFilteredDomainObjectsCommand(
          (DataContainerMap)TestableClientTransaction.DataManager.DataContainers,
          invalidDomainObjectManager,
          (RelationEndPointMap)TestableClientTransaction.DataManager.RelationEndPoints,
          TestableClientTransaction.DataManager.TransactionEventSink,
          domainObjectFilter);
    }

  }
}
