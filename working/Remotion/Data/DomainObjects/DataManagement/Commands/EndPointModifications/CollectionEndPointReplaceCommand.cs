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
  /// Represents the replacement of an element in a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public class CollectionEndPointReplaceCommand : RelationEndPointModificationCommand
  {
    private readonly int _index;
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly DomainObjectCollection _modifiedCollection;

    public CollectionEndPointReplaceCommand (
        ICollectionEndPoint modifiedEndPoint, 
        DomainObject replacedObject, 
        int index, 
        DomainObject replacementObject, 
        IDomainObjectCollectionData collectionData,
        IClientTransactionEventSink transactionEventSink)
      : base (
            ArgumentUtility.CheckNotNull ("modifiedEndPoint", modifiedEndPoint),
            ArgumentUtility.CheckNotNull ("replacedObject", replacedObject),
            ArgumentUtility.CheckNotNull ("replacementObject", replacementObject),
            ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink))
    {
      if (modifiedEndPoint.IsNull)
        throw new ArgumentException ("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      _index = index;
      _modifiedCollectionData = collectionData;
      _modifiedCollection = modifiedEndPoint.Collection;
    }

    public DomainObjectCollection ModifiedCollection
    {
      get { return _modifiedCollection; }
    }

    public IDomainObjectCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    public override void Begin ()
    {
      using (EnterTransactionScope())
      {
        ((IDomainObjectCollectionEventRaiser) ModifiedCollection).BeginRemove (_index, OldRelatedObject);
        ((IDomainObjectCollectionEventRaiser) ModifiedCollection).BeginAdd (_index, NewRelatedObject);
      }

      base.Begin ();
    }

    public override void Perform ()
    {
      ModifiedCollectionData.Replace (_index, NewRelatedObject);
      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      base.End ();

      using (EnterTransactionScope())
      {
        ((IDomainObjectCollectionEventRaiser) ModifiedCollection).EndAdd (_index, NewRelatedObject);
        ((IDomainObjectCollectionEventRaiser) ModifiedCollection).EndRemove (_index, OldRelatedObject);
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
      var endPointOfNewObject = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IRealObjectEndPoint> (NewRelatedObject);
      // the end point that was linked to the collection end point before the operation
      var endPointOfOldObject = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IRealObjectEndPoint> (OldRelatedObject);
      // the object that was linked to the new related object before the operation
      var oldRelatedObjectOfNewObject = endPointOfNewObject.GetOppositeObject ();
      // the end point that was linked to the new related object before the operation
      var oldRelatedEndPointOfNewObject = endPointOfNewObject.GetEndPointWithOppositeDefinition<ICollectionEndPoint> (oldRelatedObjectOfNewObject);

      return new ExpandedCommand (
          // customer.Order[index].Customer = null
          endPointOfOldObject.CreateRemoveCommand (ModifiedEndPoint.GetDomainObject()),
          // newOrder.Customer = customer
          endPointOfNewObject.CreateSetCommand (ModifiedEndPoint.GetDomainObject()),
          // customer.Orders[index] = newOrder
          this,
          // oldCustomer.Orders.Remove (insertedOrder)
          oldRelatedEndPointOfNewObject.CreateRemoveCommand (NewRelatedObject));
    }
  }
}
