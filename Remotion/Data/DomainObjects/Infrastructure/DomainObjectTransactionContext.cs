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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Represents the transaction execution context of a <see cref="DomainObject"/> that is associated with a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public readonly struct DomainObjectTransactionContext
  {
    private readonly DomainObjectTransactionContextImplementation _transactionContextImplementation;

    /// <summary>
    /// Gets the <see cref="ClientTransaction"/> this context is associated with.
    /// </summary>
    /// <value>The client transaction.</value>
    public ClientTransaction ClientTransaction { get; }

    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    public DomainObjectTransactionContext (DomainObjectTransactionContextImplementation transactionContextImplementation, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull("transactionContextImplementation", transactionContextImplementation);
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      DomainObjectCheckUtility.CheckIfRightTransaction(transactionContextImplementation.DomainObject, clientTransaction);

      _transactionContextImplementation = transactionContextImplementation;
      ClientTransaction = clientTransaction;
    }

    /// <summary>
    /// Gets the <see cref="DomainObject"/> this context is associated with.
    /// </summary>
    /// <value>The domain object.</value>
    public DomainObject DomainObject => _transactionContextImplementation.DomainObject;

    /// <summary>
    /// Gets the current state of the <see cref="DomainObject"/> in the associated <see cref="ClientTransaction"/>.
    /// </summary>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    /// <exception cref="InvalidOperationException">The object cannot be used while the OnReferenceInitializing event is executing.</exception>
    public DomainObjectState State => _transactionContextImplementation.GetState(ClientTransaction);

    /// <summary>
    /// Gets the timestamp used for optimistic locking when the object is committed to the database.
    /// </summary>
    /// <value>The timestamp of the object.</value>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    /// <exception cref="InvalidOperationException">The object cannot be used while the OnReferenceInitializing event is executing.</exception>
    public object? Timestamp => _transactionContextImplementation.GetTimestamp(ClientTransaction);

    /// <summary>
    /// Ensures that the <see cref="DomainObject"/> is included in the commit set of the <see cref="ClientTransaction"/>. 
    /// The object may not have the <see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsDeleted"/> flag is set, and if
    /// its <see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/> flag is set, this method loads the object's data.
    /// </summary>
    /// <exception cref="ObjectDeletedException">The object has already been deleted.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the transaction.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the transaction.</exception>
    /// <exception cref="InvalidOperationException">The object cannot be used while the OnReferenceInitializing event is executing.</exception>
    /// <remarks>
    /// <para>
    /// This operation affects the <see cref="DomainObject"/> as follows (in the default transaction):
    /// <list type="table">
    /// <item>
    /// <term><see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/> flag is set</term>
    /// <description>The object is loaded and then handled according to its new state, see below.</description>
    /// </item>
    /// <item>
    /// <term><see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsUnchanged"/> flag is set</term>
    /// <description>
    /// The object's <see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/> flag will be set, even though no property value is
    /// actually changed. The object will then behave like any object with the <see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/>
    /// flag set. On commit (of a root transaction), it is checked for concurrency violations, and its timestamp is updated.
    /// </description>
    /// </item>
    /// <item>
    /// <term><see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/> flag is set</term>
    /// <description>
    /// The object's state is modified so that even when all changed properties are reset to their original values (so that it would
    /// usually have its <see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/> flag cleared), the flag remains set.
    /// In that case, the object will behave as if the <see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsUnchanged"/> had been set
    /// prior to calling <see cref="RegisterForCommit"/> (see above).
    /// </description>
    /// </item>
    /// <item>
    /// <term><see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsNew"/> flag is set</term>
    /// <description>The method has no effect.</description>
    /// </item>
    /// <item>
    /// <term><see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsDeleted"/> flag is set</term>
    /// <description>An <see cref="ObjectDeletedException"/> is thrown.</description>
    /// </item>
    /// <item>
    /// <term><see cref="DomainObjects.DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag is set</term>
    /// <description>An <see cref="ObjectInvalidException"/> is thrown.</description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// When the <see cref="ClientTransaction"/> affected by this operation is rolled back (before being committed), any modifications made by this 
    /// API are also rolled back.
    /// </para>
    /// </remarks>
    public void RegisterForCommit ()
    {
      _transactionContextImplementation.RegisterForCommit(ClientTransaction);
    }

    /// <summary>
    /// Ensures that the <see cref="DomainObject"/>'s data has been loaded. If it hasn't, this method causes the object's data to be loaded.
    /// If the object's data can't be found, an exception is thrown.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the associated transaction.</exception>
    /// <exception cref="ObjectsNotFoundException">No data could be loaded for the <see cref="DomainObject"/> because the object was not
    /// found in the data source.</exception>
    /// <exception cref="InvalidOperationException">The object cannot be used while the OnReferenceInitializing event is executing.</exception>
    public void EnsureDataAvailable ()
    {
      _transactionContextImplementation.EnsureDataAvailable(ClientTransaction);
    }

    /// <summary>
    /// Ensures that the <see cref="DomainObject"/>'s data has been loaded. If it hasn't, this method causes the object's data to be loaded.
    /// The method returns a value indicating whether the object's data was found.
    /// </summary>
    /// <returns><see langword="true" /> if the object's data is now available in the <see cref="ClientTransaction"/>, <see langword="false" /> if the 
    /// data couldn't be found.</returns>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the associated transaction.</exception>
    /// <exception cref="InvalidOperationException">The object cannot be used while the OnReferenceInitializing event is executing.</exception>
    public bool TryEnsureDataAvailable ()
    {
      return _transactionContextImplementation.TryEnsureDataAvailable(ClientTransaction);
    }
  }
}
