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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class DomainObjectWithHierarchyTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectWithHierarchyTest ()
    {
    }

    // methods and properties

    [Test]
    public void GetObjectInHierarchy ()
    {
      Employee employee = DomainObjectIDs.Employee1.GetObject<Employee> ();

      Assert.That (employee, Is.Not.Null);
      Assert.That (employee.ID, Is.EqualTo (DomainObjectIDs.Employee1));
    }

    [Test]
    public void GetChildren ()
    {
      Employee employee = DomainObjectIDs.Employee1.GetObject<Employee> ();
      DomainObjectCollection subordinates = employee.Subordinates;

      Assert.That (subordinates, Is.Not.Null);
      Assert.That (subordinates.Count, Is.EqualTo (2));
      Assert.That (subordinates[DomainObjectIDs.Employee4], Is.Not.Null);
      Assert.That (subordinates[DomainObjectIDs.Employee5], Is.Not.Null);
    }

    [Test]
    public void GetChildrenTwice ()
    {
      Employee employee = DomainObjectIDs.Employee1.GetObject<Employee> ();
      DomainObjectCollection subordinates = employee.Subordinates;

      Assert.That (ReferenceEquals (subordinates, employee.Subordinates), Is.True);
    }

    [Test]
    public void GetParent ()
    {
      Employee employee = DomainObjectIDs.Employee4.GetObject<Employee> ();
      Employee supervisor = employee.Supervisor;

      Assert.That (supervisor, Is.Not.Null);
      Assert.That (supervisor.ID, Is.EqualTo (DomainObjectIDs.Employee1));
    }

    [Test]
    public void GetParentTwice ()
    {
      Employee employee1 = DomainObjectIDs.Employee4.GetObject<Employee> ();
      Employee employee2 = DomainObjectIDs.Employee5.GetObject<Employee> ();

      Assert.That (ReferenceEquals (employee1.Supervisor, employee2.Supervisor), Is.True);
    }
  }
}
