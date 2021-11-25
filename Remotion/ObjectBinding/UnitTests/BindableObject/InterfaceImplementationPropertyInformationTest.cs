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
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class InterfaceImplementationPropertyInformationTest
  {
    private Mock<IPropertyInformation> _implementationPropertyInformationStub;
    private Mock<IPropertyInformation> _declarationPropertyInformationStub;
    private InterfaceImplementationPropertyInformation _interfaceImplementationPropertyInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationPropertyInformationStub = new Mock<IPropertyInformation>();
      _declarationPropertyInformationStub = new Mock<IPropertyInformation>();
      _interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          _implementationPropertyInformationStub.Object, _declarationPropertyInformationStub.Object);
    }

    [Test]
    public void Name ()
    {
      _implementationPropertyInformationStub.Setup(stub => stub.Name).Returns("Test");

      Assert.That(_interfaceImplementationPropertyInformation.Name, Is.EqualTo("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      _implementationPropertyInformationStub.Setup(stub => stub.DeclaringType).Returns(typeInformationStub.Object);

      Assert.That(_interfaceImplementationPropertyInformation.DeclaringType, Is.SameAs(typeInformationStub.Object));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      _implementationPropertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(typeInformationStub.Object);

      Assert.That(_interfaceImplementationPropertyInformation.GetOriginalDeclaringType(), Is.SameAs(typeInformationStub.Object));
    }

    [Test]
    public void GetOriginalDeclaration ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      _implementationPropertyInformationStub.Setup(stub => stub.GetOriginalDeclaration()).Returns(propertyInformationStub.Object);

      Assert.That(_interfaceImplementationPropertyInformation.GetOriginalDeclaration(), Is.SameAs(propertyInformationStub.Object));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      _implementationPropertyInformationStub.Setup(stub => stub.GetCustomAttribute<string>(false)).Returns("Test");

      Assert.That(_interfaceImplementationPropertyInformation.GetCustomAttribute<string>(false), Is.EqualTo("Test"));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new string[0];
      _implementationPropertyInformationStub.Setup(stub => stub.GetCustomAttributes<string>(false)).Returns(objToReturn);

      Assert.That(_interfaceImplementationPropertyInformation.GetCustomAttributes<string>(false), Is.SameAs(objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationPropertyInformationStub.Setup(stub => stub.IsDefined<Attribute>(false)).Returns(false);

      Assert.That(_interfaceImplementationPropertyInformation.IsDefined<Attribute>(false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var propertyInfoAdapter = PropertyInfoAdapter.Create(typeof (string).GetProperty("Length"));
      _implementationPropertyInformationStub.Setup(stub => stub.FindInterfaceImplementation(typeof (bool))).Returns(propertyInfoAdapter);

      Assert.That(_interfaceImplementationPropertyInformation.FindInterfaceImplementation(typeof (bool)), Is.SameAs(propertyInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That(_interfaceImplementationPropertyInformation.FindInterfaceDeclarations(), Is.EqualTo(new[] { _declarationPropertyInformationStub.Object }));
    }

    [Test]
    public void GetIndexParameters ()
    {
      var objToReturn = new ParameterInfo[0];
      _implementationPropertyInformationStub.Setup(stub => stub.GetIndexParameters()).Returns(objToReturn);

      Assert.That(_interfaceImplementationPropertyInformation.GetIndexParameters(), Is.SameAs(objToReturn));
    }

    [Test]
    public void GetAccessors ()
    {
      var objToReturn = new IMethodInformation[0];
      _implementationPropertyInformationStub.Setup(stub => stub.GetAccessors(false)).Returns(objToReturn);

      Assert.That(_interfaceImplementationPropertyInformation.GetAccessors(false), Is.SameAs(objToReturn));
    }

    [Test]
    public void PropertyType ()
    {
      _implementationPropertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof (bool));

      Assert.That(_interfaceImplementationPropertyInformation.PropertyType, Is.SameAs(typeof (bool)));
    }

    [Test]
    public void SetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();

      _declarationPropertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns(
          MethodInfoAdapter.Create(typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty("ImplicitInterfaceScalar").GetSetMethod(true)));
      _implementationPropertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns(
          MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty("ImplicitInterfaceScalar").GetSetMethod(true)));

      _interfaceImplementationPropertyInformation.SetValue(instance, value, null);
      Assert.That(instance.ImplicitInterfaceScalar, Is.SameAs(value));
    }

    [Test]
    public void SetValue_ImplementationAddsSetAccessor ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();

      _declarationPropertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns((IMethodInformation) null);
      _implementationPropertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns(
          MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty("ImplicitInterfaceScalar").GetSetMethod(true)));

      _interfaceImplementationPropertyInformation.SetValue(instance, value, null);
      Assert.That(instance.ImplicitInterfaceScalar, Is.SameAs(value));
    }

    [Test]
    public void SetValue_ImplementationAddsSetAccessor_IndexedProperty ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();

      _declarationPropertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns((IMethodInformation) null);
      _implementationPropertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns(
          MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty("Item", new[] { typeof (int) }).GetSetMethod(true)));

      _interfaceImplementationPropertyInformation.SetValue(instance, value, new object[] { 0 });
      Assert.That(instance[0], Is.SameAs(value));
    }

    [Test]
    public void GetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();
      instance.ImplicitInterfaceScalar = value;

      _declarationPropertyInformationStub.Setup(stub => stub.GetGetMethod(true)).Returns(
          MethodInfoAdapter.Create(typeof (IInterfaceWithReferenceType<SimpleReferenceType>).GetProperty("ImplicitInterfaceScalar").GetGetMethod(true)));
      _implementationPropertyInformationStub.Setup(stub => stub.GetGetMethod(true)).Returns(
          MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty("ImplicitInterfaceScalar").GetGetMethod(true)));

      Assert.That(_interfaceImplementationPropertyInformation.GetValue(instance, null), Is.SameAs(value));
    }

    [Test]
    public void GetValue_ImplementationsAddsGetAccessor ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();
      instance.ImplicitInterfaceScalar = value;

      _declarationPropertyInformationStub.Setup(stub => stub.GetGetMethod(true)).Returns((IMethodInformation) null);
      _implementationPropertyInformationStub.Setup(stub => stub.GetGetMethod(true)).Returns(
          MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty("ImplicitInterfaceScalar").GetGetMethod(true)));

      Assert.That(_interfaceImplementationPropertyInformation.GetValue(instance, null), Is.SameAs(value));
    }


    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("ImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("ImplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "get_ImplicitProperty",
          getMethodResult);

      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "set_ImplicitProperty",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty(
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.ExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("ExplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.get_ExplicitProperty",
          getMethodResult);

      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.set_ExplicitProperty",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ReadOnlyImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("ReadOnlyImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("ReadOnlyImplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "get_ReadOnlyImplicitProperty",
          getMethodResult);
      Assert.That(interfaceImplementationPropertyInformation.GetSetMethod(false), Is.Null);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_WriteOnlyImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("WriteOnlyImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("WriteOnlyImplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.GetGetMethod(false), Is.Null);
      var getMethodResult = interfaceImplementationPropertyInformation.GetSetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "set_WriteOnlyImplicitProperty",
          getMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ReadOnlyExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty(
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.ReadOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("ReadOnlyExplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.get_ReadOnlyExplicitProperty",
          getMethodResult);
      Assert.That(interfaceImplementationPropertyInformation.GetSetMethod(false), Is.Null);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_WriteOnlyExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty(
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.WriteOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("WriteOnlyExplicitProperty"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.GetGetMethod(false), Is.Null);
      var getMethodResult = interfaceImplementationPropertyInformation.GetSetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.set_WriteOnlyExplicitProperty",
          getMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingGetAccessor ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("PropertyAddingGetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("PropertyAddingGetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod(false);
      CheckMethodInformation(
          typeof (MethodInfoAdapter),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "get_PropertyAddingGetAccessor",
          getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "set_PropertyAddingGetAccessor",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitReadOnlyOnlyPropertyImplementationAddingSetAccessor ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("PropertyAddingSetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("PropertyAddingSetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "get_PropertyAddingSetAccessor",
          getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod(false);
      CheckMethodInformation(
          typeof (MethodInfoAdapter),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "set_PropertyAddingSetAccessor",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateGetAccessor_NonPublicFlagTrue ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("PropertyAddingPrivateGetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("PropertyAddingPrivateGetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod(true);
      CheckMethodInformation(
          typeof (MethodInfoAdapter),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "get_PropertyAddingPrivateGetAccessor",
          getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "set_PropertyAddingPrivateGetAccessor",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateGetAccessor_NonPublicFlagFalse ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("PropertyAddingPrivateGetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("PropertyAddingPrivateGetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.GetGetMethod(false), Is.Null);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "set_PropertyAddingPrivateGetAccessor",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateSetAccessor_NonPublicFlagTrue ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("PropertyAddingPrivateSetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("PropertyAddingPrivateSetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "get_PropertyAddingPrivateSetAccessor",
          getMethodResult);
      var setMethodResult = interfaceImplementationPropertyInformation.GetSetMethod(true);
      CheckMethodInformation(
          typeof (MethodInfoAdapter),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "set_PropertyAddingPrivateSetAccessor",
          setMethodResult);
    }

    [Test]
    public void GetGetMethod_GetSetMethod_ImplicitWriteOnlyPropertyImplementationAddingPrivateSetAccessor_NonPublicFlagFalse ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("PropertyAddingPrivateSetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("PropertyAddingPrivateSetAccessor"));

      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      var getMethodResult = interfaceImplementationPropertyInformation.GetGetMethod(false);
      CheckMethodInformation(
          typeof (InterfaceImplementationMethodInformation),
          TypeAdapter.Create(typeof (ClassImplementingInterface)),
          "get_PropertyAddingPrivateSetAccessor",
          getMethodResult);
      Assert.That(interfaceImplementationPropertyInformation.GetSetMethod(false), Is.Null);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("ImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("ImplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty(
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.ExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("ExplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitReadOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("ReadOnlyImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("ReadOnlyImplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitWriteOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("WriteOnlyImplicitProperty"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("WriteOnlyImplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitReadOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty(
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.ReadOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("ReadOnlyExplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void CanBeSetFromOutside_ExplicitWriteOnlyPropertyImplementation ()
    {
      var implementationPropertyInfo =
          PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty(
              "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceToImplement.WriteOnlyExplicitProperty",
              BindingFlags.NonPublic | BindingFlags.Instance));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("WriteOnlyExplicitProperty"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitPropertyImplementationAddingSetAccessor ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("PropertyAddingSetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("PropertyAddingSetAccessor"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_ImplicitPropertyImplementationAddingPrivateSetAccessor ()
    {
      var implementationPropertyInfo = PropertyInfoAdapter.Create(typeof (ClassImplementingInterface).GetProperty("PropertyAddingPrivateSetAccessor"));
      var declaringPropertyInfo = PropertyInfoAdapter.Create(typeof (IInterfaceToImplement).GetProperty("PropertyAddingPrivateSetAccessor"));
      var interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          implementationPropertyInfo, declaringPropertyInfo);

      Assert.That(interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void EqualsTest ()
    {
      Assert.That(_interfaceImplementationPropertyInformation.Equals(null), Is.False);
      Assert.That(_interfaceImplementationPropertyInformation.Equals("Test"), Is.False);
      Assert.That(
          _interfaceImplementationPropertyInformation.Equals(
              new InterfaceImplementationPropertyInformation(
                  PropertyInfoAdapter.Create(typeof (string).GetProperty("Length")),
                  PropertyInfoAdapter.Create(typeof (string).GetProperty("Length")))),
          Is.False);

      Assert.That(
          _interfaceImplementationPropertyInformation.Equals(
              new InterfaceImplementationPropertyInformation(_implementationPropertyInformationStub.Object, _declarationPropertyInformationStub.Object)),
          Is.True);
    }

    [Test]
    public void GetHashCodeTest ()
    {
      var equalObject = new InterfaceImplementationPropertyInformation(_implementationPropertyInformationStub.Object, _declarationPropertyInformationStub.Object);
      Assert.That(_interfaceImplementationPropertyInformation.GetHashCode(), Is.EqualTo(equalObject.GetHashCode()));
    }

    [Test]
    public void To_String ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub.Setup(stub => stub.Name).Returns("Boolean");
      _implementationPropertyInformationStub.Setup(stub => stub.Name).Returns("Test");
      _declarationPropertyInformationStub.Setup(stub => stub.DeclaringType).Returns(typeInformationStub.Object);

      Assert.That(_interfaceImplementationPropertyInformation.ToString(), Is.EqualTo("Test (impl of 'Boolean')"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(((IPropertyInformation) _interfaceImplementationPropertyInformation).IsNull, Is.False);
    }

    private void CheckMethodInformation (
        Type expectedType, ITypeInformation expectedPropertyDeclaringType, string expectedPropertyName, IMethodInformation actualMethodInformation)
    {
      Assert.That(actualMethodInformation, Is.TypeOf(expectedType));
      Assert.That(actualMethodInformation.DeclaringType, Is.SameAs(expectedPropertyDeclaringType));
      Assert.That(actualMethodInformation.Name, Is.EqualTo(expectedPropertyName));
    }
  }
}