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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.EagerFetching
{
  [TestFixture]
  public class FetchedRealObjectRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private FetchedRealObjectRelationDataRegistrationAgent _agent;

    private ILoadedObjectData _originatingOrderTicketData1;
    private ILoadedObjectData _originatingOrderTicketData2;

    private LoadedObjectDataWithDataSourceData _fetchedOrderData1;
    private LoadedObjectDataWithDataSourceData _fetchedOrderData2;
    private LoadedObjectDataWithDataSourceData _fetchedOrderData3;

    public override void SetUp ()
    {
      base.SetUp();

      _agent = new FetchedRealObjectRelationDataRegistrationAgent();

      var originatingOrderTicket1 = DomainObjectMother.CreateFakeObject<OrderTicket>(DomainObjectIDs.OrderTicket1);
      var originatingOrderTicket2 = DomainObjectMother.CreateFakeObject<OrderTicket>(DomainObjectIDs.OrderTicket2);

      _originatingOrderTicketData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(originatingOrderTicket1).Object;
      _originatingOrderTicketData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(originatingOrderTicket2).Object;

      var fetchedOrder1 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order1);
      var fetchedOrder2 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order3);
      var fetchedOrder3 = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order4);

      _fetchedOrderData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(fetchedOrder1);
      _fetchedOrderData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(fetchedOrder2);
      _fetchedOrderData3 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(fetchedOrder3);

    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(OrderTicket), "Order");

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { _originatingOrderTicketData1, _originatingOrderTicketData2 },
          new[] { _fetchedOrderData1, _fetchedOrderData2, _fetchedOrderData3 });
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullOriginalObject ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(OrderTicket), "Order");

      _agent.GroupAndRegisterRelatedObjects(endPointDefinition, new[] { new NullLoadedObjectData() }, new LoadedObjectDataWithDataSourceData[0]);
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullRelatedObject ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(OrderTicket), "Order");

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { _originatingOrderTicketData1 },
          new[] { LoadedObjectDataObjectMother.CreateNullLoadedObjectDataWithDataSourceData() });
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_PropertyOnBaseType ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(FileSystemItem), "ParentFolder");

      var originatingFile = DomainObjectMother.CreateFakeObject<File>();
      var originatingFileData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(originatingFile).Object;

      var fetchedParentFolder = DomainObjectMother.CreateFakeObject<Folder>();
      var fetchedParentFolderData = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(fetchedParentFolder);

      originatingFile.InternalDataContainer.SetValue(GetPropertyDefinition(typeof(FileSystemItem), "ParentFolder"), fetchedParentFolder.ID);

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { originatingFileData },
          new[] { fetchedParentFolderData });
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_PropertyOnDerivedType ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Partner), "ContactPerson");

      var originatingCompany = DomainObjectMother.CreateFakeObject<Company>();
      var originatingCompanyData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(originatingCompany).Object;

      var originatingPartner = DomainObjectMother.CreateFakeObject<Partner>();
      var originatingPartnerData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(originatingPartner).Object;

      var originatingCustomer = DomainObjectMother.CreateFakeObject<Customer>();
      var originatingCustomerData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(originatingCustomer).Object;

      var fetchedPerson = DomainObjectMother.CreateFakeObject<Person>();
      var fetchedPersonData = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(fetchedPerson);

      originatingPartner.InternalDataContainer.SetValue(GetPropertyDefinition(typeof(Partner), "ContactPerson"), fetchedPerson.ID);

      _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { originatingCompanyData, originatingPartnerData, originatingCustomerData },
          new[] { fetchedPersonData });
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_InvalidOriginalObject ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(OrderTicket), "Order");
      Assert.That(
          () => _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(DomainObjectIDs.Order1).Object },
          new LoadedObjectDataWithDataSourceData[0]),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot register relation end-point 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' for domain object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. The end-point belongs to an object of type "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket' but the domain object "
                  + "has type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order'."));
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_InvalidRelatedObject ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(OrderTicket), "Order");
      Assert.That(
          () => _agent.GroupAndRegisterRelatedObjects(
          endPointDefinition,
          new[] { _originatingOrderTicketData1 },
          new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(DomainObjectIDs.OrderTicket2) }),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot associate object 'OrderTicket|0005bdf4-4ccc-4a41-b9b5-baab3eb95237|System.Guid' with the relation end-point "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'. An object of type "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' was expected."));
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WrongVirtuality ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");

      Assert.That(
          () => _agent.GroupAndRegisterRelatedObjects(endPointDefinition, new[] { _originatingOrderTicketData1 }, new[] { _fetchedOrderData1 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only non-virtual object-valued relation end-points can be handled by this registration agent.", "relationEndPointDefinition"));
    }

    [Test]
    public void Serialization ()
    {
      Serializer.SerializeAndDeserialize(_agent);
    }
  }
}
