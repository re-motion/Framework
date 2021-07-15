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
using Moq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.ValidationRules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Providers
{
  [TestFixture]
  public class AttributeBasedValidationRuleCollectorProviderBaseTest
  {
    private Mock<IAttributesBasedValidationPropertyRuleReflector> _validationPropertyRuleReflectorMock1;
    private Mock<IPropertyValidator> _propertyValidatorStub1;
    private Mock<IPropertyValidator> _propertyValidatorStub2;
    private Mock<IPropertyValidator> _propertyValidatorStub3;
    private RemovingValidatorRegistration _removingValidatorRegistration1;
    private RemovingValidatorRegistration _removingValidatorRegistration2;
    private RemovingValidatorRegistration _removingValidatorRegistration3;
    private RemovingValidatorRegistration _removingValidatorRegistration4;
    private Mock<IPropertyValidator> _propertyValidatorStub4;
    private Mock<IPropertyValidator> _propertyValidatorStub5;
    private Mock<IPropertyValidator> _propertyValidatorStub6;
    private MaxLengthPropertyMetaValidationRule _propertyMetaValidationRule1;
    private MaxValidatorCountRule _metaValidationRule2;
    private MaxLengthPropertyMetaValidationRule _propertyMetaValidationRule3;
    private Mock<IAttributesBasedValidationPropertyRuleReflector> _validationPropertyRuleReflectorMock2;

    [SetUp]
    public void SetUp ()
    {
      _propertyValidatorStub1 = new Mock<IPropertyValidator>();
      _propertyValidatorStub2 = new Mock<IPropertyValidator>();
      _propertyValidatorStub3 = new Mock<IPropertyValidator>();
      _propertyValidatorStub4 = new Mock<IPropertyValidator>();
      _propertyValidatorStub5 = new Mock<IPropertyValidator>();
      _propertyValidatorStub6 = new Mock<IPropertyValidator>();

      _propertyMetaValidationRule1 = new MaxLengthPropertyMetaValidationRule();
      _metaValidationRule2 = new MaxValidatorCountRule();
      _propertyMetaValidationRule3 = new MaxLengthPropertyMetaValidationRule();

      _removingValidatorRegistration1 = new RemovingValidatorRegistration (typeof (NotNullValidator), null);
      _removingValidatorRegistration2 = new RemovingValidatorRegistration (typeof (NotEmptyValidator), null);
      _removingValidatorRegistration3 = new RemovingValidatorRegistration (typeof (NotNullValidator), null);
      _removingValidatorRegistration4 = new RemovingValidatorRegistration (typeof (NotEmptyValidator), null);

      _validationPropertyRuleReflectorMock1 = new Mock<IAttributesBasedValidationPropertyRuleReflector> (MockBehavior.Strict);
      _validationPropertyRuleReflectorMock2 = new Mock<IAttributesBasedValidationPropertyRuleReflector> (MockBehavior.Strict);
    }

    [Test]
    public void GetValidationRuleCollectors ()
    {
      var dictionary = new Dictionary<Type, Mock<IAttributesBasedValidationPropertyRuleReflector>>();
      dictionary.Add (typeof (Employee), _validationPropertyRuleReflectorMock1);
      dictionary.Add (typeof (SpecialCustomer1), _validationPropertyRuleReflectorMock2);

      var collectorProvider =
          new TestableAttributeBasedValidationRuleCollectorProviderBase (
              dictionary,
              _propertyValidatorStub1.Object,
              _propertyValidatorStub2.Object,
              _propertyValidatorStub3.Object,
              _propertyValidatorStub4.Object,
              _propertyValidatorStub5.Object,
              _propertyValidatorStub6.Object,
              _removingValidatorRegistration1,
              _removingValidatorRegistration2,
              _removingValidatorRegistration3,
              _removingValidatorRegistration4,
              _propertyMetaValidationRule1,
              _metaValidationRule2,
              _propertyMetaValidationRule3);

      var result = collectorProvider.GetValidationRuleCollectors (new[] { typeof (Employee), typeof (SpecialCustomer1) }).SelectMany (g => g).ToArray();

      _validationPropertyRuleReflectorMock1.Verify();
      _validationPropertyRuleReflectorMock2.Verify();
      Assert.That (result.Count(), Is.EqualTo (2));
      Assert.That (result[0].Collector.GetType ().Name, Is.EqualTo ("AttributeBasedValidationRuleCollector"));
      Assert.That (result[0].ProviderType, Is.EqualTo (typeof (TestableAttributeBasedValidationRuleCollectorProviderBase)));

      var addingPropertyRuleValidators = result[0].Collector.AddedPropertyRules.ToArray().SelectMany (pr => pr.Validators);
      Assert.That (
          addingPropertyRuleValidators,
          Is.EquivalentTo (new[] { _propertyValidatorStub1.Object, _propertyValidatorStub2.Object, _propertyValidatorStub3.Object }));
      Assert.That (result[0].Collector.PropertyMetaValidationRules.ToArray().SelectMany (pr => pr.MetaValidationRules).Any(), Is.False);

      var removedPropertyRuleRegistrations =
          result[0].Collector.RemovedPropertyRules.ToArray().SelectMany (pr => pr.Validators.Select (v => v.ValidatorType));
      Assert.That (
          removedPropertyRuleRegistrations,
          Is.EquivalentTo (new[] { _removingValidatorRegistration1.ValidatorType, _removingValidatorRegistration2.ValidatorType }));

      Assert.That (
          result[1].Collector.GetType().Name,
          Is.EqualTo ("AttributeBasedValidationRuleCollector"));
      Assert.That (result[1].ProviderType, Is.EqualTo (typeof (TestableAttributeBasedValidationRuleCollectorProviderBase)));

      Assert.That (result[1].Collector.AddedPropertyRules.Count(), Is.EqualTo (4));
      addingPropertyRuleValidators = result[1].Collector.AddedPropertyRules.ToArray().SelectMany (pr => pr.Validators);
      Assert.That (
          addingPropertyRuleValidators,
          Is.EquivalentTo (new[] { _propertyValidatorStub4.Object, _propertyValidatorStub5.Object, _propertyValidatorStub6.Object }));
      var addingPropertyRuleMetaValidationRules =
          result[1].Collector.PropertyMetaValidationRules.ToArray().SelectMany (pr => pr.MetaValidationRules);
      Assert.That (
          addingPropertyRuleMetaValidationRules,
          Is.EquivalentTo (new IPropertyMetaValidationRule[] { _propertyMetaValidationRule1, _propertyMetaValidationRule3, _metaValidationRule2 }));

      removedPropertyRuleRegistrations =
          result[1].Collector.RemovedPropertyRules.ToArray().SelectMany (pr => pr.Validators.Select (v => v.ValidatorType));
      Assert.That (
          removedPropertyRuleRegistrations,
          Is.EquivalentTo (new[] { _removingValidatorRegistration3.ValidatorType, _removingValidatorRegistration4.ValidatorType }));
    }
  }
}