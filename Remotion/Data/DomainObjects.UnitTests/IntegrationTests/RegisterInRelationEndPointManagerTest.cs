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

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class RegisterInRelationEndPointManagerTest : ClientTransactionBaseTest
  {
    private RelationEndPointManager _relationEndPointManager;

    public override void SetUp ()
    {
      base.SetUp();

      _relationEndPointManager = (RelationEndPointManager)DataManagerTestHelper.GetRelationEndPointManager(TestableClientTransaction.DataManager);
    }

    [Test]
    public void DataContainerWithNoRelation ()
    {
      var container = TestDataContainerObjectMother.CreateClassWithAllDataTypes1DataContainer();
      _relationEndPointManager.RegisterEndPointsForDataContainer(container);

      Assert.That(_relationEndPointManager.RelationEndPoints.Count, Is.EqualTo(0));
    }

    [Test]
    public void OrderTicket ()
    {
      var orderTicketContainer = TestDataContainerObjectMother.CreateOrderTicket1DataContainer();
      _relationEndPointManager.RegisterEndPointsForDataContainer(orderTicketContainer);

      Assert.That(_relationEndPointManager.RelationEndPoints.Count, Is.EqualTo(2), "Count");

      var expectedEndPointIDForOrderTicket = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1,
                                                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");

      Assert.That(
          _relationEndPointManager.GetRelationEndPointWithoutLoading(expectedEndPointIDForOrderTicket).ID,
          Is.EqualTo(expectedEndPointIDForOrderTicket),
          "RelationEndPointID for OrderTicket");

      Assert.That(
          ((IObjectEndPoint)_relationEndPointManager.GetRelationEndPointWithoutLoading(expectedEndPointIDForOrderTicket)).OppositeObjectID,
          Is.EqualTo(DomainObjectIDs.Order1),
          "OppositeObjectID for OrderTicket");

      var expectedEndPointIDForOrder = RelationEndPointID.Create(DomainObjectIDs.Order1,
                                              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

      Assert.That(
          _relationEndPointManager.GetRelationEndPointWithoutLoading(expectedEndPointIDForOrder).ID,
          Is.EqualTo(expectedEndPointIDForOrder),
          "RelationEndPointID for Order");

      Assert.That(
          ((IObjectEndPoint)_relationEndPointManager.GetRelationEndPointWithoutLoading(expectedEndPointIDForOrder)).OppositeObjectID,
          Is.EqualTo(DomainObjectIDs.OrderTicket1),
          "OppositeObjectID for Order");
    }

    [Test]
    public void VirtualEndPoint ()
    {
      DataContainer container = TestDataContainerObjectMother.CreateClassWithGuidKeyDataContainer();
      _relationEndPointManager.RegisterEndPointsForDataContainer(container);

      Assert.That(_relationEndPointManager.RelationEndPoints.Count, Is.EqualTo(0));
    }

    [Test]
    public void DerivedDataContainer ()
    {
      DataContainer distributorContainer = TestDataContainerObjectMother.CreateDistributor2DataContainer();
      _relationEndPointManager.RegisterEndPointsForDataContainer(distributorContainer);

      Assert.That(_relationEndPointManager.RelationEndPoints.Count, Is.EqualTo(4));
    }

    [Test]
    public void DataContainerWithOneToManyRelation ()
    {
      DataContainer orderContainer = TestDataContainerObjectMother.CreateOrder1DataContainer();

      _relationEndPointManager.RegisterEndPointsForDataContainer(orderContainer);

      Assert.That(_relationEndPointManager.RelationEndPoints.Count, Is.EqualTo(4), "Count");

      var customerEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
      var customerEndPoint = (IObjectEndPoint)_relationEndPointManager.GetRelationEndPointWithoutLoading(customerEndPointID);
      Assert.That(customerEndPoint.OppositeObjectID, Is.EqualTo(DomainObjectIDs.Customer1));

      var customerOrdersEndPointID = RelationEndPointID.Create(
          DomainObjectIDs.Customer1,
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
      var customerOrdersEndPoint = _relationEndPointManager.GetRelationEndPointWithoutLoading(customerOrdersEndPointID);
      Assert.That(customerOrdersEndPoint, Is.Not.Null);

      var officialEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official");
      var officialEndPoint = (IObjectEndPoint)_relationEndPointManager.GetRelationEndPointWithoutLoading(officialEndPointID);
      Assert.That(officialEndPoint.OppositeObjectID, Is.EqualTo(DomainObjectIDs.Official1));
      var officialOrdersEndPointID = RelationEndPointID.Create(
          DomainObjectIDs.Official1,
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Official.Orders");
      var officialOrdersEndPoint = _relationEndPointManager.GetRelationEndPointWithoutLoading(officialOrdersEndPointID);
      Assert.That(officialOrdersEndPoint, Is.Not.Null);
    }
  }
}
