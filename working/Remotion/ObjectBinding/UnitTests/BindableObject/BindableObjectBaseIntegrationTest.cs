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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectBaseIntegrationTest
  {
    private ClassDerivedFromBindableObjectBase _instance;

    [SetUp]
    public void SetUp ()
    {
      _instance = new ClassDerivedFromBindableObjectBase();
    }

    [Test]
    public void BusinessObjectClass ()
    {
      Assert.That (_instance.BusinessObjectClass, Is.InstanceOf (typeof (BindableObjectClass)));
      var bindableObjectClass = (BindableObjectClass) _instance.BusinessObjectClass;
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.InstanceOf (typeof (BindableObjectProvider)));
      Assert.That (bindableObjectClass.ConcreteType, Is.EqualTo (typeof (ClassDerivedFromBindableObjectBase)));
      Assert.That (bindableObjectClass.TargetType, Is.EqualTo (typeof (ClassDerivedFromBindableObjectBase)));
    }

    [Test]
    public void GetProperty ()
    {
      _instance.String = "hoo";
      Assert.That (_instance.GetProperty ("String"), Is.EqualTo ("hoo"));
    }

    [Test]
    public void GetProperty_ExplicitInterfaceScalar ()
    {
      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);
      var value = new SimpleReferenceType();
      ((IInterfaceWithReferenceType<SimpleReferenceType>) instance).ExplicitInterfaceScalar = value;
      Assert.That (((IBusinessObject) instance).GetProperty ("ExplicitInterfaceScalar"), Is.SameAs (value));
    }

    [Test]
    public void GetProperty_ImplicitInterfaceScalar ()
    {
      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);
      var value = new SimpleReferenceType();
      instance.ImplicitInterfaceScalar = value;
      Assert.That (((IBusinessObject) instance).GetProperty ("ImplicitInterfaceScalar"), Is.SameAs (value));
    }

    [Test]
    public void GetProperty_ImplicitInterfaceReadOnlyScalar_WithReadWriteImplementation ()
    {
      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);
      var value = new SimpleReferenceType();
      instance.ImplicitInterfaceReadOnlyScalar = value;
      Assert.That (((IBusinessObject) instance).GetProperty ("ImplicitInterfaceReadOnlyScalar"), Is.SameAs (value));
    }

    [Test]
    public void SetProperty ()
    {
      _instance.SetProperty ("String", "damn");
      Assert.That (_instance.String, Is.EqualTo ("damn"));
    }

    [Test]
    public void SetProperty_ExplicitInterfaceScalar ()
    {
      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);
      var value = new SimpleReferenceType();
      ((IBusinessObject) instance).SetProperty ("ExplicitInterfaceScalar", value);
      Assert.That (((IInterfaceWithReferenceType<SimpleReferenceType>) instance).ExplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    public void SetProperty_ImplicitInterfaceScalar ()
    {
      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);
      var value = new SimpleReferenceType();
      ((IBusinessObject) instance).SetProperty ("ImplicitInterfaceScalar", value);
      Assert.That (instance.ImplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    [Ignore ("COMMONS-1439")]
    public void SetProperty_ImplicitInterfaceReadOnlyScalar_WithReadWriteImplementation ()
    {
      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>> (ParamList.Empty);
      var value = new SimpleReferenceType();
      ((IBusinessObject) instance).SetProperty ("ImplicitInterfaceReadOnlyScalar", value);
      Assert.That (instance.ImplicitInterfaceReadOnlyScalar, Is.SameAs (value));
    }

    [Test]
    public void GetProvider ()
    {
      Assert.That (
          BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassDerivedFromBindableObjectBase)),
          Is.SameAs (BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute>()));
      Assert.That (
          BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassDerivedFromBindableObjectBase)),
          Is.Not.SameAs (BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()));
    }

    [Test]
    public void ClassDerivedFromBindableObjectBaseOverridingMixinMethod ()
    {
      var instance = new ClassDerivedFromBindableObjectBaseOverridingMixinMethod();
      Assert.That (instance.BusinessObjectClass, Is.InstanceOf (typeof (BindableObjectClass)));
      Assert.That (
          ((BindableObjectClass) instance.BusinessObjectClass).TargetType,
          Is.SameAs (typeof (ClassDerivedFromBindableObjectBaseOverridingMixinMethod)));
      Assert.That (
          ((BindableObjectClass) instance.BusinessObjectClass).ConcreteType,
          Is.SameAs (TypeFactory.GetConcreteType (typeof (ClassDerivedFromBindableObjectBaseOverridingMixinMethod))));
    }
  }
}
