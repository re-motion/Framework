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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class RelationEndPointManagerTest : ClientTransactionBaseTest
  {
    private RelationEndPointManager _relationEndPointManager;

    public override void SetUp ()
    {
      base.SetUp ();
      _relationEndPointManager = (RelationEndPointManager) DataManagerTestHelper.GetRelationEndPointManager (TestableClientTransaction.DataManager);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RelationEndPointManager' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void RelationEndPointManagerIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_relationEndPointManager);
    }

    [Test]
    public void RelationEndPointManagerIsFlattenedSerializable ()
    {
      RelationEndPointManager deserializedManager = FlattenedSerializer.SerializeAndDeserialize (_relationEndPointManager);
      Assert.That (deserializedManager, Is.Not.Null);
      Assert.That (deserializedManager, Is.Not.SameAs (_relationEndPointManager));
    }

    [Test]
    public void RelationEndPointManager_Content ()
    {
      DomainObjectIDs.Order1.GetObject<Order> ().OrderItems.EnsureDataComplete();
      Assert.That (_relationEndPointManager.RelationEndPoints.Count, Is.EqualTo (7));

      var deserializedManager = (RelationEndPointManager) DataManagerTestHelper.GetRelationEndPointManager (
          Serializer.SerializeAndDeserialize (TestableClientTransaction.DataManager));

      Assert.That (deserializedManager.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedManager.ClientTransaction, Is.InstanceOf (typeof (TestableClientTransaction)));
      Assert.That (deserializedManager.ClientTransaction, Is.Not.SameAs (TestableClientTransaction));
      Assert.That (deserializedManager.LazyLoader, Is.Not.Null);
      Assert.That (deserializedManager.LazyLoader, Is.TypeOf (_relationEndPointManager.LazyLoader.GetType()));
      Assert.That (deserializedManager.EndPointFactory, Is.Not.Null);
      Assert.That (deserializedManager.EndPointFactory, Is.TypeOf (_relationEndPointManager.EndPointFactory.GetType()));
      Assert.That (deserializedManager.RegistrationAgent, Is.TypeOf (_relationEndPointManager.RegistrationAgent.GetType ()));
      Assert.That (deserializedManager.DataContainerEndPointsRegistrationAgent, Is.Not.Null);

      Assert.That (deserializedManager.RelationEndPoints.Count, Is.EqualTo (7));

      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));
      var endPoint = (ICollectionEndPoint) deserializedManager.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (endPoint.ClientTransaction, Is.SameAs (deserializedManager.ClientTransaction));
    }
  }
}
