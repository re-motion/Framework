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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Synchronization
{
  [TestFixture]
  public class VirtualEndPointGarbageCollectionTest : ClientTransactionBaseTest
  {
    private DataManager _dataManager;

    public override void SetUp ()
    {
      base.SetUp();

      _dataManager = TestableClientTransaction.DataManager;
    }

    [Test]
    public void UnloadLastFK_CausesCollectionEndPointToBeRemoved_ButKeepsDomainObjectCollectionInMemory ()
    {
      var industrialSector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector>();
      var companies = industrialSector.Companies;
      industrialSector.Companies.EnsureDataComplete();

      var unsynchronizedCompanyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction(industrialSector.ID);
      var unsynchronizedCompany = unsynchronizedCompanyID.GetObject<Company>();

      var virtualEndPointID = RelationEndPointID.Resolve(industrialSector, s => s.Companies);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, virtualEndPointID);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.False);

      UnloadService.UnloadData(TestableClientTransaction, unsynchronizedCompany.ID);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Null);

      // But DomainObjectCollection stays valid
      Assert.That(industrialSector.Companies, Is.SameAs(companies));
    }

    [Test]
    public void UnloadUnsynchronizedFK_LeavesCompleteEmptyCollection ()
    {
      var employee = DomainObjectIDs.Employee3.GetObject<Employee>();
      employee.Subordinates.EnsureDataComplete();
      employee.Name = "Employee";

      Assert.That(employee.Subordinates, Is.Empty);

      var unsynchronizedSubordinateID =
          RelationInconcsistenciesTestHelper.CreateAndInitializeObjectAndSetRelationInOtherTransaction<Employee, Employee>(
            employee.ID,
            (subOrdinate, e) =>
            {
              subOrdinate.Name = "Subordinate";
              subOrdinate.Supervisor = e;
            });
      var unsynchronizedSubordinate = unsynchronizedSubordinateID.GetObject<Employee>();

      var virtualEndPointID = RelationEndPointID.Resolve(employee, o => o.Subordinates);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadData(TestableClientTransaction, unsynchronizedSubordinate.ID);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);
    }

    [Test]
    public void UnloadLastFK_CausesVirtualObjectEndPointToBeRemoved ()
    {
      var employee = DomainObjectIDs.Employee3.GetObject<Employee>();
      var virtualEndPointID = RelationEndPointID.Resolve(employee, e => e.Computer);
      TestableClientTransaction.EnsureDataComplete(virtualEndPointID);

      var unsynchronizedComputerID =
          RelationInconcsistenciesTestHelper.CreateAndInitializeObjectAndSetRelationInOtherTransaction<Computer, Employee>(
            employee.ID,
            (c, e) =>
            {
              c.SerialNumber = "12345";
              c.Employee = e;
            });
      var unsynchronizedComputer = unsynchronizedComputerID.GetObject<Computer>();

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadData(TestableClientTransaction, employee.Computer.ID);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.False);

      UnloadService.UnloadData(TestableClientTransaction, unsynchronizedComputer.ID);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Null);
    }

    [Test]
    public void UnloadUnsynchronizedFK_LeavesNullCompleteVirtualObjectEndPoint ()
    {
      var employee = DomainObjectIDs.Employee1.GetObject<Employee>();
      employee.Name = "Employee";

      var virtualEndPointID = RelationEndPointID.Resolve(employee, e => e.Computer);
      TestableClientTransaction.EnsureDataComplete(virtualEndPointID);
      Assert.That(employee.Computer, Is.Null);

      var unsynchronizedComputerID =
          RelationInconcsistenciesTestHelper.CreateAndInitializeObjectAndSetRelationInOtherTransaction<Computer, Employee>(
            employee.ID,
            (c, e) =>
            {
              c.SerialNumber = "12345";
              c.Employee = e;
            });
      var unsynchronizedComputer = unsynchronizedComputerID.GetObject<Computer>();

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadData(TestableClientTransaction, unsynchronizedComputer.ID);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);
    }

    [Test]
    public void UnloadCollectionEndPoint_WithoutReferences_CausesEndPointToBeRemoved_ButKeepsDomainObjectCollectionInMemory ()
    {
      var customer = DomainObjectIDs.Customer2.GetObject<Customer>();
      var customerOrders = customer.Orders;
      customer.Orders.EnsureDataComplete();
      Assert.That(customer.Orders, Is.Empty);

      var virtualEndPointID = RelationEndPointID.Resolve(customer, c => c.Orders);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, virtualEndPointID);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Null);

      // But DomainObjectCollection stays valid
      Assert.That(customerOrders, Is.SameAs(customer.Orders));
    }

    [Test]
    public void UnloadCollectionEndPoint_WithReferences_LeavesIncompleteEndPoint ()
    {
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      var customerOrders = customer.Orders;
      customerOrders.EnsureDataComplete();
      Assert.That(customer.Orders, Is.Not.Empty);

      var virtualEndPointID = RelationEndPointID.Resolve(customer, c => c.Orders);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, virtualEndPointID);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.False);
      Assert.That(customerOrders, Is.SameAs(customer.Orders));
    }

    [Test]
    public void UnloadVirtualObjectEndPoint_WithoutReferences_CausesEndPointToBeRemoved ()
    {
      var employee = DomainObjectIDs.Employee1.GetObject<Employee>();
      Assert.That(employee.Computer, Is.Null);

      var virtualEndPointID = RelationEndPointID.Resolve(employee, e => e.Computer);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, virtualEndPointID);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualObjectEndPoint_WithReferences_LeavesIncompleteEndPoint ()
    {
      var employee = DomainObjectIDs.Employee3.GetObject<Employee>();
      Assert.That(employee.Computer, Is.Not.Null);

      var virtualEndPointID = RelationEndPointID.Resolve(employee, e => e.Computer);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, virtualEndPointID);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadCollectionEndPoint_WithoutReferences_AfterSettingDifferentCollection_AndCommit_CausesEndPointToBeRemoved_ButKeepsDomainObjectCollectionInMemory ()
    {
      var customer = DomainObjectIDs.Customer2.GetObject<Customer>();

      var newCustomerOrders = new OrderCollection();
      customer.Orders = newCustomerOrders;
      Assert.That(customer.Orders, Is.Empty);

      TestableClientTransaction.Commit();

      var virtualEndPointID = RelationEndPointID.Resolve(customer, c => c.Orders);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, virtualEndPointID);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Null);

      // But DomainObjectCollection stays valid - and uses new collection
      Assert.That(newCustomerOrders, Is.SameAs(customer.Orders));
    }

    [Test]
    public void UnloadCollectionEndPoint_WithoutReferences_AfterSettingDifferentCollection_AndRollback_CausesEndPointToBeRemoved_ButKeepsDomainObjectCollectionInMemory ()
    {
      var customer = DomainObjectIDs.Customer2.GetObject<Customer>();
      var oldCustomerOrders = customer.Orders;
      Assert.That(customer.Orders, Is.Empty);

      var newCustomerOrders = new OrderCollection();
      customer.Orders = newCustomerOrders;
      Assert.That(customer.Orders, Is.Empty);

      TestableClientTransaction.Rollback();

      var virtualEndPointID = RelationEndPointID.Resolve(customer, c => c.Orders);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Not.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, virtualEndPointID);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(virtualEndPointID), Is.Null);

      // But DomainObjectCollection stays valid - and uses original collection
      Assert.That(oldCustomerOrders, Is.SameAs(customer.Orders));
    }

    protected ObjectID CreateCompanyAndSetIndustrialSectorInOtherTransaction (ObjectID industrialSectorID)
    {
      return RelationInconcsistenciesTestHelper.CreateAndInitializeObjectAndSetRelationInOtherTransaction<Company, IndustrialSector>(industrialSectorID, (c, s) =>
      {
        c.Name = "Company";
        c.IndustrialSector = s;
        c.IndustrialSector.Name = "Sector";
        c.Ceo = Ceo.NewObject();
        c.Ceo.Name = "CEO";
      });
    }
  }
}
