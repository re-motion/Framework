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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Represents the minimal API for working with a <see cref="DomainObject"/>
  /// </summary>
  /// <seealso cref="DomainObject"/>
  public interface IDomainObject
  {
    /// <summary>
    /// Gets the <see cref="ObjectID"/> of the <see cref="IDomainObject"/>.
    /// </summary>
    [NotNull]
    ObjectID ID { get; }

    /// <summary>
    /// Returns the public type representation of this <see cref="IDomainObject"/> implementation, i.e. the type object visible to mappings, database, etc.
    /// </summary>
    /// <returns>The public type representation of this domain object.</returns>
    [NotNull]
    Type GetPublicDomainObjectType ();

    /// <summary>
    /// Gets the root <see cref="ClientTransaction"/> of the transaction hierarchy this <see cref="DomainObject"/> is associated with.
    /// </summary>
    /// <value>The <see cref="DomainObjects.ClientTransaction"/> this object is bound to.</value>
    /// <remarks>
    /// When a <see cref="DomainObject"/> is created, loaded, or its reference is initialized within a specific <see cref="ClientTransaction"/>, it
    /// automatically becomes associated with the hierarchy of that <see cref="ClientTransaction"/>. It can then always be used in 
    /// </remarks>
    [NotNull]
    ClientTransaction RootTransaction { get; }

    /// <summary>
    /// Gets a <see cref="DomainObjectTransactionContextIndexer"/> object that can be used to select an <see cref="DomainObjectTransactionContext"/>
    /// for a specific <see cref="DomainObjects.ClientTransaction"/>. 
    /// To obtain the default context, use <see cref="DomainObjectExtensions.GetDefaultTransactionContext"/>.
    /// </summary>
    /// <value>The transaction context.</value>
    [NotNull]
    DomainObjectTransactionContextIndexer TransactionContext { get; }
  }
}
