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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Decorates <see cref="IRelationEndPointFactory"/> instances by wrapping the created <see cref="IVirtualEndPoint"/> instances into decorators
  /// that cause <see cref="IVirtualEndPointStateUpdateListener"/> events to be raised.
  /// </summary>
  public class StateUpdateRaisingRelationEndPointFactoryDecorator : IRelationEndPointFactory
  {
    private readonly IRelationEndPointFactory _innerFactory;
    private readonly IVirtualEndPointStateUpdateListener _listener;

    public StateUpdateRaisingRelationEndPointFactoryDecorator (IRelationEndPointFactory innerFactory, IVirtualEndPointStateUpdateListener listener)
    {
      ArgumentUtility.CheckNotNull("innerFactory", innerFactory);
      ArgumentUtility.CheckNotNull("listener", listener);

      _innerFactory = innerFactory;
      _listener = listener;
    }

    public IRelationEndPointFactory InnerFactory
    {
      get { return _innerFactory; }
    }

    public IVirtualEndPointStateUpdateListener Listener
    {
      get { return _listener; }
    }

    public IRealObjectEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID, DataContainer dataContainer)
    {
      return _innerFactory.CreateRealObjectEndPoint(endPointID, dataContainer);
    }

    public IVirtualObjectEndPoint CreateVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      var endPoint = _innerFactory.CreateVirtualObjectEndPoint(endPointID);
      return new StateUpdateRaisingVirtualObjectEndPointDecorator(endPoint, _listener);
    }

    public IVirtualCollectionEndPoint CreateVirtualCollectionEndPoint (RelationEndPointID endPointID)
    {
      var endPoint = _innerFactory.CreateVirtualCollectionEndPoint(endPointID);
      return new StateUpdateRaisingVirtualCollectionEndPointDecorator(endPoint, _listener);
    }

    public IDomainObjectCollectionEndPoint CreateDomainObjectCollectionEndPoint (RelationEndPointID endPointID)
    {
      var endPoint = _innerFactory.CreateDomainObjectCollectionEndPoint(endPointID);
      return new StateUpdateRaisingDomainObjectCollectionEndPointDecorator(endPoint, _listener);
    }
  }
}
