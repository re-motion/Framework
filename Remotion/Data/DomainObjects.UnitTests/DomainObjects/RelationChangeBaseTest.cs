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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  public class RelationChangeBaseTest : ClientTransactionBaseTest
  {
    protected void CheckTouching (Action modification, TestDomainBase foreignKeyObject, string simpleForeignKeyPropertyName,
                                params RelationEndPointID[] endPointsInvolved)
    {
      // Ensure all end points are loaded into the RelationEndPointManager before trying to check them
      foreach (RelationEndPointID id in endPointsInvolved)
      {
        TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad(id);
      }

      if (foreignKeyObject != null)
      {
        Assert.IsFalse(
            foreignKeyObject.Properties[foreignKeyObject.GetPublicDomainObjectType(), simpleForeignKeyPropertyName].HasBeenTouched,
            "ObjectID before modification");
      }

      foreach (RelationEndPointID id in endPointsInvolved)
        Assert.IsFalse(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id).HasBeenTouched, id + " before modification");

      modification();

      if (foreignKeyObject != null)
      {
        Assert.That(
            foreignKeyObject.Properties[foreignKeyObject.GetPublicDomainObjectType(), simpleForeignKeyPropertyName].HasBeenTouched,
            Is.True,
            "ObjectID after modification");
      }

      foreach (RelationEndPointID id in endPointsInvolved)
        Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(id).HasBeenTouched, Is.True, id + " after modification");
    }
  }
}
