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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Synchronization
{
  [TestFixture]
  public class SyncStateTest : ClientTransactionBaseTest
  {
    [Test]
    public void CollectionItems_Synchronized_WithUnload ()
    {
      var orderItem = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var endPointID = RelationEndPointID.Resolve(orderItem, oi => oi.Order);
      var endPoint = (RealObjectEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID);
      Assert.That(endPoint, Is.Not.Null);

      Assert.That(RealObjectEndPointTestHelper.GetSyncState(endPoint), Is.TypeOf(typeof(UnknownRealObjectEndPointSyncState)));

      orderItem.Order.OrderItems.EnsureDataComplete();

      Assert.That(RealObjectEndPointTestHelper.GetSyncState(endPoint), Is.TypeOf(typeof(SynchronizedRealObjectEndPointSyncState)));

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, orderItem.Order.OrderItems.AssociatedEndPointID);

      Assert.That(RealObjectEndPointTestHelper.GetSyncState(endPoint), Is.TypeOf(typeof(UnknownRealObjectEndPointSyncState)));
    }

    [Test]
    public void CollectionItems_Unsynchronized_WithUnload ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      order.OrderItems.EnsureDataComplete();

      var orderItemID = RelationInconcsistenciesTestHelper.CreateAndInitializeObjectAndSetRelationInOtherTransaction<OrderItem, Order>(order.ID, (oi, o) =>
      {
        oi.Order = o;
        oi.Product = "Product1";
      });
      var orderItem = orderItemID.GetObject<OrderItem>();
      var endPointID = RelationEndPointID.Resolve(orderItem, oi => oi.Order);
      var endPoint = (RealObjectEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID);
      Assert.That(endPoint, Is.Not.Null);

      Assert.That(RealObjectEndPointTestHelper.GetSyncState(endPoint), Is.TypeOf(typeof(UnsynchronizedRealObjectEndPointSyncState)));

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, orderItem.Order.OrderItems.AssociatedEndPointID);

      Assert.That(RealObjectEndPointTestHelper.GetSyncState(endPoint), Is.TypeOf(typeof(UnknownRealObjectEndPointSyncState)));

      order.OrderItems.EnsureDataComplete();

      Assert.That(RealObjectEndPointTestHelper.GetSyncState(endPoint), Is.TypeOf(typeof(SynchronizedRealObjectEndPointSyncState)));
    }
  }
}
