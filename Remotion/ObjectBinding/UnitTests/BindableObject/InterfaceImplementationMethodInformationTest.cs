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
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class InterfaceImplementationMethodInformationTest
  {
    private IMethodInformation _implementationMethodInformationStub;
    private IMethodInformation _declarationMethodInformationStub;
    private InterfaceImplementationMethodInformation _interfaceImplementationMethodInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationMethodInformationStub = MockRepository.GenerateStub<IMethodInformation>();
      _declarationMethodInformationStub = MockRepository.GenerateStub<IMethodInformation>();

      _interfaceImplementationMethodInformation = new InterfaceImplementationMethodInformation (
          _implementationMethodInformationStub,
          _declarationMethodInformationStub);
    }

    [Test]
    public void Name ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.Name).Return ("Test");

      Assert.That (_interfaceImplementationMethodInformation.Name, Is.EqualTo ("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      _implementationMethodInformationStub.Stub (stub => stub.DeclaringType).Return (typeInformationStub);

      Assert.That (_interfaceImplementationMethodInformation.DeclaringType, Is.SameAs (typeInformationStub));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation> ();
      _implementationMethodInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (typeInformationStub);

      Assert.That (_interfaceImplementationMethodInformation.GetOriginalDeclaringType(), Is.SameAs (typeInformationStub));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.GetCustomAttribute<string> (false)).Return ("Test");

      Assert.That (_interfaceImplementationMethodInformation.GetCustomAttribute<string> (false), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new string[0];
      _implementationMethodInformationStub.Stub (stub => stub.GetCustomAttributes<string> (false)).Return (objToReturn);

      Assert.That (_interfaceImplementationMethodInformation.GetCustomAttributes<string> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.IsDefined<Attribute> (false)).Return (false);

      Assert.That (_interfaceImplementationMethodInformation.IsDefined<Attribute> (false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var methodInfoAdapter = MethodInfoAdapter.Create(typeof (object).GetMethod ("ToString"));
      _implementationMethodInformationStub.Stub (stub => stub.FindInterfaceImplementation (typeof (bool))).Return (methodInfoAdapter);

      Assert.That (_interfaceImplementationMethodInformation.FindInterfaceImplementation (typeof (bool)), Is.SameAs (methodInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That (_interfaceImplementationMethodInformation.FindInterfaceDeclarations(), Is.EqualTo (new[] { _declarationMethodInformationStub }));
    }

    [Test]
    public void GetFastInvoker ()
    {
      var objToReturn = (Func<string>) (() => "Test");
      _declarationMethodInformationStub.Stub (stub => stub.GetFastInvoker (typeof (Func<string>))).Return (objToReturn);

      var invoker = _interfaceImplementationMethodInformation.GetFastInvoker<Func<string>>();

      Assert.That (invoker, Is.SameAs (objToReturn));
    }

    [Test]
    public void GetParameters ()
    {
      var objToReturn = new ParameterInfo[0];
      _implementationMethodInformationStub.Stub (stub => stub.GetParameters()).Return (objToReturn);

      Assert.That (_interfaceImplementationMethodInformation.GetParameters(), Is.SameAs (objToReturn));
    }

    [Test]
    public void GetOriginalDeclaration ()
    {
      var objToReturn = MethodInfoAdapter.Create(typeof (string).GetMethod ("get_Length"));
      _implementationMethodInformationStub.Stub (stub => stub.GetOriginalDeclaration ()).Return (objToReturn);

      Assert.That (_interfaceImplementationMethodInformation.GetOriginalDeclaration (), Is.SameAs (objToReturn));
    }

    [Test]
    public void FindDeclaringProperty ()
    {
      var objToReturn = PropertyInfoAdapter.Create(typeof (string).GetProperty ("Length"));
      _implementationMethodInformationStub.Stub (stub => stub.FindDeclaringProperty()).Return (objToReturn);

      Assert.That (_interfaceImplementationMethodInformation.FindDeclaringProperty(), Is.SameAs (objToReturn));
    }

    [Test]
    public void ReturnsType ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.ReturnType).Return (typeof (bool));

      Assert.That (_interfaceImplementationMethodInformation.ReturnType, Is.SameAs (typeof (bool)));
    }

    [Test]
    public void Invoke ()
    {
      var instance = new object();
      var parameters = new object[0];
      _declarationMethodInformationStub.Stub (stub => stub.Invoke (instance, parameters)).Return ("Test");

      var result = _interfaceImplementationMethodInformation.Invoke (instance, parameters);

      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void Equals ()
    {
      Assert.That (_interfaceImplementationMethodInformation.Equals (null), Is.False);
      Assert.That (_interfaceImplementationMethodInformation.Equals ("test"), Is.False);

      Assert.That (
          _interfaceImplementationMethodInformation.Equals (
              new InterfaceImplementationMethodInformation (_declarationMethodInformationStub, _declarationMethodInformationStub)),
          Is.False);
      Assert.That (
          _interfaceImplementationMethodInformation.Equals (
              new InterfaceImplementationMethodInformation (_implementationMethodInformationStub, _implementationMethodInformationStub)),
          Is.False);
      Assert.That (
          _interfaceImplementationMethodInformation.Equals (
              new InterfaceImplementationMethodInformation (_declarationMethodInformationStub, _implementationMethodInformationStub)),
          Is.False);

      Assert.That (
          _interfaceImplementationMethodInformation.Equals (
              new InterfaceImplementationMethodInformation (_implementationMethodInformationStub, _declarationMethodInformationStub)),
          Is.True);
    }

    [Test]
    public void GetHashcode ()
    {
      var equalObject = new InterfaceImplementationMethodInformation (_implementationMethodInformationStub, _declarationMethodInformationStub);
      Assert.That (_interfaceImplementationMethodInformation.GetHashCode(), Is.EqualTo (equalObject.GetHashCode()));
    }

    [Test]
    public void To_String ()
    {
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation> ();
      typeInformationStub.Stub (stub => stub.Name).Return ("Boolean");

      _implementationMethodInformationStub.Stub (stub => stub.Name).Return ("Test");
      _declarationMethodInformationStub.Stub (stub => stub.DeclaringType).Return (typeInformationStub);

      Assert.That (_interfaceImplementationMethodInformation.ToString(), Is.EqualTo ("Test (impl of 'Boolean')"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((IMethodInformation) _interfaceImplementationMethodInformation).IsNull, Is.False);
    }
  }
}