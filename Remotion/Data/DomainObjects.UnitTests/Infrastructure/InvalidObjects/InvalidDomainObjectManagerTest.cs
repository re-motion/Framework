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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.InvalidObjects
{
  [TestFixture]
  public class InvalidDomainObjectManagerTest : StandardMappingTest
  {
    private Mock<IClientTransactionEventSink> _transactionEventSinkWithMock;

    private InvalidDomainObjectManager _manager;
    private Order _order1;

    public override void SetUp ()
    {
      base.SetUp();

      _transactionEventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);

      _manager = new InvalidDomainObjectManager(_transactionEventSinkWithMock.Object);
      _order1 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order1);
    }

    [Test]
    public void Initialization_WithoutInvalidObjects ()
    {
      var manager = new InvalidDomainObjectManager(_transactionEventSinkWithMock.Object);

      Assert.That(manager.InvalidObjectCount, Is.EqualTo(0));
    }

    [Test]
    public void Initialization_WithInvalidObjects ()
    {
      var manager = new InvalidDomainObjectManager(_transactionEventSinkWithMock.Object, new[] { _order1 });

      Assert.That(manager.InvalidObjectCount, Is.EqualTo(1));
      Assert.That(manager.IsInvalid(_order1.ID), Is.True);
      Assert.That(manager.GetInvalidObjectReference(_order1.ID), Is.SameAs(_order1));

      _transactionEventSinkWithMock.Verify(mock => mock.RaiseObjectMarkedInvalidEvent(It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Initialization_WithInvalidObjects_Duplicates ()
    {
      Assert.That(
          () => new InvalidDomainObjectManager(_transactionEventSinkWithMock.Object, new[] { _order1, _order1 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The sequence contains multiple different objects with the same ID.", "invalidObjects"));
    }

    [Test]
    public void MarkInvalid ()
    {
      Assert.That(_manager.IsInvalid(_order1.ID), Is.False);
      Assert.That(_manager.InvalidObjectCount, Is.EqualTo(0));

      _transactionEventSinkWithMock.Setup(mock => mock.RaiseObjectMarkedInvalidEvent(_order1)).Verifiable();

      var result = _manager.MarkInvalid(_order1);

      _transactionEventSinkWithMock.Verify();

      Assert.That(result, Is.True);
      Assert.That(_manager.IsInvalid(_order1.ID), Is.True);
      Assert.That(_manager.InvalidObjectCount, Is.EqualTo(1));
    }

    [Test]
    public void MarkInvalid_AlreadyInvalid ()
    {
      _transactionEventSinkWithMock.Setup(mock => mock.RaiseObjectMarkedInvalidEvent(_order1)).Verifiable();

      _manager.MarkInvalid(_order1);
      var result = _manager.MarkInvalid(_order1);

      Assert.That(result, Is.False);
      Assert.That(_manager.IsInvalid(_order1.ID), Is.True);
      Assert.That(_manager.InvalidObjectCount, Is.EqualTo(1));
    }

    [Test]
    public void MarkInvalid_OtherObjectAlreadyInvalid ()
    {
      _transactionEventSinkWithMock.Setup(mock => mock.RaiseObjectMarkedInvalidEvent(_order1)).Verifiable();

      _manager.MarkInvalid(_order1);
      var otherOrder1 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order1);
      Assert.That(
          () => _manager.MarkInvalid(otherOrder1),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot mark the given object invalid, another object with the same ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has already "
                  + "been marked."));
      _transactionEventSinkWithMock.Verify(mock => mock.RaiseObjectMarkedInvalidEvent(_order1), Times.Once());
    }

    [Test]
    public void MarkNotInvalid ()
    {
      _transactionEventSinkWithMock.Setup(mock => mock.RaiseObjectMarkedInvalidEvent(_order1));
      _manager.MarkInvalid(_order1);

      _transactionEventSinkWithMock.Setup(mock => mock.RaiseObjectMarkedNotInvalidEvent(_order1)).Verifiable();

      Assert.That(_manager.IsInvalid(_order1.ID), Is.True);
      Assert.That(_manager.InvalidObjectCount, Is.EqualTo(1));

      var result = _manager.MarkNotInvalid(_order1.ID);

      _transactionEventSinkWithMock.Verify();
      Assert.That(result, Is.True);
      Assert.That(_manager.IsInvalid(_order1.ID), Is.False);
      Assert.That(_manager.InvalidObjectCount, Is.EqualTo(0));
    }

    [Test]
    public void MarkNotInvalid_NotInvalid ()
    {
      var result = _manager.MarkNotInvalid(_order1.ID);

      _transactionEventSinkWithMock.Verify(mock => mock.RaiseObjectMarkedNotInvalidEvent(It.IsAny<DomainObject>()), Times.Never());
      Assert.That(result, Is.False);
      Assert.That(_manager.IsInvalid(_order1.ID), Is.False);
      Assert.That(_manager.InvalidObjectCount, Is.EqualTo(0));
    }

    [Test]
    public void GetInvalidObjectReference ()
    {
      _transactionEventSinkWithMock.Setup(mock => mock.RaiseObjectMarkedInvalidEvent(_order1));
      _manager.MarkInvalid(_order1);

      var result = _manager.GetInvalidObjectReference(_order1.ID);

      Assert.That(result, Is.SameAs(_order1));
    }

    [Test]
    public void GetInvalidObjectReference_NotInvalid ()
    {
      Assert.That(
          () => _manager.GetInvalidObjectReference(_order1.ID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has not been marked invalid.", "id"));
    }
  }
}
