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
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class MixinIntroducedMethodInformationTest
  {
    private IMethodInformation _implementationMethodInformationStub;
    private IMethodInformation _declarationMethodInformationStub;
    private MixinIntroducedMethodInformation _mixinIntroducedMethodInformation;
    private InterfaceImplementationMethodInformation _interfaceImplementationMethodInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationMethodInformationStub = MockRepository.GenerateStub<IMethodInformation>();
      _declarationMethodInformationStub = MockRepository.GenerateStub<IMethodInformation>();
      _interfaceImplementationMethodInformation = new InterfaceImplementationMethodInformation (
          _implementationMethodInformationStub, _declarationMethodInformationStub);
      _mixinIntroducedMethodInformation = new MixinIntroducedMethodInformation (_interfaceImplementationMethodInformation);
    }

    [Test]
    public void Name ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.Name).Return ("Test");

      Assert.That (_mixinIntroducedMethodInformation.Name, Is.EqualTo ("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation> ();
      _implementationMethodInformationStub.Stub (stub => stub.DeclaringType).Return (typeInformationStub);

      Assert.That (_mixinIntroducedMethodInformation.DeclaringType, Is.SameAs (typeInformationStub));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation> ();
      _implementationMethodInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (typeInformationStub);

      Assert.That (_mixinIntroducedMethodInformation.GetOriginalDeclaringType(), Is.SameAs (typeInformationStub));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      var objToReturn = new object();
      _implementationMethodInformationStub.Stub (stub => stub.GetCustomAttribute<object> (false)).Return (objToReturn);

      Assert.That (_mixinIntroducedMethodInformation.GetCustomAttribute<object> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new object[0];
      _implementationMethodInformationStub.Stub (stub => stub.GetCustomAttributes<object> (false)).Return (objToReturn);

      Assert.That (_mixinIntroducedMethodInformation.GetCustomAttributes<object> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.IsDefined<object> (false)).Return (false);

      Assert.That (_mixinIntroducedMethodInformation.IsDefined<object> (false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var methodInfoAdapter = MethodInfoAdapter.Create(typeof (object).GetMethod ("ToString"));
      _implementationMethodInformationStub.Stub (stub => stub.FindInterfaceImplementation (typeof (object))).Return (methodInfoAdapter);

      Assert.That (_mixinIntroducedMethodInformation.FindInterfaceImplementation (typeof (object)), Is.SameAs (methodInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      IMethodInformation methodInformation = MethodInfoAdapter.Create(typeof (object).GetMethod ("ToString"));
      _declarationMethodInformationStub
          .Stub (stub => stub.FindInterfaceDeclarations())
          .Return (EnumerableUtility.Singleton (methodInformation).AsOneTime());

      Assert.That (_mixinIntroducedMethodInformation.FindInterfaceDeclarations (), Is.EqualTo (new[] { _declarationMethodInformationStub }));
      Assert.That (_mixinIntroducedMethodInformation.FindInterfaceDeclarations (), Is.EqualTo (new[] { _declarationMethodInformationStub }));
    }

    [Test]
    public void FindDeclaringProperty ()
    {
      var propertyInfoAdapter = PropertyInfoAdapter.Create(typeof (string).GetProperty ("Length"));
      _implementationMethodInformationStub.Stub (stub => stub.FindDeclaringProperty()).Return (propertyInfoAdapter);

      Assert.That (_mixinIntroducedMethodInformation.FindDeclaringProperty(), Is.SameAs (propertyInfoAdapter));
    }

    [Test]
    public void ReturnType ()
    {
      _implementationMethodInformationStub.Stub (stub => stub.ReturnType).Return (typeof (object));

      Assert.That (_mixinIntroducedMethodInformation.ReturnType, Is.SameAs (typeof (object)));
    }

    [Test]
    public void Invoke ()
    {
      var methodInfoAdapter = MethodInfoAdapter.Create(typeof (object).GetMethod ("ToString"));
      _declarationMethodInformationStub.Stub (stub => stub.Invoke ("Test", new object[] { })).Return (methodInfoAdapter);

      var result = _mixinIntroducedMethodInformation.Invoke ("Test", new object[] { });

      Assert.That (result, Is.SameAs (methodInfoAdapter));
    }

    [Test]
    public void GetFastInvoker ()
    {
      var fakeResult = new object();

      _declarationMethodInformationStub.Stub (stub => stub.GetFastInvoker (typeof (Func<object>))).Return ((Func<object>) (() => fakeResult));

      var invoker = _mixinIntroducedMethodInformation.GetFastInvoker<Func<object>>();

      Assert.That (invoker(), Is.SameAs (fakeResult));
    }

    [Test]
    public void GetParameters ()
    {
      var objToReturn = new ParameterInfo[0];
      _implementationMethodInformationStub.Stub (stub => stub.GetParameters()).Return (objToReturn);

      Assert.That (_mixinIntroducedMethodInformation.GetParameters(), Is.SameAs (objToReturn));
    }

    [Test]
    public void GetOriginalDeclaration ()
    {
      var objToReturn = MethodInfoAdapter.Create(typeof (string).GetMethod ("get_Length"));
      _implementationMethodInformationStub.Stub (stub => stub.GetOriginalDeclaration ()).Return (objToReturn);

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
                  new InterfaceImplementationMethodInformation (_declarationMethodInformationStub, _implementationMethodInformationStub))),
          Is.False);

      Assert.That (
          _mixinIntroducedMethodInformation.Equals (
              new MixinIntroducedMethodInformation (
                  new InterfaceImplementationMethodInformation (_implementationMethodInformationStub, _declarationMethodInformationStub))),
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
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation> ();
      typeInformationStub.Stub (stub => stub.Name).Return ("Boolean");
      _implementationMethodInformationStub.Stub (stub => stub.Name).Return ("Test");
      _declarationMethodInformationStub.Stub (stub => stub.DeclaringType).Return (typeInformationStub);

      Assert.That (_mixinIntroducedMethodInformation.ToString(), Is.EqualTo ("Test (impl of 'Boolean') (Mixin)"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((IMethodInformation) _mixinIntroducedMethodInformation).IsNull, Is.False);
    }
  }
}