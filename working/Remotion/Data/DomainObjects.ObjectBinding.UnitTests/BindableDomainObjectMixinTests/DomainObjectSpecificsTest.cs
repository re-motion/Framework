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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class DomainObjectSpecificsTest : ObjectBindingTestBase
  {
    private BindableObjectClass _businessObjectClassWithProperties;
    private BindableObjectClass _businessObjectSampleClass;

    public override void SetUp ()
    {
      base.SetUp ();
      
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (BindableDomainObjectWithProperties));
      _businessObjectClassWithProperties = provider.GetBindableObjectClass (typeof (BindableDomainObjectWithProperties));
      _businessObjectSampleClass = provider.GetBindableObjectClass (typeof (SampleBindableMixinDomainObject));
    }

    [Test]
    public void OrdinaryProperty ()
    {
      Assert.That (_businessObjectSampleClass.GetPropertyDefinition ("Name"), Is.Not.Null);
    }

    [Test]
    public void UsesBindableDomainObjectMetadataFactory ()
    {
      Assert.That (
        BindableObjectProvider.GetProviderForBindableObjectType(typeof (SampleBindableMixinDomainObject)).MetadataFactory,
        Is.InstanceOf (typeof (BindableDomainObjectMetadataFactory)));
    }

    [Test]
    public void NoIDProperty ()
    {
      Assert.That (_businessObjectSampleClass.GetPropertyDefinition ("ID"), Is.Null);
    }

    [Test]
    public void NoPropertyFromDomainObject ()
    {
      PropertyBase[] properties = (PropertyBase[]) _businessObjectSampleClass.GetPropertyDefinitions ();

      foreach (PropertyBase property in properties)
        Assert.That (property.PropertyInfo.DeclaringType, Is.Not.EqualTo (typeof (DomainObject)));
    }

    [Test]
    public void PropertyNotInMapping ()
    {
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredPropertyNotInMapping"), Is.Not.Null);
    }

    [Test]
    public void PropertyInMapping ()
    {
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredStringProperty"), Is.Not.Null);
    }

    [Test]
    public void ProtectedPropertyInMapping ()
    {
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("ProtectedStringProperty"), Is.Null);
    }

    [Test]
    public void Requiredness ()
    {
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredPropertyNotInMapping").IsRequired, Is.True);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredStringProperty").IsRequired, Is.True);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredValueProperty").IsRequired, Is.True);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredEnumProperty").IsRequired, Is.True);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredRelatedObjectProperty").IsRequired, Is.True);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredBidirectionalRelatedObjectProperty").IsRequired, Is.True);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredBidirectionalRelatedObjectsProperty").IsRequired, Is.True);

      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredPropertyNotInMapping").IsRequired, Is.False);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredStringProperty").IsRequired, Is.False);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredValueProperty").IsRequired, Is.False);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredEnumProperty").IsRequired, Is.False);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredUndefinedEnumProperty").IsRequired, Is.False);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredRelatedObjectProperty").IsRequired, Is.False);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredBidirectionalRelatedObjectProperty").IsRequired, Is.False);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredBidirectionalRelatedObjectsProperty").IsRequired, Is.False);
    }

    [Test]
    public void MaxLength ()
    {
      Assert.That (((IBusinessObjectStringProperty)
                    _businessObjectClassWithProperties.GetPropertyDefinition ("MaxLength7StringProperty")).MaxLength, Is.EqualTo (7));

      Assert.That (((IBusinessObjectStringProperty)
                    _businessObjectClassWithProperties.GetPropertyDefinition ("NoMaxLengthStringPropertyNotInMapping")).MaxLength, Is.Null);
      Assert.That (((IBusinessObjectStringProperty)
                    _businessObjectClassWithProperties.GetPropertyDefinition ("NoMaxLengthStringProperty")).MaxLength, Is.Null);
    }

    [Test]
    public void InheritanceAndOverriding ()
    {
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("BasePropertyWithMaxLength3"), Is.Not.Null);
      Assert.That (_businessObjectClassWithProperties.GetPropertyDefinition ("BasePropertyWithMaxLength4"), Is.Not.Null);

      Assert.That (((IBusinessObjectStringProperty)
                    _businessObjectClassWithProperties.GetPropertyDefinition ("BasePropertyWithMaxLength3")).MaxLength, Is.EqualTo (33));
      Assert.That (((IBusinessObjectStringProperty)
                    _businessObjectClassWithProperties.GetPropertyDefinition ("BasePropertyWithMaxLength4")).MaxLength, Is.EqualTo (4));
    }

    [Test]
    public void NullabilityResolvedFromAboveInheritanceRoot ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (BindableDomainObjectAboveInheritanceRoot));
      var businessObjectClass = provider.GetBindableObjectClass (typeof (BindableDomainObjectAboveInheritanceRoot));

      var notNullableBooleanProperty = businessObjectClass.GetPropertyDefinition ("NotNullableBooleanProperty");
      Assert.That (notNullableBooleanProperty.IsRequired, Is.True);

      var notNullableStringProperty = businessObjectClass.GetPropertyDefinition ("NotNullableStringPropertyWithLengthConstraint");
      Assert.That (notNullableStringProperty.IsRequired, Is.True);

      var notNullableRelationProperty = businessObjectClass.GetPropertyDefinition ("MandatoryUnidirectionalRelation");
      Assert.That (notNullableRelationProperty.IsRequired, Is.True);

      var nullableBooleanProperty = businessObjectClass.GetPropertyDefinition ("NullableBooleanProperty");
      Assert.That (nullableBooleanProperty.IsRequired, Is.False);

      var nullableStringProperty = businessObjectClass.GetPropertyDefinition ("NullableStringPropertyWithoutLengthConstraint");
      Assert.That (nullableStringProperty.IsRequired, Is.False);

      var nullableRelationProperty = businessObjectClass.GetPropertyDefinition ("NotMandatoryUnidirectionalRelation");
      Assert.That (nullableRelationProperty.IsRequired, Is.False);
    }

    [Test]
    public void LengthConstraintResolvedFromAboveInheritanceRoot ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (BindableDomainObjectAboveInheritanceRoot));
      var businessObjectClass = provider.GetBindableObjectClass (typeof (BindableDomainObjectAboveInheritanceRoot));

      var stringPropertyWithLengthConstraint =
          (IBusinessObjectStringProperty) businessObjectClass.GetPropertyDefinition ("NotNullableStringPropertyWithLengthConstraint");
      Assert.That (stringPropertyWithLengthConstraint.MaxLength, Is.EqualTo (100));

      var stringPropertyWithoutLengthConstraint =
          (IBusinessObjectStringProperty) businessObjectClass.GetPropertyDefinition ("NullableStringPropertyWithoutLengthConstraint");
      Assert.That (stringPropertyWithoutLengthConstraint.MaxLength, Is.Null);
    }
  }
}
