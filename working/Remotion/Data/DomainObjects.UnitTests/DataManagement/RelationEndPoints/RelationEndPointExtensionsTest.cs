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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointExtensionsTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetEndPointWithOppositeDefinition_Object ()
    {
      var id = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      var customer = DomainObjectIDs.Customer1.GetObject<Customer> ();
      var oppositeEndPoint = endPoint.GetEndPointWithOppositeDefinition<ICollectionEndPoint> (customer);

      var oppositeID = RelationEndPointID.Create(customer.ID, endPoint.Definition.GetOppositeEndPointDefinition());
      Assert.That (oppositeEndPoint, Is.SameAs (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (oppositeID)));
    }

    [Test]
    public void GetEndPointWithOppositeDefinition_Object_Null ()
    {
      var id = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      var oppositeEndPoint = endPoint.GetEndPointWithOppositeDefinition<ICollectionEndPoint> ((DomainObject) null);

      Assert.That (oppositeEndPoint, Is.InstanceOf (typeof (NullCollectionEndPoint)));
      var expectedID = RelationEndPointID.Create(null, endPoint.Definition.GetOppositeEndPointDefinition ());
      Assert.That (oppositeEndPoint.ID, Is.EqualTo (expectedID));
    }

    [Test]
    public void GetEndPointWithOppositeDefinition_ID ()
    {
      var id = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      var customer = DomainObjectIDs.Customer1.GetObject<Customer> ();
      var oppositeEndPoint = endPoint.GetEndPointWithOppositeDefinition<ICollectionEndPoint> (customer.ID);

      var oppositeID = RelationEndPointID.Create(customer.ID, endPoint.Definition.GetOppositeEndPointDefinition ());
      Assert.That (oppositeEndPoint, Is.SameAs (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (oppositeID)));
    }

    [Test]
    public void GetEndPointWithOppositeDefinition_ID_Null ()
    {
      var id = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      var oppositeEndPoint = endPoint.GetEndPointWithOppositeDefinition<ICollectionEndPoint> ((ObjectID) null);

      Assert.That (oppositeEndPoint, Is.InstanceOf (typeof (NullCollectionEndPoint)));
      var expectedID = RelationEndPointID.Create(null, endPoint.Definition.GetOppositeEndPointDefinition ());
      Assert.That (oppositeEndPoint.ID, Is.EqualTo (expectedID));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The opposite end point 'null/Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' is of type "
        + "'Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.NullCollectionEndPoint', not of type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.IObjectEndPoint'.")]
    public void GetEndPointWithOppositeDefinition_ID_InvalidType ()
    {
      var id = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (id, null);

      endPoint.GetEndPointWithOppositeDefinition<IObjectEndPoint> ((ObjectID) null);
    }
  }
}