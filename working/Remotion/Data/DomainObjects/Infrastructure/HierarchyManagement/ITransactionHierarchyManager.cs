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
using System.Collections.ObjectModel;

namespace Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement
{
  /// <summary>
  /// Defines an interface for classes managing the position and state of a <see cref="ClientTransaction"/> in a transaction hierarchy.
  /// </summary>
  public interface ITransactionHierarchyManager
  {
    IClientTransactionHierarchy TransactionHierarchy { get; }

    ClientTransaction ParentTransaction { get; }
    bool IsWriteable { get; }
    ClientTransaction SubTransaction { get; }
    
    void OnBeforeTransactionInitialize ();
    void OnTransactionDiscard ();
    void OnBeforeObjectRegistration (ReadOnlyCollection<ObjectID> loadedObjectIDs);
    // Calls to OnAfterObjectRegistration must be exactly matched with OnBeforeObjectRegistration; they must not be swallowed in case of exceptions.
    void OnAfterObjectRegistration (ReadOnlyCollection<ObjectID> objectIDsToBeLoaded);
    void OnBeforeSubTransactionObjectRegistration (ICollection<ObjectID> loadedObjectIDs);

    ClientTransaction CreateSubTransaction (Func<ClientTransaction, ClientTransaction> subTransactionFactory);
    void RemoveSubTransaction ();
    IDisposable Unlock ();
    IDisposable UnlockIfRequired ();
    void InstallListeners (IClientTransactionEventBroker eventBroker);
  }
}