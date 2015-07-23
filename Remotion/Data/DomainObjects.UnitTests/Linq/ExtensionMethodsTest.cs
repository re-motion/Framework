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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class ExtensionMethodsTest : ClientTransactionBaseTest
  {
    [Test]
    public void ToObjectList ()
    {
      IQueryable<Order> queryable = from o in QueryFactory.CreateLinqQuery<Order>()
                                    where o.OrderNumber == 1 || o.ID == DomainObjectIDs.Order3 
                                    select o;
      ObjectList<Order> list = queryable.ToObjectList();
      Assert.That (list, Is.EquivalentTo (new[] { DomainObjectIDs.Order1.GetObject<Order> (), DomainObjectIDs.Order3.GetObject<Order> () }));
    }
  }
}
