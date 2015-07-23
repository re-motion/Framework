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

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.ForbiddenOperations
{
  [TestFixture]
  public class VirtualCollectionRelationPropertyWriteTest : ReadOnlyTransactionsTestBase
  {
    private Order _order1;
    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;
    private OrderItem _orderItem4;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.Order1.GetObject<Order> ());
      _orderItem1 = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>());
      _orderItem2 = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.OrderItem2.GetObject<OrderItem>());

      _orderItem3 = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.OrderItem3.GetObject<OrderItem>());
      _orderItem4 = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.OrderItem4.GetObject<OrderItem>());

      ExecuteInWriteableSubTransaction (() => _order1.OrderItems.Add (_orderItem3));
      ExecuteInWriteableSubTransaction (() => _orderItem4.Order.EnsureDataAvailable ());
    }

    [Test]
    public void RelationSetInReadOnlyRootTransaction_IsForbidden ()
    {
      CheckPropertyEquivalent (ReadOnlyRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ReadOnlyMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (WriteableSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });

      CheckForbidden (() => ExecuteInReadOnlyRootTransaction (() => _order1.OrderItems.Add (_orderItem4)), "RelationChanging");
      CheckForbidden (() => ExecuteInReadOnlyRootTransaction (() => _order1.OrderItems.Insert (0, _orderItem4)), "RelationChanging");
      CheckForbidden (() => ExecuteInReadOnlyRootTransaction (() => _order1.OrderItems.Remove (_orderItem1)), "RelationChanging");
      CheckForbidden (() => ExecuteInReadOnlyRootTransaction (() => _order1.OrderItems[0] = _orderItem4), "RelationChanging");
      CheckForbidden (() => ExecuteInReadOnlyRootTransaction (() => _order1.OrderItems.Clear ()), "RelationChanging");
      CheckForbidden (() => ExecuteInReadOnlyRootTransaction (() => _order1.OrderItems = new ObjectList<OrderItem>()), "RelationChanging");

      CheckPropertyEquivalent (ReadOnlyRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ReadOnlyMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (WriteableSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });
    }

    [Test]
    public void RelationSetInReadOnlyMiddleTransaction_IsForbidden ()
    {
      CheckPropertyEquivalent (ReadOnlyRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ReadOnlyMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (WriteableSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });

      CheckForbidden (() => ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderItems.Add (_orderItem4)), "RelationChanging");
      CheckForbidden (() => ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderItems.Insert (0, _orderItem4)), "RelationChanging");
      CheckForbidden (() => ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderItems.Remove (_orderItem1)), "RelationChanging");
      CheckForbidden (() => ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderItems[0] = _orderItem4), "RelationChanging");
      CheckForbidden (() => ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderItems.Clear ()), "RelationChanging");
      CheckForbidden (() => ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderItems = new ObjectList<OrderItem> ()), "RelationChanging");

      CheckPropertyEquivalent (ReadOnlyRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ReadOnlyMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (WriteableSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });
    }
  }
}