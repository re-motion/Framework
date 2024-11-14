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
using Microsoft.Extensions.Logging;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Implements <see cref="IRelationEndPointRegistrationAgent"/> for virtual object-valued relation end-points.
  /// </summary>
  public class FetchedVirtualObjectRelationDataRegistrationAgent : FetchedRelationDataRegistrationAgentBase
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<FetchedVirtualObjectRelationDataRegistrationAgent>();

    private readonly IVirtualEndPointProvider _virtualEndPointProvider;

    public FetchedVirtualObjectRelationDataRegistrationAgent (IVirtualEndPointProvider virtualEndPointProvider)
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

      if (relationEndPointDefinition.Cardinality != CardinalityType.One || !relationEndPointDefinition.IsVirtual)
      {
        throw new ArgumentException(
            "Only virtual object-valued relation end-points can be handled by this registration agent.",
            "relationEndPointDefinition");
      }

      var groupedRelatedObjects = CorrelateRelatedObjects(relatedObjects, relationEndPointDefinition);

      CheckOriginatingObjects(relationEndPointDefinition, originatingObjects);

      RegisterEndPointData(relationEndPointDefinition, originatingObjects, groupedRelatedObjects);
    }

    private IDictionary<ObjectID, ILoadedObjectData> CorrelateRelatedObjects (
        IEnumerable<LoadedObjectDataWithDataSourceData> relatedObjects,
        IRelationEndPointDefinition relationEndPointDefinition)
    {
      var relatedObjectsWithForeignKey = GetForeignKeysForVirtualEndPointDefinition(relatedObjects, relationEndPointDefinition);
      var dictionary = new Dictionary<ObjectID, ILoadedObjectData>();
      foreach (var tuple in relatedObjectsWithForeignKey)
      {
        try
        {
          dictionary.Add(tuple.Item1, tuple.Item2.LoadedObjectData);
        }
        catch (ArgumentException ex)
        {
          var message = string.Format(
              "Two items in the related object result set point back to the same object. This is not allowed in a 1:1 relation. "
              + "Object 1: '{0}'. Object 2: '{1}'. Foreign key property: '{2}'",
              dictionary[tuple.Item1].ObjectID,
              tuple.Item2.LoadedObjectData.ObjectID,
              relationEndPointDefinition.GetOppositeEndPointDefinition().PropertyName);
          throw new InvalidOperationException(message, ex);
        }
      }
      return dictionary;
    }

    private void RegisterEndPointData (
       IRelationEndPointDefinition relationEndPointDefinition,
       IEnumerable<ILoadedObjectData> originatingObjects,
       IDictionary<ObjectID, ILoadedObjectData> groupedRelatedObjects)
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
            var relatedObjectData = relatedObjectsByOriginalObject.GetValueOrDefault(originatingObject.ObjectID) ?? new NullLoadedObjectData();
            var relatedObject = relatedObjectData.GetDomainObjectReference();
            if (relationEndPointDefinition.IsMandatory && relatedObject == null)
            {
              var message = string.Format(
                  "The fetched mandatory relation property '{0}' on object '{1}' contains no related object.",
                  relationEndPointDefinition.PropertyName,
                  relationEndPointID.ObjectID);
              throw new InvalidOperationException(message);
            }
            if (!TrySetVirtualObjectEndPointData(relationEndPointID, relatedObject))
              s_logger.LogDebug("Relation data for relation end-point '{0}' is discarded; the end-point has already been loaded.", relationEndPointID);
          }
        }
      }
    }

    private bool TrySetVirtualObjectEndPointData (RelationEndPointID endPointID, DomainObject? item)
    {
      Assertion.DebugIsNotNull(endPointID.ObjectID, "endPointID.ObjectID != null");
      var endPoint = (IVirtualObjectEndPoint)_virtualEndPointProvider.GetOrCreateVirtualEndPoint(endPointID);
      if (endPoint.IsDataComplete)
        return false;

      endPoint.MarkDataComplete(item);
      return true;
    }
  }
}
