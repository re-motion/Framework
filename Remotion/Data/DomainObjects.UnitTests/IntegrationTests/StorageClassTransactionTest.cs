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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class StorageClassTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void Commit_Rollback_NewObject ()
    {
      DateTime referenceDateTime = DateTime.Now;
      Employee referenceEmployee = Employee.NewObject();
      referenceEmployee.Name = "Employee";

      Computer computer = Computer.NewObject();
      computer.SerialNumber = "12345";
      CheckDefaultValueAndValueAfterSet(computer, referenceDateTime, referenceEmployee);
      CheckValueAfterCommitAndRollback(computer, referenceDateTime, referenceEmployee);
      CheckValueInParallelRootTransaction(computer, referenceEmployee);

      computer.Delete();
      referenceEmployee.Delete();
      TestableClientTransaction.Commit();
    }

    [Test]
    public void Commit_Rollback_ExistingObject ()
    {
      DateTime referenceDateTime = DateTime.Now;
      Employee referenceEmployee = DomainObjectIDs.Employee1.GetObject<Employee>();

      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer>();
      CheckDefaultValueAndValueAfterSet(computer, referenceDateTime, referenceEmployee);

      TestableClientTransaction.Rollback();
      Assert.That(computer.Int32TransactionProperty, Is.EqualTo(0));
      Assert.That(computer.DateTimeTransactionProperty, Is.EqualTo(new DateTime()));
      Assert.That(computer.EmployeeTransactionProperty, Is.Null);

      computer.Int32TransactionProperty = 5;
      computer.DateTimeTransactionProperty = referenceDateTime;
      computer.EmployeeTransactionProperty = referenceEmployee;

      using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
      {
        CheckValueAfterCommitAndRollback(computer, referenceDateTime, referenceEmployee);
      }
      CheckValueInParallelRootTransaction(computer, referenceEmployee);
    }

    [Test]
    public void Commit_Rollback_SubtransactionExistingObject ()
    {
      DateTime referenceDateTime = DateTime.MinValue;
      DateTime referenceDateTime2 = DateTime.MaxValue;
      Employee referenceEmployee = DomainObjectIDs.Employee1.GetObject<Employee>();
      Employee referenceEmployee2 = DomainObjectIDs.Employee2.GetObject<Employee>();

      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer>();
      computer.Int32TransactionProperty = 5;
      computer.DateTimeTransactionProperty = referenceDateTime;
      computer.EmployeeTransactionProperty = referenceEmployee;

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        CheckPropertiesAfterSet(computer, referenceDateTime, referenceEmployee);

        computer.Int32TransactionProperty = 6;
        computer.DateTimeTransactionProperty = referenceDateTime2;
        computer.EmployeeTransactionProperty = referenceEmployee2;

        ClientTransaction.Current.Commit();
      }

      Assert.That(computer.Int32TransactionProperty, Is.EqualTo(6));
      Assert.That(computer.DateTimeTransactionProperty, Is.EqualTo(referenceDateTime2));
      Assert.That(computer.EmployeeTransactionProperty, Is.SameAs(referenceEmployee2));
      Assert.That(computer.EmployeeTransactionProperty.ComputerTransactionProperty, Is.SameAs(computer));
      Assert.That(referenceEmployee.ComputerTransactionProperty, Is.Null);
    }

    [Test]
    public void Commit_Rollback_SubtransactionNewObject ()
    {
      DateTime referenceDateTime = DateTime.Now;
      Employee referenceEmployee = DomainObjectIDs.Employee1.GetObject<Employee>();

      Computer computer;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        computer = Computer.NewObject();
        computer.Int32TransactionProperty = 5;
        computer.DateTimeTransactionProperty = referenceDateTime;
        computer.EmployeeTransactionProperty = referenceEmployee;
        CheckPropertiesAfterSet(computer, referenceDateTime, referenceEmployee);

        ClientTransaction.Current.Commit();
      }

      CheckPropertiesAfterSet(computer, referenceDateTime, referenceEmployee);
    }

    private void CheckPropertiesAfterSet (Computer computer, DateTime referenceDateTime, Employee referenceEmployee)
    {
      Assert.That(computer.Int32TransactionProperty, Is.EqualTo(5));
      Assert.That(computer.DateTimeTransactionProperty, Is.EqualTo(referenceDateTime));
      Assert.That(computer.EmployeeTransactionProperty, Is.SameAs(referenceEmployee));
      Assert.That(computer.EmployeeTransactionProperty.ComputerTransactionProperty, Is.SameAs(computer));
    }

    private void CheckDefaultValueAndValueAfterSet (Computer computer, DateTime referenceDateTime, Employee referenceEmployee)
    {
      Assert.That(computer.Int32TransactionProperty, Is.EqualTo(0));
      Assert.That(computer.DateTimeTransactionProperty, Is.EqualTo(new DateTime()));
      Assert.That(computer.EmployeeTransactionProperty, Is.Null);

      computer.Int32TransactionProperty = 5;
      computer.DateTimeTransactionProperty = referenceDateTime;
      computer.EmployeeTransactionProperty = referenceEmployee;

      CheckPropertiesAfterSet(computer, referenceDateTime, referenceEmployee);
    }

    private void CheckValueAfterCommitAndRollback (Computer computer, DateTime referenceDateTime, Employee referenceEmployee)
    {
      TestableClientTransaction.Commit();
      CheckPropertiesAfterSet(computer, referenceDateTime, referenceEmployee);

      TestableClientTransaction.Rollback();
      CheckPropertiesAfterSet(computer, referenceDateTime, referenceEmployee);
    }

    private void CheckValueInParallelRootTransaction (Computer computer, Employee referenceEmployee)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Computer sameComputer = computer.ID.GetObject<Computer>();
        Employee sameReferenceEmployee = referenceEmployee.ID.GetObject<Employee>();
        Assert.That(sameComputer.Int32TransactionProperty, Is.EqualTo(0));
        Assert.That(sameComputer.DateTimeTransactionProperty, Is.EqualTo(new DateTime()));
        Assert.That(sameComputer.EmployeeTransactionProperty, Is.Null);
        Assert.That(sameReferenceEmployee.ComputerTransactionProperty, Is.Null);
      }
    }
  }
}
