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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class CollectionEndPointTest : ClientTransactionBaseTest
  {
    private CollectionEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      Dev.Null = DomainObjectIDs.Order1.GetObject<Order> ().OrderItems;
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));
      _endPoint = (CollectionEndPoint) 
          ((StateUpdateRaisingCollectionEndPointDecorator) TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID)).InnerEndPoint;
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.CollectionEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void CollectionEndPointIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void CollectionEndPointIsFlattenedSerializable ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint, Is.Not.Null);
      Assert.That (deserializedEndPoint, Is.Not.SameAs (_endPoint));
    }

    [Test]
    public void CollectionEndPoint_Content ()
    {
      _endPoint.Collection.Add (DomainObjectIDs.OrderItem5.GetObject<OrderItem>());

      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.Definition, Is.SameAs (_endPoint.Definition));
      Assert.That (deserializedEndPoint.HasBeenTouched, Is.True);

      Assert.That (deserializedEndPoint.Collection.Count, Is.EqualTo (3));
      Assert.That (deserializedEndPoint.Collection.Contains (DomainObjectIDs.OrderItem1), Is.True);
      Assert.That (deserializedEndPoint.Collection.Contains (DomainObjectIDs.OrderItem2), Is.True);
      Assert.That (deserializedEndPoint.Collection.Contains (DomainObjectIDs.OrderItem5), Is.True);
      Assert.That (deserializedEndPoint.Collection.IsReadOnly, Is.False);

      Assert.That (deserializedEndPoint.GetCollectionWithOriginalData().Count, Is.EqualTo (2));
      Assert.That (deserializedEndPoint.GetCollectionWithOriginalData().Contains (DomainObjectIDs.OrderItem1), Is.True);
      Assert.That (deserializedEndPoint.GetCollectionWithOriginalData().Contains (DomainObjectIDs.OrderItem2), Is.True);
      Assert.That (deserializedEndPoint.GetCollectionWithOriginalData().IsReadOnly, Is.True);
    }

    [Test]
    public void CollectionEndPoint_Touched ()
    {
      _endPoint.Touch ();

      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void CollectionEndPoint_Untouched ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void CollectionEndPoint_ClientTransaction ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.ClientTransaction, Is.Not.Null);
    }

    [Test]
    public void CollectionEndPoint_DelegatingDataMembers ()
    {
      _endPoint.Collection.Add (DomainObjectIDs.OrderItem5.GetObject<OrderItem>());

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      DomainObjectCollectionDataTestHelper.CheckAssociatedCollectionStrategy (
          deserializedEndPoint.Collection,
          _endPoint.Collection.RequiredItemType,
          deserializedEndPoint.ID);
    }

    [Test]
    public void CollectionEndPoint_ReplacedCollection ()
    {
      var newOpposites = _endPoint.Collection.Clone();
      _endPoint.CreateSetCollectionCommand (newOpposites).ExpandToAllRelatedObjects().NotifyAndPerform();
      
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.HasChanged, Is.True);

      var deserializedNewOpposites = deserializedEndPoint.Collection;
      deserializedEndPoint.Rollback ();
      
      Assert.That (deserializedEndPoint.HasChanged, Is.False);
      var deserializedOldOpposites = deserializedEndPoint.Collection;
      Assert.That (deserializedOldOpposites, Is.Not.SameAs (deserializedNewOpposites));
      Assert.That (deserializedOldOpposites, Is.Not.Null);
    }

    [Test]
    public void CollectionEndPoint_ReplacedCollection_ReferenceEqualityWithOtherCollection ()
    {
      var industrialSector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector> ();
      var oldOpposites = industrialSector.Companies;
      var newOpposites = industrialSector.Companies.Clone ();
      industrialSector.Companies = newOpposites;

      var tuple = Tuple.Create (TestableClientTransaction, industrialSector, oldOpposites, newOpposites);
      var deserializedTuple = Serializer.SerializeAndDeserialize (tuple);
      using (deserializedTuple.Item1.EnterDiscardingScope())
      {
        Assert.That (deserializedTuple.Item2.Companies, Is.SameAs (deserializedTuple.Item4));
        ClientTransaction.Current.Rollback();
        Assert.That (deserializedTuple.Item2.Companies, Is.SameAs (deserializedTuple.Item3));
      }
    }

    [Test]
    public void Serialization_IntegrationWithRelationEndPointMap ()
    {
      var deserializedTransactionMock = Serializer.SerializeAndDeserialize (TestableClientTransaction);
      var deserializedCollectionEndPoint = deserializedTransactionMock.DataManager.GetRelationEndPointWithLazyLoad (_endPoint.ID);
      Assert.That (deserializedCollectionEndPoint, Is.Not.Null);
    }

    [Test]
    public void Serialization_InjectedObjects ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      var originalEndPoint = new CollectionEndPoint (
          ClientTransaction.Current,
          endPointID,
          new SerializableCollectionEndPointCollectionManagerFake(),
          new SerializableLazyLoaderFake(),
          new SerializableRelationEndPointProviderFake(),
          new SerializableClientTransactionEventSinkFake(),
          new SerializableCollectionEndPointDataManagerFactoryFake());

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (originalEndPoint);

      var deserializedLoadState = PrivateInvoke.GetNonPublicField (deserializedEndPoint, "_loadState");
      Assert.That (deserializedLoadState, Is.Not.Null);

      Assert.That (deserializedEndPoint.CollectionManager, Is.Not.Null);
      Assert.That (deserializedEndPoint.LazyLoader, Is.Not.Null);
      Assert.That (deserializedEndPoint.EndPointProvider, Is.Not.Null);
      Assert.That (deserializedEndPoint.TransactionEventSink, Is.Not.Null);
      Assert.That (deserializedEndPoint.DataManagerFactory, Is.Not.Null);
    }
  }
}
