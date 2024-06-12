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
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Returns <see cref="AlreadyExistingLoadedObjectData"/> and <see cref="InvalidLoadedObjectData"/> instances for objects known by a given 
  /// <see cref="ILoadedDataContainerProvider"/> or <see cref="IInvalidDomainObjectManager"/>.
  /// </summary>
  public class LoadedObjectDataProvider : ILoadedObjectDataProvider
  {
    private readonly ILoadedDataContainerProvider _loadedDataContainerProvider;
    private readonly IInvalidDomainObjectManager _invalidDomainObjectManager;

    public LoadedObjectDataProvider (ILoadedDataContainerProvider loadedDataContainerProvider, IInvalidDomainObjectManager invalidDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull("loadedDataContainerProvider", loadedDataContainerProvider);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);

      _loadedDataContainerProvider = loadedDataContainerProvider;
      _invalidDomainObjectManager = invalidDomainObjectManager;
    }

    public ILoadedDataContainerProvider LoadedDataContainerProvider
    {
      get { return _loadedDataContainerProvider; }
    }

    public IInvalidDomainObjectManager InvalidDomainObjectManager
    {
      get { return _invalidDomainObjectManager; }
    }

    public ILoadedObjectData? GetLoadedObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      if (_invalidDomainObjectManager.IsInvalid(objectID))
        return new InvalidLoadedObjectData(_invalidDomainObjectManager.GetInvalidObjectReference(objectID));

      var dataContainer = _loadedDataContainerProvider.GetDataContainerWithoutLoading(objectID);
      return dataContainer != null ? new AlreadyExistingLoadedObjectData(dataContainer) : null;
    }
  }
}
