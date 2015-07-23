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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectWithIdentityBaseIntegrationTest
  {
    private ClassDerivedFromBindableObjectWithIdentityBase _instance;
    private ClassDerivedFromBindableObjectWithIdentityBaseOverridingDisplayName _instanceOverridingDisplayName;

    [SetUp]
    public void SetUp()
    {
      _instance = new ClassDerivedFromBindableObjectWithIdentityBase ();
      _instanceOverridingDisplayName = new ClassDerivedFromBindableObjectWithIdentityBaseOverridingDisplayName ();
    }

    [Test]
    public void BusinessObjectClass()
    {
      Assert.That (_instance.BusinessObjectClass, Is.InstanceOf (typeof (BindableObjectClass)));
      var bindableObjectClass = (BindableObjectClass) _instance.BusinessObjectClass;
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.InstanceOf (typeof (BindableObjectProvider)));
      Assert.That (bindableObjectClass.ConcreteType, Is.EqualTo (typeof (ClassDerivedFromBindableObjectWithIdentityBase)));
      Assert.That (bindableObjectClass.TargetType, Is.EqualTo (typeof (ClassDerivedFromBindableObjectWithIdentityBase)));
    }

    [Test]
    public void DisplayName_Default()
    {
      Assert.That (_instance.DisplayName, Is.EqualTo (TypeUtility.GetPartialAssemblyQualifiedName (typeof (ClassDerivedFromBindableObjectWithIdentityBase))));
    }

    [Test]
    public void DisplayName_Overridden ()
    {
      Assert.That (_instanceOverridingDisplayName.DisplayName, Is.EqualTo ("Overrotten!"));
    }

    [Test]
    public void GetProperty()
    {
      _instance.String = "hoo";
      Assert.That (_instance.GetProperty ("String"), Is.EqualTo ("hoo"));
    }

    [Test]
    public void SetProperty ()
    {
      _instance.SetProperty ("String", "damn");
      Assert.That (_instance.String, Is.EqualTo ("damn"));
    }

    [Test]
    public void UniqueIdentifier()
    {
      _instance.SetUniqueIdentifier ("hu");
      Assert.That (_instance.UniqueIdentifier, Is.EqualTo ("hu"));
    }

    [Test]
    public void GetProvider ()
    {
      Assert.That (
          BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassDerivedFromBindableObjectWithIdentityBase)),
          Is.SameAs (BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute> ()));
      Assert.That (
          BindableObjectProvider.GetProviderForBindableObjectType (typeof (ClassDerivedFromBindableObjectWithIdentityBase)),
          Is.Not.SameAs (BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute> ()));
    }

    [Test]
    public void ClassDerivedFromBindableObjectWithIdentityBaseOverridingMixinMethod ()
    {
      var instance = new ClassDerivedFromBindableObjectWithIdentityBaseOverridingMixinMethod ();
      Assert.That (instance.BusinessObjectClass, Is.InstanceOf (typeof (BindableObjectClass)));
      Assert.That (((BindableObjectClass) instance.BusinessObjectClass).TargetType, Is.SameAs (typeof (ClassDerivedFromBindableObjectWithIdentityBaseOverridingMixinMethod)));
      Assert.That (((BindableObjectClass) instance.BusinessObjectClass).ConcreteType, Is.SameAs (TypeFactory.GetConcreteType (typeof (ClassDerivedFromBindableObjectWithIdentityBaseOverridingMixinMethod))));
    }
  }
}
