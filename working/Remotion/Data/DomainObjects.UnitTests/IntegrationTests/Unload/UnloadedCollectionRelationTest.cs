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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadedCollectionRelationTest : ClientTransactionBaseTest
  {
    private OrderItem _newOrderItem;
    private Order _orderWithUnloadedCollection;
    private int _originalOrderItemCount;

    public override void SetUp ()
    {
      base.SetUp ();

      _newOrderItem = OrderItem.NewObject();
      _orderWithUnloadedCollection = DomainObjectIDs.Order1.GetObject<Order> ();
      _originalOrderItemCount = _orderWithUnloadedCollection.OrderItems.Count;
      UnloadService.UnloadVirtualEndPoint (TestableClientTransaction, _orderWithUnloadedCollection.OrderItems.AssociatedEndPointID);
    }

    [Test]
    public void Insert ()
    {
      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.False);

      _orderWithUnloadedCollection.OrderItems.Insert (0, _newOrderItem);

      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.True);
      Assert.That (_orderWithUnloadedCollection.OrderItems.Count, Is.EqualTo (_originalOrderItemCount + 1));
      Assert.That (_orderWithUnloadedCollection.OrderItems[0], Is.SameAs (_newOrderItem));
    }

    [Test]
    public void Remove ()
    {
      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.False);

      _orderWithUnloadedCollection.OrderItems.Remove (_orderWithUnloadedCollection.OrderItems[0]);

      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.True);
      Assert.That (_orderWithUnloadedCollection.OrderItems.Count, Is.EqualTo (_originalOrderItemCount - 1));
    }

    [Test]
    public void Delete ()
    {
      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.False);

      _orderWithUnloadedCollection.Delete();

      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.True);
      Assert.That (_orderWithUnloadedCollection.OrderItems.Count, Is.EqualTo (0));
    }

    [Test]
    public void Replace ()
    {
      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.False);

      _orderWithUnloadedCollection.OrderItems[0] = _newOrderItem;

      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.True);
      Assert.That (_orderWithUnloadedCollection.OrderItems[0], Is.SameAs (_newOrderItem));
    }

    [Test]
    public void SetCollection ()
    {
      var newOrderItems = new ObjectList<OrderItem> (new[] { _newOrderItem });
      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.False);

      _orderWithUnloadedCollection.OrderItems = newOrderItems;

      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.True);
      Assert.That (_orderWithUnloadedCollection.OrderItems, Is.SameAs (newOrderItems));
    }

    [Test]
    public void GetEnumerator ()
    {
      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.False);

      using (var enumerator = _orderWithUnloadedCollection.OrderItems.GetEnumerator ())
      {
        Assert.That (enumerator.MoveNext (), Is.True);
        Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.True);
      }
    }

    [Test]
    public void GetOriginalValue ()
    {
      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.False);

      var originalOrderItems = _orderWithUnloadedCollection.Properties[typeof (Order), "OrderItems"].GetOriginalValue<ObjectList<OrderItem>>();

      Assert.That (_orderWithUnloadedCollection.OrderItems.IsDataComplete, Is.True);
      Assert.That (originalOrderItems.Count, Is.GreaterThan (0));
    }
  }
}