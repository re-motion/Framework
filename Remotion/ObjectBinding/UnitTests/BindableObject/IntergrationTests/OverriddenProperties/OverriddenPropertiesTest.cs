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
using Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.OverriddenProperties.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.OverriddenProperties
{
  [TestFixture]
  public class OverriddenPropertiesTest
  {
    [Test]
    public void GetProperty_OverriddenByMixin_AndOnlyAccessorIsMixed ()
    {
      var instance = ObjectFactory.Create<TargetClass>();
      var businessObject = (IBusinessObject)instance;
      var businessObjectClass = businessObject.BusinessObjectClass;
      var businessObjectProperty = businessObjectClass.GetPropertyDefinition("MixinOverrideTargetProperty");
      Assert.That(businessObjectProperty, Is.Not.Null);

      instance.MixinOverrideTargetProperty = "Value";

      Assert.That(businessObject.GetProperty(businessObjectProperty), Is.EqualTo("Value (mixed)"));
    }

    [Test]
    public void GetProperty_OverriddenByMixin_PropertyMetadataIsMixed ()
    {
      var instance = ObjectFactory.Create<TargetClass>();
      var businessObject = (IBusinessObject)instance;
      var businessObjectClass = businessObject.BusinessObjectClass;
      var businessObjectProperty = businessObjectClass.GetPropertyDefinition("MixinOverrideTargetPropertyWithPropertyMetadata");
      Assert.That(businessObjectProperty, Is.Not.Null);

      instance.MixinOverrideTargetPropertyWithPropertyMetadata = "Value";

      Assert.That(businessObject.GetProperty(businessObjectProperty), Is.EqualTo("Value (mixed)"));
    }

    [Test]
    [Ignore ("RM-6877 - InvalidCaseException")]
    public void GetProperty_OverriddenByMixin_AndOnlyAccessorIsMixed_AndDerivedInstanceType ()
    {
      var instance = ObjectFactory.Create<TargetClass>();
      var businessObjectClass = ((IBusinessObject)instance).BusinessObjectClass;
      var businessObjectProperty = businessObjectClass.GetPropertyDefinition("MixinOverrideTargetProperty");
      Assert.That(businessObjectProperty, Is.Not.Null);

      var derived = ObjectFactory.Create<DerivedTargetClass>();

      derived.MixinOverrideTargetProperty = "Value";

      Assert.That(((IBusinessObject)derived).GetProperty(businessObjectProperty), Is.EqualTo("Value (mixed)"));
    }

    [Test]
    [Ignore ("RM-6877 - InvalidCaseException")]
    public void GetProperty_OverriddenByMixin_PropertyMetadataIsMixed_AndDerivedInstanceType ()
    {
      var instance = ObjectFactory.Create<TargetClass>();
      var businessObjectClass = ((IBusinessObject)instance).BusinessObjectClass;
      var businessObjectProperty = businessObjectClass.GetPropertyDefinition("MixinOverrideTargetPropertyWithPropertyMetadata");
      Assert.That(businessObjectProperty, Is.Not.Null);

      var derived = ObjectFactory.Create<DerivedTargetClass>();

      derived.MixinOverrideTargetPropertyWithPropertyMetadata = "Value";

      Assert.That(((IBusinessObject)derived).GetProperty(businessObjectProperty), Is.EqualTo("Value (mixed)"));
    }
  }
}
