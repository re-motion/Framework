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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Contains extension methods for <see cref="IDomainObjectHandle{T}"/>.
  /// </summary>
  public static class DomainObjectHandleExtensions
  {
    /// <summary>
    /// Gets a <see cref="DomainObject"/> that already exists or attempts to load it from the data source. If the object's data can't be found, an 
    /// exception is thrown, and the object is marked <see cref="StateType.Invalid"/> in the <paramref name="clientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="DomainObject"/> to return. Can be a base type of the actual object type.</typeparam>
    /// <param name="handle">A handle to the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>. If <see langword="null" /> (or unspecified), the 
    /// <see cref="ClientTransaction.Current"/> transaction is used.</param>
    /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
    /// <returns>
    /// The <see cref="DomainObject"/> with the specified <paramref name="handle"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="clientTransaction"/> or <paramref name="handle"/> are <see langword="null"/>.</exception>
    /// <exception cref="ObjectsNotFoundException">
    /// The object could not be found in the data source. Note that the <see cref="ClientTransaction"/> marks
    /// not found objects as <see cref="StateType.Invalid"/>, so calling this API again witht he same <see cref="ObjectID"/> results in a 
    /// <see cref="ObjectInvalidException"/> being thrown.
    /// </exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the <paramref name="clientTransaction"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    /// The Mapping does not contain a class definition for the given <paramref name="handle"/>.<br/> -or- <br/>
    /// An error occurred while reading a <see cref="PropertyValue"/>.<br/> -or- <br/>
    /// An error occurred while accessing the data source.
    /// </exception>
    /// <exception cref="ObjectDeletedException">The object has already been deleted and the <paramref name="includeDeleted"/> flag is 
    /// <see langword="false" />.</exception>
    public static T GetObject<T> ([NotNull] this IDomainObjectHandle<T> handle, ClientTransaction clientTransaction = null, bool includeDeleted = false)
        where T : DomainObject, ISupportsGetObject
    {
      ArgumentUtility.CheckNotNull ("handle", handle);
      return (T) LifetimeService.GetObject (GetMandatoryClientTransaction (clientTransaction), handle.ObjectID, includeDeleted);
    }

    /// <summary>
    /// Gets a <see cref="DomainObject"/> that already exists or attempts to load it from the data source. 
    /// If an object cannot be found, it will be marked <see cref="StateType.Invalid"/> in the <paramref name="clientTransaction"/>, and the method will
    /// return a <see langword="null" /> reference in its place.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="DomainObject"/> to return. Can be a base type of the actual object type.</typeparam>
    /// <param name="handle">A handle to the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>. If <see langword="null" /> (or unspecified), the 
    /// <see cref="ClientTransaction.Current"/> transaction is used.</param>
    /// <returns>
    /// The <see cref="DomainObject"/> with the specified <paramref name="handle"/>, or <see langword="null" /> if it couldn't be found.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="clientTransaction"/> or <paramref name="handle"/> are <see langword="null"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    /// The Mapping does not contain a class definition for the given <paramref name="handle"/>.<br/> -or- <br/>
    /// An error occurred while reading a <see cref="PropertyValue"/>.<br/> -or- <br/>
    /// An error occurred while accessing the data source.
    /// </exception>
    public static T TryGetObject<T> ([NotNull] this IDomainObjectHandle<T> handle, ClientTransaction clientTransaction = null)
        where T : DomainObject, ISupportsGetObject
    {
      ArgumentUtility.CheckNotNull ("handle", handle);
      return (T) LifetimeService.TryGetObject (GetMandatoryClientTransaction (clientTransaction), handle.ObjectID);
    }

    /// <summary>
    /// Gets a reference to a <see cref="DomainObject"/> with the given <see cref="ObjectID"/> in a specific <see cref="ClientTransaction"/>. If the
    /// transaction does not currently hold an object with this <see cref="ObjectID"/>, an object reference representing that <see cref="ObjectID"/>
    /// is created without calling a constructor and without loading the object's data from the data source. This method does not check whether an
    /// object with the given <see cref="ObjectID"/> actually exists in the data source.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="DomainObject"/> to return. Can be a base type of the actual object type.</typeparam>
    /// <param name="handle">A handle to the <see cref="DomainObject"/> that should be returned. Must not be <see langword="null"/>.</param>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>. If <see langword="null" /> (or unspecified), the 
    /// <see cref="ClientTransaction.Current"/> transaction is used.</param>
    /// <returns>
    /// An object with the given <see cref="ObjectID"/>, possibly in <see cref="StateType.NotLoadedYet"/> state.
    /// </returns>
    /// <remarks>
    /// When an object with the given <paramref name="handle"/> has already been enlisted in the transaction, that object is returned. Otherwise,
    /// an object in <see cref="StateType.NotLoadedYet"/> state is created and enlisted without loading its data from the data source. In such a case,
    /// the object's data is loaded when it's first needed; e.g., when one of its properties is accessed or when
    /// <see cref="DomainObjectExtensions.EnsureDataAvailable"/> is called on it. At that point, an
    /// <see cref="ObjectsNotFoundException"/> may be triggered when the object's data cannot be found.
    /// </remarks>
    /// <exception cref="ArgumentNullException">One of the parameters passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object with the given <paramref name="handle"/> is invalid in the given 
    /// <paramref name="clientTransaction"/>.</exception>
    public static T GetObjectReference<T> ([NotNull] this IDomainObjectHandle<T> handle, ClientTransaction clientTransaction = null)
        where T : DomainObject, ISupportsGetObject
    {
      ArgumentUtility.CheckNotNull ("handle", handle);
      return (T) LifetimeService.GetObjectReference (GetMandatoryClientTransaction (clientTransaction), handle.ObjectID);
    }

    /// <summary>
    /// Gets a number of objects that are already loaded or attempts to load them from the data source.
    /// If an object's data can't be found, an exception is thrown, and the object is marked <see cref="StateType.Invalid"/> in the 
    /// <see cref="ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="DomainObject"/> instances to return. Can be a base type of the actual object type.</typeparam>
    /// <param name="handles">Handles to the <see cref="DomainObject"/> that should be loaded.</param>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>. If <see langword="null" /> (or unspecified), the 
    /// <see cref="ClientTransaction.Current"/> transaction is used.</param>
    /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
    /// <paramref name="handles"/>. This list might include deleted objects.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="handles"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the expected type <typeparamref name="T"/>.</exception>
    /// <exception cref="ObjectInvalidException">One of the retrieved objects is invalid in this transaction.</exception>
    /// <exception cref="ObjectsNotFoundException">
    /// One or more objects could not be found in the data source. Note that the <see cref="ClientTransaction"/> marks
    /// not found objects as <see cref="StateType.Invalid"/>, so calling this API again witht he same <see cref="ObjectID"/> results in a 
    /// <see cref="ObjectInvalidException"/> being thrown.
    /// </exception>
    public static T[] GetObjects<T> ([NotNull] this IEnumerable<IDomainObjectHandle<T>> handles, ClientTransaction clientTransaction = null)
        where T : DomainObject, ISupportsGetObject
    {
      ArgumentUtility.CheckNotNull ("handles", handles);
      return LifetimeService.GetObjects<T> (GetMandatoryClientTransaction (clientTransaction), handles.Select (h => h.ObjectID));
    }

    /// <summary>
    /// Gets a number of objects that are already loaded (including invalid objects) or attempts to load them from the data source. 
    /// If an object cannot be found, it will be marked <see cref="StateType.Invalid"/> in the <see cref="ClientTransaction"/>, and the result array will 
    /// contain a <see langword="null" /> reference in its place.
    /// </summary>
    /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
    /// <param name="handles">Handles to the <see cref="DomainObject"/> that should be loaded.</param>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>. If <see langword="null" /> (or unspecified), the 
    /// <see cref="ClientTransaction.Current"/> transaction is used.</param>
    /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
    /// <paramref name="handles"/>. This list can contain invalid and <see langword="null" /> <see cref="DomainObject"/> references.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="handles"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the specified type <typeparamref name="T"/>.</exception>
    public static T[] TryGetObjects<T> ([NotNull] this IEnumerable<IDomainObjectHandle<T>> handles, ClientTransaction clientTransaction = null)
        where T : DomainObject, ISupportsGetObject
    {
      ArgumentUtility.CheckNotNull ("handles", handles);
      return LifetimeService.TryGetObjects<T> (GetMandatoryClientTransaction (clientTransaction), handles.Select (h => h.ObjectID));
    }

    private static ClientTransaction GetMandatoryClientTransaction (ClientTransaction specifiedTransactionOrNull)
    {
      return specifiedTransactionOrNull ?? ClientTransactionScope.CurrentTransaction;
    }
  }
}