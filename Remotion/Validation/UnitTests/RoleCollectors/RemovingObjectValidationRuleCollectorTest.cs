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
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Reflection;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestDomain.Validators;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class RemovingObjectValidationRuleCollectorTest
  {
    private IRemovingObjectValidationRuleCollector _removingObjectValidationRuleCollector;

    [SetUp]
    public void SetUp ()
    {
      _removingObjectValidationRuleCollector = RemovingObjectValidationRuleCollector.Create<Customer>(typeof(CustomerValidationRuleCollector1));
    }

    [Test]
    public void Initialization_PropertyDeclaredInSameClass ()
    {
      Assert.That(_removingObjectValidationRuleCollector.ValidatedType, Is.EqualTo(TypeAdapter.Create(typeof(Customer))));
      Assert.That(_removingObjectValidationRuleCollector.CollectorType, Is.EqualTo(typeof(CustomerValidationRuleCollector1)));
      Assert.That(_removingObjectValidationRuleCollector.Validators.Any(), Is.False);
    }

    [Test]
    public void RegisterValidator_WithCollectorNullAndPredicateNull_AddsValidatorToCollector ()
    {
      _removingObjectValidationRuleCollector.RegisterValidator(typeof(StubObjectValidator), null, null);

      Assert.That(_removingObjectValidationRuleCollector.Validators.Count(), Is.EqualTo(1));

      var removingPropertyValidatorRegistration = _removingObjectValidationRuleCollector.Validators.Single();
      Assert.That(removingPropertyValidatorRegistration.ValidatorType, Is.EqualTo(typeof(StubObjectValidator)));
      Assert.That(removingPropertyValidatorRegistration.CollectorTypeToRemoveFrom, Is.Null);
      Assert.That(removingPropertyValidatorRegistration.ValidatorPredicate, Is.Null);
      Assert.That(
          removingPropertyValidatorRegistration.RemovingObjectValidationRuleCollector,
          Is.SameAs(_removingObjectValidationRuleCollector));
    }

    [Test]
    public void RegisterValidator_WithCollectorNotNullAndPredicateNull_AddsValidatorToCollector ()
    {
      _removingObjectValidationRuleCollector.RegisterValidator(typeof(FakeCustomerValidator), typeof(CustomerValidationRuleCollector1), null);

      Assert.That(_removingObjectValidationRuleCollector.Validators.Count(), Is.EqualTo(1));

      var removingPropertyValidatorRegistration = _removingObjectValidationRuleCollector.Validators.Single();

      Assert.That(removingPropertyValidatorRegistration.ValidatorType, Is.EqualTo(typeof(FakeCustomerValidator)));
      Assert.That(removingPropertyValidatorRegistration.CollectorTypeToRemoveFrom, Is.EqualTo(typeof(CustomerValidationRuleCollector1)));
      Assert.That(removingPropertyValidatorRegistration.ValidatorPredicate, Is.Null);
      Assert.That(
          removingPropertyValidatorRegistration.RemovingObjectValidationRuleCollector,
          Is.SameAs(_removingObjectValidationRuleCollector));

    }

    [Test]
    public void RegisterValidator_WithCollectorNullAndPredicateNotNull_AddsValidatorToCollector ()
    {
      Func<IObjectValidator, bool> validatorPredicate = _ => false;
      _removingObjectValidationRuleCollector.RegisterValidator(typeof(FakeCustomerValidator), null, validatorPredicate);

      Assert.That(_removingObjectValidationRuleCollector.Validators.Count(), Is.EqualTo(1));

      var removingPropertyValidatorRegistration = _removingObjectValidationRuleCollector.Validators.Single();

      Assert.That(removingPropertyValidatorRegistration.ValidatorType, Is.EqualTo(typeof(FakeCustomerValidator)));
      Assert.That(removingPropertyValidatorRegistration.CollectorTypeToRemoveFrom, Is.Null);
      Assert.That(removingPropertyValidatorRegistration.ValidatorPredicate, Is.SameAs(validatorPredicate));
      Assert.That(
          removingPropertyValidatorRegistration.RemovingObjectValidationRuleCollector,
          Is.SameAs(_removingObjectValidationRuleCollector));
    }

    [Test]
    public void RegisterValidator_WithMultipleValidators_AddsAllValidatorsToCollector ()
    {
      _removingObjectValidationRuleCollector.RegisterValidator(typeof(FakeCustomerValidator), null, null);
      _removingObjectValidationRuleCollector.RegisterValidator(typeof(FakeCustomerValidator), null, null);
      _removingObjectValidationRuleCollector.RegisterValidator(typeof(StubObjectValidator), null, null);

      Assert.That(
          _removingObjectValidationRuleCollector.Validators.Select(v => v.ValidatorType),
          Is.EqualTo(new[] { typeof(FakeCustomerValidator), typeof(FakeCustomerValidator), typeof(StubObjectValidator) }));
    }

    [Test]
    public void RegisterValidator_ValidatorTypeDoesNotImplementIObjectValidator_ThrowsArgumentException ()
    {
      Assert.That(
          () => _removingObjectValidationRuleCollector.RegisterValidator(typeof(Customer), typeof(CustomerValidationRuleCollector1), null),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'validatorType' is a 'Remotion.Validation.UnitTests.TestDomain.Customer', "
                  + "which cannot be assigned to type 'Remotion.Validation.Validators.IObjectValidator'.",
                  "validatorType"));
    }

    [Test]
    public void RegisterValidator_CollectorTypeDoesNotImplementIValidationRuleCollector_ThrowsArgumentException ()
    {
      Assert.That(
          () => _removingObjectValidationRuleCollector.RegisterValidator(
              typeof(FakeCustomerValidator),
              typeof(Customer),
              null),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'collectorTypeToRemoveFrom' is a 'Remotion.Validation.UnitTests.TestDomain.Customer', "
                  + "which cannot be assigned to type 'Remotion.Validation.IValidationRuleCollector'.",
                  "collectorTypeToRemoveFrom"));
    }

    [Test]
    public void ToString_Overridden ()
    {
      var result = _removingObjectValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo("RemovingObjectValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer"));
    }
  }
}
