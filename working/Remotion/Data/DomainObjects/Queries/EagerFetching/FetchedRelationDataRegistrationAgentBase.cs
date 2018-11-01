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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Provides common functionality for implementations of <see cref="IFetchedRelationDataRegistrationAgent"/>.
  /// </summary>
  [Serializable]
  public abstract class FetchedRelationDataRegistrationAgentBase : IFetchedRelationDataRegistrationAgent
  {
    public abstract void GroupAndRegisterRelatedObjects (IRelationEndPointDefinition relationEndPointDefinition, ICollection<ILoadedObjectData> originatingObjects, ICollection<LoadedObjectDataWithDataSourceData> relatedObjects);

    protected void CheckOriginatingObjects (IRelationEndPointDefinition relationEndPointDefinition, IEnumerable<ILoadedObjectData> originatingObjects)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("originatingObjects", originatingObjects);

      foreach (var originatingObject in originatingObjects)
      {
        if (!originatingObject.IsNull)
          CheckClassDefinitionOfOriginatingObject (relationEndPointDefinition, originatingObject);
      }
    }

    protected void CheckRelatedObjects (IRelationEndPointDefinition relationEndPointDefinition, IEnumerable<LoadedObjectDataWithDataSourceData> relatedObjects)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("relatedObjects", relatedObjects);

      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();

      foreach (var relatedObject in relatedObjects)
      {
        if (!relatedObject.IsNull)
          CheckClassDefinitionOfRelatedObject (relationEndPointDefinition, oppositeEndPointDefinition, relatedObject.LoadedObjectData.ObjectID);
      }
    }

    private void CheckClassDefinitionOfOriginatingObject (IRelationEndPointDefinition relationEndPointDefinition, ILoadedObjectData originatingObject)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("originatingObject", originatingObject);

      var relationEndPointDefinitionInheritanceRoot = relationEndPointDefinition.ClassDefinition.GetInheritanceRootClass();
      var originatingObjectInheritanceRoot = originatingObject.ObjectID.ClassDefinition.GetInheritanceRootClass();

      if (ReferenceEquals (relationEndPointDefinitionInheritanceRoot, originatingObjectInheritanceRoot))
        return;

      var message = string.Format (
          "Cannot register relation end-point '{0}' for domain object '{1}'. The end-point belongs to an object of "
          + "class '{2}' but the domain object has class '{3}'.",
          relationEndPointDefinition.PropertyName,
          originatingObject.ObjectID,
          relationEndPointDefinition.ClassDefinition.ID,
          originatingObject.ObjectID.ClassDefinition.ID);

      throw new InvalidOperationException (message);
    }

    private void CheckClassDefinitionOfRelatedObject (
        IRelationEndPointDefinition relationEndPointDefinition,
        IRelationEndPointDefinition oppositeEndPointDefinition, 
        ObjectID relatedObjectID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("oppositeEndPointDefinition", oppositeEndPointDefinition);
      ArgumentUtility.CheckNotNull ("relatedObjectID", relatedObjectID);
      

      if (!oppositeEndPointDefinition.ClassDefinition.IsSameOrBaseClassOf (relatedObjectID.ClassDefinition))
      {
        var message = string.Format (
            "Cannot associate object '{0}' with the relation end-point '{1}'. An object of type '{2}' was expected.",
            relatedObjectID,
            relationEndPointDefinition.PropertyName,
            oppositeEndPointDefinition.ClassDefinition.ClassType);

        throw new InvalidOperationException (message);
      }
    }

    protected IEnumerable<Tuple<ObjectID, LoadedObjectDataWithDataSourceData>> GetForeignKeysForVirtualEndPointDefinition (
        IEnumerable<LoadedObjectDataWithDataSourceData> loadedObjectData, 
        VirtualRelationEndPointDefinition virtualEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("loadedObjectData", loadedObjectData);
      ArgumentUtility.CheckNotNull ("virtualEndPointDefinition", virtualEndPointDefinition);

      var oppositeEndPointDefinition = (RelationEndPointDefinition) virtualEndPointDefinition.GetOppositeEndPointDefinition ();
      return from data in loadedObjectData
             where !data.IsNull
             let dataContainer = CheckRelatedObjectAndGetDataContainer (
                 data,
                 virtualEndPointDefinition,
                 oppositeEndPointDefinition)
             let originatingObjectID = (ObjectID) dataContainer.GetValueWithoutEvents (oppositeEndPointDefinition.PropertyDefinition, ValueAccess.Current)
             select Tuple.Create (originatingObjectID, data);
    }

    private DataContainer CheckRelatedObjectAndGetDataContainer (
        LoadedObjectDataWithDataSourceData relatedObject,
        IRelationEndPointDefinition relationEndPointDefinition, 
        IRelationEndPointDefinition oppositeEndPointDefinition)
    {
      CheckClassDefinitionOfRelatedObject (relationEndPointDefinition, oppositeEndPointDefinition, relatedObject.LoadedObjectData.ObjectID);
      return relatedObject.DataSourceData;
    }
  }
}