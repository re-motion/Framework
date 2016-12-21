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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DomainObjectCollectionTest : ClientTransactionBaseTest
  {
    [Test]
    public void DomainObjectCollection_IsSerializable ()
    {
      var collection = new DomainObjectCollection ();
      collection.Add (DomainObjectIDs.Order1.GetObject<Order> ());

      DomainObjectCollection deserializedCollection = Serializer.SerializeAndDeserialize (collection);
      Assert.That (deserializedCollection.Count, Is.EqualTo (1));
      Assert.That (deserializedCollection.Contains (DomainObjectIDs.Order1), Is.True);
      Assert.That (deserializedCollection[0].ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void DomainObjectCollection_StandAlone_Contents ()
    {
      var collection = new DomainObjectCollection (typeof (Order));
      collection.Add (DomainObjectIDs.Order1.GetObject<Order> ());

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);
      Assert.That (deserializedCollection.Count, Is.EqualTo (1));
      Assert.That (deserializedCollection.Contains (DomainObjectIDs.Order1), Is.True);
      Assert.That (deserializedCollection[0].ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (deserializedCollection.RequiredItemType, Is.EqualTo (typeof (Order)));
      Assert.That (deserializedCollection.IsReadOnly, Is.False);
      Assert.That (DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (deserializedCollection), Is.Null);
    }

    [Test]
    public void DomainObjectCollection_StandAlone_Data ()
    {
      var collection = new DomainObjectCollection (typeof (Order));
      collection.Add (DomainObjectIDs.Order1.GetObject<Order> ());

      DomainObjectCollection deserializedCollection = SerializeAndDeserialize (collection);

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (deserializedCollection, typeof (Order));
      Assert.That (deserializedCollection.Count, Is.EqualTo (1));
      Assert.That (deserializedCollection[0].ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void DomainObjectCollection_Associated ()
    {
      var customer1 = DomainObjectIDs.Customer1.GetObject<Customer> ();
      var collection = customer1.Orders;
      var endPointID = collection.AssociatedEndPointID;
      var relatedIDs = collection.Select (obj => obj.ID).ToArray();

      var deserializedCollectionAndTransaction = Serializer.SerializeAndDeserialize (Tuple.Create (collection, TestableClientTransaction));
      var deserializedCollection = deserializedCollectionAndTransaction.Item1;
      var deserializedTransaction = deserializedCollectionAndTransaction.Item2;

      var deserializedEndPoint = deserializedTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID);
      Assert.That (DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (deserializedCollection), Is.SameAs (deserializedEndPoint));

      var deserializedData = DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (deserializedCollection);
      DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (deserializedData);

      Assert.That (collection.Select (obj => obj.ID).ToArray (), Is.EqualTo (relatedIDs));
    }

    [Test]
    public void DomainObjectCollection_Events_Contents ()
    {
      var collection = new DomainObjectCollection (typeof (Order)) {DomainObjectIDs.Order1.GetObject<Order> ()};

      var eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      var deserializedCollectionAndEventReceiver = Serializer.SerializeAndDeserialize (Tuple.Create (collection, eventReceiver));
      var deserializedCollection = deserializedCollectionAndEventReceiver.Item1;
      var deserializedEventReceiver = deserializedCollectionAndEventReceiver.Item2;

      Assert.That (deserializedEventReceiver.HasAddedEventBeenCalled, Is.False);
      Assert.That (deserializedEventReceiver.HasAddingEventBeenCalled, Is.False);
      Assert.That (deserializedEventReceiver.HasRemovedEventBeenCalled, Is.False);
      Assert.That (deserializedEventReceiver.HasRemovingEventBeenCalled, Is.False);

      deserializedCollection.Add (Order.NewObject());
      deserializedCollection.RemoveAt (0);

      Assert.That (deserializedEventReceiver.HasAddedEventBeenCalled, Is.True);
      Assert.That (deserializedEventReceiver.HasAddingEventBeenCalled, Is.True);
      Assert.That (deserializedEventReceiver.HasRemovedEventBeenCalled, Is.True);
      Assert.That (deserializedEventReceiver.HasRemovingEventBeenCalled, Is.True);
    }

    [Test]
    public void DomainObjectCollection_ReadOnlyContents ()
    {
      var collection = new DomainObjectCollection (typeof (Order));
      collection = collection.Clone (true);

      var deserializedCollection = SerializeAndDeserialize (collection);
      Assert.That (deserializedCollection.IsReadOnly, Is.True);
    }

    private DomainObjectCollection SerializeAndDeserialize (DomainObjectCollection source)
    {
      return Serializer.SerializeAndDeserialize (source);
    }
  }
}
