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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class RealObjectEndPointTest : ClientTransactionBaseTest
  {
    private RealObjectEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      DomainObjectIDs.Computer1.GetObject<Computer> ();
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Computer1, ReflectionMappingHelper.GetPropertyName (typeof (Computer), "Employee"));
      _endPoint = (RealObjectEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
      "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void RealObjectEndPoint_IsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void RealObjectEndPoint_IsFlattenedSerializable ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint, Is.Not.Null);
      Assert.That (deserializedEndPoint, Is.Not.SameAs (_endPoint));
    }

    [Test]
    public void UntouchedContent ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void TouchedContent ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.Employee1);

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.Definition, Is.SameAs (_endPoint.Definition));
      Assert.That (deserializedEndPoint.HasBeenTouched, Is.True);
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Employee1));
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.Employee3));
    }

    [Test]
    public void ForeignKeyProperty ()
    {
      DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
      var id = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1, typeof (OrderTicket) + ".Order");
      var endPoint = (RealObjectEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad (id);
      Assert.That (endPoint.PropertyDefinition, Is.Not.Null);

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (endPoint);

      Assert.That (deserializedEndPoint.PropertyDefinition, Is.SameAs (endPoint.PropertyDefinition));
      Assert.That (deserializedEndPoint.ForeignKeyDataContainer, Is.Not.Null);
    }

    [Test]
    public void ForeignKeyDataContainer_IntegrationWithDataManager ()
    {
      DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
      var id = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1, typeof (OrderTicket) + ".Order");

      var deserializedDataManager = Serializer.SerializeAndDeserialize (TestableClientTransaction.DataManager);

      var deserializedEndPoint = (RealObjectEndPoint) deserializedDataManager.GetRelationEndPointWithLazyLoad (id);

      Assert.That (deserializedEndPoint.ForeignKeyDataContainer, Is.Not.Null);
      Assert.That (deserializedEndPoint.ForeignKeyDataContainer, Is.SameAs (deserializedDataManager.DataContainers[DomainObjectIDs.OrderTicket1]));
          }

    [Test]
    public void SyncState ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      var syncState = RealObjectEndPointTestHelper.GetSyncState (deserializedEndPoint);
      Assert.That (syncState, Is.Not.Null);
      Assert.That (syncState.GetType (), Is.SameAs (RealObjectEndPointTestHelper.GetSyncState (_endPoint).GetType ()));
    }

    [Test]
    public void InjectedProperties ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      Assert.That (deserializedEndPoint.EndPointProvider, Is.Not.Null);
      Assert.That (deserializedEndPoint.TransactionEventSink, Is.Not.Null);
    }
  }
}
