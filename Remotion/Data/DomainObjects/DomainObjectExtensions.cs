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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides useful extension methods for working with <see cref="DomainObject"/> instances.
  /// </summary>
  public static class DomainObjectExtensions
  {
    /// <summary>
    /// Returns the <see cref="DomainObject.ID"/> of the given <paramref name="domainObjectOrNull"/>, or <see langword="null" /> if 
    /// <paramref name="domainObjectOrNull"/> is itself <see langword="null" />.
    /// </summary>
    /// <param name="domainObjectOrNull">The <see cref="IDomainObject"/> whose <see cref="IDomainObject.ID"/> to get. If this parameter is 
    /// <see langword="null" />, the method returns <see langword="null" />.</param>
    /// <returns>The <paramref name="domainObjectOrNull"/>'s <see cref="DomainObject.ID"/>, or <see langword="null" /> if <paramref name="domainObjectOrNull"/>
    /// is <see langword="null" />.</returns>
    [CanBeNull]
    [ContractAnnotation("null => null; notnull => notnull")]
    [return: NotNullIfNotNull("domainObjectOrNull")]
    public static ObjectID? GetSafeID ([CanBeNull] this IDomainObject? domainObjectOrNull)
    {
      if (domainObjectOrNull == null)
        return null;

      var objectID = domainObjectOrNull.ID;
      Assertion.DebugIsNotNull(objectID, "domainObjectOrNull.ID must not be null.");

      return domainObjectOrNull.ID;
    }

    /// <summary>
    /// Returns a typed handle to the given <paramref name="domainObject"/>. The generic type parameter <typeparamref name="T"/> is inferred from the 
    /// static type of the value passed as <paramref name="domainObject"/>.
    /// </summary>
    /// <typeparam name="T">The type to be used for the returned <see cref="IDomainObjectHandle{T}"/>.</typeparam>
    /// <param name="domainObject">The <see cref="IDomainObject"/> to get a handle for. Must not be <see langword="null" />.</param>
    /// <returns>A typed handle to the given <paramref name="domainObject"/>.</returns>
    [JetBrains.Annotations.NotNull]
    public static IDomainObjectHandle<T> GetHandle<T> ([JetBrains.Annotations.NotNull] this T domainObject) where T : IDomainObject
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      var objectID = domainObject.ID;
      Assertion.DebugIsNotNull(objectID, "domainObject.ID must not be null.");

      return objectID.GetHandle<T>();
    }

    /// <summary>
    /// Returns a typed handle to the given <paramref name="domainObjectOrNull"/>, or <see langword="null" /> if 
    /// <paramref name="domainObjectOrNull"/> is itself <see langword="null" />.
    /// The generic type parameter <typeparamref name="T"/> is inferred from the 
    /// static type of the value passed as <paramref name="domainObjectOrNull"/>.
    /// </summary>
    /// <typeparam name="T">The type to be used for the returned <see cref="IDomainObjectHandle{T}"/>.</typeparam>
    /// <param name="domainObjectOrNull">The <see cref="IDomainObject"/> to get a handle for. If this parameter is 
    /// <see langword="null" />, the method returns <see langword="null" />.</param>
    /// <returns>A typed handle to the given <paramref name="domainObjectOrNull"/>, or <see langword="null" /> if <paramref name="domainObjectOrNull"/>
    /// is <see langword="null" />.</returns>
    [CanBeNull]
    [ContractAnnotation("null => null; notnull => notnull")]
    [return: NotNullIfNotNull("domainObjectOrNull")]
    public static IDomainObjectHandle<T>? GetSafeHandle<T> ([CanBeNull] this T? domainObjectOrNull) where T : class, IDomainObject
    {
      return domainObjectOrNull != null ? domainObjectOrNull.GetHandle() : null;
    }

    /// <summary>
    /// Gets the current state of the <paramref name="domainObject"/> in the <see cref="ClientTransaction.ActiveTransaction"/>.
    /// </summary>
    /// <param name="domainObject">The <see cref="IDomainObject"/> to get the <see cref="DomainObjectState"/> for. Must not be <see langword="null" />.</param>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    public static DomainObjectState GetState ([JetBrains.Annotations.NotNull] this IDomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);

      var defaultTransactionContext = GetDefaultTransactionContext(domainObject);
      return defaultTransactionContext.State;
    }

    /// <summary>
    /// Gets the timestamp used for optimistic locking when the <paramref name="domainObject"/> is committed to the database in its 
    /// <see cref="IDomainObject.RootTransaction"/>.
    /// </summary>
    /// <value>The timestamp of the object.</value>
    /// <exception cref="ObjectInvalidException">The object is invalid in the transaction.</exception>
    [CanBeNull]
    public static object? GetTimestamp ([JetBrains.Annotations.NotNull] this IDomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);

      var defaultTransactionContext = GetDefaultTransactionContext(domainObject);
      return defaultTransactionContext.Timestamp;
    }

    /// <summary>
    /// Gets the default <see cref="DomainObjectTransactionContext"/>, i.e. the transaction context that is used when
    /// <paramref name="domainObject"/>'s properties are accessed without specifying a <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <param name="domainObject">
    /// The <see cref="IDomainObject"/> to get the default <see cref="DomainObjectTransactionContext"/> for. Must not be <see langword="null" />.
    /// </param>
    /// <returns>The default transaction context.</returns>
    /// <remarks>
    /// The default transaction for a <see cref="DomainObject"/> is the <see cref="ClientTransaction.ActiveTransaction"/> of the associated 
    /// <see cref="IDomainObject.RootTransaction"/>. The <see cref="ClientTransaction.ActiveTransaction"/> is usually the 
    /// <see cref="ClientTransaction.LeafTransaction"/>, but it can be changed by using <see cref="ClientTransaction"/> APIs.
    /// </remarks>
    [JetBrains.Annotations.NotNull]
    public static DomainObjectTransactionContext GetDefaultTransactionContext ([JetBrains.Annotations.NotNull] this IDomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);

      var rootTransaction = domainObject.RootTransaction;
      Assertion.DebugAssert(rootTransaction != null, "domainObject.RootTransaction must not be null.");

      return domainObject.TransactionContext[rootTransaction.ActiveTransaction];
    }

    /// <summary>
    /// Ensures that the <paramref name="domainObject"/> is included in the commit set of its <see cref="ClientTransaction.ActiveTransaction"/>. 
    /// The object may not have it's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag set,
    /// and if its <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/> flag is set, this method loads the object's data.
    /// </summary>
    /// <param name="domainObject">The <see cref="IDomainObject"/> to register for commit. Must not be <see langword="null" />.</param>
    /// <exception cref="ObjectDeletedException">The object has already been deleted.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the transaction.</exception>
    /// <remarks>
    /// <para>
    /// This operation affects the <see cref="DomainObject"/> as follows (in the default transaction):
    /// <list type="table">
    /// <item>
    /// <term><see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/> flag is set</term>
    /// <description>The object is loaded and then handled according to its new state, see below.</description>
    /// </item>
    /// <item>
    /// <term><see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsUnchanged"/> flag is set</term>
    /// <description>
    /// The object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/> flag will be set, even though no property value is
    /// actually changed. The object will then behave like any object with the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/>
    /// flag set. On commit (of a root transaction), it is checked for concurrency violations, and its timestamp is updated.
    /// </description>
    /// </item>
    /// <item>
    /// <term><see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/> flag is set</term>
    /// <description>
    /// The object's state is modified so that even when all changed properties are reset to their original values (so that it would
    /// usually have it's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/> flag cleared), the flag remains set.
    /// In that case, the object will behave as if the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsUnchanged"/> had been set
    /// prior to calling <see cref="RegisterForCommit"/> (see above).
    /// </description>
    /// </item>
    /// <item>
    /// <term><see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNew"/> flag is set</term>
    /// <description>The method has no effect.</description>
    /// </item>
    /// <item>
    /// <term><see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsDeleted"/> flag is set</term>
    /// <description>An <see cref="ObjectDeletedException"/> is thrown.</description>
    /// </item>
    /// <item>
    /// <term><see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag is set</term>
    /// <description>An <see cref="ObjectInvalidException"/> is thrown.</description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// When the <see cref="ClientTransaction"/> affected by this operation is rolled back (before being committed), any modifications made by this 
    /// API are also rolled back.
    /// </para>
    /// </remarks>
    public static void RegisterForCommit ([JetBrains.Annotations.NotNull] this IDomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);

      var defaultTransactionContext = domainObject.GetDefaultTransactionContext();
      defaultTransactionContext.RegisterForCommit();
    }

    /// <summary>
    /// Ensures that the <paramref name="domainObject"/>'s data has been loaded into the its <see cref="ClientTransaction.ActiveTransaction"/>.
    /// If it hasn't, this method causes the objec's data to be loaded. If the object's data can't be found, an exception is thrown.
    /// </summary>
    /// <param name="domainObject">The <see cref="IDomainObject"/> to ensure the data for. Must not be <see langword="null" />.</param>
    /// <exception cref="ObjectInvalidException">The object is invalid in the transaction.</exception>
    /// <exception cref="ObjectsNotFoundException">No data could be loaded for this <see cref="DomainObject"/> because the object was not
    /// found in the data source.</exception>
    public static void EnsureDataAvailable ([JetBrains.Annotations.NotNull] this IDomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);

      var defaultTransactionContext = domainObject.GetDefaultTransactionContext();
      defaultTransactionContext.EnsureDataAvailable();
    }

    /// <summary>
    /// Ensures that the <paramref name="domainObject"/>'s data has been loaded into its <see cref="ClientTransaction.ActiveTransaction"/>.
    /// If it hasn't, this method causes the object's data to be loaded. The method returns a value indicating whether the object's data was found.
    /// </summary>
    /// <param name="domainObject">The <see cref="IDomainObject"/> to ensure the data for. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the object's data is now available in the <see cref="ClientTransaction"/>, <see langword="false" /> if the 
    /// data couldn't be found.</returns>
    /// <exception cref="ObjectInvalidException">The object is invalid in the transaction.</exception>
    public static bool TryEnsureDataAvailable ([JetBrains.Annotations.NotNull] this IDomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);

      var defaultTransactionContext = domainObject.GetDefaultTransactionContext();
      return defaultTransactionContext.TryEnsureDataAvailable();
    }
  }
}
