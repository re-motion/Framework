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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.NUnit.UnitTesting;

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

      var dataManager = ClientTransactionTestHelper.GetDataManager(_transaction);
      _relationEndPointManager = (RelationEndPointManager)DataManagerTestHelper.GetRelationEndPointManager(dataManager);
    }

    [Test]
    public void IsSynchronized ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.OrderItem1, typeof(OrderItem), "Order");
      var endPointStub = new Mock<IRelationEndPoint>();
      endPointStub.Setup(stub => stub.ID).Returns(endPointID);
      endPointStub.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointStub.Setup(stub => stub.IsDataComplete).Returns(true);
      var sequence = new MockSequence();
      endPointStub.InSequence(sequence).Setup(stub => stub.IsSynchronized).Returns(true);
      endPointStub.InSequence(sequence).Setup(stub => stub.IsSynchronized).Returns(false);
      RelationEndPointManagerTestHelper.AddEndPoint(_relationEndPointManager, endPointStub.Object);

      Assert.That(BidirectionalRelationSyncService.IsSynchronized(_transaction, endPointID), Is.True);
      Assert.That(BidirectionalRelationSyncService.IsSynchronized(_transaction, endPointID), Is.False);
    }

    [Test]
    public void IsSynchronized_CalledFromSubTransaction_UsesRootTransaction ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.OrderItem1, typeof(OrderItem), "Order");
      var endPointStub = new Mock<IRelationEndPoint>();
      endPointStub.Setup(stub => stub.ID).Returns(endPointID);
      endPointStub.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointStub.Setup(stub => stub.IsDataComplete).Returns(true);
      var sequence = new MockSequence();
      endPointStub.InSequence(sequence).Setup(stub => stub.IsSynchronized).Returns(true);
      endPointStub.InSequence(sequence).Setup(stub => stub.IsSynchronized).Returns(false);
      RelationEndPointManagerTestHelper.AddEndPoint(_relationEndPointManager, endPointStub.Object);

      var subTransaction = _transaction.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Assert.That(BidirectionalRelationSyncService.IsSynchronized(subTransaction, endPointID), Is.True);
        Assert.That(BidirectionalRelationSyncService.IsSynchronized(subTransaction, endPointID), Is.False);
      }
    }

    [Test]
    public void IsSynchronized_EndPointNotRegistered ()
    {
      var result = BidirectionalRelationSyncService.IsSynchronized(_transaction, RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket"));
      Assert.That(result, Is.Null);
    }

    [Test]
    public void IsSynchronized_EndPointReturnsNull ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      var endPointStub = new Mock<IRelationEndPoint>();
      endPointStub.Setup(stub => stub.ID).Returns(endPointID);
      endPointStub.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointStub.Setup(stub => stub.IsSynchronized).Returns((bool?)null);
      RelationEndPointManagerTestHelper.AddEndPoint(_relationEndPointManager, endPointStub.Object);

      var result = BidirectionalRelationSyncService.IsSynchronized(_transaction, endPointID);
      Assert.That(result, Is.Null);
    }

    [Test]
    public void IsSynchronized_UnidirectionalRelationEndPoint ()
    {
      Assert.That(
          () => BidirectionalRelationSyncService.IsSynchronized(_transaction, RelationEndPointID.Create(DomainObjectIDs.Location1, typeof(Location), "Client")),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.", "endPointID"));
    }

    [Test]
    public void IsSynchronized_AnonymousRelationEndPoint ()
    {
      var locationClientEndPoint = RelationEndPointID.Create(DomainObjectIDs.Location1, typeof(Location), "Client");
      var oppositeEndPoint = RelationEndPointID.Create(DomainObjectIDs.Client1, locationClientEndPoint.Definition.GetOppositeEndPointDefinition());
      Assert.That(
          () => BidirectionalRelationSyncService.IsSynchronized(_transaction, oppositeEndPoint),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.", "endPointID"));
    }

    [Test]
    public void Synchronize ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");

      var endPointMock = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.ID).Returns(endPointID);
      endPointMock.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointMock.Setup(stub => stub.IsDataComplete).Returns(true);
      endPointMock.Setup(mock => mock.Synchronize()).Verifiable();
      RelationEndPointManagerTestHelper.AddEndPoint(_relationEndPointManager, endPointMock.Object);

      BidirectionalRelationSyncService.Synchronize(_transaction, endPointID);

      endPointMock.Verify();
    }

    [Test]
    public void Synchronize_InTransactionHierarchy_DescendsToSubTransactions ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");

      var endPointMockInParent = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      endPointMockInParent.Setup(stub => stub.ID).Returns(endPointID);
      endPointMockInParent.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointMockInParent.Setup(stub => stub.IsDataComplete).Returns(true);
      endPointMockInParent.Setup(mock => mock.Synchronize()).Verifiable();
      RelationEndPointManagerTestHelper.AddEndPoint(_relationEndPointManager, endPointMockInParent.Object);

      var subTransaction = _transaction.CreateSubTransaction();
      var endPointMockInSub = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      endPointMockInSub.Setup(stub => stub.ID).Returns(endPointID);
      endPointMockInSub.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointMockInSub.Setup(stub => stub.IsDataComplete).Returns(true);
      endPointMockInSub.Setup(mock => mock.Synchronize()).Verifiable();
      DataManagerTestHelper.AddEndPoint(ClientTransactionTestHelper.GetDataManager(subTransaction), endPointMockInSub.Object);

      BidirectionalRelationSyncService.Synchronize(_transaction, endPointID);

      endPointMockInParent.Verify();
      endPointMockInSub.Verify();
    }

    [Test]
    public void Synchronize_InTransactionHierarchy_StartsWithRoot ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");

      var endPointMockInParent = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      endPointMockInParent.Setup(stub => stub.ID).Returns(endPointID);
      endPointMockInParent.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointMockInParent.Setup(stub => stub.IsDataComplete).Returns(true);
      endPointMockInParent.Setup(mock => mock.Synchronize()).Verifiable();
      RelationEndPointManagerTestHelper.AddEndPoint(_relationEndPointManager, endPointMockInParent.Object);

      var subTransaction = _transaction.CreateSubTransaction();
      var endPointMockInSub = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      endPointMockInSub.Setup(stub => stub.ID).Returns(endPointID);
      endPointMockInSub.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointMockInSub.Setup(stub => stub.IsDataComplete).Returns(true);
      endPointMockInSub.Setup(mock => mock.Synchronize()).Verifiable();
      DataManagerTestHelper.AddEndPoint(ClientTransactionTestHelper.GetDataManager(subTransaction), endPointMockInSub.Object);

      BidirectionalRelationSyncService.Synchronize(subTransaction, endPointID);

      endPointMockInParent.Verify();
      endPointMockInSub.Verify();
    }

    [Test]
    public void Synchronize_InTransactionHierarchy_StopsWhenEndPointNotLoadedInSub ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");

      var endPointMockInParent = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      endPointMockInParent.Setup(stub => stub.ID).Returns(endPointID);
      endPointMockInParent.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointMockInParent.Setup(stub => stub.IsDataComplete).Returns(true);
      endPointMockInParent.Setup(mock => mock.Synchronize()).Verifiable();
      RelationEndPointManagerTestHelper.AddEndPoint(_relationEndPointManager, endPointMockInParent.Object);

      var subTransaction = _transaction.CreateSubTransaction();

      BidirectionalRelationSyncService.Synchronize(subTransaction, endPointID);

      endPointMockInParent.Verify();
    }

    [Test]
    public void Synchronize_UnidirectionalEndpoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Location1, typeof(Location), "Client");
      Assert.That(
          () => BidirectionalRelationSyncService.Synchronize(_transaction, endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.", "endPointID"));
    }

    [Test]
    public void Synchronize_AnonymousEndPoint ()
    {
      var locationClientEndPoint = RelationEndPointID.Create(DomainObjectIDs.Location1, typeof(Location), "Client");
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Client1, locationClientEndPoint.Definition.GetOppositeEndPointDefinition());
      Assert.That(
          () => BidirectionalRelationSyncService.Synchronize(_transaction, endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.", "endPointID"));
    }

    [Test]
    public void Synchronize_EndPointNotRegistered ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.OrderItem1, typeof(OrderItem), "Order");
      Assert.That(
          () => BidirectionalRelationSyncService.Synchronize(_transaction, endPointID),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' of object "
                  + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' has not yet been fully loaded into the given ClientTransaction."));
    }

    [Test]
    public void Synchronize_EndPointIncomplete ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      var endPointStub = new Mock<IRelationEndPoint>();
      endPointStub.Setup(stub => stub.ID).Returns(endPointID);
      endPointStub.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointStub.Setup(stub => stub.IsDataComplete).Returns(false);
      RelationEndPointManagerTestHelper.AddEndPoint(_relationEndPointManager, endPointStub.Object);
      Assert.That(
          () => BidirectionalRelationSyncService.Synchronize(_transaction, endPointID),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' of object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has not yet been fully loaded into the given ClientTransaction."));
    }
  }
}
