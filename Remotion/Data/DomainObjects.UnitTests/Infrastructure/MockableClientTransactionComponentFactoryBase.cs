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
using System.Linq.Expressions;
using Moq;
using Moq.Language.Flow;
using Moq.Protected;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  public abstract class MockableClientTransactionComponentFactoryBase : ClientTransactionComponentFactoryBase
  {
    public MockableClientTransactionComponentFactoryBase ()
    {
    }

    public abstract override ITransactionHierarchyManager CreateTransactionHierarchyManager (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink);

    public abstract override IDictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction);

    public abstract override IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction);

    public abstract override IInvalidDomainObjectManager CreateInvalidDomainObjectManager (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink);

    public abstract override IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction);

    protected abstract override IRelationEndPointManager CreateRelationEndPointManager (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink,
        IDataContainerMapReadOnlyView dataContainerMap);

    protected abstract override IObjectLoader CreateObjectLoader (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IPersistenceStrategy persistenceStrategy,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager,
        ITransactionHierarchyManager hierarchyManager);

    protected abstract override IDataContainerEventListener CreateDataContainerEventListener (IClientTransactionEventSink eventSink);

    protected abstract override ILazyLoader GetLazyLoader (IDataManager dataManager);

    protected abstract override IRelationEndPointProvider GetEndPointProvider (IDataManager dataManager);

    public ISetup<MockableClientTransactionComponentFactoryBase, IEnumerable<IClientTransactionListener>> SetupCreateListeners (
        Mock<MockableClientTransactionComponentFactoryBase> mock,
        ClientTransaction constructedTransaction)
    {
      Assertion.IsTrue(ReferenceEquals(Mock.Get(this), mock), "ReferenceEquals(Mock.Get(this), mock)");

      return mock.Setup(_ => _.CreateListeners(constructedTransaction));
    }

    public ISetup<MockableClientTransactionComponentFactoryBase, IRelationEndPointManager> SetupCreateRelationEndPointManager (
        Mock<MockableClientTransactionComponentFactoryBase> mock,
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink,
        Expression<Func<IDataContainerMapReadOnlyView, bool>> dataContainerMapMatchExpression)
    {
      Assertion.IsTrue(ReferenceEquals(Mock.Get(this), mock), "ReferenceEquals(Mock.Get(this), mock)");

      return mock
          .Protected()
          .Setup<IRelationEndPointManager>(
              nameof(CreateRelationEndPointManager),
              false,
              constructedTransaction,
              endPointProvider,
              lazyLoader,
              eventSink,
              ItExpr.Is(dataContainerMapMatchExpression));
    }

    public ISetup<MockableClientTransactionComponentFactoryBase, IObjectLoader> SetupCreateObjectLoader (
        Mock<MockableClientTransactionComponentFactoryBase> mock,
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IPersistenceStrategy persistenceStrategy,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        Expression<Func<IDataManager, bool>> dataManagerMatchExpression,
        ITransactionHierarchyManager hierarchyManager)
    {
      Assertion.IsTrue(ReferenceEquals(Mock.Get(this), mock), "ReferenceEquals(Mock.Get(this), mock)");

      return mock
          .Protected()
          .Setup<IObjectLoader>(
              nameof(CreateObjectLoader),
              false,
              constructedTransaction,
              eventSink,
              persistenceStrategy,
              invalidDomainObjectManager,
              ItExpr.Is(dataManagerMatchExpression),
              hierarchyManager);
    }

    public ISetup<MockableClientTransactionComponentFactoryBase, IDataContainerEventListener> SetupCreateDataContainerEventListener (
        Mock<MockableClientTransactionComponentFactoryBase> mock,
        IClientTransactionEventSink eventSink)
    {
      Assertion.IsTrue(ReferenceEquals(Mock.Get(this), mock), "ReferenceEquals(Mock.Get(this), mock)");

      return mock.Setup(_ => _.CreateDataContainerEventListener(eventSink));
    }

    public ISetup<MockableClientTransactionComponentFactoryBase, IRelationEndPointProvider> SetupGetEndPointProvider (
        Mock<MockableClientTransactionComponentFactoryBase> mock,
        Expression<Func<IDataManager, bool>> dataManagerMatchExpression)
    {
      Assertion.IsTrue(ReferenceEquals(Mock.Get(this), mock), "ReferenceEquals(Mock.Get(this), mock)");

      return mock.Protected().Setup<IRelationEndPointProvider>(nameof(GetEndPointProvider), false, ItExpr.Is(dataManagerMatchExpression));
    }

    public ISetup<MockableClientTransactionComponentFactoryBase, ILazyLoader> SetupGetLazyLoader (
        Mock<MockableClientTransactionComponentFactoryBase> mock,
        Expression<Func<IDataManager, bool>> dataManagerMatchExpression)
    {
      Assertion.IsTrue(ReferenceEquals(Mock.Get(this), mock), "ReferenceEquals(Mock.Get(this), mock)");

      return mock.Protected().Setup<ILazyLoader>(nameof(GetLazyLoader), false, ItExpr.Is(dataManagerMatchExpression));
    }
  }
}
