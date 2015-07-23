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
  public class AddToOneToManyRelationWithLazyLoadTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public AddToOneToManyRelationWithLazyLoadTest ()
    {
    }

    // methods and properties

    [Test]
    public void Insert ()
    {
      Employee newSupervisor = DomainObjectIDs.Employee1.GetObject<Employee> ();
      Employee subordinate = DomainObjectIDs.Employee3.GetObject<Employee> ();

      int countBeforeInsert = newSupervisor.Subordinates.Count;

      newSupervisor.Subordinates.Insert (0, subordinate);

      Assert.That (newSupervisor.Subordinates.Count, Is.EqualTo (countBeforeInsert + 1));
      Assert.That (newSupervisor.Subordinates.IndexOf (subordinate), Is.EqualTo (0));
      Assert.That (subordinate.Supervisor, Is.SameAs (newSupervisor));

      Employee oldSupervisor = DomainObjectIDs.Employee2.GetObject<Employee> ();
      Assert.That (oldSupervisor.Subordinates.ContainsObject (subordinate), Is.False);
    }
  }
}
