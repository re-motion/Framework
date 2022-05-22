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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the operation of setting the object stored by an <see cref="ObjectEndPoint"/> that is part of a one-to-one relation.
  /// </summary>
  public class ObjectEndPointSetOneOneCommand : ObjectEndPointSetCommand
  {
    public ObjectEndPointSetOneOneCommand (
        IObjectEndPoint modifiedEndPoint,
        IDomainObject? newRelatedObject,
        Action<IDomainObject?> oppositeObjectSetter,
        IClientTransactionEventSink transactionEventSink)
        : base(modifiedEndPoint, newRelatedObject, oppositeObjectSetter, transactionEventSink)
    {
      if (modifiedEndPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous)
      {
        var message = string.Format("EndPoint '{0}' is from a unidirectional relation - use a ObjectEndPointSetUnidirectionalCommand instead.",
            modifiedEndPoint.Definition.PropertyName);
        throw new ArgumentException(message, "modifiedEndPoint");
      }

      if (modifiedEndPoint.Definition.GetOppositeEndPointDefinition().Cardinality == CardinalityType.Many)
      {
        var message = string.Format("EndPoint '{0}' is from a 1:n relation - use a ObjectEndPointSetOneManyCommand instead.",
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

    /// <summary>
    /// Creates all commands needed to perform a bidirectional 1:1 set operation on this <see cref="ObjectEndPoint"/>. One of the steps is 
    /// this command, the other steps are the opposite commands on the new/old related objects.
    /// </summary>
    /// <remarks>
    /// A 1:1 set operation of the form "order.OrderTicket = newTicket" needs four steps:
    /// <list type="bullet">
    ///   <item>order.OrderTicket = newTicket,</item>
    ///   <item>oldTicket.Order = null, </item>
    ///   <item>newTicket.Order = order, and</item>
    ///   <item>oldOrderOfNewTicket.OrderTicket = null.</item>
    /// </list>
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var newRelatedEndPoint = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IObjectEndPoint>(NewRelatedObject);
      var oldRelatedEndPoint = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IObjectEndPoint>(OldRelatedObject);

      var oldRelatedObjectOfNewRelatedObject = NewRelatedObject == null ? null : newRelatedEndPoint.GetOppositeObject();
      var oldRelatedEndPointOfNewRelatedEndPoint = newRelatedEndPoint.GetEndPointWithOppositeDefinition<IObjectEndPoint>(oldRelatedObjectOfNewRelatedObject);

      var removedRelatedObject = DomainObject;

      var bidirectionalModification = new ExpandedCommand(
        // => order.OrderTicket = newTicket
        this,
        // => oldTicket.Order = null (remove)
        oldRelatedEndPoint.CreateRemoveCommand(removedRelatedObject),
        // => newTicket.Order = order
        newRelatedEndPoint.CreateSetCommand(removedRelatedObject),
        // TODO RM-8241: IRelationEndPoint.CreateRemoveCommand (...) requires a non-null parameter.
        //               This is enforced by real implementations but ignored with NullObjectEndPoint and violates Liskov.
        // => oldOrderOfNewTicket.OrderTicket = null (remove)
        oldRelatedEndPointOfNewRelatedEndPoint.CreateRemoveCommand(NewRelatedObject!));

      return bidirectionalModification;
    }
  }
}
