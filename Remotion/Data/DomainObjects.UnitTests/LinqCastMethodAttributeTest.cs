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
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Linq.SqlBackend.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class LinqCastMethodAttributeTest
  {
    [Test]
    public void GetExpressionTransformer ()
    {
      var attribute = new LinqCastMethodAttribute();
      var transformer = attribute.GetExpressionTransformer(null);

      Assert.That(transformer, Is.TypeOf(typeof(LinqCastMethodAttribute.MethodCallTransformer)));
    }

    [Test]
    public void MethodCallTransformer_SupportedExpressionTypes ()
    {
      Assert.That(new LinqCastMethodAttribute.MethodCallTransformer().SupportedExpressionTypes, Is.EqualTo(new[] { ExpressionType.Call }));
    }

    [Test]
    public void MethodCallTransformer_Transform_InstanceMethod ()
    {
      var instance = Expression.Constant(null, typeof(TargetClassForPersistentMixin));
      var call = Expression.Call(instance, typeof(TargetClassForPersistentMixin).GetProperty("MixedMembers").GetGetMethod());
      var transformer = new LinqCastMethodAttribute.MethodCallTransformer();

      var result = transformer.Transform(call);

      var expected = Expression.Convert(instance, typeof(IMixinAddingPersistentProperties));
      SqlExpressionTreeComparer.CheckAreEqualTrees(expected, result);
    }

    [Test]
    public void MethodCallTransformer_Transform_InstanceMethod_WrongParameterCount ()
    {
      var instance = Expression.Constant(null, typeof(LinqCastMethodAttributeTest));
      var call = Expression.Call(instance, typeof(LinqCastMethodAttributeTest).GetMethod("InvalidInstanceCastMethod"), Expression.Constant(0));
      var transformer = new LinqCastMethodAttribute.MethodCallTransformer();
      Assert.That(
          () => transformer.Transform(call),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Non-static LinqCastMethods must have no arguments. Expression: 'null.InvalidInstanceCastMethod(0)'"));
    }

    [Test]
    public void MethodCallTransformer_Transform_StaticMethod ()
    {
      var instance = Expression.Constant(null, typeof(TargetClassForPersistentMixin));
      var call = Expression.Call(typeof(TargetClassForPersistentMixinExtensions).GetMethod("GetMixedMembers"), instance);
      var transformer = new LinqCastMethodAttribute.MethodCallTransformer();

      var result = transformer.Transform(call);

      var expected = Expression.Convert(instance, typeof(IMixinAddingPersistentProperties));
      SqlExpressionTreeComparer.CheckAreEqualTrees(expected, result);
    }

    [Test]
    public void MethodCallTransformer_Transform_StaticMethod_WrongParameterCount ()
    {
      var call = Expression.Call(typeof(LinqCastMethodAttributeTest).GetMethod("InvalidStaticCastMethod"));
      var transformer = new LinqCastMethodAttribute.MethodCallTransformer();
      Assert.That(
          () => transformer.Transform(call),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Static LinqCastMethods must have exactly one argument. Expression: 'InvalidStaticCastMethod()'"));
    }

    public IMixinAddingPersistentProperties InvalidInstanceCastMethod (int i)
    {
      throw new NotImplementedException();
    }

    public static IMixinAddingPersistentProperties InvalidStaticCastMethod ()
    {
      throw new NotImplementedException();
    }
  }
}
