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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DataContainerTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.DataContainer' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void DataContainerIsNotSerializable ()
    {
      var objectID = new ObjectID("Customer", Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);

      Serializer.SerializeAndDeserialize (dataContainer);
    }

    [Test]
    public void DataContainerIsFlattenedSerializable ()
    {
      var objectID = new ObjectID("Customer", Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);
      Assert.That (deserializedDataContainer, Is.Not.SameAs (dataContainer));
      Assert.That (deserializedDataContainer.ID, Is.EqualTo (dataContainer.ID));
    }

    [Test]
    public void DataContainer_Contents ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();

      Computer computer = employee.Computer;
      computer.SerialNumber = "abc";

      DataContainer dataContainer = computer.InternalDataContainer;
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);

      Assert.That (deserializedDataContainer.ID, Is.EqualTo (dataContainer.ID));
      Assert.That (deserializedDataContainer.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedDataContainer.EventListener, Is.Not.Null);
      Assert.That (deserializedDataContainer.Timestamp, Is.EqualTo (dataContainer.Timestamp));
      Assert.That (deserializedDataContainer.DomainObject, Is.Not.Null);
      Assert.That (deserializedDataContainer.DomainObject.ID, Is.EqualTo (dataContainer.DomainObject.ID));
      Assert.That (deserializedDataContainer.State, Is.EqualTo (StateType.Changed));
      Assert.That (GetPropertyValue (deserializedDataContainer, typeof (Computer), "SerialNumber"), Is.EqualTo ("abc"));
      Assert.That (GetPropertyValue (deserializedDataContainer, typeof (Computer), "Employee"), Is.EqualTo (employee.ID));
    }

    [Test]
    public void DataContainer_MarkAsChanged_Contents()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      Computer computer = employee.Computer;
      DataContainer dataContainer = computer.InternalDataContainer;
      dataContainer.MarkAsChanged();

      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);

      Assert.That (deserializedDataContainer.ID, Is.EqualTo (dataContainer.ID));
      Assert.That (deserializedDataContainer.HasBeenMarkedChanged, Is.True);
      Assert.That (deserializedDataContainer.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void DataContainer_WithoutProperties_Contents ()
    {
      var objectID = new ObjectID(typeof (ClassWithoutProperties), Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);

      Assert.That (deserializedDataContainer.ID, Is.EqualTo (dataContainer.ID));
    }

    [Test]
    public void DataContainer_Discarded_Contents ()
    {
      Computer computer = Computer.NewObject ();
      DataContainer dataContainer = computer.InternalDataContainer;
      computer.Delete ();
      Assert.That (dataContainer.IsDiscarded, Is.True);

      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);
      Assert.That (deserializedDataContainer.IsDiscarded, Is.True);
      Assert.That (deserializedDataContainer.State, Is.EqualTo (StateType.Invalid));
    }
  }
}
