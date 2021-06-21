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
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class MixinIntroducedMethodInformationTest
  {
    private Mock<IMethodInformation> _implementationMethodInformationStub;
    private Mock<IMethodInformation> _declarationMethodInformationStub;
    private MixinIntroducedMethodInformation _mixinIntroducedMethodInformation;
    private InterfaceImplementationMethodInformation _interfaceImplementationMethodInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationMethodInformationStub = new Mock<IMethodInformation>();
      _declarationMethodInformationStub = new Mock<IMethodInformation>();
      _interfaceImplementationMethodInformation = new InterfaceImplementationMethodInformation (
          _implementationMethodInformationStub.Object, _declarationMethodInformationStub.Object);
      _mixinIntroducedMethodInformation = new MixinIntroducedMethodInformation (_interfaceImplementationMethodInformation);
    }

    [Test]
    public void Name ()
    {
      _implementationMethodInformationStub.Setup (stub => stub.Name).Returns ("Test");

      Assert.That (_mixinIntroducedMethodInformation.Name, Is.EqualTo ("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      _implementationMethodInformationStub.Setup (stub => stub.DeclaringType).Returns (typeInformationStub.Object);

      Assert.That (_mixinIntroducedMethodInformation.DeclaringType, Is.SameAs (typeInformationStub.Object));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      _implementationMethodInformationStub.Setup (stub => stub.GetOriginalDeclaringType()).Returns (typeInformationStub.Object);

      Assert.That (_mixinIntroducedMethodInformation.GetOriginalDeclaringType(), Is.SameAs (typeInformationStub.Object));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      var objToReturn = new object();
      _implementationMethodInformationStub.Setup (stub => stub.GetCustomAttribute<object> (false)).Returns (objToReturn);

      Assert.That (_mixinIntroducedMethodInformation.GetCustomAttribute<object> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new object[0];
      _implementationMethodInformationStub.Setup (stub => stub.GetCustomAttributes<object> (false)).Returns (objToReturn);

      Assert.That (_mixinIntroducedMethodInformation.GetCustomAttributes<object> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationMethodInformationStub.Setup (stub => stub.IsDefined<object> (false)).Returns (false);

      Assert.That (_mixinIntroducedMethodInformation.IsDefined<object> (false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var methodInfoAdapter = MethodInfoAdapter.Create(typeof (object).GetMethod ("ToString"));
      _implementationMethodInformationStub.Setup (stub => stub.FindInterfaceImplementation (typeof (object))).Returns (methodInfoAdapter);

      Assert.That (_mixinIntroducedMethodInformation.FindInterfaceImplementation (typeof (object)), Is.SameAs (methodInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      IMethodInformation methodInformation = MethodInfoAdapter.Create(typeof (object).GetMethod ("ToString"));
      _declarationMethodInformationStub
          .Setup (stub => stub.FindInterfaceDeclarations())
          .Returns (EnumerableUtility.Singleton (methodInformation).AsOneTime());

      Assert.That (_mixinIntroducedMethodInformation.FindInterfaceDeclarations (), Is.EqualTo (new[] { _declarationMethodInformationStub.Object }));
      Assert.That (_mixinIntroducedMethodInformation.FindInterfaceDeclarations (), Is.EqualTo (new[] { _declarationMethodInformationStub.Object }));
    }

    [Test]
    public void FindDeclaringProperty ()
    {
      var propertyInfoAdapter = PropertyInfoAdapter.Create(typeof (string).GetProperty ("Length"));
      _implementationMethodInformationStub.Setup (stub => stub.FindDeclaringProperty()).Returns (propertyInfoAdapter);

      Assert.That (_mixinIntroducedMethodInformation.FindDeclaringProperty(), Is.SameAs (propertyInfoAdapter));
    }

    [Test]
    public void ReturnType ()
    {
      _implementationMethodInformationStub.Setup (stub => stub.ReturnType).Returns (typeof (object));

      Assert.That (_mixinIntroducedMethodInformation.ReturnType, Is.SameAs (typeof (object)));
    }

    [Test]
    public void Invoke ()
    {
      var methodInfoAdapter = MethodInfoAdapter.Create(typeof (object).GetMethod ("ToString"));
      _declarationMethodInformationStub.Setup (stub => stub.Invoke ("Test", new object[] { })).Returns (methodInfoAdapter);

      var result = _mixinIntroducedMethodInformation.Invoke ("Test", new object[] { });

      Assert.That (result, Is.SameAs (methodInfoAdapter));
    }

    [Test]
    public void GetFastInvoker ()
    {
      var fakeResult = new object();

      _declarationMethodInformationStub.Setup (stub => stub.GetFastInvoker (typeof (Func<object>))).Returns ((Func<object>) (() => fakeResult));

      var invoker = _mixinIntroducedMethodInformation.GetFastInvoker<Func<object>>();

      Assert.That (invoker(), Is.SameAs (fakeResult));
    }

    [Test]
    public void GetParameters ()
    {
      var objToReturn = new ParameterInfo[0];
      _implementationMethodInformationStub.Setup (stub => stub.GetParameters()).Returns (objToReturn);

      Assert.That (_mixinIntroducedMethodInformation.GetParameters(), Is.SameAs (objToReturn));
    }

    [Test]
    public void GetOriginalDeclaration ()
    {
      var objToReturn = MethodInfoAdapter.Create(typeof (string).GetMethod ("get_Length"));
      _implementationMethodInformationStub.Setup (stub => stub.GetOriginalDeclaration ()).Returns (objToReturn);

      Assert.That (_mixinIntroducedMethodInformation.GetOriginalDeclaration (), Is.SameAs (objToReturn));
    }

    [Test]
    public void Equals ()
    {
      Assert.That (_mixinIntroducedMethodInformation.Equals (null), Is.False);
      Assert.That (_mixinIntroducedMethodInformation.Equals ("test"), Is.False);
      Assert.That (
          _mixinIntroducedMethodInformation.Equals (new MixinIntroducedMethodInformation (_interfaceImplementationMethodInformation)), Is.True);
      Assert.That (
          _mixinIntroducedMethodInformation.Equals (
              new MixinIntroducedMethodInformation (
                  new InterfaceImplementationMethodInformation (_declarationMethodInformationStub.Object, _implementationMethodInformationStub.Object))),
          Is.False);

      Assert.That (
          _mixinIntroducedMethodInformation.Equals (
              new MixinIntroducedMethodInformation (
                  new InterfaceImplementationMethodInformation (_implementationMethodInformationStub.Object, _declarationMethodInformationStub.Object))),
          Is.True);}

    [Test]
    public void GetHashcode ()
    {
      Assert.That (
          _mixinIntroducedMethodInformation.GetHashCode(),
          Is.EqualTo (new MixinIntroducedMethodInformation (_interfaceImplementationMethodInformation).GetHashCode()));
    }

    [Test]
    public void To_String ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub.Setup (stub => stub.Name).Returns ("Boolean");
      _implementationMethodInformationStub.Setup (stub => stub.Name).Returns ("Test");
      _declarationMethodInformationStub.Setup (stub => stub.DeclaringType).Returns (typeInformationStub.Object);

      Assert.That (_mixinIntroducedMethodInformation.ToString(), Is.EqualTo ("Test (impl of 'Boolean') (Mixin)"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((IMethodInformation) _mixinIntroducedMethodInformation).IsNull, Is.False);
    }
  }
}