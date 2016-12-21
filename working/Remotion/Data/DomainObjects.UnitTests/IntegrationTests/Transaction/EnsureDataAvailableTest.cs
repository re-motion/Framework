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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Development.RhinoMocks.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class EnsureDataAvailableTest : EnsureDataAvailableTestBase
  {
    [Test]
    public void EnsureDataAvailable_AlreadyLoaded ()
    {
      CheckLoaded (LoadedObject1);

      TestableClientTransaction.EnsureDataAvailable (LoadedObject1.ID);

      ListenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
          Arg<ClientTransaction>.Is.Anything,
          Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      CheckLoaded (LoadedObject1);
    }

    [Test]
    public void EnsureDataAvailable_NotLoadedYet ()
    {
      CheckNotLoaded (NotLoadedObject1);

      TestableClientTransaction.EnsureDataAvailable (NotLoadedObject1.ID);

      ListenerMock.AssertWasCalled (mock => mock.ObjectsLoading (
          Arg.Is (TestableClientTransaction),
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (new[] { NotLoadedObject1.ID })));
      ListenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (
          Arg.Is (TestableClientTransaction),
          Arg<ReadOnlyCollection<DomainObject>>.List.ContainsAll (new[] { NotLoadedObject1 })));
      CheckLoaded (NotLoadedObject1);
    }

    [Test]
    [ExpectedException (
        typeof (ObjectInvalidException),
        ExpectedMessage = @"Object '.*\|.*\|.*' is invalid in this transaction\.",
        MatchType = MessageMatch.Regex)]
    public void EnsureDataAvailable_Invalid ()
    {
      TestableClientTransaction.EnsureDataAvailable (InvalidObject.ID);
    }

    [Test]
    public void EnsureDataAvailable_NotFound ()
    {
      CheckNotLoaded (NotLoadedNonExistingObject);

      Assert.That (
          () => TestableClientTransaction.EnsureDataAvailable (NotLoadedNonExistingObject.ID),
          Throws.TypeOf<ObjectsNotFoundException> ().With.Message.EqualTo ("Object(s) could not be found: '" + NotLoadedNonExistingObject.ID + "'."));

      CheckInvalid (NotLoadedNonExistingObject);
      // Note: More elaborate tests for not found objects are in NotFoundObjectTest
    }

    [Test]
    public void EnsureDataAvailable_NotEnlisted ()
    {
      Assert.That (TestableClientTransaction.GetEnlistedDomainObject (DomainObjectIDs.OrderTicket1), Is.Null);

      TestableClientTransaction.EnsureDataAvailable (DomainObjectIDs.OrderTicket1);

      Assert.That (TestableClientTransaction.GetEnlistedDomainObject (DomainObjectIDs.OrderTicket1), Is.Not.Null);
    }

    [Test]
    public void EnsureDataAvailable_Many_AlreadyLoaded ()
    {
      CheckLoaded (LoadedObject1);
      CheckLoaded (LoadedObject2);

      TestableClientTransaction.EnsureDataAvailable (new[] { LoadedObject1.ID, LoadedObject2.ID });

      ListenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
    }

    [Test]
    public void EnsureDataAvailable_Many_NotLoadedYet ()
    {
      TestableClientTransaction.EnsureDataAvailable (new[] { NotLoadedObject1.ID, NotLoadedObject2.ID });

      ListenerMock.AssertWasCalled (mock => mock.ObjectsLoading (
          Arg.Is (TestableClientTransaction),
          Arg<ReadOnlyCollection<ObjectID>>.List.Equivalent (new[] { NotLoadedObject1.ID, NotLoadedObject2.ID })));
    }

    [Test]
    public void EnsureDataAvailable_Many_SomeLoadedSomeNot ()
    {
      TestableClientTransaction.EnsureDataAvailable (new[] { NotLoadedObject1.ID, NotLoadedObject2.ID, LoadedObject1.ID });

      ListenerMock.AssertWasCalled (mock => mock.ObjectsLoading (
          Arg.Is (TestableClientTransaction),
          Arg<ReadOnlyCollection<ObjectID>>.List.Equivalent (new[] { NotLoadedObject1.ID, NotLoadedObject2.ID })));
      ListenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (
          Arg.Is (TestableClientTransaction),
          Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { NotLoadedObject1, NotLoadedObject2 })));

      ListenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
          Arg<ClientTransaction>.Is.Anything,
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (new[] { LoadedObject1.ID })));
    }

    [Test]
    [ExpectedException (
        typeof (ObjectInvalidException), 
        ExpectedMessage = @"Object '.*\|.*\|.*' is invalid in this transaction\.",
        MatchType = MessageMatch.Regex)]
    public void EnsureDataAvailable_Many_Invalid ()
    {
      TestableClientTransaction.EnsureDataAvailable (new[] { InvalidObject.ID });
    }

    [Test]
    public void EnsureDataAvailable_Many_NotFound ()
    {
      CheckNotLoaded (NotLoadedNonExistingObject);

      Assert.That (
          () => TestableClientTransaction.EnsureDataAvailable (new[] { NotLoadedNonExistingObject.ID }),
          Throws.TypeOf<ObjectsNotFoundException>().With.Message.EqualTo ("Object(s) could not be found: '" + NotLoadedNonExistingObject.ID + "'."));

      CheckInvalid (NotLoadedNonExistingObject);
      // Note: More elaborate tests for not found objects are in NotFoundObjectTest
    }

    [Test]
    public void EnsureDataAvailable_Many_NotEnlisted ()
    {
      Assert.That (TestableClientTransaction.GetEnlistedDomainObject (DomainObjectIDs.OrderTicket1), Is.Null);

      TestableClientTransaction.EnsureDataAvailable (new[] { DomainObjectIDs.OrderTicket1 });

      Assert.That (TestableClientTransaction.GetEnlistedDomainObject (DomainObjectIDs.OrderTicket1), Is.Not.Null);
    }
  }
}