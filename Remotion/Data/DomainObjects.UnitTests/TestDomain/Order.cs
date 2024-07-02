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
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Order : TestDomainBase, IOrder
  {
    public static Order NewObject ()
    {
      return NewObject<Order>();
    }

    [DBColumn("OrderNo")]
    public abstract int OrderNumber { get; set; }

    [StorageClassNone]
    public int RedirectedOrderNumber
    {
      [LinqPropertyRedirection(typeof(Order), "OrderNumber")]
      get { return OrderNumber; }
    }

    [StorageClassNone]
    public int RedirectedRedirectedOrderNumber
    {
      [LinqPropertyRedirection(typeof(Order), "RedirectedOrderNumber")]
      get { return RedirectedOrderNumber; }
    }

    public abstract DateTime DeliveryDate { get; set; }

    [Mandatory]
    [DBBidirectionalRelation("Orders")]
    public abstract Official Official { get; set; }

    [Mandatory]
    [DBBidirectionalRelation("Order")]
    public abstract OrderTicket OrderTicket { get; set; }

    [StorageClassNone]
    public OrderTicket RedirectedOrderTicket
    {
      [LinqPropertyRedirection(typeof(Order), "OrderTicket")]
      get { return OrderTicket; }
    }

    [Mandatory]
    [DBBidirectionalRelation("Orders")]
    public abstract Customer Customer { get; set; }

    [Mandatory]
    [DBBidirectionalRelation("Order")]
    public virtual ObjectList<OrderItem> OrderItems { get; set; }

    [StorageClassNone]
    public ObjectList<OrderItem> RedirectedOrderItems
    {
      [LinqPropertyRedirection(typeof(Order), "OrderItems")]
      get { return OrderItems; }
    }

    public void PreparePropertyAccess (string propertyName)
    {
      CurrentPropertyManager.PreparePropertyAccess(propertyName);
    }

    public void PropertyAccessFinished ()
    {
      CurrentPropertyManager.PropertyAccessFinished();
    }

    [StorageClassNone]
    public new PropertyAccessor CurrentProperty
    {
      get { return base.CurrentProperty; }
    }

    [StorageClassNone]
    public Customer OriginalCustomer
    {
      get { return Properties[typeof(Order), "Customer"].GetOriginalValue<Customer>(); }
    }

    [StorageClassNone]
    public virtual int NotInMapping
    {
      get { return CurrentProperty.GetValue<int>(); }
      set { CurrentProperty.SetValue(value); }
    }

    [StorageClassNone]
    public virtual OrderTicket NotInMappingRelated
    {
      get { return CurrentProperty.GetValue<OrderTicket>(); }
      set { CurrentProperty.SetValue(value); }
    }

    [StorageClassNone]
    public virtual ObjectList<OrderItem> NotInMappingRelatedObjects
    {
      get { return CurrentProperty.GetValue<ObjectList<OrderItem>>(); }
    }
  }
}
