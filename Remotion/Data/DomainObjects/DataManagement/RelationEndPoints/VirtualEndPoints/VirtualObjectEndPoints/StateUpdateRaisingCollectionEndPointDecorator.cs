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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  /// <summary>
  /// Decorates another <see cref="IVirtualObjectEndPoint"/>, raising <see cref="IVirtualEndPointStateUpdateListener"/> events whenever the 
  /// return value of the <see cref="HasChanged"/> property has possibly changed.
  /// </summary>
  public class StateUpdateRaisingVirtualObjectEndPointDecorator : IVirtualObjectEndPoint
  {
    /// <summary>
    /// Using an instance of this class around a code block asserts that the change state before and after after the block is the same.
    /// </summary>
    /// <remarks>
    /// The event might also be raised when the property hasn't actually changed. Check the boolean value passed to 
    /// <see cref="IVirtualEndPointStateUpdateListener.VirtualEndPointStateUpdated"/> to find out the new value of the <see cref="HasChanged"/>
    /// property.
    /// </remarks>
    private struct ConstantChangeStateAsserter : IDisposable
    {
      private readonly bool? _changeStateBefore;
      private readonly IVirtualObjectEndPoint _innerEndPoint;

      public ConstantChangeStateAsserter (IVirtualObjectEndPoint innerEndPoint)
      {
        _changeStateBefore = innerEndPoint.HasChanged;
        _innerEndPoint = innerEndPoint;
      }

      public void Dispose ()
      {
        Assertion.IsTrue(_changeStateBefore == _innerEndPoint.HasChanged);
      }
    }

    private readonly IVirtualObjectEndPoint _innerEndPoint;
    private readonly IVirtualEndPointStateUpdateListener _listener;

    public StateUpdateRaisingVirtualObjectEndPointDecorator (IVirtualObjectEndPoint innerEndPoint, IVirtualEndPointStateUpdateListener listener)
    {
      ArgumentUtility.CheckNotNull("innerEndPoint", innerEndPoint);
      ArgumentUtility.CheckNotNull("listener", listener);

      _innerEndPoint = innerEndPoint;
      _listener = listener;
    }

    public IVirtualEndPointStateUpdateListener Listener
    {
      get { return _listener; }
    }

    public IVirtualObjectEndPoint InnerEndPoint
    {
      get { return _innerEndPoint; }
    }

    public void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      var sourceVirtualObjectEndPoint = ArgumentUtility.CheckNotNullAndType<StateUpdateRaisingVirtualObjectEndPointDecorator>("source", source);
      var hasChangedBefore = _innerEndPoint.HasChanged;
      try
      {
        _innerEndPoint.SetDataFromSubTransaction(sourceVirtualObjectEndPoint.InnerEndPoint);
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedBefore);
      }
    }

    public void Synchronize ()
    {
      var hasChangedBefore = _innerEndPoint.HasChanged;
      try
      {
        _innerEndPoint.Synchronize();
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedBefore);
      }
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      var hasChangedBefore = _innerEndPoint.HasChanged;
      try
      {
        _innerEndPoint.SynchronizeOppositeEndPoint(oppositeEndPoint);
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedBefore);
      }
    }

    public void Commit ()
    {
      var hasChangedBefore = _innerEndPoint.HasChanged;
      try
      {
        _innerEndPoint.Commit();
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedBefore);
      }
    }

    public void Rollback ()
    {
      var hasChangedBefore = _innerEndPoint.HasChanged;
      try
      {
        _innerEndPoint.Rollback();
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedBefore);
      }
    }

    // Note: The commands created by _innerEndPoint contain a leaked this reference pointing to _innerEndPoint that bypasses this decorator.
    // This is not a problem because we wrap the command into a decorator that raises the StateUpdated notification anyway, so it doesn't matter if 
    // the command internally bypasses this decorator.

    public IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateRemoveCommand(removedRelatedObject);
        return CreateStateUpdateRaisingCommandDecorator(command);
      }
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateDeleteCommand();
        return CreateStateUpdateRaisingCommandDecorator(command);
      }
    }

    public IDataManagementCommand CreateSetCommand (DomainObject? newRelatedObject)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateSetCommand(newRelatedObject);
        return CreateStateUpdateRaisingCommandDecorator(command);
      }
    }

    #region Methods not affecting HasChanged

    public bool IsNull
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.IsNull;
        }
      }
    }

    public RelationEndPointID ID
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.ID;
        }
      }
    }

    public ClientTransaction ClientTransaction
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.ClientTransaction;
        }
      }
    }

    public ObjectID? ObjectID
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.ObjectID;
        }
      }
    }

    public IRelationEndPointDefinition Definition
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.Definition;
        }
      }
    }

    public RelationDefinition RelationDefinition
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.RelationDefinition;
        }
      }
    }

    public bool HasChanged
    {
      // No assertion on this property because the assertion uses this property...
      get { return _innerEndPoint.HasChanged; }
    }

    public bool HasBeenTouched
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.HasBeenTouched;
        }
      }
    }

    public DomainObject? GetDomainObject ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetDomainObject();
      }
    }

    public DomainObject? GetDomainObjectReference ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetDomainObjectReference();
      }
    }

    public bool IsDataComplete
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.IsDataComplete;
        }
      }
    }

    public void EnsureDataComplete ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        _innerEndPoint.EnsureDataComplete();
      }
    }

    public bool? IsSynchronized
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.IsSynchronized;
        }
      }
    }

    public void Touch ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        _innerEndPoint.Touch();
      }
    }

    public void ValidateMandatory ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        _innerEndPoint.ValidateMandatory();
      }
    }

    public IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetOppositeRelationEndPointIDs();
      }
    }

    public bool CanBeCollected
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.CanBeCollected;
        }
      }
    }

    public bool CanBeMarkedIncomplete
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.CanBeMarkedIncomplete;
        }
      }
    }

    public void MarkDataIncomplete ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        _innerEndPoint.MarkDataIncomplete();
      }
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        _innerEndPoint.RegisterOriginalOppositeEndPoint(oppositeEndPoint);
      }
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        _innerEndPoint.UnregisterOriginalOppositeEndPoint(oppositeEndPoint);
      }
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        _innerEndPoint.RegisterCurrentOppositeEndPoint(oppositeEndPoint);
      }
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        _innerEndPoint.UnregisterCurrentOppositeEndPoint(oppositeEndPoint);
      }
    }

    public DomainObject? GetData ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetData();
      }
    }

    public DomainObject? GetOriginalData ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetOriginalData();
      }
    }

    public ObjectID? OppositeObjectID
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.OppositeObjectID;
        }
      }
    }

    public ObjectID? OriginalOppositeObjectID
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.OriginalOppositeObjectID;
        }
      }
    }

    public DomainObject? GetOppositeObject ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetOppositeObject();
      }
    }

    public DomainObject? GetOriginalOppositeObject ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetOriginalOppositeObject();
      }
    }

    public RelationEndPointID? GetOppositeRelationEndPointID ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetOppositeRelationEndPointID();
      }
    }

    public void MarkDataComplete (DomainObject? item)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        _innerEndPoint.MarkDataComplete(item);
      }
    }

    public override string ToString ()
    {
 #if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return string.Format("{0} {{ {1} }}", GetType().Name, _innerEndPoint.ToString());
      }
    }

    #endregion

    private void RaiseStateUpdatedIfNecessary (bool? hasChangedBefore)
    {
      var HasChangedNow = _innerEndPoint.HasChanged;
      // We only raise the update if the state has changed or if we don't know the state before the operation.
      if (hasChangedBefore == null || hasChangedBefore != HasChangedNow)
        _listener.VirtualEndPointStateUpdated(_innerEndPoint.ID, HasChangedNow);
    }

    private IDataManagementCommand CreateStateUpdateRaisingCommandDecorator (IDataManagementCommand command)
    {
      return new VirtualEndPointStateUpdatedRaisingCommandDecorator(command, _innerEndPoint.ID, _listener, () => _innerEndPoint.HasChanged);
    }
  }
}
