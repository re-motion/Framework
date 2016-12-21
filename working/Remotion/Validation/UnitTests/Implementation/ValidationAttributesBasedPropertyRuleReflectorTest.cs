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
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.MetaValidation.Rules.Custom;
using Remotion.Validation.UnitTests.TestDomain;

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

    [SetUp]
    public void SetUp ()
    {
      _customerLastNameProperty = typeof (Customer).GetProperty ("UserName");
      _specialCustomerLastNameProperty = typeof (SpecialCustomer1).GetProperty ("UserName");
      _addressPostalCodeProperty = typeof (Address).GetProperty ("PostalCode");

      _customerPropertyReflector = new ValidationAttributesBasedPropertyRuleReflector (_customerLastNameProperty);
      _specialCustomerPropertyReflector = new ValidationAttributesBasedPropertyRuleReflector (_specialCustomerLastNameProperty);
      _addressPropertyReflector = new ValidationAttributesBasedPropertyRuleReflector (_addressPostalCodeProperty);
    }

    [Test]
    public void GetAddingPropertyValidators_Customer ()
    {
      var addingPropertyValidators = _customerPropertyReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (addingPropertyValidators.Count(), Is.EqualTo (2));
      Assert.That (
          addingPropertyValidators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (LengthValidator), typeof (NotNullValidator) }));
    }

    [Test]
    public void GetAddingPropertyValidators_SpecialCustomer ()
    {
      var addingPropertyValidators = _specialCustomerPropertyReflector.GetAddingPropertyValidators().ToArray();

      Assert.That (addingPropertyValidators.Count(), Is.EqualTo (2));
      Assert.That (
          addingPropertyValidators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (LengthValidator), typeof (NotNullValidator) }));
    }

    [Test]
    public void GetHardConstraintPropertyValidators_Customer ()
    {
      var hardConstraintPropertyValidators = _customerPropertyReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (hardConstraintPropertyValidators.Count(), Is.EqualTo (1));
      Assert.That (hardConstraintPropertyValidators[0].GetType(), Is.EqualTo (typeof (NotEqualValidator)));
    }

    [Test]
    public void GetHardConstraintPropertyValidators_SpecialCustomer ()
    {
      var hardConstraintPropertyValidators = _specialCustomerPropertyReflector.GetHardConstraintPropertyValidators().ToArray();

      Assert.That (hardConstraintPropertyValidators.Count(), Is.EqualTo (0));
    }

    [Test]
    public void GetRemovingPropertyRegistrations_Customer ()
    {
      var removingPropertyRegistrations = _customerPropertyReflector.GetRemovingPropertyRegistrations().ToArray();

      Assert.That (removingPropertyRegistrations.Count(), Is.EqualTo (0));
    }

    [Test]
    public void GetRemovingPropertyRegistrations_SpecialCustomer ()
    {
      var removingPropertyRegistrations = _specialCustomerPropertyReflector.GetRemovingPropertyRegistrations().ToArray();

      Assert.That (removingPropertyRegistrations.Count(), Is.EqualTo (1));
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

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result.Select (r => r.GetType()), Is.EquivalentTo (new[] { typeof (AnyRuleAppliedMetaValidationRule) }));
    }
  }
}