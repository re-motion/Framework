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
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime
{
  /// <summary>
  /// Represents the context of an object being initialized via <see cref="IObjectLifetimeAgent.NewObject"/>.
  /// </summary>
  public class NewObjectInitializationContext : ObjectReferenceInitializationContext
  {
    private readonly IDataManager _dataManager;

    public NewObjectInitializationContext (
        ObjectID objectID,
        ClientTransaction rootTransaction,
        IEnlistedDomainObjectManager enlistedDomainObjectManager,
        IDataManager dataManager)
      : base (objectID, rootTransaction, enlistedDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      _dataManager = dataManager;
    }

    public IDataManager DataManager
    {
      get { return _dataManager; }
    }

    public override void RegisterObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      base.RegisterObject (domainObject);

      // The rest of this method should never throw.

      var newDataContainer = DataContainer.CreateNew (domainObject.ID);
      newDataContainer.SetDomainObject (domainObject);

      Assertion.IsNull (_dataManager.DataContainers[newDataContainer.ID], "since the registration succeeded, there cannot be a DataContainer with this ID.");
      _dataManager.RegisterDataContainer (newDataContainer);
    }
  }
}