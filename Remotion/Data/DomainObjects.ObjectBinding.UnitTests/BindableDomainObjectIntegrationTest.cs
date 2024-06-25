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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectIntegrationTest : ObjectBindingTestBase
  {
    private IBusinessObjectWithIdentity _instance;
    private IBusinessObjectWithIdentity _instanceOverridingDisplayName;

    public override void SetUp ()
    {
      base.SetUp();
      _instance = SampleBindableDomainObject.NewObject();
      _instanceOverridingDisplayName = SampleBindableDomainObjectWithOverriddenDisplayName.NewObject();
    }

    [Test]
    public void BindableDomainObjectIsDomainObject ()
    {
      Assert.That(ReflectionUtility.IsDomainObject(typeof(SampleBindableDomainObject)), Is.True);
    }

    [Test]
    public void DisplayName_Default ()
    {
      Assert.That(_instance.DisplayName, Is.EqualTo(TypeUtility.GetPartialAssemblyQualifiedName(typeof(SampleBindableDomainObject))));
    }

    [Test]
    public void DisplayName_Overridden ()
    {
      Assert.That(_instanceOverridingDisplayName.DisplayName, Is.EqualTo("TheDisplayName"));
    }

    [Test]
    public void UniqueIdentifier ()
    {
      Assert.That(_instance.UniqueIdentifier, Is.EqualTo(((SampleBindableDomainObject)_instance).ID.ToString()));
    }

    /// <summary>
    /// Verifies the interface implementation.
    /// </summary>
    [Test]
    public void VerifyInterfaceImplementation ()
    {
      IBusinessObjectWithIdentity businessObject = (SampleBindableDomainObjectWithOverriddenDisplayName)LifetimeService.NewObject(
                                                       TestableClientTransaction,
                                                       typeof(SampleBindableDomainObjectWithOverriddenDisplayName),
                                                       ParamList.Empty);
      var implementation =
          (BindableDomainObjectImplementation)PrivateInvoke.GetNonPublicField(businessObject, typeof(BindableDomainObject), "_implementation");

      Assert.That(businessObject.BusinessObjectClass, Is.SameAs(implementation.BusinessObjectClass));
      Assert.That(businessObject.DisplayName, Is.EqualTo(implementation.DisplayName));
      Assert.That(businessObject.DisplayName, Is.EqualTo("TheDisplayName"));
      businessObject.SetProperty("Int32", 1);
      Assert.That(businessObject.GetProperty("Int32"), Is.EqualTo(1));
      Assert.That(businessObject.GetProperty(implementation.BusinessObjectClass.GetPropertyDefinition("Int32")), Is.EqualTo(1));
      Assert.That(businessObject.GetPropertyString(implementation.BusinessObjectClass.GetPropertyDefinition("Int32"), "000"), Is.EqualTo("001"));
      Assert.That(businessObject.GetPropertyString("Int32"), Is.EqualTo("1"));
      Assert.That(businessObject.UniqueIdentifier, Is.EqualTo(implementation.UniqueIdentifier));
      businessObject.SetProperty(implementation.BusinessObjectClass.GetPropertyDefinition("Int32"), 2);
      Assert.That(businessObject.GetProperty("Int32"), Is.EqualTo(2));
    }

    [Test]
    public void GetProviderForBindableObjectType ()
    {
      BindableObjectProvider provider = BindableObjectProvider.GetProviderForBindableObjectType(typeof(BindableDomainObject));

      Assert.That(provider, Is.Not.Null);
      Assert.That(provider, Is.InstanceOf(typeof(BindableDomainObjectProvider)));
      Assert.That(provider, Is.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableDomainObjectProviderAttribute))));
      Assert.That(provider, Is.Not.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableObjectProviderAttribute))));
    }

    [Test]
    public void GetProvider ()
    {
      Assert.That(
          BindableDomainObjectProvider.GetProviderForBindableObjectType(typeof(SampleBindableDomainObject)),
          Is.SameAs(BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()));
      Assert.That(
          BindableDomainObjectProvider.GetProviderForBindableObjectType(typeof(SampleBindableDomainObject)),
          Is.Not.SameAs(BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute>()));
      Assert.That(
          BindableDomainObjectProvider.GetProviderForBindableObjectType(typeof(SampleBindableDomainObject)),
          Is.Not.SameAs(BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()));
    }

    [Test]
    public void NoPropertyFromDomainObject ()
    {
      var properties = (PropertyBase[])_instance.BusinessObjectClass.GetPropertyDefinitions();

      foreach (PropertyBase property in properties)
        Assert.That(property.PropertyInfo.DeclaringType, Is.Not.EqualTo(typeof(DomainObject)));
    }

    [Test]
    public void NoPropertyFromBindableDomainObject ()
    {
      var properties = (PropertyBase[])(_instance).BusinessObjectClass.GetPropertyDefinitions();

      foreach (PropertyBase property in properties)
        Assert.That(property.PropertyInfo.DeclaringType, Is.Not.EqualTo(typeof(BindableDomainObject)));
    }

    [Test]
    public void ClassDerivedFromBindableDomainObjectOverridingMixinMethod ()
    {
      var instance = (IBusinessObject)TestDomain.ClassDerivedFromBindableDomainObjectOverridingMixinMethod.NewObject();
      Assert.That(instance.BusinessObjectClass, Is.InstanceOf(typeof(BindableObjectClass)));
      Assert.That(
          ((BindableObjectClass)instance.BusinessObjectClass).TargetType,
          Is.SameAs(typeof(ClassDerivedFromBindableDomainObjectOverridingMixinMethod)));
      Assert.That(
          ((BindableObjectClass)instance.BusinessObjectClass).ConcreteType,
          Is.SameAs(TypeFactory.GetConcreteType(typeof(ClassDerivedFromBindableDomainObjectOverridingMixinMethod))));
    }
  }
}
