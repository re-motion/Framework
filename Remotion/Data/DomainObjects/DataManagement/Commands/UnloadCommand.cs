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
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Unloads a <see cref="IDomainObject"/> instance.
  /// </summary>
  public class UnloadCommand : IDataManagementCommand
  {
    private readonly IDomainObject[] _domainObjects;
    private readonly IDataManagementCommand _unloadDataCommand;
    private readonly IClientTransactionEventSink _transactionEventSink;

    public UnloadCommand (
        ICollection<IDomainObject> domainObjects,
        IDataManagementCommand unloadDataCommand,
        IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNullOrEmpty("domainObjects", domainObjects);
      ArgumentUtility.CheckNotNull("unloadDataCommand", unloadDataCommand);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);

      _domainObjects = domainObjects.ToArray();
      _unloadDataCommand = unloadDataCommand;
      _transactionEventSink = transactionEventSink;
    }

    public ReadOnlyCollection<IDomainObject> DomainObjects
    {
      get { return Array.AsReadOnly(_domainObjects); }
    }

    public IDataManagementCommand UnloadDataCommand
    {
      get { return _unloadDataCommand; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return _unloadDataCommand.GetAllExceptions();
    }

    public void Begin ()
    {
      this.EnsureCanExecute();

      _transactionEventSink.RaiseObjectsUnloadingEvent( Array.AsReadOnly(_domainObjects));
      _unloadDataCommand.Begin();
    }

    public void Perform ()
    {
      this.EnsureCanExecute();

      _unloadDataCommand.Perform();
    }

    public void End ()
    {
      this.EnsureCanExecute();

      _unloadDataCommand.End();
      _transactionEventSink.RaiseObjectsUnloadedEvent(Array.AsReadOnly(_domainObjects));
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand(this);
    }
  }
}
