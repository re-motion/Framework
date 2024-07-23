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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Implements <see cref="IRelationEndPointRegistrationAgent"/> for collection-valued relation end-points.
  /// </summary>
  public class FetchedCollectionRelationDataRegistrationAgent : FetchedRelationDataRegistrationAgentBase
  {
    private static readonly ILog s_log = LogManager.GetLogger(typeof(FetchedCollectionRelationDataRegistrationAgent));

    private readonly IVirtualEndPointProvider _virtualEndPointProvider;

    public FetchedCollectionRelationDataRegistrationAgent (IVirtualEndPointProvider virtualEndPointProvider)
    {
      ArgumentUtility.CheckNotNull("virtualEndPointProvider", virtualEndPointProvider);
      _virtualEndPointProvider = virtualEndPointProvider;
    }

    public IVirtualEndPointProvider VirtualEndPointProvider
    {
      get { return _virtualEndPointProvider; }
    }

    public override void GroupAndRegisterRelatedObjects (
        IRelationEndPointDefinition relationEndPointDefinition,
        ICollection<ILoadedObjectData> originatingObjects,
        ICollection<LoadedObjectDataWithDataSourceData> relatedObjects)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull("originatingObjects", originatingObjects);
      ArgumentUtility.CheckNotNull("relatedObjects", relatedObjects);

      if (relationEndPointDefinition.Cardinality != CardinalityType.Many || relationEndPointDefinition.IsAnonymous)
        throw new ArgumentException("Only collection-valued relations can be handled by this registration agent.", "relationEndPointDefinition");

      var groupedRelatedObjects = CorrelateRelatedObjects(relatedObjects, relationEndPointDefinition);

      CheckOriginatingObjects(relationEndPointDefinition, originatingObjects);

      RegisterEndPointData(relationEndPointDefinition, originatingObjects, groupedRelatedObjects);
    }

    private ILookup<ObjectID, ILoadedObjectData> CorrelateRelatedObjects (
        IEnumerable<LoadedObjectDataWithDataSourceData> relatedObjects,
        IRelationEndPointDefinition relationEndPointDefinition)
    {
      var relatedObjectsWithForeignKey = GetForeignKeysForVirtualEndPointDefinition(relatedObjects, relationEndPointDefinition);
      return relatedObjectsWithForeignKey.ToLookup(k => k.Item1, k => k.Item2.LoadedObjectData);
    }

    private void RegisterEndPointData (
       IRelationEndPointDefinition relationEndPointDefinition,
       IEnumerable<ILoadedObjectData> originatingObjects,
       ILookup<ObjectID, ILoadedObjectData> groupedRelatedObjects)
    {
      var relatedObjectsByOriginalObject = groupedRelatedObjects;
      foreach (var originatingObject in originatingObjects)
      {
        if (!originatingObject.IsNull)
        {
          Assertion.DebugIsNotNull(originatingObject.ObjectID, "originatingObject.ObjectID != null when originatingObject.IsNull == false");
          if (originatingObject.ObjectID.ClassDefinition.IsRelationEndPoint(relationEndPointDefinition))
          {
            var relationEndPointID = RelationEndPointID.Create(originatingObject.ObjectID, relationEndPointDefinition);
            var relatedObjectData = relatedObjectsByOriginalObject[originatingObject.ObjectID];
            var relatedObjects = relatedObjectData.Select<ILoadedObjectData, DomainObject>(
                data =>
                {
                  Assertion.IsFalse(data.IsNull, "data.IsNull == false");
                  var domainObjectReference = data.GetDomainObjectReference();
                  Assertion.DebugIsNotNull(domainObjectReference, "data.GetDomainObjectReference() != null when data.IsNull == false");
                  return domainObjectReference;
                }).ToArray();

            if (relationEndPointDefinition.IsMandatory && relatedObjects.Length == 0)
            {
              var message = string.Format(
                  "The fetched mandatory collection property '{0}' on object '{1}' contains no items.",
                  relationEndPointDefinition.PropertyName,
                  relationEndPointID.ObjectID);
              throw new InvalidOperationException(message);
            }

            if (!TrySetCollectionEndPointData(relationEndPointID, relatedObjects))
              s_log.DebugFormat("Relation data for relation end-point '{0}' is discarded; the end-point has already been loaded.", relationEndPointID);
          }
        }
      }
    }

    private bool TrySetCollectionEndPointData (RelationEndPointID endPointID, DomainObject[] items)
    {
      Assertion.DebugIsNotNull(endPointID.ObjectID, "endPointID.ObjectID != null");
      var endPoint = (ICollectionEndPoint<ICollectionEndPointData>)_virtualEndPointProvider.GetOrCreateVirtualEndPoint(endPointID);
      if (endPoint.IsDataComplete)
        return false;

      endPoint.MarkDataComplete(items);
      return true;
    }
  }
}
