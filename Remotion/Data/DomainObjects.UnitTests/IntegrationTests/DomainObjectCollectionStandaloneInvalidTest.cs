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
  public class DomainObjectCollectionStandaloneInvalidTest : ClientTransactionBaseTest
  {
    [Test]
    public void Remove_WithInvalidObject ()
    {
      var collection = new DomainObjectCollection ();
      Customer customer = Customer.NewObject ();
      collection.Add (customer);
      customer.Delete ();
      Assert.That (customer.State, Is.EqualTo (StateType.Invalid));

      //The next line does not throw an ObjectInvalidException:
      collection.Remove (customer);

      Assert.That (collection, Is.Empty);
    }

    [Test]
    public void Clear_WithInvalidObject ()
    {
      var collection = new DomainObjectCollection ();
      Customer customer = Customer.NewObject ();
      collection.Add (customer);

      customer.Delete ();
      Assert.That (customer.State, Is.EqualTo (StateType.Invalid));

      //The next line does not throw an exception:
      collection.Clear ();

      Assert.That (collection, Is.Empty);
    }


  }
}