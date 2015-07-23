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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class ChangeUnidirectionalRelationWithNullTest : ClientTransactionBaseTest
  {
    [Test]
    public void SetRelatedObjectwithNewNullObject ()
    {
      Client oldClient = DomainObjectIDs.Client1.GetObject<Client> ();
      Location location = DomainObjectIDs.Location1.GetObject<Location>();
      Assert.That (location.Client, Is.SameAs (oldClient));

      location.Client = null;

      Assert.That (location.Client, Is.Null);
      Assert.That (location.Properties[typeof (Location), "Client"].GetRelatedObjectID(), Is.Null);
      Assert.That (location.State, Is.EqualTo (StateType.Changed));
      Assert.That (oldClient.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void SetRelatedObjectWithOldNullObject ()
    {
      Client client = DomainObjectIDs.Client4.GetObject<Client> ();
      Client newClient = DomainObjectIDs.Client1.GetObject<Client> ();

      client.ParentClient = newClient;

      Assert.That (client.ParentClient, Is.SameAs (newClient));
      Assert.That (client.Properties[typeof (Client), "ParentClient"].GetRelatedObjectID (), Is.EqualTo (newClient.ID));
      Assert.That (client.State, Is.EqualTo (StateType.Changed));
      Assert.That (newClient.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void SetRelatedObjectWithOldAndNewNullObject ()
    {
      Client client = DomainObjectIDs.Client4.GetObject<Client> ();
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (client);

      client.ParentClient = null;

      eventReceiver.Check (new ChangeState[0]);
      Assert.That (client.ParentClient, Is.Null);
      Assert.That (client.Properties[typeof (Client), "ParentClient"].GetRelatedObjectID (), Is.Null);
      Assert.That (client.State, Is.EqualTo (StateType.Unchanged));
    }
  }
}
