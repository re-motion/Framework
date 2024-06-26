// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Utilities.MemberInfoEqualityComparerTestDomain;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class MemberInfoEqualityComparerTest
  {
    [Test]
    public void Equals_True_SameInstance ()
    {
      var one = typeof(ClassWithMethods).GetMethod("SimpleMethod1");
      var two = one;

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_True_DifferentReflectedTypes ()
    {
      var one = typeof(ClassWithMethods).GetMethod("SimpleMethod1")!;
      var two = typeof(DerivedClassWithMethods).GetMethod("SimpleMethod1")!;

      Assert.That(one.DeclaringType, Is.SameAs(two.DeclaringType));

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_True_NullMembers ()
    {
      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(null, null);

      Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_False_NullMemberOnLeftSide ()
    {
      var one = typeof(ClassWithMethods).GetMethod("SimpleMethod1");

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, null);

      Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_False_NullMemberOnRightSide ()
    {
      var one = typeof(ClassWithMethods).GetMethod("SimpleMethod1");

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(null, one);

      Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_True_NullDeclaringType ()
    {
      var one = new FakeMemberInfo(null, 1, typeof(object).Module);
      var two = new FakeMemberInfo(null, 1, typeof(object).Module);

      Assert.That(one.DeclaringType, Is.SameAs(two.DeclaringType));

      var result = MemberInfoEqualityComparer<FakeMemberInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_True_ArrayMembers ()
    {
      var one = typeof(int[]).GetMethod("Set");
      var two = typeof(int[]).GetMethod("Set");

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_True_AbstractArrayMembers ()
    {
      var one = typeof(Array).GetMethod("CopyTo", new[] { typeof(Array), typeof(int) });
      var two = typeof(int[]).GetMethod("CopyTo", new[] { typeof(Array), typeof(int) });

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_True_SameMember_OnSameDeclaringTypeInstantiations ()
    {
      var one = typeof(GenericClassWithMethods<object>).GetMethod("SimpleMethod");
      var two = typeof(GenericClassWithMethods<object>).GetMethod("SimpleMethod");

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_True_SameMember_OnGenericTypeDefinition ()
    {
      var one = typeof(GenericClassWithMethods<>).GetMethod("SimpleMethod");
      var two = typeof(GenericClassWithMethods<>).GetMethod("SimpleMethod");

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_True_SameMethod_WithSameMethodInstantiations ()
    {
      var one = typeof(ClassWithMethods).GetMethod("GenericMethod")!.MakeGenericMethod(typeof(object));
      var two = typeof(ClassWithMethods).GetMethod("GenericMethod")!.MakeGenericMethod(typeof(object));

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_True_SameMethod_GenericMethodDefinition ()
    {
      var one = typeof(ClassWithMethods).GetMethod("GenericMethod")!;
      var two = typeof(ClassWithMethods).GetMethod("GenericMethod")!;

      Assert.That(one.IsGenericMethodDefinition, Is.True);

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_False_DifferentMetadataTokens ()
    {
      var one = typeof(ClassWithMethods).GetMethod("SimpleMethod1");
      var two = typeof(ClassWithMethods).GetMethod("SimpleMethod2");

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_False_DifferentTypes ()
    {
      var one = typeof(ClassWithMethods).GetMethod("SimpleMethod1")!;
      var two = new FakeMemberInfo(typeof(string), one.MetadataToken, one.Module);

      var result = MemberInfoEqualityComparer<MemberInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_False_DifferentModules ()
    {
      var one = new FakeMemberInfo(typeof(object), 1, typeof(object).Module);
      var two = new FakeMemberInfo(one.DeclaringType, one.MetadataToken, typeof(MemberInfoEqualityComparerTest).Module);

      var result = MemberInfoEqualityComparer<FakeMemberInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_False_SameMember_OnDifferentDeclaringTypeInstantiations ()
    {
      var one = typeof(GenericClassWithMethods<string>).GetMethod("SimpleMethod");
      var two = typeof(GenericClassWithMethods<object>).GetMethod("SimpleMethod");

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_False_SameMethod_WithDifferentMethodInstantiations ()
    {
      var one = typeof(ClassWithMethods).GetMethod("GenericMethod")!.MakeGenericMethod(typeof(string));
      var two = typeof(ClassWithMethods).GetMethod("GenericMethod")!.MakeGenericMethod(typeof(object));

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_False_WithGenericMethodDefinition_AndInstantiation ()
    {
      var one = typeof(ClassWithMethods).GetMethod("GenericMethod")!;
      var two = typeof(ClassWithMethods).GetMethod("GenericMethod")!.MakeGenericMethod(typeof(object));

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_False_ArrayMembers ()
    {
      var one = typeof(int[]).GetMethod("Set", new[] { typeof(int), typeof(int) });
      var two = typeof(int[]).GetMethod("Address", new[] { typeof(int) });

      Assert.That(one, Is.Not.SameAs(two));

      var result = MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(one, two);

      Assert.That(result, Is.False);
    }

    [Test]
    public void Equals_Methods ()
    {
      var methodInfo1a = typeof(ClassWithMethods).GetMethod("SimpleMethod1");
      var methodInfo1b = typeof(DerivedClassWithMethods).GetMethod("SimpleMethod1");
      var methodInfo2 = typeof(ClassWithMethods).GetMethod("SimpleMethod2");
      var methodInfo3 = typeof(ClassWithMethods).GetMethod("OverriddenMethod");
      var methodInfo4 = typeof(DerivedClassWithMethods).GetMethod("OverriddenMethod");

      Assert.That(MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(methodInfo1a, methodInfo1b), Is.True);
      Assert.That(MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(methodInfo1a, methodInfo2), Is.False);
      Assert.That(MemberInfoEqualityComparer<MethodInfo>.Instance.Equals(methodInfo3, methodInfo4), Is.False);
    }

    [Test]
    public void Equals_Properties ()
    {
      var propertyInfo1a = typeof(ClassWithProperties).GetProperty("Property1");
      var propertyInfo1b = typeof(DerivedClassWithProperties).GetProperty("Property1");
      var propertyInfo2 = typeof(ClassWithProperties).GetProperty("Property2");

      Assert.That(MemberInfoEqualityComparer<PropertyInfo>.Instance.Equals(propertyInfo1a, propertyInfo1b), Is.True);
      Assert.That(MemberInfoEqualityComparer<PropertyInfo>.Instance.Equals(propertyInfo1a, propertyInfo2), Is.False);
    }

    [Test]
    public void Equals_Field ()
    {
      var fieldInfo1a = typeof(ClassWithFields).GetField("Field1");
      var fieldInfo1b = typeof(DerivedClassWithFields).GetField("Field1");
      var fieldInfo2 = typeof(ClassWithFields).GetField("Field2");

      Assert.That(MemberInfoEqualityComparer<FieldInfo>.Instance.Equals(fieldInfo1a, fieldInfo1b), Is.True);
      Assert.That(MemberInfoEqualityComparer<FieldInfo>.Instance.Equals(fieldInfo1a, fieldInfo2), Is.False);
    }

    [Test]
    public void Equals_Types ()
    {
      Assert.That(MemberInfoEqualityComparer<Type>.Instance.Equals(typeof(Enumerable), typeof(Enumerable)), Is.True);
      Assert.That(MemberInfoEqualityComparer<Type>.Instance.Equals(typeof(List<int>), typeof(List<int>)), Is.True);
      Assert.That(MemberInfoEqualityComparer<Type>.Instance.Equals(typeof(List<>), typeof(List<>)), Is.True);
      Assert.That(MemberInfoEqualityComparer<Type>.Instance.Equals(typeof(List<int>), typeof(List<>)), Is.False);
      Assert.That(MemberInfoEqualityComparer<Type>.Instance.Equals(typeof(List<>), typeof(List<int>)), Is.False);
    }

    [Test]
    public void GetHashcode_SameInstance ()
    {
      var one = typeof(ClassWithMethods).GetMethod("SimpleMethod1")!;
      var two = one;

      Assert.That(
          MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(one),
          Is.EqualTo(MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(two)));
    }

    [Test]
    public void GetHashcode_DifferentReflectedType ()
    {
      var one = typeof(ClassWithMethods).GetMethod("SimpleMethod1")!;
      var two = typeof(DerivedClassWithMethods).GetMethod("SimpleMethod1")!;

      Assert.That(one.DeclaringType, Is.SameAs(two.DeclaringType));
      Assert.That(
          MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(one),
          Is.EqualTo(MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(two)));
    }

    [Test]
    public void GetHashcode_NullDeclaringType ()
    {
      var one = new FakeMemberInfo(null, 1, typeof(object).Module);
      var two = new FakeMemberInfo(null, 1, typeof(object).Module);

      Assert.That(one.DeclaringType, Is.SameAs(two.DeclaringType));
      Assert.That(
          MemberInfoEqualityComparer<FakeMemberInfo>.Instance.GetHashCode(one),
          Is.EqualTo(MemberInfoEqualityComparer<FakeMemberInfo>.Instance.GetHashCode(two)));
    }

    [Test]
    public void GetHashcode_ArrayMembers ()
    {
      var one = typeof(int[]).GetMethod("Set")!;
      var two = typeof(int[]).GetMethod("Set")!;

      Assert.That(
          MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(one),
          Is.EqualTo(MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(two)));
    }

    [Test]
    public void GetHashcode_AbstractArrayMembers ()
    {
      var one = typeof(Array).GetMethod("CopyTo", new[] { typeof(Array), typeof(int) })!;
      var two = typeof(bool[]).GetMethod("CopyTo", new[] { typeof(Array), typeof(int) })!;

      Assert.That(
          MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(one),
          Is.EqualTo(MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(two)));
    }

    [Test]
    public void GetHashcode_SameMember_OnSameDeclaringTypeInstantiations ()
    {
      var one = typeof(GenericClassWithMethods<object>).GetMethod("SimpleMethod")!;
      var two = typeof(GenericClassWithMethods<object>).GetMethod("SimpleMethod")!;

      Assert.That(
          MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(one),
          Is.EqualTo(MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(two)));
    }

    [Test]
    public void GetHashcode_SameMember_OnGenericTypeDefinition ()
    {
      var one = typeof(GenericClassWithMethods<>).GetMethod("SimpleMethod")!;
      var two = typeof(GenericClassWithMethods<>).GetMethod("SimpleMethod")!;

      Assert.That(
          MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(one),
          Is.EqualTo(MemberInfoEqualityComparer<MethodInfo>.Instance.GetHashCode(two)));
    }

    [Test]
    public void GetHashCode_NullMember ()
    {
      Assert.That(() => MemberInfoEqualityComparer<MemberInfo>.Instance.GetHashCode(null!), Throws.Exception.TypeOf<ArgumentNullException>());
    }
  }
}
