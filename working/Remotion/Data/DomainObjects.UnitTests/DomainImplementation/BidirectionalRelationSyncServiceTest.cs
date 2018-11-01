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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation
{
  [TestFixture]
  public class BidirectionalRelationSyncServiceTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private RelationEndPointManager _relationEndPointManager;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();

      var dataManager = ClientTransactionTestHelper.GetDataManager (_transaction);
      _relationEndPointManager = (RelationEndPointManager) DataManagerTestHelper.GetRelationEndPointManager (dataManager);
    }
    
    [Test]
    public void IsSynchronized ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint>();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      endPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointStub.Stub (stub => stub.IsDataComplete).Return (true);
      endPointStub.Stub (stub => stub.IsSynchronized).Return (true).Repeat.Once();
      endPointStub.Stub (stub => stub.IsSynchronized).Return (false).Repeat.Once ();
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub);

      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, endPointID), Is.True);
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, endPointID), Is.False);
    }

    [Test]
    public void IsSynchronized_CalledFromSubTransaction_UsesRootTransaction ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      endPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointStub.Stub (stub => stub.IsDataComplete).Return (true);
      endPointStub.Stub (stub => stub.IsSynchronized).Return (true).Repeat.Once ();
      endPointStub.Stub (stub => stub.IsSynchronized).Return (false).Repeat.Once ();
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub);

      var subTransaction = _transaction.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope())
      {
        Assert.That (BidirectionalRelationSyncService.IsSynchronized (subTransaction, endPointID), Is.True);
        Assert.That (BidirectionalRelationSyncService.IsSynchronized (subTransaction, endPointID), Is.False);
      }
    }

    [Test]
    public void IsSynchronized_EndPointNotRegistered ()
    {
      var result = BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket"));
      Assert.That (result, Is.Null);
    }

    [Test]
    public void IsSynchronized_EndPointReturnsNull ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      endPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointStub.Stub (stub => stub.IsSynchronized).Return (null);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub);

      var result = BidirectionalRelationSyncService.IsSynchronized (_transaction, endPointID);
      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void IsSynchronized_UnidirectionalRelationEndPoint ()
    {
      BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void IsSynchronized_AnonymousRelationEndPoint ()
    {
      var locationClientEndPoint = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");
      var oppositeEndPoint = RelationEndPointID.Create (DomainObjectIDs.Client1, locationClientEndPoint.Definition.GetOppositeEndPointDefinition());
      BidirectionalRelationSyncService.IsSynchronized (_transaction, oppositeEndPoint);
    }

    [Test]
    public void Synchronize ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint>();
      endPointMock.Stub (stub => stub.ID).Return (endPointID);
      endPointMock.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMock.Stub (stub => stub.IsDataComplete).Return (true);
      endPointMock.Expect (mock => mock.Synchronize ());
      endPointMock.Replay();
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMock);

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);

      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void Synchronize_InTransactionHierarchy_DescendsToSubTransactions ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var endPointMockInParent = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMockInParent.Stub (stub => stub.ID).Return (endPointID);
      endPointMockInParent.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMockInParent.Stub (stub => stub.IsDataComplete).Return (true);
      endPointMockInParent.Expect (mock => mock.Synchronize ());
      endPointMockInParent.Replay ();
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMockInParent);

      var subTransaction = _transaction.CreateSubTransaction();
      var endPointMockInSub = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMockInSub.Stub (stub => stub.ID).Return (endPointID);
      endPointMockInSub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMockInSub.Stub (stub => stub.IsDataComplete).Return (true);
      endPointMockInSub.Expect (mock => mock.Synchronize ());
      endPointMockInSub.Replay ();
      DataManagerTestHelper.AddEndPoint (ClientTransactionTestHelper.GetDataManager (subTransaction), endPointMockInSub);

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);

      endPointMockInParent.VerifyAllExpectations ();
      endPointMockInSub.VerifyAllExpectations();
    }

    [Test]
    public void Synchronize_InTransactionHierarchy_StartsWithRoot ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var endPointMockInParent = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMockInParent.Stub (stub => stub.ID).Return (endPointID);
      endPointMockInParent.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMockInParent.Stub (stub => stub.IsDataComplete).Return (true);
      endPointMockInParent.Expect (mock => mock.Synchronize ());
      endPointMockInParent.Replay ();
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMockInParent);

      var subTransaction = _transaction.CreateSubTransaction ();
      var endPointMockInSub = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMockInSub.Stub (stub => stub.ID).Return (endPointID);
      endPointMockInSub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMockInSub.Stub (stub => stub.IsDataComplete).Return (true);
      endPointMockInSub.Expect (mock => mock.Synchronize ());
      endPointMockInSub.Replay ();
      DataManagerTestHelper.AddEndPoint (ClientTransactionTestHelper.GetDataManager (subTransaction), endPointMockInSub);

      BidirectionalRelationSyncService.Synchronize (subTransaction, endPointID);

      endPointMockInParent.VerifyAllExpectations ();
      endPointMockInSub.VerifyAllExpectations ();
    }

    [Test]
    public void Synchronize_InTransactionHierarchy_StopsWhenEndPointNotLoadedInSub ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");

      var endPointMockInParent = MockRepository.GenerateStrictMock<IRelationEndPoint> ();
      endPointMockInParent.Stub (stub => stub.ID).Return (endPointID);
      endPointMockInParent.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointMockInParent.Stub (stub => stub.IsDataComplete).Return (true);
      endPointMockInParent.Expect (mock => mock.Synchronize ());
      endPointMockInParent.Replay ();
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointMockInParent);

      var subTransaction = _transaction.CreateSubTransaction ();

      BidirectionalRelationSyncService.Synchronize (subTransaction, endPointID);

      endPointMockInParent.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
      "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void Synchronize_UnidirectionalEndpoint ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void Synchronize_AnonymousEndPoint ()
    {
      var locationClientEndPoint = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Client1, locationClientEndPoint.Definition.GetOppositeEndPointDefinition ());

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' of object "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' has not yet been fully loaded into the given ClientTransaction.")]
    public void Synchronize_EndPointNotRegistered ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, typeof (OrderItem), "Order");
      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' of object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has not yet been fully loaded into the given ClientTransaction.")]
    public void Synchronize_EndPointIncomplete ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ID).Return (endPointID);
      endPointStub.Stub (stub => stub.Definition).Return (endPointID.Definition);
      endPointStub.Stub (stub => stub.IsDataComplete).Return (false);
      RelationEndPointManagerTestHelper.AddEndPoint (_relationEndPointManager, endPointStub);

      BidirectionalRelationSyncService.Synchronize (_transaction, endPointID);
    }
  }
}