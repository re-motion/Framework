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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Encapsulates all logic that is required to delete a <see cref="DomainObject"/>.
  /// </summary>
  public class DeleteCommand : IDataManagementCommand
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly DomainObject _deletedObject;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly DataContainer _dataContainer;
    private readonly IRelationEndPoint[] _endPoints;
    private readonly CompositeCommand _endPointDeleteCommands;

    public DeleteCommand (ClientTransaction clientTransaction, DomainObject deletedObject, IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("deletedObject", deletedObject);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);

      _clientTransaction = clientTransaction;
      _deletedObject = deletedObject;
      _transactionEventSink = transactionEventSink;

      _dataContainer = _clientTransaction.DataManager.GetDataContainerWithLazyLoad(_deletedObject.ID, throwOnNotFound: true)!;
      Assertion.IsFalse(_dataContainer.State.IsDeleted);
      Assertion.IsFalse(_dataContainer.State.IsDiscarded);

      _endPoints = (from endPointID in _dataContainer.AssociatedRelationEndPointIDs
                    let endPoint = _clientTransaction.DataManager.GetRelationEndPointWithLazyLoad(endPointID)
                    select endPoint).ToArray();
      _endPointDeleteCommands = new CompositeCommand(_endPoints.Select(endPoint => endPoint.CreateDeleteCommand()));
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public DomainObject DeletedObject
    {
      get { return _deletedObject; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return Enumerable.Empty<Exception>();
    }

    public void Begin ()
    {
      _transactionEventSink.RaiseObjectDeletingEvent(_deletedObject);
      _endPointDeleteCommands.Begin();
    }

    public void Perform ()
    {
      _endPointDeleteCommands.Perform();

      if (_dataContainer.State.IsNew)
        _clientTransaction.DataManager.Discard(_dataContainer);
      else
        _dataContainer.Delete();
    }

    public void End ()
    {
      _endPointDeleteCommands.End();
      _transactionEventSink.RaiseObjectDeletedEvent(_deletedObject);
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var commands =
          from endPoint in _endPoints
          from oppositeEndPointID in endPoint.GetOppositeRelationEndPointIDs()
          // Filter self-referencing endpoints. These are covered by the _endPointDeleteCommands.
          where oppositeEndPointID.ObjectID != _deletedObject.ID
          let oppositeEndPoint = _clientTransaction.DataManager.GetRelationEndPointWithLazyLoad(oppositeEndPointID)
          select oppositeEndPoint.CreateRemoveCommand(_deletedObject);

      return new ExpandedCommand(this).CombineWith(commands);
    }
  }
}
