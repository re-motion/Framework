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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectWithIdentityMixinTest : TestBase
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That(
          ObjectFactory.Create<ClassWithIdentity>(ParamList.Create("TheUniqueIdentifier")),
          Is.InstanceOf(typeof(IBusinessObjectWithIdentity)));
    }

    [Test]
    public void GetUniqueIdentifier ()
    {
      BindableObjectWithIdentityMixin mixin =
          Mixin.Get<BindableObjectWithIdentityMixin>(ObjectFactory.Create<ClassWithIdentity>(ParamList.Create("TheUniqueIdentifier")));
      IBusinessObjectWithIdentity businessObjectWithIdentity = mixin;

      Assert.That(businessObjectWithIdentity.UniqueIdentifier, Is.SameAs("TheUniqueIdentifier"));
    }

    [Test]
    public void DisplayName_Overridden_ValueFromMixin ()
    {
      BindableObjectWithIdentityMixin mixin =
          Mixin.Get<BindableObjectWithIdentityMixin>(ObjectFactory.Create<ClassWithIdentityAndDisplayName>(ParamList.Create("TheUniqueIdentifier")));
      IBusinessObjectWithIdentity businessObjectWithIdentity = mixin;

      Assert.That(businessObjectWithIdentity.DisplayName, Is.SameAs("TheUniqueIdentifier"));
    }

    [Test]
    public void DisplayName_Overridden_ValueFromConcreteType ()
    {
      var businessObject = (IBusinessObjectWithIdentity)ObjectFactory.Create<ClassWithOverriddenDisplayName>(ParamList.Empty);

      Assert.That(businessObject.DisplayName, Is.EqualTo("TheDisplayName"));
    }

    [Test]
    public void DisplayName_BaseImplementation ()
    {
      var mixin = Mixin.Get<BindableObjectWithIdentityMixin>(ObjectFactory.Create<ClassWithIdentity>(ParamList.Empty));
      IBusinessObjectWithIdentity businessObject = mixin;

      Assert.That(
          businessObject.DisplayName,
          Is.EqualTo("Remotion.ObjectBinding.UnitTests.TestDomain.ClassWithIdentity, Remotion.ObjectBinding.UnitTests"));
    }

    [Test]
    public void GetProvider ()
    {
      Assert.That(
          BindableObjectProvider.GetProviderForBindableObjectType(typeof(SimpleBusinessObjectClass)),
          Is.SameAs(BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute>()));
      Assert.That(
          BindableObjectProvider.GetProviderForBindableObjectType(typeof(SimpleBusinessObjectClass)),
          Is.Not.SameAs(BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()));
    }

    [Test]
    public void HasMixin ()
    {
      Assert.That(MixinTypeUtility.HasMixin(typeof(ClassWithIdentity), typeof(BindableObjectWithIdentityMixin)), Is.True);
      Assert.That(MixinTypeUtility.HasMixin(typeof(ClassWithAllDataTypes), typeof(BindableObjectWithIdentityMixin)), Is.False);
      Assert.That(MixinTypeUtility.HasMixin(typeof(object), typeof(BindableObjectWithIdentityMixin)), Is.False);
    }
  }
}
