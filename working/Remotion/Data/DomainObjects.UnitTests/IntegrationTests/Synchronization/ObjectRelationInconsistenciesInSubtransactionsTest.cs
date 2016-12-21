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

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Synchronization
{
  [TestFixture]
  public class ObjectRelationInconsistenciesInSubtransactionsTest : RelationInconsistenciesTestBase
  {
    [Test]
    public void VirtualEndPointQuery_OneOne_Consistent_RealEndPointLoadedFirst ()
    {
      OrderTicket orderTicket1;
      Order order1;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
        order1 = DomainObjectIDs.Order1.GetObject<Order> ();

        Assert.That (orderTicket1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

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

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_Consistent_VirtualEndPointLoadedFirst ()
    {
      Order order1;
      OrderTicket orderTicket1;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        order1 = DomainObjectIDs.Order1.GetObject<Order> ();
        order1.OrderTicket.EnsureDataAvailable();
        orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();

        Assert.That (orderTicket1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

        Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));
        Assert.That (orderTicket1.Order, Is.SameAs (order1));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

        // these do nothing
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (orderTicket1, oi => oi.Order));
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (orderTicket1.Order, o => o.OrderTicket));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
      }

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_ObjectReturned_ThatLocallyPointsToNull ()
    {
      Computer computer;
      Employee employee;
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        PrepareInconsistentState_OneOne_ObjectReturned_ThatLocallyPointsToNull (out computer, out employee);

        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (computer.Employee, Is.Null);

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, false);

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);

        Assert.That (employee.Computer, Is.Null);
        Assert.That (computer.Employee, Is.Null);
      }

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);

      Assert.That (employee.Computer, Is.Null);
      Assert.That (computer.Employee, Is.Null);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse ()
    {
      Computer computer;
      Employee employee;
      Employee employee2;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        PrepareInconsistentState_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse (out computer, out employee, out employee2);

        Assert.That (computer.Employee, Is.SameAs (employee2));
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (employee2.Computer, Is.SameAs (computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, false);
        CheckSyncState (employee2, e => e.Computer, true);

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);
        CheckSyncState (employee2, e => e.Computer, true);

        Assert.That (computer.Employee, Is.SameAs (employee2));
        Assert.That (employee2.Computer, Is.SameAs (computer));
        Assert.That (employee.Computer, Is.Null);
      }

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (employee2, e => e.Computer, true);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee2.Computer, Is.SameAs (computer));
      Assert.That (employee.Computer, Is.Null);
    }

    [Test]
    public void VirtualEndPointQuery_OneOne_ObjectNotReturned_ThatLocallyPointsToHere ()
    {
      Employee employee;
      Employee employee2;
      Computer computer;

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        PrepareInconsistentState_OneOne_ObjectNotReturned_ThatLocallyPointsToHere (out computer, out employee, out employee2);

        Assert.That (computer.Employee, Is.SameAs (employee2));
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (employee2.Computer, Is.Null);

        CheckSyncState (computer, c => c.Employee, false);
        CheckSyncState (employee, e => e.Computer, false);
        CheckSyncState (employee2, e => e.Computer, true);

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

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (employee2, e => e.Computer, true);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.Null);
      Assert.That (employee2.Computer, Is.SameAs (computer));
    }

    [Test]
    public void ObjectLoaded_WithInconsistentForeignKey_OneOne_Null ()
    {
      Employee employee;
      Computer computer;

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNull (out employee, out computer);

        Assert.That (computer.Employee, Is.SameAs (employee));
        Assert.That (employee.Computer, Is.Null);

        CheckSyncState (computer, c => c.Employee, false);
        CheckSyncState (employee, e => e.Computer, true);

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (computer, c => c.Employee));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);

        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (computer.Employee, Is.SameAs (employee));
      }

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

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNonNull (out employee, out computer, out computer2);

        // computer.Employee and employee.Computer match, but computer2.Employee doesn't
        Assert.That (computer.Employee, Is.SameAs (employee));
        Assert.That (computer2.Employee, Is.SameAs (employee));
        Assert.That (employee.Computer, Is.SameAs (computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);
        CheckSyncState (computer2, c => c.Employee, false);

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

        // By unloading the employee.Computer -> computer relation, we can now synchronize computer2.Employee -> employee with employee.Computer
        UnloadService.UnloadVirtualEndPoint (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (computer2, c => c.Employee));

        CheckSyncState (computer, c => c.Employee, false);
        CheckSyncState (computer2, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);

        Assert.That (employee.Computer, Is.SameAs (computer2));
        Assert.That (computer2.Employee, Is.SameAs (employee));
        Assert.That (computer.Employee, Is.SameAs (employee));
      }

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (computer2, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);

      Assert.That (employee.Computer, Is.SameAs (computer2));
      Assert.That (computer2.Employee, Is.SameAs (employee));
      Assert.That (computer.Employee, Is.SameAs (employee));
    }

    [Test]
    public void ConsistentState_GuaranteedInSubTransaction_OneOne ()
    {
      var orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();

      Assert.That (orderTicket1.Order, Is.SameAs (order1));
      Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

      CheckSyncState (orderTicket1, oi => oi.Order, true);
      CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (orderTicket1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderTicket, Is.SameAs (orderTicket1));

        CheckSyncState (orderTicket1, oi => oi.Order, true);
        CheckSyncState (orderTicket1.Order, o => o.OrderTicket, true);
      }

      order1.OrderTicket = null;

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (orderTicket1.Order, Is.Null);
        Assert.That (order1.OrderTicket, Is.Null);

        CheckSyncState (orderTicket1, oi => oi.Order, true);
      }
      ClientTransaction.Current.Rollback ();

      orderTicket1.Order = null;

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (orderTicket1.Order, Is.Null);
        Assert.That (order1.OrderTicket, Is.Null);

        CheckSyncState (orderTicket1, oi => oi.Order, true);
      }
    }

    [Test]
    public void InconsistentState_GuaranteedInSubTransaction_OneOne_ObjectReturned_ThatLocallyPointsToNull ()
    {
      Computer computer;
      Employee employee;

      PrepareInconsistentState_OneOne_ObjectReturned_ThatLocallyPointsToNull (out computer, out employee);

      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (computer.Employee, Is.Null);

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (computer.Employee, Is.Null);

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, false);
      }
    }

    [Test]
    public void InconsistentState_GuaranteedInSubTransaction_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse ()
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

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (computer.Employee, Is.SameAs (employee2));
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (employee2.Computer, Is.SameAs (computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, false);
        CheckSyncState (employee2, e => e.Computer, true);
      }
    }

    [Test]
    public void InconsistentState_GuaranteedInSubTransaction_OneOne_ObjectNotReturned_ThatLocallyPointsToHere ()
    {
      Employee employee;
      Employee employee2;
      Computer computer;

      PrepareInconsistentState_OneOne_ObjectNotReturned_ThatLocallyPointsToHere (out computer, out employee, out employee2);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (computer.Employee, Is.SameAs (employee2));
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (employee2.Computer, Is.Null);

        CheckSyncState (computer, c => c.Employee, false);
        CheckSyncState (employee, e => e.Computer, false);
        CheckSyncState (employee2, e => e.Computer, true);
      }
    }

    [Test]
    public void InconsistentState_GuaranteedInSubTransaction_ObjectLoaded_WithInconsistentForeignKey_OneOne_Null ()
    {
      Employee employee;
      Computer computer;

      PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNull (out employee, out computer);

      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, true);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (computer.Employee, Is.SameAs (employee));
        Assert.That (employee.Computer, Is.Null);

        CheckSyncState (computer, c => c.Employee, false);
        CheckSyncState (employee, e => e.Computer, true);
      }
    }

    [Test]
    public void InconsistentState_GuaranteedInSubTransaction_ObjectLoaded_WithInconsistentForeignKey_OneOne_NonNull ()
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

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (computer.Employee, Is.SameAs (employee));
        Assert.That (computer2.Employee, Is.SameAs (employee));
        Assert.That (employee.Computer, Is.SameAs (computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);
        CheckSyncState (computer2, c => c.Employee, false);
      }
    }

    [Test]
    public void Commit_InSubTransaction_InconsistentState_OneOne_ObjectReturned_ThatLocallyPointsToNull ()
    {
      Computer computer;
      Employee employee;

      PrepareInconsistentState_OneOne_ObjectReturned_ThatLocallyPointsToNull (out computer, out employee);

      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (computer.Employee, Is.Null);

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);

      Employee newEmployee;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (computer.Employee, Is.Null);

        newEmployee = Employee.NewObject();
        computer.Employee = newEmployee;

        ClientTransaction.Current.Commit();

        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (computer.Employee, Is.SameAs (newEmployee));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, false);
      }

      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (computer.Employee, Is.SameAs (newEmployee));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);
    }

    [Test]
    public void Commit_InSubTransaction_InconsistentState_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse ()
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

      Employee newEmployee;
      Computer newComputer;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (computer.Employee, Is.SameAs (employee2));
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (employee2.Computer, Is.SameAs (computer));

        newEmployee = Employee.NewObject();
        computer.Employee = newEmployee;

        newComputer = Computer.NewObject();
        employee2.Computer = newComputer;

        ClientTransaction.Current.Commit();

        Assert.That (computer.Employee, Is.SameAs (newEmployee));
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (employee2.Computer, Is.SameAs (newComputer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, false);
        CheckSyncState (employee2, e => e.Computer, true);
      }

      Assert.That (computer.Employee, Is.SameAs (newEmployee));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.SameAs (newComputer));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);
    }

    [Test]
    public void Commit_InSubTransaction_InconsistentState_OneOne_ObjectNotReturned_ThatLocallyPointsToHere ()
    {
      Employee employee;
      Employee employee2;
      Computer computer;

      PrepareInconsistentState_OneOne_ObjectNotReturned_ThatLocallyPointsToHere (out computer, out employee, out employee2);

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);

      Computer newComputer;

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (computer.Employee, Is.SameAs (employee2));
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (employee2.Computer, Is.Null);

        newComputer = Computer.NewObject();
        employee2.Computer = newComputer;

        ClientTransaction.Current.Commit ();

        Assert.That (computer.Employee, Is.SameAs (employee2));
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (employee2.Computer, Is.SameAs (newComputer));

        CheckSyncState (computer, c => c.Employee, false);
        CheckSyncState (employee, e => e.Computer, false);
        CheckSyncState (employee2, e => e.Computer, true);
      }

      Assert.That (computer.Employee, Is.SameAs (employee2));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (employee2.Computer, Is.SameAs (newComputer));

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, false);
      CheckSyncState (employee2, e => e.Computer, true);
    }

    [Test]
    public void Commit_InSubTransaction_InconsistentState_ObjectLoaded_WithInconsistentForeignKey_OneOne_Null ()
    {
      Employee employee;
      Computer computer;

      PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNull (out employee, out computer);

      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.Null);

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, true);

      Computer newComputer;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (computer.Employee, Is.SameAs (employee));
        Assert.That (employee.Computer, Is.Null);

        newComputer = Computer.NewObject();
        employee.Computer = newComputer;

        ClientTransaction.Current.Commit ();

        Assert.That (computer.Employee, Is.SameAs (employee));
        Assert.That (employee.Computer, Is.SameAs (newComputer));

        CheckSyncState (computer, c => c.Employee, false);
        CheckSyncState (employee, e => e.Computer, true);
      }

      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.SameAs (newComputer));

      CheckSyncState (computer, c => c.Employee, false);
      CheckSyncState (employee, e => e.Computer, true);
    }

    [Test]
    public void Commit_InSubTransaction_InconsistentState_ObjectLoaded_WithInconsistentForeignKey_OneOne_NonNull ()
    {
      Employee employee;
      Computer computer;
      Computer computer2;

      PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNonNull (out employee, out computer, out computer2);

      // computer.Employee and employee.Computer match, but computer2.Employee doesn't
      Assert.That (computer.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.SameAs (computer));
      Assert.That (computer2.Employee, Is.SameAs (employee));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (computer2, c => c.Employee, false);

      Employee newEmployee;
      Computer newComputer;

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        // computer.Employee and employee.Computer match, but computer2.Employee doesn't
        Assert.That (computer.Employee, Is.SameAs (employee));
        Assert.That (employee.Computer, Is.SameAs (computer));
        Assert.That (computer2.Employee, Is.SameAs (employee));

        newEmployee = Employee.NewObject ();
        computer.Employee = newEmployee;

        newComputer = Computer.NewObject ();
        employee.Computer = newComputer;

        ClientTransaction.Current.Commit();

        Assert.That (computer.Employee, Is.SameAs (newEmployee));
        Assert.That (employee.Computer, Is.SameAs (newComputer));
        Assert.That (computer2.Employee, Is.SameAs (employee));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);
        CheckSyncState (computer2, c => c.Employee, false);
      }

      Assert.That (computer.Employee, Is.SameAs (newEmployee));
      Assert.That (employee.Computer, Is.SameAs (newComputer));
      Assert.That (computer2.Employee, Is.SameAs (employee));

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (computer2, c => c.Employee, false);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootOnly_SyncedInRoot ()
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

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);
        CheckSyncState (employee2, e => e.Computer, true);
      }

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (employee2, e => e.Computer, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootOnly_SyncedInSub ()
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

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);
        CheckSyncState (employee2, e => e.Computer, true);
      }

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (employee2, e => e.Computer, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootAndSub_SyncedInRoot ()
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

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Resolve (computer, c => c.Employee));
        TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Resolve (employee, e => e.Computer));
        TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Resolve (employee2, e => e.Computer));

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current.ParentTransaction, RelationEndPointID.Resolve (employee, e => e.Computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);
        CheckSyncState (employee2, e => e.Computer, true);
      }

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (employee2, e => e.Computer, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootAndSub_SyncedInSub ()
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

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Resolve (computer, c => c.Employee));
        TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Resolve (employee, e => e.Computer));
        TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Resolve (employee2, e => e.Computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, false);
        CheckSyncState (employee2, e => e.Computer, true);

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (employee, e => e.Computer));

        CheckSyncState (computer, c => c.Employee, true);
        CheckSyncState (employee, e => e.Computer, true);
        CheckSyncState (employee2, e => e.Computer, true);
      }

      CheckSyncState (computer, c => c.Employee, true);
      CheckSyncState (employee, e => e.Computer, true);
      CheckSyncState (employee2, e => e.Computer, true);
    }
  }
}