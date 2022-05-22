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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="VirtualObjectEndPoint"/> where not all of its data is available (ie., the end-point has not been (lazily) 
  /// loaded, or it has been unloaded).
  /// </summary>
  public class IncompleteVirtualObjectEndPointLoadState
      : IncompleteVirtualEndPointLoadStateBase<IVirtualObjectEndPoint, IDomainObject?, IVirtualObjectEndPointDataManager, IVirtualObjectEndPointLoadState>,
        IVirtualObjectEndPointLoadState
  {
    private readonly IVirtualObjectEndPointDataManagerFactory _dataManagerFactory;

    public IncompleteVirtualObjectEndPointLoadState (
        IEndPointLoader endPointLoader,
        IVirtualObjectEndPointDataManagerFactory dataManagerFactory)
        : base(endPointLoader)
    {
      ArgumentUtility.CheckNotNull("dataManagerFactory", dataManagerFactory);
      _dataManagerFactory = dataManagerFactory;
    }

    public IVirtualObjectEndPointDataManagerFactory DataManagerFactory
    {
      get { return _dataManagerFactory; }
    }

    public void EnsureDataComplete (IVirtualObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);

      EndPointLoader.LoadEndPointAndGetNewState(endPoint);
    }

    public void MarkDataComplete (IVirtualObjectEndPoint endPoint, IDomainObject? item, Action<IVirtualObjectEndPointDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      ArgumentUtility.CheckNotNull("stateSetter", stateSetter);

      var items = item == null ? Array.Empty<IDomainObject>() : EnumerableUtility.Singleton(item);
      MarkDataComplete(endPoint, items, stateSetter);
    }

    public IDataManagementCommand CreateSetCommand (IVirtualObjectEndPoint virtualObjectEndPoint, IDomainObject? newRelatedObject)
    {
      ArgumentUtility.CheckNotNull("virtualObjectEndPoint", virtualObjectEndPoint);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState(virtualObjectEndPoint);
      return completeState.CreateSetCommand(virtualObjectEndPoint, newRelatedObject);
    }

    public IDataManagementCommand CreateDeleteCommand (IVirtualObjectEndPoint virtualObjectEndPoint)
    {
      ArgumentUtility.CheckNotNull("virtualObjectEndPoint", virtualObjectEndPoint);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState(virtualObjectEndPoint);
      return completeState.CreateDeleteCommand(virtualObjectEndPoint);
    }

    protected override IVirtualObjectEndPointDataManager CreateEndPointDataManager (IVirtualObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull("endPoint", endPoint);
      return _dataManagerFactory.CreateEndPointDataManager(endPoint.ID);
    }

    #region Serialization

    public IncompleteVirtualObjectEndPointLoadState (FlattenedDeserializationInfo info)
        : base(info)
    {
      _dataManagerFactory = info.GetValueForHandle<IVirtualObjectEndPointDataManagerFactory>();
    }

    protected override void SerializeSubclassData (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull("info", info);
      info.AddHandle(_dataManagerFactory);
    }

    #endregion Serialization

  }
}
