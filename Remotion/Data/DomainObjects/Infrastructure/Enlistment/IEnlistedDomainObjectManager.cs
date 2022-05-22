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

namespace Remotion.Data.DomainObjects.Infrastructure.Enlistment
{
  /// <summary>
  /// Provides an interface for classes managing the set of <see cref="IDomainObject"/> references that can be used in a <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IEnlistedDomainObjectManager
  {
    /// <summary>
    /// Gets the number of domain objects enlisted by this <see cref="IEnlistedDomainObjectManager"/>.
    /// </summary>
    /// <value>The number of domain objects enlisted by this <see cref="IEnlistedDomainObjectManager"/>.</value>
    int EnlistedDomainObjectCount { get; }

    /// <summary>
    /// Gets all domain objects enlisted by this <see cref="IEnlistedDomainObjectManager"/>.
    /// </summary>
    /// <value>The domain objects enlisted via <see cref="EnlistDomainObject"/>.</value>
    IEnumerable<IDomainObject> GetEnlistedDomainObjects ();

    /// <summary>
    /// Returns the <see cref="IDomainObject"/> enlisted for the given <paramref name="objectID"/> via <see cref="EnlistDomainObject"/>, or 
    /// <see langword="null"/> if no such object exists.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> for which to retrieve a <see cref="IDomainObject"/>.</param>
    /// <returns>
    /// A <see cref="IDomainObject"/> with the given <paramref name="objectID"/> previously enlisted via <see cref="EnlistDomainObject"/>,
    /// or <see langword="null"/> if no such object exists.
    /// </returns>
    IDomainObject? GetEnlistedDomainObject (ObjectID objectID);

    /// <summary>
    /// Determines whether the specified <paramref name="domainObject"/> has been enlisted via <see cref="EnlistDomainObject"/>.
    /// </summary>
    /// <param name="domainObject">The domain object to be checked.</param>
    /// <returns>
    /// <see langword="true" /> if the specified domain object has been enlisted via <see cref="EnlistDomainObject"/>; otherwise, 
    /// <see langword="false" />.
    /// </returns>
    bool IsEnlisted (IDomainObject domainObject);

    /// <summary>
    /// Enlists the given domain object in the transaction managed by this <see cref="IEnlistedDomainObjectManager"/>.
    /// </summary>
    /// <param name="domainObject">The domain object to be enlisted.</param>
    /// <returns>
    /// <see langword="true" /> if the object was newly enlisted; <see langword="false" /> if it had already been enlisted before this 
    /// method was called.
    /// </returns>
    /// <remarks>
    /// From within this method, the object's <see cref="DataContainer"/> must not be accessed (directly or indirectly).
    /// </remarks>
    /// <exception cref="InvalidOperationException">Another object has already been registered for the <paramref name="domainObject"/>'s 
    /// <see cref="IDomainObject.ID"/>.</exception>
    void EnlistDomainObject (IDomainObject domainObject);

    /// <summary>
    /// Disenlists the given <see cref="IDomainObject"/>, throwing an exception if the object wasn't enlisted in the first place.
    /// </summary>
    /// <remarks>
    /// Note: Disenlist is presently only intended for use during DomainObject construction. Should this ever change, the default implementation of 
    /// <see cref="DictionaryBasedEnlistedDomainObjectManager"/> must be updated to implement garbage collection.
    /// </remarks>
    /// <param name="domainObject">The <see cref="IDomainObject"/> to be disenlisted.</param>
    void DisenlistDomainObject (IDomainObject domainObject);
  }
}
