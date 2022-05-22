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

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public class ClientTransactionEventReceiver
  {
    // types

    // static members and constants

    // member fields

    private readonly ClientTransaction _clientTransaction;
    private List<IReadOnlyList<IDomainObject>> _loadedDomainObjectLists;
    private List<IReadOnlyList<IDomainObject>> _committingDomainObjectLists;
    private List<IReadOnlyList<IDomainObject>> _committedDomainObjectLists;

    // construction and disposing

    public ClientTransactionEventReceiver (ClientTransaction clientTransaction)
    {
      _loadedDomainObjectLists = new List<IReadOnlyList<IDomainObject>>();
      _committingDomainObjectLists = new List<IReadOnlyList<IDomainObject>>();
      _committedDomainObjectLists = new List<IReadOnlyList<IDomainObject>>();
      _clientTransaction = clientTransaction;

      _clientTransaction.Loaded += ClientTransaction_Loaded;
      _clientTransaction.Committing += ClientTransaction_Committing;
      _clientTransaction.Committed += ClientTransaction_Committed;
    }

    // methods and properties

    public void Clear ()
    {
      _loadedDomainObjectLists = new List<IReadOnlyList<IDomainObject>>();
      _committingDomainObjectLists = new List<IReadOnlyList<IDomainObject>>();
      _committedDomainObjectLists = new List<IReadOnlyList<IDomainObject>>();
    }

    public void Unregister ()
    {
      _clientTransaction.Loaded -= ClientTransaction_Loaded;
      _clientTransaction.Committing -= ClientTransaction_Committing;
      _clientTransaction.Committed -= ClientTransaction_Committed;
    }

    private void ClientTransaction_Loaded (object sender, ClientTransactionEventArgs args)
    {
      _loadedDomainObjectLists.Add(args.DomainObjects);
    }

    private void ClientTransaction_Committing (object sender, ClientTransactionEventArgs args)
    {
      _committingDomainObjectLists.Add(args.DomainObjects);
    }

    private void ClientTransaction_Committed (object sender, ClientTransactionEventArgs args)
    {
      _committedDomainObjectLists.Add(args.DomainObjects);
    }

    public List<IReadOnlyList<IDomainObject>> LoadedDomainObjectLists
    {
      get { return _loadedDomainObjectLists; }
    }

    public List<IReadOnlyList<IDomainObject>> CommittingDomainObjectLists
    {
      get { return _committingDomainObjectLists; }
    }

    public List<IReadOnlyList<IDomainObject>> CommittedDomainObjectLists
    {
      get { return _committedDomainObjectLists; }
    }
  }
}
