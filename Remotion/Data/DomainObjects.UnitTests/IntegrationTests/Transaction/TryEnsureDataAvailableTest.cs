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
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class TryEnsureDataAvailableTest : EnsureDataAvailableTestBase
  {
    [Test]
    public void TryEnsureDataAvailable_AlreadyLoaded ()
    {
      CheckLoaded(LoadedObject1);

      var result = TestableClientTransaction.TryEnsureDataAvailable(LoadedObject1.ID);

      ListenerMock.Verify(
          mock => mock.ObjectsLoading(It.IsAny<ClientTransaction>(), It.IsAny<ReadOnlyCollection<ObjectID>>()),
          Times.Never());
      CheckLoaded(LoadedObject1);
      Assert.That(result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_NotLoadedYet ()
    {
      CheckNotLoaded(NotLoadedObject1);

      var result = TestableClientTransaction.TryEnsureDataAvailable(NotLoadedObject1.ID);

      ListenerMock.Verify(
          mock => mock.ObjectsLoading(TestableClientTransaction, It.Is<ReadOnlyCollection<ObjectID>>(p => p.SetEquals(new[] { NotLoadedObject1.ID }))),
          Times.AtLeastOnce());
      ListenerMock.Verify(
          mock => mock.ObjectsLoaded(TestableClientTransaction, It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new[] { NotLoadedObject1 }))),
          Times.AtLeastOnce());
      CheckLoaded(NotLoadedObject1);
      Assert.That(result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_Invalid ()
    {
      Assert.That(
          () => TestableClientTransaction.TryEnsureDataAvailable(InvalidObject.ID),
          Throws.InstanceOf<ObjectInvalidException>()
              .With.Message.Matches(@"Object '.*\|.*\|.*' is invalid in this transaction\."));
    }

    [Test]
    public void TryEnsureDataAvailable_NotFound ()
    {
      var result = TestableClientTransaction.TryEnsureDataAvailable(NotLoadedNonExistingObject.ID);

      CheckInvalid(NotLoadedNonExistingObject);
      Assert.That(result, Is.False);
      // Note: More elaborate tests for not found objects are in NotFoundObjectTest
    }

    [Test]
    public void TryEnsureDataAvailable_NotEnlisted ()
    {
      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.OrderTicket1), Is.Null);

      var result = TestableClientTransaction.TryEnsureDataAvailable(DomainObjectIDs.OrderTicket1);

      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.OrderTicket1), Is.Not.Null);
      Assert.That(result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_AlreadyLoaded ()
    {
      CheckLoaded(LoadedObject1);
      CheckLoaded(LoadedObject2);

      var result = TestableClientTransaction.TryEnsureDataAvailable(new[] { LoadedObject1.ID, LoadedObject2.ID });

      ListenerMock.Verify(mock => mock.ObjectsLoading(It.IsAny<ClientTransaction>(), It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      Assert.That(result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_NotLoadedYet ()
    {
      var result = TestableClientTransaction.TryEnsureDataAvailable(new[] { NotLoadedObject1.ID, NotLoadedObject2.ID });

      ListenerMock.Verify(
          mock => mock.ObjectsLoading(TestableClientTransaction, It.Is<ReadOnlyCollection<ObjectID>>(p => p.SetEquals(new[] { NotLoadedObject1.ID, NotLoadedObject2.ID }))),
          Times.AtLeastOnce());
      Assert.That(result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_SomeLoadedSomeNot ()
    {
      var result = TestableClientTransaction.TryEnsureDataAvailable(new[] { NotLoadedObject1.ID, NotLoadedObject2.ID, LoadedObject1.ID });

      ListenerMock.Verify(
          mock => mock.ObjectsLoading(TestableClientTransaction, It.Is<ReadOnlyCollection<ObjectID>>(p => p.SetEquals(new[] { NotLoadedObject1.ID, NotLoadedObject2.ID }))),
          Times.AtLeastOnce());
      ListenerMock.Verify(
          mock => mock.ObjectsLoaded(TestableClientTransaction, It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new[] { NotLoadedObject1, NotLoadedObject2 }))),
          Times.AtLeastOnce());

      ListenerMock.Verify(
          mock => mock.ObjectsLoading(It.IsAny<ClientTransaction>(), It.Is<ReadOnlyCollection<ObjectID>>(p => p.SetEquals(new[] { LoadedObject1.ID }))),
          Times.Never());

      Assert.That(result, Is.True);
    }

    [Test]
    public void TryEnsureDataAvailable_Many_Invalid ()
    {
      Assert.That(
          () => TestableClientTransaction.TryEnsureDataAvailable(new[] { InvalidObject.ID }),
          Throws.InstanceOf<ObjectInvalidException>()
              .With.Message.Matches(@"Object '.*\|.*\|.*' is invalid in this transaction\."));
    }

    [Test]
    public void TryEnsureDataAvailable_Many_SomeNotFound ()
    {
      var result = TestableClientTransaction.TryEnsureDataAvailable(new[] { NotLoadedNonExistingObject.ID, NotLoadedObject1.ID });

      CheckLoaded(NotLoadedObject1);
      CheckInvalid(NotLoadedNonExistingObject);
      Assert.That(result, Is.False);
      // Note: More elaborate tests for not found objects are in NotFoundObjectTest
    }

    [Test]
    public void TryEnsureDataAvailable_Many_NotEnlisted ()
    {
      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.OrderTicket1), Is.Null);

      var result = TestableClientTransaction.TryEnsureDataAvailable(new[] { DomainObjectIDs.OrderTicket1 });

      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.OrderTicket1), Is.Not.Null);
      Assert.That(result, Is.True);
    }
  }
}
