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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public abstract class ClientTransactionMockEventReceiver
  {
    private readonly ClientTransaction _clientTransaction;

    protected ClientTransactionMockEventReceiver (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      _clientTransaction = clientTransaction;

      _clientTransaction.Loaded += Loaded;
      _clientTransaction.Committing += Committing;
      _clientTransaction.Committed += Committed;
      _clientTransaction.RollingBack += RollingBack;
      _clientTransaction.RolledBack += RolledBack;
      _clientTransaction.SubTransactionCreated += SubTransactionCreated;
    }

    public abstract void Loaded (object sender, ClientTransactionEventArgs args);
    public abstract void Committing (object sender, ClientTransactionCommittingEventArgs args);
    public abstract void Committed (object sender, ClientTransactionEventArgs args);
    public abstract void RollingBack (object sender, ClientTransactionEventArgs args);
    public abstract void RolledBack (object sender, ClientTransactionEventArgs args);
    public abstract void SubTransactionCreated (object sender, SubTransactionCreatedEventArgs args);

    public void Loaded (params DomainObject[] domainObjects)
    {
      Loaded (Arg.Is (_clientTransaction), Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SetEquals (domainObjects)));
    }

    public void RollingBack (params DomainObject[] domainObjects)
    {
      RollingBack (Arg.Is (_clientTransaction), Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SetEquals (domainObjects)));
    }

    public void RolledBack (params DomainObject[] domainObjects)
    {
      RolledBack (Arg.Is (_clientTransaction), Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SetEquals (domainObjects)));
    }

    public void Committing (params DomainObject[] domainObjects)
    {
      Committing (Arg.Is (_clientTransaction), Arg<ClientTransactionCommittingEventArgs>.Matches (args => args.DomainObjects.SetEquals (domainObjects)));
    }

    public void Committed (params DomainObject[] domainObjects)
    {
      Committed (Arg.Is (_clientTransaction), Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SetEquals (domainObjects)));
    }
  }
}
