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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.InvalidObjects
{
  [TestFixture]
  public class InvalidDomainObjectManagerTest : StandardMappingTest
  {
    private IClientTransactionEventSink _transactionEventSinkWithMock;

    private InvalidDomainObjectManager _manager;
    private Order _order1;

    public override void SetUp ()
    {
      base.SetUp ();

      _transactionEventSinkWithMock = MockRepository.GenerateStrictMock<IClientTransactionEventSink>();

      _manager = new InvalidDomainObjectManager (_transactionEventSinkWithMock);
      _order1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
    }

    [Test]
    public void Initialization_WithoutInvalidObjects ()
    {
      var manager = new InvalidDomainObjectManager (_transactionEventSinkWithMock);

      Assert.That (manager.InvalidObjectCount, Is.EqualTo (0));
    }

    [Test]
    public void Initialization_WithInvalidObjects ()
    {
      var manager = new InvalidDomainObjectManager (_transactionEventSinkWithMock, new[] { _order1 });

      Assert.That (manager.InvalidObjectCount, Is.EqualTo (1));
      Assert.That (manager.IsInvalid (_order1.ID), Is.True);
      Assert.That (manager.GetInvalidObjectReference (_order1.ID), Is.SameAs (_order1));

      _transactionEventSinkWithMock.AssertWasNotCalled (mock => mock.RaiseObjectMarkedInvalidEvent ( Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Initialization_WithInvalidObjects_Duplicates ()
    {
      Assert.That (
          () => new InvalidDomainObjectManager (_transactionEventSinkWithMock, new[] { _order1, _order1 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "The sequence contains multiple different objects with the same ID.\r\nParameter name: invalidObjects"));
    }

    [Test]
    public void MarkInvalid ()
    {
      Assert.That (_manager.IsInvalid (_order1.ID), Is.False);
      Assert.That (_manager.InvalidObjectCount, Is.EqualTo (0));

      _transactionEventSinkWithMock.Expect (mock => mock.RaiseObjectMarkedInvalidEvent ( _order1));
      _transactionEventSinkWithMock.Replay();

      var result = _manager.MarkInvalid (_order1);

      _transactionEventSinkWithMock.VerifyAllExpectations();

      Assert.That (result, Is.True);
      Assert.That (_manager.IsInvalid (_order1.ID), Is.True);
      Assert.That (_manager.InvalidObjectCount, Is.EqualTo (1));
    }

    [Test]
    public void MarkInvalid_AlreadyInvalid ()
    {
      _transactionEventSinkWithMock.Expect (mock => mock.RaiseObjectMarkedInvalidEvent ( _order1)).Repeat.Once ();
      _transactionEventSinkWithMock.Replay();

      _manager.MarkInvalid (_order1);
      var result = _manager.MarkInvalid (_order1);

      Assert.That (result, Is.False);
      Assert.That (_manager.IsInvalid (_order1.ID), Is.True);
      Assert.That (_manager.InvalidObjectCount, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot mark the given object invalid, another object with the same ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has already "
        + "been marked.")]
    public void MarkInvalid_OtherObjectAlreadyInvalid ()
    {
      _transactionEventSinkWithMock.Expect (mock => mock.RaiseObjectMarkedInvalidEvent ( _order1)).Repeat.Once ();
      _transactionEventSinkWithMock.Replay();

      _manager.MarkInvalid (_order1);
      var otherOrder1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _manager.MarkInvalid (otherOrder1);

      _transactionEventSinkWithMock.AssertWasNotCalled (mock => mock.RaiseObjectMarkedInvalidEvent ( Arg.Is (otherOrder1)));
    }

    [Test]
    public void MarkNotInvalid ()
    {
      _transactionEventSinkWithMock.Stub (mock => mock.RaiseObjectMarkedInvalidEvent ( _order1));
      _manager.MarkInvalid (_order1);

      _transactionEventSinkWithMock.Expect (mock => mock.RaiseObjectMarkedNotInvalidEvent ( _order1));
      _transactionEventSinkWithMock.Replay();

      Assert.That (_manager.IsInvalid (_order1.ID), Is.True);
      Assert.That (_manager.InvalidObjectCount, Is.EqualTo (1));

      var result = _manager.MarkNotInvalid (_order1.ID);

      _transactionEventSinkWithMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
      Assert.That (_manager.IsInvalid (_order1.ID), Is.False);
      Assert.That (_manager.InvalidObjectCount, Is.EqualTo (0));
    }

    [Test]
    public void MarkNotInvalid_NotInvalid ()
    {
      var result = _manager.MarkNotInvalid (_order1.ID);

      _transactionEventSinkWithMock.AssertWasNotCalled (mock => mock.RaiseObjectMarkedNotInvalidEvent ( Arg<DomainObject>.Is.Anything));
      Assert.That (result, Is.False);
      Assert.That (_manager.IsInvalid (_order1.ID), Is.False);
      Assert.That (_manager.InvalidObjectCount, Is.EqualTo (0));
    }

    [Test]
    public void GetInvalidObjectReference ()
    {
      _transactionEventSinkWithMock.Stub (mock => mock.RaiseObjectMarkedInvalidEvent ( _order1));
      _manager.MarkInvalid (_order1);

      var result = _manager.GetInvalidObjectReference (_order1.ID);

      Assert.That (result, Is.SameAs (_order1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has not been marked invalid.\r\nParameter name: id")]
    public void GetInvalidObjectReference_NotInvalid ()
    {
      _manager.GetInvalidObjectReference (_order1.ID);
    }

    [Test]
    public void Serializable ()
    {
      var order = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);

      var transactionEventSink = new SerializableClientTransactionEventSinkFake();
      var manager = new InvalidDomainObjectManager (transactionEventSink);

      manager.MarkInvalid (order);

      var deserializedInstance = Serializer.SerializeAndDeserialize (manager);

      Assert.That (deserializedInstance.TransactionEventSink, Is.Not.Null);
      Assert.That (deserializedInstance.InvalidObjectIDs, Has.Member (order.ID));
    }

    
  }
}