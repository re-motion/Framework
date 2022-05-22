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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the replacement of an element in a <see cref="DomainObjectCollectionEndPoint"/>.
  /// </summary>
  public class DomainObjectCollectionEndPointReplaceCommand : RelationEndPointModificationCommand
  {
    private readonly int _index;
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly IDomainObjectCollectionEventRaiser _modifiedCollectionEventRaiser;

    public DomainObjectCollectionEndPointReplaceCommand (
        IDomainObjectCollectionEndPoint modifiedEndPoint,
        IDomainObject replacedObject,
        int index,
        IDomainObject replacementObject,
        IDomainObjectCollectionData collectionData,
        IClientTransactionEventSink transactionEventSink)
      : base(
            modifiedEndPoint,
            ArgumentUtility.CheckNotNull("replacedObject", replacedObject),
            ArgumentUtility.CheckNotNull("replacementObject", replacementObject),
            transactionEventSink)
    {
      _index = index;
      _modifiedCollectionData = collectionData;
      _modifiedCollectionEventRaiser = modifiedEndPoint.GetCollectionEventRaiser();
    }

    public IDomainObjectCollectionEventRaiser ModifiedCollectionEventRaiser
    {
      get { return _modifiedCollectionEventRaiser; }
    }

    public IDomainObjectCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    public new IDomainObject NewRelatedObject
    {
      get { return base.NewRelatedObject!; }
    }

    public new IDomainObject OldRelatedObject
    {
      get { return base.OldRelatedObject!; }
    }

    public override void Begin ()
    {
      using (EnterTransactionScope())
      {
        ModifiedCollectionEventRaiser.BeginRemove(_index, OldRelatedObject);
        ModifiedCollectionEventRaiser.BeginAdd(_index, NewRelatedObject);
      }

      base.Begin();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Replace(_index, NewRelatedObject);
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      base.End();

      using (EnterTransactionScope())
      {
        ModifiedCollectionEventRaiser.EndAdd(_index, NewRelatedObject);
        ModifiedCollectionEventRaiser.EndRemove(_index, OldRelatedObject);
      }
    }

    /// <summary>
    /// Creates all commands needed to perform a bidirectional replace operation within this collection end point.
    /// </summary>
    /// <remarks>
    /// A replace operation of the form "customer.Orders[index] = newOrder" needs four steps:
    /// <list type="bullet">
    ///   <item>customer.Order[index].Customer = null,</item>
    ///   <item>newOrder.Customer = customer,</item>
    ///   <item>customer.Orders[index] = newOrder,</item>
    ///   <item>oldCustomer.Orders.Remove (insertedOrder) - with oldCustomer being the old customer of the new order (if non-null).</item>
    /// </list>
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      // the end point that will be linked to the collection end point after the operation
      var endPointOfNewObject = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IRealObjectEndPoint>(NewRelatedObject);
      // the end point that was linked to the collection end point before the operation
      var endPointOfOldObject = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IRealObjectEndPoint>(OldRelatedObject);
      // the object that was linked to the new related object before the operation
      var oldRelatedObjectOfNewObject = endPointOfNewObject.GetOppositeObject();
      // the end point that was linked to the new related object before the operation
      var oldRelatedEndPointOfNewObject = endPointOfNewObject.GetEndPointWithOppositeDefinition<IDomainObjectCollectionEndPoint>(oldRelatedObjectOfNewObject);

      var removedRelatedObject = DomainObject;

      return new ExpandedCommand(
          // customer.Order[index].Customer = null
          endPointOfOldObject.CreateRemoveCommand(removedRelatedObject),
          // newOrder.Customer = customer
          endPointOfNewObject.CreateSetCommand(removedRelatedObject),
          // customer.Orders[index] = newOrder
          this,
          // oldCustomer.Orders.Remove (insertedOrder)
          oldRelatedEndPointOfNewObject.CreateRemoveCommand(NewRelatedObject));
    }
  }
}
