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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class DomainObjectTransactionContextIndexerTest : ClientTransactionBaseTest
  {
    [Test]
    public void Item()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order> ();
      var tx = order.RootTransaction.CreateSubTransaction();
      var indexer = new DomainObjectTransactionContextIndexer (order, false);

      var item = indexer[tx];
      Assert.That (item, Is.InstanceOf (typeof (DomainObjectTransactionContext)));
      Assert.That (((DomainObjectTransactionContext) item).DomainObject, Is.SameAs (order));
      Assert.That (item.ClientTransaction, Is.SameAs (tx));
    }

    [Test]
    public void Item_WhileEventIsExecuting ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order> ();
      var tx = order.RootTransaction.CreateSubTransaction();
      var indexer = new DomainObjectTransactionContextIndexer (order, true);

      var item = indexer[tx];
      Assert.That (item, Is.InstanceOf (typeof (InitializedEventDomainObjectTransactionContextDecorator)));
      Assert.That (item.ClientTransaction, Is.SameAs (tx));
    }
  }
}
