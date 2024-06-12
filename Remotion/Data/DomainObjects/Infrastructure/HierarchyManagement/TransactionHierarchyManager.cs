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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement
{
  /// <summary>
  /// Manages the position and state of a <see cref="ClientTransaction"/> in a transaction hierarchy.
  /// </summary>
  public class TransactionHierarchyManager : ITransactionHierarchyManager
  {
    /// <summary>
    /// Temporarily makes a read-only <see cref="ClientTransaction"/> writeable. This can destroy the integrity of a <see cref="ClientTransaction"/> 
    /// hierarchy. Use at your own risk.
    /// </summary>
    private class TransactionUnlocker : IDisposable
    {
      private readonly TransactionHierarchyManager _hierarchyManager;
      private bool _isDisposed;

      public TransactionUnlocker (TransactionHierarchyManager hierarchyManager)
      {
        ArgumentUtility.CheckNotNull("hierarchyManager", hierarchyManager);

        if (hierarchyManager._isWriteable)
        {
          string message = string.Format(
              "{0} cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed "
              + "while its parent transaction is engaged in an infrastructure operation. During such an operation, the subtransaction cannot be used.",
              hierarchyManager.ThisTransaction);
          throw new InvalidOperationException(message);
        }

        _hierarchyManager = hierarchyManager;
        _hierarchyManager._isWriteable = true;
        _isDisposed = false;
      }

      public void Dispose ()
      {
        if (!_isDisposed)
        {
          Assertion.IsTrue(_hierarchyManager.IsWriteable);
          _hierarchyManager._isWriteable = false;
          _isDisposed = true;
        }
      }
    }

    private readonly ClientTransaction _thisTransaction;
    private readonly IClientTransactionEventSink _thisEventSink;
    private readonly ClientTransaction? _parentTransaction;
    private readonly ITransactionHierarchyManager? _parentHierarchyManager;
    private readonly IClientTransactionEventSink? _parentEventSink;
    private readonly IClientTransactionHierarchy _transactionHierarchy;

    private readonly ReadOnlyClientTransactionListenerWithLoadRules _readOnlyClientTransactionListener;
    private readonly NewObjectHierarchyInvalidationClientTransactionListener _newObjectHierarchyInvalidationClientTransactionListener;

    private bool _isWriteable = true;
    private ClientTransaction? _subTransaction;

    public TransactionHierarchyManager (ClientTransaction thisTransaction, IClientTransactionEventSink thisEventSink)
        : this(
            thisTransaction,
            thisEventSink,
            new ClientTransactionHierarchy(ArgumentUtility.CheckNotNull("thisTransaction", thisTransaction)),
            null,
            null,
            null)
    {
    }

    public TransactionHierarchyManager (
        ClientTransaction thisTransaction,
        IClientTransactionEventSink thisEventSink,
        ClientTransaction parentTransaction,
        ITransactionHierarchyManager parentHierarchyManager,
        IClientTransactionEventSink parentEventSink)
        : this(
            thisTransaction,
            thisEventSink,
            parentHierarchyManager.TransactionHierarchy,
            parentTransaction,
            ArgumentUtility.CheckNotNull("parentHierarchyManager", parentHierarchyManager),
            parentEventSink)
    {
      ArgumentUtility.CheckNotNull("parentTransaction", parentTransaction);
      ArgumentUtility.CheckNotNull("parentEventSink", parentEventSink);

      _parentTransaction = parentTransaction;
      _parentHierarchyManager = parentHierarchyManager;
      _parentEventSink = parentEventSink;
    }

    private TransactionHierarchyManager (
        ClientTransaction thisTransaction,
        IClientTransactionEventSink thisEventSink,
        IClientTransactionHierarchy transactionHierarchy,
        ClientTransaction? parentTransaction,
        ITransactionHierarchyManager? parentHierarchyManager,
        IClientTransactionEventSink? parentEventSink)
    {
      ArgumentUtility.CheckNotNull("thisTransaction", thisTransaction);
      ArgumentUtility.CheckNotNull("thisEventSink", thisEventSink);
      ArgumentUtility.CheckNotNull("transactionHierarchy", transactionHierarchy);


      _thisTransaction = thisTransaction;
      _thisEventSink = thisEventSink;

      _parentTransaction = parentTransaction;
      _parentHierarchyManager = parentHierarchyManager;
      _parentEventSink = parentEventSink;

      _transactionHierarchy = transactionHierarchy;

      _readOnlyClientTransactionListener = new ReadOnlyClientTransactionListenerWithLoadRules();
      _newObjectHierarchyInvalidationClientTransactionListener = new NewObjectHierarchyInvalidationClientTransactionListener();
    }

    public ClientTransaction ThisTransaction
    {
      get { return _thisTransaction; }
    }

    public IClientTransactionEventSink ThisEventSink
    {
      get { return _thisEventSink; }
    }

    public IClientTransactionHierarchy TransactionHierarchy
    {
      get { return _transactionHierarchy; }
    }

    public ClientTransaction? ParentTransaction
    {
      get { return _parentTransaction; }
    }

    public IClientTransactionEventSink? ParentEventSink
    {
      get { return _parentEventSink; }
    }

    public ITransactionHierarchyManager? ParentHierarchyManager
    {
      get { return _parentHierarchyManager; }
    }

    public bool IsWriteable
    {
      get { return _isWriteable; }
    }

    public ClientTransaction? SubTransaction
    {
      get { return _subTransaction; }
    }

    public ReadOnlyClientTransactionListenerWithLoadRules ReadOnlyClientTransactionListener
    {
      get { return _readOnlyClientTransactionListener; }
    }

    public NewObjectHierarchyInvalidationClientTransactionListener NewObjectHierarchyInvalidationClientTransactionListener
    {
      get { return _newObjectHierarchyInvalidationClientTransactionListener; }
    }

    public void InstallListeners (IClientTransactionEventBroker eventBroker)
    {
      ArgumentUtility.CheckNotNull("eventBroker", eventBroker);
      eventBroker.AddListener(_readOnlyClientTransactionListener);
      eventBroker.AddListener(_newObjectHierarchyInvalidationClientTransactionListener);
    }

    public void OnBeforeTransactionInitialize ()
    {
      if (_parentTransaction != null)
      {
        Assertion.DebugIsNotNull(_parentEventSink, "_parentEventSink != null");
        _parentEventSink.RaiseSubTransactionInitializeEvent(_thisTransaction);
      }
    }

    public void OnTransactionDiscard ()
    {
      if (_subTransaction != null)
        _subTransaction.Discard();

      if (_parentHierarchyManager != null)
        _parentHierarchyManager.RemoveSubTransaction();
    }

    public void OnBeforeObjectRegistration (IReadOnlyList<ObjectID> loadedObjectIDs)
    {
      ArgumentUtility.CheckNotNull("loadedObjectIDs", loadedObjectIDs);
      if (_parentHierarchyManager != null)
        _parentHierarchyManager.OnBeforeSubTransactionObjectRegistration(loadedObjectIDs);
      _readOnlyClientTransactionListener.AddCurrentlyLoadingObjectIDs(loadedObjectIDs);
    }

    public void OnAfterObjectRegistration (IReadOnlyList<ObjectID> objectIDsToBeLoaded)
    {
      ArgumentUtility.CheckNotNull("objectIDsToBeLoaded", objectIDsToBeLoaded);
      _readOnlyClientTransactionListener.RemoveCurrentlyLoadingObjectIDs(objectIDsToBeLoaded);
    }

    public void OnBeforeSubTransactionObjectRegistration (IReadOnlyList<ObjectID> loadedObjectIDs)
    {
      ArgumentUtility.CheckNotNull("loadedObjectIDs", loadedObjectIDs);

      var conflictingIDs = loadedObjectIDs.Intersect(_readOnlyClientTransactionListener.CurrentlyLoadingObjectIDs).ConvertToCollection();
      if (conflictingIDs.Any())
      {
        var message =
            string.Format(
                "It's not possible to load objects into a subtransaction while they are being loaded into a parent transaction: {0}.",
                string.Join(", ", conflictingIDs.Select(id => "'" + id + "'")));
        throw new InvalidOperationException(message);
      }
    }

    public ClientTransaction CreateSubTransaction (Func<ClientTransaction, ClientTransaction> subTransactionFactory)
    {
      _thisEventSink.RaiseSubTransactionCreatingEvent();

      _isWriteable = false;

      ClientTransaction subTransaction;
      try
      {
        subTransaction = subTransactionFactory(_thisTransaction);
        if (subTransaction.ParentTransaction != _thisTransaction)
          throw new InvalidOperationException("The given factory did not create a sub-transaction for this transaction.");
      }
      catch
      {
        _isWriteable = true;
        throw;
      }

      _transactionHierarchy.AppendLeafTransaction(subTransaction);
      _subTransaction = subTransaction;

      _thisEventSink.RaiseSubTransactionCreatedEvent(subTransaction);
      return subTransaction;
    }

    public void RemoveSubTransaction ()
    {
      if (_subTransaction != null)
      {
        Assertion.IsTrue(_transactionHierarchy.LeafTransaction == _subTransaction);
        _transactionHierarchy.RemoveLeafTransaction();

        _subTransaction = null;
        _isWriteable = true;
      }
    }

    public IDisposable Unlock ()
    {
      return new TransactionUnlocker(this);
    }

    public IDisposable? UnlockIfRequired ()
    {
      return IsWriteable ? null : new TransactionUnlocker(this);
    }
  }
}
