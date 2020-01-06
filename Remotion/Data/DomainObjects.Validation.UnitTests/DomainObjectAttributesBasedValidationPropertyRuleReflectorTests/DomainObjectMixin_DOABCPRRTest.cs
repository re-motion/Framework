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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.Validation.UnitTests.DomainObjectAttributesBasedValidationPropertyRuleReflectorTests
{
  [TestFixture]
// ReSharper disable InconsistentNaming
    public class DomainObjectMixin_DomainObjectAttributesBasedValidationPropertyRuleReflectorTest
// ReSharper enable InconsistentNaming
  {
    private PropertyInfo _mixinPropertyWithoutAttribute;
    private PropertyInfo _mixinPropertyWithMandatoryAttribute;
    private PropertyInfo _mixinPropertyWithNullableStringPropertyAttribute;
    private PropertyInfo _mixinPropertyWithMandatoryStringPropertyAttribute;
    private PropertyInfo _interfacePropertyWithoutAttribute;
    private PropertyInfo _interfacePropertyWithMandatoryAttribute;
    private PropertyInfo _interfacePropertyWithNullableStringPropertyAttribute;
    private PropertyInfo _interfacePropertyWithMandatoryStringPropertyAttribute;
    private PropertyInfo _mixinIntProperty;
    private PropertyInfo _interfaceIntProperty;
    private PropertyInfo _mixinBidirectionalRelationProperty;
    private PropertyInfo _interfaceBidirectionalRelationProperty;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithoutAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithNullableStringPropertyAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithMandatoryStringPropertyAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _propertyWithMandatoryAttributeReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _intPropertyReflector;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _bidirectionalRelationReflector;
    private PropertyInfo _mixinBidirectionalMultipleRelationProperty;
    private PropertyInfo _interfaceBidirectionalMultipleRelationProperty;
    private DomainObjectAttributesBasedValidationPropertyRuleReflector _bidirectionalMultipleRelationReflector;
    private IValidationMessageFactory _validationMessageFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _mixinPropertyWithoutAttribute =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithoutAttribute");
      _interfacePropertyWithoutAttribute =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithoutAttribute");

      _mixinPropertyWithMandatoryAttribute =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithMandatoryAttribute");
      _interfacePropertyWithMandatoryAttribute =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithMandatoryAttribute");

      _mixinBidirectionalRelationProperty =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("BidirectionalPropertyWithMandatoryAttribute");
      _interfaceBidirectionalRelationProperty =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("BidirectionalPropertyWithMandatoryAttribute");

      _mixinBidirectionalMultipleRelationProperty =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("BidirectionalMultiplePropertyWithMandatoryAttribute");
      _interfaceBidirectionalMultipleRelationProperty =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("BidirectionalMultiplePropertyWithMandatoryAttribute");

      _mixinPropertyWithNullableStringPropertyAttribute =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithNullableStringPropertyAttribute");
      _interfacePropertyWithNullableStringPropertyAttribute =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithNullableStringPropertyAttribute");

      _mixinPropertyWithMandatoryStringPropertyAttribute =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("PropertyWithMandatoryStringPropertyAttribute");
      _interfacePropertyWithMandatoryStringPropertyAttribute =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty (
              "PropertyWithMandatoryStringPropertyAttribute");

      _mixinIntProperty =
          typeof (MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("IntProperty");
      _interfaceIntProperty =
          typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty ("IntProperty");

      var domainModelConstraintProvider = new DomainModelConstraintProvider();
      _validationMessageFactoryStub = MockRepository.GenerateStub<IValidationMessageFactory>();

      _propertyWithoutAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _interfacePropertyWithoutAttribute,
          _mixinPropertyWithoutAttribute,
          domainModelConstraintProvider,
          _validationMessageFactoryStub);
      _propertyWithMandatoryAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _interfacePropertyWithMandatoryAttribute,
          _mixinPropertyWithMandatoryAttribute,
          domainModelConstraintProvider,
          _validationMessageFactoryStub);
      _propertyWithNullableStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector (
              _interfacePropertyWithNullableStringPropertyAttribute,
              _mixinPropertyWithNullableStringPropertyAttribute,
              domainModelConstraintProvider,
              _validationMessageFactoryStub);
      _propertyWithMandatoryStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector (
              _interfacePropertyWithMandatoryStringPropertyAttribute,
              _mixinPropertyWithMandatoryStringPropertyAttribute,
              domainModelConstraintProvider,
              _validationMessageFactoryStub);

      _intPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _interfaceIntProperty,
          _mixinIntProperty,
          domainModelConstraintProvider,
          _validationMessageFactoryStub);

      _bidirectionalRelationReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _interfaceBidirectionalRelationProperty,
          _mixinBidirectionalRelationProperty, 
          domainModelConstraintProvider,
          _validationMessageFactoryStub);

      _bidirectionalMultipleRelationReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector (
          _interfaceBidirectionalMultipleRelationProperty,
          _mixinBidirectionalMultipleRelationProperty,
          domainModelConstraintProvider,
          _validationMessageFactoryStub);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (
          ((PropertyInfoAdapter) _propertyWithoutAttributeReflector.ValidatedProperty).PropertyInfo,
          Is.EqualTo (_interfacePropertyWithoutAttribute));
      Assert.That (
          ((PropertyInfoAdapter) _propertyWithMandatoryAttributeReflector.ValidatedProperty).PropertyInfo,
          Is.EqualTo (_interfacePropertyWithMandatoryAttribute));
      Assert.That (
          ((PropertyInfoAdapter) _propertyWithNullableStringPropertyAttributeReflector.ValidatedProperty).PropertyInfo,
          Is.EqualTo (_interfacePropertyWithNullableStringPropertyAttribute));
      Assert.That (
          ((PropertyInfoAdapter) _propertyWithMandatoryStringPropertyAttributeReflector.ValidatedProperty).PropertyInfo,
          Is.EqualTo (_interfacePropertyWithMandatoryStringPropertyAttribute));
    }

    [Test]
    public void GetPropertyAccessExpression_NonRelationStringProperty ()
    {
      var propertyAccessor = _propertyWithMandatoryStringPropertyAttributeReflector
          .GetValidatedPropertyFunc (typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject ();
        var propertyValue = "test";
        ((IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface) obj).PropertyWithMandatoryStringPropertyAttribute = propertyValue;
        var result = propertyAccessor (obj);
        Assert.That (result, Is.SameAs (propertyValue));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_NonRelationIntProperty ()
    {
      var propertyAccessor = _intPropertyReflector
          .GetValidatedPropertyFunc (typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject ();
        var propertyValue = 10;
        ((IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface) obj).IntProperty = propertyValue;
        var result = propertyAccessor (obj);
        Assert.That (result, Is.EqualTo (propertyValue));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_UnidirectionalRelation ()
    {
      var propertyAccessor = _propertyWithMandatoryAttributeReflector
          .GetValidatedPropertyFunc (typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject();
        var propertyValue = TestDomainObject.NewObject();
        ((IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface) obj).PropertyWithMandatoryAttribute = propertyValue;
        var result = propertyAccessor (obj);
        Assert.That (result, Is.SameAs (propertyValue));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_BidirectionalRelation ()
    {
      var propertyAccessor = _bidirectionalRelationReflector
          .GetValidatedPropertyFunc (typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject ();
        var propertyValue = TestDomainObject.NewObject ();
        ((IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface) obj).BidirectionalPropertyWithMandatoryAttribute = propertyValue;
        var result = propertyAccessor (obj);
        Assert.That (result, Is.SameAs (propertyValue));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_BidirectionalMultipleRelation ()
    {
      var propertyAccessor = _bidirectionalMultipleRelationReflector
          .GetValidatedPropertyFunc (typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject ();
        var propertyValue = TestDomainObject.NewObject ();
        ((IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface) obj).BidirectionalMultiplePropertyWithMandatoryAttribute.Add(propertyValue);
        var result = propertyAccessor (obj);
        Assert.That (result, Is.EqualTo (new[] { propertyValue }));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_BidirectionalRelation_NotLoaded ()
    {
      var propertyAccessor = _bidirectionalRelationReflector
         .GetValidatedPropertyFunc (typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var obj = (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface) MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject ();
        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          var result = propertyAccessor (obj);

          Assert.That (result, Is.Not.Null);
          Assert.That (result.GetType ().Name, Is.EqualTo ("FakeDomainObject"));
        }
      }
    }

    [Test]
    public void GetPropertyAccessExpression_BidirectionalMultipleRelation_NotLoaded ()
    {
      var propertyAccessor = _bidirectionalMultipleRelationReflector
         .GetValidatedPropertyFunc (typeof (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var obj = (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface) MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject ();
        using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
        {
          var result = propertyAccessor (obj) as IEnumerable<object>;

          Assert.That (result, Is.Not.Null);
          Assert.That (result.Select (r => r.GetType().Name), Is.EqualTo (new[] { "FakeDomainObject" }));
        }
      }
    }
    
    [Test]
    public void NoAttributes ()
    {
      Assert.That (_propertyWithoutAttributeReflector.GetAddingPropertyValidators().Any(), Is.False);
      Assert.That (_propertyWithoutAttributeReflector.GetHardConstraintPropertyValidators().Any(), Is.False);
      Assert.That (_propertyWithoutAttributeReflector.GetRemovingPropertyRegistrations().Any(), Is.False);
      Assert.That (_propertyWithoutAttributeReflector.GetMetaValidationRules().Any(), Is.False);
    }

    [Test]
    public void GetHardConstraintPropertyValidators_MandatoryAttribute ()
    {
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (NotNullValidator),
                  PropertyInfoAdapter.Create (_mixinPropertyWithMandatoryAttribute)))
          .Return (validationMessageStub);

      var result = _propertyWithMandatoryAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (NotNullValidator)));
      Assert.That (((NotNullValidator) result[0]).ValidationMessage, Is.SameAs (validationMessageStub));
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void GetAddingPropertyValidators_MandatoryAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetAddingPropertyValidators_NullableStringPropertyAttribute ()
    {
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (LengthValidator),
                  PropertyInfoAdapter.Create (_mixinPropertyWithNullableStringPropertyAttribute)))
          .Return (validationMessageStub);

      var result = _propertyWithNullableStringPropertyAttributeReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (LengthValidator)));
      Assert.That (((LengthValidator) result[0]).Max, Is.EqualTo (10));
      Assert.That (((LengthValidator) result[0]).ValidationMessage, Is.SameAs (validationMessageStub));
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void GetAddingPropertyValidators_NullableStringPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetAddingPropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var lengthValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (LengthValidator),
                  PropertyInfoAdapter.Create (_mixinPropertyWithMandatoryStringPropertyAttribute)))
          .Return (lengthValidationMessageStub);

      var notEmptyValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (NotEmptyValidator),
                  PropertyInfoAdapter.Create (_mixinPropertyWithMandatoryStringPropertyAttribute)))
          .Return (notEmptyValidationMessageStub);

      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (2));
      Assert.That (result[0], Is.TypeOf (typeof (LengthValidator)));
      Assert.That (((LengthValidator) result[0]).Max, Is.EqualTo (20));
      Assert.That (((LengthValidator) result[0]).ValidationMessage, Is.SameAs (lengthValidationMessageStub));

      Assert.That (result[1], Is.TypeOf (typeof (NotEmptyValidator)));
      Assert.That (((NotEmptyValidator) result[1]).ValidationMessage, Is.SameAs (notEmptyValidationMessageStub));
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void GetAddingPropertyValidators_MandatoryStringPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetHardConstraintPropertyValidators_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetHardConstraintPropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  typeof (NotNullValidator),
                  PropertyInfoAdapter.Create (_mixinPropertyWithMandatoryStringPropertyAttribute)))
          .Return (validationMessageStub);

      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (NotNullValidator)));
      Assert.That (((NotNullValidator) result[0]).ValidationMessage, Is.SameAs (validationMessageStub));
    }

    [Test]
    public void GetRemovingPropertyRegistrations ()
    {
      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetRemovingPropertyRegistrations().ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetMetaValidationRules_MandatoryAttribute ()
    {
      var result = _propertyWithMandatoryAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That (result.Any(), Is.False);
    }

    [Test]
    public void GetMetaValidationRules_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (RemotionMaxLengthMetaValidationRule)));
      Assert.That (((RemotionMaxLengthMetaValidationRule) result[0]).MaxLength, Is.EqualTo (10));
    }

    [Test]
    public void Initialize_WithMixinPropertyAsInterfaceProperty_ThrowsArgumentException ()
    {
      var domainModelConstraintProvider = new DomainModelConstraintProvider();
      Assert.That (
          () => new DomainObjectAttributesBasedValidationPropertyRuleReflector (
              _mixinPropertyWithMandatoryAttribute,
              _mixinPropertyWithMandatoryAttribute,
              domainModelConstraintProvider,
              _validationMessageFactoryStub),
          Throws.ArgumentException.And.Message.EqualTo (
              "The property 'PropertyWithMandatoryAttribute' was declared on type 'MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface' "
              + "but only interface declarations are supported when using mixin properties.\r\nParameter name: interfaceProperty"));
    }
  }
}