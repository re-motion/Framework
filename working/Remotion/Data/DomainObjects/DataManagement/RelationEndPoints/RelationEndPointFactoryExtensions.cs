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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Implements extension methods for the <see cref="IRelationEndPointFactory"/> interface.
  /// </summary>
  public static class RelationEndPointFactoryExtensions
  {
    public static IVirtualEndPoint CreateVirtualEndPoint (this IRelationEndPointFactory endPointFactory, RelationEndPointID endPointID, bool markDataComplete)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (!endPointID.Definition.IsVirtual)
        throw new ArgumentException ("The RelationEndPointID must identify a virtual end-point.", "endPointID");

      if (endPointID.Definition.Cardinality == CardinalityType.One)
      {
        var virtualObjectEndPoint = endPointFactory.CreateVirtualObjectEndPoint (endPointID);
        if (markDataComplete)
          virtualObjectEndPoint.MarkDataComplete (null);
        return virtualObjectEndPoint;
      }
      else
      {
        var collectionEndPoint = endPointFactory.CreateCollectionEndPoint (endPointID);
        if (markDataComplete)
          collectionEndPoint.MarkDataComplete (new DomainObject[0]);
        return collectionEndPoint;
      }
    }
  }
}