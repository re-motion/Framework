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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [Serializable]
  public class TestableClientTransaction : ClientTransaction
  {
    public TestableClientTransaction ()
        : this(
            RootClientTransactionComponentFactory.Create(
                SafeServiceLocator.Current.GetInstance<IStorageSettings>(),
                SafeServiceLocator.Current.GetInstance<IPersistenceService>(),
                SafeServiceLocator.Current.GetInstance<IPersistenceExtensionFactory>(),
                SafeServiceLocator.Current.GetInstance<IStorageAccessResolver>()))
    {
    }

    protected TestableClientTransaction (IClientTransactionComponentFactory componentFactory) : base(componentFactory)
    {
    }

    public IClientTransactionEventBroker EventBroker
    {
      get { return (IClientTransactionEventBroker)PrivateInvoke.GetNonPublicProperty(this, typeof(ClientTransaction), "_eventBroker"); }
    }

    public new DomainObject GetObject (ObjectID id, bool includeDeleted)
    {
      return base.GetObject(id, includeDeleted);
    }

    public new DomainObject TryGetObject (ObjectID id)
    {
      return base.TryGetObject(id);
    }

    public new DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
    {
      return base.GetRelatedObject(relationEndPointID);
    }

    public new IReadOnlyList<IDomainObject> GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
    {
      return base.GetOriginalRelatedObjects(relationEndPointID);
    }

    public new IReadOnlyList<IDomainObject> GetRelatedObjects (RelationEndPointID relationEndPointID)
    {
      return base.GetRelatedObjects(relationEndPointID);
    }

    public new DataManager DataManager
    {
      get { return (DataManager)base.DataManager; }
    }

    public new void AddListener (IClientTransactionListener listener)
    {
      base.AddListener(listener);
    }

    public new void RemoveListener (IClientTransactionListener listener)
    {
      base.RemoveListener(listener);
    }

    public new void Delete (DomainObject domainObject)
    {
      base.Delete(domainObject);
    }
  }
}
