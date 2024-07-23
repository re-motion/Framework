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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Decorates another <see cref="IDomainObjectCollectionEndPoint"/>, raising <see cref="IVirtualEndPointStateUpdateListener"/> events whenever the 
  /// return value of the <see cref="HasChanged"/> property has possibly changed.
  /// </summary>
  /// <remarks>
  /// Because the <see cref="HasChanged"/> property of <see cref="IDomainObjectCollectionEndPoint"/> implementations can be expensive to determine, the 
  /// <see cref="StateUpdateRaisingDomainObjectCollectionEndPointDecorator"/> doesn't actually check the property.
  /// Therefore, events may also be raised even the the <see cref="HasChanged"/> property still returns the same value as before. If the end-point's
  /// new state is available via the <see cref="IDomainObjectCollectionEndPoint.HasChangedFast"/> property, the new state is passed to the 
  /// <see cref="IVirtualEndPointStateUpdateListener.VirtualEndPointStateUpdated"/> method as a parameter.
  /// </remarks>
  public class StateUpdateRaisingDomainObjectCollectionEndPointDecorator : IDomainObjectCollectionEndPoint
  {
    /// <summary>
    /// Using an instance of this class around a code block asserts that the change state before and after after the block is the same.
    /// </summary>
    private struct ConstantChangeStateAsserter : IDisposable
    {
      private readonly bool? _changeStateBefore;
      private readonly IDomainObjectCollectionEndPoint _innerEndPoint;

      public ConstantChangeStateAsserter (IDomainObjectCollectionEndPoint innerEndPoint)
      {
        _changeStateBefore = innerEndPoint.HasChangedFast;
        _innerEndPoint = innerEndPoint;
      }

      public void Dispose ()
      {
        Assertion.IsTrue(_changeStateBefore == _innerEndPoint.HasChangedFast);
      }
    }

    private readonly IDomainObjectCollectionEndPoint _innerEndPoint;
    private readonly IVirtualEndPointStateUpdateListener _listener;

    public StateUpdateRaisingDomainObjectCollectionEndPointDecorator (IDomainObjectCollectionEndPoint innerEndPoint, IVirtualEndPointStateUpdateListener listener)
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

    public IDomainObjectCollectionEndPoint InnerEndPoint
    {
      get { return _innerEndPoint; }
    }

    public void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      var sourceCollectionEndPoint = ArgumentUtility.CheckNotNullAndType<StateUpdateRaisingDomainObjectCollectionEndPointDecorator>("source", source);
      var hasChangedFastBefore = _innerEndPoint.HasChangedFast;
      try
      {
        _innerEndPoint.SetDataFromSubTransaction(sourceCollectionEndPoint.InnerEndPoint);
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedFastBefore);
      }
    }

    public void Synchronize ()
    {
      var hasChangedFastBefore = _innerEndPoint.HasChangedFast;
      try
      {
        _innerEndPoint.Synchronize();
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedFastBefore);
      }
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      var hasChangedFastBefore = _innerEndPoint.HasChangedFast;
      try
      {
        _innerEndPoint.SynchronizeOppositeEndPoint(oppositeEndPoint);
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedFastBefore);
      }
    }

    public void Commit ()
    {
      var hasChangedFastBefore = _innerEndPoint.HasChangedFast;
      try
      {
        _innerEndPoint.Commit();
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedFastBefore);
      }
    }

    public void Rollback ()
    {
      var hasChangedFastBefore = _innerEndPoint.HasChangedFast;
      try
      {
        _innerEndPoint.Rollback();
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedFastBefore);
      }
    }

    public void SortCurrentData (Comparison<DomainObject> comparison)
    {
      var hasChangedFastBefore = _innerEndPoint.HasChangedFast;
      try
      {
        _innerEndPoint.SortCurrentData(comparison);
      }
      finally
      {
        RaiseStateUpdatedIfNecessary(hasChangedFastBefore);
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

    public IDataManagementCommand CreateSetCollectionCommand (DomainObjectCollection newCollection)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateSetCollectionCommand(newCollection);
        return CreateStateUpdateRaisingCommandDecorator(command);
      }
    }

    public IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateInsertCommand(insertedRelatedObject, index);
        return CreateStateUpdateRaisingCommandDecorator(command);
      }
    }

    public IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateAddCommand(addedRelatedObject);
        return CreateStateUpdateRaisingCommandDecorator(command);
      }
    }

    public IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateReplaceCommand(index, replacementObject);
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

    public bool? HasChangedFast
    {
      // No assertion on this property because the assertion uses this property...
      get { return _innerEndPoint.HasChangedFast; }
    }

    public bool HasChanged
    {
      // Evaluating this property can indeed change the result of HasChangedFast to switch from null to something else.
      get
      {
#if DEBUG
        var hasChangedFastBefore = _innerEndPoint.HasChangedFast;
        try
        {
#endif
          return _innerEndPoint.HasChanged;
#if DEBUG
        }
        finally
        {
          Assertion.DebugAssert(hasChangedFastBefore == null || hasChangedFastBefore == _innerEndPoint.HasChangedFast);
        }
#endif
      }
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

    public ReadOnlyDomainObjectCollectionDataDecorator GetData ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetData();
      }
    }

    public ReadOnlyDomainObjectCollectionDataDecorator GetOriginalData ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetOriginalData();
      }
    }

    public DomainObjectCollection Collection
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.Collection;
        }
      }
    }

    public DomainObjectCollection OriginalCollection
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
        {
          return _innerEndPoint.OriginalCollection;
        }
      }
    }

    public IDomainObjectCollectionEventRaiser GetCollectionEventRaiser ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetCollectionEventRaiser();
      }
    }

    public DomainObjectCollection GetCollectionWithOriginalData ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetCollectionWithOriginalData();
      }
    }

    public void MarkDataComplete (DomainObject[] items)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter(_innerEndPoint))
#endif
      {
        _innerEndPoint.MarkDataComplete(items);
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

    private void RaiseStateUpdatedIfNecessary (bool? hasChangedFastBefore)
    {
      var hasChangedFastNow = _innerEndPoint.HasChangedFast;
      // We only raise the update if the state has changed or if we don't know the state before the operation.
      if (hasChangedFastBefore == null || hasChangedFastBefore != hasChangedFastNow)
        _listener.VirtualEndPointStateUpdated(_innerEndPoint.ID, hasChangedFastNow);
    }

    private IDataManagementCommand CreateStateUpdateRaisingCommandDecorator (IDataManagementCommand command)
    {
      return new VirtualEndPointStateUpdatedRaisingCommandDecorator(command, _innerEndPoint.ID, _listener, () => _innerEndPoint.HasChangedFast);
    }
  }
}
