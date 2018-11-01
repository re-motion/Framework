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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class OneToOneRelationChangeWithNullTest : RelationChangeBaseTest
  {
    [Test]
    public void OldRelatedObjectOfNewRelatedObjectIsNull ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      Computer newComputerWithoutEmployee = DomainObjectIDs.Computer4.GetObject<Computer> ();

      employee.Computer = newComputerWithoutEmployee;

      // expectation: no exception
    }

    [Test]
    public void NewRelatedObjectIsNull ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      employee.Computer = null;

      // expectation: no exception
    }

    [Test]
    public void OldRelatedObjectIsNull ()
    {
      Employee employeeWithoutComputer = DomainObjectIDs.Employee1.GetObject<Employee> ();
      Computer computerWithoutEmployee = DomainObjectIDs.Computer4.GetObject<Computer> ();
      employeeWithoutComputer.Computer = computerWithoutEmployee;

      // expectation: no exception
    }

    [Test]
    public void SetRelatedObjectAndOldRelatedObjectIsNull ()
    {
      Computer computerWithoutEmployee = DomainObjectIDs.Computer4.GetObject<Computer> ();
      Employee employee = DomainObjectIDs.Employee1.GetObject<Employee> ();
      computerWithoutEmployee.Employee = employee;

      Assert.That (computerWithoutEmployee.Properties[typeof (Computer), "Employee"].GetRelatedObjectID (), Is.EqualTo (employee.ID));

      Assert.That (computerWithoutEmployee.Employee, Is.SameAs (employee));
      Assert.That (employee.Computer, Is.SameAs (computerWithoutEmployee));
    }

    [Test]
    public void SetRelatedObjectOverVirtualEndPointAndOldRelatedObjectIsNull ()
    {
      Employee employeeWithoutComputer = DomainObjectIDs.Employee1.GetObject<Employee> ();
      Computer computer = DomainObjectIDs.Computer4.GetObject<Computer> ();
      employeeWithoutComputer.Computer = computer;

      Assert.That (computer.Properties[typeof (Computer), "Employee"].GetRelatedObjectID (), Is.EqualTo (employeeWithoutComputer.ID));

      Assert.That (employeeWithoutComputer.Computer, Is.SameAs (computer));
      Assert.That (computer.Employee, Is.SameAs (employeeWithoutComputer));
    }

    [Test]
    public void SetNewRelatedObjectNull ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      Computer computer = employee.Computer;
      computer.Employee = null;

      Assert.That (computer.Properties[typeof (Computer), "Employee"].GetRelatedObjectID (), Is.Null);

      Assert.That (computer.Employee, Is.Null);
      Assert.That (employee.Computer, Is.Null);
    }

    [Test]
    public void SetNewRelatedObjectNullOverVirtualEndPoint ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      Computer computer = employee.Computer;
      employee.Computer = null;

      Assert.That (computer.Properties[typeof (Computer), "Employee"].GetRelatedObjectID(), Is.Null);

      Assert.That (employee.Computer, Is.Null);
      Assert.That (computer.Employee, Is.Null);
    }

    [Test]
    public void HasBeenTouchedWithNull_RealSide ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      Computer computer = employee.Computer;

      CheckTouching (delegate { computer.Employee = null; }, computer, "Employee",
          RelationEndPointID.Create(employee.ID, typeof (Employee).FullName + ".Computer"),
          RelationEndPointID.Create(computer.ID, typeof (Computer).FullName + ".Employee"));
    }

    [Test]
    public void HasBeenTouchedWithNullTwice_RealSide ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      Computer computer = employee.Computer;

      computer.Employee = null;
      
      SetDatabaseModifyable ();
      TestableClientTransaction.Commit ();

      CheckTouching (delegate { computer.Employee = null; }, computer, "Employee",
          RelationEndPointID.Create(computer.ID, typeof (Computer).FullName + ".Employee"));
    }

    [Test]
    public void HasBeenTouchedWithNull_VirtualSide ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      Computer computer = employee.Computer;

      CheckTouching (delegate { employee.Computer = null; }, computer, "Employee",
          RelationEndPointID.Create(employee.ID, typeof (Employee).FullName + ".Computer"),
          RelationEndPointID.Create(computer.ID, typeof (Computer).FullName + ".Employee"));
    }

    [Test]
    public void HasBeenTouchedWithNullTwice_VirtualSide ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();

      employee.Computer = null;

      SetDatabaseModifyable ();
      TestableClientTransaction.Commit ();

      CheckTouching (delegate { employee.Computer = null; }, null, null,
          RelationEndPointID.Create(employee.ID, typeof (Employee).FullName + ".Computer"));
    }
  }
}
