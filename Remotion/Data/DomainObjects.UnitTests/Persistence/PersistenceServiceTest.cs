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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class PersistenceServiceTest : ClientTransactionBaseTest
  {
    private class TestablePersistenceService : PersistenceService
    {
      public Action<IEnumerable<IStorageProvider>> CheckProvidersCompatibleForSaveCallback { get; set; }
      public Func<IEnumerable<IReadOnlyStorageProvider>, IDisposable> BeginTransactionCallback { get; set; }
      public Action<IEnumerable<IReadOnlyStorageProvider>, IDisposable> CommitTransactionCallback { get; set; }
      public Action<IEnumerable<IReadOnlyStorageProvider>, IDisposable> RollbackTransactionCallback { get; set; }

      public TestablePersistenceService ()
      {
      }

      protected override void CheckProvidersCompatibleForSave (IEnumerable<IStorageProvider> providers)
      {
        if (CheckProvidersCompatibleForSaveCallback != null)
          CheckProvidersCompatibleForSaveCallback(providers);
        else
          base.CheckProvidersCompatibleForSave(providers);
      }

      protected override IDisposable BeginTransaction (IEnumerable<IReadOnlyStorageProvider> providers)
      {
        if (BeginTransactionCallback != null)
          return BeginTransactionCallback(providers);
        else
        {
          return base.BeginTransaction(providers);
        }
      }

      protected override void CommitTransaction (IEnumerable<IReadOnlyStorageProvider> providers, IDisposable context)
      {
        if (CommitTransactionCallback != null)
          CommitTransactionCallback(providers, context);
        else
        {
          base.CommitTransaction(providers, context);
        }
      }

      protected override void RollbackTransaction (IEnumerable<IReadOnlyStorageProvider> providers, IDisposable context)
      {
        if (RollbackTransactionCallback != null)
          RollbackTransactionCallback(providers, context);
        else
        {
          base.RollbackTransaction(providers, context);
        }
      }
    }

    private PersistenceService _persistenceService;
    private StorageProviderManager _storageProviderManager;

    private ObjectID _invalidOrderID1;
    private ObjectID _invalidOrderID2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _persistenceService = new PersistenceService();
      _storageProviderManager = new StorageProviderManager(NullPersistenceExtension.Instance, StorageSettings);

      var guid1 = new Guid("11111111111111111111111111111111");
      var guid2 = new Guid("22222222222222222222222222222222");
      _invalidOrderID1 = new ObjectID(typeof(Order), guid1);
      _invalidOrderID2 = new ObjectID(typeof(Order), guid2);
    }

    public override void TearDown ()
    {
      _storageProviderManager.Dispose();
      base.TearDown();
    }

    [Test]
    public void LoadDataContainer ()
    {
      var result = _persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.Order1);
      Assert.That(result.ObjectID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(result.LocatedObject.ID, Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void LoadDataContainer_WithNonExistingValue ()
    {
      var result = _persistenceService.LoadDataContainer(_storageProviderManager, _invalidOrderID1);
      Assert.That(result.ObjectID, Is.EqualTo(_invalidOrderID1));
      Assert.That(result.LocatedObject, Is.Null);
    }

    [Test]
    public void LoadDataContainers ()
    {
      Assert.AreNotEqual(DomainObjectIDs.Order1.StorageProviderDefinition.Name, DomainObjectIDs.Official1, "Different storage providers");

      var storageProviderMock = new Mock<StorageProvider>(MockBehavior.Strict, UnitTestStorageProviderDefinition, NullPersistenceExtension.Instance);

      var officialDC1 = DataContainer.CreateNew(DomainObjectIDs.Official1);
      var officialDC2 = DataContainer.CreateNew(DomainObjectIDs.Official2);

      var officialDCs = new List<ObjectLookupResult<DataContainer>>();
      officialDCs.Add(new ObjectLookupResult<DataContainer>(DomainObjectIDs.Official1, officialDC1));
      officialDCs.Add(new ObjectLookupResult<DataContainer>(DomainObjectIDs.Official2, officialDC2));

      storageProviderMock
          .Setup(mock => mock.LoadDataContainers(new[] { DomainObjectIDs.Official1, DomainObjectIDs.Official2 }))
          .Returns(officialDCs)
          .Verifiable();

      ObjectLookupResult<DataContainer>[] actualResults;
      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope(storageProviderMock.Object))
      {
        actualResults = _persistenceService.LoadDataContainers(
            _storageProviderManager,
            new[]
            {
                DomainObjectIDs.Order1, DomainObjectIDs.Official1, DomainObjectIDs.Order3,
                DomainObjectIDs.Official2
            }).ToArray();
      }

      storageProviderMock.Verify();

      Assert.That(actualResults, Has.Length.EqualTo(4));
      Assert.That(actualResults[0].ObjectID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(actualResults[0].LocatedObject.ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(actualResults[1].ObjectID, Is.EqualTo(officialDC1.ID));
      Assert.That(actualResults[1].LocatedObject, Is.SameAs(officialDC1));
      Assert.That(actualResults[2].ObjectID, Is.EqualTo(DomainObjectIDs.Order3));
      Assert.That(actualResults[2].LocatedObject.ID, Is.EqualTo(DomainObjectIDs.Order3));
      Assert.That(actualResults[3].ObjectID, Is.EqualTo(officialDC2.ID));
      Assert.That(actualResults[3].LocatedObject, Is.SameAs(officialDC2));
    }

    [Test]
    public void LoadDataContainers_DuplicatesAreReplacedBySingleDataContainer ()
    {
      Assert.AreNotEqual(DomainObjectIDs.Order1.StorageProviderDefinition.Name, DomainObjectIDs.Official1, "Different storage providers");

      var storageProviderMock = new Mock<StorageProvider>(MockBehavior.Strict, UnitTestStorageProviderDefinition, NullPersistenceExtension.Instance);

      var officialDC1a = DataContainer.CreateNew(DomainObjectIDs.Official1);
      var officialDC1b = DataContainer.CreateNew(DomainObjectIDs.Official1);

      var officialResults = new List<ObjectLookupResult<DataContainer>>();
      officialResults.Add(new ObjectLookupResult<DataContainer>(DomainObjectIDs.Official1, officialDC1a));
      officialResults.Add(new ObjectLookupResult<DataContainer>(DomainObjectIDs.Official1, officialDC1b));

      storageProviderMock
          .Setup(mock => mock.LoadDataContainers(new[] { DomainObjectIDs.Official1, DomainObjectIDs.Official1 }))
          .Returns(officialResults)
          .Verifiable();

      ObjectLookupResult<DataContainer>[] actualResults;
      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope(storageProviderMock.Object))
      {
        actualResults = _persistenceService.LoadDataContainers(
            _storageProviderManager,
            new[]
            {
                DomainObjectIDs.Official1, DomainObjectIDs.Official1
            }).ToArray();
      }

      storageProviderMock.Verify();

      Assert.That(actualResults, Has.Length.EqualTo(2));
      Assert.That(actualResults[0].ObjectID, Is.EqualTo(DomainObjectIDs.Official1));
      Assert.That(actualResults[0].LocatedObject, Is.SameAs(officialDC1a));
      Assert.That(actualResults[1].ObjectID, Is.EqualTo(DomainObjectIDs.Official1));
      Assert.That(actualResults[1].LocatedObject, Is.SameAs(officialDC1a));
    }

    [Test]
    public void LoadDataContainers_NotFound ()
    {
      var objectIds = new[] { _invalidOrderID1, _invalidOrderID2, DomainObjectIDs.Order1 };

      var dataContainers = _persistenceService.LoadDataContainers(_storageProviderManager, objectIds).ToArray();

      Assert.That(dataContainers.Length, Is.EqualTo(3));
      Assert.That(dataContainers[0].ObjectID, Is.EqualTo(_invalidOrderID1));
      Assert.That(dataContainers[0].LocatedObject, Is.Null);
      Assert.That(dataContainers[1].ObjectID, Is.EqualTo(_invalidOrderID2));
      Assert.That(dataContainers[1].LocatedObject, Is.Null);
      Assert.That(dataContainers[2].ObjectID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(dataContainers[2].LocatedObject.ID, Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void LoadRelatedDataContainer ()
    {
      DataContainer orderTicketContainer = _persistenceService.LoadRelatedDataContainer(
          _storageProviderManager,
          RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));

      var checker = new DataContainerChecker();
      checker.Check(TestDataContainerObjectMother.CreateOrderTicket1DataContainer(), orderTicketContainer);
    }

    [Test]
    public void LoadRelatedDataContainer_NonVirtualEndPoint ()
    {
      var relationEndPointID = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainer(_storageProviderManager, relationEndPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("LoadRelatedDataContainer can only be used with virtual end points.", "relationEndPointID"));
    }

    [Test]
    public void LoadRelatedDataContainer_OptionalNullID ()
    {
      var id = new ObjectID("ClassWithGuidKey", new Guid("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      DataContainer relatedDataContainer = _persistenceService.LoadRelatedDataContainer(
          _storageProviderManager,
          RelationEndPointID.Create(id, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional"));

      Assert.That(relatedDataContainer, Is.Null);
    }

    [Test]
    public void LoadRelatedDataContainer_NonOptionalNullID ()
    {
      var id = new ObjectID("ClassWithGuidKey", new Guid("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainer(
          _storageProviderManager,
          RelationEndPointID.Create(id, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional")),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional' on "
                  + "object 'ClassWithGuidKey|672c8754-c617-4b7a-890c-bfef8ac86564|System.Guid' contains no item."));
    }

    [Test]
    public void LoadRelatedDataContainer_NonOptionalNullID_WithInheritance ()
    {
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainer(
          _storageProviderManager,
          RelationEndPointID.Create(DomainObjectIDs.PartnerWithoutCeo, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo")),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo' on object "
                  + "'Partner|a65b123a-6e17-498e-a28e-946217c0ae30|System.Guid' contains no item."));
    }

    [Test]
    public void LoadRelatedDataContainer_OverValidMandatoryRelation ()
    {
      var id = new ObjectID("ClassWithGuidKey", new Guid("{D0A1BDDE-B13F-47c1-98BD-EBAE21189B01}"));

      DataContainer relatedContainer = _persistenceService.LoadRelatedDataContainer(
          _storageProviderManager,
          RelationEndPointID.Create(id, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional"));

      ObjectID expectedID = new ObjectID("ClassWithValidRelations", new Guid("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      Assert.That(relatedContainer, Is.Not.Null);
      Assert.That(relatedContainer.ID, Is.EqualTo(expectedID));
    }

    [Test]
    public void LoadRelatedDataContainer_OverInvalidNonOptionalRelation ()
    {
      ObjectID id = new ObjectID("ClassWithGuidKey", new Guid("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainer(
          _storageProviderManager,
          RelationEndPointID.Create(id, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional")),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional' on object "
                  + "'ClassWithGuidKey|672c8754-c617-4b7a-890c-bfef8ac86564|System.Guid' contains no item."));
    }

    [Test]
    public void LoadRelatedDataContainer_ForOneToManyRelation ()
    {
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainer(
          _storageProviderManager,
          RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems")),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "Cannot load a single related data container for one-to-many relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem:"
                  + "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order->Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems'."));
    }

    [Test]
    public void LoadRelatedDataContainer_WithInvalidClassIDOverVirtualEndPoint ()
    {
      ObjectID companyID = new ObjectID("Company", new Guid("{C3DB20D6-138E-4ced-8576-E81BB4B7961F}"));

      RelationEndPointID endPointID = RelationEndPointID.Create(companyID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainer(_storageProviderManager, endPointID),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company' of the loaded DataContainer "
                  + "'Ceo|c3db20d6-138e-4ced-8576-e81bb4b7961f|System.Guid' refers to ClassID 'Customer', but the actual ClassID is 'Company'."));
    }

    [Test]
    public void LoadRelatedDataContainer_OverOneToOneRelation_WithMultipleFound ()
    {
      var endPointID = RelationEndPointID.Create(
          DomainObjectIDs.ContactPersonInTwoOrganizations,
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany");
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainer(_storageProviderManager, endPointID),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "Multiple related DataContainers where found for property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany' of"
                  + " DataContainer 'Person|911957d1-483c-4a8b-aa53-ff07464c58f9|System.Guid'."));
    }

    [Test]
    public void LoadRelatedDataContainers ()
    {
      DataContainerCollection collection = _persistenceService.LoadRelatedDataContainers(
          _storageProviderManager,
          RelationEndPointID.Create(DomainObjectIDs.Customer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.That(collection, Is.Not.Null);
      Assert.AreEqual(2, collection.Count, "DataContainerCollection.Count");
      Assert.IsNotNull(collection[DomainObjectIDs.Order1], "ID of Order with OrdnerNo 1");
      Assert.IsNotNull(collection[DomainObjectIDs.Order2], "ID of Order with OrdnerNo 2");
    }

    [Test]
    public void LoadRelatedDataContainers_ForNonVirtualEndPoint ()
    {
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainers(
          _storageProviderManager,
          RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer")),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point, relation: "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order:Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer->"
                  +"Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders', "
                  + "property: 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. Check your mapping configuration."));
    }

    [Test]
    public void LoadRelatedDataContainers_Empty_ForMandatoryRelation ()
    {
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainers(
          _storageProviderManager,
          RelationEndPointID.Create(DomainObjectIDs.OrderWithoutOrderItems, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems")),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "Collection for mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' on object "
                  + "'Order|f7607cbc-ab34-465c-b282-0531d51f3b04|System.Guid' contains no items."));
    }

    [Test]
    public void LoadRelatedDataContainers_Empty_ForMandatoryRelationWithOptionalOppositeEndPoint ()
    {
      DataContainerCollection orderContainers = _persistenceService.LoadRelatedDataContainers(
          _storageProviderManager,
          RelationEndPointID.Create(DomainObjectIDs.Customer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.That(orderContainers.Count, Is.EqualTo(0));
    }


    [Test]
    public void LoadRelatedDataContainers_ForOneToOneRelation ()
    {
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainers(
          _storageProviderManager,
          RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket")),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "Cannot load multiple related data containers for one-to-one relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket:"
                  + "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order->Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket'."));
    }

    [Test]
    public void LoadRelatedDataContainers_WithInvalidClassID ()
    {
      ObjectID customerID = new ObjectID("Customer", new Guid("{DA658F26-8107-44ce-9DD0-1804503ECCAF}"));

      RelationEndPointID endPointID = RelationEndPointID.Create(customerID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
      Assert.That(
          () => _persistenceService.LoadRelatedDataContainers(_storageProviderManager, endPointID),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of the loaded DataContainer "
                  + "'Order|da658f26-8107-44ce-9dd0-1804503eccaf|System.Guid' refers to ClassID 'Company', but the actual ClassID is 'Customer'."));
    }

    [Test]
    public void SaveInDifferentStorageProviders_WithDefaultBehavior_ThrowsPersistenceException ()
    {
      DataContainer orderContainer = _persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.Order1).LocatedObject;
      DataContainer officialContainer = _persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.Official1).LocatedObject;

      ClientTransactionTestHelper.RegisterDataContainer(TestableClientTransaction, orderContainer);
      ClientTransactionTestHelper.RegisterDataContainer(TestableClientTransaction, officialContainer);

      var dataContainers = new DataContainerCollection { orderContainer, officialContainer };

      SetPropertyValue(orderContainer, typeof(Order), "OrderNumber", 42);
      SetPropertyValue(officialContainer, typeof(Official), "Name", "Zaphod");
      Assert.That(
          () => _persistenceService.Save(_storageProviderManager, dataContainers),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "Save does not support multiple storage providers."));
    }

    [Test]
    public void SaveInDifferentStorageProviders_WithOverriddenCheck_CallsSaveOnBothStorageProviders ()
    {
      var persistenceService = new TestablePersistenceService();
      DataContainer orderContainer = persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.Order1).LocatedObject;
      DataContainer officialContainer1 = persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.Official1).LocatedObject;
      DataContainer officialContainer2 = persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.Official2).LocatedObject;
      ClientTransactionTestHelper.RegisterDataContainer(TestableClientTransaction, orderContainer);
      ClientTransactionTestHelper.RegisterDataContainer(TestableClientTransaction, officialContainer1);
      ClientTransactionTestHelper.RegisterDataContainer(TestableClientTransaction, officialContainer2);
      SetPropertyValue(orderContainer, typeof(Order), "OrderNumber", 42);
      SetPropertyValue(officialContainer1, typeof(Official), "Name", "Zaphod1");
      SetPropertyValue(officialContainer2, typeof(Official), "Name", "Zaphod2");
      var secondStorageProvider = new[] { officialContainer1, officialContainer2 };
      var storageProviderMock = new Mock<StorageProvider>(MockBehavior.Strict, UnitTestStorageProviderDefinition, NullPersistenceExtension.Instance);
      storageProviderMock.Setup(mock => mock.BeginTransaction()).Verifiable();
      storageProviderMock
          .Setup(mock => mock.Save(It.IsNotNull<IReadOnlyCollection<DataContainer>>()))
          .Callback((IReadOnlyCollection<DataContainer> dataContainers) => Assert.That(dataContainers, Is.EquivalentTo(secondStorageProvider)))
          .Verifiable();
      storageProviderMock
          .Setup(mock => mock.UpdateTimestamps(It.IsNotNull<IReadOnlyCollection<DataContainer>>()))
          .Callback((IReadOnlyCollection<DataContainer> dataContainers) => Assert.That(dataContainers, Is.EquivalentTo(secondStorageProvider)))
          .Verifiable();
      storageProviderMock.Setup(mock => mock.Commit()).Verifiable();
      persistenceService.CheckProvidersCompatibleForSaveCallback = providers =>
      {
        Assert.That(
            providers.Select(p => p.GetType()),
            Is.EquivalentTo(new[] { typeof(RdbmsProvider), typeof(UnitTestStorageProviderStub) }));
      };
      var dataContainers = new DataContainerCollection { officialContainer2, orderContainer, officialContainer1 };
      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope(storageProviderMock.Object))
      {
        persistenceService.Save(_storageProviderManager, dataContainers);
      }
      storageProviderMock.Verify();
    }

    [Test]
    public void SaveInDifferentStorageProviders_WithOnlyOnePersistentStorageProvider_CallsSaveOnBothStorageProviders ()
    {
      DataContainer orderContainer = _persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.Order1).LocatedObject;
      DataContainer orderViewModelContainer =  DataContainer.CreateNew(DomainObjectIDs.OrderViewModel1);

      ClientTransactionTestHelper.RegisterDataContainer(TestableClientTransaction, orderContainer);
      ClientTransactionTestHelper.RegisterDataContainer(TestableClientTransaction, orderViewModelContainer);

      SetPropertyValue(orderContainer, typeof(Order), "OrderNumber", 42);
      SetPropertyValue(orderViewModelContainer, typeof(OrderViewModel), "OrderSum", 42);
      SetPropertyValue(orderViewModelContainer, typeof(OrderViewModel), "Object", orderContainer.ID);

      var dataContainers = new DataContainerCollection { orderContainer, orderViewModelContainer };

      var originalOrderTimestamp = orderContainer.Timestamp;
      var originalOrderViewModleTimestamp = orderViewModelContainer.Timestamp;

      _persistenceService.Save(_storageProviderManager, dataContainers);

      Assert.That(orderContainer.Timestamp, Is.Not.EqualTo(originalOrderTimestamp));
      Assert.That(orderViewModelContainer.Timestamp, Is.Not.EqualTo(originalOrderViewModleTimestamp));
    }

    [Test]
    public void Save_WithCommit_CallsExtensionPoints ()
    {
      var persistenceService = new TestablePersistenceService();
      DataContainer officialContainer = persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.Official1).LocatedObject;
      ClientTransactionTestHelper.RegisterDataContainer(TestableClientTransaction, officialContainer);
      SetPropertyValue(officialContainer, typeof(Official), "Name", "Zaphod");
      var secondStorageProvider = new[] { officialContainer };
      var isCommitCalled = false;
      var contextMock = new Mock<IDisposable>(MockBehavior.Strict);
      contextMock.Setup(mock => mock.Dispose()).Callback(() => Assert.That(isCommitCalled, Is.True)).Verifiable();
      var storageProviderMock = new Mock<StorageProvider>(MockBehavior.Strict, UnitTestStorageProviderDefinition, NullPersistenceExtension.Instance);
      persistenceService.BeginTransactionCallback = _ => contextMock.Object;
      storageProviderMock
          .Setup(mock => mock.Save(It.IsNotNull<IReadOnlyCollection<DataContainer>>()))
          .Callback((IReadOnlyCollection<DataContainer> dataContainers) => Assert.That(dataContainers, Is.EquivalentTo(secondStorageProvider)))
          .Verifiable();
      storageProviderMock
          .Setup(mock => mock.UpdateTimestamps(It.IsNotNull<IReadOnlyCollection<DataContainer>>()))
          .Callback((IReadOnlyCollection<DataContainer> dataContainers) => Assert.That(dataContainers, Is.EquivalentTo(secondStorageProvider)))
          .Verifiable();
      persistenceService.CommitTransactionCallback = (_, context) =>
      {
        Assert.That(context, Is.SameAs(contextMock.Object));
        isCommitCalled = true;
      };
      persistenceService.CheckProvidersCompatibleForSaveCallback = providers =>
      {
        Assert.That(
            providers.Select(p => p.GetType()),
            Is.EquivalentTo(new[] { typeof(UnitTestStorageProviderStub) }));
      };
      var dataContainers = new DataContainerCollection { officialContainer };
      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope(storageProviderMock.Object))
      {
        persistenceService.Save(_storageProviderManager, dataContainers);
      }
      contextMock.Verify();
      storageProviderMock.Verify();
    }

    [Test]
    public void Save_WithRollback_CallsExtensionPoints ()
    {
      var persistenceService = new TestablePersistenceService();
      DataContainer officialContainer = persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.Official1).LocatedObject;
      ClientTransactionTestHelper.RegisterDataContainer(TestableClientTransaction, officialContainer);
      SetPropertyValue(officialContainer, typeof(Official), "Name", "Zaphod");
      var exception = new Exception();
      var isRollbackCalled = false;
      var contextMock = new Mock<IDisposable>(MockBehavior.Strict);
      contextMock.Setup(mock => mock.Dispose()).Callback(() => Assert.That(isRollbackCalled, Is.True)).Verifiable();
      var storageProviderMock = new Mock<StorageProvider>(MockBehavior.Strict, UnitTestStorageProviderDefinition, NullPersistenceExtension.Instance);
      persistenceService.BeginTransactionCallback = _ => contextMock.Object;
      storageProviderMock
          .Setup(mock => mock.Save(It.IsNotNull<IReadOnlyCollection<DataContainer>>()))
          .Throws(exception);
      persistenceService.RollbackTransactionCallback = (_, context) =>
      {
        Assert.That(context, Is.SameAs(contextMock.Object));
        isRollbackCalled = true;
      };
      persistenceService.CheckProvidersCompatibleForSaveCallback = providers =>
      {
        Assert.That(
            providers.Select(p => p.GetType()),
            Is.EquivalentTo(new[] { typeof(UnitTestStorageProviderStub) }));
      };
      var dataContainers = new DataContainerCollection { officialContainer };
      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope(storageProviderMock.Object))
      {
        Assert.That(() => persistenceService.Save(_storageProviderManager, dataContainers), Throws.Exception.SameAs(exception));
      }
      contextMock.Verify();
      storageProviderMock.Verify();
    }

    [Test]
    public void Save_DeletedDataContainersAreIgnoredForUpdateTimestamps ()
    {
      var dataContainer = _persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.ClassWithAllDataTypes1).LocatedObject;
      Assert.That(dataContainer, Is.Not.Null);
      dataContainer.Delete();

      var timestampBefore = dataContainer.Timestamp;
      _persistenceService.Save(_storageProviderManager, new DataContainerCollection { dataContainer });
      Assert.That(dataContainer.Timestamp, Is.SameAs(timestampBefore));

      Assert.That(() => _persistenceService.LoadDataContainer(_storageProviderManager, DomainObjectIDs.ClassWithAllDataTypes1).LocatedObject, Is.Null);
    }

    [Test]
    public void CreateNewObjectID ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.GetTypeDefinition(typeof(Order));
      ObjectID id1 = _persistenceService.CreateNewObjectID(_storageProviderManager, orderClass);
      Assert.That(id1, Is.Not.Null);
      ObjectID id2 = _persistenceService.CreateNewObjectID(_storageProviderManager, orderClass);
      Assert.That(id2, Is.Not.Null);
      Assert.That(id2, Is.Not.EqualTo(id1));
    }

    [Test]
    public void CreateNewDataContainer ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.GetTypeDefinition(typeof(Order));
      DataContainer container = CreateDataContainer(orderClass);

      Assert.That(container, Is.Not.Null);
      Assert.That(container.State.IsNew, Is.True);
      Assert.That(container.ID, Is.Not.Null);
    }

    private DataContainer CreateDataContainer (ClassDefinition classDefinition)
    {
      return DataContainer.CreateNew(_persistenceService.CreateNewObjectID(_storageProviderManager, classDefinition));
    }

  }
}
