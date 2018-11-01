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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class PersistenceManagerTest : ClientTransactionBaseTest
  {
    private PersistenceManager _persistenceManager;

    private ObjectID _invalidOrderID1;
    private ObjectID _invalidOrderID2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _persistenceManager = new PersistenceManager (NullPersistenceExtension.Instance);

      var guid1 = new Guid ("11111111111111111111111111111111");
      var guid2 = new Guid ("22222222222222222222222222222222");
      _invalidOrderID1 = new ObjectID(typeof (Order), guid1);
      _invalidOrderID2 = new ObjectID(typeof (Order), guid2);
    }

    public override void TearDown ()
    {
      _persistenceManager.Dispose ();
      base.TearDown ();
    }

    [Test]
    public void Initialize ()
    {
      var persistenceTracer = MockRepository.GenerateStub<IPersistenceExtension>();
      using (var persistenceManager = new PersistenceManager (persistenceTracer))
      {
        Assert.That (persistenceManager.StorageProviderManager, Is.Not.Null);

        using (var storageProvider = persistenceManager.StorageProviderManager.GetMandatory (c_testDomainProviderID))
        {
          Assert.That (storageProvider.PersistenceExtension, Is.SameAs (persistenceTracer));
        }
      }
    }

    [Test]
    public void LoadDataContainer ()
    {
      var result = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1);
      Assert.That (result.ObjectID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (result.LocatedObject.ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void LoadDataContainer_WithNonExistingValue ()
    {
      var result = _persistenceManager.LoadDataContainer (_invalidOrderID1);
      Assert.That (result.ObjectID, Is.EqualTo (_invalidOrderID1));
      Assert.That (result.LocatedObject, Is.Null);
    }

    [Test]
    public void LoadDataContainers ()
    {
      Assert.AreNotEqual (DomainObjectIDs.Order1.StorageProviderDefinition.Name, DomainObjectIDs.Official1, "Different storage providers");

      var mockRepository = new MockRepository ();
      var storageProviderMock = mockRepository.StrictMock<StorageProvider> (
          UnitTestStorageProviderDefinition, 
          NullPersistenceExtension.Instance);

      var officialDC1 = DataContainer.CreateNew (DomainObjectIDs.Official1);
      var officialDC2 = DataContainer.CreateNew (DomainObjectIDs.Official2);

      var officialDCs = new List<ObjectLookupResult<DataContainer>>();
      officialDCs.Add (new ObjectLookupResult<DataContainer>(DomainObjectIDs.Official1, officialDC1));
      officialDCs.Add (new ObjectLookupResult<DataContainer>(DomainObjectIDs.Official2, officialDC2));

      storageProviderMock
          .Expect (mock => mock.LoadDataContainers (Arg<IEnumerable<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Official1, DomainObjectIDs.Official2 })))
          .Return (officialDCs);

      mockRepository.ReplayAll();

      ObjectLookupResult<DataContainer>[] actualResults;
      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope (storageProviderMock))
      {
        actualResults = _persistenceManager.LoadDataContainers (
            new[]
            {
                DomainObjectIDs.Order1, DomainObjectIDs.Official1, DomainObjectIDs.Order3,
                DomainObjectIDs.Official2
            }).ToArray();
      }

      mockRepository.VerifyAll ();

      Assert.That (actualResults, Has.Length.EqualTo (4));
      Assert.That (actualResults[0].ObjectID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (actualResults[0].LocatedObject.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (actualResults[1].ObjectID, Is.EqualTo (officialDC1.ID));
      Assert.That (actualResults[1].LocatedObject, Is.SameAs (officialDC1));
      Assert.That (actualResults[2].ObjectID, Is.EqualTo (DomainObjectIDs.Order3));
      Assert.That (actualResults[2].LocatedObject.ID, Is.EqualTo (DomainObjectIDs.Order3));
      Assert.That (actualResults[3].ObjectID, Is.EqualTo (officialDC2.ID));
      Assert.That (actualResults[3].LocatedObject, Is.SameAs (officialDC2));
    }

    [Test]
    public void LoadDataContainers_DuplicatesAreReplacedBySingleDataContainer ()
    {
      Assert.AreNotEqual (DomainObjectIDs.Order1.StorageProviderDefinition.Name, DomainObjectIDs.Official1, "Different storage providers");

      var mockRepository = new MockRepository ();
      var storageProviderMock = mockRepository.StrictMock<StorageProvider> (
          UnitTestStorageProviderDefinition,
          NullPersistenceExtension.Instance);

      var officialDC1a = DataContainer.CreateNew (DomainObjectIDs.Official1);
      var officialDC1b = DataContainer.CreateNew (DomainObjectIDs.Official1);

      var officialResults = new List<ObjectLookupResult<DataContainer>> ();
      officialResults.Add (new ObjectLookupResult<DataContainer> (DomainObjectIDs.Official1, officialDC1a));
      officialResults.Add (new ObjectLookupResult<DataContainer> (DomainObjectIDs.Official1, officialDC1b));

      storageProviderMock
          .Expect (mock => mock.LoadDataContainers (Arg<IEnumerable<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Official1, DomainObjectIDs.Official1 })))
          .Return (officialResults);

      mockRepository.ReplayAll ();

      ObjectLookupResult<DataContainer>[] actualResults;
      using (UnitTestStorageProviderStub.EnterMockStorageProviderScope (storageProviderMock))
      {
        actualResults = _persistenceManager.LoadDataContainers (
            new[]
            {
                DomainObjectIDs.Official1, DomainObjectIDs.Official1
            }).ToArray ();
      }

      mockRepository.VerifyAll ();

      Assert.That (actualResults, Has.Length.EqualTo (2));
      Assert.That (actualResults[0].ObjectID, Is.EqualTo (DomainObjectIDs.Official1));
      Assert.That (actualResults[0].LocatedObject, Is.SameAs (officialDC1a));
      Assert.That (actualResults[1].ObjectID, Is.EqualTo (DomainObjectIDs.Official1));
      Assert.That (actualResults[1].LocatedObject, Is.SameAs (officialDC1a));
    }

    [Test]
    public void LoadDataContainers_NotFound ()
    {
      var objectIds = new[] { _invalidOrderID1, _invalidOrderID2, DomainObjectIDs.Order1 };
      
      var dataContainers = _persistenceManager.LoadDataContainers (objectIds).ToArray();

      Assert.That (dataContainers.Length, Is.EqualTo (3));
      Assert.That (dataContainers[0].ObjectID, Is.EqualTo (_invalidOrderID1));
      Assert.That (dataContainers[0].LocatedObject, Is.Null);
      Assert.That (dataContainers[1].ObjectID, Is.EqualTo (_invalidOrderID2));
      Assert.That (dataContainers[1].LocatedObject, Is.Null);
      Assert.That (dataContainers[2].ObjectID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (dataContainers[2].LocatedObject.ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void LoadRelatedDataContainer ()
    {
      DataContainer orderTicketContainer = _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create (DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));

      var checker = new DataContainerChecker();
      checker.Check (TestDataContainerObjectMother.CreateOrderTicket1DataContainer(), orderTicketContainer);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "LoadRelatedDataContainer can only be used with virtual end points.\r\nParameter name: relationEndPointID")]
    public void LoadRelatedDataContainer_NonVirtualEndPoint ()
    {
      var relationEndPointID = RelationEndPointID.Create (DomainObjectIDs.OrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
      _persistenceManager.LoadRelatedDataContainer (relationEndPointID);
    }

    [Test]
    public void LoadRelatedDataContainer_OptionalNullID ()
    {
      var id = new ObjectID("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create (id, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional"));

      Assert.That (relatedDataContainer, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional' on "
        + "object 'ClassWithGuidKey|672c8754-c617-4b7a-890c-bfef8ac86564|System.Guid' contains no item.")]
    public void LoadRelatedDataContainer_NonOptionalNullID ()
    {
      var id = new ObjectID("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create(id, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo' on object "
        + "'Partner|a65b123a-6e17-498e-a28e-946217c0ae30|System.Guid' contains no item.")]
    public void LoadRelatedDataContainer_NonOptionalNullID_WithInheritance ()
    {
      _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create (DomainObjectIDs.PartnerWithoutCeo, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo"));
    }

    [Test]
    public void LoadRelatedDataContainer_OverValidMandatoryRelation ()
    {
      var id = new ObjectID("ClassWithGuidKey", new Guid ("{D0A1BDDE-B13F-47c1-98BD-EBAE21189B01}"));

      DataContainer relatedContainer = _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create(id, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional"));

      ObjectID expectedID = new ObjectID("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      Assert.That (relatedContainer, Is.Not.Null);
      Assert.That (relatedContainer.ID, Is.EqualTo (expectedID));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional' on object "
        + "'ClassWithGuidKey|672c8754-c617-4b7a-890c-bfef8ac86564|System.Guid' contains no item.")]
    public void LoadRelatedDataContainer_OverInvalidNonOptionalRelation ()
    {
      ObjectID id = new ObjectID("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create (id, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Cannot load a single related data container for one-to-many relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem:"
        + "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order->Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems'.")]
    public void LoadRelatedDataContainer_ForOneToManyRelation ()
    {
      _persistenceManager.LoadRelatedDataContainer (
          RelationEndPointID.Create (DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company' of the loaded DataContainer "
        + "'Ceo|c3db20d6-138e-4ced-8576-e81bb4b7961f|System.Guid' refers to ClassID 'Customer', but the actual ClassID is 'Company'.")]
    public void LoadRelatedDataContainer_WithInvalidClassIDOverVirtualEndPoint ()
    {
      ObjectID companyID = new ObjectID("Company", new Guid ("{C3DB20D6-138E-4ced-8576-E81BB4B7961F}"));

      RelationEndPointID endPointID = RelationEndPointID.Create (companyID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");

      _persistenceManager.LoadRelatedDataContainer (endPointID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException),
        ExpectedMessage =
            "Multiple related DataContainers where found for property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany' of"
            + " DataContainer 'Person|911957d1-483c-4a8b-aa53-ff07464c58f9|System.Guid'.")]
    public void LoadRelatedDataContainer_OverOneToOneRelation_WithMultipleFound ()
    {
      var endPointID = RelationEndPointID.Create (
          DomainObjectIDs.ContactPersonInTwoOrganizations,
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany");

      _persistenceManager.LoadRelatedDataContainer (endPointID);
    }

    [Test]
    public void LoadRelatedDataContainers ()
    {
      DataContainerCollection collection = _persistenceManager.LoadRelatedDataContainers (
          RelationEndPointID.Create (DomainObjectIDs.Customer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.That (collection, Is.Not.Null);
      Assert.AreEqual (2, collection.Count, "DataContainerCollection.Count");
      Assert.IsNotNull (collection[DomainObjectIDs.Order1], "ID of Order with OrdnerNo 1");
      Assert.IsNotNull (collection[DomainObjectIDs.Order2], "ID of Order with OrdnerNo 2");
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point, relation: "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order:Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer->"
        +"Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders', "
        + "property: 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer'. Check your mapping configuration.")]
    public void LoadRelatedDataContainers_ForNonVirtualEndPoint ()
    {
      _persistenceManager.LoadRelatedDataContainers (
          RelationEndPointID.Create (DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Collection for mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' on object "
        + "'Order|f7607cbc-ab34-465c-b282-0531d51f3b04|System.Guid' contains no items.")]
    public void LoadRelatedDataContainers_Empty_ForMandatoryRelation ()
    {
      _persistenceManager.LoadRelatedDataContainers (
          RelationEndPointID.Create (DomainObjectIDs.OrderWithoutOrderItems, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
    }

    [Test]
    public void LoadRelatedDataContainers_Empty_ForMandatoryRelationWithOptionalOppositeEndPoint ()
    {
      DataContainerCollection orderContainers = _persistenceManager.LoadRelatedDataContainers (
          RelationEndPointID.Create (DomainObjectIDs.Customer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.That (orderContainers.Count, Is.EqualTo (0));
    }


    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "Cannot load multiple related data containers for one-to-one relation 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket:"
        + "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order->Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket'.")]
    public void LoadRelatedDataContainers_ForOneToOneRelation ()
    {
      _persistenceManager.LoadRelatedDataContainers (
          RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' of the loaded DataContainer "
        + "'Order|da658f26-8107-44ce-9dd0-1804503eccaf|System.Guid' refers to ClassID 'Company', but the actual ClassID is 'Customer'.")]
    public void LoadRelatedDataContainers_WithInvalidClassID ()
    {
      ObjectID customerID = new ObjectID("Customer", new Guid ("{DA658F26-8107-44ce-9DD0-1804503ECCAF}"));

      RelationEndPointID endPointID = RelationEndPointID.Create (customerID, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

      _persistenceManager.LoadRelatedDataContainers (endPointID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = "Save does not support multiple storage providers.")]
    public void SaveInDifferentStorageProviders ()
    {
      DataContainer orderContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Order1).LocatedObject;
      DataContainer officialContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.Official1).LocatedObject;

      ClientTransactionTestHelper.RegisterDataContainer (TestableClientTransaction, orderContainer);
      ClientTransactionTestHelper.RegisterDataContainer (TestableClientTransaction, officialContainer);

      var dataContainers = new DataContainerCollection { orderContainer, officialContainer };

      SetPropertyValue (orderContainer, typeof (Order), "OrderNumber", 42);
      SetPropertyValue (officialContainer, typeof (Official), "Name", "Zaphod");

      _persistenceManager.Save (dataContainers);
    }

    [Test]
    public void Save_DeletedDataContainersAreIgnoredForUpdateTimestamps ()
    {
      SetDatabaseModifyable();

      var dataContainer = _persistenceManager.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1).LocatedObject;
      Assert.That (dataContainer, Is.Not.Null);
      dataContainer.Delete ();

      var timestampBefore = dataContainer.Timestamp;
      _persistenceManager.Save (new DataContainerCollection { dataContainer });
      Assert.That (dataContainer.Timestamp, Is.SameAs (timestampBefore));

      Assert.That (() => _persistenceManager.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1).LocatedObject, Is.Null);
    }

    [Test]
    public void CreateNewObjectID ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.GetTypeDefinition (typeof (Order));
      ObjectID id1 = _persistenceManager.CreateNewObjectID (orderClass);
      Assert.That (id1, Is.Not.Null);
      ObjectID id2 = _persistenceManager.CreateNewObjectID (orderClass);
      Assert.That (id2, Is.Not.Null);
      Assert.That (id2, Is.Not.EqualTo (id1));
    }

    [Test]
    public void CreateNewDataContainer ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.GetTypeDefinition (typeof (Order));
      DataContainer container = CreateDataContainer (orderClass);

      Assert.That (container, Is.Not.Null);
      Assert.That (container.State, Is.EqualTo (StateType.New));
      Assert.That (container.ID, Is.Not.Null);
    }

    private DataContainer CreateDataContainer (ClassDefinition classDefinition)
    {
      return DataContainer.CreateNew (_persistenceManager.CreateNewObjectID (classDefinition));
    }

  }
}