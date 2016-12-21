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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Holds an <see cref="ILoadedObjectData"/> object along with a <see cref="DataContainer"/> that holds the data from the underlying data source. 
  /// originally loaded from thedata source.
  /// </summary>
  public struct LoadedObjectDataWithDataSourceData
  {
    private readonly ILoadedObjectData _loadedObjectData;
    private readonly DataContainer _dataSourceData;

    public LoadedObjectDataWithDataSourceData (ILoadedObjectData loadedObjectData, DataContainer dataSourceData)
    {
      ArgumentUtility.CheckNotNull ("loadedObjectData", loadedObjectData);

      if (!loadedObjectData.IsNull && dataSourceData == null)
        throw new ArgumentException ("The dataSourceData parameter must not be null when loadedObjectData.IsNull is false.", "dataSourceData");
      if (loadedObjectData.IsNull && dataSourceData != null)
        throw new ArgumentException ("The dataSourceData parameter must be null when loadedObjectData.IsNull is true.", "dataSourceData");
      if (!loadedObjectData.IsNull && loadedObjectData.ObjectID != dataSourceData.ID)
        throw new ArgumentException ("The ID of the dataSourceData parameter does not match the loadedObjectData.", "dataSourceData");

      _loadedObjectData = loadedObjectData;
      _dataSourceData = dataSourceData;
    }

    public ILoadedObjectData LoadedObjectData
    {
      get { return _loadedObjectData; }
    }

    public DataContainer DataSourceData
    {
      get { return _dataSourceData; }
    }

    public bool IsNull
    {
      get { return _loadedObjectData.IsNull; }
    }
  }
}