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
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class OrderTicket : TestDomainBase
  {
    public static OrderTicket NewObject ()
    {
      return NewObject<OrderTicket>();
    }

    // New OrderTickets need an associated order for correct initialization.
    public static OrderTicket NewObject (Order order)
    {
      OrderTicket orderTicket = NewObject<OrderTicket>(ParamList.Create(order));
      return orderTicket;
    }

    protected OrderTicket ()
    {
    }

    protected OrderTicket (Order order)
    {
      ArgumentUtility.CheckNotNull("order", order);
      Order = order;
    }

    [StringProperty(IsNullable = false, MaximumLength = 255)]
    public abstract string FileName { get; set; }

    [DBBidirectionalRelation("OrderTicket", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Order Order { get; set; }

    [StorageClassTransaction]
    public abstract int Int32TransactionProperty { get; set; }
  }
}
