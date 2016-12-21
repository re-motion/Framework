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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.EagerFetching
{
  [TestFixture]
  public class FetchedVirtualObjectRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private ILoadedDataContainerProvider _loadedDataContainerProviderStub;
    private IVirtualEndPointProvider _virtualEndPointProviderMock;

    private FetchedVirtualObjectRelationDataRegistrationAgent _agent;

    private Employee _originatingEmployee1;
    private Employee _originatingEmployee2;

    private ILoadedObjectData _originatingEmployeeData1;
    private ILoadedObjectData _originatingEmployeeData2;

    private Computer _fetchedComputer1;
    private Computer _fetchedComputer2;
    private Computer _fetchedComputer3;

    private LoadedObjectDataWithDataSourceData _fetchedComputerData1;
    private LoadedObjectDataWithDataSourceData _fetchedComputerData2;
    private LoadedObjectDataWithDataSourceData _fetchedComputerData3;

    public override void SetUp ()
    {
      base.SetUp ();

      _loadedDataContainerProviderStub = MockRepository.GenerateStub<ILoadedDataContainerProvider> ();
      _virtualEndPointProviderMock = MockRepository.GenerateStrictMock<IVirtualEndPointProvider> ();

      _agent = new FetchedVirtualObjectRelationDataRegistrationAgent (_virtualEndPointProviderMock);

      _originatingEmployee1 = DomainObjectMother.CreateFakeObject<Employee> (DomainObjectIDs.Employee1);
      _originatingEmployee2 = DomainObjectMother.CreateFakeObject<Employee> (DomainObjectIDs.Employee2);

      _originatingEmployeeData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingEmployee1);
      _originatingEmployeeData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingEmployee2);

      _fetchedComputer1 = DomainObjectMother.CreateFakeObject<Computer> (DomainObjectIDs.Computer1);
      _fetchedComputer2 = DomainObjectMother.CreateFakeObject<Computer> (DomainObjectIDs.Computer2);
      _fetchedComputer3 = DomainObjectMother.CreateFakeObject<Computer> (DomainObjectIDs.Computer3);

      _fetchedComputerData1 = CreateFetchedComputerData (_fetchedComputer1, _originatingEmployee1.ID);
      _fetchedComputerData2 = CreateFetchedComputerData (_fetchedComputer2, _originatingEmployee2.ID);
      _fetchedComputerData3 = CreateFetchedComputerData (_fetchedComputer3, DomainObjectIDs.Employee3);
    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Employee), "Computer");

      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedComputer1.ID)).Return (_fetchedComputerData1.DataSourceData);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedComputer2.ID)).Return (_fetchedComputerData2.DataSourceData);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedComputer3.ID)).Return (_fetchedComputerData3.DataSourceData);

      var endPointMock1 = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(_originatingEmployee1.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock1, false);
      endPointMock1.Expect (mock => mock.MarkDataComplete (_fetchedComputer1));

      var endPointMock2 = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint (_originatingEmployee2.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock2, false);
      endPointMock2.Expect (mock => mock.MarkDataComplete (_fetchedComputer2));

      _virtualEndPointProviderMock.Replay ();
      endPointMock1.Replay ();
      endPointMock2.Replay ();

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { _originatingEmployeeData1, _originatingEmployeeData2 },
          new[] { _fetchedComputerData1, _fetchedComputerData2, _fetchedComputerData3 });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock1.VerifyAllExpectations ();
      endPointMock2.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithOriginatingWithoutRelated ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Employee), "Computer");

      var endPointMock1 = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint (_originatingEmployee1.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock1, false);
      endPointMock1.Expect (mock => mock.MarkDataComplete (null));

      _virtualEndPointProviderMock.Replay ();
      endPointMock1.Replay ();

      _agent.GroupAndRegisterRelatedObjects (endPointDefinition, new[] { _originatingEmployeeData1 }, new LoadedObjectDataWithDataSourceData[0]);

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock1.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullOriginalObject ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Employee), "Computer");

      _virtualEndPointProviderMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          endPointDefinition,
          new[] { new NullLoadedObjectData() },
          new LoadedObjectDataWithDataSourceData[0]);

      _virtualEndPointProviderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullRelatedObject_AndNonMandatoryRelation ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Employee), "Computer");

      Assert.That (endPointDefinition.IsMandatory, Is.False);

      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(_originatingEmployee1.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock, false);
      endPointMock.Expect (mock => mock.MarkDataComplete (null));

      _virtualEndPointProviderMock.Replay ();
      endPointMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (
          endPointDefinition, 
          new[] { _originatingEmployeeData1 }, 
          new[] { LoadedObjectDataObjectMother.CreateNullLoadedObjectDataWithDataSourceData() });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_PropertyOnBaseType ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Company), "Ceo");

      var originatingSupplier = DomainObjectMother.CreateFakeObject<Supplier>();
      var originatingSupplierData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (originatingSupplier);

      var fetchedCeo = DomainObjectMother.CreateFakeObject<Ceo>();
      var fetchedCeoData = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (fetchedCeo);
      fetchedCeoData.DataSourceData.SetValue (GetPropertyDefinition (typeof (Ceo), "Company"), originatingSupplier.ID);

      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (fetchedCeo.ID)).Return (fetchedCeoData.DataSourceData);

      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(originatingSupplier.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock, false);
      endPointMock.Expect (mock => mock.MarkDataComplete (fetchedCeo));

      _virtualEndPointProviderMock.Replay ();
      endPointMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { originatingSupplierData },
          new[] { fetchedCeoData });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_PropertyOnDerivedType ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Customer), "ContactPerson");

      var originatingCustomer = DomainObjectMother.CreateFakeObject<Customer>();
      var originatingCustomerData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (originatingCustomer);

      var originatingCompany = DomainObjectMother.CreateFakeObject<Company>();
      var originatingCompanyData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (originatingCompany);
      
      var originatingSupplier = DomainObjectMother.CreateFakeObject<Supplier>();
      var originatingSupplierData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (originatingSupplier);

      var fetchedPerson = DomainObjectMother.CreateFakeObject<Person>();
      var fetchedPersonData = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (fetchedPerson);
      fetchedPersonData.DataSourceData.SetValue (GetPropertyDefinition (typeof (Person), "AssociatedCustomerCompany"), originatingCustomer.ID);

      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (fetchedPerson.ID)).Return (fetchedPersonData.DataSourceData);

      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(originatingCustomer.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock, false);
      endPointMock.Expect (mock => mock.MarkDataComplete (fetchedPerson));

      _virtualEndPointProviderMock.Replay ();
      endPointMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { originatingCustomerData, originatingCompanyData, originatingSupplierData },
          new[] { fetchedPersonData });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The fetched mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' on object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' contains no related object.")]
    public void GroupAndRegisterRelatedObjects_WithNullRelatedObject_AndMandatoryRelation ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
      Assert.That (endPointDefinition.IsMandatory, Is.True);
      var originatingOrderData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Order1);

      _agent.GroupAndRegisterRelatedObjects (
          endPointDefinition,
          new[] { originatingOrderData },
          new[] { LoadedObjectDataObjectMother.CreateNullLoadedObjectDataWithDataSourceData () });
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNull ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Employee), "Computer");

      var fetchedComputerDataPointingToNull = CreateFetchedComputerData (_fetchedComputer1, null);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (fetchedComputerDataPointingToNull.LoadedObjectData.ObjectID)).Return (fetchedComputerDataPointingToNull.DataSourceData);

      var endPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(_originatingEmployee1.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock, false);
      endPointMock.Expect (mock => mock.MarkDataComplete (null));

      _virtualEndPointProviderMock.Replay ();
      endPointMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (endPointDefinition, new[] { _originatingEmployeeData1 }, new[] { fetchedComputerDataPointingToNull });

      _virtualEndPointProviderMock.VerifyAllExpectations ();
      endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithEndPointAlreadyComplete ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Employee), "Computer");

      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedComputer1.ID)).Return (_fetchedComputerData1.DataSourceData);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedComputer2.ID)).Return (_fetchedComputerData2.DataSourceData);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedComputer3.ID)).Return (_fetchedComputerData3.DataSourceData);

      var endPointMock1 = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint(_originatingEmployee1.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock1, true);

      var endPointMock2 = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      ExpectGetEndPoint (_originatingEmployee2.ID, endPointDefinition, _virtualEndPointProviderMock, endPointMock2, false);
      endPointMock2.Expect (mock => mock.MarkDataComplete (_fetchedComputer2));

      _virtualEndPointProviderMock.Replay ();
      endPointMock1.Replay ();
      endPointMock2.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          endPointDefinition,
          new[] { _originatingEmployeeData1, _originatingEmployeeData2 },
          new[] { _fetchedComputerData1, _fetchedComputerData2 });

      _virtualEndPointProviderMock.VerifyAllExpectations();
      endPointMock1.VerifyAllExpectations();
      endPointMock2.VerifyAllExpectations();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithInvalidDuplicateForeignKey ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Employee), "Computer");

      var fetchedComputerWithDuplicateKey = CreateFetchedComputerData (_fetchedComputer2, _originatingEmployee1.ID);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedComputer1.ID)).Return (_fetchedComputerData1.DataSourceData);
      _loadedDataContainerProviderStub.Stub (stub => stub.GetDataContainerWithoutLoading (fetchedComputerWithDuplicateKey.LoadedObjectData.ObjectID)).Return (fetchedComputerWithDuplicateKey.DataSourceData);

      _virtualEndPointProviderMock.Replay ();

      Assert.That (
          () => _agent.GroupAndRegisterRelatedObjects (
              endPointDefinition,
              new[] { _originatingEmployeeData1 },
              new[] { _fetchedComputerData1, fetchedComputerWithDuplicateKey }),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Two items in the related object result set point back to the same object. This is not allowed in a 1:1 relation. "
              + "Object 1: 'Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid'. "
              + "Object 2: 'Computer|176a0ff6-296d-4934-bd1a-23cf52c22411|System.Guid'. "
              + "Foreign key property: 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee'"));

      _virtualEndPointProviderMock.VerifyAllExpectations ();
    }
    
    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot register relation end-point "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer' for domain object "
        + "'Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid'. The end-point belongs to an object of class 'Employee' but the domain object "
        + "has class 'Computer'.")]
    public void GroupAndRegisterRelatedObjects_OriginatingObjectOfInvalidType ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Employee), "Computer");

      _virtualEndPointProviderMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (
          endPointDefinition, 
          new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Computer1) }, 
          new LoadedObjectDataWithDataSourceData[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot associate object 'Employee|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e|System.Guid' with the relation end-point "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer'. An object of type "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer' was expected.")]
    public void GroupAndRegisterRelatedObjects_InvalidRelatedObject ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Employee), "Computer");

      _virtualEndPointProviderMock.Replay();

      _agent.GroupAndRegisterRelatedObjects (
          endPointDefinition, 
          new[] { _originatingEmployeeData1 },  
          new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (DomainObjectIDs.Employee2) });
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WrongCardinality ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Employee), "Subordinates");

      Assert.That (
          () =>
          _agent.GroupAndRegisterRelatedObjects (
              endPointDefinition,
              new[] { _originatingEmployeeData1 },
              new[] { _fetchedComputerData1 }), 
          Throws.ArgumentException.With.Message.EqualTo (
              "Only virtual object-valued relation end-points can be handled by this registration agent.\r\nParameter name: relationEndPointDefinition"));
    }

    [Test]
    public void Serialization ()
    {
      var agent = new FetchedVirtualObjectRelationDataRegistrationAgent (new SerializableVirtualEndPointProviderFake ());

      var deserializedInstance = Serializer.SerializeAndDeserialize (agent);

      Assert.That (deserializedInstance.VirtualEndPointProvider, Is.Not.Null);
    }

    private LoadedObjectDataWithDataSourceData CreateFetchedComputerData (Computer fetchedObject, ObjectID EmployeeID)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (fetchedObject.ID, "Employee");
      var loadedObjectDataStub = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (fetchedObject);
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, EmployeeID);
      return new LoadedObjectDataWithDataSourceData (loadedObjectDataStub, dataContainer);
    }

    private void ExpectGetEndPoint (
        ObjectID objectID,
        IRelationEndPointDefinition endPointDefinition,
        IVirtualEndPointProvider virtualEndPointProviderMock,
        IVirtualObjectEndPoint virtualObjectEndPointMock,
        bool expectedIsDataComplete)
    {
      var relationEndPointID = RelationEndPointID.Create (objectID, endPointDefinition);
      virtualEndPointProviderMock.Expect (mock => mock.GetOrCreateVirtualEndPoint (relationEndPointID)).Return (virtualObjectEndPointMock);
      virtualObjectEndPointMock.Expect (mock => mock.IsDataComplete).Return (expectedIsDataComplete);
    }
  }
}