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
    private MaxValidatorCountRule _propertyMetaValidationRule2;
    private MaxLengthPropertyMetaValidationRule _propertyMetaValidationRule3;
    private Mock<IAttributesBasedValidationPropertyRuleReflector> _validationPropertyRuleReflectorMockForEmployeePosition;
    private Mock<IAttributesBasedValidationPropertyRuleReflector> _validationPropertyRuleReflectorMockForEmployeeNotes;
    private Mock<IAttributesBasedValidationPropertyRuleReflector> _validationPropertyRuleReflectorMockForSpecialCustomer1LastName;
    private Mock<IAttributesBasedValidationPropertyRuleReflector> _validationPropertyRuleReflectorMockForSpecialCustomer1UserName;

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
      _propertyMetaValidationRule2 = new MaxValidatorCountRule();
      _propertyMetaValidationRule3 = new MaxLengthPropertyMetaValidationRule();

      _removingValidatorRegistration1 = new RemovingValidatorRegistration(typeof(NotNullValidator), null);
      _removingValidatorRegistration2 = new RemovingValidatorRegistration(typeof(NotEmptyOrWhitespaceValidator), null);
      _removingValidatorRegistration3 = new RemovingValidatorRegistration(typeof(NotNullValidator), null);
      _removingValidatorRegistration4 = new RemovingValidatorRegistration(typeof(NotEmptyOrWhitespaceValidator), null);

      _validationPropertyRuleReflectorMockForEmployeePosition = new Mock<IAttributesBasedValidationPropertyRuleReflector>(MockBehavior.Strict);
      _validationPropertyRuleReflectorMockForEmployeeNotes= new Mock<IAttributesBasedValidationPropertyRuleReflector>(MockBehavior.Strict);
      _validationPropertyRuleReflectorMockForSpecialCustomer1LastName = new Mock<IAttributesBasedValidationPropertyRuleReflector>(MockBehavior.Strict);
      _validationPropertyRuleReflectorMockForSpecialCustomer1UserName = new Mock<IAttributesBasedValidationPropertyRuleReflector>(MockBehavior.Strict);
    }

    [Test]
    public void GetValidationRuleCollectors ()
    {
      var dictionary = new Dictionary<(Type DeclaringType, string PropertyName), Mock<IAttributesBasedValidationPropertyRuleReflector>>();

      dictionary.Add((typeof(Employee), nameof(Employee.Position)), _validationPropertyRuleReflectorMockForEmployeePosition);
      TestableAttributeBasedValidationRuleCollectorProviderBase.SetupPropertyRuleReflector(
          _validationPropertyRuleReflectorMockForEmployeePosition,
          typeof(Employee),
          nameof(Employee.Position),
          e => ((Employee)e).Position,
          removablePropertyValidators: new[] { _propertyValidatorStub1.Object },
          nonRemovablePropertyValidators: new[] { _propertyValidatorStub2.Object });

      dictionary.Add((typeof(Employee), nameof(Employee.Notes)), _validationPropertyRuleReflectorMockForEmployeeNotes);
      TestableAttributeBasedValidationRuleCollectorProviderBase.SetupPropertyRuleReflector(
          _validationPropertyRuleReflectorMockForEmployeeNotes,
          typeof(Employee),
          nameof(Employee.Notes),
          e => ((Employee)e).Notes,
          removablePropertyValidators: new[] { _propertyValidatorStub3.Object },
          removingValidatorRegistrations: new[] { _removingValidatorRegistration1, _removingValidatorRegistration2 });

      dictionary.Add((typeof(SpecialCustomer1), nameof(SpecialCustomer1.LastName)), _validationPropertyRuleReflectorMockForSpecialCustomer1LastName);
      TestableAttributeBasedValidationRuleCollectorProviderBase.SetupPropertyRuleReflector(
          _validationPropertyRuleReflectorMockForSpecialCustomer1LastName,
          typeof(SpecialCustomer1),
          nameof(SpecialCustomer1.LastName),
          c => ((SpecialCustomer1)c).LastName,
          removablePropertyValidators: new[] { _propertyValidatorStub4.Object, _propertyValidatorStub5.Object },
          propertyMetaValidationRules: new[] { _propertyMetaValidationRule1, _propertyMetaValidationRule3 });

      dictionary.Add((typeof(SpecialCustomer1), nameof(SpecialCustomer1.UserName)), _validationPropertyRuleReflectorMockForSpecialCustomer1UserName);
      TestableAttributeBasedValidationRuleCollectorProviderBase.SetupPropertyRuleReflector(
          _validationPropertyRuleReflectorMockForSpecialCustomer1UserName,
          typeof(SpecialCustomer1),
          nameof(SpecialCustomer1.UserName),
          c => ((SpecialCustomer1)c).UserName,
          removablePropertyValidators: new[] { _propertyValidatorStub6.Object },
          removingValidatorRegistrations: new[] { _removingValidatorRegistration3, _removingValidatorRegistration4 },
          propertyMetaValidationRules: new[] { _propertyMetaValidationRule2 });

      var collectorProvider = new TestableAttributeBasedValidationRuleCollectorProviderBase(dictionary);

      var result = collectorProvider.GetValidationRuleCollectors(new[] { typeof(Employee), typeof(SpecialCustomer1) }).SelectMany(g => g).ToArray();

      _validationPropertyRuleReflectorMockForEmployeePosition.Verify();
      _validationPropertyRuleReflectorMockForEmployeeNotes.Verify();
      _validationPropertyRuleReflectorMockForSpecialCustomer1LastName.Verify();
      _validationPropertyRuleReflectorMockForSpecialCustomer1UserName.Verify();

      Assert.That(result.Count(), Is.EqualTo(2));
      Assert.That(result[0].Collector.GetType().Name, Is.EqualTo("AttributeBasedValidationRuleCollector"));
      Assert.That(result[0].ProviderType, Is.EqualTo(typeof(TestableAttributeBasedValidationRuleCollectorProviderBase)));

      var addingPropertyRuleValidators = result[0].Collector.AddedPropertyRules.ToArray().SelectMany(pr => pr.Validators);
      Assert.That(
          addingPropertyRuleValidators,
          Is.EquivalentTo(new[] { _propertyValidatorStub1.Object, _propertyValidatorStub2.Object, _propertyValidatorStub3.Object }));
      Assert.That(result[0].Collector.PropertyMetaValidationRules.ToArray().SelectMany(pr => pr.MetaValidationRules).Any(), Is.False);

      var removedPropertyRuleRegistrations =
          result[0].Collector.RemovedPropertyRules.ToArray().SelectMany(pr => pr.Validators.Select(v => v.ValidatorType));
      Assert.That(
          removedPropertyRuleRegistrations,
          Is.EquivalentTo(new[] { _removingValidatorRegistration1.ValidatorType, _removingValidatorRegistration2.ValidatorType }));

      Assert.That(
          result[1].Collector.GetType().Name,
          Is.EqualTo("AttributeBasedValidationRuleCollector"));
      Assert.That(result[1].ProviderType, Is.EqualTo(typeof(TestableAttributeBasedValidationRuleCollectorProviderBase)));

      Assert.That(result[1].Collector.AddedPropertyRules.Count(), Is.EqualTo(4));
      addingPropertyRuleValidators = result[1].Collector.AddedPropertyRules.ToArray().SelectMany(pr => pr.Validators);
      Assert.That(
          addingPropertyRuleValidators,
          Is.EquivalentTo(new[] { _propertyValidatorStub4.Object, _propertyValidatorStub5.Object, _propertyValidatorStub6.Object }));
      var addingPropertyRuleMetaValidationRules =
          result[1].Collector.PropertyMetaValidationRules.ToArray().SelectMany(pr => pr.MetaValidationRules);
      Assert.That(
          addingPropertyRuleMetaValidationRules,
          Is.EquivalentTo(new IPropertyMetaValidationRule[] { _propertyMetaValidationRule1, _propertyMetaValidationRule3, _propertyMetaValidationRule2 }));

      removedPropertyRuleRegistrations =
          result[1].Collector.RemovedPropertyRules.ToArray().SelectMany(pr => pr.Validators.Select(v => v.ValidatorType));
      Assert.That(
          removedPropertyRuleRegistrations,
          Is.EquivalentTo(new[] { _removingValidatorRegistration3.ValidatorType, _removingValidatorRegistration4.ValidatorType }));
    }
  }
}
