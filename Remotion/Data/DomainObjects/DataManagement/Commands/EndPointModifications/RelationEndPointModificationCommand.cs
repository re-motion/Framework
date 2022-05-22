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

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents a modification performed on a <see cref="RelationEndPoint"/>. Provides default behavior for triggering the required
  /// events and notifying the <see cref="ClientTransaction"/> about the modification. The actual modification has to be specified by subclasses
  /// by implementing <see cref="Perform"/>. In addition, <see cref="ExpandToAllRelatedObjects"/> has to be overridden to return a 
  /// composite object containing all commands needed to be performed when this modification starts a bidirectional relation change.
  /// </summary>
  public abstract class RelationEndPointModificationCommand : IDataManagementCommand
  {
    private readonly IRelationEndPoint _modifiedEndPoint;
    private readonly IDomainObject _domainObject;

    private readonly IDomainObject? _oldRelatedObject;
    private readonly IDomainObject? _newRelatedObject;

    private readonly IClientTransactionEventSink _transactionEventSink;

    protected RelationEndPointModificationCommand (
        IRelationEndPoint modifiedEndPoint,
        IDomainObject? oldRelatedObject,
        IDomainObject? newRelatedObject,
        IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("modifiedEndPoint", modifiedEndPoint);
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);
      if (modifiedEndPoint.IsNull)
        throw new ArgumentException("Modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      var domainObject = modifiedEndPoint.GetDomainObject();
      if (domainObject == null)
        throw new ArgumentException("DomainObject of modified end point is null, a NullEndPointModificationCommand is needed.", "modifiedEndPoint");

      _modifiedEndPoint = modifiedEndPoint;
      _domainObject = domainObject;

      _oldRelatedObject = oldRelatedObject;
      _newRelatedObject = newRelatedObject;

      _transactionEventSink = transactionEventSink;
    }

    public IRelationEndPoint ModifiedEndPoint
    {
      get { return _modifiedEndPoint; }
    }

    public IDomainObject DomainObject
    {
      get { return _domainObject; }
    }

    public IDomainObject? OldRelatedObject
    {
      get { return _oldRelatedObject; }
    }

    public IDomainObject? NewRelatedObject
    {
      get { return _newRelatedObject; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    /// <summary>
    /// Performs this command without raising any events and without performing any bidirectional modifications.
    /// </summary>
    public abstract void Perform ();

    /// <summary>
    /// Returns a new <see cref="IDataManagementCommand"/> instance that involves changes to all objects affected by this
    /// <see cref="RelationEndPointModificationCommand"/>. If no other objects are involved by the change, this method returns just this
    /// <see cref="IDataManagementCommand"/>.
    /// </summary>
    /// <returns>A new <see cref="IDataManagementCommand"/> instance that involves changes to all objects affected by this
    /// <see cref="RelationEndPointModificationCommand"/>.</returns>
    public abstract ExpandedCommand ExpandToAllRelatedObjects ();

    public virtual IEnumerable<Exception> GetAllExceptions ()
    {
      return Enumerable.Empty<Exception>();
    }

    public virtual void Begin ()
    {
      RaiseClientTransactionBeginNotification(_oldRelatedObject, _newRelatedObject);
    }

    public virtual void End ()
    {
      RaiseClientTransactionEndNotification(_oldRelatedObject, _newRelatedObject);
    }

    protected void RaiseClientTransactionBeginNotification (IDomainObject? oldRelatedObject, IDomainObject? newRelatedObject)
    {
      _transactionEventSink.RaiseRelationChangingEvent(_domainObject, _modifiedEndPoint.Definition, oldRelatedObject, newRelatedObject);
    }

    protected void RaiseClientTransactionEndNotification (IDomainObject? oldRelatedObject, IDomainObject? newRelatedObject)
    {
      _transactionEventSink.RaiseRelationChangedEvent(_domainObject, _modifiedEndPoint.Definition, oldRelatedObject, newRelatedObject);
    }

    protected IRelationEndPoint GetOppositeEndPoint (
        IRelationEndPoint originatingEndPoint,
        IDomainObject? oppositeObject,
        IRelationEndPointProvider endPointProvider)
    {
      var oppositeEndPointID = RelationEndPointID.CreateOpposite(originatingEndPoint.Definition, oppositeObject.GetSafeID());
      return endPointProvider.GetRelationEndPointWithLazyLoad(oppositeEndPointID);
    }

    protected ClientTransactionScope EnterTransactionScope ()
    {
      return ModifiedEndPoint.ClientTransaction.EnterNonDiscardingScope();
    }
  }
}
