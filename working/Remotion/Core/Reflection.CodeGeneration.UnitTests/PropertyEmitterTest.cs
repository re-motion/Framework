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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class PropertyEmitterTest : CodeGenerationTestBase
  {
    private CustomClassEmitter _classEmitter;

    public override void SetUp ()
    {
      base.SetUp ();
      _classEmitter = new CustomClassEmitter (Scope, UniqueName, typeof (object), Type.EmptyTypes, TypeAttributes.Public, true); // force unsigned because we use SimpleAttribute below
    }

    public override void TearDown ()
    {
      if (!_classEmitter.HasBeenBuilt)
        _classEmitter.BuildType();

      base.TearDown();
    }

    [Test]
    public void SimpleInstanceProperty ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("SimpleProperty", PropertyKind.Instance, typeof (int));

      Assert.That (property.Name, Is.EqualTo ("SimpleProperty"));
      Assert.That (property.PropertyType, Is.EqualTo (typeof (int)));
      Assert.That (property.PropertyKind, Is.EqualTo (PropertyKind.Instance));
      Assert.That (property.IndexParameters, Is.Empty);

      property.CreateGetMethod();
      property.CreateSetMethod();
      property.ImplementWithBackingField ();

      object instance = BuildInstance ();
      Assert.That (GetPropertyValue (instance, property), Is.EqualTo (0));
      SetPropertyValue (17, instance, property);
      Assert.That (GetPropertyValue (instance, property), Is.EqualTo (17));

      var generatedPropertyInfo = GetProperty (instance, property);
      CheckCallingConvention (generatedPropertyInfo, CallingConventions.Standard | CallingConventions.HasThis);
    }

    public int I { get; set;  }

    [Test]
    public void StaticProperty ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("StaticProperty", PropertyKind.Static, typeof (string));

      Assert.That (property.Name, Is.EqualTo ("StaticProperty"));
      Assert.That (property.PropertyType, Is.EqualTo (typeof (string)));
      Assert.That (property.PropertyKind, Is.EqualTo (PropertyKind.Static));
      Assert.That (property.IndexParameters, Is.Empty);

      property.CreateGetMethod();
      property.CreateSetMethod();
      property.ImplementWithBackingField ();

      Type type = _classEmitter.BuildType ();
      Assert.That (GetPropertyValue (type, property), Is.EqualTo (null));
      SetPropertyValue ("bla", type, property);
      Assert.That (GetPropertyValue (type, property), Is.EqualTo ("bla"));

      var generatedPropertyInfo = GetProperty (type, property);
      CheckCallingConvention (generatedPropertyInfo, CallingConventions.Standard);
    }

    [Test]
    public void IndexedProperty ()
    {
      CustomPropertyEmitter property =
          _classEmitter.CreateProperty (
              "StaticProperty", PropertyKind.Static, typeof (string), new Type[] {typeof (int), typeof (double)}, PropertyAttributes.None);

      Assert.That (property.IndexParameters, Is.EqualTo (new Type[] { typeof (int), typeof (double) }));

      property.CreateGetMethod ();
      property.CreateSetMethod ();
      property.ImplementWithBackingField ();

      Type type = _classEmitter.BuildType ();

      Assert.That (GetPropertyValue (type, property, 2, 2.0), Is.EqualTo (null));
      SetPropertyValue ("whatever", type, property, 2, 2.0);
      Assert.That (GetPropertyValue (type, property, 2, 2.0), Is.EqualTo ("whatever"));
    }

    [Test]
    public void NoGetMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("NoGetMethod", PropertyKind.Static, typeof (string));
      Assert.That (property.GetMethod, Is.Null);
      Type type = _classEmitter.BuildType ();
      Assert.That (GetProperty (type, property).GetGetMethod (), Is.Null);
    }

    [Test]
    public void NoSetMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("NoSetMethod", PropertyKind.Static, typeof (string));
      Assert.That (property.SetMethod, Is.Null);
      Type type = _classEmitter.BuildType ();
      Assert.That (GetProperty (type, property).GetSetMethod (), Is.Null);
    }

    [Test]
    public void SpecificGetMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("SpecificGetMethod", PropertyKind.Static, typeof (string));
      property.CreateGetMethod ().ImplementByReturning (new ConstReference ("You are my shunsine").ToExpression ());
      Type type = _classEmitter.BuildType ();
      Assert.That (GetPropertyValue (type, property), Is.EqualTo ("You are my shunsine"));
    }

    [Test]
    public void SpecificSetMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("SpecificSetMethod", PropertyKind.Static, typeof (string));
      property.CreateSetMethod ().ImplementByThrowing (typeof (Exception), "My only shunsine");
      Type type = _classEmitter.BuildType ();
      try
      {
        SetPropertyValue ("", type, property);
      }
      catch (TargetInvocationException ex)
      {
        Assert.That (ex.InnerException.GetType () == typeof (Exception), Is.True);
        Assert.That (ex.InnerException.Message, Is.EqualTo ("My only shunsine"));
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException),
        ExpectedMessage = "Due to limitations in Reflection.Emit, property accessors cannot be set to null.", MatchType = MessageMatch.Contains)]
    public void GetMethodCannotBeSetToNull()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("GetMethodCannotBeSetToNull", PropertyKind.Static, typeof (string));
      property.CreateGetMethod ();
      property.GetMethod = null;
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException),
        ExpectedMessage = "Due to limitations in Reflection.Emit, property accessors cannot be set to null.", MatchType = MessageMatch.Contains)]
    public void SetMethodCannotBeSetToNull ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("SetMethodCannotBeSetToNull", PropertyKind.Static, typeof (string));
      property.CreateSetMethod ();
      property.SetMethod = null;
    }

    [Test]
    public void ImplementWithBackingFieldStatic ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ( "StaticProperty", PropertyKind.Static, typeof (string));

      property.CreateGetMethod ();
      property.CreateSetMethod ();
      property.ImplementWithBackingField ();

      Type type = _classEmitter.BuildType ();

      FieldInfo backingField = type.GetField ("_fieldForStaticProperty");
      Assert.That (backingField, Is.Not.Null);
      Assert.That (backingField.IsStatic, Is.True);

      SetPropertyValue ("test", type, property);
      Assert.That (backingField.GetValue (null), Is.EqualTo ("test"));

      backingField.SetValue (null, "yup");

      Assert.That (GetPropertyValue (type, property), Is.EqualTo ("yup"));
    }

    [Test]
    public void ImplementWithBackingFieldInstance ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("InstanceProperty", PropertyKind.Instance, typeof (string));

      property.CreateGetMethod ();
      property.CreateSetMethod ();
      property.ImplementWithBackingField ();

      object instance = BuildInstance ();
      Type type = instance.GetType();

      FieldInfo backingField = type.GetField ("_fieldForInstanceProperty");
      Assert.That (backingField, Is.Not.Null);
      Assert.That (backingField.IsStatic, Is.False);

      SetPropertyValue ("what you see", instance, property);
      Assert.That (backingField.GetValue (instance), Is.EqualTo ("what you see"));

      backingField.SetValue (instance, "is what you get");

      Assert.That (GetPropertyValue (instance, property), Is.EqualTo ("is what you get"));
    }

    [Test]
    public void ImplementWithBackingFieldWithoutMethods ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("PropertyWithoutAccessors", PropertyKind.Instance, typeof (string));

      Assert.That (property.GetMethod, Is.Null);
      Assert.That (property.SetMethod, Is.Null);

      property.ImplementWithBackingField ();

      Assert.That (property.GetMethod, Is.Null);
      Assert.That (property.SetMethod, Is.Null);
    }

    [Test]
    public void CreateGetMethodStatic ()
    {
      CustomPropertyEmitter property =
          _classEmitter.CreateProperty (
              "CreateGetMethodStatic", PropertyKind.Static, typeof (string), new Type[] {typeof (int)}, PropertyAttributes.None);

      Assert.That (property.GetMethod, Is.Null);
      var method = property.CreateGetMethod ();
      Assert.That (method.MethodBuilder.IsStatic, Is.True);
      Assert.That (method.ParameterTypes, Is.EqualTo (new Type[] { typeof (int) }));
      Assert.That (method.ReturnType, Is.EqualTo (typeof (string)));
    }

    [Test]
    public void CreateGetMethodInstance ()
    {
      CustomPropertyEmitter property =
          _classEmitter.CreateProperty (
              "CreateGetMethodStatic", PropertyKind.Instance, typeof (string), new Type[] { typeof (int) }, PropertyAttributes.None);

      Assert.That (property.GetMethod, Is.Null);
      var method = property.CreateGetMethod ();
      Assert.That (method.MethodBuilder.IsStatic, Is.False);
      Assert.That (method.ParameterTypes, Is.EqualTo (new Type[] { typeof (int) }));
      Assert.That (method.ReturnType, Is.EqualTo (typeof (string)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This property already has a getter method.")]
    public void CreateGetMethodThrowsOnDuplicateMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("CreateGetMethodThrowsOnDuplicateGetMethod", PropertyKind.Instance,
          typeof (string));

      property.CreateGetMethod ();
      property.CreateGetMethod ();
    }

    [Test]
    public void CreateSetMethodStatic ()
    {
      CustomPropertyEmitter property =
          _classEmitter.CreateProperty (
              "CreateSetMethodStatic", PropertyKind.Static, typeof (string), new Type[] { typeof (int) }, PropertyAttributes.None);

      Assert.That (property.SetMethod, Is.Null);
      var method = property.CreateSetMethod ();
      Assert.That (method.MethodBuilder.IsStatic, Is.True);
      Assert.That (method.ParameterTypes, Is.EqualTo (new Type[] { typeof (int), typeof (string) }));
      Assert.That (method.ReturnType, Is.EqualTo (typeof (void)));
    }

    [Test]
    public void CreateSetMethodInstance ()
    {
      CustomPropertyEmitter property =
          _classEmitter.CreateProperty (
              "CreateSetMethodStatic", PropertyKind.Instance, typeof (string), new Type[] { typeof (int) }, PropertyAttributes.None);

      Assert.That (property.SetMethod, Is.Null);
      var method = property.CreateSetMethod ();
      Assert.That (method.MethodBuilder.IsStatic, Is.False);
      Assert.That (method.ParameterTypes, Is.EqualTo (new Type[] { typeof (int), typeof (string) }));
      Assert.That (method.ReturnType, Is.EqualTo (typeof (void)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This property already has a setter method.")]
    public void CreateSetMethodThrowsOnDuplicateMethod ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("CreateSetMethodThrowsOnDuplicateMethod", PropertyKind.Instance,
          typeof (string));

      property.CreateSetMethod ();
      property.CreateSetMethod ();
    }

    [Test]
    public void AddCustomAttribute ()
    {
      CustomPropertyEmitter property = _classEmitter.CreateProperty ("AddCustomAttribute", PropertyKind.Static, typeof (string));
      property.AddCustomAttribute (new CustomAttributeBuilder (typeof (SimpleAttribute).GetConstructor (Type.EmptyTypes), new object[0]));

      Type type = _classEmitter.BuildType ();
      Assert.That (GetProperty (type, property).IsDefined (typeof (SimpleAttribute), false), Is.True);
      Assert.That (GetProperty (type, property).GetCustomAttributes (false).Length, Is.EqualTo (1));
      Assert.That (GetProperty (type, property).GetCustomAttributes (false)[0], Is.EqualTo (new SimpleAttribute()));
    }

    private void CheckCallingConvention (PropertyInfo generatedPropertyInfo, CallingConventions expectedCallingConventions)
    {
      // Note: There doesn't seem to be any visible effect of choosing the right calling convention, so we'll check the internal Signature property.
      var signature = PrivateInvoke.GetNonPublicProperty (generatedPropertyInfo, "Signature");
      Assertion.IsNotNull (signature, "Internal Signature member on PropertyInfo has been removed - adapt test");
      var callingConvention = (CallingConventions) PrivateInvoke.GetNonPublicProperty (signature, "CallingConvention");
      Assert.That (callingConvention, Is.EqualTo (expectedCallingConventions));
    }

    private object BuildInstance ()
    {
      return Activator.CreateInstance (_classEmitter.BuildType ());
    }

    private object GetPropertyValue (object instance, CustomPropertyEmitter property, params object[] arguments)
    {
      return GetProperty (instance, property).GetValue (instance, arguments);
    }

    private object GetPropertyValue (Type type, CustomPropertyEmitter property, params object[] arguments)
    {
      return GetProperty (type, property).GetValue (null, arguments);
    }

    private void SetPropertyValue (object value, object instance, CustomPropertyEmitter property, params object[] arguments)
    {
      GetProperty (instance, property).SetValue (instance, value, arguments);
    }

    private void SetPropertyValue (object value, Type type, CustomPropertyEmitter property, params object[] arguments)
    {
      GetProperty (type, property).SetValue (null, value, arguments);
    }

    private PropertyInfo GetProperty (Type builtType, CustomPropertyEmitter property)
    {
      return builtType.GetProperty (property.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

    private PropertyInfo GetProperty (object instance, CustomPropertyEmitter property)
    {
      return GetProperty (instance.GetType (), property);
    }

  }
}
