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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes
{
  [Serializable]
  public class SerializableTransactionHierarchyManagerFake : ITransactionHierarchyManager
  {
    public IClientTransactionHierarchy TransactionHierarchy
    {
      get { return new SerializableClientTransactionHierarchyFake(); }
    }

    public ClientTransaction ParentTransaction
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsWriteable
    {
      get { throw new NotImplementedException(); }
    }

    public ClientTransaction SubTransaction
    {
      get { throw new NotImplementedException(); }
    }

    public void OnBeforeTransactionInitialize ()
    {
      throw new NotImplementedException();
    }

    public void OnTransactionDiscard ()
    {
      throw new NotImplementedException();
    }

    public void OnBeforeSubTransactionObjectRegistration (ICollection<ObjectID> loadedObjectIDs)
    {
      throw new NotImplementedException();
    }

    public ClientTransaction CreateSubTransaction (Func<ClientTransaction, ClientTransaction> subTransactionFactory)
    {
      throw new NotImplementedException();
    }

    public void RemoveSubTransaction ()
    {
      throw new NotImplementedException();
    }

    public IDisposable Unlock ()
    {
      throw new NotImplementedException();
    }

    public IDisposable UnlockIfRequired ()
    {
      throw new NotImplementedException();
    }

    public void InstallListeners (IClientTransactionEventBroker eventBroker)
    {
      throw new NotImplementedException();
    }

    public void OnBeforeObjectRegistration (ReadOnlyCollection<ObjectID> loadedObjectIDs)
    {
      throw new NotImplementedException();
    }

    public void OnAfterObjectRegistration (ReadOnlyCollection<ObjectID> objectIDsToBeLoaded)
    {
      throw new NotImplementedException();
    }
  }
}