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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class InterfaceImplementationPropertyInformationTest
  {
    private IPropertyInformation _implementationPropertyInformationStub;
    private IPropertyInformation _declarationPropertyInformationStub;
    private InterfaceImplementationPropertyInformation _interfaceImplementationPropertyInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationPropertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _declarationPropertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          _implementationPropertyInformationStub, _declarationPropertyInformationStub);
    }

    [Test]
    public void Name ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.Name).Return ("Test");

      Assert.That (_interfaceImplementationPropertyInformation.Name, Is.EqualTo ("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      _implementationPropertyInformationStub.Stub (stub => stub.DeclaringType).Return (typeInformationStub);

      Assert.That (_interfaceImplementationPropertyInformation.DeclaringType, Is.SameAs (typeInformationStub));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation> ();
      _implementationPropertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (typeInformationStub);

      Assert.That (_interfaceImplementationPropertyInformation.GetOriginalDeclaringType(), Is.SameAs (typeInformationStub));
    }

    [Test]
    public void GetOriginalDeclaration ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      _implementationPropertyInformationStub.Stub (stub => stub.GetOriginalDeclaration()).Return (propertyInformationStub);

      Assert.That (_interfaceImplementationPropertyInformation.GetOriginalDeclaration(), Is.SameAs (propertyInformationStub));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.GetCustomAttribute<string> (false)).Return ("Test");

      Assert.That (_interfaceImplementationPropertyInformation.GetCustomAttribute<string> (false), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new string[0];
      _implementationPropertyInformationStub.Stub (stub => stub.GetCustomAttributes<string> (false)).Return (objToReturn);

      Assert.That (_interfaceImplementationPropertyInformation.GetCustomAttributes<string> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.IsDefined<Attribute> (false)).Return (false);

      Assert.That (_interfaceImplementationPropertyInformation.IsDefined<Attribute> (false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var propertyInfoAdapter = PropertyInfoAdapter.Create(typeof (string).GetProperty ("Length"));
      _implementationPropertyInformationStub.Stub (stub => stub.FindInterfaceImplementation (typeof (bool))).Return (propertyInfoAdapter);

      Assert.That (_interfaceImplementationPropertyInformation.FindInterfaceImplementation (typeof (bool)), Is.SameAs (propertyInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That (_interfaceImplementationPropertyInformation.FindInterfaceDeclarations(), Is.EqualTo (new[] { _declarationPropertyInformationStub }));
    }

    [Test]
    public void GetIndexParameters ()
    {
      var objToReturn = new ParameterInfo[0];
      _implementationPropertyInformationStub.Stub (stub => stub.GetIndexParameters()).Return (objToReturn);

      Assert.That (_interfaceImplementationPropertyInformation.GetIndexParameters(), Is.SameAs (objToReturn));
    }

    [Test]
    public void GetAccessors ()
    {
      var objToReturn = new IMethodInformation[0];
      _implementationPropertyInformationStub.Stub (stub => stub.GetAccessors (false)).Return (objToReturn);

      Assert.That (_interfaceImplementationPropertyInformation.GetAccessors (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void PropertyType ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.PropertyType).Return (typeof (bool));

      Assert.That (_interfaceImplementationPropertyInformation.PropertyType, Is.SameAs (typeof (bool)));
    }

    [Test]
    public void SetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();

      _declarationPropertyInformationStub.Stub (stub => stub.GetSetMethod (true)).Return (
          MethodInfoAdapter.Create(typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar").GetSetMethod (true)));
      _implementationPropertyInformationStub.Stub (stub => stub.GetSetMethod (true)).Return (
          MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar").GetSetMethod (true)));

      _interfaceImplementationPropertyInformation.SetValue (instance, value, null);
      Assert.That (instance.ImplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    public void SetValue_ImplementationAddsSetAccessor ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();

      _declarationPropertyInformationStub.Stub (stub => stub.GetSetMethod (true)).Return (null);
      _implementationPropertyInformationStub.Stub (stub => stub.GetSetMethod (true)).Return (
          MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar").GetSetMethod (true)));

      _interfaceImplementationPropertyInformation.SetValue (instance, value, null);
      Assert.That (instance.ImplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    public void SetValue_ImplementationAddsSetAccessor_IndexedProperty ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();

      _declarationPropertyInformationStub.Stub (stub => stub.GetSetMethod (true)).Return (null);
      _implementationPropertyInformationStub.Stub (stub => stub.GetSetMethod (true)).Return (
          MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Item", new[] { typeof (int) }).GetSetMethod (true)));

      _interfaceImplementationPropertyInformation.SetValue (instance, value, new object[] { 0 });
      Assert.That (instance[0], Is.SameAs (value));
    }

    [Test]
    public void GetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();
      instance.ImplicitInterfaceScalar = value;

      _declarationPropertyInformationStub.Stub (stub => stub.GetGetMethod (true)).Return (
          MethodInfoAdapter.Create(typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod (true)));
      _implementationPropertyInformationStub.Stub (stub => stub.GetGetMethod (true)).Return (
          MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod (true)));

      Assert.That (_interfaceImplementationPropertyInformation.GetValue (instance, null), Is.SameAs (value));
    }

    [Test]
    public void GetValue_ImplementationsAddsGetAccessor ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();
      instance.ImplicitInterfaceScalar = value;

      _declarationPropertyInformationStub.Stub (stub => stub.GetGetMethod (true)).Return (null);
      _implementationPropertyInformationStub.Stub (stub => stub.GetGetMethod (true)).Return (
          MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar").GetGetMethod (true)));

      Assert.That (_interfaceImplementationPropertyInformation.GetValue (instance, null), Is.SameAs (value));
    }


    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("ImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("ImplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "get_ImplicitProperty",
          getMethodResult);

      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "set_ImplicitProperty",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty (
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.ExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("ExplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.get_ExplicitProperty",
          getMethodResult);

      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.set_ExplicitProperty",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ReadOnlyImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("ReadOnlyImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("ReadOnlyImplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "get_ReadOnlyImplicitProperty",
          getMethodResult);
      Assert.That (interfaceImplementationPropertyInformation.GetSetMethod (false), Is.Null);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_WriteOnlyImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("WriteOnlyImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("WriteOnlyImplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.GetGetMethod (false), Is.Null);
      var getMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "set_WriteOnlyImplicitProperty",
          getMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ReadOnlyExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty (
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.ReadOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("ReadOnlyExplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.get_ReadOnlyExplicitProperty",
          getMethodResult);
      Assert.That (interfaceImplementationPropertyInformation.GetSetMethod (false), Is.Null);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_WriteOnlyExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty (
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.WriteOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("WriteOnlyExplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.GetGetMethod (false), Is.Null);
      var getMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.set_WriteOnlyExplicitProperty",
          getMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingGetAccessor ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("PropertyAddingGetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("PropertyAddingGetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (MethodInfoAdapter),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "get_PropertyAddingGetAccessor",
          getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "set_PropertyAddingGetAccessor",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitReadOnlyOnlyPropertyImplementationAddingSetAccessor ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("PropertyAddingSetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("PropertyAddingSetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "get_PropertyAddingSetAccessor",
          getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (MethodInfoAdapter),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "set_PropertyAddingSetAccessor",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateGetAccessor_NonPublicFlagTrue ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("PropertyAddingPrivateGetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("PropertyAddingPrivateGetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (true);
      CheckMethodInformation (
          typeof (MethodInfoAdapter),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "get_PropertyAddingPrivateGetAccessor",
          getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "set_PropertyAddingPrivateGetAccessor",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateGetAccessor_NonPublicFlagFalse ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("PropertyAddingPrivateGetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("PropertyAddingPrivateGetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.GetGetMethod (false), Is.Null);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "set_PropertyAddingPrivateGetAccessor",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateSetAccessor_NonPublicFlagTrue ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("PropertyAddingPrivateSetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("PropertyAddingPrivateSetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "get_PropertyAddingPrivateSetAccessor",
          getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod (true);
      CheckMethodInformation (
          typeof (MethodInfoAdapter),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "set_PropertyAddingPrivateSetAccessor",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateSetAccessor_NonPublicFlagFalse ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("PropertyAddingPrivateSetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("PropertyAddingPrivateSetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod (false);
      CheckMethodInformation (
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create (typeof (ClassImplementingInterface)),
          "get_PropertyAddingPrivateSetAccessor",
          getMethodResult);
      Assert.That (interfaceImplementationPropertyInformation.GetSetMethod (false), Is.Null);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("ImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("ImplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty (
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.ExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("ExplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitReadOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("ReadOnlyImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("ReadOnlyImplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitWriteOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("WriteOnlyImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("WriteOnlyImplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitReadOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty (
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.ReadOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("ReadOnlyExplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitWriteOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty (
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.WriteOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("WriteOnlyExplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitPropertyImplementationAddingSetAccessor ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("PropertyAddingSetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("PropertyAddingSetAccessor"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitPropertyImplementationAddingPrivateSetAccessor ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty ("PropertyAddingPrivateSetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty ("PropertyAddingPrivateSetAccessor"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That (interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void EqualsTest ()
    {
      Assert.That (_interfaceImplementationPropertyInformation.Equals (null), Is.False);
      Assert.That (_interfaceImplementationPropertyInformation.Equals ("Test"), Is.False);
      Assert.That (
          _interfaceImplementationPropertyInformation.Equals (
              new InterfaceImplementationPropertyInformation (
                  PropertyInfoAdapter.Create(typeof (string).GetProperty ("Length")),
                  PropertyInfoAdapter.Create(typeof (string).GetProperty ("Length")))),
          Is.False);

      Assert.That (
          _interfaceImplementationPropertyInformation.Equals (
              new InterfaceImplementationPropertyInformation (_implementationPropertyInformationStub, _declarationPropertyInformationStub)),
          Is.True);
    }

    [Test]
    public void GetHashCodeTest ()
    {
      var equalObject = new InterfaceImplementationPropertyInformation (_implementationPropertyInformationStub, _declarationPropertyInformationStub);
      Assert.That (_interfaceImplementationPropertyInformation.GetHashCode(), Is.EqualTo (equalObject.GetHashCode()));
    }

    [Test]
    public void To_String ()
    {
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation> ();
      typeInformationStub.Stub (stub => stub.Name).Return ("Boolean");
      _implementationPropertyInformationStub.Stub (stub => stub.Name).Return ("Test");
      _declarationPropertyInformationStub.Stub (stub => stub.DeclaringType).Return (typeInformationStub);

      Assert.That (_interfaceImplementationPropertyInformation.ToString(), Is.EqualTo ("Test (impl of 'Boolean')"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((IPropertyInformation) _interfaceImplementationPropertyInformation).IsNull, Is.False);
    }

    private void CheckMethodInformation (
        Type expectedType, ITypeInformation expectedPropertyDeclaringType, string expectedPropertyName, IMethodInformation actualMethodInformation)
    {
      Assert.That (actualMethodInformation, Is.TypeOf (expectedType));
      Assert.That (actualMethodInformation.DeclaringType, Is.SameAs (expectedPropertyDeclaringType));
      Assert.That (actualMethodInformation.Name, Is.EqualTo (expectedPropertyName));
    }
  }
}