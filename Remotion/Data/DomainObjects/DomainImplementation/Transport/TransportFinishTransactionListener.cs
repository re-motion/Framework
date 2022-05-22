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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation.Transport
{
  internal class TransportFinishTransactionListener : ClientTransactionListenerBase
  {
    private readonly Func<IDomainObject, bool> _filter;

    public TransportFinishTransactionListener (Func<IDomainObject, bool> filter)
    {
      ArgumentUtility.CheckNotNull("filter", filter);

      _filter = filter;
    }

    public override void TransactionCommitting (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      // Rollback the state of all objects not matched by the filter - we don't want those objects to be committed to the transaction

      Assertion.IsTrue(
          clientTransaction.ActiveTransaction == clientTransaction, "It's not possible to invoke FinishTransport on an inactive transaction.");
      using (clientTransaction.EnterNonDiscardingScope()) // filter must be executed in scope of clientTransaction
      {
        foreach (var domainObject in domainObjects)
        {
          if (!_filter(domainObject))
            RollbackObject(clientTransaction, domainObject);
        }
      }
    }

    private void RollbackObject (ClientTransaction clientTransaction, IDomainObject domainObject)
    {
      // Note that we do not roll back any end points - this will cause us to create dangling end points. Doesn't matter, though, the transaction
      // is discarded after transport anyway.

      var dataContainer = clientTransaction.DataManager.GetDataContainerWithLazyLoad(domainObject.ID, throwOnNotFound: true)!;
      if (dataContainer.State.IsNew)
      {
        var deleteCommand = clientTransaction.DataManager.CreateDeleteCommand(domainObject);
        deleteCommand.Perform(); // no events, no bidirectional changes
        Assertion.IsTrue(dataContainer.State.IsDiscarded);
      }
      else
      {
        dataContainer.RollbackState();
      }
    }
  }
}
