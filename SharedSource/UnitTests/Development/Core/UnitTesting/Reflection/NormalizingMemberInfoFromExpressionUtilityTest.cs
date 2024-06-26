// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Development.Core.UnitTesting.Reflection
{
  [TestFixture]
  public class NormalizingMemberInfoFromExpressionUtilityTest
  {
    private class TestClass
    {
    }

    [Test]
    public void GetMember_Static_MemberExpression ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember(() => DomainType.StaticField);

      var expected = typeof(DomainType).GetMember("StaticField").Single();
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMember_Static_MethodCallExpression ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember(() => DomainType.StaticMethod());

      var expected = typeof(DomainType).GetMember("StaticMethod").Single();
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMember_Static_NewExpression ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember(() => new DomainType());

      var expected = typeof(DomainType).GetMember(".ctor").Single();
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMember_Static_InvalidExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetMember(() => 1),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Must be a MemberExpression, MethodCallExpression or NewExpression.", "expression"));
    }

    [Test]
    public void GetMember_Instance_MemberExpression ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember((DomainType obj) => obj.InstanceProperty);

      var expected = typeof(DomainType).GetMember("InstanceProperty").Single();
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMember_Instance_MemberExpression_OverridingProperty ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember((DomainType obj) => obj.OverridingProperty);

      var expected = typeof(DomainType).GetProperty("OverridingProperty");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMember_Instance_MethodCallExpression ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember((DomainType obj) => obj.InstanceMethod());

      var expected = typeof(DomainType).GetMember("InstanceMethod").Single();
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMember_Instance_NewExpression ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMember((DomainType obj) => new DomainType());

      var expected = typeof(DomainType).GetMember(".ctor").Single();
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMember_Instance_InvalidExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetMember((DomainType obj) => 1),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Must be a MemberExpression, MethodCallExpression or NewExpression.", "expression"));
    }

    [Test]
    public void GetField_Static ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetField(() => DomainType.StaticField);

      var expected = typeof(DomainType).GetField("StaticField");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetField_Static_NonMemberExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetField(() => DomainType.StaticMethod()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must be a MemberExpression.", "expression"));
    }

    [Test]
    public void GetField_Static_NonField ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetField(() => DomainType.StaticProperty),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must hold a field access expression.", "expression"));
    }

    [Test]
    public void GetField_Instance ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetField((DomainType obj) => obj.InstanceField);

      var expected = typeof(DomainType).GetField("InstanceField");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetField_Instance_NonMemberExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetField((DomainType obj) => obj.InstanceMethod()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must be a MemberExpression.", "expression"));
    }

    [Test]
    public void GetField_Instance_NonField ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetField((DomainType obj) => obj.InstanceProperty),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must hold a field access expression.", "expression"));
    }

    [Test]
    public void GetConstructor ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetConstructor(() => new DomainType());

      var expected = typeof(DomainType).GetConstructor(Type.EmptyTypes);
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetConstructor_NonNewExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetConstructor(() => DomainType.StaticField),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must be a NewExpression.", "expression"));
    }

    [Test]
    public void GetMethod_StaticVoid ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod(() => DomainType.StaticVoidMethod());

      var expected = typeof(DomainType).GetMethod("StaticVoidMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Static ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod(() => DomainType.StaticMethod());

      var expected = typeof(DomainType).GetMethod("StaticMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Static_VoidGeneric ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod(() => DomainType.StaticVoidGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("StaticVoidGenericMethod")!.MakeGenericMethod(typeof(TestClass));
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Static_Generic ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod(() => DomainType.StaticGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("StaticGenericMethod")!.MakeGenericMethod(typeof(TestClass));
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Static_NonMethodCallExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetMethod(() => DomainType.StaticProperty),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must be a MethodCallExpression.", "expression"));
    }

    [Test]
    public void GetMethod_InstanceVoid ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.InstanceVoidMethod());

      var expected = typeof(DomainType).GetMethod("InstanceVoidMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Instance ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.InstanceMethod());

      var expected = typeof(DomainType).GetMethod("InstanceMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Instance_OverridingVoid ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.OverridingVoidMethod());

      var expected = typeof(DomainType).GetMethod("OverridingVoidMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Instance_Overriding ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.OverridingMethod());

      var expected = typeof(DomainType).GetMethod("OverridingMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Instance_OverridingVoid_WithNonBaseMethodInExpressionTree ()
    {
      // The C# compiler always inserts the root method definition for virtual methods into expression trees.
      // To test behavior with non-root definitions, we need to construct an expression tree manually.
      var parameter = Expression.Parameter(typeof(DomainType), "obj");
      var method = typeof(DomainType).GetMethod("OverridingVoidMethod")!;
      var expression = Expression.Lambda<Action<DomainType>>(Expression.Call(parameter, method), parameter);

      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod(expression);

      Assert.That(member, Is.EqualTo(method));
    }

    [Test]
    public void GetMethod_Instance_Overriding_WithNonBaseMethodInExpressionTree ()
    {
      // The C# compiler always inserts the root method definition for virtual methods into expression trees.
      // To test behavior with non-root definitions, we need to construct an expression tree manually.
      var parameter = Expression.Parameter(typeof(DomainType), "obj");
      var method = typeof(DomainType).GetMethod("OverridingMethod")!;
      var expression = Expression.Lambda<Func<DomainType, int>>(Expression.Call(parameter, method), parameter);

      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod(expression);

      Assert.That(member, Is.EqualTo(method));
    }

    [Test]
    public void GetMethod_Instance_NonMethodCallExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.InstanceProperty),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must be a MethodCallExpression.", "expression"));
    }

    [Test]
    public void GetMethod_Instance_VoidGenericMethod ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.InstanceVoidGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("InstanceVoidGenericMethod")!.MakeGenericMethod(typeof(TestClass));
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Instance_GenericMethod ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.InstanceGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("InstanceGenericMethod")!.MakeGenericMethod(typeof(TestClass));
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Instance_OverridingVoidGenericMethod ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.OverridingVoidGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("OverridingVoidGenericMethod")!.MakeGenericMethod(typeof(TestClass));
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Instance_OverridingGenericMethod ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.OverridingGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("OverridingGenericMethod")!.MakeGenericMethod(typeof(TestClass));
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Instance_OverridingVoidGeneric_WithNonBaseMethodInExpressionTree ()
    {
      // The C# compiler always inserts the root method definition for virtual methods into expression trees.
      // To test behavior with non-root definitions, we need to construct an expression tree manually.
      var parameter = Expression.Parameter(typeof(DomainType), "obj");
      var method = typeof(DomainType).GetMethod("OverridingVoidGenericMethod")!.MakeGenericMethod(typeof(TestClass));
      var expression = Expression.Lambda<Action<DomainType>>(
          Expression.Call(parameter, method, Expression.Constant(null, typeof(TestClass))), parameter);

      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod(expression);

      Assert.That(member, Is.EqualTo(method));
    }

    [Test]
    public void GetMethod_Instance_OverridingGeneric_WithNonBaseMethodInExpressionTree ()
    {
      // The C# compiler always inserts the root method definition for virtual methods into expression trees.
      // To test behavior with non-root definitions, we need to construct an expression tree manually.
      var parameter = Expression.Parameter(typeof(DomainType), "obj");
      var method = typeof(DomainType).GetMethod("OverridingGenericMethod")!.MakeGenericMethod(typeof(TestClass));
      var expression = Expression.Lambda<Func<DomainType, int>>(
          Expression.Call(parameter, method, Expression.Constant(null, typeof(TestClass))), parameter);

      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod(expression);

      Assert.That(member, Is.EqualTo(method));
    }

    [Test]
    public void GetMethod_Instance_BaseMethod ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.BaseMethod());

      var expected = typeof(DomainType).GetMethod("BaseMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Instance_VirtualBaseMethod ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.VirtualBaseMethod());

      var expected = typeof(DomainType).GetMethod("VirtualBaseMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_Instance_InterfaceMethod ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => obj.InterfaceMethod());

      var expected = typeof(DomainType).GetMethod("InterfaceMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetMethod_FromInterface ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((IDomainInterface obj) => obj.InterfaceMethod());

      var expected = typeof(IDomainInterface).GetMethod("InterfaceMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Ignore("TODO: normalizing interfaces")]
    [Test]
    public void GetMethod_FromCastedInstance ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetMethod((DomainType obj) => ((IDomainInterface)obj).InterfaceMethod());

      var expected = typeof(DomainType).GetMethod("InterfaceMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetGenericMethodDefinition_StaticVoid ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition(() => DomainType.StaticVoidGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("StaticVoidGenericMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetGenericMethodDefinition_StaticVoid_NonGenericMethod ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition(() => DomainType.StaticVoidMethod()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must hold a generic method access expression.", "expression"));
    }

    [Test]
    public void GetGenericMethodDefinition_Static ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition(() => DomainType.StaticGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("StaticGenericMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetGenericMethodDefinition_Static_NonMethodCallExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition(() => DomainType.StaticProperty),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must be a MethodCallExpression.", "expression"));
    }

    [Test]
    public void GetGenericMethodDefinition_Static_NonGenericMethod ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition(() => DomainType.StaticMethod()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must hold a generic method access expression.", "expression"));
    }

    [Test]
    public void GetGenericMethodDefinition_Instance_Void ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition((DomainType obj) => obj.InstanceVoidGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("InstanceVoidGenericMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetGenericMethodDefinition_Instance ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition((DomainType obj) => obj.InstanceGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("InstanceGenericMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetGenericMethodDefinition_Instance_OverridingVoidMethod ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition((DomainType obj) => obj.OverridingVoidGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("OverridingVoidGenericMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetGenericMethodDefinition_Instance_OverridingMethod ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition((DomainType obj) => obj.OverridingGenericMethod<TestClass?>(null));

      var expected = typeof(DomainType).GetMethod("OverridingGenericMethod");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetGenericMethodDefinition_Instance_NonMethodCallExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition((DomainType obj) => obj.InstanceProperty),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must be a MethodCallExpression.", "expression"));
    }

    [Test]
    public void GetGenericMethodDefinition_Instance_VoidNonGenericMethod ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition((DomainType obj) => obj.InstanceVoidMethod()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must hold a generic method access expression.", "expression"));
    }

    [Test]
    public void GetGenericMethodDefinition_Instance_NonGenericMethod ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetGenericMethodDefinition((DomainType obj) => obj.InstanceMethod()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must hold a generic method access expression.", "expression"));
    }

    [Test]
    public void GetProperty_Static ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetProperty(() => DomainType.StaticProperty);

      var expected = typeof(DomainType).GetProperty("StaticProperty");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetProperty_Static_NonMemberExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetProperty(() => DomainType.StaticMethod()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must be a MemberExpression.", "expression"));
    }

    [Test]
    public void GetProperty_Static_NonProperty ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetProperty(() => DomainType.StaticField),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must hold a property access expression.", "expression"));
    }

    [Test]
    public void GetProperty_Instance ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetProperty((DomainType obj) => obj.InstanceProperty);

      var expected = typeof(DomainType).GetProperty("InstanceProperty");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetProperty_Instance_NonMemberExpression ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetProperty((DomainType obj) => obj.InstanceMethod()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must be a MemberExpression.", "expression"));
    }

    [Test]
    public void GetProperty_Instance_NonProperty ()
    {
      Assert.That(
          () => NormalizingMemberInfoFromExpressionUtility.GetProperty((DomainType obj) => obj.InstanceField),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Must hold a property access expression.", "expression"));
    }

    [Test]
    public void GetProperty_Instance_BaseProperty ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetProperty((DomainType obj) => obj.BaseProperty);

      var expected = typeof(DomainType).GetProperty("BaseProperty");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetProperty_Instance_VirtualBaseProperty ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetProperty((DomainType obj) => obj.VirtualBaseProperty);

      var expected = typeof(DomainType).GetProperty("VirtualBaseProperty");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetProperty_Instance_OverridingProperty ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetProperty((DomainType obj) => obj.OverridingProperty);

      var expected = typeof(DomainType).GetProperty("OverridingProperty");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetProperty_Instance_SpecialOverridingProperty ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetProperty((DomainType obj) => obj.SpecialOverridingProperty);

      var expected = typeof(DomainType).GetProperty("SpecialOverridingProperty", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetProperty_Instance_InterfaceProperty ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetProperty((DomainType obj) => obj.InterfaceProperty);

      var expected = typeof(DomainType).GetProperty("InterfaceProperty");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Test]
    public void GetProperty_FromInterface ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetProperty((IDomainInterface obj) => obj.InterfaceProperty);

      var expected = typeof(IDomainInterface).GetProperty("InterfaceProperty");
      Assert.That(member, Is.EqualTo(expected));
    }

    [Ignore("TODO: normalizing interfaces")]
    [Test]
    public void GetProperty_FromCastedInstance ()
    {
      var member = NormalizingMemberInfoFromExpressionUtility.GetProperty((DomainType obj) => ((IDomainInterface)obj).InterfaceProperty);

      var expected = typeof(DomainType).GetProperty("InterfaceProperty");
      Assert.That(member, Is.EqualTo(expected));
    }

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable ValueParameterNotUsed
// ReSharper disable UnusedParameter.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
    public class DomainTypeBase
    {
      public void BaseMethod () { }
      public void VirtualBaseMethod () { }

      public virtual void OverridingVoidMethod () { }
      public virtual int OverridingMethod () { return 0; }
      public virtual void OverridingVoidGenericMethod<T> (T t) { }
      public virtual int OverridingGenericMethod<T> (T t) { return 0; }

      public int BaseProperty { get; set; }
      public virtual int VirtualBaseProperty { get; set; }
      public virtual int OverridingProperty { get; set; }

      public virtual int SpecialOverridingProperty
      {
        get { return 7; }
        internal set { }
      }
    }

    public class DomainType : DomainTypeBase, IDomainInterface
    {
      public static int StaticField;
      public readonly int InstanceField;

      public DomainType ()
      {
        StaticField = 0;
        InstanceField = 0;
      }

      public static void StaticVoidMethod () { }
      public static int StaticMethod () { return 0; }
      public void InstanceVoidMethod () { }
      public int InstanceMethod () { return 0; }

      public static void StaticVoidGenericMethod<T> (T t) { }
      public static int StaticGenericMethod<T> (T t) { return 0; }
      public void InstanceVoidGenericMethod<T> (T t) { }
      public int InstanceGenericMethod<T> (T t) { return 0; }

      public override void OverridingVoidMethod () { }
      public override int OverridingMethod () { return 0; }
      public override void OverridingVoidGenericMethod<T> (T t) { }
      public override int OverridingGenericMethod<T> (T t) { return 0; }

      public static int StaticProperty { get; set; }
      public int InstanceProperty { get; set; }
      public override int OverridingProperty { get; set; }

      public override int SpecialOverridingProperty
      {
        // No accessor
        internal set { }
      }

      public void InterfaceMethod () { }
      public int InterfaceProperty { get; set; }
    }

    public interface IDomainInterface
    {
      void InterfaceMethod ();
      int InterfaceProperty { get; set; }
    }
  }
// ReSharper restore UnusedMemberInSuper.Global
// ReSharper restore ValueParameterNotUsed
// ReSharper restore UnusedParameter.Global
// ReSharper restore UnusedAutoPropertyAccessor.Global
}
