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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Persistence;

namespace Remotion.Data.DomainObjects
{

  /// <summary>
  /// Represents a collection of <see cref="DomainObject"/>s always populated from the current state of the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When an <see cref="IObjectList{TDomainObject}"/> is used to model a bidirectional 1:n relation, the contents of the collection is lazily loaded.
  /// This means that the contents is usually not loaded together with the object owning the collection, but later, eg., when the collection's items
  /// are first accessed. In a root transaction, the collection's contents is loaded by executing a query on the underlying data source.
  /// </para>
  /// <para>
  /// The relation's in-memory state is always kept in-sync, i.e. it automatically updates it's contents when (lazy) loading additional objects from the storage provider,
  /// when performing an unload, or when changing the sort-condition's property value. This means that the use of the <see cref="BidirectionalRelationSyncService"/>
  /// is not required for collections of this type. In addition, the collection may always be unloaded regardless of the change-state of the <see cref="DomainObject"/>s
  /// it contains and vice-versa.
  /// </para>
  /// <para>
  /// Due to lazy loading, it is possible (and common) to load items that are part of a collection and the full collection contents within different
  /// database transactions. If the underlying data source changes between two lazy load operations involving the same relation, that relation will
  /// be updated automatically when it is accessed the next time. To ensure read consistency, use a database transaction or some 
  /// similar concept so that the different read operations on the data source work on the same database state.
  /// </para>
  /// <para>
  /// To be notified on <see cref="ClientTransaction.Commit"/> when the contents of an <see cref="IObjectList{TDomainObject}"/> has changed while 
  /// calculations were made in a <see cref="ClientTransaction"/>:
  /// <list type="bullet">
  ///   <item>the object owning the collection should be included in the commit set, and</item>
  ///   <item>the domain model should be programmed in such a way that all changes to the collection always cause that object to be included in the 
  ///   respective commit set as well.</item>
  /// </list>
  /// This can be achieved by calling the <see cref="DomainObjectExtensions.RegisterForCommit"/> method in the respective places. That way, a 
  /// <see cref="ConcurrencyViolationException"/> will be raised on <see cref="ClientTransaction.Commit"/> if the collection changes while the value 
  /// is being calculated.
  /// </para>
  /// <para>
  /// If a <see cref="DomainObjectCollection"/> is used to model a bidirectional 1:n relation, consider the following about ordering:
  /// <list type="bullet">
  ///   <item>
  ///     When loading the collection from the database (via loading an object in a root transaction), the order of items is defined
  ///     by the sort order of the relation (see <see cref="BidirectionalRelationAttribute.SortExpression"/>). If there is no sort
  ///     order, the order of items is based on the <see cref="DomainObject"/>.<see cref="DomainObject.ID"/>.s value to provide a stable ordering.
  ///   </item>
  ///   <item>
  ///     When committing a root transaction, the order of items in the collection is ignored; the next time the object is loaded
  ///     in another root transaction, the sort order is again defined by the sort order (or undefined).
  ///   </item>
  ///   <item>
  ///     When loading the collection from a parent transaction via loading an object in a subtransaction, the order of items in the subtransaction
  ///     is updated to match the state within the subtransaction.
  ///   </item>
  ///   <item>
  ///     When committing a subtransaction, the order of items in the collection is propagated to the parent transaction. After the commit,
  ///     the parent transaction's collection will have the items in the same order as the committed subtransaction.
  ///   </item>
  /// </list>
  /// </para>
  /// <para>
  /// The API of the <see cref="IObjectList{TDomainObject}"/> is defined as read-only. Modifications to the relation must therefor always be made
  /// from the one-side of the relation, i.e. by setting the foreign-key property.
  /// </para>
  /// </remarks>
  public interface IObjectList<out TDomainObject>
      : IReadOnlyList<TDomainObject>, IList // TODO: RM-7294 add support for IReadOnlyList<T> to BocList. Fallback: implement IList {IsReadOnly=true}
      where TDomainObject : IDomainObject
  {
    /// <summary>
    /// Gets the number of elements contained in the <see cref="IObjectList{TDomainObject}"/>.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the <see cref="DomainObject"/> with a given <paramref name="index"/> in the <see cref="IObjectList{TDomainObject}"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
    ///   <paramref name="index"/> is equal to or greater than the number of items in the collection.
    /// </exception>
    new TDomainObject this [int index] { get; }

    /// <summary>
    /// Gets the <see cref="IVirtualCollectionEndPoint"/> for the <see cref="IObjectList{TDomainObject}"/>.
    /// </summary>
    /// <value>The associated end point.</value>
    /// <remarks>Calling this API will not load the end point's data.</remarks>
    RelationEndPointID AssociatedEndPointID { get; }

    /// <summary>
    /// Gets a flag to check if the end point's data has been loaded.
    /// </summary>
    /// <returns><see langword="true" /> if the end point's data has been loaded; otherwise, <see langword="false" /></returns>
    /// <remarks>Calling this API will not load the end point's data.</remarks>
    bool IsDataComplete { get; }

    /// <summary>
    /// Ensures that the end point's data has been loaded, loading the data if necessary.
    /// </summary>
    void EnsureDataComplete ();

    /// <summary>
    /// Determines whether an item is in the <see cref="IObjectList{TDomainObject}"/>.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to locate in the <see cref="IObjectList{TDomainObject}"/>. Must not be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="DomainObject"/> with the <paramref name="objectID"/> is found in the <see cref="IObjectList{TDomainObject}"/>; otherwise, <see langword="false" />false;</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="objectID"/> is <see langword="null"/></exception>
    bool Contains (ObjectID objectID);

    /// <summary>
    /// Gets the <see cref="DomainObject"/> with a given <see cref="ObjectID"/> from the <see cref="IObjectList{TDomainObject}"/>.
    /// </summary>
    /// <returns>The <see cref="DomainObject"/> for the <paramref name="objectID"/> or <see langword="null" /> of the <see cref="DomainObject"/> was not found.</returns>
    TDomainObject GetObject (ObjectID objectID);

    [Obsolete ("IObjectList is readonly.", true)]
    new int Add (object value);

    [Obsolete ("IObjectList is readonly.", true)]
    new void Clear ();

    [Obsolete ("IObjectList is readonly.", true)]
    new void Insert (int index, object value);

    [Obsolete ("IObjectList is readonly.", true)]
    new void Remove (object value);

    [Obsolete ("IObjectList is readonly.", true)]
    new void RemoveAt (int index);
  }
}
