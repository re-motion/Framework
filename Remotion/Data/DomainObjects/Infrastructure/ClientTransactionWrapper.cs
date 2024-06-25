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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// <see cref="ClientTransactionWrapper"/> provides a wrapper for ClientTransactions that implements the <see cref="ITransaction"/> interface.
  /// </summary>
  public class ClientTransactionWrapper : ITransaction
  {
    private readonly ClientTransaction _wrappedInstance;

    protected internal ClientTransactionWrapper (ClientTransaction wrappedInstance)
    {
      ArgumentUtility.CheckNotNull("wrappedInstance", wrappedInstance);
      _wrappedInstance = wrappedInstance;
    }

    public ClientTransaction WrappedInstance
    {
      get { return _wrappedInstance; }
    }

    /// <summary> Gets the native transaction.</summary>
    /// <typeparam name="TTransaction">The type of the transaction abstracted by this instance.</typeparam>
    public TTransaction To<TTransaction> ()
    {
// ReSharper disable NotResolvedInText - We use the generic parameter on purpose.
      ArgumentUtility.CheckTypeIsAssignableFrom("TTransaction", typeof(TTransaction), typeof(ClientTransaction));
// ReSharper restore NotResolvedInText
      return (TTransaction)(object)_wrappedInstance;
    }

    /// <summary> Commits the transaction. </summary>
    public virtual void Commit ()
    {
      _wrappedInstance.Commit();
    }

    /// <summary> Rolls the transaction back. </summary>
    public virtual void Rollback ()
    {
      _wrappedInstance.Rollback();
    }

    /// <summary> 
    ///   Gets a flag that describes whether the transaction supports creating child transactions by invoking
    ///   <see cref="ITransaction.CreateChild"/>.
    /// </summary>
    public virtual bool CanCreateChild
    {
      get { return true; }
    }

    /// <summary> Creates a new child transaction for the current transaction. </summary>
    /// <returns> 
    ///   A new instance of the of a type implementing <see cref="ITransaction"/> that has the creating transaction
    ///   as a parent.
    /// </returns>
    /// <exception cref="NotSupportedException"> 
    ///   Thrown if the method is invoked while <see cref="ITransaction.CanCreateChild"/> is <see langword="false"/>.
    /// </exception>
    public virtual ITransaction CreateChild ()
    {
      return _wrappedInstance.CreateSubTransaction().ToITransaction();
    }

    /// <summary> Allows the transaction to implement clean up logic. </summary>
    /// <remarks> This method is called when the transaction is no longer needed. </remarks>
    public virtual void Release ()
    {
      _wrappedInstance.Discard();
    }

    /// <summary> Gets the transaction's parent transaction. </summary>
    /// <value> 
    ///   An instance of the of a type implementing <see cref="ITransaction"/> or <see langword="null"/> if the
    ///   transaction is a root transaction.
    /// </value>
    public virtual ITransaction? Parent
    {
      get { return _wrappedInstance.ParentTransaction?.ToITransaction(); }
    }

    /// <summary>Gets a flag describing whether the transaction is a child transaction.</summary>
    /// <value> <see langword="true"/> if the transaction is a child transaction. </value>
    public virtual bool IsChild
    {
      get { return _wrappedInstance.ParentTransaction != null; }
    }

    /// <summary>Gets a flag describing whether the transaction has been changed since the last commit or rollback.</summary>
    /// <value> <see langword="true"/> if the transaction has uncommitted changes. </value>
    public virtual bool HasUncommittedChanges
    {
      get { return _wrappedInstance.HasChanged(); }
    }

    /// <summary>Gets a flag describing whether the transaction is in a read-only state.</summary>
    /// <value> <see langword="true"/> if the transaction cannot be modified. </value>
    /// <remarks>Implementations that do not support read-only transactions should always return false.</remarks>
    public virtual bool IsReadOnly
    {
      get { return !_wrappedInstance.IsWriteable; }
    }

    /// <summary>
    /// Enters a new scope for the given transaction, making it the current transaction while the scope exists.
    /// </summary>
    /// <returns>The scope keeping the transaction active.</returns>
    /// <remarks>The scope must not discard the transaction when it is left.</remarks>
    public virtual ITransactionScope EnterScope ()
    {
      return _wrappedInstance.EnterNonDiscardingScope();
    }

    /// <summary>Registers the <paramref name="objects"/> with the transaction.</summary>
    /// <param name="objects">The objects to be registered. Must not be <see langword="null" />.</param>
    /// <remarks>If the type of of of the objects is not supported by the transaction, the object must be ignored.</remarks>
    public virtual void EnsureCompatibility (IEnumerable objects)
    {
      ArgumentUtility.CheckNotNull("objects", objects);

      var domainObjects = objects.OfType<DomainObject>().Distinct();
      var incompatibleObjects = domainObjects.Where(obj => _wrappedInstance.RootTransaction != obj.RootTransaction).ToArray();
      if (incompatibleObjects.Length > 0)
      {
        var message = string.Format(
            "The following objects are incompatible with the target transaction: {0}. Objects of type '{1}' could be used instead.",
            string.Join(", ", incompatibleObjects.Select(obj => obj.ID.ToString())),
            typeof(IDomainObjectHandle<>));
        throw new InvalidOperationException(message);
      }
    }
  }
}
