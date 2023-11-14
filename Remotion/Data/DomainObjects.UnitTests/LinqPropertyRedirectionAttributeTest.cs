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
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Linq.SqlBackend.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class LinqPropertyRedirectionAttributeTest
  {
    [Test]
    public void Initialization ()
    {
      var attribute = new LinqPropertyRedirectionAttribute(typeof(Order), "OrderNumber");

      var expected = Is.EqualTo(typeof(Order).GetProperty("OrderNumber"));
      Assert.That(attribute.GetMappedProperty(), expected);
    }

    [Test]
    public void Initialization_NonPublicProperty ()
    {
      var attribute = new LinqPropertyRedirectionAttribute(typeof(ClassWithNonPublicProperties), "PrivateGetSet");

      var expected = typeof(ClassWithNonPublicProperties).GetProperty("PrivateGetSet", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That(attribute.GetMappedProperty(), Is.EqualTo(expected));
    }

    [Test]
    public void Initialization_NonExistingProperty ()
    {
      Assert.That(
          () => new LinqPropertyRedirectionAttribute(typeof(Order), "Hugo").GetMappedProperty(),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The member redirects LINQ queries to 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Hugo', which does not exist."));
    }

    [Test]
    public void MethodCallTransformer_SupportedExpressionTypes ()
    {
      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(typeof(Order).GetProperty("OrderNumber"));
      Assert.That(transformer.SupportedExpressionTypes, Is.EqualTo(new[] { ExpressionType.Call }));
    }

    [Test]
    public void GetTransformer ()
    {
      var attribute = new LinqPropertyRedirectionAttribute(typeof(Order), "OrderNumber");

      var transformer = attribute.GetExpressionTransformer(null);

      Assert.That(transformer, Is.TypeOf(typeof(LinqPropertyRedirectionAttribute.MethodCallTransformer)));
      Assert.That(((LinqPropertyRedirectionAttribute.MethodCallTransformer)transformer).MappedProperty,
          Is.SameAs(typeof(Order).GetProperty("OrderNumber")));
    }

    [Test]
    public void GetTransformer_NonExistingProperty ()
    {
      var attribute = new LinqPropertyRedirectionAttribute(typeof(Order), "Hugo");
      Assert.That(
          () => attribute.GetExpressionTransformer(null),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The member redirects LINQ queries to 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Hugo', which does not exist."));
    }

    [Test]
    public void MethodCallTransformer_Transform ()
    {
      var instance = Expression.Constant(null, typeof(Order));
      var call = Expression.Call(instance, typeof(Order).GetProperty("RedirectedOrderNumber").GetGetMethod());

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(typeof(Order).GetProperty("OrderNumber"));
      var result = transformer.Transform(call);

      var expected = Expression.MakeMemberAccess(instance, typeof(Order).GetProperty("OrderNumber"));
      SqlExpressionTreeComparer.CheckAreEqualTrees(expected, result);
    }

    [Test]
    public void MethodCallTransformer_Transform_PublicToPrivateProperty ()
    {
      var instance = Expression.Constant(null, typeof(ClassWithNonPublicProperties));
      var call = Expression.Call(instance, typeof(ClassWithNonPublicProperties).GetProperty("PublicGetSet").GetGetMethod());

      var privateProperty = typeof(ClassWithNonPublicProperties).GetProperty("PrivateGetSet", BindingFlags.Instance | BindingFlags.NonPublic);
      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(privateProperty);
      var result = transformer.Transform(call);

      var expected = Expression.MakeMemberAccess(instance, privateProperty);
      SqlExpressionTreeComparer.CheckAreEqualTrees(expected, result);
    }

    [Test]
    public void MethodCallTransformer_Transform_ConvertRequired ()
    {
      var instance = Expression.Constant(null, typeof(TargetClassForPersistentMixin));
      var call = Expression.Call(instance, typeof(TargetClassForPersistentMixin).GetProperty("RedirectedPersistentProperty").GetGetMethod());

      var mixinProperty = typeof(IMixinAddingPersistentProperties).GetProperty("PersistentProperty");
      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(mixinProperty);
      var result = transformer.Transform(call);

      var expected = Expression.MakeMemberAccess(Expression.Convert(instance, typeof(IMixinAddingPersistentProperties)), mixinProperty);
      SqlExpressionTreeComparer.CheckAreEqualTrees(expected, result);
    }

    [Test]
    public void MethodCallTransformer_Transform_ExtensionMethod ()
    {
      var instance = Expression.Constant(null, typeof(Order));
      var call = Expression.Call(typeof(OrderExtensions).GetMethod("GetRedirectedOrderNumber"), instance);

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(typeof(Order).GetProperty("OrderNumber"));
      var result = transformer.Transform(call);

      var expected = Expression.MakeMemberAccess(instance, typeof(Order).GetProperty("OrderNumber"));
      SqlExpressionTreeComparer.CheckAreEqualTrees(expected, result);
    }

    [Test]
    public void MethodCallTransformer_Transform_InvalidType ()
    {
      var instance = Expression.Constant(null, typeof(OrderItem));
      var call = Expression.Call(instance, typeof(OrderItem).GetProperty("Position").GetGetMethod());

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(typeof(Order).GetProperty("OrderNumber"));
      Assert.That(
          () => transformer.Transform(call),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "The method call 'null.get_Position()' cannot be redirected to the property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber'. No coercion operator is defined between types "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem' and 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order'."));
    }

    [Test]
    public void MethodCallTransformer_Transform_RedirectionToPropertyWithOtherReturnType ()
    {
      var instance = Expression.Constant(null, typeof(Order));
      var call = Expression.Call(instance, typeof(Order).GetProperty("OrderNumber").GetGetMethod());

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(typeof(Order).GetProperty("DeliveryDate"));
      Assert.That(
          () => transformer.Transform(call),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "The method call 'null.get_OrderNumber()' cannot be redirected to the property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate'. The property has an incompatible return type."));
    }

    [Test]
    public void MethodCallTransformer_Transform_RedirectionToSelf ()
    {
      var instance = Expression.Constant(null, typeof(Order));
      var call = Expression.Call(instance, typeof(Order).GetProperty("OrderNumber").GetGetMethod());

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(typeof(Order).GetProperty("OrderNumber"));
      Assert.That(
          () => transformer.Transform(call),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "The method call 'null.get_OrderNumber()' cannot be redirected to the property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber'. The method would redirect to itself."));
    }

    [Test]
    public void MethodCallTransformer_Transform_StaticMethod ()
    {
      var call = Expression.Call(NormalizingMemberInfoFromExpressionUtility.GetMethod(() => StaticMethod()));

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(typeof(Order).GetProperty("OrderNumber"));
      Assert.That(
          () => transformer.Transform(call),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "The method call 'StaticMethod()' cannot be redirected to the property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber'. "
                  + "Only instance or extension methods can be redirected, but the method is static."));
    }

    [Test]
    public void MethodCallTransformer_Transform_MethodWithArguments ()
    {
      var call = Expression.Call(
          Expression.Constant(null, typeof(LinqPropertyRedirectionAttributeTest)),
          NormalizingMemberInfoFromExpressionUtility.GetMethod(() => MethodWithArguments(null)),
          Expression.Constant(null));

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(typeof(Order).GetProperty("OrderNumber"));
      Assert.That(
          () => transformer.Transform(call),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "The method call 'null.MethodWithArguments(null)' cannot be redirected to the property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber'. Only methods without parameters can be redirected."));
    }

    [Test]
    public void MethodCallTransformer_Transform_ExtensionMethodWithArguments ()
    {
      var call = Expression.Call(
          typeof(OrderExtensions).GetMethod("GetRedirectedOrderNumberWithInvalidSignature"),
          Expression.Constant(null, typeof(Order)),
          Expression.Constant(null));

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer(typeof(Order).GetProperty("OrderNumber"));
      Assert.That(
          () => transformer.Transform(call),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "The method call 'null.GetRedirectedOrderNumberWithInvalidSignature(null)' cannot be redirected to the property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber'. "
                  + "Extensions method expecting parameters other than the instance parameter cannot be redirected."));
    }

    private static void StaticMethod ()
    {
    }

    private void MethodWithArguments ([UsedImplicitly] object o)
    {
    }
  }
}
