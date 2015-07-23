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
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Providers;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.ValidationRules;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Providers
{
  [TestFixture]
  public class AttributeBasedValidationCollectorProviderTest
  {
    private IAttributesBasedValidationPropertyRuleReflector _validationPropertyRuleReflectorMock1;
    private IPropertyValidator _propertyValidatorStub1;
    private IPropertyValidator _propertyValidatorStub2;
    private IPropertyValidator _propertyValidatorStub3;
    private ValidatorRegistration _validatorRegistration1;
    private ValidatorRegistration _validatorRegistration2;
    private ValidatorRegistration _validatorRegistration3;
    private ValidatorRegistration _validatorRegistration4;
    private IPropertyValidator _propertyValidatorStub4;
    private IPropertyValidator _propertyValidatorStub5;
    private IPropertyValidator _propertyValidatorStub6;
    private MaxLengthMetaValidationRule _metaValidationRule1;
    private MaxValidatorCountRule _metaValidationRule2;
    private MaxLengthMetaValidationRule _metaValidationRule3;
    private IAttributesBasedValidationPropertyRuleReflector _validationPropertyRuleReflectorMock2;

    [SetUp]
    public void SetUp ()
    {
      _propertyValidatorStub1 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub2 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub3 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub4 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub5 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub6 = MockRepository.GenerateStub<IPropertyValidator>();

      _metaValidationRule1 = new MaxLengthMetaValidationRule();
      _metaValidationRule2 = new MaxValidatorCountRule();
      _metaValidationRule3 = new MaxLengthMetaValidationRule();

      _validatorRegistration1 = new ValidatorRegistration (typeof (NotNullValidator), null);
      _validatorRegistration2 = new ValidatorRegistration (typeof (NotEmptyValidator), null);
      _validatorRegistration3 = new ValidatorRegistration (typeof (NotNullValidator), null);
      _validatorRegistration4 = new ValidatorRegistration (typeof (NotEmptyValidator), null);

      _validationPropertyRuleReflectorMock1 = MockRepository.GenerateStrictMock<IAttributesBasedValidationPropertyRuleReflector>();
      _validationPropertyRuleReflectorMock2 = MockRepository.GenerateStrictMock<IAttributesBasedValidationPropertyRuleReflector> ();
    }

    [Test]
    public void GetComponentValidationCollectors ()
    {
      var dictionary = new Dictionary<Type, IAttributesBasedValidationPropertyRuleReflector>();
      dictionary.Add (typeof (Employee), _validationPropertyRuleReflectorMock1);
      dictionary.Add (typeof (SpecialCustomer1), _validationPropertyRuleReflectorMock2);

      var collectorProvider =
          new TestableAttributeBasedValidationCollectorProviderBase (
              dictionary,
              _propertyValidatorStub1,
              _propertyValidatorStub2,
              _propertyValidatorStub3,
              _propertyValidatorStub4,
              _propertyValidatorStub5,
              _propertyValidatorStub6,
              _validatorRegistration1,
              _validatorRegistration2,
              _validatorRegistration3,
              _validatorRegistration4,
              _metaValidationRule1,
              _metaValidationRule2,
              _metaValidationRule3);

      var result = collectorProvider.GetValidationCollectors (new[] { typeof (Employee), typeof (SpecialCustomer1) }).SelectMany (g => g).ToArray();

      _validationPropertyRuleReflectorMock1.VerifyAllExpectations ();
      _validationPropertyRuleReflectorMock2.VerifyAllExpectations ();
      Assert.That (result.Count(), Is.EqualTo (2));
      Assert.That (result[0].Collector.GetType ().Name, Is.EqualTo ("AttributeBasedComponentValidationCollector"));
      Assert.That (result[0].ProviderType, Is.EqualTo (typeof (TestableAttributeBasedValidationCollectorProviderBase)));

      var addingPropertyRuleValidators = result[0].Collector.AddedPropertyRules.ToArray().SelectMany (pr => pr.Validators);
      Assert.That (
          addingPropertyRuleValidators,
          Is.EquivalentTo (new[] { _propertyValidatorStub1, _propertyValidatorStub2, _propertyValidatorStub3 }));
      Assert.That (result[0].Collector.AddedPropertyMetaValidationRules.ToArray().SelectMany (pr => pr.MetaValidationRules).Any(), Is.False);

      var removedPropertyRuleRegistrations =
          result[0].Collector.RemovedPropertyRules.ToArray().SelectMany (pr => pr.Validators.Select (v => v.ValidatorType));
      Assert.That (
          removedPropertyRuleRegistrations,
          Is.EquivalentTo (new[] { _validatorRegistration1.ValidatorType, _validatorRegistration2.ValidatorType }));

      Assert.That (
          result[1].Collector.GetType().Name,
          Is.EqualTo ("AttributeBasedComponentValidationCollector"));
      Assert.That (result[1].ProviderType, Is.EqualTo (typeof (TestableAttributeBasedValidationCollectorProviderBase)));

      Assert.That (result[1].Collector.AddedPropertyRules.Count(), Is.EqualTo (4));
      addingPropertyRuleValidators = result[1].Collector.AddedPropertyRules.ToArray().SelectMany (pr => pr.Validators);
      Assert.That (
          addingPropertyRuleValidators,
          Is.EquivalentTo (new[] { _propertyValidatorStub4, _propertyValidatorStub5, _propertyValidatorStub6 }));
      var addingPropertyRuleMetaValidationRules =
          result[1].Collector.AddedPropertyMetaValidationRules.ToArray().SelectMany (pr => pr.MetaValidationRules);
      Assert.That (
          addingPropertyRuleMetaValidationRules,
          Is.EquivalentTo (new IMetaValidationRule[] { _metaValidationRule1, _metaValidationRule3, _metaValidationRule2 }));

      removedPropertyRuleRegistrations =
          result[1].Collector.RemovedPropertyRules.ToArray().SelectMany (pr => pr.Validators.Select (v => v.ValidatorType));
      Assert.That (
          removedPropertyRuleRegistrations,
          Is.EquivalentTo (new[] { _validatorRegistration3.ValidatorType, _validatorRegistration4.ValidatorType }));
    }
  }
}