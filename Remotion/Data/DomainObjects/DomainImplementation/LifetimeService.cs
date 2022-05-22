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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation
{
  /// <summary>
  /// Provides functionality to instantiate, get, and delete <see cref="DomainObject"/> instances.
  /// </summary>
  public static class LifetimeService
  {
    /// <summary>
    /// Returns a new instance of a <see cref="DomainObject"/> with the supplied constructor arguments in the given <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="domainObjectType">The <see cref="Type"/> of the <see cref="DomainObject"/> to be created.</param>
    /// <param name="constructorParameters">A <see cref="ParamList"/> encapsulating the parameters to be passed to the constructor. Instantiate this
    ///   by using one of the <see cref="ParamList.Create{A1,A2}"/> methods.</param>
    /// <returns>A new domain object instance.</returns>
    /// <remarks>
    /// 	<para>
    /// Objects created by this factory method are not directly instantiated; instead a proxy is dynamically created, which will assist in
    /// management tasks at runtime.
    /// </para>
    /// 	<para>
    /// This method should only be used by infrastructure code, ordinary code should use <see cref="DomainObject.NewObject{T}(ParamList)"/>.
    /// </para>
    /// 	<para>For more information, also see the constructor documentation (<see cref="DomainObject"/>).</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">One of the parameters is <see langword="null" />.</exception>
    /// <exception cref="MappingException">The <paramref name="domainObjectType"/> parameter does not specify a domain object type with mapping information.</exception>
    /// <exception cref="ArgumentException">The type <paramref name="domainObjectType"/> cannot be extended to a proxy, for example because it is sealed
    /// or abstract and non-instantiable.</exception>
    /// <exception cref="MissingMethodException">The <paramref name="domainObjectType"/> does not implement the required constructor (see Remarks
    /// section).
    /// </exception>
    public static IDomainObject NewObject (ClientTransaction clientTransaction, Type domainObjectType, ParamList constructorParameters)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("domainObjectType", domainObjectType);
      ArgumentUtility.CheckNotNull("constructorParameters", constructorParameters);

      return clientTransaction.NewObject(domainObjectType, constructorParameters);
    }

    /// <summary>
    /// Gets a <see cref="DomainObject"/> that already exists or attempts to load it from the data source. If the object's data can't be found, an 
    /// exception is thrown, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag is set, and the object becomes
    /// <b>invalid</b> in the <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="objectID">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
    /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
    /// <returns>
    /// The <see cref="DomainObject"/> with the specified <paramref name="objectID"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="clientTransaction"/> or <paramref name="objectID"/> are <see langword="null"/>.</exception>
    /// <exception cref="ObjectsNotFoundException">
    /// The object could not be found in the data source. Note that the <see cref="ClientTransaction"/> sets the not found object's
    /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag, so calling this API again with the same <see cref="ObjectID"/>
    /// results in a <see cref="ObjectInvalidException"/> being thrown.
    /// </exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the <paramref name="clientTransaction"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    /// The Mapping does not contain a class definition for the given <paramref name="objectID"/>.<br/> -or- <br/>
    /// An error occurred while reading a <see cref="PropertyValue"/>.<br/> -or- <br/>
    /// An error occurred while accessing the data source.
    /// </exception>
    /// <exception cref="ObjectDeletedException">The object has already been deleted and the <paramref name="includeDeleted"/> flag is 
    /// <see langword="false" />.</exception>
    public static IDomainObject GetObject (ClientTransaction clientTransaction, ObjectID objectID, bool includeDeleted)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectID", objectID);

      return clientTransaction.GetObject(objectID, includeDeleted);
    }

    /// <summary>
    /// Gets a <see cref="DomainObject"/> that already exists or attempts to load it from the data source. 
    /// If an object's data can't be found, an exception is thrown, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/>
    /// flag is set, and the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="objectID">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
    /// <returns>
    /// The <see cref="DomainObject"/> with the specified <paramref name="objectID"/>, or <see langword="null" /> if it couldn't be found.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="clientTransaction"/> or <paramref name="objectID"/> are <see langword="null"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    /// The Mapping does not contain a class definition for the given <paramref name="objectID"/>.<br/> -or- <br/>
    /// An error occurred while reading a <see cref="PropertyValue"/>.<br/> -or- <br/>
    /// An error occurred while accessing the data source.
    /// </exception>
    public static IDomainObject? TryGetObject (ClientTransaction clientTransaction, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectID", objectID);

      return clientTransaction.TryGetObject(objectID);
    }

    /// <summary>
    /// Gets a reference to a <see cref="DomainObject"/> with the given <see cref="ObjectID"/> in a specific <see cref="ClientTransaction"/>. If the
    /// transaction does not currently hold an object with this <see cref="ObjectID"/>, an object reference representing that <see cref="ObjectID"/>
    /// is created without calling a constructor and without loading the object's data from the data source. This method does not check whether an
    /// object with the given <see cref="ObjectID"/> actually exists in the data source.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to get the reference from.</param>
    /// <param name="objectID">The <see cref="ObjectID"/> to get an object reference for.</param>
    /// <returns>
    /// An object with the given <see cref="ObjectID"/>, possibly with the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/> flag set.
    /// </returns>
    /// <remarks>
    /// When an object with the given <paramref name="objectID"/> has already been enlisted in the transaction, that object is returned. Otherwise,
    /// an object with the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/> flag set is created and enlisted without
    /// loading its data from the data source. In such a case, the object's data is loaded when it's first needed; e.g., when one of its properties
    /// is accessed or when <see cref="DomainObjectExtensions.EnsureDataAvailable"/> is called on it. At that point, an
    /// <see cref="ObjectsNotFoundException"/> may be triggered when the object's data cannot be found.
    /// </remarks>
    /// <exception cref="ArgumentNullException">One of the parameters passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object with the given <paramref name="objectID"/> is invalid in the given 
    /// <paramref name="clientTransaction"/>.</exception>
    public static IDomainObject GetObjectReference (ClientTransaction clientTransaction, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectID", objectID);

      return clientTransaction.GetObjectReference(objectID);
    }

    /// <summary>
    /// Gets a number of objects that are already loaded or attempts to load them from the data source.
    /// If an object's data can't be found, an exception is thrown, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/>
    /// flag is set, and the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
    /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
    /// <paramref name="objectIDs"/>. This list might include deleted objects.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the expected type <typeparamref name="T"/>.</exception>
    /// <exception cref="ObjectInvalidException">One of the retrieved objects is invalid in this transaction.</exception>
    /// <exception cref="ObjectsNotFoundException">
    /// One or more objects could not be found in the data source. Note that the <see cref="ClientTransaction"/> sets the not found objects'
    /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag, so calling this API again with the same <see cref="ObjectID"/>s
    /// results in a <see cref="ObjectInvalidException"/> being thrown.
    /// </exception>
    public static T[] GetObjects<T> (ClientTransaction clientTransaction, params ObjectID[] objectIDs)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      return GetObjects<T>(clientTransaction, (IEnumerable<ObjectID>)objectIDs);
    }

    /// <summary>
    /// Gets a number of objects that are already loaded or attempts to load them from the data source.
    /// If an object's data can't be found, an exception is thrown, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/>
    /// flag is set, and the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
    /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
    /// <paramref name="objectIDs"/>. This list might include deleted objects.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the expected type <typeparamref name="T"/>.</exception>
    /// <exception cref="ObjectInvalidException">One of the retrieved objects is invalid in this transaction.</exception>
    /// <exception cref="ObjectsNotFoundException">
    /// One or more objects could not be found in the data source. Note that the <see cref="ClientTransaction"/> sets the not found objects'
    /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag, so calling this API again with the same <see cref="ObjectID"/>s
    /// results in a <see cref="ObjectInvalidException"/> being thrown.
    /// </exception>
    public static T[] GetObjects<T> (ClientTransaction clientTransaction, IEnumerable<ObjectID> objectIDs)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      return clientTransaction.GetObjects<T>(objectIDs);
    }

    /// <summary>
    /// Gets a number of objects that are already loaded (including invalid objects) or attempts to load them from the data source. 
    /// If an object cannot be found, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag is set,
    /// the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>, and the result array will contain a <see langword="null" />
    /// reference in its place.
    /// </summary>
    /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
    /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
    /// <paramref name="objectIDs"/>. This list can contain invalid and <see langword="null" /> <see cref="DomainObject"/> references.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the specified type <typeparamref name="T"/>.</exception>
    public static T?[] TryGetObjects<T> (ClientTransaction clientTransaction, params ObjectID[] objectIDs)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      return clientTransaction.TryGetObjects<T>(objectIDs);
    }

    /// <summary>
    /// Gets a number of objects that are already loaded (including invalid objects) or attempts to load them from the data source. 
    /// If an object cannot be found, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag is set,
    /// the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>, and the result array will contain a <see langword="null" />
    /// reference in its place.
    /// </summary>
    /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
    /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
    /// <paramref name="objectIDs"/>. This list can contain invalid and <see langword="null" /> <see cref="DomainObject"/> references.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the specified type <typeparamref name="T"/>.</exception>
    public static T?[] TryGetObjects<T> (ClientTransaction clientTransaction, IEnumerable<ObjectID> objectIDs)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      return clientTransaction.TryGetObjects<T>(objectIDs);
    }

    /// <summary>
    /// Deletes the given <see cref="IDomainObject"/>.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/>.</param>
    /// <param name="objectToBeDeleted">The object to be deleted.</param>
    /// <exception cref="ArgumentNullException">One of the parameters is <see langword="null" />.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the given <paramref name="clientTransaction"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the current transaction.</exception>
    /// <remarks>See also <see cref="DomainObject.Delete"/>.</remarks>
    public static void DeleteObject (ClientTransaction clientTransaction, IDomainObject objectToBeDeleted)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectToBeDeleted", objectToBeDeleted);

      clientTransaction.Delete(objectToBeDeleted);
    }
  }
}
