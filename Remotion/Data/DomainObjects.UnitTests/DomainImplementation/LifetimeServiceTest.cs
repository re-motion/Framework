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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation
{
  [TestFixture]
  public class LifetimeServiceTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewObject_InvalidType ()
    {
      Assert.That(
          () => LifetimeService.NewObject(TestableClientTransaction, typeof(object), ParamList.Empty),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo("Mapping does not contain type 'System.Object'."));
    }

    [Test]
    public void NewObject_NoCtorArgs ()
    {
      var instance = (Order)LifetimeService.NewObject(TestableClientTransaction, typeof(Order), ParamList.Empty);
      Assert.That(instance, Is.Not.Null);
      Assert.That(instance.CtorCalled, Is.True);
    }

    [Test]
    public void NewObject_WithCtorArgs ()
    {
      var order = Order.NewObject();
      var instance = (OrderItem)LifetimeService.NewObject(TestableClientTransaction, typeof(OrderItem), ParamList.Create(order));
      Assert.That(instance, Is.Not.Null);
      Assert.That(instance.Order, Is.SameAs(order));
    }

    [Test]
    public void NewObject_WrongCtorArgs ()
    {
      Assert.That(
          () => LifetimeService.NewObject(TestableClientTransaction, typeof(OrderItem), ParamList.Create(0m)),
          Throws.InstanceOf<MissingMethodException>()
              .With.Message.EqualTo(
                  "Type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem' does not contain a constructor with the following signature: (Decimal)."));
    }

    [Test]
    public void NewObject_InitializesMixins ()
    {
      var domainObject = LifetimeService.NewObject(TestableClientTransaction, typeof(ClassWithAllDataTypes), ParamList.Empty);
      var mixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>>(domainObject);
      Assert.That(mixin, Is.Not.Null);
      Assert.That(mixin.OnDomainObjectCreatedCalled, Is.True);
      Assert.That(mixin.OnDomainObjectCreatedTx, Is.SameAs(TestableClientTransaction));
    }

    [Test]
    public void GetObject ()
    {
      var order = (Order)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Order1, false);
      Assert.That(order, Is.Not.Null);
      Assert.That(order.ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(order.CtorCalled, Is.False);
    }

    [Test]
    public void GetObject_IncludeDeleted_False ()
    {
      DomainObjectIDs.Order1.GetObject<Order>().Delete();
      Assert.That(
          () => LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Order1, false),
          Throws.InstanceOf<ObjectDeletedException>());
    }

    [Test]
    public void GetObject_IncludeDeleted_True ()
    {
      DomainObjectIDs.Order1.GetObject<Order>().Delete();
      var order = (Order)LifetimeService.GetObject(TestableClientTransaction, DomainObjectIDs.Order1, true);
      Assert.That(order, Is.Not.Null);
      Assert.That(order.ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(order.State.IsDeleted, Is.True);
    }

    [Test]
    public void GetObject_WithInvalidObject_Throws ()
    {
      var instance = Order.NewObject();
      instance.Delete();
      Assert.That(instance.State.IsInvalid, Is.True);

      Assert.That(() => LifetimeService.GetObject(TestableClientTransaction, instance.ID, false), Throws.TypeOf<ObjectInvalidException>());
      Assert.That(() => LifetimeService.GetObject(TestableClientTransaction, instance.ID, true), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void TryGetObject ()
    {
      var order = (Order)LifetimeService.TryGetObject(TestableClientTransaction, DomainObjectIDs.Order1);
      Assert.That(order, Is.Not.Null);
      Assert.That(order.ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(order.CtorCalled, Is.False);
    }

    [Test]
    public void TryGetObject_Deleted ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      order.Delete();

      var orderAgain = LifetimeService.TryGetObject(TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That(orderAgain, Is.SameAs(order));
    }

    [Test]
    public void TryGetObject_Invalid ()
    {
      var instance = Order.NewObject();
      instance.Delete();
      Assert.That(instance.State.IsInvalid, Is.True);

      var instanceAgain = LifetimeService.TryGetObject(TestableClientTransaction, instance.ID);

      Assert.That(instanceAgain, Is.SameAs(instance));
    }

    [Test]
    public void TryGetObject_NotFound ()
    {
      var id = new ObjectID(typeof(Order), Guid.NewGuid());
      Assert.That(TestableClientTransaction.IsInvalid(id), Is.False);

      var result = LifetimeService.TryGetObject(TestableClientTransaction, id);

      Assert.That(result, Is.Null);
      Assert.That(TestableClientTransaction.IsInvalid(id), Is.True);
    }

    [Test]
    public void GetObjectReference ()
    {
      var result = (DomainObject)LifetimeService.GetObjectReference(TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That(result, Is.InstanceOf(typeof(Order)));
      Assert.That(result.ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(result.State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void GetObjectReference_WithInvalidObject ()
    {
      var instance = Order.NewObject();
      instance.Delete();
      Assert.That(instance.State.IsInvalid, Is.True);

      var result = LifetimeService.GetObjectReference(TestableClientTransaction, instance.ID);

      Assert.That(result, Is.SameAs(instance));
    }

    [Test]
    public void GetObjects ()
    {
      var deletedObjectID = DomainObjectIDs.Order4;
      var deletedObject = deletedObjectID.GetObject<Order>();
      deletedObject.Delete();

      Order[] orders = LifetimeService.GetObjects<Order>(TestableClientTransaction, DomainObjectIDs.Order1, DomainObjectIDs.Order3, deletedObjectID);

      Assert.That(orders, Is.EqualTo(new[] { DomainObjectIDs.Order1.GetObject<Order>(), DomainObjectIDs.Order3.GetObject<Order>(), deletedObject }));
    }

    [Test]
    public void GetObjects_WithInvalidObject_Throws ()
    {
      var instance = Order.NewObject();
      instance.Delete();
      Assert.That(instance.State.IsInvalid, Is.True);

      Assert.That(() => LifetimeService.GetObjects<Order>(TestableClientTransaction, instance.ID, instance.ID), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void TryGetObjects ()
    {
      var notFoundObjectID = new ObjectID(typeof(Order), Guid.NewGuid());

      var deletedObjectID = DomainObjectIDs.Order4;
      var deletedObject = deletedObjectID.GetObject<Order>();
      deletedObject.Delete();

      var invalidInstance = Order.NewObject();
      invalidInstance.Delete();
      Assert.That(invalidInstance.State.IsInvalid, Is.True);

      Order[] orders = LifetimeService.TryGetObjects<Order>(
          TestableClientTransaction, DomainObjectIDs.Order1, notFoundObjectID, deletedObjectID, invalidInstance.ID);

      Assert.That(orders, Is.EqualTo(new[] { DomainObjectIDs.Order1.GetObject<Order>(), null, deletedObject, invalidInstance }));
    }

    [Test]
    public void DeleteObject ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(order.State.IsDeleted, Is.False);
      LifetimeService.DeleteObject(TestableClientTransaction, order);
      Assert.That(order.State.IsDeleted, Is.True);
    }

    [Test]
    public void DeleteObject_Twice ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      LifetimeService.DeleteObject(TestableClientTransaction, order);
      LifetimeService.DeleteObject(TestableClientTransaction, order);
    }
  }
}
