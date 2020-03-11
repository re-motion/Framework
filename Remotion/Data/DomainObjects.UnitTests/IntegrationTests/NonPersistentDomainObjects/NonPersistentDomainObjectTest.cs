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
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.NonPersistentDomainObjects
{
  [TestFixture]
  public class NonPersistentDomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewObject_CreatesNewInstance ()
    {
      var orderViewModel = OrderViewModel.NewObject();

      Assert.That (orderViewModel.State.IsNew, Is.True);
    }

    [Test]
    public void DeleteObject_WithNewObject_SetsStateToInvalid ()
    {
      var orderViewModel = OrderViewModel.NewObject();
      Assert.That (orderViewModel.State.IsNew, Is.True);

      orderViewModel.Delete();

      Assert.That (orderViewModel.State.IsInvalid, Is.True);
    }

    [Test]
    public void DeleteObject_WithExistingObject_SetsStateToDeleted ()
    {
      var orderViewModel = OrderViewModel.NewObject();
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        orderViewModel.EnsureDataAvailable();
        Assert.That (orderViewModel.State.IsUnchanged, Is.True);

        orderViewModel.Delete();

        Assert.That (orderViewModel.State.IsDeleted, Is.True);
      }
    }

    [Test]
    public void SetScalarProperty_WithNewObject_SetsValueAndLeavesStateAsNew ()
    {
      var orderViewModel = OrderViewModel.NewObject();

      var property = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.OrderSum)];

      Assert.That (property.HasChanged, Is.False);
      Assert.That (property.GetValue<int>(), Is.EqualTo (0));
      Assert.That (property.GetOriginalValue<int>(), Is.EqualTo (0));

      orderViewModel.OrderSum = 42;

      Assert.That (property.HasChanged, Is.True);
      Assert.That (property.GetValue<int>(), Is.EqualTo (42));
      Assert.That (property.GetOriginalValue<int>(), Is.EqualTo (0));
      Assert.That (orderViewModel.State.IsNew, Is.True);
    }

    [Test]
    public void SetScalarProperty_WithExistingObject_SetsValueAndSetsStateToChanged ()
    {
      var orderViewModel = OrderViewModel.NewObject();
      orderViewModel.OrderSum = 13;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var property = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.OrderSum)];
        orderViewModel.EnsureDataAvailable();

        Assert.That (property.HasChanged, Is.False);
        Assert.That (property.GetValue<int>(), Is.EqualTo (13));
        Assert.That (property.GetOriginalValue<int>(), Is.EqualTo (13));

        orderViewModel.OrderSum = 42;

        Assert.That (property.HasChanged, Is.True);
        Assert.That (property.GetValue<int>(), Is.EqualTo (42));
        Assert.That (property.GetOriginalValue<int>(), Is.EqualTo (13));
        Assert.That (orderViewModel.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void GetScalarProperty_WithNewObject_GetsDefaultValue ()
    {
      var orderViewModel = OrderViewModel.NewObject();

      Assert.That (orderViewModel.OrderSum, Is.EqualTo (0));
    }

    [Test]
    public void GetScalarProperty_WithExistingObject_GetsCurrentValue ()
    {
      var orderViewModel = OrderViewModel.NewObject();
      var property = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.OrderSum)];
      property.SetValue (42);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (orderViewModel.OrderSum, Is.EqualTo (42));
      }
    }

    [Test]
    public void SetUnidirectionalRelationProperty_WithNewObject_SetsValueAndLeavesStateAsNew ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderViewModel = OrderViewModel.NewObject();

      var property = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.Object)];

      Assert.That (property.HasChanged, Is.False);
      Assert.That (property.GetValue<Order>(), Is.Null);
      Assert.That (property.GetOriginalValue<Order>(), Is.Null);

      orderViewModel.Object = order;

      Assert.That (property.HasChanged, Is.True);
      Assert.That (property.GetValue<Order>(), Is.SameAs (order));
      Assert.That (property.GetOriginalValue<Order>(), Is.Null);
      Assert.That (orderViewModel.State.IsNew, Is.True);
    }

    [Test]
    public void SetUnidirectionalRelationProperty_WithExistingObject_SetsValueAndSetsStateToChanged ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      var order2 = DomainObjectIDs.Order2.GetObject<Order>();
      var orderViewModel = OrderViewModel.NewObject();

      orderViewModel.Object = order1;
      order1.OrderNumber = 15;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var property = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.Object)];
        orderViewModel.EnsureDataAvailable();

        Assert.That (property.HasChanged, Is.False);
        Assert.That (property.GetValue<Order>(), Is.SameAs (order1));
        Assert.That (property.GetOriginalValue<Order>(), Is.EqualTo (order1));
        Assert.That (orderViewModel.State.IsUnchanged, Is.True);

        orderViewModel.Object = order2;

        Assert.That (property.HasChanged, Is.True);
        Assert.That (property.GetValue<Order>(), Is.SameAs (order2));
        Assert.That (property.GetOriginalValue<Order>(), Is.SameAs (order1));
        Assert.That (orderViewModel.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void GetUnidirectionalRelationProperty_WithNewObject_GetsDefaultValue ()
    {
      var orderViewModel = OrderViewModel.NewObject();

      Assert.That (orderViewModel.Object, Is.Null);
    }

    [Test]
    public void GetUnidirectionalRelationProperty_WithExistingObject_GetsCurrentValue ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderViewModel = OrderViewModel.NewObject();
      var property = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.Object)];

      property.SetValue (order);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (orderViewModel.Object, Is.SameAs (order));
      }
    }

    [Test]
    public void CommitChangedObject_WithExistingObject_AppliesValueAndUpdatesStateToUnchanged ()
    {
      var orderViewModel = OrderViewModel.NewObject();
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      orderViewModel.OrderSum = 42;
      orderViewModel.Object = order;

      var propertyOrderSum = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.OrderSum)];
      var propertyObject = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.Object)];

      ClientTransaction.Current.Commit();

      Assert.That (propertyOrderSum.HasChanged, Is.False);
      Assert.That (propertyOrderSum.GetValue<int>(), Is.EqualTo (42));
      Assert.That (propertyOrderSum.GetOriginalValue<int>(), Is.EqualTo (42));

      Assert.That (propertyObject.HasChanged, Is.False);
      Assert.That (propertyObject.GetValue<Order>(), Is.SameAs (order));
      Assert.That (propertyObject.GetOriginalValue<Order>(), Is.SameAs (order));

      Assert.That (orderViewModel.State.IsUnchanged, Is.True);
    }

    [Test]
    public void CommitChangedObject_WithPersistentAndNonPersistentObjectInSubTransaction_AppliesValueAndUpdatesStateToUnchanged ()
    {
      var orderViewModel = OrderViewModel.NewObject();
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          orderViewModel.OrderSum = 42;
          orderViewModel.Object = order;
          order.OrderNumber = 13;

          ClientTransaction.Current.Commit();

          Assert.That (orderViewModel.State.IsUnchanged, Is.True);
          Assert.That (order.State.IsUnchanged, Is.True);
        }

        var propertyOrderSum = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.OrderSum)];
        var propertyObject = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.Object)];
        var propertyOrderNumber = order.Properties[typeof (Order), nameof (Order.OrderNumber)];

        Assert.That (propertyOrderSum.HasChanged, Is.True);
        Assert.That (propertyOrderSum.GetValue<int>(), Is.EqualTo (42));
        Assert.That (propertyOrderSum.GetOriginalValue<int>(), Is.EqualTo (0));

        Assert.That (propertyObject.HasChanged, Is.True);
        Assert.That (propertyObject.GetValue<Order>(), Is.SameAs (order));
        Assert.That (propertyObject.GetOriginalValue<Order>(), Is.Null);

        Assert.That (orderViewModel.State.IsChanged, Is.True);

        Assert.That (propertyOrderNumber.HasChanged, Is.True);
        Assert.That (propertyOrderNumber.GetValue<int>(), Is.EqualTo (13));
        Assert.That (propertyOrderNumber.GetOriginalValue<int>(), Is.EqualTo (1));

        Assert.That (order.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void CommitChangedObject_WithPersistentAndNonPersistentObjectInRootTransaction_AppliesValueAndUpdatesStateToUnchanged ()
    {
      SetDatabaseModifyable();

      var orderViewModel = OrderViewModel.NewObject();
      var order = DomainObjectIDs.Order1.GetObject<Order>();

      orderViewModel.OrderSum = 42;
      orderViewModel.Object = order;
      order.OrderNumber = 13;

      ClientTransaction.Current.Commit();

      var propertyOrderSum = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.OrderSum)];
      var propertyObject = orderViewModel.Properties[typeof (OrderViewModel), nameof (OrderViewModel.Object)];
      var propertyOrderNumber = order.Properties[typeof (Order), nameof (Order.OrderNumber)];

      Assert.That (propertyOrderSum.HasChanged, Is.False);
      Assert.That (propertyOrderSum.GetValue<int>(), Is.EqualTo (42));
      Assert.That (propertyOrderSum.GetOriginalValue<int>(), Is.EqualTo (42));

      Assert.That (propertyObject.HasChanged, Is.False);
      Assert.That (propertyObject.GetValue<Order>(), Is.SameAs (order));
      Assert.That (propertyObject.GetOriginalValue<Order>(), Is.SameAs (order));

      Assert.That (orderViewModel.State.IsUnchanged, Is.True);

      Assert.That (propertyOrderNumber.HasChanged, Is.False);
      Assert.That (propertyOrderNumber.GetValue<int>(), Is.EqualTo (13));
      Assert.That (propertyOrderNumber.GetOriginalValue<int>(), Is.EqualTo (13));

      Assert.That (order.State.IsUnchanged, Is.True);
    }
  }
}