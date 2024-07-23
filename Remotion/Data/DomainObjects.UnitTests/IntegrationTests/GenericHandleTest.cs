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
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class GenericHandleTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetHandle_FromID_ReturnsMatchingHandle ()
    {
      var result = DomainObjectIDs.Order1.GetHandle<Order>();

      Assert.That(result, Is.TypeOf<DomainObjectHandle<Order>>());
      Assert.That(result.ObjectID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(VariableTypeInferrer.GetVariableType(result), Is.SameAs(typeof(IDomainObjectHandle<Order>)));
    }

    [Test]
    public void GetHandle_FromID_InvalidType_ThrowsInvalidCastException ()
    {
      Assert.That(
          () => DomainObjectIDs.Order1.GetHandle<OrderItem>(),
          Throws.TypeOf<ArgumentException>()
                .With.ArgumentExceptionMessageEqualTo(
                    "The ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be represented as an "
                    + "'IDomainObjectHandle<Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem>'.",
                    "T"));
    }

    [Test]
    public void GetHandle_FromID_ReturnsCovariantInterface ()
    {
      var result = DomainObjectIDs.Order1.GetHandle<Order>();

      Assert.That(result, Is.AssignableTo<IDomainObjectHandle<Order>>());
      Assert.That(result, Is.AssignableTo<IDomainObjectHandle<TestDomainBase>>());
      Assert.That(result, Is.AssignableTo<IDomainObjectHandle<DomainObject>>());
    }

    [Test]
    public void GetHandle_FromID_AllowsCovariantTypeParameters ()
    {
      var result1 = DomainObjectIDs.Order1.GetHandle<Order>();
      var result2 = DomainObjectIDs.Order1.GetHandle<TestDomainBase>();
      var result3 = DomainObjectIDs.Order1.GetHandle<DomainObject>();

      Assert.That(result1, Is.TypeOf<DomainObjectHandle<Order>>());
      Assert.That(VariableTypeInferrer.GetVariableType(result1), Is.SameAs(typeof(IDomainObjectHandle<Order>)));

      Assert.That(result2, Is.TypeOf<DomainObjectHandle<Order>>());
      Assert.That(VariableTypeInferrer.GetVariableType(result2), Is.SameAs(typeof(IDomainObjectHandle<TestDomainBase>)));

      Assert.That(result3, Is.TypeOf<DomainObjectHandle<Order>>());
      Assert.That(VariableTypeInferrer.GetVariableType(result3), Is.SameAs(typeof(IDomainObjectHandle<DomainObject>)));
    }

    [Test]
    public void GetHandle_FromObject ()
    {
      Order order = Order.NewObject();

      var orderTypedHandle = order.GetHandle();
      var testDomainBaseTypedHandle1 = ((TestDomainBase)order).GetHandle();
      var testDomainBaseTypedHandle2 = order.GetHandle<TestDomainBase>();
      var domainObjectTypedHandle = ((DomainObject)order).GetHandle();

      Assert.That(orderTypedHandle.ObjectID, Is.EqualTo(order.ID));
      Assert.That(orderTypedHandle, Is.TypeOf<DomainObjectHandle<Order>>());
      Assert.That(VariableTypeInferrer.GetVariableType(orderTypedHandle), Is.SameAs(typeof(IDomainObjectHandle<Order>)));

      Assert.That(testDomainBaseTypedHandle1, Is.EqualTo(orderTypedHandle));
      Assert.That(VariableTypeInferrer.GetVariableType(testDomainBaseTypedHandle1), Is.SameAs(typeof(IDomainObjectHandle<TestDomainBase>)));

      Assert.That(testDomainBaseTypedHandle2, Is.EqualTo(orderTypedHandle));
      Assert.That(VariableTypeInferrer.GetVariableType(testDomainBaseTypedHandle2), Is.SameAs(typeof(IDomainObjectHandle<TestDomainBase>)));

      Assert.That(domainObjectTypedHandle, Is.EqualTo(orderTypedHandle));
      Assert.That(VariableTypeInferrer.GetVariableType(domainObjectTypedHandle), Is.SameAs(typeof(IDomainObjectHandle<DomainObject>)));
    }

    [Test]
    public void GetHandle_Null ()
    {
      Assert.That(() => ((Order)null).GetHandle(), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void GetSafeHandle_FromObject ()
    {
      Order order = Order.NewObject();

      var handle = order.GetSafeHandle();
      Assert.That(handle.ObjectID, Is.EqualTo(order.ID));
      Assert.That(handle, Is.TypeOf<DomainObjectHandle<Order>>());
      Assert.That(VariableTypeInferrer.GetVariableType(handle), Is.SameAs(typeof(IDomainObjectHandle<Order>)));
    }

    [Test]
    public void GetSafeHandle_Null ()
    {
      Assert.That(((Order)null).GetSafeHandle(), Is.Null);
    }
  }
}
