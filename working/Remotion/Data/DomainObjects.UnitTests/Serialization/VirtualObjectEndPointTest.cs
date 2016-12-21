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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class VirtualObjectEndPointTest : ClientTransactionBaseTest
  {
    private VirtualObjectEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Employee3, ReflectionMappingHelper.GetPropertyName (typeof (Employee), "Computer"));
      _endPoint = (VirtualObjectEndPoint) 
          ((StateUpdateRaisingVirtualObjectEndPointDecorator) TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad (endPointID))
          .InnerEndPoint;
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage =
      "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualObjectEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void VirtualObjectEndPoint_IsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void VirtualObjectEndPoint_IsFlattenedSerializable ()
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
    public void TouchedChangedContent ()
    {
      _endPoint.CreateSetCommand (DomainObjectIDs.Computer2.GetObject<Computer> ()).Perform ();
      _endPoint.Touch();

      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      Assert.That (deserializedEndPoint.Definition, Is.SameAs (_endPoint.Definition));
      Assert.That (deserializedEndPoint.HasBeenTouched, Is.True);
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Computer2));
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.Computer1));
    }

    [Test]
    public void Internals ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      Assert.That (deserializedEndPoint.LazyLoader, Is.Not.Null);
      Assert.That (deserializedEndPoint.EndPointProvider, Is.Not.Null);
      Assert.That (deserializedEndPoint.TransactionEventSink, Is.Not.Null);
      Assert.That (deserializedEndPoint.DataManagerFactory, Is.Not.Null);

      var loadState = VirtualObjectEndPointTestHelper.GetLoadState(deserializedEndPoint);
      Assert.That (loadState, Is.Not.Null);
    }
  }
}
