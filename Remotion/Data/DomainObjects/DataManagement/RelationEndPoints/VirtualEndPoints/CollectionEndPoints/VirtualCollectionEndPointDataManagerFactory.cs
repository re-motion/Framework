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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// The <see cref="VirtualCollectionEndPointDataManagerFactory"/> is responsible to create a new <see cref="IVirtualCollectionEndPointDataManager"/> instance.
  /// </summary>
  public class VirtualCollectionEndPointDataManagerFactory : IVirtualCollectionEndPointDataManagerFactory
  {
    public IVirtualCollectionEndPointChangeDetectionStrategy ChangeDetectionStrategy { get; }

    public IDataContainerMapReadOnlyView DataContainerMap { get; }

    public VirtualCollectionEndPointDataManagerFactory (
        IVirtualCollectionEndPointChangeDetectionStrategy changeDetectionStrategy,
        IDataContainerMapReadOnlyView dataContainerMap)
    {
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);
      ArgumentUtility.CheckNotNull ("dataContainerMap", dataContainerMap);

      ChangeDetectionStrategy = changeDetectionStrategy;
      DataContainerMap = dataContainerMap;
    }


    public IVirtualCollectionEndPointDataManager CreateEndPointDataManager (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      return new VirtualCollectionEndPointDataManager (endPointID, ChangeDetectionStrategy, DataContainerMap);
    }

    #region Serialization

    private VirtualCollectionEndPointDataManagerFactory (FlattenedDeserializationInfo info)
    {
      ChangeDetectionStrategy = info.GetValueForHandle<IVirtualCollectionEndPointChangeDetectionStrategy>();
      DataContainerMap = info.GetValueForHandle<IDataContainerMapReadOnlyView>();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (ChangeDetectionStrategy);
      info.AddHandle (DataContainerMap);
    }

    #endregion
  }
}