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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public abstract class IntegrationTestBase : ClientTransactionBaseTest
  {
    protected void CheckQueryResult<T> (IEnumerable<T> query, params ObjectID[] expectedObjectIDs)
        where T : DomainObject
    {
      T[] results = query.ToArray();
      T[] expected = GetExpectedObjects<T>(expectedObjectIDs);
      if (expectedObjectIDs != null)
      {
        Assert.That(
            results.Length,
            Is.EqualTo(expected.Length),
            "Number of returned objects doesn't match; returned: " + string.Join(", ", results.Select(obj => obj.ID.ToString())));
      }
      Assert.That(results, Is.EquivalentTo(expected));
    }

    protected void CheckOrderedQueryResult<T> (IEnumerable<T> query, params ObjectID[] expectedObjectIDs)
        where T : DomainObject
    {
      T[] results = query.ToArray();
      T[] expected = GetExpectedObjects<T>(expectedObjectIDs);
      if (expectedObjectIDs != null)
      {
        Assert.That(
            results.Length,
            Is.EqualTo(expected.Length),
            "Number of returned objects doesn't match; returned: " + string.Join(", ", results.Select(obj => obj.ID.ToString())));
      }
      Assert.That(results, Is.EqualTo(expected));
    }

    protected T[] GetExpectedObjects<T> (params ObjectID[] expectedObjectIDs)
        where T: DomainObject
    {
      if(expectedObjectIDs==null)
        return new T[] { null };
      return (from id in expectedObjectIDs
              select (id == null ? null : (T)LifetimeService.GetObject(TestableClientTransaction, id, false))).ToArray();
    }

    protected void CheckDataContainersRegistered (params ObjectID[] objectIDs)
    {
      // check that related objects have been loaded
      foreach (var id in objectIDs)
        Assert.That(TestableClientTransaction.DataManager.DataContainers[id], Is.Not.Null);
    }

    protected void CheckDomainObjectCollectionRelationRegistered (
        ObjectID originatingObjectID, string shortPropertyName, bool checkOrdering, params ObjectID[] expectedRelatedObjectIDs)
    {
      var declaringType = originatingObjectID.ClassDefinition.Type;
      CheckDomainObjectCollectionRelationRegistered(originatingObjectID, declaringType, shortPropertyName, checkOrdering, expectedRelatedObjectIDs);
    }

    protected void CheckDomainObjectCollectionRelationRegistered (
        ObjectID originatingObjectID, Type declaringType, string shortPropertyName, bool checkOrdering, params ObjectID[] expectedRelatedObjectIDs)
    {
      var relationEndPointDefinition =
          originatingObjectID.ClassDefinition.GetMandatoryRelationEndPointDefinition(
              declaringType.FullName + "." + shortPropertyName);

      var endPointID = RelationEndPointID.Create(originatingObjectID, relationEndPointDefinition);
      var collectionEndPoint = (IDomainObjectCollectionEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID);
      Assert.That(collectionEndPoint, Is.Not.Null);

      var expectedRelatedObjects = expectedRelatedObjectIDs.Select(id => LifetimeService.GetObject(TestableClientTransaction, id, false)).ToArray();
      if (checkOrdering)
        Assert.That(collectionEndPoint.Collection, Is.EqualTo(expectedRelatedObjects));
      else
        Assert.That(collectionEndPoint.Collection, Is.EquivalentTo(expectedRelatedObjects));
    }

    protected void CheckObjectRelationRegistered (ObjectID originatingObjectID, string shortPropertyName, ObjectID expectedRelatedObjectID)
    {
      var declaringType = originatingObjectID.ClassDefinition.Type;
      CheckObjectRelationRegistered(originatingObjectID, declaringType, shortPropertyName, expectedRelatedObjectID);
    }

    protected void CheckObjectRelationRegistered (ObjectID originatingObjectID, Type declaringType, string shortPropertyName, ObjectID expectedRelatedObjectID)
    {
      var longPropertyName = declaringType.FullName + "." + shortPropertyName;
      var relationEndPointDefinition =
          originatingObjectID.ClassDefinition.GetMandatoryRelationEndPointDefinition(
              longPropertyName);

      var endPointID = RelationEndPointID.Create(originatingObjectID, relationEndPointDefinition);
      var objectEndPoint = (IObjectEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID);
      Assert.That(objectEndPoint, Is.Not.Null);
      Assert.That(objectEndPoint.OppositeObjectID, Is.EqualTo(expectedRelatedObjectID));
    }
  }
}
