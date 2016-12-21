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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation
{
  /// <summary>
  /// Provides APIs for checking whether the opposite relation properties in a bidirectional relation are out-of-sync, and - if yes -
  /// allows to synchronize them. Synchronization is performed only in the scope of a <see cref="ClientTransaction"/>, not with the underlying data 
  /// source. When applied to a sub-transaction or to a transaction with sub-transactions, the <see cref="BidirectionalRelationSyncService"/>
  /// affects the whole transaction hierarchy.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a bidirectional relation property is loaded from the underlying data source, re-store always tries to keep the two opposite
  /// sides in the relation in-sync. For example, in an Order-OrderItems relation, both the Order's relation property and 
  /// the individual OrderItems' relation properties should reflect the same relation data.
  /// </para>
  /// <para>
  /// There are, however, scenarios under which re-store cannot keep that promise of consistency. Due to the nature of lazy loading, the two
  /// sides of a bidirectional relation can become out-of-sync when the underlying data source changes between the loading of the two sides of the
  /// relation. This is most notable with 1:n relations, where the collection side is not fully loaded when only a single collection item
  /// is loaded into memory.
  /// </para>
  /// <para>
  /// Here is an example: Consider that the collection side of a bidirectional relation property is resolved from the underlying data source 
  /// (eg., by accessing the contents of the <see cref="DomainObjectCollection"/>). The <see cref="ClientTransaction"/> loads the contents of the 
  /// relation by means of a query to the underlying data source and stores the result for further use (again, via the 
  /// <see cref="DomainObjectCollection"/>). Now consider that the data source changes so that the foreign key property of another item gets set
  /// in a way that would normally qualify it as a collection item. After that, the item is loaded into the same <see cref="ClientTransaction"/> that
  /// already holds the <see cref="DomainObjectCollection"/>'s contents. That relation is now out-of-sync: the item's foreign key property points
  /// to the owner of the <see cref="DomainObjectCollection"/>, but the collection does not contain the item.
  /// </para>
  /// <para>
  /// Here is that same example in code:
  /// <code>
  /// var order = Order.GetObject (DomainObjectIDs.Order1);
  /// var orderItemsArray = order.OrderItems.ToArray(); // cause the full relation contents to be loaded and stored in-memory
  ///
  /// // data source now changes: an additional OrderItem with ID NewOrderItem is added which points back to DomainObjectIDs.Order1
  /// 
  /// // load that new item
  /// var newOrderItem = OrderItem.GetObject (DomainObjectIDs.NewOrderItem);
  /// 
  /// // prints "True" - the foreign key property points to DomainObjectIDs.Order1
  /// Console.WriteLine (newOrderItem.Order == order);
  ///
  /// // prints "False" - the collection has still the same state as before; it does not contain the item
  /// Console.WriteLine (order.OrderItems.ContainsObject (newOrderItem));
  /// </code>
  /// </para>
  /// <para>
  /// There are four scenarios where an out-of-sync state can happen:
  /// <list type="bullet">
  /// <item>
  /// A collection is loaded that does not contain an item. Later, that item's data is loaded, and it points back to the collection owner.
  /// The foreign key property is out-of-sync.
  /// </item>
  /// <item>
  /// A collection is loaded that does contain an item. Later, that item's data is loaded, and it does not points back to the collection owner.
  /// The collection is out-of-sync.
  /// </item>
  /// <item>
  /// An item is loaded that does point to the owner of a collection. Later, that collection is loaded, and it does not contain the item.
  /// The foreign key property is out-of-sync.
  /// </item>
  /// <item>
  /// An item is loaded that does not point to the owner of a collection. Later, that collection is loaded, and it contains the item.
  /// The collection is out-of-sync.
  /// </item>
  /// </list>
  /// (No matter whether the item or the collection is loaded first, the same foreign key/collection situation causes the same out-of-sync state.)
  /// </para>
  /// <para>
  /// The <see cref="BidirectionalRelationSyncService"/> class allows users to check whether a relation is out-of-sync (<see cref="IsSynchronized"/>)
  /// and, if so, get re-store to synchronize the opposite sides in the relation (<see cref="Synchronize(Remotion.Data.DomainObjects.ClientTransaction,RelationEndPointID)"/>):
  /// <code>
  /// var endPointID = RelationEndPointID.Create (newOrderItem, oi => oi.Order);
  /// 
  /// // Prints "False" - the relation is out-of-sync
  /// Console.WriteLine (BidirectionalRelationSyncService.IsSynchronized (ClientTransaction.Current, endPointID));
  /// 
  /// BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, endPointID);
  /// 
  /// // Prints "True" - the relation is now synchronized
  /// Console.WriteLine (BidirectionalRelationSyncService.IsSynchronized (ClientTransaction.Current, endPointID));
  /// 
  /// // prints "True" - the foreign key property points to DomainObjectIDs.Order1
  /// Console.WriteLine (newOrderItem.Order == order);
  ///
  /// // prints "True" - the collection now contains the item
  /// Console.WriteLine (order.OrderItems.ContainsObject (newOrderItem));
  /// </code>
  /// </para>
  /// <para>
  /// The <see cref="Synchronize"/> API always adjusts the collection so that the foreign keys in the <see cref="ClientTransaction"/> match. They
  /// never adjust a foreign key property, as this would violate the integrity of the respective <see cref="DomainObject"/> (and its timestamp).
  /// </para>
  /// </remarks>
  public static class BidirectionalRelationSyncService
  {
    /// <summary>
    /// Determines whether the given relation property is in-sync with the opposite relation property/properties.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to check the relation property in. In a transaction hierarchy,
    /// <see cref="IsSynchronized"/> returns the same result, no matter to which transaction (root or sub-transaction) in the hierarchy it is applied.</param>
    /// <param name="endPointID">The ID of the relation property to check. This contains the ID of the originating object and the
    /// relation property to check. The relation property must have been loaded into the given <paramref name="clientTransaction"/>.</param>
    /// <returns>
    /// 	<see langword="true"/> if the specified relation property is synchronized; <see langword="false"/> if it is out-of-sync.
    ///   If the relation has not been completely loaded, the result is <see langword="null" />.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///   <paramref name="endPointID"/> denotes a unidirectional (or anonymous) relation property.
    /// </exception>
    public static bool? IsSynchronized (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      CheckNotUnidirectional (endPointID, "endPointID");

      var endPoint = GetEndPoint (clientTransaction.RootTransaction, endPointID);
      if (endPoint == null)
        return null;

      return endPoint.IsSynchronized;
    }

    /// <summary>
    /// Synchronizes the given relation property with its opposite relation property/properties.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to synchronize the relation property in. In a transaction hierarchy,
    /// <see cref="Synchronize"/> affects the whole hierarchy, no matter to which transaction (root or sub-transaction) it is applied. </param>
    /// <param name="endPointID">The ID of the relation property to synchronize. This contains the ID of the originating object and the
    /// relation property to check.</param>
    /// <exception cref="ArgumentException">
    ///   <paramref name="endPointID"/> denotes a unidirectional (or anonymous) relation property.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The relation property denoted by <paramref name="endPointID"/> has not yet been fully loaded into the given <paramref name="clientTransaction"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    ///   If <paramref name="endPointID"/> denotes an object-valued end-point that is out-of-sync with an opposite collection (eg., OrderItem.Order), 
    ///   the opposite collection 
    ///   (eg., Order.OrderItems) is adjusted to match the foreign key value. This results in the item being added to the collection.
    ///   If <paramref name="endPointID"/> denotes a collection-valued end-point that is out-of-sync (eg., Order.OrderItems), the collection is 
    ///   synchronized with the opposite foreign key values. This results in all items being removed from the collection that do not have a foreign 
    ///   key value pointing at the collection.
    /// </para>
    /// <para>
    ///   If the relation is already synchronized, this method does nothing.
    /// </para>
    /// <para> 
    ///   When a relation involving a <see cref="DomainObjectCollection"/> is synchronized, its current and original contents may be changed.
    ///   For these changes, the <see cref="BidirectionalRelationAttribute.SortExpression"/> is not re-executed, the 
    ///   <see cref="DomainObjectCollection.Adding"/>/<see cref="DomainObjectCollection.Added"/> events are not raised (and the 
    ///   <see cref="DomainObjectCollection.OnAdding"/>/<see cref="DomainObjectCollection.OnAdded"/> methods not called), and no relation change 
    ///   events are raised. Because synchronization affects current and original relation value alike, the <see cref="DomainObject.State"/> of the
    ///   <see cref="DomainObjects"/> involved in the relation is not changed.
    /// </para>
    /// </remarks>
    public static void Synchronize (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      CheckNotUnidirectional (endPointID, "endPointID");

      var currentTransaction = clientTransaction.RootTransaction;
      var endPoint = GetAndCheckLoadedEndPoint (endPointID, currentTransaction);

      while (endPoint != null)
      {
        endPoint.Synchronize();

        currentTransaction = currentTransaction.SubTransaction;
        endPoint = currentTransaction != null ? GetEndPoint (currentTransaction, endPointID) : null;
      }
    }

    private static void CheckNotUnidirectional (RelationEndPointID endPointID, string paramName)
    {
      if (endPointID.Definition.RelationDefinition.RelationKind == RelationKindType.Unidirectional)
        throw new ArgumentException ("BidirectionalRelationSyncService cannot be used with unidirectional relation end-points.", paramName);
    }

    private static IRelationEndPoint GetAndCheckLoadedEndPoint (RelationEndPointID endPointID, ClientTransaction clientTransaction)
    {
      var endPoint = GetEndPoint (clientTransaction, endPointID);

      if (endPoint == null)
      {
        var message = String.Format (
            "The relation property '{0}' of object '{1}' has not yet been fully loaded into the given ClientTransaction.",
            endPointID.Definition.PropertyName,
            endPointID.ObjectID);
        throw new InvalidOperationException (message);
      }
      return endPoint;
    }

    private static IRelationEndPoint GetEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      var endPoint = clientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID);
      return endPoint == null || !endPoint.IsDataComplete ? null : endPoint;
    }
  }
}