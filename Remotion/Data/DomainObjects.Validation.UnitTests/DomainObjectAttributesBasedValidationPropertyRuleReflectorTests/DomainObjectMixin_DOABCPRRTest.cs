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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

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
    private PropertyInfo _mixinBidirectionalDomainObjectCollectionRelationProperty;
    private PropertyInfo _interfaceBidirectionalDomainObjectCollectionRelationProperty;
    private PropertyInfo _mixinBidirectionalVirtualCollectionRelationProperty;
    private PropertyInfo _interfaceBidirectionalVirtualCollectionRelationProperty;
    private IAttributesBasedValidationPropertyRuleReflector _propertyWithoutAttributeReflector;
    private IAttributesBasedValidationPropertyRuleReflector _propertyWithNullableStringPropertyAttributeReflector;
    private IAttributesBasedValidationPropertyRuleReflector _propertyWithMandatoryStringPropertyAttributeReflector;
    private IAttributesBasedValidationPropertyRuleReflector _propertyWithMandatoryAttributeReflector;
    private IAttributesBasedValidationPropertyRuleReflector _intPropertyReflector;
    private IAttributesBasedValidationPropertyRuleReflector _bidirectionalRelationReflector;
    private IAttributesBasedValidationPropertyRuleReflector _bidirectionalDomainObjectCollectionRelationReflector;
    private IAttributesBasedValidationPropertyRuleReflector _bidirectionalVirtualCollectionRelationReflector;
    private Mock<IValidationMessageFactory> _validationMessageFactoryStub;
    private DomainModelConstraintProvider _domainModelConstraintProvider;

    [SetUp]
    public void SetUp ()
    {
      _mixinPropertyWithoutAttribute =
          typeof(MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty("PropertyWithoutAttribute");
      _interfacePropertyWithoutAttribute =
          typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty("PropertyWithoutAttribute");

      _mixinPropertyWithMandatoryAttribute =
          typeof(MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty("PropertyWithMandatoryAttribute");
      _interfacePropertyWithMandatoryAttribute =
          typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty("PropertyWithMandatoryAttribute");

      _mixinBidirectionalRelationProperty =
          typeof(MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty("BidirectionalPropertyWithMandatoryAttribute");
      _interfaceBidirectionalRelationProperty =
          typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty(
              "BidirectionalPropertyWithMandatoryAttribute");

      _mixinBidirectionalDomainObjectCollectionRelationProperty =
          typeof(MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty(
              "BidirectionalDomainObjectCollectionPropertyWithMandatoryAttribute");
      _interfaceBidirectionalDomainObjectCollectionRelationProperty =
          typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty(
              "BidirectionalDomainObjectCollectionPropertyWithMandatoryAttribute");

      _mixinBidirectionalVirtualCollectionRelationProperty =
          typeof(MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty(
              "BidirectionalVirtualCollectionPropertyWithMandatoryAttribute");
      _interfaceBidirectionalVirtualCollectionRelationProperty =
          typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty(
              "BidirectionalVirtualCollectionPropertyWithMandatoryAttribute");

      _mixinPropertyWithNullableStringPropertyAttribute =
          typeof(MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty("PropertyWithNullableStringPropertyAttribute");
      _interfacePropertyWithNullableStringPropertyAttribute =
          typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty(
              "PropertyWithNullableStringPropertyAttribute");

      _mixinPropertyWithMandatoryStringPropertyAttribute =
          typeof(MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty(
              "PropertyWithMandatoryStringPropertyAttribute");
      _interfacePropertyWithMandatoryStringPropertyAttribute =
          typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty(
              "PropertyWithMandatoryStringPropertyAttribute");

      _mixinIntProperty =
          typeof(MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty("IntProperty");
      _interfaceIntProperty =
          typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty("IntProperty");

      _domainModelConstraintProvider = new DomainModelConstraintProvider();
      _validationMessageFactoryStub = new Mock<IValidationMessageFactory>();

      _propertyWithoutAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _interfacePropertyWithoutAttribute,
          _mixinPropertyWithoutAttribute,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);
      _propertyWithMandatoryAttributeReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _interfacePropertyWithMandatoryAttribute,
          _mixinPropertyWithMandatoryAttribute,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);
      _propertyWithNullableStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector(
              _interfacePropertyWithNullableStringPropertyAttribute,
              _mixinPropertyWithNullableStringPropertyAttribute,
              _domainModelConstraintProvider,
              _validationMessageFactoryStub.Object);
      _propertyWithMandatoryStringPropertyAttributeReflector =
          new DomainObjectAttributesBasedValidationPropertyRuleReflector(
              _interfacePropertyWithMandatoryStringPropertyAttribute,
              _mixinPropertyWithMandatoryStringPropertyAttribute,
              _domainModelConstraintProvider,
              _validationMessageFactoryStub.Object);

      _intPropertyReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _interfaceIntProperty,
          _mixinIntProperty,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);

      _bidirectionalRelationReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _interfaceBidirectionalRelationProperty,
          _mixinBidirectionalRelationProperty,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);

      _bidirectionalDomainObjectCollectionRelationReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _interfaceBidirectionalDomainObjectCollectionRelationProperty,
          _mixinBidirectionalDomainObjectCollectionRelationProperty,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);

      _bidirectionalVirtualCollectionRelationReflector = new DomainObjectAttributesBasedValidationPropertyRuleReflector(
          _interfaceBidirectionalVirtualCollectionRelationProperty,
          _mixinBidirectionalVirtualCollectionRelationProperty,
          _domainModelConstraintProvider,
          _validationMessageFactoryStub.Object);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(
          ((PropertyInfoAdapter)_propertyWithoutAttributeReflector.ValidatedProperty).PropertyInfo,
          Is.EqualTo(_interfacePropertyWithoutAttribute));
      Assert.That(
          ((PropertyInfoAdapter)_propertyWithMandatoryAttributeReflector.ValidatedProperty).PropertyInfo,
          Is.EqualTo(_interfacePropertyWithMandatoryAttribute));
      Assert.That(
          ((PropertyInfoAdapter)_propertyWithNullableStringPropertyAttributeReflector.ValidatedProperty).PropertyInfo,
          Is.EqualTo(_interfacePropertyWithNullableStringPropertyAttribute));
      Assert.That(
          ((PropertyInfoAdapter)_propertyWithMandatoryStringPropertyAttributeReflector.ValidatedProperty).PropertyInfo,
          Is.EqualTo(_interfacePropertyWithMandatoryStringPropertyAttribute));
    }

    [Test]
    public void GetPropertyAccessExpression_NonRelationStringProperty ()
    {
      var propertyAccessor = _propertyWithMandatoryStringPropertyAttributeReflector
          .GetValidatedPropertyFunc(typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject();
        var propertyValue = "test";
        ((IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)obj).PropertyWithMandatoryStringPropertyAttribute = propertyValue;
        var result = propertyAccessor(obj);
        Assert.That(result, Is.SameAs(propertyValue));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_NonRelationIntProperty ()
    {
      var propertyAccessor = _intPropertyReflector
          .GetValidatedPropertyFunc(typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject();
        var propertyValue = 10;
        ((IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)obj).IntProperty = propertyValue;
        var result = propertyAccessor(obj);
        Assert.That(result, Is.EqualTo(propertyValue));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_UnidirectionalRelation ()
    {
      var propertyAccessor = _propertyWithMandatoryAttributeReflector
          .GetValidatedPropertyFunc(typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject();
        var propertyValue = TestDomainObject.NewObject();
        ((IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)obj).PropertyWithMandatoryAttribute = propertyValue;
        var result = propertyAccessor(obj);
        Assert.That(result, Is.SameAs(propertyValue));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_BidirectionalRelation ()
    {
      var propertyAccessor = _bidirectionalRelationReflector
          .GetValidatedPropertyFunc(typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject();
        var propertyValue = TestDomainObject.NewObject();
        ((IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)obj).BidirectionalPropertyWithMandatoryAttribute = propertyValue;
        var result = propertyAccessor(obj);
        Assert.That(result, Is.SameAs(propertyValue));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_BidirectionalDomainObjectCollectionRelation ()
    {
      var propertyAccessor = _bidirectionalDomainObjectCollectionRelationReflector
          .GetValidatedPropertyFunc(typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject();
        var propertyValue = TestDomainObject.NewObject();
        ((IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)obj).BidirectionalDomainObjectCollectionPropertyWithMandatoryAttribute.Add(propertyValue);
        var result = propertyAccessor(obj);
        Assert.That(result, Is.EqualTo(new[] { propertyValue }));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_BidirectionalVirtualCollectionRelation ()
    {
      var propertyAccessor = _bidirectionalVirtualCollectionRelationReflector
          .GetValidatedPropertyFunc(typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject();
        var propertyValue = TestDomainObject.NewObject();
        propertyValue.OppositeObjectForVirtualCollectionProperty = obj;
        var result = propertyAccessor(obj);
        Assert.That(result, Is.EqualTo(new[] { propertyValue }));
      }
    }

    [Test]
    public void GetPropertyAccessExpression_BidirectionalRelation_NotLoaded ()
    {
      var propertyAccessor = _bidirectionalRelationReflector
         .GetValidatedPropertyFunc(typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject();
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          var result = propertyAccessor(obj);

          Assert.That(result, Is.Not.Null);
          Assert.That(result.GetType().Name, Is.EqualTo("FakeDomainObject"));
        }
      }
    }

    [Test]
    public void GetPropertyAccessExpression_BidirectionalDomainObjectCollectionRelation_NotLoaded ()
    {
      var propertyAccessor = _bidirectionalDomainObjectCollectionRelationReflector
         .GetValidatedPropertyFunc(typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject();
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          var result = propertyAccessor(obj) as IEnumerable<object>;

          Assert.That(result, Is.Not.Null);
          Assert.That(result.Select(r => r.GetType().Name), Is.EqualTo(new[] { "FakeDomainObject" }));
        }
      }
    }

    [Test]
    public void GetPropertyAccessExpression_BidirectionalVirtualCollectionRelation_NotLoaded ()
    {
      var propertyAccessor = _bidirectionalVirtualCollectionRelationReflector
          .GetValidatedPropertyFunc(typeof(IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface));

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var obj = (IMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface)MixinTarget_AnnotatedPropertiesPartOfInterface.NewObject();
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          var result = propertyAccessor(obj) as IEnumerable<object>;

          Assert.That(result, Is.Not.Null);
          Assert.That(result.Select(r => r.GetType().Name), Is.EqualTo(new[] { "FakeDomainObject" }));
        }
      }
    }

    [Test]
    public void NoAttributes ()
    {
      Assert.That(_propertyWithoutAttributeReflector.GetRemovablePropertyValidators().Any(), Is.False);
      Assert.That(_propertyWithoutAttributeReflector.GetNonRemovablePropertyValidators().Any(), Is.False);
      Assert.That(_propertyWithoutAttributeReflector.GetRemovingValidatorRegistrations().Any(), Is.False);
      Assert.That(_propertyWithoutAttributeReflector.GetMetaValidationRules().Any(), Is.False);
    }

    [Test]
    public void GetNonRemovablePropertyValidators_MandatoryAttribute ()
    {
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotNullValidator>(),
                  PropertyInfoAdapter.Create(_mixinPropertyWithMandatoryAttribute)))
          .Returns(validationMessageStub.Object);

      var result = _propertyWithMandatoryAttributeReflector.GetNonRemovablePropertyValidators().ToArray();

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(NotNullValidator)));
      Assert.That(((NotNullValidator)result[0]).ValidationMessage.ToString, Is.EqualTo("Stub Message"));
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetRemovablePropertyValidators_MandatoryAttributeAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetRemovablePropertyValidators_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetRemovablePropertyValidators().ToArray();

      Assert.That(result, Is.Empty);
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetRemovablePropertyValidators_NullableStringPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetRemovablePropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var lengthValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<MaximumLengthValidator>(),
                  PropertyInfoAdapter.Create(_mixinPropertyWithMandatoryStringPropertyAttribute)))
          .Returns(lengthValidationMessageStub.Object);

      var notEmptyValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotEmptyOrWhitespaceValidator>(),
                  PropertyInfoAdapter.Create(_mixinPropertyWithMandatoryStringPropertyAttribute)))
          .Returns(notEmptyValidationMessageStub.Object);

      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetRemovablePropertyValidators().ToArray();

      notEmptyValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for NotEmpty");
      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(NotEmptyOrWhitespaceValidator)));
      Assert.That(((NotEmptyOrWhitespaceValidator)result[0]).ValidationMessage.ToString, Is.EqualTo("Stub Message for NotEmpty"));
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetRemovablePropertyValidators_MandatoryStringPropertyAndValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    public void GetNonRemovablePropertyValidators_NullableStringPropertyAttribute ()
    {
      var lengthValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<MaximumLengthValidator>(),
                  PropertyInfoAdapter.Create(_mixinPropertyWithNullableStringPropertyAttribute)))
          .Returns(lengthValidationMessageStub.Object);

      var result = _propertyWithNullableStringPropertyAttributeReflector.GetNonRemovablePropertyValidators().ToArray();

      lengthValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for length");
      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(MaximumLengthValidator)));
      Assert.That(((MaximumLengthValidator)result[0]).ValidationMessage.ToString, Is.EqualTo("Stub Message for length"));
    }

    [Test]
    public void GetNonRemovablePropertyValidators_MandatoryStringPropertyAttribute ()
    {
      var notNullValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<NotNullValidator>(),
                  PropertyInfoAdapter.Create(_mixinPropertyWithMandatoryStringPropertyAttribute)))
          .Returns(notNullValidationMessageStub.Object);

      var lengthValidationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsAny<MaximumLengthValidator>(),
                  PropertyInfoAdapter.Create(_mixinPropertyWithMandatoryStringPropertyAttribute)))
          .Returns(lengthValidationMessageStub.Object);

      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetNonRemovablePropertyValidators().ToArray();

      lengthValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for length");
      Assert.That(result.Count(), Is.EqualTo(2));
      Assert.That(result[0], Is.TypeOf(typeof(MaximumLengthValidator)));
      Assert.That(((MaximumLengthValidator)result[0]).ValidationMessage.ToString, Is.EqualTo("Stub Message for length"));

      notNullValidationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message for not null");
      Assert.That(result[1], Is.TypeOf(typeof(NotNullValidator)));
      Assert.That(((NotNullValidator)result[1]).ValidationMessage.ToString, Is.EqualTo("Stub Message for not null"));
    }

    [Test]
    public void GetRemovingValidatorRegistrations ()
    {
      var result = _propertyWithMandatoryStringPropertyAttributeReflector.GetRemovingValidatorRegistrations().ToArray();

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetMetaValidationRules_MandatoryAttribute ()
    {
      var result = _propertyWithMandatoryAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetMetaValidationRules_NullableStringPropertyAttribute ()
    {
      var result = _propertyWithNullableStringPropertyAttributeReflector.GetMetaValidationRules().ToArray();

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Initialize_WithMixinPropertyAsInterfaceProperty_ThrowsArgumentException ()
    {
      Assert.That(
          () => new DomainObjectAttributesBasedValidationPropertyRuleReflector(
              _mixinPropertyWithMandatoryAttribute,
              _mixinPropertyWithMandatoryAttribute,
              _domainModelConstraintProvider,
              _validationMessageFactoryStub.Object),
          Throws.ArgumentException.And.ArgumentExceptionMessageEqualTo(
              "The property 'PropertyWithMandatoryAttribute' was declared on type "
              + "'Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain.MixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface' "
              + "but only interface declarations are supported when using mixin properties.", "interfaceProperty"));
    }

    [Test]
    public void Initialize_WithDerivedImplementationProperty_ThrowsArgumentException ()
    {
      var overriddenProperty = typeof(DerivedMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface).GetProperty("PropertyWithNullableStringPropertyAttribute");

      Assert.That(
          () => new DomainObjectAttributesBasedValidationPropertyRuleReflector(
              interfaceProperty: _interfacePropertyWithNullableStringPropertyAttribute,
              implementationProperty: overriddenProperty,
              domainModelConstraintProvider: _domainModelConstraintProvider,
              validationMessageFactory: _validationMessageFactoryStub.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The property 'PropertyWithNullableStringPropertyAttribute' was used from the overridden declaration on type "
              + "'Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain.DerivedMixinTypeWithDomainObjectAttributes_AnnotatedPropertiesPartOfInterface' "
              + "but only original declarations are supported.", "implementationProperty"));
    }
  }
}
