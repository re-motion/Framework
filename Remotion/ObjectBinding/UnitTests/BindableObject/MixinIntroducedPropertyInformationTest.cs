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
  public class MixinIntroducedPropertyInformationTest
  {
    private Mock<IPropertyInformation> _implementationPropertyInformationStub;
    private MixinIntroducedPropertyInformation _mixinIntroducedPropertyInformation;
    private Mock<IPropertyInformation> _declarationPropertyInformationStub;
    private InterfaceImplementationPropertyInformation _interfaceImplementationPropertyInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationPropertyInformationStub = new Mock<IPropertyInformation>();
      _declarationPropertyInformationStub = new Mock<IPropertyInformation>();
      _interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation(
          _implementationPropertyInformationStub.Object, _declarationPropertyInformationStub.Object);
      _mixinIntroducedPropertyInformation = new MixinIntroducedPropertyInformation(_interfaceImplementationPropertyInformation);
    }

    [Test]
    public void Name ()
    {
      _implementationPropertyInformationStub.Setup(stub => stub.Name).Returns("Test");

      Assert.That(_mixinIntroducedPropertyInformation.Name, Is.EqualTo("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      _implementationPropertyInformationStub.Setup(stub => stub.DeclaringType).Returns(typeInformationStub.Object);

      Assert.That(_mixinIntroducedPropertyInformation.DeclaringType, Is.SameAs(typeInformationStub.Object));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      _implementationPropertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(typeInformationStub.Object);

      Assert.That(_mixinIntroducedPropertyInformation.GetOriginalDeclaringType(), Is.SameAs(typeInformationStub.Object));
    }

    [Test]
    public void GetOriginalDeclaration ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      _implementationPropertyInformationStub.Setup(stub => stub.GetOriginalDeclaration()).Returns(propertyInformationStub.Object);

      Assert.That(_mixinIntroducedPropertyInformation.GetOriginalDeclaration(), Is.SameAs(propertyInformationStub.Object));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      var objToReturn = new object();
      _implementationPropertyInformationStub.Setup(stub => stub.GetCustomAttribute<object>(false)).Returns(objToReturn);

      Assert.That(_mixinIntroducedPropertyInformation.GetCustomAttribute<object>(false), Is.SameAs(objToReturn));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new object[0];
      _implementationPropertyInformationStub.Setup(stub => stub.GetCustomAttributes<object>(false)).Returns(objToReturn);

      Assert.That(_mixinIntroducedPropertyInformation.GetCustomAttributes<object>(false), Is.SameAs(objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationPropertyInformationStub.Setup(stub => stub.IsDefined<object>(false)).Returns(false);

      Assert.That(_mixinIntroducedPropertyInformation.IsDefined<object>(false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var propertyInfoAdapter = PropertyInfoAdapter.Create(typeof(string).GetProperty("Length"));
      _implementationPropertyInformationStub.Setup(stub => stub.FindInterfaceImplementation(typeof(object))).Returns(propertyInfoAdapter);

      Assert.That(_mixinIntroducedPropertyInformation.FindInterfaceImplementation(typeof(object)), Is.SameAs(propertyInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That(_mixinIntroducedPropertyInformation.FindInterfaceDeclarations(), Is.EqualTo(new[] { _declarationPropertyInformationStub.Object }));
    }

    [Test]
    public void PropertyType ()
    {
      _implementationPropertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(object));

      Assert.That(_mixinIntroducedPropertyInformation.PropertyType, Is.SameAs(typeof(object)));
    }

    [Test]
    public void CanBeSetFromOutside_IsBasedOnGetSetMethod_TrueForInterfaceImplementationPropertySetter ()
    {
      var methodInfoAdapter = MethodInfoAdapter.Create(typeof(object).GetMethod("ToString"));
      _implementationPropertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns(methodInfoAdapter);
      _declarationPropertyInformationStub.Setup(stub => stub.GetSetMethod(false)).Returns(methodInfoAdapter);

      Assert.That(_mixinIntroducedPropertyInformation.CanBeSetFromOutside, Is.True);
    }

    [Test]
    public void CanBeSetFromOutside_IsBasedOnGetSetMethod_FalseForImplementationOnlyPropertySetter ()
    {
      var methodInfoAdapter = MethodInfoAdapter.Create(typeof(object).GetMethod("ToString"));
      _implementationPropertyInformationStub.Setup(stub => stub.GetSetMethod(false)).Returns(methodInfoAdapter);

      Assert.That(_mixinIntroducedPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void CanBeSetFromOutside_IsBasedOnGetSetMethod_FalseForNoPropertySetter ()
    {
      _implementationPropertyInformationStub.Setup(stub => stub.GetSetMethod(false)).Returns((IMethodInformation) null);

      Assert.That(_mixinIntroducedPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void GetGetMethod ()
    {
      var methodInfoAdapter = MethodInfoAdapter.Create(typeof(object).GetMethod("ToString"));
      _implementationPropertyInformationStub.Setup(stub => stub.GetGetMethod(true)).Returns(methodInfoAdapter);
      _declarationPropertyInformationStub.Setup(stub => stub.GetGetMethod(false)).Returns(methodInfoAdapter);

      var result = _mixinIntroducedPropertyInformation.GetGetMethod(false);

      Assert.That(result, Is.TypeOf(typeof(MixinIntroducedMethodInformation)));
      Assert.That(result.Name, Is.EqualTo("ToString"));
    }

    [Test]
    public void GetGetMethod_ReturnsNull ()
    {
      _implementationPropertyInformationStub.Setup(stub => stub.GetGetMethod(false)).Returns((IMethodInformation) null);

      Assert.That(_mixinIntroducedPropertyInformation.GetGetMethod(false), Is.Null);
    }

    [Test]
    public void GetSetMethod ()
    {
      var methodInfoAdapter = MethodInfoAdapter.Create(typeof(object).GetMethod("ToString"));
      _implementationPropertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns(methodInfoAdapter);
      _declarationPropertyInformationStub.Setup(stub => stub.GetSetMethod(false)).Returns(methodInfoAdapter);

      var result = _mixinIntroducedPropertyInformation.GetSetMethod(false);

      Assert.That(result, Is.TypeOf(typeof(MixinIntroducedMethodInformation)));
      Assert.That(result.Name, Is.EqualTo("ToString"));
    }

    [Test]
    public void GetSetMethod_ReturnsNull ()
    {
      _implementationPropertyInformationStub.Setup(stub => stub.GetSetMethod(false)).Returns((IMethodInformation) null);

      Assert.That(_mixinIntroducedPropertyInformation.GetSetMethod(false), Is.Null);
    }

    [Test]
    public void SetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();

      _declarationPropertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns((IMethodInformation) null);
      _implementationPropertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns(
          MethodInfoAdapter.Create(typeof(ClassWithReferenceType<SimpleReferenceType>).GetProperty("ImplicitInterfaceScalar").GetSetMethod(true)));

      _mixinIntroducedPropertyInformation.SetValue(instance, value, null);

      Assert.That(instance.ImplicitInterfaceScalar, Is.SameAs(value));
    }

    [Test]
    public void GetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType>();
      var value = new SimpleReferenceType();
      instance.ImplicitInterfaceScalar = value;

      _declarationPropertyInformationStub.Setup(stub => stub.GetGetMethod(true)).Returns((IMethodInformation) null);
      _implementationPropertyInformationStub.Setup(stub => stub.GetGetMethod(true)).Returns(
          MethodInfoAdapter.Create(typeof(ClassWithReferenceType<SimpleReferenceType>).GetProperty("ImplicitInterfaceScalar").GetGetMethod(true)));

      Assert.That(_mixinIntroducedPropertyInformation.GetValue(instance, null), Is.SameAs(value));
    }

    [Test]
    public void GetIndexParameters ()
    {
      var objToReturn = new ParameterInfo[0];
      _implementationPropertyInformationStub.Setup(stub => stub.GetIndexParameters()).Returns(objToReturn);

      Assert.That(_mixinIntroducedPropertyInformation.GetIndexParameters(), Is.SameAs(objToReturn));
    }

    [Test]
    public void GetAccessors ()
    {
      var objToReturn = new IMethodInformation[0];
      _implementationPropertyInformationStub.Setup(stub => stub.GetAccessors(false)).Returns(objToReturn);

      Assert.That(_mixinIntroducedPropertyInformation.GetAccessors(false), Is.SameAs(objToReturn));
    }

    [Test]
    public void Equals_ChecksPropertyInfo ()
    {
      Assert.That(_mixinIntroducedPropertyInformation.Equals(null), Is.False);
      Assert.That(_mixinIntroducedPropertyInformation.Equals("Test"), Is.False);
      Assert.That(
          _mixinIntroducedPropertyInformation.Equals(
              new MixinIntroducedPropertyInformation(
                  new InterfaceImplementationPropertyInformation(
                      PropertyInfoAdapter.Create(typeof(string).GetProperty("Length")),
                      PropertyInfoAdapter.Create(typeof(string).GetProperty("Length"))))),
          Is.False);

      Assert.That(
        _mixinIntroducedPropertyInformation.Equals(new MixinIntroducedPropertyInformation(_interfaceImplementationPropertyInformation)), Is.True);
    }

    [Test]
    public void GetHashCode_UsesPropertyInfo ()
    {
      Assert.That(
          _mixinIntroducedPropertyInformation.GetHashCode(),
          Is.EqualTo(new MixinIntroducedPropertyInformation(_interfaceImplementationPropertyInformation).GetHashCode()));

    }

    [Test]
    public void To_String ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub.Setup(stub => stub.Name).Returns("Boolean");
      _implementationPropertyInformationStub.Setup(stub => stub.Name).Returns("Test");
      _declarationPropertyInformationStub.Setup(stub => stub.DeclaringType).Returns(typeInformationStub.Object);
      Assert.That(_mixinIntroducedPropertyInformation.ToString(), Is.EqualTo("Test (impl of 'Boolean') (Mixin)"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(((IPropertyInformation) _mixinIntroducedPropertyInformation).IsNull, Is.False);
    }
  }
}
