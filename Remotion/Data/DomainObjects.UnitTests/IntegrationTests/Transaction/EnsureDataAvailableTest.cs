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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class EnsureDataAvailableTest : EnsureDataAvailableTestBase
  {
    [Test]
    public void EnsureDataAvailable_AlreadyLoaded ()
    {
      CheckLoaded(LoadedObject1);

      TestableClientTransaction.EnsureDataAvailable(LoadedObject1.ID);

      ListenerMock.Verify(mock => mock.ObjectsLoading(
          It.IsAny<ClientTransaction>(),
          It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
      CheckLoaded(LoadedObject1);
    }

    [Test]
    public void EnsureDataAvailable_NotLoadedYet ()
    {
      CheckNotLoaded(NotLoadedObject1);

      TestableClientTransaction.EnsureDataAvailable(NotLoadedObject1.ID);

      ListenerMock.Verify(
          mock => mock.ObjectsLoading(
              TestableClientTransaction,
              It.Is<ReadOnlyCollection<ObjectID>>(p => p.SetEquals(new[] { NotLoadedObject1.ID }))),
          Times.AtLeastOnce());
      ListenerMock.Verify(
          mock => mock.ObjectsLoaded(
              TestableClientTransaction,
              It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new[] { NotLoadedObject1 }))),
          Times.AtLeastOnce());
      CheckLoaded(NotLoadedObject1);
    }

    [Test]
    public void EnsureDataAvailable_Invalid ()
    {
      Assert.That(
          () => TestableClientTransaction.EnsureDataAvailable(InvalidObject.ID),
          Throws.InstanceOf<ObjectInvalidException>()
              .With.Message.Matches(@"Object '.*\|.*\|.*' is invalid in this transaction\."));
    }

    [Test]
    public void EnsureDataAvailable_NotFound ()
    {
      CheckNotLoaded(NotLoadedNonExistingObject);

      Assert.That(
          () => TestableClientTransaction.EnsureDataAvailable(NotLoadedNonExistingObject.ID),
          Throws.TypeOf<ObjectsNotFoundException>().With.Message.EqualTo("Object(s) could not be found: '" + NotLoadedNonExistingObject.ID + "'."));

      CheckInvalid(NotLoadedNonExistingObject);
      // Note: More elaborate tests for not found objects are in NotFoundObjectTest
    }

    [Test]
    public void EnsureDataAvailable_NotEnlisted ()
    {
      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.OrderTicket1), Is.Null);

      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.OrderTicket1);

      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.OrderTicket1), Is.Not.Null);
    }

    [Test]
    public void EnsureDataAvailable_Many_AlreadyLoaded ()
    {
      CheckLoaded(LoadedObject1);
      CheckLoaded(LoadedObject2);

      TestableClientTransaction.EnsureDataAvailable(new[] { LoadedObject1.ID, LoadedObject2.ID });

      ListenerMock.Verify(mock => mock.ObjectsLoading(It.IsAny<ClientTransaction>(), It.IsAny<ReadOnlyCollection<ObjectID>>()), Times.Never());
    }

    [Test]
    public void EnsureDataAvailable_Many_NotLoadedYet ()
    {
      TestableClientTransaction.EnsureDataAvailable(new[] { NotLoadedObject1.ID, NotLoadedObject2.ID });

      ListenerMock.Verify(mock => mock.ObjectsLoading(
          TestableClientTransaction,
          It.Is<ReadOnlyCollection<ObjectID>>(p => p.SetEquals(new[] { NotLoadedObject1.ID, NotLoadedObject2.ID }))), Times.AtLeastOnce());
    }

    [Test]
    public void EnsureDataAvailable_Many_SomeLoadedSomeNot ()
    {
      TestableClientTransaction.EnsureDataAvailable(new[] { NotLoadedObject1.ID, NotLoadedObject2.ID, LoadedObject1.ID });

      ListenerMock.Verify(mock => mock.ObjectsLoading(
          TestableClientTransaction,
          It.Is<ReadOnlyCollection<ObjectID>>(p => p.SetEquals(new[] { NotLoadedObject1.ID, NotLoadedObject2.ID }))), Times.AtLeastOnce());
      ListenerMock.Verify(mock => mock.ObjectsLoaded(
          TestableClientTransaction,
          It.Is<ReadOnlyCollection<DomainObject>>(p => p.SetEquals(new[] { NotLoadedObject1, NotLoadedObject2 }))), Times.AtLeastOnce());

      ListenerMock.Verify(mock => mock.ObjectsLoading(
          It.IsAny<ClientTransaction>(),
          It.Is<ReadOnlyCollection<ObjectID>>(p => p.SetEquals(new[] { LoadedObject1.ID }))), Times.Never());
    }

    [Test]
    public void EnsureDataAvailable_Many_Invalid ()
    {
      Assert.That(
          () => TestableClientTransaction.EnsureDataAvailable(new[] { InvalidObject.ID }),
          Throws.InstanceOf<ObjectInvalidException>()
              .With.Message.Matches(@"Object '.*\|.*\|.*' is invalid in this transaction\."));
    }

    [Test]
    public void EnsureDataAvailable_Many_NotFound ()
    {
      CheckNotLoaded(NotLoadedNonExistingObject);

      Assert.That(
          () => TestableClientTransaction.EnsureDataAvailable(new[] { NotLoadedNonExistingObject.ID }),
          Throws.TypeOf<ObjectsNotFoundException>().With.Message.EqualTo("Object(s) could not be found: '" + NotLoadedNonExistingObject.ID + "'."));

      CheckInvalid(NotLoadedNonExistingObject);
      // Note: More elaborate tests for not found objects are in NotFoundObjectTest
    }

    [Test]
    public void EnsureDataAvailable_Many_NotEnlisted ()
    {
      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.OrderTicket1), Is.Null);

      TestableClientTransaction.EnsureDataAvailable(new[] { DomainObjectIDs.OrderTicket1 });

      Assert.That(TestableClientTransaction.GetEnlistedDomainObject(DomainObjectIDs.OrderTicket1), Is.Not.Null);
    }
  }
}
