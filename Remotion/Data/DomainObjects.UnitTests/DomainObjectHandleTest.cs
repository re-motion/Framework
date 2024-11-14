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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class DomainObjectHandleTest : StandardMappingTest
  {
    [Test]
    public void Initialization ()
    {
      var handle = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);
      Assert.That(handle.ObjectID, Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void Initialization_ThrowsWhenTypeDoesntMatchID ()
    {
      Assert.That(
          () => new DomainObjectHandle<OrderItem>(DomainObjectIDs.Order1),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The class type of ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' doesn't match the handle type "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'.", "objectID"));
      Assert.That(
          () => new DomainObjectHandle<DomainObject>(DomainObjectIDs.Order1),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The class type of ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' doesn't match the handle type "
              + "'Remotion.Data.DomainObjects.DomainObject'.", "objectID"));
    }

    [Test]
    public void Cast_CanUpcast ()
    {
      var handle = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);

      var result = handle.Cast<DomainObject>();

      Assert.That(result, Is.SameAs(handle));
      Assert.That(VariableTypeInferrer.GetVariableType(result), Is.SameAs(typeof(IDomainObjectHandle<DomainObject>)));
    }

    [Test]
    public void Cast_CanDowncast ()
    {
      var handle = new DomainObjectHandle<Order>(DomainObjectIDs.Order1).Cast<DomainObject>();

      var result = handle.Cast<Order>();

      Assert.That(result, Is.SameAs(handle));
      Assert.That(VariableTypeInferrer.GetVariableType(result), Is.SameAs(typeof(IDomainObjectHandle<Order>)));
    }

    [Test]
    public void Cast_ThrowsOnUnsupportedCast ()
    {
      var handle = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);

      Assert.That(
          () => handle.Cast<OrderItem>(),
          Throws.TypeOf<InvalidCastException>().With.Message.EqualTo(
              "The handle for object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be represented as a handle for type "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem'."));
    }

    [Test]
    public void Equals_Object_True ()
    {
      var handle1 = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);
      var handle2 = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);

      Assert.That(handle1.Equals((object)handle2), Is.True);
    }

    [Test]
    public void Equals_Object_False_Null ()
    {
      var handle = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);

      Assert.That(handle.Equals((object)null), Is.False);
    }

    [Test]
    public void Equals_Object_False_DifferentIDs ()
    {
      var handle1 = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);
      var handle2 = new DomainObjectHandle<Order>(DomainObjectIDs.Order2);

      Assert.That(handle1.Equals((object)handle2), Is.False);
    }

    [Test]
    public void Equals_Object_False_UnrelatedObject ()
    {
      var handle = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);

      Assert.That(handle.Equals("what?"), Is.False);
    }

    [Test]
    public void Equals_Object_False_WrongType ()
    {
      var handle1 = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);
      var handle2 = new Mock<IDomainObjectHandle<DomainObject>>();
      handle2.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);

      Assert.That(handle1.Equals((object)handle2), Is.False);
    }

    [Test]
    public void Equals_Equatable_True ()
    {
      var handle1 = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);
      var handle2 = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);

      Assert.That(handle1.Equals(handle2), Is.True);
    }

    [Test]
    public void Equals_Equatable_False_Null ()
    {
      var handle = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);

// ReSharper disable RedundantCast
      Assert.That(handle.Equals((IDomainObjectHandle<DomainObject>)null), Is.False);
// ReSharper restore RedundantCast
    }

    [Test]
    public void Equals_Equatable_False_DifferentIDs ()
    {
      var handle1 = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);
      var handle2 = new DomainObjectHandle<Order>(DomainObjectIDs.Order2);

      Assert.That(handle1.Equals(handle2), Is.False);
    }

    [Test]
    public void Equals_Equatable_False_WrongType ()
    {
      var handle1 = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);
      var handle2 = new Mock<IDomainObjectHandle<DomainObject>>();
      handle2.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);

      Assert.That(handle1.Equals(handle2.Object), Is.False);
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var handle1 = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);
      var handle2 = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);

      Assert.That(handle1.GetHashCode(), Is.EqualTo(handle2.GetHashCode()));
    }

    [Test]
    public new void ToString ()
    {
      var handle = new DomainObjectHandle<Order>(DomainObjectIDs.Order1);
      Assert.That(handle.ToString(), Is.EqualTo("Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid (handle)"));
    }

    [Test]
    public void TypeProvider_OnInterface ()
    {
      Assert.That(GetTypeConversionProvider().CanConvert(typeof(IDomainObjectHandle<Order>), typeof(string)), Is.True);
      Assert.That(GetTypeConversionProvider().CanConvert(typeof(string), typeof(IDomainObjectHandle<Order>)), Is.True);
    }

    [Test]
    public void TypeProvider_OnClass ()
    {
      // Using the DomainObjectHandle<T> class in APIs is not recommended, IDomainObjectHandle<T> should be used instead.
      // To get a type converter, the source type should always be explicitly given (as IDomainObjectHandle<T>), not inferred via value.GetType().
      // Therefore, we don't need a TypeConverter for DomainObjectHandle<T>.
      Assert.That(GetTypeConversionProvider().CanConvert(typeof(DomainObjectHandle<Order>), typeof(string)), Is.False);
      Assert.That(GetTypeConversionProvider().CanConvert(typeof(string), typeof(DomainObjectHandle<Order>)), Is.False);
    }

    [Test]
    public void HandleAttribute_OnInterface ()
    {
      Assert.That(typeof(IDomainObjectHandle<>).IsDefined(typeof(DomainObjectHandleAttribute), false), Is.True);
    }

    [Test]
    public void HandleAttribute_OnClass ()
    {
      // Using the DomainObjectHandle<T> class in APIs is not recommended, IDomainObjectHandle<T> should be used instead.
      // Therefore, we don't need the handle attribute on DomainObjectHandle<T>.
      Assert.That(typeof(DomainObjectHandle<>).IsDefined(typeof(DomainObjectHandleAttribute), false), Is.False);
    }

    private ITypeConversionProvider GetTypeConversionProvider ()
    {
      return SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>();
    }
  }
}
