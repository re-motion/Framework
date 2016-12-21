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
using Remotion.Data.DomainObjects.UnitTests.DomainObjects;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Relations
{
  [TestFixture]
  public class UnidirectionalRelationTest : RelationChangeBaseTest
  {
    private Client _oldClient;
    private Client _newClient;
    private Location _location;

    public override void SetUp ()
    {
      base.SetUp ();

      _oldClient = DomainObjectIDs.Client1.GetObject<Client> ();
      _newClient = DomainObjectIDs.Client2.GetObject<Client> ();
      _location = DomainObjectIDs.Location1.GetObject<Location>();
    }

    [Test]
    public void SetRelatedObject ()
    {
      _location.Client = _newClient;

      Assert.That (_location.Client, Is.SameAs (_newClient));
      Assert.That (_location.Properties[typeof (Location), "Client"].GetRelatedObjectID (), Is.EqualTo (_newClient.ID));
      Assert.That (_location.State, Is.EqualTo (StateType.Changed));
      Assert.That (_oldClient.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_newClient.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void EventsForSetRelatedObject ()
    {
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] { _location, _oldClient, _newClient }, new DomainObjectCollection[0]);

      _location.Client = _newClient;

      ChangeState[] expectedStates = new ChangeState[]
    {
      new RelationChangeState (_location, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", _oldClient, _newClient, "1. Changing event of location"),
      new RelationChangeState (_location, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", null, null, "2. Changed event of location")
    };

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void SetRelatedObjectWithSameOldAndNewObject ()
    {
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] { _location, _oldClient, _newClient }, new DomainObjectCollection[0]);

      _location.Client = _oldClient;

      eventReceiver.Check (new ChangeState[0]);
      Assert.That (_location.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetRelatedObject ()
    {
      Assert.That (_location.GetRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"), Is.SameAs (_oldClient));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Assert.That (_location.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"), Is.SameAs (_oldClient));

      _location.Client = _newClient;

      Assert.That (_location.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"), Is.SameAs (_oldClient));
    }

    [Test]
    public void CreateObjectsAndCommit ()
    {
      SetDatabaseModifyable ();

      Client client1 = Client.NewObject ();
      Client client2 = Client.NewObject ();
      Location location = Location.NewObject();

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] { location, client1, client2 }, new DomainObjectCollection[0]);

      location.Client = client1;

      Assert.That (client1.State, Is.EqualTo (StateType.New));
      Assert.That (client2.State, Is.EqualTo (StateType.New));
      Assert.That (location.State, Is.EqualTo (StateType.New));

      ObjectID clientID1 = client1.ID;
      ObjectID clientID2 = client2.ID;
      ObjectID locationID = location.ID;


      ChangeState[] expectedStates = new ChangeState[]
    {
      new RelationChangeState (location, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", null, client1, "1. Changing event of location"),
      new RelationChangeState (location, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", null, null, "2. Changed event of location")
    };

      eventReceiver.Check (expectedStates);

      TestableClientTransaction.Commit ();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        client1 = clientID1.GetObject<Client> ();
        client2 = clientID2.GetObject<Client> ();
        location = locationID.GetObject<Location>();

        Assert.That (client1, Is.Not.Null);
        Assert.That (client2, Is.Not.Null);
        Assert.That (location, Is.Not.Null);
        Assert.That (location.Client, Is.SameAs (client1));
      }
    }

    [Test]
    public void DeleteLocationAndCommit ()
    {
      SetDatabaseModifyable ();

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] { _location, _oldClient, _newClient }, new DomainObjectCollection[0]);

      _location.Delete ();
      TestableClientTransaction.Commit ();

      ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_location, "1. Deleting event of location"),
      new ObjectDeletionState (_location, "2. Deleted event of location")
    };

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void DeleteMultipleObjectsAndCommit ()
    {
      SetDatabaseModifyable ();

      _location.Delete ();
      _oldClient.Delete ();
      _newClient.Delete ();

      Client client3 = DomainObjectIDs.Client3.GetObject<Client> ();
      client3.Delete ();

      Location location2 = DomainObjectIDs.Location2.GetObject<Location>();
      location2.Delete ();

      Location location3 = DomainObjectIDs.Location3.GetObject<Location>();
      location3.Delete ();

      TestableClientTransaction.Commit ();
    }

    [Test]
    public void RelationAccessToDeletedLoaded_ReturnsDeletedObject_AndThrowsOnChanges ()
    {
      _location.Client.Delete ();
      Client client = _location.Client;

      Assert.That (client.State, Is.EqualTo (StateType.Deleted));
      Assert.That (() => client.ParentClient, Is.Null);
      Assert.That (() => client.ParentClient = null, Throws.TypeOf<ObjectDeletedException> ());
    }

    [Test]
    public void DeletedObject_CanBeOverwritten ()
    {
      Location location = Location.NewObject ();
      location.Client = DomainObjectIDs.Client1.GetObject<Client> ();
      location.Client.Delete ();
      location.Client = Client.NewObject ();
    }

    [Test]
    public void RelationAccessToDeletedNew_ReturnsInvalidObject_AndThrowsOnAccess ()
    {
      _location.Client = Client.NewObject ();
      _location.Client.Delete ();
      Client client = _location.Client;

      Assert.That (client.State, Is.EqualTo (StateType.Invalid));
      Assert.That (() => client.ParentClient, Throws.TypeOf<ObjectInvalidException> ());
      Assert.That (() => client.ParentClient = null, Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void InvalidObject_CanBeOverwritten ()
    {
      Location location = Location.NewObject ();
      location.Client = Client.NewObject ();
      location.Client.Delete ();
      location.Client = Client.NewObject ();
    }


    [Test]
    public void DeleteClientAndCommit_CausesRelatedObjectToBecomeInvalid ()
    {
      // Need to perform this test within a subtransaction, otherwise, the database will yield a foreign key violation
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        _location.Client.Delete();
        ClientTransaction.Current.Commit();
        Assert.That (_location.Client, Is.Not.Null);
        Assert.That (_location.Client.State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void ResettingDeletedLoadedWorks ()
    {
      _location.Client.Delete ();
      Client newClient = Client.NewObject ();
      _location.Client = newClient;
      Assert.That (_location.Client, Is.SameAs (newClient));
    }

    [Test]
    public void ResettingDeletedNewWorks ()
    {
      _location.Client = Client.NewObject ();
      _location.Client.Delete ();
      Client newClient = Client.NewObject ();
      _location.Client = newClient;
      Assert.That (_location.Client, Is.SameAs (newClient));
    }

    [Test]
    public void StateRemainsUnchangedWhenDeletingRelatedObject ()
    {
      Assert.That (_location.State, Is.EqualTo (StateType.Unchanged));
      _location.Client.Delete ();
      Assert.That (_location.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Rollback ()
    {
      _location.Delete ();
      Location newLocation = Location.NewObject();
      newLocation.Client = _newClient;

      TestableClientTransaction.Rollback ();

      Assert.That (_location.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void CreateHierarchy ()
    {
      SetDatabaseModifyable ();

      Client newClient1 = Client.NewObject ();
      Client newClient2 = Client.NewObject ();
      newClient2.ParentClient = newClient1;

      ObjectID newClientID1 = newClient1.ID;
      ObjectID newClientID2 = newClient2.ID;

      TestableClientTransaction.Commit ();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        newClient1 = newClientID1.GetObject<Client> ();
        newClient2 = newClientID2.GetObject<Client> ();

        Assert.That (newClient1, Is.Not.Null);
        Assert.That (newClient2, Is.Not.Null);
        Assert.That (newClient2.ParentClient, Is.SameAs (newClient1));
      }
    }

    [Test]
    public void HasBeenTouched ()
    {
      CheckTouching (delegate { _location.Client = _newClient; }, _location, "Client",
          RelationEndPointID.Create(_location.ID, typeof (Location).FullName + ".Client"));
    }

    [Test]
    public void HasBeenTouched_OriginalValue ()
    {
      CheckTouching (delegate { _location.Client = _location.Client; }, _location, "Client",
          RelationEndPointID.Create(_location.ID, typeof (Location).FullName + ".Client"));
    }
  }
}
