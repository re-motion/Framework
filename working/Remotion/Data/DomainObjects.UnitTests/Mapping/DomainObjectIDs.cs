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

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public sealed class DomainObjectIDs
  {
    // OrderTicket: OrderTicket1
    // OrderItems: OrderItem1, OrderItem2
    // Customer: Customer1
    // Official: Official1
    // OrderNumber: 1
    public readonly ObjectID Order1 = new ObjectID("Order", new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}"));


    // Order: Order1
    // Product: Mainboard
    public readonly ObjectID OrderItem1 = new ObjectID("OrderItem", new Guid ("{2F4D42C7-7FFA-490d-BFCD-A9101BBF4E1A}"));


    // Order: Order1
    public readonly ObjectID OrderTicket1 = new ObjectID("OrderTicket", new Guid ("{058EF259-F9CD-4cb1-85E5-5C05119AB596}"));
  }
}