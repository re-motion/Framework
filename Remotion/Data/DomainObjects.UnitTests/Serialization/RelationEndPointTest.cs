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
  public class RelationEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      var id = RelationEndPointID.Create(DomainObjectIDs.Computer1, ReflectionMappingHelper.GetPropertyName (typeof (Computer), "Employee"));
      _endPoint = new TestableRelationEndPoint (TestableClientTransaction, id);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RelationEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void RelationEndPointIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void RelationEndPointIsFlattenedSerializable ()
    {
      RelationEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint, Is.Not.Null);
      Assert.That (deserializedEndPoint, Is.Not.SameAs (_endPoint));
    }

    [Test]
    public void RelationEndPoint_Content ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedEndPoint.Definition, Is.SameAs (_endPoint.Definition));
      Assert.That (deserializedEndPoint.ID, Is.EqualTo (_endPoint.ID));
    }
  }
}
