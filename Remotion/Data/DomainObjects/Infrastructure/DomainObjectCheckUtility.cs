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
using System.Diagnostics;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Contains commonly used get and check methods dealing with <see cref="IDomainObject"/> instances.
  /// </summary>
  public static class DomainObjectCheckUtility
  {
    /// <summary>
    /// Checks if an object is invalid in the given <paramref name="clientTransaction"/>, and, if yes, throws an <see cref="ObjectInvalidException"/>.
    /// </summary>
    /// <param name="domainObject">The domain object to check.</param>
    /// <param name="clientTransaction">The transaction to check the object against.</param>
    /// <exception cref="ObjectInvalidException">The object is invalid in the given <see cref="ClientTransaction"/>.</exception>
    [AssertionMethod]
    public static void EnsureNotInvalid ([NotNull] IDomainObject domainObject, [NotNull] ClientTransaction clientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("clientTransaction", clientTransaction);

      if (domainObject.TransactionContext[clientTransaction].State.IsInvalid)
        throw new ObjectInvalidException(domainObject.ID);
    }

    /// <summary>
    /// Checks if an object has been deleted in the given <paramref name="clientTransaction"/>, and, if yes, throws an 
    /// <see cref="ObjectDeletedException"/>.
    /// </summary>
    /// <param name="domainObject">The domain object to check.</param>
    /// <param name="clientTransaction">The transaction to check the object against.</param>
    /// <exception cref="ObjectDeletedException">The object has been deleted in the given <see cref="ClientTransaction"/>.</exception>
    [AssertionMethod]
    public static void EnsureNotDeleted ([NotNull] IDomainObject domainObject, [NotNull] ClientTransaction clientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("clientTransaction", clientTransaction);

      if (domainObject.TransactionContext[clientTransaction].State.IsDeleted)
        throw new ObjectDeletedException(domainObject.ID);
    }

    /// <summary>
    /// Checks if the given <see cref="IDomainObject"/> can be used in the given transaction, and, if not, throws a 
    /// <see cref="ClientTransactionsDifferException"/>. If the method succeeds, <see cref="ClientTransaction.IsEnlisted"/> is guaranteed to be
    /// <see langword="true" /> for the given <see cref="DomainObject"/>.
    /// </summary>
    /// <param name="domainObject">The domain object to check.</param>
    /// <param name="clientTransaction">The transaction to check the object against.</param>
    /// <returns>Returns <see langword="true"/> if the method succeeds without throwing an exception. This return value is available so that the 
    /// method can be used from within an expression.</returns>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    [AssertionMethod]
    public static void CheckIfRightTransaction ([NotNull] IDomainObject domainObject, [NotNull] ClientTransaction clientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("clientTransaction", clientTransaction);

      if (clientTransaction.RootTransaction != domainObject.RootTransaction)
      {
        string message = String.Format(
            "Domain object '{0}' cannot be used in the given transaction as it was loaded or created in another "
            + "transaction. Enter a scope for the transaction, or enlist the object "
            + "in the transaction. (If no transaction was explicitly given, ClientTransaction.Current was used.)",
            domainObject.ID);
        throw new ClientTransactionsDifferException(message);
      }
    }

    /// <summary>
    /// Checks if the given <see cref="IDomainObject"/> can be used in the given transaction, and, if not, throws a
    /// <see cref="ClientTransactionsDifferException"/>. If the method succeeds, <see cref="ClientTransaction.IsEnlisted"/> is guaranteed to be
    /// <see langword="true" /> for the given <see cref="DomainObject"/>. Calls to this method will be compiled iff the 'DEBUG' condition is set.
    /// </summary>
    /// <param name="domainObject">The domain object to check.</param>
    /// <param name="clientTransaction">The transaction to check the object against.</param>
    /// <returns>Returns <see langword="true"/> if the method succeeds without throwing an exception. This return value is available so that the
    /// method can be used from within an expression.</returns>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    [AssertionMethod]
    [Conditional("DEBUG")]
    public static void DebugCheckIfRightTransaction ([NotNull] IDomainObject domainObject, [NotNull] ClientTransaction clientTransaction)
    {
      CheckIfRightTransaction(domainObject, clientTransaction);
    }
  }
}
