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
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class ValidationAttributesBasedPropertyRuleReflectorTest
  {
    private PropertyInfo _customerLastNameProperty;
    private PropertyInfo _specialCustomerLastNameProperty;
    private ValidationAttributesBasedPropertyRuleReflector _customerPropertyReflector;
    private ValidationAttributesBasedPropertyRuleReflector _specialCustomerPropertyReflector;
    private PropertyInfo _addressPostalCodeProperty;
    private ValidationAttributesBasedPropertyRuleReflector _addressPropertyReflector;
    private Mock<IValidationMessageFactory> _validationMessageFactory;

    [SetUp]
    public void SetUp ()
    {
      _validationMessageFactory = new Mock<IValidationMessageFactory>();

      _customerLastNameProperty = typeof(Customer).GetProperty("UserName");
      _specialCustomerLastNameProperty = typeof(SpecialCustomer1).GetProperty("UserName");
      _addressPostalCodeProperty = typeof(Address).GetProperty("PostalCode");

      _customerPropertyReflector = new ValidationAttributesBasedPropertyRuleReflector(
          _customerLastNameProperty,
          _validationMessageFactory.Object);

      _specialCustomerPropertyReflector = new ValidationAttributesBasedPropertyRuleReflector(
          _specialCustomerLastNameProperty,
          _validationMessageFactory.Object);

      _addressPropertyReflector = new ValidationAttributesBasedPropertyRuleReflector(
          _addressPostalCodeProperty,
          _validationMessageFactory.Object);
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetValidatedPropertyFunc_ReturnsFuncForPropertyAccess ()
    {
    }

    [Test]
    public void GetRemovablePropertyValidators_Customer ()
    {
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactory
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsNotNull<IPropertyValidator>(),
                  PropertyInfoAdapter.Create(_customerLastNameProperty)))
          .Returns(validationMessageStub.Object);

      var addingPropertyValidators = _customerPropertyReflector.GetRemovablePropertyValidators().ToArray();

      Assert.That(addingPropertyValidators.Length, Is.EqualTo(2));
      Assert.That(
          addingPropertyValidators.Select(v => v.GetType()),
          Is.EquivalentTo(new[] { typeof(MaximumLengthValidator), typeof(NotNullValidator) }));

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(addingPropertyValidators.OfType<MaximumLengthValidator>().Single().ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
      Assert.That(addingPropertyValidators.OfType<NotNullValidator>().Single().ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetRemovablePropertyValidators_SpecialCustomer ()
    {
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactory
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsNotNull<IPropertyValidator>(),
                  PropertyInfoAdapter.Create(_specialCustomerLastNameProperty)))
          .Returns(validationMessageStub.Object);

      var addingPropertyValidators = _specialCustomerPropertyReflector.GetRemovablePropertyValidators().ToArray();

      Assert.That(addingPropertyValidators.Length, Is.EqualTo(2));
      Assert.That(
          addingPropertyValidators.Select(v => v.GetType()),
          Is.EquivalentTo(new[] { typeof(MaximumLengthValidator), typeof(NotNullValidator) }));

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(addingPropertyValidators.OfType<MaximumLengthValidator>().Single().ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
      Assert.That(addingPropertyValidators.OfType<NotNullValidator>().Single().ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetNonRemovablePropertyValidators_Customer ()
    {
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactory
          .Setup(
              _ => _.CreateValidationMessageForPropertyValidator(
                  It.IsNotNull<IPropertyValidator>(),
                  PropertyInfoAdapter.Create(_customerLastNameProperty)))
          .Returns(validationMessageStub.Object);

      var hardConstraintPropertyValidators = _customerPropertyReflector.GetNonRemovablePropertyValidators().ToArray();

      Assert.That(hardConstraintPropertyValidators.Length, Is.EqualTo(1));
      Assert.That(hardConstraintPropertyValidators[0].GetType(), Is.EqualTo(typeof(NotEqualValidator)));

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(((NotEqualValidator)hardConstraintPropertyValidators[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetNonRemovablePropertyValidators_SpecialCustomer ()
    {
      var hardConstraintPropertyValidators = _specialCustomerPropertyReflector.GetNonRemovablePropertyValidators().ToArray();

      Assert.That(hardConstraintPropertyValidators.Length, Is.EqualTo(0));
    }

    [Test]
    public void GetRemovingValidatorRegistrations_Customer ()
    {
      var removingValidatorRegistrations = _customerPropertyReflector.GetRemovingValidatorRegistrations().ToArray();

      Assert.That(removingValidatorRegistrations.Length, Is.EqualTo(0));
    }

    [Test]
    public void GetRemovingValidatorRegistrations_SpecialCustomer ()
    {
      var removingValidatorRegistrations = _specialCustomerPropertyReflector.GetRemovingValidatorRegistrations().ToArray();

      Assert.That(removingValidatorRegistrations.Length, Is.EqualTo(1));
      Assert.That(removingValidatorRegistrations[0].ValidatorType, Is.EqualTo(typeof(MaximumLengthValidator)));
    }

    [Test]
    public void GetMetaValidationRules_NoMetaValidationRule ()
    {
      Assert.That(_customerPropertyReflector.GetMetaValidationRules().Any(), Is.False);
    }

    [Test]
    public void GetMetaValidationRules_WithMetaValidationRule ()
    {
      var result = _addressPropertyReflector.GetMetaValidationRules().ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result.Select(r => r.GetType()), Is.EquivalentTo(new[] { typeof(AnyRuleAppliedPropertyMetaValidationRule) }));
    }
  }
}
