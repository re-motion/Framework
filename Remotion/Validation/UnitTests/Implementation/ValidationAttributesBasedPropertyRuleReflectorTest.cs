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
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;
using Rhino.Mocks;

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
    private IValidationMessageFactory _validationMessageFactory;

    [SetUp]
    public void SetUp ()
    {
      _validationMessageFactory = MockRepository.GenerateStub<IValidationMessageFactory>();

      _customerLastNameProperty = typeof (Customer).GetProperty ("UserName");
      _specialCustomerLastNameProperty = typeof (SpecialCustomer1).GetProperty ("UserName");
      _addressPostalCodeProperty = typeof (Address).GetProperty ("PostalCode");

      _customerPropertyReflector = new ValidationAttributesBasedPropertyRuleReflector (
          _customerLastNameProperty,
          _validationMessageFactory);

      _specialCustomerPropertyReflector = new ValidationAttributesBasedPropertyRuleReflector (
          _specialCustomerLastNameProperty,
          _validationMessageFactory);

      _addressPropertyReflector = new ValidationAttributesBasedPropertyRuleReflector (
          _addressPostalCodeProperty,
          _validationMessageFactory);
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void GetValidatedPropertyFunc_ReturnsFuncForPropertyAccess ()
    {
    }

    [Test]
    public void GetAddingPropertyValidators_Customer ()
    {
      var validationMessage = new InvariantValidationMessage ("Fake Message");
      _validationMessageFactory
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  Arg<Type>.Is.NotNull,
                  Arg.Is (PropertyInfoAdapter.Create (_customerLastNameProperty))))
          .Return (validationMessage);

      var addingPropertyValidators = _customerPropertyReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (addingPropertyValidators.Length, Is.EqualTo (2));
      Assert.That (
          addingPropertyValidators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (LengthValidator), typeof (NotNullValidator) }));

      Assert.That (addingPropertyValidators.OfType<LengthValidator>().Single().ValidationMessage, Is.SameAs (validationMessage));
      Assert.That (addingPropertyValidators.OfType<NotNullValidator>().Single().ValidationMessage, Is.SameAs (validationMessage));
    }

    [Test]
    public void GetAddingPropertyValidators_SpecialCustomer ()
    {
      var validationMessage = new InvariantValidationMessage ("Fake Message");
      _validationMessageFactory
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  Arg<Type>.Is.NotNull,
                  Arg.Is (PropertyInfoAdapter.Create (_specialCustomerLastNameProperty))))
          .Return (validationMessage);

      var addingPropertyValidators = _specialCustomerPropertyReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (addingPropertyValidators.Length, Is.EqualTo (2));
      Assert.That (
          addingPropertyValidators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (LengthValidator), typeof (NotNullValidator) }));

      Assert.That (addingPropertyValidators.OfType<LengthValidator>().Single().ValidationMessage, Is.SameAs (validationMessage));
      Assert.That (addingPropertyValidators.OfType<NotNullValidator>().Single().ValidationMessage, Is.SameAs (validationMessage));
    }

    [Test]
    public void GetHardConstraintPropertyValidators_Customer ()
    {
      var validationMessage = new InvariantValidationMessage ("Fake Message");
      _validationMessageFactory
          .Stub (
              _ => _.CreateValidationMessageForPropertyValidator (
                  Arg<Type>.Is.NotNull,
                  Arg.Is (PropertyInfoAdapter.Create (_customerLastNameProperty))))
          .Return (validationMessage);

      var hardConstraintPropertyValidators = _customerPropertyReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (hardConstraintPropertyValidators.Length, Is.EqualTo (1));
      Assert.That (hardConstraintPropertyValidators[0].GetType(), Is.EqualTo (typeof (NotEqualValidator)));

      Assert.That (((NotEqualValidator)hardConstraintPropertyValidators[0]).ValidationMessage, Is.SameAs (validationMessage));
    }

    [Test]
    public void GetHardConstraintPropertyValidators_SpecialCustomer ()
    {
      var hardConstraintPropertyValidators = _specialCustomerPropertyReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (hardConstraintPropertyValidators.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetRemovingPropertyRegistrations_Customer ()
    {
      var removingPropertyRegistrations = _customerPropertyReflector.GetRemovingPropertyRegistrations().ToArray();

      Assert.That (removingPropertyRegistrations.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetRemovingPropertyRegistrations_SpecialCustomer ()
    {
      var removingPropertyRegistrations = _specialCustomerPropertyReflector.GetRemovingPropertyRegistrations().ToArray();

      Assert.That (removingPropertyRegistrations.Length, Is.EqualTo (1));
      Assert.That (removingPropertyRegistrations[0].ValidatorType, Is.EqualTo (typeof (LengthValidator)));
    }

    [Test]
    public void GetMetaValidationRules_NoMetaValidationRule ()
    {
      Assert.That (_customerPropertyReflector.GetMetaValidationRules().Any(), Is.False);
    }

    [Test]
    public void GetMetaValidationRules_WithMetaValidationRule ()
    {
      var result = _addressPropertyReflector.GetMetaValidationRules().ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result.Select (r => r.GetType()), Is.EquivalentTo (new[] { typeof (AnyRuleAppliedMetaValidationRule) }));
    }
  }
}