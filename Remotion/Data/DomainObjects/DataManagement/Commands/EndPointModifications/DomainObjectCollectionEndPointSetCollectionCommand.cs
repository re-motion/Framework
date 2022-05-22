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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents the replacement of the whole <see cref="DomainObjectCollectionEndPoint.Collection"/>, including the transformation
  /// of the involved <see cref="DomainObjectCollection"/> instances into stand-alone resp. associated collections.
  /// </summary>
  public class DomainObjectCollectionEndPointSetCollectionCommand : RelationEndPointModificationCommand
  {
    private readonly DomainObjectCollection _newCollection;
    private readonly IDomainObjectCollectionData _modifiedCollectionData;
    private readonly IDomainObjectCollectionEndPointCollectionManager _collectionEndPointCollectionManager;

    private readonly IDomainObject[] _removedObjects;
    private readonly IDomainObject[] _addedObjects;

    public DomainObjectCollectionEndPointSetCollectionCommand (
        IDomainObjectCollectionEndPoint modifiedEndPoint,
        DomainObjectCollection newCollection,
        IDomainObjectCollectionData modifiedCollectionData,
        IDomainObjectCollectionEndPointCollectionManager collectionEndPointCollectionManager,
        IClientTransactionEventSink transactionEventSink)
      : base(
          modifiedEndPoint,
          null,
          null,
          transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("newCollection", newCollection);
      ArgumentUtility.CheckNotNull("modifiedCollectionData", modifiedCollectionData);
      ArgumentUtility.CheckNotNull("collectionEndPointCollectionManager", collectionEndPointCollectionManager);

      _newCollection = newCollection;
      _modifiedCollectionData = modifiedCollectionData;
      _collectionEndPointCollectionManager = collectionEndPointCollectionManager;

      var oldOppositeObjects = ModifiedCollectionData;
      _removedObjects = oldOppositeObjects.Where(oldObject => !NewCollection.Contains(oldObject.ID)).ToArray();

      var newOppositeObjects = NewCollection.Cast<DomainObject>();
      _addedObjects = newOppositeObjects.Where(newObject => !ModifiedCollectionData.ContainsObjectID(newObject.ID)).ToArray();
    }

    public DomainObjectCollection NewCollection
    {
      get { return _newCollection; }
    }

    public IDomainObjectCollectionData ModifiedCollectionData
    {
      get { return _modifiedCollectionData; }
    }

    public IDomainObjectCollectionEndPointCollectionManager CollectionEndPointCollectionManager
    {
      get { return _collectionEndPointCollectionManager; }
    }

    public IDomainObject[] RemovedObjects
    {
      get { return _removedObjects; }
    }

    public IDomainObject[] AddedObjects
    {
      get { return _addedObjects; }
    }

    public override void Begin ()
    {
      for (int i = 0; i < RemovedObjects.Length; i++)
        RaiseClientTransactionBeginNotification(RemovedObjects[i], null);
      for (int i = 0; i < AddedObjects.Length; i++)
        RaiseClientTransactionBeginNotification(null, AddedObjects[i]);
    }

    public override void Perform ()
    {
      // After this operation, NewCollection will be associated with the end-point and ModifiedEndPoint.Collection will return NewCollection.
      // The previous ModifiedEndPoint.Collection will be a standalone copy of the end-point data.
      var oldDataStrategyOfNewCollection = CollectionEndPointCollectionManager.AssociateCollectionWithEndPoint(NewCollection);

      // Now, replace end-point's data with the data that was held by NewCollection before it was associated.
      ModifiedCollectionData.ReplaceContents(oldDataStrategyOfNewCollection);

      ModifiedEndPoint.Touch();
    }

    public override void End ()
    {
      for (int i = AddedObjects.Length - 1; i >= 0; i--)
        RaiseClientTransactionEndNotification(null, AddedObjects[i]);
      for (int i = RemovedObjects.Length - 1; i >= 0; i--)
        RaiseClientTransactionEndNotification(RemovedObjects[i], null);
    }

    /// <summary>
    /// Creates all commands needed to perform a bidirectional collection replace operation within this collection end point.
    /// </summary>
    /// <remarks>
    /// A replace operation of the form "customer.Orders = newOrders" involves the following steps:
    /// <list type="bullet">
    ///   <item>for each oldOrder the old collection (Orders) that's not in the new one: oldOrder.Customer = <see langword="null" />,</item>
    ///   <item>for each newOrder in the new collection (newOrders) that's not in the old one: newOrder.Customer.Orders.Remove (newOrder),</item>
    ///   <item>for each newOrder in the new collection (newOrders) that's not in the old one: newOrder.Customer = customer,</item>
    ///   <item>customer.Orders = newOrders.</item>
    /// </list>
    /// </remarks>
    public override ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var commandsForRemoved =
          from oldObject in RemovedObjects
          let endPoint = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IRealObjectEndPoint>(oldObject)
          select endPoint.CreateRemoveCommand(DomainObject); // oldOrder.Customer = null

      var commandsForAdded =
          from newObject in AddedObjects
          let endPointOfNewObject = ModifiedEndPoint.GetEndPointWithOppositeDefinition<IRealObjectEndPoint>(newObject) // newOrder.Customer
          let oldRelatedOfNewObject = endPointOfNewObject.GetOppositeObject() // newOrder.Customer
          let endPointOfOldRelatedOfNewObject =
              endPointOfNewObject.GetEndPointWithOppositeDefinition<IDomainObjectCollectionEndPoint>(oldRelatedOfNewObject) // newOrder.Customer.Orders
          let removeCommand = endPointOfOldRelatedOfNewObject.CreateRemoveCommand(newObject) // newOrder.Customer.Orders.Remove (newOrder)
          let setCommand = endPointOfNewObject.CreateSetCommand(DomainObject) // newOrder.Customer = customer
          from command in new[] { removeCommand, setCommand }
          select command;

      return new ExpandedCommand(commandsForRemoved).CombineWith(commandsForAdded).CombineWith(this);
    }
  }
}
