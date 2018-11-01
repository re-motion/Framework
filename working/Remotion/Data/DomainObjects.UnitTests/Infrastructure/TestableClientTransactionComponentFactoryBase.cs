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
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  public class TestableClientTransactionComponentFactoryBase : ClientTransactionComponentFactoryBase
  {
    public override ITransactionHierarchyManager CreateTransactionHierarchyManager (ClientTransaction constructedTransaction, IClientTransactionEventSink eventSink)
    {
      throw new NotImplementedException();
    }

    public override IDictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction)
    {
      throw new NotImplementedException();
    }

    public override IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction)
    {
      throw new NotImplementedException();
    }

    public override IInvalidDomainObjectManager CreateInvalidDomainObjectManager (ClientTransaction constructedTransaction, IClientTransactionEventSink eventSink)
    {
      throw new NotImplementedException();
    }

    public override IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction)
    {
      throw new NotImplementedException();
    }

    protected override IRelationEndPointManager CreateRelationEndPointManager (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink)
    {
      throw new NotImplementedException();
    }

    protected override IObjectLoader CreateObjectLoader (ClientTransaction constructedTransaction, IClientTransactionEventSink eventSink, IPersistenceStrategy persistenceStrategy, IInvalidDomainObjectManager invalidDomainObjectManager, IDataManager dataManager, ITransactionHierarchyManager hierarchyManager)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IClientTransactionListener> CallCreateListeners (ClientTransaction constructedTransaction)
    {
      return CreateListeners (constructedTransaction);
    }

    public IRelationEndPointManager CallCreateRelationEndPointManager (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink)
    {
      return CreateRelationEndPointManager (constructedTransaction, endPointProvider, lazyLoader, eventSink);
    }

    public IObjectLoader CallCreateObjectLoader (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IPersistenceStrategy persistenceStrategy,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager,
        ITransactionHierarchyManager hierarchyManager)
    {
      return CreateObjectLoader (constructedTransaction, eventSink, persistenceStrategy, invalidDomainObjectManager, dataManager, hierarchyManager);
    }

    public IDataContainerEventListener CallCreateDataContainerEventListener (IClientTransactionEventSink eventSink)
    {
      return CreateDataContainerEventListener (eventSink);
    }

    public IRelationEndPointProvider CallGetEndPointProvider (IDataManager dataManager)
    {
      return GetEndPointProvider (dataManager);
    }

    public ILazyLoader CallGetLazyLoader (IDataManager dataManager)
    {
      return GetLazyLoader (dataManager);
    }
  }
}