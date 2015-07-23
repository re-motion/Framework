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

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Represents the context of a <see cref="DomainObject"/> that is associated with a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IDomainObjectTransactionContext
  {
    /// <summary>
    /// Gets the <see cref="ClientTransaction"/> this context is associated with.
    /// </summary>
    /// <value>The client transaction.</value>
    ClientTransaction ClientTransaction { get; }

    /// <summary>
    /// Gets the current state of the <see cref="DomainObject"/> in the associated <see cref="ClientTransaction"/>.
    /// </summary>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    StateType State { get; }

    /// <summary>
    /// Gets a value indicating the invalid status of the object in the associated <see cref="ClientTransaction"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when an object becomes invalid see <see cref="ObjectInvalidException"/>.
    /// </remarks>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the associated transaction.</exception>
    bool IsInvalid { get; }

    [Obsolete ("This state is now called Invalid. (1.13.60)", true)]
    bool IsDiscarded { get; }

    /// <summary>
    /// Gets the timestamp used for optimistic locking when the object is committed to the database.
    /// </summary>
    /// <value>The timestamp of the object.</value>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    object Timestamp { get; }

    /// <summary>
    /// Ensures that the <see cref="DomainObject"/> is included in the commit set of the <see cref="ClientTransaction"/>. 
    /// The object may not be in state <see cref="StateType.Deleted"/>, and if
    /// its state is <see cref="StateType.NotLoadedYet"/>, this method loads the object's data.
    /// </summary>
    /// <exception cref="ObjectDeletedException">The object has already been deleted.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the transaction.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the transaction.</exception>
    /// <remarks>
    /// <para>
    /// This operation affects the <see cref="DomainObject"/> as follows (in the default transaction):
    /// <list type="table">
    /// <item>
    /// <term><see cref="StateType.NotLoadedYet"/></term>
    /// <description>The object is loaded and then handled according to its new state, see below.</description>
    /// </item>
    /// <item>
    /// <term><see cref="StateType.Unchanged"/></term>
    /// <description>The object's state is modified to be <see cref="StateType.Changed"/>, even though no property value is actually changed. The 
    /// object will then behave like any <see cref="StateType.Changed"/> object. On commit (of a root transaction), it is checked for concurrency 
    /// violations, and its timestamp is updated.</description>
    /// </item>
    /// <item>
    /// <term><see cref="StateType.Changed"/></term>
    /// <description>The object's state is modified so that even when all changed properties are reset to their original values (so that it would
    /// usually become <see cref="StateType.Unchanged"/> again), it still remains <see cref="StateType.Changed"/>. In that case, the object will 
    /// behave like in the <see cref="StateType.Unchanged"/> case above.
    /// </description>
    /// </item>
    /// <item>
    /// <term><see cref="StateType.New"/></term>
    /// <description>The method has no effect.</description>
    /// </item>
    /// <item>
    /// <term><see cref="StateType.Deleted"/></term>
    /// <description>An <see cref="ObjectDeletedException"/> is thrown.</description>
    /// </item>
    /// <item>
    /// <term><see cref="StateType.Invalid"/></term>
    /// <description>An <see cref="ObjectInvalidException"/> is thrown.</description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// When the <see cref="ClientTransaction"/> affected by this operation is rolled back (before being committed), any modifications made by this 
    /// API are also rolled back.
    /// </para>
    /// </remarks>
    void RegisterForCommit ();

    /// <summary>
    /// Ensures that the <see cref="DomainObject"/>'s data has been loaded. If it hasn't, this method causes the object's data to be loaded.
    /// If the object's data can't be found, an exception is thrown.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the associated transaction.</exception>
    /// <exception cref="ObjectsNotFoundException">No data could be loaded for the <see cref="DomainObject"/> because the object was not
    /// found in the data source.</exception>
    void EnsureDataAvailable ();

    /// <summary>
    /// Ensures that the <see cref="DomainObject"/>'s data has been loaded. If it hasn't, this method causes the object's data to be loaded.
    /// The method returns a value indicating whether the object's data was found.
    /// </summary>
    /// <returns><see langword="true" /> if the object's data is now available in the <see cref="ClientTransaction"/>, <see langword="false" /> if the 
    /// data couldn't be found.</returns>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the associated transaction.</exception>
    bool TryEnsureDataAvailable ();

    [Obsolete ("This method has been removed. Use ClientTransaction.ExecuteInScope instead. (1.13.189.0)", true)]
    T Execute<T> (Func<DomainObject, ClientTransaction, T> func);

    [Obsolete ("This method has been removed. Use ClientTransaction.ExecuteInScope instead. (1.13.189.0)", true)]
    void Execute (Action<DomainObject, ClientTransaction> action);

    [Obsolete ("This method has been replaced by RegisterForCommit. (1.13.181.0)", true)]
    void MarkAsChanged ();
  }
}
