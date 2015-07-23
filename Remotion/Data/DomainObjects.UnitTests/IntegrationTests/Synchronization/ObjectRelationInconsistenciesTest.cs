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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Synchronization
{
  [TestFixture]
  public class ObjectRelationInconsistenciesTest : RelationInconsistenciesTestBase
  {
    [Test]
    public void VirtualEndPointQuery_OneOne_Consistent_RealEndPointLoadedFirst ()
    {
      var orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();

      Assert.That (orderTicket1.Order, Is.SameAs (order1));
      Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

      // these do nothing
      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (orderTicket1, oi => oi.Order));
      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (orderTicket1.Order, o => o.OrderTicket));

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_Consistent_VirtualEndPointLoadedFirst ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      order1.OrderTicket.EnsureDataAvailable();
      var orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();

      Assert.That (orderTicket1.Order, Is.SameAs (order1));
      Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

      // these do nothing
      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (orderTicket1, oi => oi.Order));
      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (orderTicket1.Order, o => o.OrderTicket));

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_ObjectReturned_ThatLocallyPointsToNull ()
    {
      Computer computer;
      Employee employee;
      PrepareInconsistentState_OneOne_ObjectReturned_ThatLocallyPointsToNull (out computer, out employee);

      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (computer.Employee, Is.Null);

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);

      CheckActionWorks (computer.Delete);
      ClientTransaction.Current.Rollback();

      // sync states not changed by Rollback
      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);

      CheckActionThrows<InvalidOperationException> (employee.Delete, "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => employee.Computer = null, "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => employee.Computer = Computer.NewObject (), "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => computer.Employee = employee, "out of sync with the opposite object property");

      var newEmployee = Employee.NewObject();
      CheckActionWorks (() => computer.Employee = newEmployee);

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);

      Assert.That (computer.Employee, Is.SameAs (newEmployee));
      Assert.That (newEmployee.Computer, Is.SameAs (computer));
      Assert.That (employee.Computer, Is.Null);

      CheckActionWorks (() => computer.Employee = employee);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse ()
    {
      Computer computer;
      Employee employee;
      Employee employee2;
      PrepareInconsistentState_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse (out computer, out employee, out employee2);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.SameAs (computer));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);

      CheckActionWorks (computer.Delete);
      CheckActionWorks (employee2.Delete);
      ClientTransaction.Current.Rollback ();

      // sync states not changed by Rollback
      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);

      CheckActionThrows<InvalidOperationException> (employee.Delete, "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => employee.Computer = null, "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => employee.Computer = Computer.NewObject (), "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => computer.Employee = employee, "out of sync with the opposite object property");

      CheckActionWorks (() => computer.Employee = null);
      computer.Employee = employee2;
      Assert.That (computer.State, Is.EqualTo (StateType.Unchanged));

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (employee2, e => e.Computer, true);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee2.Computer, Is.SameAs (computer));
      Assert.That (employee.Computer, Is.Null);

      CheckActionWorks (() => computer.Employee = employee);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse_SolvableViaReload ()
    {
      Computer computer;
      Employee employee;
      Employee employee2;
      PrepareInconsistentState_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse (out computer, out employee, out employee2);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.SameAs (computer));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);

      UnloadService.UnloadData (ClientTransaction.Current, computer.ID);

      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (employee2, e => e.Computer, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_ObjectNotReturned_ThatLocallyPointsToHere ()
    {
      Computer computer;
      Employee employee;
      Employee employee2;
      PrepareInconsistentState_OneOne_ObjectNotReturned_ThatLocallyPointsToHere (out computer, out employee, out employee2);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);

      // sync states not changed by Rollback
      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);

      CheckActionThrows<InvalidOperationException> (computer.Delete, "out of sync with the opposite property");
      CheckActionThrows<InvalidOperationException> (employee.Delete, "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (employee2.Delete, "out of sync with the virtual property");
      CheckActionThrows<InvalidOperationException> (() => employee.Computer = null, "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => employee.Computer = Computer.NewObject (), "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => computer.Employee = employee, "out of sync with the opposite property");
      CheckActionThrows<InvalidOperationException> (() => computer.Employee = null, "out of sync with the opposite property");

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (computer, c => c.Employee));
      
      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.SameAs (computer));

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (employee2, e => e.Computer, true);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.Null);
      Assert.That (employee2.Computer, Is.SameAs (computer));
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_ObjectNotReturned_ThatLocallyPointsToHere_WithChangedRelation ()
    {
      SetDatabaseModifyable ();

      var employee2 = DomainObjectIDs.Employee2.GetObject<Employee> ();

      var computer = CreateComputerAndSetEmployeeInOtherTransaction (employee2.ID).GetObject<Computer> ();
      Assert.That (computer.Employee, Is.SameAs (employee2));

      // 1:1 relations automatically cause virtual end-points to be marked loaded when the foreign key object is loaded, so unload the virtual side
      UnloadService.UnloadVirtualEndPoint (ClientTransaction.Current, RelationEndPointID.Resolve (employee2, e => e.Computer));

      SetEmployeeInOtherTransaction (computer.ID, DomainObjectIDs.Employee1);

      // Resolve virtual end point - the database says that computer points to employee1, but the transaction says computer points to employee2!
      Dev.Null = employee2.Computer;

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee2.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee2, e => e.Computer, true);

      var newComputer = Computer.NewObject();
      employee2.Computer = newComputer;

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (computer, c => c.Employee));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee2, e => e.Computer, true);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee2.Computer, Is.SameAs (newComputer));
      Assert.That (employee2.Properties[typeof (Employee), "Computer"].GetOriginalValue<Computer>(), Is.SameAs (computer));
      Assert.That (employee2.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void ObjectLoaded_WithInconsistentForeignKey_OneOne_Null ()
    {
      Employee employee;
      Computer computer;
      PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNull(out employee, out computer);

      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, true);

      // sync states not changed by Rollback
      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, true);

      CheckActionThrows<InvalidOperationException> (employee.Delete, "out of sync with the virtual property");
      CheckActionThrows<InvalidOperationException> (computer.Delete, "out of sync with the opposite property");
      CheckActionThrows<InvalidOperationException> (() => computer.Employee = null, "out of sync with the opposite property");
      CheckActionThrows<InvalidOperationException> (() => employee.Computer = computer, "out of sync with the virtual property");

      CheckActionWorks (() => employee.Computer = Computer.NewObject());
      ClientTransaction.Current.Rollback ();

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (computer, c => c.Employee));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);

      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (computer.Employee, Is.SameAs (employee));
    }

    [Test]
    public void ObjectLoaded_WithInconsistentForeignKey_OneOne_Null_UnloadCorrectsIssue ()
    {
      Employee employee;
      Computer computer;
      PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNull (out employee, out computer);

      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, true);

      // Reload virtual relation from database => the two sides now match
      UnloadService.UnloadVirtualEndPoint (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));
      Dev.Null = employee.Computer;

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);

      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (computer.Employee, Is.SameAs (employee));
    }

    [Test]
    public void ObjectLoaded_WithInconsistentForeignKey_OneOne_NonNull ()
    {
      Employee employee;
      Computer computer;
      Computer computer2;
      PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNonNull(out employee, out computer, out computer2);

      // computer.Employee and employee.Computer match, but computer2.Employee doesn't
      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (computer2.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.SameAs (computer));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (computer2, c => c.Employee, false);

      CheckActionWorks (computer.Delete);
      ClientTransaction.Current.Rollback ();

      // sync states not changed by Rollback
      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (computer2, c => c.Employee, false);

      CheckActionThrows<InvalidOperationException> (employee.Delete, "out of sync with the virtual property");
      CheckActionThrows<InvalidOperationException> (computer2.Delete, "out of sync with the opposite property");
      CheckActionThrows<InvalidOperationException> (() => computer2.Employee = null, "out of sync with the opposite property");
      CheckActionThrows<InvalidOperationException> (() => employee.Computer = computer2, "out of sync with the virtual property");

      CheckActionWorks (() => computer.Employee = null);
      ClientTransaction.Current.Rollback ();

      CheckActionWorks (() => employee.Computer = null);
      ClientTransaction.Current.Rollback ();

      CheckActionThrows<InvalidOperationException> (() =>
          BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (computer2, c => c.Employee)),
          "The object end-point '{0}/Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee' "
          + "cannot be synchronized with the virtual object end-point "
          + "'{2}/Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer' because "
          + "the virtual relation property already refers to another object ('{1}'). To synchronize "
          + "'{0}/Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee', use "
          + "UnloadService to unload either object '{1}' or the virtual object end-point "
          + "'{2}/Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer'.",
          computer2.ID,
          computer.ID,
          employee.ID);

      // By unloading computer, we can notw synchronize the relation
      UnloadService.UnloadData (ClientTransaction.Current, computer.ID);
      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (computer2, c => c.Employee));

      ClientTransaction.Current.EnsureDataAvailable (computer.ID);

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (computer2, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);

      Assert.That (computer.Employee, Is.Null);
      Assert.That (employee.Computer, Is.SameAs (computer2));
      Assert.That (computer2.Employee, Is.SameAs (employee));
    }

    [Test]
    public void ObjectLoaded_WithInconsistentForeignKey_OneOne_NonNull_UnloadCorrectsIssue ()
    {
      Employee employee;
      Computer computer;
      Computer computer2;
      PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNonNull (out employee, out computer, out computer2);

      // computer.Employee and employee.Computer match, but computer2.Employee doesn't
      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (computer2.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.SameAs (computer));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (computer2, c => c.Employee, false);

      // Reload virtual relation from database => computer2 and employee now match, computer is unsynchronized
      UnloadService.UnloadVirtualEndPoint (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));
      Dev.Null = employee.Computer;
      
      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (computer2, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);

      Assert.That (employee.Computer, Is.SameAs (computer2));
      Assert.That (computer2.Employee, Is.SameAs (employee));
      Assert.That (computer.Employee, Is.SameAs (employee));
    }

    [Test]
    public void Commit_DoesNotChangeInconsistentState_OneOne_VirtualSideInconsistent ()
    {
      Computer computer;
      Employee employee;
      Employee employee2;
      PrepareInconsistentState_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse (out computer, out employee, out employee2);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.SameAs (computer));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);

      ClientTransaction.Current.Commit();

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.SameAs (computer));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);
    }

    [Test]
    public void Commit_DoesNotChangeInconsistentState_OneOne_RealSideInconsistent ()
    {
      Employee employee;
      Computer computer;
      PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNull (out employee, out computer);

      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, true);

      ClientTransaction.Current.Commit();

      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, true);
    }
  }
}