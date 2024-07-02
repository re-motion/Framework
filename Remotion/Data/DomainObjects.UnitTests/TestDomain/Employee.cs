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

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Employee : TestDomainBase
  {
    public static Employee NewObject ()
    {
      return NewObject<Employee>();
    }

    protected Employee ()
    {
    }

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation("Supervisor")]
    public abstract ObjectList<Employee> Subordinates { get; }

    [DBBidirectionalRelation("Subordinates")]
    public abstract Employee Supervisor { get; set; }

    [DBBidirectionalRelation("Employee")]
    public Computer Computer
    {
      get { return Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"].GetValue<Computer>(); }
      set { Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"].SetValue(value); }
    }

    [StorageClassTransaction]
    [DBBidirectionalRelation("EmployeeTransactionProperty")]
    public abstract Computer ComputerTransactionProperty { get; set; }

    public void DeleteWithSubordinates ()
    {
      foreach (Employee employee in Subordinates.Clone())
        employee.Delete();

      this.Delete();
    }
  }
}
