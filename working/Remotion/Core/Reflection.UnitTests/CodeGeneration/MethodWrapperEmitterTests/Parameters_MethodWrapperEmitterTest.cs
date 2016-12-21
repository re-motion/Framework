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
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection.UnitTests.CodeGeneration.MethodWrapperEmitterTests.TestDomain;

namespace Remotion.Reflection.UnitTests.CodeGeneration.MethodWrapperEmitterTests
{
  [TestFixture]
  public class Parameters_MethodWrapperEmitterTest : MethodWrapperEmitterTestBase
  {
    [Test]
    public void EmitMethodBody_ForInstanceMethodWithReferenceTypeParameter_PublicParameterTypeIsBaseType ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      var value = new SimpleReferenceType();
      var obj = new ClassWithMethods();
      BuildTypeAndInvokeMethod (method, obj, value);

      Assert.That (obj.InstanceReferenceTypeValue, Is.SameAs (value));
    }

    [Test]
    public void EmitMethodBody_ForInstanceMethodWithReferenceTypeParameter_PublicParameterTypeIsBaseType_WithNull ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      var obj = new ClassWithMethods();
      obj.InstanceReferenceTypeValue = new SimpleReferenceType();
      BuildTypeAndInvokeMethod (method, obj, null);

      Assert.That (obj.InstanceReferenceTypeValue, Is.Null);
    }

    [Test]
    public void EmitMethodBody_ForInstanceMethodWithReferenceTypeParameter_ParameterTypesMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (SimpleReferenceType) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      var value = new SimpleReferenceType();
      var obj = new ClassWithMethods();
      BuildTypeAndInvokeMethod (method, obj, value);

      Assert.That (obj.InstanceReferenceTypeValue, Is.SameAs (value));
    }

    [Test]
    public void EmitMethodBody_ForInstanceMethodWithValueTypeParameter_PublicParameterTypeIsBaseType ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithValueTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      var value = 100;
      var obj = new ClassWithMethods();
      BuildTypeAndInvokeMethod (method, obj, value);

      Assert.That (obj.InstanceValueTypeValue, Is.EqualTo (value));
    }

    [Test]
    public void EmitMethodBody_ForInstanceMethodWithValueTypeParameter_ParameterTypesMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithValueTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (int) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      var value = 100;
      var obj = new ClassWithMethods();
      BuildTypeAndInvokeMethod (method, obj, value);

      Assert.That (obj.InstanceValueTypeValue, Is.EqualTo (value));
    }

    [Test]
    public void EmitMethodBody_ForInstanceMethodWithNullableValueTypeParameter_PublicParameterTypeIsBaseType ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithNullableValueTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      int? value = 100;
      var obj = new ClassWithMethods();
      BuildTypeAndInvokeMethod (method, obj, value);

      Assert.That (obj.InstanceNullableValueTypeValue, Is.EqualTo (value));
    }

    [Test]
    public void EmitMethodBody_ForInstanceMethodWithNullableValueTypeParameter_PublicParameterTypeIsBaseType_WithNull ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithNullableValueTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      int? value = null;
      var obj = new ClassWithMethods();
      BuildTypeAndInvokeMethod (method, obj, value);

      Assert.That (obj.InstanceNullableValueTypeValue, Is.Null);
    }

    [Test]
    public void EmitMethodBody_ForInstanceMethodWithNullableValueTypeParameter_ParameterTypesMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithNullableValueTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (int?) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      int? value = 100;
      var obj = new ClassWithMethods();
      BuildTypeAndInvokeMethod (method, obj, value);

      Assert.That (obj.InstanceNullableValueTypeValue, Is.EqualTo (value));
    }

    [Test]
    public void EmitMethodBody_ForInstanceMethodWithMultipleParameters ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithMultipleParameters", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (object), typeof (object), typeof (object), typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      var instanceReferenceTypeValue = new SimpleReferenceType();
      int instanceValueTypeValue = 100;
      int? instanceNullableValueTypeValue = 200;
      var staticReferenceTypeValue = new SimpleReferenceType();
      var obj = new ClassWithMethods();
      BuildTypeAndInvokeMethod (
          method, obj, instanceReferenceTypeValue, instanceValueTypeValue, instanceNullableValueTypeValue, staticReferenceTypeValue);

      Assert.That (obj.InstanceReferenceTypeValue, Is.SameAs (instanceReferenceTypeValue));
      Assert.That (obj.InstanceValueTypeValue, Is.EqualTo (instanceValueTypeValue));
      Assert.That (obj.InstanceNullableValueTypeValue, Is.EqualTo (instanceNullableValueTypeValue));
      Assert.That (ClassWithMethods.StaticReferenceTypeValue, Is.SameAs (staticReferenceTypeValue));
    }

    [Test]
    public void EmitMethodBody_ForStaticMethodWithReferenceTypeParameter ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("StaticMethodWithReferenceTypeParameter", BindingFlags.Public | BindingFlags.Static);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      var value = new SimpleReferenceType();
      BuildTypeAndInvokeMethod (method, new object[] { null, value });

      Assert.That (ClassWithMethods.StaticReferenceTypeValue, Is.SameAs (value));
    }
  }
}