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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides extension methods for <see cref="DomainObjectCollection"/>.
  /// </summary>
  public static class DomainObjectCollectionExtensions
  {
    /// <summary>
    /// Checks that the given <see cref="DomainObjectCollection"/> is not read only, throwing a <see cref="NotSupportedException"/> if it is.
    /// </summary>
    /// <param name="collection">The collection to check.</param>
    /// <param name="message">The message the exception should have if one is thrown.</param>
    public static void CheckNotReadOnly (this DomainObjectCollection collection, string message)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      if (collection.IsReadOnly)
        throw new NotSupportedException (message);
    }

    /// <summary>
    /// Adds all items of the given <see cref="DomainObjectCollection"/> to the <see cref="DomainObjectCollection"/>, that are not already part of it.
    /// This method is a convenience method combining <see cref="DomainObjectCollection.Contains"/> and <see cref="DomainObjectCollection.AddRange"/>. If there are no changes made to this
    /// collection, the <see cref="DomainObjectCollection"/> method does not touch the associated end point (if any).
    /// </summary>
    /// <param name="collection">The collection to add items to.</param>
    /// <param name="sourceCollection">The collection to add items from. Must not be <see langword="null"/>.</param>
    /// <remarks>
    /// <para>
    /// To check if an item is already part of the <see cref="DomainObject.ID"/> its <see cref="DomainObject"/> is used.
    /// <see cref="DomainObject.ID"/> does not check if the item references are identical. In case the two <see cref="DomainObject"/> contain
    /// different items with the same <see cref="DomainObjectCollection"/>, <see cref="System"/> will thus ignore those items.
    /// </para>
    /// <para>
    /// This method calls <see cref="DomainObjectCollection.AddRange"/> and might throw any of the exceptions that can be thrown by 
    /// <see cref="DomainObjectCollection.AddRange"/>-
    /// </para>
    /// </remarks>
    public static void UnionWith (this DomainObjectCollection collection, DomainObjectCollection sourceCollection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);
      ArgumentUtility.CheckNotNull ("sourceCollection", sourceCollection);
      
      collection.CheckNotReadOnly ("A read-only collection cannot be combined with another collection.");

      collection.AddRange (sourceCollection.Cast<DomainObject> ().Where (obj => !collection.Contains (obj.ID)));
    }

    /// <summary>
    /// Returns all items of a given <see cref="DomainObjectCollection"/> that are not part of another <see cref="DomainObjectCollection"/>. The
    /// comparison is made by <see cref="DomainObject.ID"/>, not by reference.
    /// </summary>
    /// <param name="collection">The collection to return items from.</param>
    /// <param name="exceptedDomainObjects">A collection containing items that should not be returned.</param>
    /// <returns>
    /// An enumeration of all items from <paramref name="collection"/> that are not part of <paramref name="exceptedDomainObjects"/>.
    /// </returns>
    /// <remarks>
    /// 	<para>The method does not modify the given <see cref="DomainObjectCollection"/> istances.</para>
    /// </remarks>
    public static IEnumerable<DomainObject> GetItemsExcept (this DomainObjectCollection collection, HashSet<DomainObject> exceptedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);
      ArgumentUtility.CheckNotNull ("exceptedDomainObjects", exceptedDomainObjects);

      return collection.Cast<DomainObject>().Where (domainObject => !exceptedDomainObjects.Contains (domainObject));
    }

    /// <summary>
    /// Checks whether a <see cref="DomainObjectCollection"/> matches a sequence of <see cref="DomainObject"/> items by reference. 
    /// The comparison takes the order of elements into account.
    /// </summary>
    /// <param name="collection">The <see cref="DomainObjectCollection"/> to check.</param>
    /// <param name="comparedSequence">The sequence of elements to check against.</param>
    /// <returns><see langword="true"/> if the collection contains the same items as the comparedCollection in the same order; otherwise, <see langword="false"/>.</returns>
    public static bool SequenceEqual (this DomainObjectCollection collection, IEnumerable<DomainObject> comparedSequence)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);
      ArgumentUtility.CheckNotNull ("comparedSequence", comparedSequence);

      return collection.Cast<DomainObject> ().SequenceEqual (comparedSequence);
    }

    /// <summary>
    /// Checks whether a <see cref="DomainObjectCollection"/> matches another set of <see cref="DomainObject"/> items by reference. 
    /// The comparison does not take the order of elements into account.
    /// </summary>
    /// <param name="collection">The <see cref="DomainObjectCollection"/> to check.</param>
    /// <param name="comparedSet">The set of elements to check against.</param>
    /// <returns><see langword="true"/> if the collection contains the same items as the set in any order; otherwise, <see langword="false"/>.</returns>
    public static bool SetEquals (this DomainObjectCollection collection, IEnumerable<DomainObject> comparedSet)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);
      ArgumentUtility.CheckNotNull ("comparedSet", comparedSet);

      var setOfComparedObjects = new HashSet<DomainObject> (); // this is used to get rid of all duplicates to get a correct result
      foreach (var domainObject in comparedSet)
      {
        if (!collection.ContainsObject (domainObject))
          return false;

        setOfComparedObjects.Add (domainObject);
      }

      return collection.Count == setOfComparedObjects.Count; // the collection must contain exactly the number of items in the comparedSet - without dups
    }

    /// <summary>
    /// Adapts the given <see cref="DomainObjectCollection"/> as an <see cref="IList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The desired item type. This must be assignable from the <see cref="DomainObjectCollection"/>'s 
    /// <see cref="DomainObjectCollection.RequiredItemType"/>. If it is more general than the item type, the <see cref="DomainObjectCollection"/>'s
    /// runtime checks will ensure that only compatible items are inserted into the list.</typeparam>
    /// <param name="collection">The collection to be wrapped..</param>
    /// <returns>An implementation of <see cref="IList{T}"/> that wraps the given <paramref name="collection"/>.</returns>
    public static IList<T> AsList<T> (this DomainObjectCollection collection)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      return new DomainObjectCollectionWrapper<T> (collection);
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlyCollection{T}"/> representing the data of the <see cref="DomainObjectCollection"/>.
    /// The data is not copied; instead, the returned collection holds the same data store as the original collection and will therefore reflect
    /// any changes made to the original.
    /// </summary>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> representing the data of the <see cref="DomainObjectCollection"/>.</returns>
    public static ReadOnlyCollection<DomainObject> AsReadOnlyCollection (this DomainObjectCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      var listAdapter = collection.AsList<DomainObject> ();
      return new ReadOnlyCollection<DomainObject> (listAdapter);
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlyCollection{T}"/> representing the data of the <see cref="ObjectList{T}"/>.
    /// The data is not copied; instead, the returned collection holds the same data store as the original collection and will therefore reflect
    /// any changes made to the original.
    /// </summary>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> representing the data of the <see cref="ObjectList{T}"/>.</returns>
    public static ReadOnlyCollection<T> AsReadOnlyCollection<T> (this ObjectList<T> collection) where T: DomainObject
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      var listAdapter = collection.AsList<T> ();
      return new ReadOnlyCollection<T> (listAdapter);
    }
  }
}