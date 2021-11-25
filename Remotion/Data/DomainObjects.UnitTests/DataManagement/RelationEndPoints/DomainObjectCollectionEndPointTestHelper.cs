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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  public static class DomainObjectCollectionEndPointTestHelper
  {
    public static IRealObjectEndPoint GetFakeOppositeEndPoint (DomainObject item)
    {
      var fakeEndPoint = MockRepository.GenerateStub<IRealObjectEndPoint>();
      fakeEndPoint.Stub(stub => stub.ObjectID).Return(item.ID);
      fakeEndPoint.Stub(stub => stub.GetDomainObject()).Return(item);
      fakeEndPoint.Stub(stub => stub.GetDomainObjectReference()).Return(item);
      return fakeEndPoint;
    }

    public static void FillCollectionEndPointWithInitialContents (DomainObjectCollectionEndPoint endPoint, IEnumerable<DomainObject> initialContents)
    {
      var dataManager = ClientTransactionTestHelper.GetDataManager(endPoint.ClientTransaction);
      var domainObjects = initialContents.ToArray();
      foreach (var domainObject in domainObjects)
      {
        var oppositeEndPointID = RelationEndPointID.Create(domainObject.ID, endPoint.Definition.GetOppositeEndPointDefinition());
        var oppositeEndPoint = (IRealObjectEndPoint) dataManager.GetRelationEndPointWithLazyLoad(oppositeEndPointID);
        endPoint.RegisterOriginalOppositeEndPoint(oppositeEndPoint);
      }
      endPoint.MarkDataComplete(domainObjects);
    }

    public static IDomainObjectCollectionEndPointLoadState GetLoadState (DomainObjectCollectionEndPoint collectionEndPoint)
    {
      return (IDomainObjectCollectionEndPointLoadState) PrivateInvoke.GetNonPublicField(collectionEndPoint, "_loadState");
    }

    public static void SetLoadState (DomainObjectCollectionEndPoint collectionEndPoint, IDomainObjectCollectionEndPointLoadState loadState)
    {
      PrivateInvoke.SetNonPublicField(collectionEndPoint, "_loadState", loadState);
    }
  }
}
