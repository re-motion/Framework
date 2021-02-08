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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Keeps the data of a <see cref="IVirtualCollectionEndPoint"/>.
  /// </summary>
  public class VirtualCollectionEndPointDataManager : IVirtualCollectionEndPointDataManager
  {
    //TODO: RM-7294: merge ChangeTrackingVirtualCollectionDataDecorator into DataManager and make DataManager work for loaded and unloaded state

    public RelationEndPointID EndPointID { get; }

    public IDataContainerMapReadOnlyView DataContainerMap { get; }

    private readonly VirtualCollectionData _virtualCollectionData;

    public VirtualCollectionEndPointDataManager (
        RelationEndPointID endPointID,
        IDataContainerMapReadOnlyView dataContainerMap)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("dataContainerMap", dataContainerMap);

      EndPointID = endPointID;
      DataContainerMap = dataContainerMap;

      _virtualCollectionData = new VirtualCollectionData (endPointID, dataContainerMap, ValueAccess.Current);
    }

    public IVirtualCollectionData CollectionData
    {
      get { return _virtualCollectionData; }
    }

    public ReadOnlyVirtualCollectionDataDecorator GetOriginalCollectionData ()
    {
      return _virtualCollectionData.GetOriginalData();
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _virtualCollectionData.ResetCachedDomainObjects();
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _virtualCollectionData.ResetCachedDomainObjects();
    }

    public void SetDataFromSubTransaction (IVirtualCollectionEndPointDataManager sourceDataManager, IRelationEndPointProvider endPointProvider)
    {
      ArgumentUtility.CheckNotNull ("sourceDataManager", sourceDataManager);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      _virtualCollectionData.ResetCachedDomainObjects();
    }

    public void Commit ()
    {
      _virtualCollectionData.ResetCachedDomainObjects();
    }

    public void Rollback ()
    {
      _virtualCollectionData.ResetCachedDomainObjects();
    }

    #region Serialization

    // ReSharper disable UnusedMember.Local
    private VirtualCollectionEndPointDataManager (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      EndPointID = info.GetValueForHandle<RelationEndPointID>();

      _virtualCollectionData = info.GetValueForHandle<VirtualCollectionData>();
    }

    // ReSharper restore UnusedMember.Local

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (EndPointID);
      info.AddHandle (_virtualCollectionData);
    }

    #endregion
  }
}