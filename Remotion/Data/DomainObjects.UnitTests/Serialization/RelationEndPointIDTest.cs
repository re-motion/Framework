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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class RelationEndPointIDTest : StandardMappingTest
  {
    private RelationEndPointID _id;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _id = RelationEndPointID.Create(DomainObjectIDs.Computer1, ReflectionMappingHelper.GetPropertyName (typeof (Computer), "Employee"));
    }


    [Test]
    public void RelationEndPointID_IsSerializable ()
    {
      RelationEndPointID deserializedEndPoint = Serializer.SerializeAndDeserialize (_id);
      Assert.That (deserializedEndPoint, Is.Not.Null);
      Assert.That (deserializedEndPoint, Is.Not.SameAs (_id));

      Assert.That (deserializedEndPoint, Is.EqualTo (_id));
    }

    [Test]
    public void RelationEndPointID_IsFlattenedSerializable ()
    {
      RelationEndPointID deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_id);
      Assert.That (deserializedEndPoint, Is.Not.Null);
      Assert.That (deserializedEndPoint, Is.Not.SameAs (_id));

      Assert.That (deserializedEndPoint, Is.EqualTo (_id));
    }

    [Test]
    public void RelationEndPointID_Content ()
    {
      RelationEndPointID deserializedID = Serializer.SerializeAndDeserialize (_id);
      Assert.That (deserializedID.ObjectID, Is.EqualTo (DomainObjectIDs.Computer1));
      Assert.That (deserializedID.Definition, Is.SameAs (_id.Definition));
    }

    [Test]
    public void RelationEndPointID_GetHashCode ()
    {
      RelationEndPointID deserializedID = Serializer.SerializeAndDeserialize (_id);
      Assert.That (deserializedID.GetHashCode (), Is.EqualTo (_id.GetHashCode ()));
    }
  }
}
