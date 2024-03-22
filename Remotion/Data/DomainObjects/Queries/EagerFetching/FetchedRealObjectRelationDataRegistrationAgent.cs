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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Implements <see cref="IRelationEndPointRegistrationAgent"/> for non-virtual object-valued relation end-points.
  /// </summary>
  [Serializable]
  public class FetchedRealObjectRelationDataRegistrationAgent : FetchedRelationDataRegistrationAgentBase
  {
    public FetchedRealObjectRelationDataRegistrationAgent ()
    {
    }

    public override void GroupAndRegisterRelatedObjects (
        IRelationEndPointDefinition relationEndPointDefinition,
        ICollection<ILoadedObjectData> originatingObjects,
        ICollection<LoadedObjectDataWithDataSourceData> relatedObjects)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull("originatingObjects", originatingObjects);
      ArgumentUtility.CheckNotNull("relatedObjects", relatedObjects);

      if (relationEndPointDefinition.IsVirtual)
      {
        throw new ArgumentException(
            "Only non-virtual object-valued relation end-points can be handled by this registration agent.",
            "relationEndPointDefinition");
      }

      // Real end-point data is automatically registered when the DataContainer is registered, so we don't have anything to do here, apart from
      // checking that the objects fit the end-point definition.

      CheckOriginatingObjects(relationEndPointDefinition, originatingObjects);
      CheckRelatedObjects(relationEndPointDefinition, relatedObjects);
    }
  }
}
