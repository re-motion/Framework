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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the operation of setting the object stored by an <see cref="ObjectEndPoint"/> that is part of a unidirectional relation.
  /// </summary>
  public class ObjectEndPointSetUnidirectionalCommand : ObjectEndPointSetCommand
  {
    public ObjectEndPointSetUnidirectionalCommand (
        IObjectEndPoint modifiedEndPoint,
        IDomainObject? newRelatedObject,
        Action<IDomainObject?> oppositeObjectSetter,
        IClientTransactionEventSink transactionEventSink)
        : base(modifiedEndPoint, newRelatedObject, oppositeObjectSetter, transactionEventSink)
    {
      if (!modifiedEndPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous)
      {
        var message = string.Format("EndPoint '{0}' is from a bidirectional relation - use a ObjectEndPointSetOneOneCommand or ObjectEndPointSetOneManyCommand instead.",
            modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException(message, "modifiedEndPoint");
      }

      if (newRelatedObject == modifiedEndPoint.GetOppositeObject())
      {
        var message = string.Format("New related object for EndPoint '{0}' is the same as its old value - use a ObjectEndPointSetSameCommand instead.",
            modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException(message, "newRelatedObject");
      }
    }

    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand(this);
    }
  }
}
