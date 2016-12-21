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
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class ObjectBindingToMappingTest : ObjectBindingTestBase
  {
    private BindableDomainObjectWithProperties _classWithPropertiesInstance;
    private BindableDomainObjectMixin _classWithPropertiesMixin;
    private BindableDomainObjectWithMixedPersistentProperties _classWithMixedPropertiesInstance;
    private IBusinessObject _classWithMixedPropertiesInstanceAsBusinessObject;
    private IBusinessObjectClass _classWithMixedPropertiesInstanceAsBusinessObjectClass;
    private IBusinessObject _classWithPropertiesMixinInstanceAsBusinessObject;
    private IBusinessObjectClass _classWithPropertiesMixinInstanceAsBusinessObjectClass;

    public override void SetUp ()
    {
      base.SetUp();
      _classWithPropertiesInstance = BindableDomainObjectWithProperties.NewObject();
      _classWithPropertiesMixin = Mixin.Get<BindableDomainObjectMixin> (_classWithPropertiesInstance);
      _classWithMixedPropertiesInstance = BindableDomainObjectWithMixedPersistentProperties.NewObject();
      
      _classWithPropertiesMixinInstanceAsBusinessObject = _classWithPropertiesMixin;
      _classWithPropertiesMixinInstanceAsBusinessObjectClass = _classWithPropertiesMixinInstanceAsBusinessObject.BusinessObjectClass;
      
      _classWithMixedPropertiesInstanceAsBusinessObject = _classWithMixedPropertiesInstance;
      _classWithMixedPropertiesInstanceAsBusinessObjectClass = _classWithMixedPropertiesInstanceAsBusinessObject.BusinessObjectClass;
    }

    [Test]
    public void StandardProperty_DefaultValue ()
    {
      IBusinessObjectProperty property = _classWithPropertiesMixinInstanceAsBusinessObjectClass.GetPropertyDefinition ("RequiredStringProperty");
      Assert.That (_classWithPropertiesMixinInstanceAsBusinessObject.GetProperty (property), Is.Null);
    }

    [Test]
    public void StandardProperty_NonDefaultValue ()
    {
      IBusinessObjectProperty property = _classWithPropertiesMixinInstanceAsBusinessObjectClass.GetPropertyDefinition ("RequiredStringProperty");
      string propertyValue = "test";
      _classWithPropertiesInstance.RequiredStringProperty = propertyValue;
      Assert.That (_classWithPropertiesMixinInstanceAsBusinessObject.GetProperty (property), Is.EqualTo (propertyValue));
    }

    [Test]
    public void StandardProperty_NonDefaultValue_WithUnchangedValue ()
    {
      IBusinessObjectProperty property = _classWithPropertiesMixinInstanceAsBusinessObjectClass.GetPropertyDefinition ("RequiredStringProperty");
      var propertyValue = _classWithPropertiesInstance.RequiredStringProperty;
      _classWithPropertiesInstance.RequiredStringProperty = propertyValue;
      Assert.That (_classWithPropertiesMixinInstanceAsBusinessObject.GetProperty (property), Is.EqualTo (propertyValue));
    }

    [Test]
    public void InterfaceProperty_DefaultValue ()
    {
      IBusinessObjectProperty property = _classWithPropertiesMixinInstanceAsBusinessObjectClass.GetPropertyDefinition ("RequiredStringPropertyInInterface");
      Assert.That (_classWithPropertiesMixinInstanceAsBusinessObject.GetProperty (property), Is.Null);
    }

    [Test]
    public void InterfaceProperty_NonDefaultValue ()
    {
      IBusinessObjectProperty property = _classWithPropertiesMixinInstanceAsBusinessObjectClass.GetPropertyDefinition ("RequiredStringPropertyInInterface");
      string propertyValue = "test";
      _classWithPropertiesInstance.RequiredStringPropertyInInterface = propertyValue;
      Assert.That (_classWithPropertiesMixinInstanceAsBusinessObject.GetProperty (property), Is.EqualTo (propertyValue));
    }

    [Test]
    public void InterfaceProperty_NonDefaultValue_WithUnchangedValue ()
    {
      IBusinessObjectProperty property = _classWithPropertiesMixinInstanceAsBusinessObjectClass.GetPropertyDefinition ("RequiredStringPropertyInInterface");
      var propertyValue = _classWithPropertiesInstance.RequiredStringPropertyInInterface;
      _classWithPropertiesInstance.RequiredStringPropertyInInterface = propertyValue;
      Assert.That (_classWithPropertiesMixinInstanceAsBusinessObject.GetProperty (property), Is.EqualTo (propertyValue));
    }

    [Test]
    public void ExplicitInterfaceProperty_DefaultValue ()
    {
      IBusinessObjectProperty property = _classWithPropertiesMixinInstanceAsBusinessObjectClass.GetPropertyDefinition ("RequiredStringPropertyExplicitInInterface");
      Assert.That (_classWithPropertiesMixinInstanceAsBusinessObject.GetProperty (property), Is.Null);
    }

    [Test]
    public void ExplicitInterfaceProperty_NonDefaultValue ()
    {
      IBusinessObjectProperty property = _classWithPropertiesMixinInstanceAsBusinessObjectClass.GetPropertyDefinition ("RequiredStringPropertyExplicitInInterface");
      string propertyValue = "test";
      ((IBindableDomainObjectWithProperties) _classWithPropertiesInstance).RequiredStringPropertyExplicitInInterface = propertyValue;
      Assert.That (_classWithPropertiesMixinInstanceAsBusinessObject.GetProperty (property), Is.EqualTo (propertyValue));
    }

    [Test]
    public void ExplicitInterfaceProperty_NonDefaultValue_WithUnchangedValue ()
    {
      IBusinessObjectProperty property = _classWithPropertiesMixinInstanceAsBusinessObjectClass.GetPropertyDefinition ("RequiredStringPropertyExplicitInInterface");
      var propertyValue = ((IBindableDomainObjectWithProperties) _classWithPropertiesInstance).RequiredStringPropertyExplicitInInterface;
      ((IBindableDomainObjectWithProperties) _classWithPropertiesInstance).RequiredStringPropertyExplicitInInterface = propertyValue;
      Assert.That (_classWithPropertiesMixinInstanceAsBusinessObject.GetProperty (property), Is.EqualTo (propertyValue));
    }
    
    [Test]
    public void MixedPublicProperty_DefaultValue ()
    {
      IBusinessObjectProperty mixedProperty = _classWithMixedPropertiesInstanceAsBusinessObjectClass.GetPropertyDefinition ("PublicMixedProperty");
      Assert.That (_classWithMixedPropertiesInstanceAsBusinessObject.GetProperty (mixedProperty), Is.Null);
    }

    [Test]
    public void MixedPublicProperty_NonDefaultValue ()
    {
      IBusinessObjectProperty mixedProperty = _classWithMixedPropertiesInstanceAsBusinessObjectClass.GetPropertyDefinition ("PublicMixedProperty");
      var dateTime = new DateTime (2008, 08, 01);
      ((IMixinAddingPersistentProperties) _classWithMixedPropertiesInstance).PublicMixedProperty = dateTime;
      Assert.That (_classWithMixedPropertiesInstanceAsBusinessObject.GetProperty (mixedProperty), Is.EqualTo (dateTime));
    }

    [Test]
    public void MixedPublicProperty_NonDefaultValue_WithUnchangedValue ()
    {
      IBusinessObjectProperty mixedProperty = _classWithMixedPropertiesInstanceAsBusinessObjectClass.GetPropertyDefinition ("PublicMixedProperty");
      var dateTime = ((IMixinAddingPersistentProperties) _classWithMixedPropertiesInstance).PublicMixedProperty;
      ((IMixinAddingPersistentProperties) _classWithMixedPropertiesInstance).PublicMixedProperty = dateTime;
      Assert.That (_classWithMixedPropertiesInstanceAsBusinessObject.GetProperty (mixedProperty), Is.EqualTo (dateTime));
    }

    [Test]
    public void MixedPrivateProperty_DefaultValue ()
    {
      IBusinessObjectProperty mixedProperty = _classWithMixedPropertiesInstanceAsBusinessObjectClass.GetPropertyDefinition ("PrivateMixedProperty");
      Assert.That (_classWithMixedPropertiesInstanceAsBusinessObject.GetProperty (mixedProperty), Is.Null);
    }

    [Test]
    public void MixedPrivateProperty_NonDefaultValue ()
    {
      IBusinessObjectProperty mixedProperty = _classWithMixedPropertiesInstanceAsBusinessObjectClass.GetPropertyDefinition ("PrivateMixedProperty");
      var dateTime = new DateTime (2008, 08, 01);
      ((IMixinAddingPersistentProperties) _classWithMixedPropertiesInstance).PrivateMixedProperty = dateTime;
      Assert.That (_classWithMixedPropertiesInstanceAsBusinessObject.GetProperty (mixedProperty), Is.EqualTo (dateTime));
    }

    [Test]
    public void MixedPrivateProperty_NonDefaultValue_WithUnchangedValue ()
    {
      IBusinessObjectProperty mixedProperty = _classWithMixedPropertiesInstanceAsBusinessObjectClass.GetPropertyDefinition ("PrivateMixedProperty");
      var dateTime = ((IMixinAddingPersistentProperties) _classWithMixedPropertiesInstance).PrivateMixedProperty;
      ((IMixinAddingPersistentProperties) _classWithMixedPropertiesInstance).PrivateMixedProperty = dateTime;
      Assert.That (_classWithMixedPropertiesInstanceAsBusinessObject.GetProperty (mixedProperty), Is.EqualTo (dateTime));
    }

    [Test]
    public void MixedExplicitProperty_DefaultValue ()
    {
      IBusinessObjectProperty mixedProperty = _classWithMixedPropertiesInstanceAsBusinessObjectClass.GetPropertyDefinition ("ExplicitMixedProperty");
      Assert.That (_classWithMixedPropertiesInstanceAsBusinessObject.GetProperty (mixedProperty), Is.Null);
    }

    [Test]
    public void MixedExplicitProperty_NonDefaultValue ()
    {
      IBusinessObjectProperty mixedProperty = _classWithMixedPropertiesInstanceAsBusinessObjectClass.GetPropertyDefinition ("ExplicitMixedProperty");
      var dateTime = new DateTime (2008, 08, 01);
      ((IMixinAddingPersistentProperties) _classWithMixedPropertiesInstance).ExplicitMixedProperty = dateTime;
      Assert.That (_classWithMixedPropertiesInstanceAsBusinessObject.GetProperty (mixedProperty), Is.EqualTo (dateTime));
    }

    [Test]
    public void MixedExplicitProperty_NonDefaultValue_WithUnchangedValue ()
    {
      var property = (PropertyBase) _classWithMixedPropertiesInstanceAsBusinessObjectClass.GetPropertyDefinition ("ExplicitMixedProperty");
      var dateTime = ((IMixinAddingPersistentProperties) _classWithMixedPropertiesInstance).ExplicitMixedProperty;
      ((IMixinAddingPersistentProperties) _classWithMixedPropertiesInstance).ExplicitMixedProperty = dateTime;
      Assert.That (_classWithMixedPropertiesInstanceAsBusinessObject.GetProperty (property), Is.EqualTo (dateTime));
    }
    
  }
}