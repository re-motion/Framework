using System;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  public readonly struct DomainObjectTransactionContextStruct
  {
    private readonly DomainObjectTransactionContext _strategy;

    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    public DomainObjectTransactionContextStruct (DomainObject domainObject, ClientTransaction clientTransaction, DomainObjectTransactionContext strategy)
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
}
