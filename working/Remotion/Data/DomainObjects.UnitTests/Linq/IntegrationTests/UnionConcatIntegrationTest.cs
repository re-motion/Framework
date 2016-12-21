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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class UnionConcatIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void UnionQuery ()
    {
      var computers =
          QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "Mainboard")
              .Union (QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "CPU Fan"))
              .Union (QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "Mainboard"));

      CheckQueryResult (computers, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void ConcatQuery ()
    {
      var computers =
          QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "Mainboard")
              .Concat (QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "CPU Fan"));

      CheckQueryResult (computers, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);

      var computersWithDuplicates =
          QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "Mainboard")
              .Concat (QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "CPU Fan"))
              .Concat (QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "Mainboard"));
      Assert.That (computersWithDuplicates.Count(), Is.EqualTo (3));
    }

    [Test]
    public void ThrowingQueries ()
    {
      Assert.That (
          () => QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "Mainboard").Select (oi => SomeMethod (oi.Product))
              .Union (QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "CPU Fan").Select (oi => SomeMethod (oi.Product)))
              .ToArray(),
          Throws.TypeOf<NotSupportedException>()
              .With.Message.StringContaining ("In-memory method calls are not supported when a set operation (such as Union or Concat) is used."));

      Assert.That (
          () => QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "Mainboard").Select (oi => SomeMethod (oi.Product))
              .Concat (QueryFactory.CreateLinqQuery<OrderItem>().Where (oi => oi.Product == "CPU Fan").Select (oi => SomeMethod (oi.Product)))
              .ToArray(),
          Throws.TypeOf<NotSupportedException>()
              .With.Message.StringContaining ("In-memory method calls are not supported when a set operation (such as Union or Concat) is used."));
    }

    private static string SomeMethod (string s)
    {
      throw new NotImplementedException();
    }
  }
}