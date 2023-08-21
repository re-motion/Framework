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
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp();
      Assert2.IgnoreIfFeatureSerializationIsDisabled();
    }

    [Test]
    public void DataManagerIsSerializable ()
    {
      var dataManager = TestableClientTransaction.DataManager;

      DataManager dataManager2 = Serializer.SerializeAndDeserialize(dataManager);
      Assert.That(dataManager2, Is.Not.Null);
      Assert.That(dataManager, Is.Not.SameAs(dataManager2));
    }

    [Test]
    public void DataManager_Content ()
    {
      DataManager dataManager = TestableClientTransaction.DataManager;
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Dev.Null = order.OrderItems[0];

      Assert.That(dataManager.DataContainers.Count, Is.Not.EqualTo(0));
      Assert.That(dataManager.RelationEndPoints.Count, Is.Not.EqualTo(0));

      Tuple<ClientTransaction, DataManager> deserializedData =
          Serializer.SerializeAndDeserialize(Tuple.Create(ClientTransaction.Current, dataManager));

      Assert.That(deserializedData.Item2.TransactionEventSink, Is.Not.Null);
      Assert.That(deserializedData.Item2.DataContainerEventListener, Is.Not.Null);

      Assert.That(deserializedData.Item2.DataContainers.Count, Is.Not.EqualTo(0));
      Assert.That(deserializedData.Item2.RelationEndPoints.Count, Is.Not.EqualTo(0));

      Assert.That(PrivateInvoke.GetNonPublicField(deserializedData.Item2, "_clientTransaction"), Is.SameAs(deserializedData.Item1));
      Assert.That(PrivateInvoke.GetNonPublicField(deserializedData.Item2, "_invalidDomainObjectManager"), Is.Not.Null);
      Assert.That(PrivateInvoke.GetNonPublicField(deserializedData.Item2, "_objectLoader"), Is.Not.Null);
    }

    public void DumpSerializedDataManager ()
    {
      DataManager dataManager = TestableClientTransaction.DataManager;
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Dev.Null = order.OrderItems[0];

      Order invalidOrder = Order.NewObject();
      invalidOrder.Delete();

      for (int i = 0; i < 500; ++i)
      {
        Order newOrder = Order.NewObject();
        newOrder.OrderTicket = OrderTicket.NewObject();
      }

#pragma warning disable SYSLIB0050
      var info = new SerializationInfo(typeof(DataManager), new FormatterConverter());
      ((ISerializable)dataManager).GetObjectData(info, new StreamingContext());
#pragma warning restore SYSLIB0050
      var data = (object[])info.GetValue("doInfo.GetData", typeof(object[]));
      Console.WriteLine("Object stream:");
      Dump((object[])data[0]);
      Console.WriteLine("Int stream:");
      Dump((int[])data[1]);
      Console.WriteLine("Bool stream:");
      Dump((bool[])data[2]);
    }

    private void Dump<T> (T[] data)
    {
      Console.WriteLine("The data array contains {0} elements.", data.Length);
      var types = new Dictionary<Type, int>();
      foreach (var o in data)
      {
// ReSharper disable CompareNonConstrainedGenericWithNull
        Type type = o != null ? o.GetType() : typeof(void);
// ReSharper restore CompareNonConstrainedGenericWithNull
        if (!types.ContainsKey(type))
          types.Add(type, 0);
        ++types[type];
      }
      var typeList = new List<KeyValuePair<Type, int>>(types);
      typeList.Sort((one, two) => one.Value.CompareTo(two.Value));
      foreach (var entry in typeList)
        Console.WriteLine("{0}: {1}", entry.Key != typeof(void) ? entry.Key.ToString() : "<null>", entry.Value);
    }
  }
}
