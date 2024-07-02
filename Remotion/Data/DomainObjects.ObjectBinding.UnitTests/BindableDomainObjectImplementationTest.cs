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
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectImplementationTest : TestBase
  {
    [Test]
    public void Create ()
    {
      var wrapper = SampleBindableDomainObject.NewObject();
      var mixin = BindableDomainObjectImplementation.Create(wrapper);
      Assert.That(mixin.BusinessObjectClass, Is.Not.Null);
      Assert.That(PrivateInvoke.GetNonPublicProperty(mixin, "Target"), Is.SameAs(wrapper));
    }

    [Test]
    public void UniqueIdentifierViaImplementation ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var mixin = (BindableDomainObjectImplementation)PrivateInvoke.GetNonPublicField(instance, typeof(BindableDomainObject), "_implementation");
      Assert.That(mixin.UniqueIdentifier, Is.EqualTo(instance.ID.ToString()));
    }

    [Test]
    public void BaseUniqueIdentifier ()
    {
      var wrapper = SampleBindableDomainObject.NewObject();
      var implementation = BindableDomainObjectImplementation.Create(wrapper);
      Assert.That(implementation.BaseUniqueIdentifier, Is.EqualTo(wrapper.ID.ToString()));
    }

    [Test]
    public void UniqueIdentifier_ViaImplementation () // overriding UniqueIdentifier is not possbile in BindableDomainObjects
    {
      var wrapper = SampleBindableDomainObject.NewObject();
      var implementation = BindableDomainObjectImplementation.Create(wrapper);
      Assert.That(implementation.UniqueIdentifier, Is.EqualTo(wrapper.ID.ToString()));
    }

    [Test]
    public void BaseDisplayName ()
    {
      var wrapper = SampleBindableDomainObject.NewObject();
      var implementation = BindableDomainObjectImplementation.Create(wrapper);
      Assert.That(implementation.BaseDisplayName, Is.EqualTo(((IBusinessObject)wrapper).BusinessObjectClass.Identifier));
    }

    [Test]
    public void DisplayName_ViaImplementation_Default ()
    {
      var wrapper = SampleBindableDomainObject.NewObject();
      var implementation = BindableDomainObjectImplementation.Create(wrapper);
      Assert.That(implementation.DisplayName, Is.EqualTo(((IBusinessObject)wrapper).BusinessObjectClass.Identifier));
    }

    [Test]
    public void DisplayName_ViaImplementation_Overridden ()
    {
      var wrapper = SampleBindableDomainObjectWithOverriddenDisplayName.NewObject();
      var implementation = BindableDomainObjectImplementation.Create(wrapper);
      Assert.That(implementation.DisplayName, Is.EqualTo("TheDisplayName"));
    }

    [Test]
    public void BindableDomainObjectMixin_ImplementsMixinWithoutBaseObjectDependency ()
    {
      Assert.That(typeof(BindableDomainObjectImplementation).CanAscribeTo(typeof(Mixin<,>)), Is.False);
    }
  }
}
