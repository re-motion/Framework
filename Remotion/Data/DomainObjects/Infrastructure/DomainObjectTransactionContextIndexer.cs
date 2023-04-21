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
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an indexing property to access a <see cref="DomainObject"/>'s transaction-dependent context for a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public struct DomainObjectTransactionContextIndexer
  {
    private readonly DomainObject _domainObject;
    private readonly IDomainObjectTransactionContextStrategy _strategy;

    public DomainObjectTransactionContextIndexer (DomainObject domainObject, IDomainObjectTransactionContextStrategy strategy)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      _domainObject = domainObject;
      _strategy = strategy;
    }

    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    public DomainObjectTransactionContextStruct this[ClientTransaction clientTransaction]
    {
      get
      {
        return new DomainObjectTransactionContextStruct(_domainObject, clientTransaction, _strategy);
      }
    }
  }

  public readonly struct DomainObjectTransactionContextStruct
  {
    private readonly IDomainObjectTransactionContextStrategy _strategy;

    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>

    public DomainObjectTransactionContextStruct (DomainObject domainObject, ClientTransaction clientTransaction, IDomainObjectTransactionContextStrategy strategy)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("associatedTransaction", clientTransaction);
      DomainObjectCheckUtility.CheckIfRightTransaction(domainObject, clientTransaction);

      _strategy = strategy;
      ClientTransaction = clientTransaction;
    }

    public ClientTransaction ClientTransaction { get; }

    public DomainObjectState State => _strategy.GetState(ClientTransaction);
    public object? Timestamp => _strategy.GetTimestamp(ClientTransaction);
    public void RegisterForCommit ()
    {
      _strategy.RegisterForCommit(ClientTransaction);
    }

    public void EnsureDataAvailable ()
    {
      _strategy.EnsureDataAvailable(ClientTransaction);
    }

    public bool TryEnsureDataAvailable ()
    {
      return _strategy.TryEnsureDataAvailable(ClientTransaction);
    }
  }

  public interface IDomainObjectTransactionContextStrategy
  {
    public object? GetTimestamp (ClientTransaction clientTransaction);
    public DomainObjectState GetState (ClientTransaction clientTransaction);

    public void RegisterForCommit (ClientTransaction clientTransaction);
    public void EnsureDataAvailable (ClientTransaction clientTransaction);
    public bool TryEnsureDataAvailable (ClientTransaction clientTransaction);
  }
}

// struct wird vom Indexer returned, muss die gleiche API haben wie das Interface, kann aber nicht implementieren
// enthält TransactionContext & relevante daten (wsl nur domainobject)
