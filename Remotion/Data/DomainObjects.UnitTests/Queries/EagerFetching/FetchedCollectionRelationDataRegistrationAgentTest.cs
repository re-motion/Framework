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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.EagerFetching
{
  [TestFixture]
  public class FetchedCollectionRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private Mock<IVirtualEndPointProvider> _virtualEndPointProviderMock;

    private FetchedCollectionRelationDataRegistrationAgent _agent;

    private Customer _originatingCustomer1;
    private Customer _originatingCustomer2;

    private Mock<ILoadedObjectData> _originatingCustomerData1;
    private Mock<ILoadedObjectData> _originatingCustomerData2;

    private Order _fetchedOrder1;
    private Order _fetchedOrder2;
    private Order _fetchedOrder3;

    private LoadedObjectDataWithDataSourceData _fetchedOrderData1;
    private LoadedObjectDataWithDataSourceData _fetchedOrderData2;
    private LoadedObjectDataWithDataSourceData _fetchedOrderData3;

    public override void SetUp ()
    {
      base.SetUp();

      _virtualEndPointProviderMock = new Mock<IVirtualEndPointProvider>(MockBehavior.Strict);

      _agent = new FetchedCollectionRelationDataRegistrationAgent(_virtualEndPointProviderMock.Object);

      _originatingCustomer1 = DomainObjectMother.CreateFakeObject<Customer>(DomainObjectIDs.Customer1);
      _originatingCustomer2 = DomainObjectMother.CreateFakeObject<Customer>(DomainObjectIDs.Customer2);

      _originatingCustomerData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(_originatingCustomer1);
      _originatingCustomerData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(_originatingCustomer2);

      _fetchedOrder1 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order1);
      _fetchedOrder2 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order3);
      _fetchedOrder3 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order4);

      _fetchedOrderData1 = CreateFetchedOrderData(_fetchedOrder1, _originatingCustomer1.ID);
      _fetchedOrderData2 = CreateFetchedOrderData(_fetchedOrder2, _originatingCustomer2.ID);
      _fetchedOrderData3 = CreateFetchedOrderData(_fetchedOrder3, _originatingCustomer1.ID);
    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");

      var collectionEndPointMock1 = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      ExpectGetEndPoint(_originatingCustomer1.ID, endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock1, false);
      collectionEndPointMock1.Setup(mock => mock.MarkDataComplete(new[] { _fetchedOrder1, _fetchedOrder3 })).Verifiable();

      var collectionEndPointMock2 = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      ExpectGetEndPoint(_originatingCustomer2.ID, endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock2, false);
      collectionEndPointMock2.Setup(mock => mock.MarkDataComplete(new[] { _fetchedOrder2 })).Verifiable();

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { _originatingCustomerData1.Object, _originatingCustomerData2.Object },
          new[] { _fetchedOrderData1, _fetchedOrderData2, _fetchedOrderData3 });

      _virtualEndPointProviderMock.Verify();
      collectionEndPointMock1.Verify();
      collectionEndPointMock2.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNonOriginatingObject ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");

      var collectionEndPointMock1 = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      ExpectGetEndPoint(_originatingCustomer1.ID, endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock1, false);
      collectionEndPointMock1.Setup(mock => mock.MarkDataComplete(new[] { _fetchedOrder1, _fetchedOrder3 })).Verifiable();

      var collectionEndPointMock2 = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      ExpectGetEndPoint(_originatingCustomer2.ID, endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock2, false);
      collectionEndPointMock2.Setup(mock => mock.MarkDataComplete(new[] { _fetchedOrder2 })).Verifiable();

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { _originatingCustomerData1.Object, _originatingCustomerData2.Object },
          new[] { _fetchedOrderData1, _fetchedOrderData2, _fetchedOrderData3 });

      _virtualEndPointProviderMock.Verify();
      collectionEndPointMock1.Verify();
      collectionEndPointMock2.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_MandatoryEndPointWithoutRelatedObjects_Throws ()
    {
      var orderItemsEndPointDefinition = GetEndPointDefinition(typeof(Order), "OrderItems");
      Assert.That(orderItemsEndPointDefinition.IsMandatory, Is.True);

      var originatingOrderData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(DomainObjectIDs.Order1);
      Assert.That(
          () => _agent.GroupAndRegisterRelatedObjects(
          orderItemsEndPointDefinition,
          new[] { originatingOrderData.Object },
          new LoadedObjectDataWithDataSourceData[0]),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The fetched mandatory collection property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems' on object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' contains no items."));
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_NonMandatoryEndPointWithoutRelatedObjects_RegistersEmptyCollection ()
    {
      var originatingCustomerData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(DomainObjectIDs.Customer1);
      var ordersEndPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");
      Assert.That(ordersEndPointDefinition.IsMandatory, Is.False);

      var collectionEndPointMock = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      ExpectGetEndPoint(originatingCustomerData.Object.ObjectID, ordersEndPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock, false);

      collectionEndPointMock.Setup(mock => mock.MarkDataComplete(new DomainObject[0])).Verifiable();

      _agent.GroupAndRegisterRelatedObjects(
          ordersEndPointDefinition,
          new[] { originatingCustomerData.Object },
          new LoadedObjectDataWithDataSourceData[0]);

      collectionEndPointMock.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullOriginalObject ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");

      _agent.GroupAndRegisterRelatedObjects(endPointDefinition, new[] { new NullLoadedObjectData() }, new LoadedObjectDataWithDataSourceData[0]);

      _virtualEndPointProviderMock.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullRelatedObject ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");

      var collectionEndPointMock = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      ExpectGetEndPoint(_originatingCustomer1.ID, endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock, false);
      collectionEndPointMock.Setup(mock => mock.MarkDataComplete(new DomainObject[0])).Verifiable();

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { _originatingCustomerData1.Object },
          new[] { LoadedObjectDataObjectMother.CreateNullLoadedObjectDataWithDataSourceData() });

      _virtualEndPointProviderMock.Verify();
      collectionEndPointMock.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNull ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");

      var fetchedOrderItemDataPointingToNull = CreateFetchedOrderData(_fetchedOrder1, null);

      var collectionEndPointMock = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      ExpectGetEndPoint(_originatingCustomer1.ID, endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock, false);
      collectionEndPointMock.Setup(mock => mock.MarkDataComplete(new DomainObject[0])).Verifiable();

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { _originatingCustomerData1.Object },
          new[] { fetchedOrderItemDataPointingToNull });

      _virtualEndPointProviderMock.Verify();
      collectionEndPointMock.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithEndPointAlreadyComplete ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");

      var collectionEndPointMock1 = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      var collectionEndPointMock2 = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      ExpectGetEndPoint(_originatingCustomer1.ID, endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock1, true);

      ExpectGetEndPoint(_originatingCustomer2.ID, endPointDefinition, _virtualEndPointProviderMock, collectionEndPointMock2, false);
      collectionEndPointMock2.Setup(mock => mock.MarkDataComplete(new DomainObject[] { _fetchedOrder2 })).Verifiable();

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { _originatingCustomerData1.Object, _originatingCustomerData2.Object },
          new[] { _fetchedOrderData1, _fetchedOrderData2, _fetchedOrderData3 });

      _virtualEndPointProviderMock.Verify();
      collectionEndPointMock1.Verify(mock => mock.MarkDataComplete(It.IsAny<DomainObject[]>()), Times.Never());
      collectionEndPointMock2.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_PropertyOnBaseType ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");

      var originatingSpecialCustomer = DomainObjectMother.CreateFakeObject<SpecialCustomer>();
      var originatingSpecialCustomerData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(originatingSpecialCustomer);

      var fetchedOrder = DomainObjectMother.CreateFakeObject<Order>();
      var fetchedOrderData = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(fetchedOrder);
      fetchedOrderData.DataSourceData.SetValue(GetPropertyDefinition(typeof(Order), "Customer"), originatingSpecialCustomer.ID);

      var endPointMock = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      ExpectGetEndPoint(originatingSpecialCustomer.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock, false);
      endPointMock.Setup(mock => mock.MarkDataComplete(new DomainObject[] { fetchedOrder })).Verifiable();

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { originatingSpecialCustomerData.Object },
          new[] { fetchedOrderData });

      _virtualEndPointProviderMock.Verify();
      endPointMock.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_PropertyOnDerivedType ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Folder), "FileSystemItems");

      var originatingFolder = DomainObjectMother.CreateFakeObject<Folder>();
      var originatingFolderData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(originatingFolder);

      var originatingFile = DomainObjectMother.CreateFakeObject<File>();
      var originatingFileData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(originatingFile);

      var fetchedFile = DomainObjectMother.CreateFakeObject<File>();
      var fetchedFileData = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(fetchedFile);
      fetchedFileData.DataSourceData.SetValue(GetPropertyDefinition(typeof(FileSystemItem), "ParentFolder"), originatingFolder.ID);

      var endPointMock = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      ExpectGetEndPoint(originatingFolder.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock, false);
      endPointMock.Setup(mock => mock.MarkDataComplete(new DomainObject[] { fetchedFile })).Verifiable();

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { originatingFolderData.Object, originatingFileData.Object },
          new[] { fetchedFileData });

      _virtualEndPointProviderMock.Verify();
      endPointMock.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_OriginatingObjectOfInvalidType ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");

      Assert.That(
          () => _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(DomainObjectIDs.OrderItem1).Object },
          new LoadedObjectDataWithDataSourceData[0]),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot register relation end-point "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' for domain object "
                  + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid'. The end-point belongs to an object of class 'Customer' but the domain object "
                  + "has class 'OrderItem'."));
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_RelatedObjectOfInvalidType ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Customer), "Orders");

      Assert.That(
          () => _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { _originatingCustomerData1.Object },
          new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(DomainObjectIDs.OrderItem2) }),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot associate object 'OrderItem|ad620a11-4bc4-4791-bcf4-a0770a08c5b0|System.Guid' with the relation end-point "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders'. An object of type "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' was expected."));
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WrongCardinality ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");

      Assert.That(
          () =>
          _agent.GroupAndRegisterRelatedObjects(
              endPointDefinition,
              new[] { _originatingCustomerData1.Object },
              new[] { _fetchedOrderData1 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only collection-valued relations can be handled by this registration agent.", "relationEndPointDefinition"));
    }


    private LoadedObjectDataWithDataSourceData CreateFetchedOrderData (Order fetchedObject, ObjectID orderID)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(fetchedObject.ID, "Customer");
      var loadedObjectDataStub = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(fetchedObject);
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer(endPointID, orderID);
      return new LoadedObjectDataWithDataSourceData(loadedObjectDataStub.Object, dataContainer);
    }

    private void ExpectGetEndPoint (
        ObjectID objectID,
        IRelationEndPointDefinition endPointDefinition,
        Mock<IVirtualEndPointProvider> relationEndPointProviderMock,
        Mock<ICollectionEndPoint<ICollectionEndPointData>> collectionEndPointMock,
        bool expectedIsDataComplete)
    {
      var relationEndPointID = RelationEndPointID.Create(objectID, endPointDefinition);
      relationEndPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(relationEndPointID))
          .Returns(collectionEndPointMock.Object)
          .Verifiable();
      collectionEndPointMock
          .Setup(mock => mock.IsDataComplete)
          .Returns(expectedIsDataComplete)
          .Verifiable();
    }
  }
}
