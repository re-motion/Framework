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
using Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.Globalization.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.Globalization
{
  [TestFixture]
  public class AttributesBasedGlobalizationIntegrationTest : TestBase
  {
    private BindableObjectMetadataFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();

      _factory = BindableObjectMetadataFactory.Create();
    }

    [Test]
    public void PropertyDisplayName_NotInherited_NotOverridden_NotMixed ()
    {
      var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalizationViaAttributes), "Property2");

      Assert.That (resourceString, Is.EqualTo ("Property2 display name from TargetClassForGlobalizationViaAttributes"));
    }

    [Test]
    public void PropertyDisplayName_InheritedAndOverridden_NotMixed_PropertyEvaluatedForBaseClass ()
    {
      var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalizationViaAttributes), "Property1");

      Assert.That (resourceString, Is.EqualTo ("Property1 display name from TargetClassForGlobalizationViaAttributes"));
    }

    [Test]
    public void PropertyDisplayName_InheritedAndOverridden_NotMixed_PropertyEvaluatedForDerivedClass ()
    {
      var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalizationViaAttributes), "Property1");

      Assert.That (resourceString, Is.EqualTo ("Property1 display name from TargetClassForGlobalizationViaAttributes"));
    }

    [Test]
    public void PropertyDisplayName_TargetPropertyOverriddenByMixin ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassWithOverrideMixinForGlobalizationViaAttributes>()
          .AddMixin<MixinForGlobalizationViaAttributes>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (
            typeof (TargetClassWithOverrideMixinForGlobalizationViaAttributes),
            "PropertyForOverrideTarget");

        Assert.That (resourceString, Is.EqualTo ("PropertyForOverrideTarget display name from TargetClassForGlobalizationViaAttributes"));
      }
    }

    [Test]
    public void PropertyDisplayName_ImplicitInterfaceTargetPropertyOverriddenByMixin ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassWithOverrideMixinForGlobalizationViaAttributes>()
          .AddMixin<MixinForGlobalizationViaAttributes>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (
            typeof (TargetClassWithOverrideMixinForGlobalizationViaAttributes),
            "ImplicitImplementedPropertyForOverrideTarget");

        Assert.That (
            resourceString,
            Is.EqualTo ("ImplicitImplementedPropertyForOverrideTarget display name from TargetClassForGlobalizationViaAttributes"));
      }
    }

    [Test]
    public void PropertyDisplayName_MixinPropertyOverriddenByTarget ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassWithOverrideMixinForGlobalizationViaAttributes>()
          .AddMixin<MixinForGlobalizationViaAttributes>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (TargetClassWithOverrideMixinForGlobalizationViaAttributes), "PropertyForOverrideMixin");

        Assert.That (resourceString, Is.EqualTo ("PropertyForOverrideMixin display name from MixinForGlobalizationViaAttributes"));
      }
    }

    [Test]
    public void PropertyDisplayName_PropertyAddedViaMixin ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassWithOverrideMixinForGlobalizationViaAttributes>()
          .AddMixin<MixinForGlobalizationViaAttributes>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (TargetClassWithOverrideMixinForGlobalizationViaAttributes), "MixedProperty1");

        Assert.That (resourceString, Is.EqualTo ("MixedProperty1 display name from MixinForGlobalizationViaAttributes"));
      }
    }

    [Test]
    public void PropertyDisplayName_MixedProperty ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassWithOverrideMixinForGlobalizationViaAttributes>()
          .AddMixin<MixinForGlobalizationViaAttributes>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (TargetClassWithOverrideMixinForGlobalizationViaAttributes), "MixedProperty2");

        Assert.That (resourceString, Is.EqualTo ("MixedProperty2 display name from MixinForGlobalizationViaAttributes"));
      }
    }

    [Test]
    public void PropertyDisplayName_ImplicitInterfaceProperty ()
    {
      var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalizationViaAttributes), "ImplicitImplementedProperty");

      Assert.That (resourceString, Is.EqualTo ("ImplicitImplementedProperty display name from TargetClassForGlobalizationViaAttributes"));
    }

    [Test]
    public void PropertyDisplayName_ExplicitInterfaceProperty ()
    {
      var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalizationViaAttributes), "ExplicitImplementedProperty");

      Assert.That (resourceString, Is.EqualTo ("ExplicitImplementedProperty display name from TargetClassForGlobalizationViaAttributes"));
    }

    [Test]
    public void PropertyDisplayName_MixedExplicitProperty ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassWithOverrideMixinForGlobalizationViaAttributes>()
          .AddMixin<MixinForGlobalizationViaAttributes>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (TargetClassWithOverrideMixinForGlobalizationViaAttributes), "MixedExplicitProperty");

        Assert.That (resourceString, Is.EqualTo ("MixedExplicitProperty display name from MixinForGlobalizationViaAttributes"));
      }
    }

    [Test]
    public void PropertyDisplayName_MixedPropertyInDerivedClass ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<DerivedTargetClassWithOverrideMixinForGlobalizationViaAttributes>()
          .AddMixin<DerivedMixinForGlobalizationViaAttributes>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (DerivedTargetClassWithOverrideMixinForGlobalizationViaAttributes), "MixedProperty3");

        Assert.That (resourceString, Is.EqualTo ("MixedProperty3 display name from DerivedMixinForGlobalizationViaAttributes"));
      }
    }

    [Test]
    public void PropertyDisplayName_PropertyAddedInBaseClass_NotMixed ()
    {
      var resourceString = GetResourceStringForType (typeof (DerivedTargetClassForGlobalizationViaAttributes), "Property2");

      Assert.That (resourceString, Is.EqualTo ("Property2 display name from TargetClassForGlobalizationViaAttributes"));
    }

    [Test]
    public void PropertyDisplayName_PropertyAddedInDerivedClass_NotMixed ()
    {
      var resourceString = GetResourceStringForType (typeof (DerivedTargetClassForGlobalizationViaAttributes), "Property4");

      Assert.That (resourceString, Is.EqualTo ("Property4 display name from DerivedTargetClassForGlobalizationViaAttributes"));
    }

    private string GetResourceStringForType (Type targetType, string propertyName)
    {
      var classReflector = _factory.CreateClassReflector (targetType, new BindableObjectProvider());
      var @class = classReflector.GetMetadata();
      return @class.GetPropertyDefinition (propertyName).DisplayName;
    }
  }
}