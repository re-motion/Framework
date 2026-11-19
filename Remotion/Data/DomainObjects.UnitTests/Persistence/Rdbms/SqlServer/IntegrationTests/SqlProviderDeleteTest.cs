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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture(CreateSaveCommandFactoryBehaviour.CreateBatchedSaveCommandFactory)]
  [TestFixture(CreateSaveCommandFactoryBehaviour.CreateIndividualSaveCommandFactory)]
  public class SqlProviderDeleteTest : SqlProviderBaseTest
  {
    public SqlProviderDeleteTest (CreateSaveCommandFactoryBehaviour createSaveCommandFactoryBehaviour)
        : base(createSaveCommandFactoryBehaviour)
    {
    }

    [Test]
    public void DeleteSingleDataContainer ()
    {
      IReadOnlyCollection<DataContainer> containers = new[] { GetDeletedOrderTicketContainer() };
      Provider.Connect();
      Provider.Save(containers);

      Assert.That(Provider.LoadDataContainer(DomainObjectIDs.OrderTicket1).LocatedObject, Is.Null);
    }

    [Test]
    public void DeleteRelatedDataContainers ()
    {
      Employee supervisor = DomainObjectIDs.Employee2.GetObject<Employee>();
      Employee subordinate = DomainObjectIDs.Employee3.GetObject<Employee>();
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer>();

      supervisor.Delete();
      subordinate.Delete();
      computer.Delete();

      IReadOnlyCollection<DataContainer> containers = new[]
                                                      {
                                                          supervisor.InternalDataContainer,
                                                          subordinate.InternalDataContainer,
                                                          computer.InternalDataContainer
                                                      };

      Provider.Connect();
      Provider.Save(containers);
    }

    [Test]
    public void DeleteRelatedDataContainers_WithForeignKeyCycleBetweenTables ()
    {
      // Company.ContactPersonID -> Person and Person.AssociatedCustomerCompanyID -> Company form a foreign key cycle between two tables.
      // Both directions are used so the test does not depend on which edge the sorting provider decides to break.
      ObjectID[] objectIDs;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var partnerContactPerson = Person.NewObject();
        partnerContactPerson.Name = "Partner contact person";
        var partner = Partner.NewObject();
        partner.Name = "Partner";
        partner.ContactPerson = partnerContactPerson;
        var partnerCeo = Ceo.NewObject();
        partnerCeo.Name = "Partner CEO";
        partnerCeo.Company = partner;

        var customerContactPerson = Person.NewObject();
        customerContactPerson.Name = "Customer contact person";
        var customer = Customer.NewObject();
        customer.Name = "Customer";
        customer.ContactPerson = customerContactPerson;
        var customerCeo = Ceo.NewObject();
        customerCeo.Name = "Customer CEO";
        customerCeo.Company = customer;

        var newObjects = new TestDomainBase[] { partner, partnerContactPerson, partnerCeo, customer, customerContactPerson, customerCeo };
        objectIDs = newObjects.Select(o => o.ID).ToArray();

        using (var insertProvider = CreateRdbmsProvider())
        {
          insertProvider.Connect();
          insertProvider.Save(newObjects.Select(o => o.InternalDataContainer).ToArray());
        }
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var deletedObjects = objectIDs.Select(id => id.GetObject<TestDomainBase>()).ToArray();
        foreach (var deletedObject in deletedObjects)
          deletedObject.Delete();

        IReadOnlyCollection<DataContainer> containers = deletedObjects.Select(o => o.InternalDataContainer).ToArray();

        Provider.Connect();
        Assert.That(() => Provider.Save(containers), Throws.Nothing);
      }

      foreach (var objectID in objectIDs)
        Assert.That(Provider.LoadDataContainer(objectID).LocatedObject, Is.Null);
    }

    [Test]
    public void ConcurrentDeleteWithForeignKey ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();

      OrderTicket changedOrderTicket;
      DataContainer changedDataContainer;
      using (clientTransaction1.EnterDiscardingScope())
      {
        changedOrderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
        changedOrderTicket.FileName = @"C:\NewFile.jpg";
        changedDataContainer = changedOrderTicket.InternalDataContainer;
      }

      OrderTicket deletedOrderTicket;
      DataContainer deletedDataContainer;
      using (clientTransaction2.EnterDiscardingScope())
      {
        deletedOrderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
        deletedOrderTicket.Delete();
        deletedDataContainer = deletedOrderTicket.InternalDataContainer;
      }

      Provider.Connect();
      Provider.Save(new[] { changedDataContainer });
      Assert.That(
          () => Provider.Save(new[] { deletedDataContainer }),
          Throws.InstanceOf<ConcurrencyViolationException>());
    }

    [Test]
    public void ConcurrentDeleteWithoutForeignKey ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();

      DataContainer changedDataContainer;
      ClassWithAllDataTypes changedObject;

      using (clientTransaction1.EnterDiscardingScope())
      {
        changedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
        changedDataContainer = changedObject.InternalDataContainer;
        changedObject.StringProperty = "New text";
      }

      DataContainer deletedDataContainer;
      ClassWithAllDataTypes deletedObject;

      using (clientTransaction2.EnterDiscardingScope())
      {
        deletedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
        deletedDataContainer = deletedObject.InternalDataContainer;
        deletedObject.Delete();
      }

      Provider.Connect();
      Provider.Save(new[] { changedDataContainer });
      Assert.That(
          () => Provider.Save(new[] { deletedDataContainer }),
          Throws.InstanceOf<ConcurrencyViolationException>());
    }

    private DataContainer GetDeletedOrderTicketContainer ()
    {
      OrderTicket orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      orderTicket.Delete();
      return orderTicket.InternalDataContainer;
    }
  }
}
