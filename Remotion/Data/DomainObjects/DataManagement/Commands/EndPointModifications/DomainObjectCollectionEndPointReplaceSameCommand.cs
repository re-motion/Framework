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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the replacement of an element in a <see cref="DomainObjectCollectionEndPoint"/> with itself. Calling <see cref="ExpandToAllRelatedObjects"/>
  /// results in an <see cref="IDataManagementCommand"/> that does not raise any events.
  /// </summary>
  public class DomainObjectCollectionEndPointReplaceSameCommand : RelationEndPointModificationCommand
  {
    public DomainObjectCollectionEndPointReplaceSameCommand (
        IDomainObjectCollectionEndPoint modifiedEndPoint,
        IDomainObject selfReplacedObject,
        IClientTransactionEventSink transactionEventSink)
        : base(
            modifiedEndPoint,
            ArgumentUtility.CheckNotNull("selfReplacedObject", selfReplacedObject),
            ArgumentUtility.CheckNotNull("selfReplacedObject", selfReplacedObject),
            transactionEventSink)
    {
    }

    public override void Perform ()
    {
      ModifiedEndPoint.Touch();
    }

    public override void Begin ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    public override void End ()
    {
      // do not issue any change notifications, a same-set is not a change
    }

    /// <summary>
    /// Creates all commands needed to perform a self-replace operation within this collection end point.
    /// </summary>
    /// <remarks>
    /// A self-replace operation of the form "customer.Orders[index] = customer.Orders[index]" needs two steps:
    /// <list type="bullet">
    ///   <item>customer.Orders.Touch() and</item>
    ///   <item>customer.Orders[index].Touch().</item>
    /// </list>
    /// No change notifications are sent for this operation.
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var endPointOfRelatedObject = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IRealObjectEndPoint>(OldRelatedObject);

      return new ExpandedCommand(
          this,
          new RelationEndPointTouchCommand(endPointOfRelatedObject));
    }
  }
}
