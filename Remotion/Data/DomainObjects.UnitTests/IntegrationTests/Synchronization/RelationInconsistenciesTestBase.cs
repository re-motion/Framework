using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Synchronization
{
  public class RelationInconsistenciesTestBase : ClientTransactionBaseTest
  {
    protected ObjectID CreateCompanyAndSetIndustrialSectorInOtherTransaction (ObjectID industrialSectorID)
    {
      return RelationInconcsistenciesTestHelper.CreateObjectAndSetRelationInOtherTransaction<Company, IndustrialSector> (industrialSectorID, (c, s) =>
      {
        c.IndustrialSector = s;
        c.Ceo = Ceo.NewObject();
      });
    }

    protected void SetIndustrialSectorInOtherTransaction (ObjectID companyID, ObjectID industrialSectorID)
    {
      RelationInconcsistenciesTestHelper.SetRelationInOtherTransaction<Company, IndustrialSector> (companyID, industrialSectorID, (c, s) => c.IndustrialSector = s);
    }

    protected ObjectID CreateComputerAndSetEmployeeInOtherTransaction (ObjectID employeeID)
    {
      return RelationInconcsistenciesTestHelper.CreateObjectAndSetRelationInOtherTransaction<Computer, Employee> (employeeID, (c, e) => c.Employee = e);
    }

    protected void SetEmployeeInOtherTransaction (ObjectID computerID, ObjectID employeeID)
    {
      RelationInconcsistenciesTestHelper.SetRelationInOtherTransaction<Computer, Employee> (computerID, employeeID, (c, e) => c.Employee = e);
    }

    protected void CheckSyncState<TOriginating, TRelated> (
        TOriginating originating,
        Expression<Func<TOriginating, TRelated>> propertyAccessExpression,
        bool expectedState)
        where TOriginating: DomainObject
    {
      Assert.That (
          BidirectionalRelationSyncService.IsSynchronized (ClientTransaction.Current, RelationEndPointID.Resolve (originating, propertyAccessExpression)),
          Is.EqualTo (expectedState));
    }

    protected void CheckActionWorks (Action action)
    {
      action ();
    }

    protected void CheckActionThrows<TException> (Action action, string expectedMessageFormatString, params object[] formatArgs) where TException : Exception
    {
      var hadException = false;
      try
      {
        action ();
      }
      catch (Exception ex)
      {
        hadException = true;
        Assert.That (ex, Is.TypeOf (typeof (TException)));
        var expectedMessage = String.Format (expectedMessageFormatString, formatArgs);
        Assert.That (
            ex.Message, 
            Is.StringContaining(expectedMessage), 
            "Expected: " + expectedMessage + Environment.NewLine + "Was: " + ex.Message);
      }

      if (!hadException)
        Assert.Fail ("Expected " + typeof (TException).Name);
    }

    protected Company CreateCompanyInDatabaseAndLoad ()
    {
      ObjectID objectID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var company = Company.NewObject ();
        company.Ceo = Ceo.NewObject ();
        ClientTransaction.Current.Commit ();
        objectID = company.ID;
      }
      return objectID.GetObject<Company> ();
    }

    protected IndustrialSector CreateIndustrialSectorInDatabaseAndLoad ()
    {
      ObjectID objectID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        IndustrialSector industrialSector = IndustrialSector.NewObject ();
        Company oldCompany = Company.NewObject ();
        oldCompany.Ceo = Ceo.NewObject ();
        industrialSector.Companies.Add (oldCompany);
        objectID = industrialSector.ID;

        ClientTransaction.Current.Commit ();
      }
      return objectID.GetObject<IndustrialSector> ();
    }

    protected T CreateObjectInDatabaseAndLoad<T> () where T : DomainObject
    {
      ObjectID objectID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var domainObject = LifetimeService.NewObject (ClientTransaction.Current, typeof (T), ParamList.Empty);
        ClientTransaction.Current.Commit ();
        objectID = domainObject.ID;
      }
      return (T) LifetimeService.GetObject (ClientTransaction.Current, objectID, false);
    }

    protected void PrepareInconsistentState_OneMany_ObjectIncluded (out Company company, out IndustrialSector industrialSector)
    {
      SetDatabaseModifyable ();

      company = CreateCompanyInDatabaseAndLoad ();
      Assert.That (company.IndustrialSector, Is.Null);

      industrialSector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector> ();

      SetIndustrialSectorInOtherTransaction (company.ID, industrialSector.ID);

      // Resolve virtual end point - the database says that company points to industrialSector, but the transaction says it points to null!
      industrialSector.Companies.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (industrialSector.Companies, Has.Member(company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);
    }

    protected void PrepareInconsistentState_OneMany_ObjectNotIncluded (out Company company, out IndustrialSector industrialSector)
    {
      SetDatabaseModifyable ();

      var companyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (DomainObjectIDs.IndustrialSector1);
      company = companyID.GetObject<Company> ();

      Assert.That (company.Properties[typeof (Company), "IndustrialSector"].GetRelatedObjectID (), Is.EqualTo (DomainObjectIDs.IndustrialSector1));

      SetIndustrialSectorInOtherTransaction (company.ID, DomainObjectIDs.IndustrialSector2);

      industrialSector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector> ();

      // Resolve virtual end point - the database says that company does not point to IndustrialSector1, but the transaction says it does!
      industrialSector.Companies.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, Has.No.Member (company));
      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);
    }

    protected void PrepareInconsistentState_OneOne_ObjectReturned_ThatLocallyPointsSomewhereElse (out Computer computer, out Employee nonMatchingEmployee, out Employee matchingEmployee)
    {
      SetDatabaseModifyable ();

      computer = CreateComputerAndSetEmployeeInOtherTransaction (DomainObjectIDs.Employee2).GetObject<Computer> ();
      Assert.That (computer.Employee.ID, Is.EqualTo (DomainObjectIDs.Employee2));

      nonMatchingEmployee = DomainObjectIDs.Employee1.GetObject<Employee> ();

      SetEmployeeInOtherTransaction (computer.ID, nonMatchingEmployee.ID);

      // Resolve virtual end point - the database says that computer points to employee, but the transaction says computer points to Employee2!
      Dev.Null = nonMatchingEmployee.Computer;
      matchingEmployee = computer.Employee;
    }

    protected void PrepareInconsistentState_OneOne_ObjectReturned_ThatLocallyPointsToNull (out Computer computer, out Employee nonMatchingEmployee)
    {
      SetDatabaseModifyable ();

      computer = CreateObjectInDatabaseAndLoad<Computer> ();
      Assert.That (computer.Employee, Is.Null);

      nonMatchingEmployee = DomainObjectIDs.Employee1.GetObject<Employee> ();

      SetEmployeeInOtherTransaction (computer.ID, nonMatchingEmployee.ID);

      // Resolve virtual end point - the database says that computer points to employee, but the transaction says computer points to null!
      Dev.Null = nonMatchingEmployee.Computer;
    }

    protected void PrepareInconsistentState_OneOne_ObjectNotReturned_ThatLocallyPointsToHere (out Computer computer, out Employee employee, out Employee employee2)
    {
      SetDatabaseModifyable ();

      employee = DomainObjectIDs.Employee1.GetObject<Employee> ();
      employee2 = DomainObjectIDs.Employee2.GetObject<Employee> ();

      computer = CreateComputerAndSetEmployeeInOtherTransaction (employee2.ID).GetObject<Computer> ();
      Assert.That (computer.Employee, Is.SameAs (employee2));
      
      // 1:1 relations automatically cause virtual end-points to be marked loaded when the foreign key object is loaded, so unload the virtual side
      UnloadService.UnloadVirtualEndPoint (ClientTransaction.Current, RelationEndPointID.Resolve (employee2, e => e.Computer));

      SetEmployeeInOtherTransaction (computer.ID, DomainObjectIDs.Employee1);

      // Resolve virtual end point - the database says that computer points to employee1, but the transaction says computer points to employee2!
      Dev.Null = employee2.Computer;
      // Resolve virtual end point - the database says that computer points to employee1, but the transaction says computer points to employee2!
      Dev.Null = employee.Computer;
    }

    protected void PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNull (out Employee employee, out Computer computer)
    {
      SetDatabaseModifyable ();

      employee = DomainObjectIDs.Employee1.GetObject<Employee> ();
      // Employee has no computer
      Assert.That (employee.Computer, Is.Null);

      // This computer points to employee => conflict in the transaction
      computer = CreateComputerAndSetEmployeeInOtherTransaction (employee.ID).GetObject<Computer> ();
      Assert.That (computer.Employee, Is.SameAs (employee));
    }

    protected void PrepareInconsistentState_InconsistentForeignKeyLoaded_VirtualSideAlreadyNonNull (out Employee employee, out Computer computer, out Computer computer2)
    {
      SetDatabaseModifyable ();

      employee = DomainObjectIDs.Employee1.GetObject<Employee> ();

      // This computer points to employee
      computer = CreateComputerAndSetEmployeeInOtherTransaction (employee.ID).GetObject<Computer> ();
      Assert.That (computer.Employee, Is.SameAs (employee));

      // This computer _also_ points to employee => conflict in the transaction, 1:1 relation has two real object end-points
      computer2 = CreateComputerAndSetEmployeeInOtherTransaction (employee.ID).GetObject<Computer> ();
      Assert.That (computer2.Employee, Is.SameAs (employee));
    }
  }
}