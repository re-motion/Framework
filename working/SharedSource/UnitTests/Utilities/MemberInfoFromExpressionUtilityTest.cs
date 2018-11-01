// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class MemberInfoFromExpressionUtilityTest
  {
    private class SampleType
    {
    }

    [Test]
    public void GetMember_Static_MemberExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember (() => DomainType.StaticField);

      var expected = typeof (DomainType).GetMember ("StaticField").Single ();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMember_Static_MethodCallExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember (() => DomainType.StaticMethod ());

      var expected = typeof (DomainType).GetMember ("StaticMethod").Single ();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMember_Static_NewExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember (() => new DomainType ());

      var expected = typeof (DomainType).GetMember (".ctor").Single ();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Must be a MemberExpression, MethodCallExpression or NewExpression.\r\nParameter name: expression")]
    public void GetMember_Static_InvalidExpression ()
    {
      MemberInfoFromExpressionUtility.GetMember (() => 1);
    }

    [Test]
    public void GetMember_Instance_MemberExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember ((DomainType obj) => obj.InstanceProperty);

      var expected = typeof (DomainType).GetMember ("InstanceProperty").Single ();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMember_Instance_MemberExpression_OverridingProperty ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember ((DomainType obj) => obj.OverridingProperty);

      var expected = typeof (DomainTypeBase).GetProperty ("OverridingProperty");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMember_Instance_MethodCallExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember ((DomainType obj) => obj.InstanceMethod ());

      var expected = typeof (DomainType).GetMember ("InstanceMethod").Single ();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMember_Instance_NewExpression ()
    {
      var member = MemberInfoFromExpressionUtility.GetMember ((DomainType obj) => new DomainType ());

      var expected = typeof (DomainType).GetMember (".ctor").Single ();
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Must be a MemberExpression, MethodCallExpression or NewExpression.\r\nParameter name: expression")]
    public void GetMember_Instance_InvalidExpression ()
    {
      MemberInfoFromExpressionUtility.GetMember ((DomainType obj) => 1);
    }

    [Test]
    public void GetField_Static ()
    {
      var member = MemberInfoFromExpressionUtility.GetField (() => DomainType.StaticField);

      var expected = typeof (DomainType).GetField ("StaticField");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MemberExpression.\r\nParameter name: expression")]
    public void GetField_Static_NonMemberExpression ()
    {
      MemberInfoFromExpressionUtility.GetField (() => DomainType.StaticMethod ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a field access expression.\r\nParameter name: expression")]
    public void GetField_Static_NonField ()
    {
      MemberInfoFromExpressionUtility.GetField (() => DomainType.StaticProperty);
    }

    [Test]
    public void GetField_Instance ()
    {
      var member = MemberInfoFromExpressionUtility.GetField ((DomainType obj) => obj.InstanceField);

      var expected = typeof (DomainType).GetField ("InstanceField");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MemberExpression.\r\nParameter name: expression")]
    public void GetField_Instance_NonMemberExpression ()
    {
      MemberInfoFromExpressionUtility.GetField ((DomainType obj) => obj.InstanceMethod ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a field access expression.\r\nParameter name: expression")]
    public void GetField_Instance_NonField ()
    {
      MemberInfoFromExpressionUtility.GetField ((DomainType obj) => obj.InstanceProperty);
    }

    [Test]
    public void GetConstructor ()
    {
      var member = MemberInfoFromExpressionUtility.GetConstructor (() => new DomainType ());

      var expected = typeof (DomainType).GetConstructor (Type.EmptyTypes);
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a NewExpression.\r\nParameter name: expression")]
    public void GetConstructor_NonNewExpression ()
    {
      MemberInfoFromExpressionUtility.GetConstructor (() => DomainType.StaticField);
    }

    [Test]
    public void GetMethod_StaticVoid ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod (() => DomainType.StaticVoidMethod ());

      var expected = typeof (DomainType).GetMethod ("StaticVoidMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Static ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod (() => DomainType.StaticMethod ());

      var expected = typeof (DomainType).GetMethod ("StaticMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Static_VoidGeneric ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod (() => DomainType.StaticVoidGenericMethod<SampleType> (null));

      var expected = typeof (DomainType).GetMethod ("StaticVoidGenericMethod").MakeGenericMethod (typeof (SampleType));
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Static_Generic ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod (() => DomainType.StaticGenericMethod<SampleType> (null));

      var expected = typeof (DomainType).GetMethod ("StaticGenericMethod").MakeGenericMethod (typeof (SampleType));
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MethodCallExpression.\r\nParameter name: expression")]
    public void GetMethod_Static_NonMethodCallExpression ()
    {
      MemberInfoFromExpressionUtility.GetMethod (() => DomainType.StaticProperty);
    }

    [Test]
    public void GetMethod_InstanceVoid ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InstanceVoidMethod ());

      var expected = typeof (DomainType).GetMethod ("InstanceVoidMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InstanceMethod ());

      var expected = typeof (DomainType).GetMethod ("InstanceMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance_OverridingVoid ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.OverridingVoidMethod ());

      var expected = typeof (DomainTypeBase).GetMethod ("OverridingVoidMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance_Overriding ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.OverridingMethod ());

      var expected = typeof (DomainTypeBase).GetMethod ("OverridingMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance_OverridingVoid_WithNonBaseMethodInExpressionTree ()
    {
      // The C# compiler always inserts the root method definition for virtual methods into expression trees.
      // To test behavior with non-root definitions, we need to construct an expression tree manually.
      var parameter = Expression.Parameter (typeof (DomainType), "obj");
      var method = typeof (DomainType).GetMethod ("OverridingVoidMethod");
      var expression = Expression.Lambda<Action<DomainType>> (Expression.Call (parameter, method), parameter);

      var member = MemberInfoFromExpressionUtility.GetMethod (expression);

      Assert.That (member, Is.EqualTo (method));
    }

    [Test]
    public void GetMethod_Instance_Overriding_WithNonBaseMethodInExpressionTree ()
    {
      // The C# compiler always inserts the root method definition for virtual methods into expression trees.
      // To test behavior with non-root definitions, we need to construct an expression tree manually.
      var parameter = Expression.Parameter (typeof (DomainType), "obj");
      var method = typeof (DomainType).GetMethod ("OverridingMethod");
      var expression = Expression.Lambda<Func<DomainType, int>> (Expression.Call (parameter, method), parameter);

      var member = MemberInfoFromExpressionUtility.GetMethod (expression);

      Assert.That (member, Is.EqualTo (method));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MethodCallExpression.\r\nParameter name: expression")]
    public void GetMethod_Instance_NonMethodCallExpression ()
    {
      MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InstanceProperty);
    }

    [Test]
    public void GetMethod_Instance_VoidGenericMethod ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InstanceVoidGenericMethod<SampleType> (null));

      var expected = typeof (DomainType).GetMethod ("InstanceVoidGenericMethod").MakeGenericMethod (typeof (SampleType));
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance_GenericMethod ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InstanceGenericMethod<SampleType> (null));

      var expected = typeof (DomainType).GetMethod ("InstanceGenericMethod").MakeGenericMethod (typeof (SampleType));
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance_OverridingVoidGenericMethod ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.OverridingVoidGenericMethod<SampleType> (null));

      var expected = typeof (DomainTypeBase).GetMethod ("OverridingVoidGenericMethod").MakeGenericMethod (typeof (SampleType));
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance_OverridingGenericMethod ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.OverridingGenericMethod<SampleType> (null));

      var expected = typeof (DomainTypeBase).GetMethod ("OverridingGenericMethod").MakeGenericMethod (typeof (SampleType));
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance_OverridingVoidGeneric_WithNonBaseMethodInExpressionTree ()
    {
      // The C# compiler always inserts the root method definition for virtual methods into expression trees.
      // To test behavior with non-root definitions, we need to construct an expression tree manually.
      var parameter = Expression.Parameter (typeof (DomainType), "obj");
      var method = typeof (DomainType).GetMethod ("OverridingVoidGenericMethod").MakeGenericMethod (typeof (SampleType));
      var expression = Expression.Lambda<Action<DomainType>> (
          Expression.Call (parameter, method, Expression.Constant (null, typeof (SampleType))), parameter);

      var member = MemberInfoFromExpressionUtility.GetMethod (expression);

      Assert.That (member, Is.EqualTo (method));
    }

    [Test]
    public void GetMethod_Instance_OverridingGeneric_WithNonBaseMethodInExpressionTree ()
    {
      // The C# compiler always inserts the root method definition for virtual methods into expression trees.
      // To test behavior with non-root definitions, we need to construct an expression tree manually.
      var parameter = Expression.Parameter (typeof (DomainType), "obj");
      var method = typeof (DomainType).GetMethod ("OverridingGenericMethod").MakeGenericMethod (typeof (SampleType));
      var expression = Expression.Lambda<Func<DomainType, int>> (
          Expression.Call (parameter, method, Expression.Constant (null, typeof (SampleType))), parameter);

      var member = MemberInfoFromExpressionUtility.GetMethod (expression);

      Assert.That (member, Is.EqualTo (method));
    }

    [Test]
    public void GetMethod_Instance_BaseMethod ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.BaseMethod ());

      var expected = typeof (DomainTypeBase).GetMethod ("BaseMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance_VirtualBaseMethod ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.VirtualBaseMethod ());

      var expected = typeof (DomainTypeBase).GetMethod ("VirtualBaseMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_Instance_InterfaceMethod ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InterfaceMethod ());

      var expected = typeof (DomainType).GetMethod ("InterfaceMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetMethod_FromInterface ()
    {
      var member = MemberInfoFromExpressionUtility.GetMethod ((IDomainInterface obj) => obj.InterfaceMethod ());

      var expected = typeof (IDomainInterface).GetMethod ("InterfaceMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetGenericMethodDefinition_StaticVoid ()
    {
      var member = MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => DomainType.StaticVoidGenericMethod<SampleType> (null));

      var expected = typeof (DomainType).GetMethod ("StaticVoidGenericMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a generic method access expression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_StaticVoid_NonGenericMethod ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => DomainType.StaticVoidMethod ());
    }

    [Test]
    public void GetGenericMethodDefinition_Static ()
    {
      var member = MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => DomainType.StaticGenericMethod<SampleType> (null));

      var expected = typeof (DomainType).GetMethod ("StaticGenericMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MethodCallExpression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_Static_NonMethodCallExpression ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => DomainType.StaticProperty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a generic method access expression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_Static_NonGenericMethod ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition (() => DomainType.StaticMethod ());
    }

    [Test]
    public void GetGenericMethodDefinition_Instance_Void ()
    {
      var member = MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.InstanceVoidGenericMethod<SampleType> (null));

      var expected = typeof (DomainType).GetMethod ("InstanceVoidGenericMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetGenericMethodDefinition_Instance ()
    {
      var member = MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.InstanceGenericMethod<SampleType> (null));

      var expected = typeof (DomainType).GetMethod ("InstanceGenericMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetGenericMethodDefinition_Instance_OverridingVoidMethod ()
    {
      var member = MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.OverridingVoidGenericMethod<SampleType> (null));

      var expected = typeof (DomainTypeBase).GetMethod ("OverridingVoidGenericMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetGenericMethodDefinition_Instance_OverridingMethod ()
    {
      var member = MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.OverridingGenericMethod<SampleType> (null));

      var expected = typeof (DomainTypeBase).GetMethod ("OverridingGenericMethod");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MethodCallExpression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_Instance_NonMethodCallExpression ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.InstanceProperty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a generic method access expression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_Instance_VoidNonGenericMethod ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.InstanceVoidMethod ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a generic method access expression.\r\nParameter name: expression")]
    public void GetGenericMethodDefinition_Instance_NonGenericMethod ()
    {
      MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((DomainType obj) => obj.InstanceMethod ());
    }

    [Test]
    public void GetProperty_Static ()
    {
      var member = MemberInfoFromExpressionUtility.GetProperty (() => DomainType.StaticProperty);

      var expected = typeof (DomainType).GetProperty ("StaticProperty");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MemberExpression.\r\nParameter name: expression")]
    public void GetProperty_Static_NonMemberExpression ()
    {
      MemberInfoFromExpressionUtility.GetProperty (() => DomainType.StaticMethod ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a property access expression.\r\nParameter name: expression")]
    public void GetProperty_Static_NonProperty ()
    {
      MemberInfoFromExpressionUtility.GetProperty (() => DomainType.StaticField);
    }

    [Test]
    public void GetProperty_Instance ()
    {
      var member = MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.InstanceProperty);

      var expected = typeof (DomainType).GetProperty ("InstanceProperty");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must be a MemberExpression.\r\nParameter name: expression")]
    public void GetProperty_Instance_NonMemberExpression ()
    {
      MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.InstanceMethod ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Must hold a property access expression.\r\nParameter name: expression")]
    public void GetProperty_Instance_NonProperty ()
    {
      MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.InstanceField);
    }

    [Test]
    public void GetProperty_Instance_BaseProperty ()
    {
      var member = MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.BaseProperty);

      var expected = typeof (DomainTypeBase).GetProperty ("BaseProperty");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetProperty_Instance_VirtualBaseProperty ()
    {
      var member = MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.VirtualBaseProperty);

      var expected = typeof (DomainTypeBase).GetProperty ("VirtualBaseProperty");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetProperty_Instance_OverridingProperty ()
    {
      var member = MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.OverridingProperty);

      var expected = typeof (DomainTypeBase).GetProperty ("OverridingProperty");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetProperty_Instance_SpecialOverridingProperty ()
    {
      var member = MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.SpecialOverridingProperty);

      var expected = typeof (DomainTypeBase).GetProperty ("SpecialOverridingProperty");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetProperty_Instance_InterfaceProperty ()
    {
      var member = MemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.InterfaceProperty);

      var expected = typeof (DomainType).GetProperty ("InterfaceProperty");
      Assert.That (member, Is.EqualTo (expected));
    }

    [Test]
    public void GetProperty_FromInterface ()
    {
      var member = MemberInfoFromExpressionUtility.GetProperty ((IDomainInterface obj) => obj.InterfaceProperty);

      var expected = typeof (IDomainInterface).GetProperty ("InterfaceProperty");
      Assert.That (member, Is.EqualTo (expected));
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