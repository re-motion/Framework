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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  public abstract class ObjectEndPoint : RelationEndPoint, IObjectEndPoint
  {
    protected ObjectEndPoint (ClientTransaction clientTransaction, RelationEndPointID id)
        : base(clientTransaction, id)
    {
      if (id.Definition.Cardinality != CardinalityType.One)
        throw new ArgumentException("End point ID must refer to an end point with cardinality 'One'.", "id");

      Assertion.IsFalse(id.Definition.IsAnonymous);
    }

    public abstract ObjectID? OppositeObjectID { get; }
    public abstract ObjectID? OriginalOppositeObjectID { get; }

    public abstract IDomainObject? GetOppositeObject ();
    public abstract IDomainObject? GetOriginalOppositeObject ();

    protected abstract void SetOppositeObjectDataFromSubTransaction (IObjectEndPoint sourceObjectEndPoint);

    public abstract IDataManagementCommand CreateSetCommand (IDomainObject? newRelatedObject);

    public override void ValidateMandatory ()
    {
      if (OppositeObjectID == null)
      {
        throw new MandatoryRelationNotSetException(
            GetDomainObjectReference(),
            PropertyName,
            string.Format("Mandatory relation property '{0}' of domain object '{1}' cannot be null.", PropertyName, ObjectID));
      }
    }

    public override sealed IDataManagementCommand CreateRemoveCommand (IDomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull("removedRelatedObject", removedRelatedObject);

      if (removedRelatedObject.ID != OppositeObjectID)
      {
        var removedID = removedRelatedObject.ID.ToString();
        var currentID = OppositeObjectID != null ? OppositeObjectID.ToString() : "<null>";

        var message = string.Format(
            "Cannot remove object '{0}' from object end point '{1}' - it currently holds object '{2}'.",
            removedID,
            PropertyName,
            currentID);
        throw new InvalidOperationException(message);
      }

      return CreateSetCommand(null);
    }

    public override sealed void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      var sourceObjectEndPoint = ArgumentUtility.CheckNotNullAndType<ObjectEndPoint>("source", source);

      if (Definition != sourceObjectEndPoint.Definition)
      {
        var message = string.Format(
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.",
            source.ID);
        throw new ArgumentException(message, "source");
      }

      SetOppositeObjectDataFromSubTransaction(sourceObjectEndPoint);

      if (sourceObjectEndPoint.HasBeenTouched || HasChanged)
        Touch();
    }

    public RelationEndPointID? GetOppositeRelationEndPointID ()
    {
      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition();
      if (oppositeEndPointDefinition.IsAnonymous)
        return null;

      var oppositeEndPointID = RelationEndPointID.Create(OppositeObjectID, oppositeEndPointDefinition);
      return oppositeEndPointID;
    }

    public override IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      var oppositeEndPointID = GetOppositeRelationEndPointID();

      if (oppositeEndPointID == null)
        return Enumerable.Empty<RelationEndPointID>();
      else
        return new[] { oppositeEndPointID };
    }

    #region Serialization

    protected ObjectEndPoint (FlattenedDeserializationInfo info)
        : base(info)
    {
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
    }

    #endregion
  }
}
