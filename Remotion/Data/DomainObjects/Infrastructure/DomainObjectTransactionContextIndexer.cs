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
    private readonly bool _isInitializedEventExecuting;

    private static IDomainObjectTransactionContextStrategy s_defaultStrategy = new DomainObjectTransactionContext();
    private static IDomainObjectTransactionContextStrategy s_initializedEventStrategy = new InitializedEventDomainObjectTransactionContextDecorator();

    public DomainObjectTransactionContextIndexer (DomainObject domainObject, bool isInitializedEventExecuting)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      _domainObject = domainObject;
      _isInitializedEventExecuting = isInitializedEventExecuting;
    }

    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    public DomainObjectTransactionContextStruct this[ClientTransaction clientTransaction]
    {
      get
      {
        var strategy = _isInitializedEventExecuting ? s_initializedEventStrategy : s_defaultStrategy;
        return new DomainObjectTransactionContextStruct(_domainObject, clientTransaction, strategy);
      }
    }
  }

  public struct DomainObjectTransactionContextStruct
  {
    private DomainObject _domainObject;
    private IDomainObjectTransactionContextStrategy _strategy;

    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>

    public DomainObjectTransactionContextStruct (DomainObject domainObject, ClientTransaction clientTransaction, IDomainObjectTransactionContextStrategy strategy)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("associatedTransaction", clientTransaction);
      DomainObjectCheckUtility.CheckIfRightTransaction(domainObject, clientTransaction);

      _strategy = strategy;
      ClientTransaction = clientTransaction;
      _domainObject = domainObject;
    }

    public ClientTransaction ClientTransaction { get; }

    public DomainObjectState State => _strategy.GetState(_domainObject, ClientTransaction);
    public object? Timestamp => _strategy.GetTimestamp(_domainObject, ClientTransaction);
    public void RegisterForCommit ()
    {
      _strategy.RegisterForCommit(_domainObject, ClientTransaction);
    }

    public void EnsureDataAvailable ()
    {
      _strategy.EnsureDataAvailable(_domainObject, ClientTransaction);
    }

    public bool TryEnsureDataAvailable ()
    {
      return _strategy.TryEnsureDataAvailable(_domainObject, ClientTransaction);
    }
  }

  public interface IDomainObjectTransactionContextStrategy
  {
    public object? GetTimestamp (DomainObject domainObject, ClientTransaction clientTransaction);
    public DomainObjectState GetState (DomainObject domainObject, ClientTransaction clientTransaction);

    public void RegisterForCommit (DomainObject domainObject, ClientTransaction clientTransaction);
    public void EnsureDataAvailable (DomainObject domainObject, ClientTransaction clientTransaction);
    public bool TryEnsureDataAvailable (DomainObject domainObject, ClientTransaction clientTransaction);
  }
}

// struct wird vom Indexer returned, muss die gleiche API haben wie das Interface, kann aber nicht implementieren
// enthält TransactionContext & relevante daten (wsl nur domainobject)
