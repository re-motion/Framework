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
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class InterfaceImplementationMethodInformationTest
  {
    private Mock<IMethodInformation> _implementationMethodInformationStub;
    private Mock<IMethodInformation> _declarationMethodInformationStub;
    private InterfaceImplementationMethodInformation _interfaceImplementationMethodInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationMethodInformationStub = new Mock<IMethodInformation>();
      _declarationMethodInformationStub = new Mock<IMethodInformation>();

      _interfaceImplementationMethodInformation = new InterfaceImplementationMethodInformation(
          _implementationMethodInformationStub.Object,
          _declarationMethodInformationStub.Object);
    }

    [Test]
    public void Name ()
    {
      _implementationMethodInformationStub.Setup(stub => stub.Name).Returns("Test");

      Assert.That(_interfaceImplementationMethodInformation.Name, Is.EqualTo("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      _implementationMethodInformationStub.Setup(stub => stub.DeclaringType).Returns(typeInformationStub.Object);

      Assert.That(_interfaceImplementationMethodInformation.DeclaringType, Is.SameAs(typeInformationStub.Object));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      _implementationMethodInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(typeInformationStub.Object);

      Assert.That(_interfaceImplementationMethodInformation.GetOriginalDeclaringType(), Is.SameAs(typeInformationStub.Object));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      _implementationMethodInformationStub.Setup(stub => stub.GetCustomAttribute<string>(false)).Returns("Test");

      Assert.That(_interfaceImplementationMethodInformation.GetCustomAttribute<string>(false), Is.EqualTo("Test"));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new string[0];
      _implementationMethodInformationStub.Setup(stub => stub.GetCustomAttributes<string>(false)).Returns(objToReturn);

      Assert.That(_interfaceImplementationMethodInformation.GetCustomAttributes<string>(false), Is.SameAs(objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationMethodInformationStub.Setup(stub => stub.IsDefined<Attribute>(false)).Returns(false);

      Assert.That(_interfaceImplementationMethodInformation.IsDefined<Attribute>(false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var methodInfoAdapter = MethodInfoAdapter.Create(typeof (object).GetMethod("ToString"));
      _implementationMethodInformationStub.Setup(stub => stub.FindInterfaceImplementation(typeof (bool))).Returns(methodInfoAdapter);

      Assert.That(_interfaceImplementationMethodInformation.FindInterfaceImplementation(typeof (bool)), Is.SameAs(methodInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That(_interfaceImplementationMethodInformation.FindInterfaceDeclarations(), Is.EqualTo(new[] { _declarationMethodInformationStub.Object }));
    }

    [Test]
    public void GetFastInvoker ()
    {
      var objToReturn = (Func<string>) (() => "Test");
      _declarationMethodInformationStub.Setup(stub => stub.GetFastInvoker(typeof (Func<string>))).Returns(objToReturn);

      var invoker = _interfaceImplementationMethodInformation.GetFastInvoker<Func<string>>();

      Assert.That(invoker, Is.SameAs(objToReturn));
    }

    [Test]
    public void GetParameters ()
    {
      var objToReturn = new ParameterInfo[0];
      _implementationMethodInformationStub.Setup(stub => stub.GetParameters()).Returns(objToReturn);

      Assert.That(_interfaceImplementationMethodInformation.GetParameters(), Is.SameAs(objToReturn));
    }

    [Test]
    public void GetOriginalDeclaration ()
    {
      var objToReturn = MethodInfoAdapter.Create(typeof (string).GetMethod("get_Length"));
      _implementationMethodInformationStub.Setup(stub => stub.GetOriginalDeclaration()).Returns(objToReturn);

      Assert.That(_interfaceImplementationMethodInformation.GetOriginalDeclaration(), Is.SameAs(objToReturn));
    }

    [Test]
    public void FindDeclaringProperty ()
    {
      var objToReturn = PropertyInfoAdapter.Create(typeof (string).GetProperty("Length"));
      _implementationMethodInformationStub.Setup(stub => stub.FindDeclaringProperty()).Returns(objToReturn);

      Assert.That(_interfaceImplementationMethodInformation.FindDeclaringProperty(), Is.SameAs(objToReturn));
    }

    [Test]
    public void ReturnsType ()
    {
      _implementationMethodInformationStub.Setup(stub => stub.ReturnType).Returns(typeof (bool));

      Assert.That(_interfaceImplementationMethodInformation.ReturnType, Is.SameAs(typeof (bool)));
    }

    [Test]
    public void Invoke ()
    {
      var instance = new object();
      var parameters = new object[0];
      _declarationMethodInformationStub.Setup(stub => stub.Invoke(instance, parameters)).Returns("Test");

      var result = _interfaceImplementationMethodInformation.Invoke(instance, parameters);

      Assert.That(result, Is.EqualTo("Test"));
    }

    [Test]
    public void Equals ()
    {
      Assert.That(_interfaceImplementationMethodInformation.Equals(null), Is.False);
      Assert.That(_interfaceImplementationMethodInformation.Equals("test"), Is.False);

      Assert.That(
          _interfaceImplementationMethodInformation.Equals(
              new InterfaceImplementationMethodInformation(_declarationMethodInformationStub.Object, _declarationMethodInformationStub.Object)),
          Is.False);
      Assert.That(
          _interfaceImplementationMethodInformation.Equals(
              new InterfaceImplementationMethodInformation(_implementationMethodInformationStub.Object, _implementationMethodInformationStub.Object)),
          Is.False);
      Assert.That(
          _interfaceImplementationMethodInformation.Equals(
              new InterfaceImplementationMethodInformation(_declarationMethodInformationStub.Object, _implementationMethodInformationStub.Object)),
          Is.False);

      Assert.That(
          _interfaceImplementationMethodInformation.Equals(
              new InterfaceImplementationMethodInformation(_implementationMethodInformationStub.Object, _declarationMethodInformationStub.Object)),
          Is.True);
    }

    [Test]
    public void GetHashcode ()
    {
      var equalObject = new InterfaceImplementationMethodInformation(_implementationMethodInformationStub.Object, _declarationMethodInformationStub.Object);
      Assert.That(_interfaceImplementationMethodInformation.GetHashCode(), Is.EqualTo(equalObject.GetHashCode()));
    }

    [Test]
    public void To_String ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub.Setup(stub => stub.Name).Returns("Boolean");

      _implementationMethodInformationStub.Setup(stub => stub.Name).Returns("Test");
      _declarationMethodInformationStub.Setup(stub => stub.DeclaringType).Returns(typeInformationStub.Object);

      Assert.That(_interfaceImplementationMethodInformation.ToString(), Is.EqualTo("Test (impl of 'Boolean')"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(((IMethodInformation) _interfaceImplementationMethodInformation).IsNull, Is.False);
    }
  }
}