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
  public class VirtualObjectRelationPropertyWriteTest : ReadOnlyTransactionsTestBase
  {
    private Order _order1;
    private OrderTicket _orderTicket1;
    private OrderTicket _orderTicket2;
    private OrderTicket _orderTicket3;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.Order1.GetObject<Order> ());
      _orderTicket1 = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ());
      _orderTicket2 = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.OrderTicket2.GetObject<OrderTicket> ());
      _orderTicket3 = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.OrderTicket3.GetObject<OrderTicket> ());

      ExecuteInWriteableSubTransaction (() => _order1.OrderTicket = _orderTicket2);
      ExecuteInWriteableSubTransaction (() => _orderTicket3.Order.EnsureDataAvailable());
    }

    [Test]
    public void RelationSetInReadOnlyRootTransaction_IsForbidden ()
    {
      CheckProperty (ReadOnlyRootTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (ReadOnlyMiddleTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (WriteableSubTransaction, _order1, o => o.OrderTicket, _orderTicket2, _orderTicket1);

      CheckForbidden (() => ExecuteInReadOnlyRootTransaction (() => _order1.OrderTicket = _orderTicket3), "RelationChanging");

      CheckProperty (ReadOnlyRootTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (ReadOnlyMiddleTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (WriteableSubTransaction, _order1, o => o.OrderTicket, _orderTicket2, _orderTicket1);
    }

    [Test]
    public void RelationSetInReadOnlyMiddleTransaction_IsForbidden ()
    {
      CheckProperty (ReadOnlyRootTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (ReadOnlyMiddleTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (WriteableSubTransaction, _order1, o => o.OrderTicket, _orderTicket2, _orderTicket1);

      CheckForbidden (() => ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderTicket = _orderTicket3), "RelationChanging");

      CheckProperty (ReadOnlyRootTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (ReadOnlyMiddleTransaction, _order1, o => o.OrderTicket, _orderTicket1, _orderTicket1);
      CheckProperty (WriteableSubTransaction, _order1, o => o.OrderTicket, _orderTicket2, _orderTicket1);
    }
  }
}