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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints
{
  public class TestableCompleteVirtualEndPointLoadState : CompleteVirtualEndPointLoadStateBase<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>
  {
    private IRealObjectEndPoint[] _stubbedOriginalOppositeEndPoints;
    private DomainObject[] _stubbedOriginalItemsWithoutEndPoints;

    public TestableCompleteVirtualEndPointLoadState (
        IVirtualEndPointDataManager dataManager, IRelationEndPointProvider endPointProvider, IClientTransactionEventSink transactionEventSink)
        : base(dataManager, endPointProvider, transactionEventSink)
    {
    }

    public override object GetData (IVirtualEndPoint<object> endPoint)
    {
      throw new NotImplementedException();
    }

    public override object GetOriginalData (IVirtualEndPoint<object> endPoint)
    {
      throw new NotImplementedException();
    }

    public override void SetDataFromSubTransaction (
        IVirtualEndPoint<object> endPoint, IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager> sourceLoadState)
    {
      throw new NotImplementedException();
    }

    public void StubOriginalOppositeEndPoints (IRealObjectEndPoint[] originalOppositeEndPoints)
    {
      _stubbedOriginalOppositeEndPoints = originalOppositeEndPoints;
    }

    public void StubOriginalItemsWithoutEndPoints (DomainObject[] items)
    {
      _stubbedOriginalItemsWithoutEndPoints = items;
    }

    protected override IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ()
    {
      return _stubbedOriginalOppositeEndPoints;
    }

    protected override IEnumerable<DomainObject> GetOriginalItemsWithoutEndPoints ()
    {
      return _stubbedOriginalItemsWithoutEndPoints;
    }
  }
}
