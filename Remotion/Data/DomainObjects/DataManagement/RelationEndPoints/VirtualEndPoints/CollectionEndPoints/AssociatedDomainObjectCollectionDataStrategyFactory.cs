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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Implements <see cref="IAssociatedDomainObjectCollectionDataStrategyFactory"/> by creating instances of <see cref="EndPointDelegatingDomainObjectCollectionData"/>.
  /// </summary>
  [Serializable]
  public class AssociatedDomainObjectCollectionDataStrategyFactory : IAssociatedDomainObjectCollectionDataStrategyFactory
  {
    private readonly IVirtualEndPointProvider _virtualEndPointProvider;

    public AssociatedDomainObjectCollectionDataStrategyFactory (IVirtualEndPointProvider virtualEndPointProvider)
    {
      ArgumentUtility.CheckNotNull("virtualEndPointProvider", virtualEndPointProvider);
      _virtualEndPointProvider = virtualEndPointProvider;
    }

    public IVirtualEndPointProvider VirtualEndPointProvider
    {
      get { return _virtualEndPointProvider; }
    }

    public IDomainObjectCollectionData CreateDataStrategyForEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      var requiredItemType = endPointID.Definition.GetOppositeEndPointDefinition().TypeDefinition.Type;
      return new ModificationCheckingDomainObjectCollectionDataDecorator(
          requiredItemType, new EndPointDelegatingDomainObjectCollectionData(endPointID, _virtualEndPointProvider));
    }
  }
}
