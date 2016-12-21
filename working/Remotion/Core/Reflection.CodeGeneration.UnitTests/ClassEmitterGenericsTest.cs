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
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration.UnitTests.TestDomain;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class ClassEmitterGenericsTest : CodeGenerationTestBase
  {
    [Test]
    public void DeriveFromSimpleOpenGenericType ()
    {
      Type baseType = typeof (List<>);
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "DeriveFromSimpleOpenGenericType", baseType, Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);
      Type builtType = classEmitter.BuildType ();

      Assert.That (builtType.ContainsGenericParameters, Is.True);
      Type[] typeParameters = builtType.GetGenericArguments ();
      Assert.That (typeParameters.Length, Is.EqualTo (1));
      Assert.That (builtType.BaseType.ContainsGenericParameters, Is.True);
      Assert.That (builtType.BaseType.GetGenericArguments ()[0], Is.EqualTo (typeParameters[0]));
    }

    [Test]
    public void DeriveFromClosedGenericTypeWithConstraints ()
    {
      Type baseType = typeof (GenericClassWithConstraints<ICloneable, List<string>, int, object, ICloneable, List<List<ICloneable[]>>>);
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "DeriveFromClosedGenericTypeWithConstraints", baseType, Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);
      Type builtType = classEmitter.BuildType ();
      Assert.That (builtType.ContainsGenericParameters, Is.False);
      Assert.That (builtType.BaseType.ContainsGenericParameters, Is.False);
      Assert.That (builtType.BaseType, Is.EqualTo (typeof (GenericClassWithConstraints<ICloneable, List<string>, int, object, ICloneable, List<List<ICloneable[]>>>)));
    }

    [Test]
    public void DeriveFromOpenGenericTypeWithConstraints ()
    {
      Type baseType = typeof (GenericClassWithConstraints<,,,,,>);
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "DeriveFromOpenGenericTypeWithConstraints", baseType, Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);
      Type builtType = classEmitter.BuildType ();
      Assert.That (builtType.ContainsGenericParameters, Is.True);
      Type[] typeParameters = builtType.GetGenericArguments ();
      Assert.That (typeParameters.Length, Is.EqualTo (6));
      Assert.That (builtType.BaseType.ContainsGenericParameters, Is.True);
      Assert.That (builtType.BaseType.GetGenericArguments ()[0], Is.EqualTo (typeParameters[0]));
    }

    [Test]
    public void OverrideSimpleGenericMethod ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "OverrideSimpleGenericMethod", typeof (ClassWithSimpleGenericMethod), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = typeof (ClassWithSimpleGenericMethod).GetMethod ("GenericMethod");
      var methodEmitter = classEmitter.CreateMethodOverride (baseMethod);
      methodEmitter.ImplementByBaseCall (baseMethod);

      Type builtType = classEmitter.BuildType ();
      ClassWithSimpleGenericMethod instance = (ClassWithSimpleGenericMethod) Activator.CreateInstance (builtType);

      string result = instance.GenericMethod ("1", 2, false);
      Assert.That (result, Is.EqualTo ("1, 2, False"));
    }

    [Test]
    public void OverrideConstrainedGenericMethod ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "OverrideConstrainedGenericMethod", typeof (ClassWithConstrainedGenericMethod), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = typeof (ClassWithConstrainedGenericMethod).GetMethod ("GenericMethod");
      var methodEmitter = classEmitter.CreateMethodOverride (baseMethod);
      methodEmitter.ImplementByBaseCall (baseMethod);

      Type builtType = classEmitter.BuildType ();
      ClassWithConstrainedGenericMethod instance = (ClassWithConstrainedGenericMethod) Activator.CreateInstance (builtType);

      string result = instance.GenericMethod ("1", 2, "2");
      Assert.That (result, Is.EqualTo ("1, 2, 2"));
    }

    [Test]
    [Ignore ("Currently not supported by DynamicProxy.")]
    public void OverrideGenericMethod_WithConstraint_ModifiedByClosedGenericClass ()
    {
      Type baseType = typeof (GenericClassWithGenericMethod<IConvertible, List<string>, int, object, IConvertible, List<List<IConvertible[]>>>);
      var classEmitter = new CustomClassEmitter (
          Scope,
          "OverrideGenericMethod_WithConstraint_ModifiedByClosedGenericClass",
          baseType,
          Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = baseType.GetMethod ("GenericMethod");

      var methodEmitter = classEmitter.CreateMethodOverride (baseMethod);
      methodEmitter.ImplementByBaseCall (baseMethod);

      Type builtType = classEmitter.BuildType ();
      var instance =
          (GenericClassWithGenericMethod<IConvertible, List<string>, int, object, IConvertible, List<List<IConvertible[]>>>)
          Activator.CreateInstance (builtType);

      string result = instance.GenericMethod (1, new List<int[]> (), new List<List<IConvertible[]>> ());
      Assert.That (result, Is.EqualTo ("1, System.Collections.Generic.List`1[System.Int32[]], System.Collections.Generic.List`1[System.Collections.Generic.List`1["
                                       + "System.IConvertible[]]]"));
    }

    [Test]
    public void OverridingSimpleMembersOfClosedGenericClass ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "OverridingSimpleMembersOfClosedGenericClass", typeof (GenericClassWithAllKindsOfMembers<int>), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = typeof (GenericClassWithAllKindsOfMembers<int>).GetMethod ("Method");
      var overriddenMethod = classEmitter.CreateMethodOverride (baseMethod);
      overriddenMethod.ImplementByBaseCall (baseMethod);

      PropertyInfo baseProperty = typeof (GenericClassWithAllKindsOfMembers<int>).GetProperty ("Property");
      CustomPropertyEmitter overriddenProperty = classEmitter.CreatePropertyOverride (baseProperty);
      overriddenProperty.GetMethod = classEmitter.CreateMethodOverride (baseProperty.GetGetMethod ());
      overriddenProperty.GetMethod.ImplementByBaseCall (baseProperty.GetGetMethod ());

      EventInfo baseEvent = typeof (GenericClassWithAllKindsOfMembers<int>).GetEvent ("Event");
      CustomEventEmitter overriddenEvent = classEmitter.CreateEventOverride (baseEvent);
      overriddenEvent.AddMethod = classEmitter.CreateMethodOverride (baseEvent.GetAddMethod ());
      overriddenEvent.AddMethod.ImplementByBaseCall (baseEvent.GetAddMethod ());
      overriddenEvent.RemoveMethod = classEmitter.CreateMethodOverride (baseEvent.GetRemoveMethod ());
      overriddenEvent.RemoveMethod.ImplementByBaseCall (baseEvent.GetRemoveMethod ());

      Type builtType = classEmitter.BuildType ();
      GenericClassWithAllKindsOfMembers<int> instance = (GenericClassWithAllKindsOfMembers<int>) Activator.CreateInstance (builtType);

      instance.Method (5);
      Assert.That (instance.Property, Is.EqualTo (0));
      instance.Event += delegate { return 0;  };
      instance.Event -= delegate { return 0;  };
    }

    [Test]
    public void OverridingSimpleMembersOfOpenGenericClass ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "OverridingSimpleMembersOfOpenGenericClass", typeof (GenericClassWithAllKindsOfMembers<>), Type.EmptyTypes,
          TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = typeof (GenericClassWithAllKindsOfMembers<>).GetMethod ("Method");
      var overriddenMethod = classEmitter.CreateMethodOverride (baseMethod);
      overriddenMethod.ImplementByBaseCall (baseMethod);

      PropertyInfo baseProperty = typeof (GenericClassWithAllKindsOfMembers<>).GetProperty ("Property");
      CustomPropertyEmitter overriddenProperty = classEmitter.CreatePropertyOverride (baseProperty);
      overriddenProperty.GetMethod = classEmitter.CreateMethodOverride (baseProperty.GetGetMethod ());
      overriddenProperty.GetMethod.ImplementByBaseCall (baseProperty.GetGetMethod ());

      EventInfo baseEvent = typeof (GenericClassWithAllKindsOfMembers<>).GetEvent ("Event");
      CustomEventEmitter overriddenEvent = classEmitter.CreateEventOverride (baseEvent);
      overriddenEvent.AddMethod = classEmitter.CreateMethodOverride (baseEvent.GetAddMethod ());
      overriddenEvent.AddMethod.ImplementByBaseCall (baseEvent.GetAddMethod ());
      overriddenEvent.RemoveMethod = classEmitter.CreateMethodOverride (baseEvent.GetRemoveMethod ());
      overriddenEvent.RemoveMethod.ImplementByBaseCall (baseEvent.GetRemoveMethod ());

      Type builtType = classEmitter.BuildType ();
      GenericClassWithAllKindsOfMembers<int> instance =
          (GenericClassWithAllKindsOfMembers<int>) Activator.CreateInstance (builtType.MakeGenericType (typeof (int)));

      instance.Method (5);
      Assert.That (instance.Property, Is.EqualTo (0));
      instance.Event += delegate { return 0; };
      instance.Event -= delegate { return 0; };
    }

    [Test]
    public void ImplementingSimpleMembersOfOpenGenericInterface ()
    {
      CustomClassEmitter classEmitter =
          new CustomClassEmitter (
              Scope,
              "ImplementingSimpleMembersOfOpenGenericInterface",
              typeof (object),
              new Type[] {typeof (GenericInterfaceWithAllKindsOfMembers<int>)},
              TypeAttributes.Public | TypeAttributes.Class, false);

      MethodInfo baseMethod = typeof (GenericInterfaceWithAllKindsOfMembers<int>).GetMethod ("Method");
      var overriddenMethod = classEmitter.CreateInterfaceMethodImplementation (baseMethod);
      overriddenMethod.AddStatement (new ReturnStatement());

      PropertyInfo baseProperty = typeof (GenericInterfaceWithAllKindsOfMembers<int>).GetProperty ("Property");
      CustomPropertyEmitter overriddenProperty = classEmitter.CreateInterfacePropertyImplementation (baseProperty);
      overriddenProperty.GetMethod = classEmitter.CreateInterfaceMethodImplementation (baseProperty.GetGetMethod ());
      overriddenProperty.GetMethod.AddStatement (new ReturnStatement (new ConstReference (13)));

      EventInfo baseEvent = typeof (GenericInterfaceWithAllKindsOfMembers<int>).GetEvent ("Event");
      CustomEventEmitter overriddenEvent = classEmitter.CreateInterfaceEventImplementation (baseEvent);
      overriddenEvent.AddMethod = classEmitter.CreateInterfaceMethodImplementation (baseEvent.GetAddMethod ());
      overriddenEvent.AddMethod.AddStatement (new ReturnStatement ());
      overriddenEvent.RemoveMethod = classEmitter.CreateInterfaceMethodImplementation (baseEvent.GetRemoveMethod ());
      overriddenEvent.RemoveMethod.AddStatement (new ReturnStatement ());

      Type builtType = classEmitter.BuildType ();
      GenericInterfaceWithAllKindsOfMembers<int> instance = (GenericInterfaceWithAllKindsOfMembers<int>) Activator.CreateInstance (builtType);

      instance.Method (7);
      Assert.That (instance.Property, Is.EqualTo (13));
      instance.Event += delegate { return 0; };
      instance.Event -= delegate { return 0; };
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "This emitter does not support nested types of non-closed generic types.")]
    public void ClassEmitterThrowsOnNestedGenericBase ()
    {
      new CustomClassEmitter (Scope, "ClassEmitterThrowsOnNestedGenericBase", typeof (GenericClassWithNested<>.Nested));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "This emitter does not support open constructed types as base types. "
        + "Specify a closed type or a generic type definition.")]
    public void ClassEmitterThrowsOnOpenConstructedBase ()
    {
      Type genericParameter = typeof (ClassWithSimpleGenericMethod).GetMethod ("GenericMethod").GetGenericArguments()[0];
      Type openConstructedType = typeof (GenericClassWithNested<>).MakeGenericType (genericParameter);
      new CustomClassEmitter (Scope, "ClassEmitterThrowsOnOpenConstructedBase", openConstructedType);
    }
  }
}
