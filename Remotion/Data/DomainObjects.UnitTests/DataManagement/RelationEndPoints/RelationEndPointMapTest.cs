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
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointMapTest : StandardMappingTest
  {
    private Mock<IClientTransactionEventSink> _transactionEventSinkWithMock;
    private RelationEndPointMap _map;

    private RelationEndPointID _endPointID1;
    private RelationEndPointID _endPointID2;

    private Mock<IRelationEndPoint> _endPointMock1;
    private Mock<IRelationEndPoint> _endPointMock2;

    public override void SetUp ()
    {
      base.SetUp();

      _transactionEventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);
      _map = new RelationEndPointMap(_transactionEventSinkWithMock.Object);

      _endPointID1 = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");
      _endPointID2 = RelationEndPointID.Create(DomainObjectIDs.Order3, typeof(Order), "Customer");

      _endPointMock1 = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      _endPointMock1.Setup(stub => stub.ID).Returns(_endPointID1);
      _endPointMock2 = new Mock<IRelationEndPoint>(MockBehavior.Strict);
      _endPointMock2.Setup(stub => stub.ID).Returns(_endPointID2);
    }

    [Test]
    public void Item ()
    {
      StubEvents();
      Assert.That(_map[_endPointID1], Is.Null);

      _map.AddEndPoint(_endPointMock1.Object);

      Assert.That(_map[_endPointID1], Is.SameAs(_endPointMock1.Object));
    }

    [Test]
    public void Count ()
    {
      StubEvents();
      Assert.That(_map.Count, Is.EqualTo(0));

      _map.AddEndPoint(_endPointMock1.Object);

      Assert.That(_map.Count, Is.EqualTo(1));

      _map.AddEndPoint(_endPointMock2.Object);

      Assert.That(_map.Count, Is.EqualTo(2));
    }

    [Test]
    public void GetEnumerator ()
    {
      StubEvents();

      _map.AddEndPoint(_endPointMock1.Object);
      _map.AddEndPoint(_endPointMock2.Object);

      var items = new List<IRelationEndPoint>();

      using (var enumerator = _map.GetEnumerator())
      {
        Assert.That(enumerator.MoveNext(), Is.True);
        items.Add(enumerator.Current);
        Assert.That(enumerator.MoveNext(), Is.True);
        items.Add(enumerator.Current);
        Assert.That(enumerator.MoveNext(), Is.False);
      }

      Assert.That(items, Is.EquivalentTo(new[] { _endPointMock1.Object, _endPointMock2.Object }));
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      StubEvents();

      _map.AddEndPoint(_endPointMock1.Object);
      _map.AddEndPoint(_endPointMock2.Object);

      var items = new List<IRelationEndPoint>();

      var enumerator = ((IEnumerable)_map).GetEnumerator();
      Assert.That(enumerator.MoveNext(), Is.True);
      items.Add((IRelationEndPoint)enumerator.Current);
      Assert.That(enumerator.MoveNext(), Is.True);
      items.Add((IRelationEndPoint)enumerator.Current);
      Assert.That(enumerator.MoveNext(), Is.False);

      Assert.That(items, Is.EquivalentTo(new[] { _endPointMock1.Object, _endPointMock2.Object }));
    }

    [Test]
    public void CommitAllEndPoints ()
    {
      StubEvents();

      _map.AddEndPoint(_endPointMock1.Object);
      _map.AddEndPoint(_endPointMock2.Object);

      _endPointMock1.Setup(mock => mock.Commit()).Verifiable();
      _endPointMock2.Setup(mock => mock.Commit()).Verifiable();

      _map.CommitAllEndPoints();

      _endPointMock1.Verify();
      _endPointMock2.Verify();
    }

    [Test]
    public void RollbackAllEndPoints ()
    {
      StubEvents();

      _map.AddEndPoint(_endPointMock1.Object);
      _map.AddEndPoint(_endPointMock2.Object);

      _endPointMock1.Setup(mock => mock.Rollback()).Verifiable();
      _endPointMock2.Setup(mock => mock.Rollback()).Verifiable();

      _map.RollbackAllEndPoints();

      _endPointMock1.Verify();
      _endPointMock2.Verify();
    }

    [Test]
    public void AddEndPoint ()
    {
      _transactionEventSinkWithMock.Setup(l => l.RaiseRelationEndPointMapRegisteringEvent(_endPointMock1.Object)).Verifiable();

      _map.AddEndPoint(_endPointMock1.Object);

      _transactionEventSinkWithMock.Verify();
      Assert.That(_map[_endPointID1], Is.SameAs(_endPointMock1.Object));
    }

    [Test]
    public void AddEndPoint_KeyAlreadyExists ()
    {
      // Note: We'll get an event even when an exception is thrown. This is more an optimization than a feature.
      _transactionEventSinkWithMock
.Setup(l => l.RaiseRelationEndPointMapRegisteringEvent(It.IsAny<IRelationEndPoint>()));

      _map.AddEndPoint(_endPointMock1.Object);

      var secondEndPointStub = new Mock<IRelationEndPoint>();
      secondEndPointStub.Setup(stub => stub.ID).Returns(_endPointID1);

      Assert.That(() => _map.AddEndPoint(secondEndPointStub.Object), Throws.InvalidOperationException.With.Message.EqualTo(
          "A relation end-point with ID "
          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' "
          + "has already been registered."));

      _transactionEventSinkWithMock.Verify(l => l.RaiseRelationEndPointMapRegisteringEvent(It.IsAny<IRelationEndPoint>()), Times.Exactly(2));
    }

    [Test]
    public void RemoveEndPoint ()
    {
      StubEvents();

      _map.AddEndPoint(_endPointMock1.Object);
      Assert.That(_map[_endPointID1], Is.Not.Null);

      _transactionEventSinkWithMock.Reset();
      _transactionEventSinkWithMock.Setup(l => l.RaiseRelationEndPointMapUnregisteringEvent(_endPointID1)).Verifiable();

      _map.RemoveEndPoint(_endPointID1);

      _transactionEventSinkWithMock.Verify();
      Assert.That(_map[_endPointID1], Is.Null);
    }

    [Test]
    public void RemoveEndPoint_KeyNotExists ()
    {
      // Note: We'll get an event even when an exception is thrown. This is more an optimization than a feature.
      _transactionEventSinkWithMock.Setup(l => l.RaiseRelationEndPointMapUnregisteringEvent(_endPointID1));

      Assert.That(() => _map.RemoveEndPoint(_endPointID1), Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
          "End point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' is not "
          + "part of this map.", "endPointID"));
    }

    private void StubEvents ()
    {
      _transactionEventSinkWithMock.Setup(l => l.RaiseRelationEndPointMapRegisteringEvent(It.IsAny<IRelationEndPoint>()));
      _transactionEventSinkWithMock.Setup(l => l.RaiseRelationEndPointMapUnregisteringEvent(It.IsAny<RelationEndPointID>()));
    }
  }
}
