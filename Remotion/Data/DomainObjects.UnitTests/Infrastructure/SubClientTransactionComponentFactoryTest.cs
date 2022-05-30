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
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class SubClientTransactionComponentFactoryTest : StandardMappingTest
  {
    private TestableClientTransaction _parentTransaction;
    private Mock<IInvalidDomainObjectManager> _parentInvalidDomainObjectManagerStub;
    private Mock<IEnlistedDomainObjectManager> _parentEnlistedDomainObjectManagerStub;
    private Mock<ITransactionHierarchyManager> _parentTransactionHierarchyManagerStub;
    private Mock<IClientTransactionEventSink> _parentEventSink;

    private SubClientTransactionComponentFactory _factory;

    private TestableClientTransaction _fakeConstructedTransaction;

    public override void SetUp ()
    {
      base.SetUp();

      _parentTransaction = new TestableClientTransaction();
      _parentInvalidDomainObjectManagerStub = new Mock<IInvalidDomainObjectManager>();
      _parentEnlistedDomainObjectManagerStub = new Mock<IEnlistedDomainObjectManager>();
      _parentTransactionHierarchyManagerStub = new Mock<ITransactionHierarchyManager>();
      _parentTransactionHierarchyManagerStub
          .Setup(stub => stub.TransactionHierarchy)
          .Returns(new Mock<IClientTransactionHierarchy>().Object);
      _parentEventSink = new Mock<IClientTransactionEventSink>();

      _factory = SubClientTransactionComponentFactory.Create(
          _parentTransaction,
          _parentInvalidDomainObjectManagerStub.Object,
          _parentEnlistedDomainObjectManagerStub.Object,
          _parentTransactionHierarchyManagerStub.Object,
          _parentEventSink.Object);

      _fakeConstructedTransaction = new TestableClientTransaction();
    }

    [Test]
    public void CreateTransactionHierarchyManager ()
    {
      var eventSink = new Mock<IClientTransactionEventSink>();

      var transactionHierarchyManager = _factory.CreateTransactionHierarchyManager(_fakeConstructedTransaction, eventSink.Object);

      Assert.That(transactionHierarchyManager, Is.TypeOf<TransactionHierarchyManager>());
      Assert.That(((TransactionHierarchyManager)transactionHierarchyManager).ThisTransaction, Is.SameAs(_fakeConstructedTransaction));
      Assert.That(((TransactionHierarchyManager)transactionHierarchyManager).ThisEventSink, Is.SameAs(eventSink.Object));
      Assert.That(((TransactionHierarchyManager)transactionHierarchyManager).ParentTransaction, Is.SameAs(_parentTransaction));
      Assert.That(((TransactionHierarchyManager)transactionHierarchyManager).ParentHierarchyManager, Is.SameAs(_parentTransactionHierarchyManagerStub.Object));
      Assert.That(((TransactionHierarchyManager)transactionHierarchyManager).ParentEventSink, Is.SameAs(_parentEventSink.Object));
    }

    [Test]
    public void CreateApplicationData ()
    {
      Assert.That(_factory.CreateApplicationData(_fakeConstructedTransaction), Is.SameAs(_parentTransaction.ApplicationData));
    }

    [Test]
    public void CreateEnlistedObjectManager ()
    {
      var manager = _factory.CreateEnlistedObjectManager(_fakeConstructedTransaction);
      Assert.That(manager, Is.SameAs(_parentEnlistedDomainObjectManagerStub.Object));
    }

    [Test]
    public void CreateInvalidDomainObjectManager ()
    {
      _parentInvalidDomainObjectManagerStub.Setup(stub => stub.InvalidObjectIDs).Returns(new ObjectID[0]);
      var eventSink = new Mock<IClientTransactionEventSink>();

      var manager = _factory.CreateInvalidDomainObjectManager(_fakeConstructedTransaction, eventSink.Object);
      Assert.That(manager, Is.TypeOf(typeof(InvalidDomainObjectManager)));
      Assert.That(((InvalidDomainObjectManager)manager).TransactionEventSink, Is.SameAs(eventSink.Object));
    }

    [Test]
    public void CreateInvalidDomainObjectManager_AutomaticallyMarksInvalid_ObjectsInvalidOrDeletedInParentTransaction ()
    {
      var objectInvalidInParent = _parentTransaction.ExecuteInScope(() => Order.NewObject());
      var objectDeletedInParent = _parentTransaction.GetObject(DomainObjectIDs.Order3, false);
      var objectLoadedInParent = _parentTransaction.GetObject(DomainObjectIDs.Order4, false);

      _parentInvalidDomainObjectManagerStub.Setup(stub => stub.InvalidObjectIDs).Returns(new[] { objectInvalidInParent.ID });
      _parentInvalidDomainObjectManagerStub.Setup(stub => stub.GetInvalidObjectReference(objectInvalidInParent.ID)).Returns(objectInvalidInParent);

      _parentTransaction.Delete(objectDeletedInParent);

      var invalidOjectManager = _factory.CreateInvalidDomainObjectManager(
          _fakeConstructedTransaction, new Mock<IClientTransactionEventSink>().Object);

      Assert.That(invalidOjectManager.IsInvalid(objectInvalidInParent.ID), Is.True);
      Assert.That(invalidOjectManager.IsInvalid(objectDeletedInParent.ID), Is.True);
      Assert.That(invalidOjectManager.IsInvalid(objectLoadedInParent.ID), Is.False);
    }

    [Test]
    public void CreatePersistenceStrategy ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_parentTransaction, false);

      var result = _factory.CreatePersistenceStrategy(_fakeConstructedTransaction);

      Assert.That(result, Is.TypeOf<SubPersistenceStrategy>());
      var parentTransactionContext = ((SubPersistenceStrategy)result).ParentTransactionContext;
      Assert.That(parentTransactionContext, Is.TypeOf<ParentTransactionContext>());
      Assert.That(((ParentTransactionContext)parentTransactionContext).ParentTransaction, Is.SameAs(_parentTransaction));
      Assert.That(
          ((ParentTransactionContext)parentTransactionContext).ParentInvalidDomainObjectManager,
          Is.SameAs(_parentInvalidDomainObjectManagerStub.Object));
    }

    [Test]
    public void CreatePersistenceStrategy_CanBeMixed ()
    {
      ClientTransactionTestHelper.SetIsWriteable(_parentTransaction, false);

      using (MixinConfiguration.BuildNew().ForClass<SubPersistenceStrategy>().AddMixin<NullMixin>().EnterScope())
      {
        var result = _factory.CreatePersistenceStrategy(_fakeConstructedTransaction);
        Assert.That(Mixin.Get<NullMixin>(result), Is.Not.Null);
      }
    }

    [Test]
    public void CreateRelationEndPointManager ()
    {
      var lazyLoader = new Mock<ILazyLoader>();
      var endPointProvider = new Mock<IRelationEndPointProvider>();
      var eventSink = new Mock<IClientTransactionEventSink>();
      var dataContainerMap = new Mock<IDataContainerMapReadOnlyView>();

      var relationEndPointManager =
          (RelationEndPointManager)PrivateInvoke.InvokeNonPublicMethod(
              _factory,
              "CreateRelationEndPointManager",
              _fakeConstructedTransaction,
              endPointProvider.Object,
              lazyLoader.Object,
              eventSink.Object,
              dataContainerMap.Object);

      Assert.That(relationEndPointManager.ClientTransaction, Is.SameAs(_fakeConstructedTransaction));
      Assert.That(relationEndPointManager.RegistrationAgent, Is.TypeOf<RelationEndPointRegistrationAgent>());
      Assert.That(relationEndPointManager.EndPointFactory, Is.TypeOf<StateUpdateRaisingRelationEndPointFactoryDecorator>());

      Assert.That(RelationEndPointManagerTestHelper.GetMap(relationEndPointManager).TransactionEventSink, Is.SameAs(eventSink.Object));

      var stateUpdateRaisingFactory = (StateUpdateRaisingRelationEndPointFactoryDecorator)relationEndPointManager.EndPointFactory;
      Assert.That(
          stateUpdateRaisingFactory.Listener,
          Is.TypeOf<VirtualEndPointStateUpdateListener>()
              .With.Property<VirtualEndPointStateUpdateListener>(l => l.TransactionEventSink).SameAs(eventSink.Object));
      Assert.That(stateUpdateRaisingFactory.InnerFactory, Is.TypeOf<RelationEndPointFactory>());
      var endPointFactory = ((RelationEndPointFactory)stateUpdateRaisingFactory.InnerFactory);
      Assert.That(endPointFactory.ClientTransaction, Is.SameAs(_fakeConstructedTransaction));
      Assert.That(endPointFactory.LazyLoader, Is.SameAs(lazyLoader.Object));
      Assert.That(endPointFactory.EndPointProvider, Is.SameAs(endPointProvider.Object));
      Assert.That(endPointFactory.TransactionEventSink, Is.SameAs(eventSink.Object));

      Assert.That(endPointFactory.DomainObjectCollectionEndPointDataManagerFactory, Is.TypeOf(typeof(DomainObjectCollectionEndPointDataManagerFactory)));
      var domainObjectCollectionEndPointDataManagerFactory = (DomainObjectCollectionEndPointDataManagerFactory)endPointFactory.DomainObjectCollectionEndPointDataManagerFactory;
      Assert.That(domainObjectCollectionEndPointDataManagerFactory.ChangeDetectionStrategy, Is.TypeOf<SubDomainObjectCollectionEndPointChangeDetectionStrategy>());
      Assert.That(endPointFactory.VirtualObjectEndPointDataManagerFactory, Is.TypeOf<VirtualObjectEndPointDataManagerFactory>());

      Assert.That(endPointFactory.DomainObjectCollectionEndPointCollectionProvider, Is.TypeOf<DomainObjectCollectionEndPointCollectionProvider>());
      var domainObjectCollectionEndPointCollectionProvider = (DomainObjectCollectionEndPointCollectionProvider)endPointFactory.DomainObjectCollectionEndPointCollectionProvider;
      Assert.That(
          domainObjectCollectionEndPointCollectionProvider.DataStrategyFactory,
          Is.TypeOf<AssociatedDomainObjectCollectionDataStrategyFactory>()
              .With.Property((AssociatedDomainObjectCollectionDataStrategyFactory f) => f.VirtualEndPointProvider).SameAs(endPointProvider.Object));

      Assert.That(endPointFactory.VirtualCollectionEndPointDataManagerFactory, Is.TypeOf(typeof(VirtualCollectionEndPointDataManagerFactory)));
      var virtualCollectionEndPointDataManagerFactory = (VirtualCollectionEndPointDataManagerFactory)endPointFactory.VirtualCollectionEndPointDataManagerFactory;
      Assert.That(virtualCollectionEndPointDataManagerFactory.DataContainerMap, Is.SameAs(dataContainerMap.Object));

      Assert.That(endPointFactory.VirtualCollectionEndPointCollectionProvider, Is.TypeOf<VirtualCollectionEndPointCollectionProvider>());
      var virtualCollectionEndPointCollectionProvider = (VirtualCollectionEndPointCollectionProvider)endPointFactory.VirtualCollectionEndPointCollectionProvider;
      Assert.That(virtualCollectionEndPointCollectionProvider.VirtualEndPointProvider, Is.SameAs(endPointProvider.Object));
    }

    [Test]
    public void CreateObjectLoader ()
    {
      var persistenceStrategy = new Mock<IPersistenceStrategy>();
      var dataManager = new Mock<IDataManager>();
      var invalidDomainObjectManager = new Mock<IInvalidDomainObjectManager>();
      var eventSink = new Mock<IClientTransactionEventSink>();
      var hierarchyManager = new Mock<ITransactionHierarchyManager>();
      var fakeBasicObjectLoader = new Mock<IObjectLoader>();

      var factoryPartialMock = new Mock<SubClientTransactionComponentFactory>(
          _parentTransaction,
          _parentInvalidDomainObjectManagerStub.Object,
          _parentEnlistedDomainObjectManagerStub.Object,
          _parentTransactionHierarchyManagerStub.Object,
          _parentEventSink.Object)
          { CallBase = true };
      factoryPartialMock
          .Protected()
          .Setup<IObjectLoader>(
              "CreateBasicObjectLoader",
              false,
              _fakeConstructedTransaction,
              eventSink.Object,
              persistenceStrategy.Object,
              invalidDomainObjectManager.Object,
              dataManager.Object,
              hierarchyManager.Object)
          .Returns(fakeBasicObjectLoader.Object)
          .Verifiable();

      var result = PrivateInvoke.InvokeNonPublicMethod(
          factoryPartialMock.Object,
          "CreateObjectLoader",
          _fakeConstructedTransaction,
          eventSink.Object,
          persistenceStrategy.Object,
          invalidDomainObjectManager.Object,
          dataManager.Object,
          hierarchyManager.Object);

      Assert.That(result, Is.SameAs(fakeBasicObjectLoader.Object));
    }

    [Test]
    public void CreateBasicObjectLoader ()
    {
      var persistenceStrategy = new Mock<IPersistenceStrategy>();
      var dataManager = new Mock<IDataManager>();
      var invalidDomainObjectManager = new Mock<IInvalidDomainObjectManager>();
      var eventSink = new Mock<IClientTransactionEventSink>();
      var hierarchyManager = new Mock<ITransactionHierarchyManager>();

      var result = PrivateInvoke.InvokeNonPublicMethod(
          _factory,
          "CreateBasicObjectLoader",
          _fakeConstructedTransaction,
          eventSink.Object,
          persistenceStrategy.Object,
          invalidDomainObjectManager.Object,
          dataManager.Object,
          hierarchyManager.Object);

      Assert.That(result, Is.TypeOf(typeof(ObjectLoader)));
      var objectLoader = (ObjectLoader)result;
      Assert.That(objectLoader.PersistenceStrategy, Is.SameAs(persistenceStrategy.Object));
      Assert.That(
          objectLoader.LoadedObjectDataRegistrationAgent,
          Is.TypeOf<LoadedObjectDataRegistrationAgent>()
              .With.Property((LoadedObjectDataRegistrationAgent agent) => agent.ClientTransaction).SameAs(_fakeConstructedTransaction)
              .With.Property((LoadedObjectDataRegistrationAgent agent) => agent.DataManager).SameAs(dataManager.Object));

      var registrationListener = ((LoadedObjectDataRegistrationAgent)objectLoader.LoadedObjectDataRegistrationAgent).RegistrationListener;
      Assert.That(registrationListener, Is.TypeOf<LoadedObjectDataRegistrationListener>());
      Assert.That(((LoadedObjectDataRegistrationListener)registrationListener).EventSink, Is.SameAs(eventSink.Object));
      Assert.That(((LoadedObjectDataRegistrationListener)registrationListener).HierarchyManager, Is.SameAs(hierarchyManager.Object));

      Assert.That(objectLoader.LoadedObjectDataProvider, Is.TypeOf<LoadedObjectDataProvider>());
      var loadedObjectDataProvider = (LoadedObjectDataProvider)objectLoader.LoadedObjectDataProvider;
      Assert.That(loadedObjectDataProvider.LoadedDataContainerProvider, Is.SameAs(dataManager.Object));
      Assert.That(loadedObjectDataProvider.InvalidDomainObjectManager, Is.SameAs(invalidDomainObjectManager.Object));
    }
  }
}
