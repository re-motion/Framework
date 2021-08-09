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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DataContainerMapTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp();
      Assert2.IgnoreIfFeatureSerializationIsDisabled();
    }

    [Test]
    public void DataContainerMapIsNotSerializable ()
    {
      Assert.That (
          () => Serializer.SerializeAndDeserialize (TestableClientTransaction.DataManager.DataContainers),
          Throws.InstanceOf<SerializationException>()
              .With.Message.Matches (
                  "Type 'Remotion.Data.DomainObjects.DataManagement.DataContainerMap' in Assembly "
                  + ".* is not marked as serializable."));
    }

    [Test]
    public void DataContainerMapIsFlattenedSerializable ()
    {
      DataContainerMap map = DataManagerTestHelper.GetDataContainerMap (TestableClientTransaction.DataManager);

      DataContainerMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);
      Assert.That (deserializedMap, Is.Not.Null);
    }

    [Test]
    public void DataContainerMap_Content ()
    {
      DataContainerMap map = DataManagerTestHelper.GetDataContainerMap (TestableClientTransaction.DataManager);
      DomainObjectIDs.Order1.GetObject<Order> ();
      Assert.That (map.Count, Is.EqualTo (1));

      DataContainerMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (map);
      Assert.That (deserializedMap.TransactionEventSink, Is.Not.Null);
      Assert.That (deserializedMap.Count, Is.EqualTo (1));
    }
  }
}
