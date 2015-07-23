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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class VirtualObjectEndPointDataManagerFactoryTest : StandardMappingTest
  {
    private VirtualObjectEndPointDataManagerFactory _factory;
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      _factory = new VirtualObjectEndPointDataManagerFactory ();
      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
    }

    [Test]
    public void Create ()
    {
      var result = _factory.CreateEndPointDataManager (_endPointID);

      Assert.That (result, Is.TypeOf (typeof (VirtualObjectEndPointDataManager)));
      Assert.That (((VirtualObjectEndPointDataManager) result).EndPointID, Is.SameAs (_endPointID));
    }
  }
}