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
using System.Collections;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Holds a set of <see cref="IRelationEndPoint"/> instances and provides access to them.
  /// </summary>
  public class RelationEndPointMap : IRelationEndPointMapReadOnlyView
  {
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly Dictionary<RelationEndPointID, IRelationEndPoint> _relationEndPoints;

    public RelationEndPointMap (IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull("transactionEventSink", transactionEventSink);

      _transactionEventSink = transactionEventSink;
      _relationEndPoints = new Dictionary<RelationEndPointID, IRelationEndPoint>();
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public IRelationEndPoint? this[RelationEndPointID endPointID]
    {
      get { return _relationEndPoints.GetValueOrDefault(endPointID); }
    }

    public int Count
    {
      get { return _relationEndPoints.Count; }
    }

    public IEnumerator<IRelationEndPoint> GetEnumerator ()
    {
      return _relationEndPoints.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void CommitAllEndPoints ()
    {
      foreach (var endPoint in this)
        endPoint.Commit();
    }

    public void RollbackAllEndPoints ()
    {
      foreach (var endPoint in this)
        endPoint.Rollback();
    }

    public void AddEndPoint (IRelationEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      _transactionEventSink.RaiseRelationEndPointMapRegisteringEvent(endPoint);
      var id = endPoint.ID;
      try
      {
        _relationEndPoints.Add(id, endPoint);
      }
      catch (ArgumentException ex)
      {
        var message = string.Format("A relation end-point with ID '{0}' has already been registered.", id);
        throw new InvalidOperationException(message, ex);
      }
    }

    public void RemoveEndPoint (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      if (!_relationEndPoints.ContainsKey(endPointID))
      {
        var message = string.Format("End point '{0}' is not part of this map.", endPointID);
        throw new ArgumentException(message, "endPointID");
      }

      _transactionEventSink.RaiseRelationEndPointMapUnregisteringEvent(endPointID);
      _relationEndPoints.Remove(endPointID);
    }
  }
}
