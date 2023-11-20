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
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class RemovingPropertyValidationRuleCollectorTest
  {
    private Expression<Func<Customer, string>> _userNameExpression;
    private Expression<Func<Customer, string>> _lastNameExpression;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector;
    private IPropertyInformation _property;

    [SetUp]
    public void SetUp ()
    {
      _property = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("UserName"));

      _userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.UserName);
      _lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.LastName);

      _removingPropertyValidationRuleCollector = RemovingPropertyValidationRuleCollector.Create(_userNameExpression, typeof(RemovingPropertyValidationRuleCollectorTest));
    }

    [Test]
    public void Initialization_PropertyDeclaredInSameClass ()
    {
      Assert.That(_removingPropertyValidationRuleCollector.Property.Equals(_property), Is.True);
      Assert.That(_removingPropertyValidationRuleCollector.Property, Is.EqualTo(_property));
      Assert.That(_removingPropertyValidationRuleCollector.CollectorType, Is.EqualTo(typeof(RemovingPropertyValidationRuleCollectorTest)));
      Assert.That(_removingPropertyValidationRuleCollector.Validators.Any(), Is.False);
    }

    [Test]
    public void Create_MemberInfoIsNoPropertyInfo_ExceptionIsThrown ()
    {
      var dummyExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.Dummy());

      Assert.That(
          () => RemovingPropertyValidationRuleCollector.Create(dummyExpression, typeof(CustomerValidationRuleCollector1)),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("Must be a MemberExpression.", "expression"));
    }

    [Test]
    public void Create_PropertyDeclaredInBaseClass ()
    {
      var componentPropertyRule = AddingPropertyValidationRuleCollector.Create(_lastNameExpression, typeof(CustomerValidationRuleCollector1));
      var propertyInfo = ((PropertyInfoAdapter)componentPropertyRule.Property).PropertyInfo;

      //TODO-5906 simplify assertion with PropertyInfoAdapter compare
      Assert.That(
          MemberInfoEqualityComparer<MemberInfo>.Instance.Equals(propertyInfo, typeof(Customer).GetMember("LastName")[0]),
          Is.True);
    }

    [Test]
    public void RegisterValidator_WithCollectorNullAndPredicateNull_AddsValidatorToCollector ()
    {
      _removingPropertyValidationRuleCollector.RegisterValidator(typeof(StubPropertyValidator), null, null);

      Assert.That(_removingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo(1));

      var removingPropertyValidatorRegistration = _removingPropertyValidationRuleCollector.Validators.Single();
      Assert.That(removingPropertyValidatorRegistration.ValidatorType, Is.EqualTo(typeof(StubPropertyValidator)));
      Assert.That(removingPropertyValidatorRegistration.CollectorTypeToRemoveFrom, Is.Null);
      Assert.That(removingPropertyValidatorRegistration.ValidatorPredicate, Is.Null);
      Assert.That(
          removingPropertyValidatorRegistration.RemovingPropertyValidationRuleCollector,
          Is.SameAs(_removingPropertyValidationRuleCollector));
    }

    [Test]
    public void RegisterValidator_WithCollectorNotNullAndPredicateNull_AddsValidatorToCollector ()
    {
      _removingPropertyValidationRuleCollector.RegisterValidator(typeof(NotEmptyOrWhitespaceValidator), typeof(CustomerValidationRuleCollector1), null);

      Assert.That(_removingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo(1));

      var removingPropertyValidatorRegistration = _removingPropertyValidationRuleCollector.Validators.Single();

      Assert.That(removingPropertyValidatorRegistration.ValidatorType, Is.EqualTo(typeof(NotEmptyOrWhitespaceValidator)));
      Assert.That(removingPropertyValidatorRegistration.CollectorTypeToRemoveFrom, Is.EqualTo(typeof(CustomerValidationRuleCollector1)));
      Assert.That(removingPropertyValidatorRegistration.ValidatorPredicate, Is.Null);
      Assert.That(
          removingPropertyValidatorRegistration.RemovingPropertyValidationRuleCollector,
          Is.SameAs(_removingPropertyValidationRuleCollector));

    }

    [Test]
    public void RegisterValidator_WithCollectorNullAndPredicateNotNull_AddsValidatorToCollector ()
    {
      Func<IPropertyValidator, bool> validatorPredicate = _ => false;
      _removingPropertyValidationRuleCollector.RegisterValidator(typeof(NotEmptyOrWhitespaceValidator), null, validatorPredicate);

      Assert.That(_removingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo(1));

      var removingPropertyValidatorRegistration = _removingPropertyValidationRuleCollector.Validators.Single();

      Assert.That(removingPropertyValidatorRegistration.ValidatorType, Is.EqualTo(typeof(NotEmptyOrWhitespaceValidator)));
      Assert.That(removingPropertyValidatorRegistration.CollectorTypeToRemoveFrom, Is.Null);
      Assert.That(removingPropertyValidatorRegistration.ValidatorPredicate, Is.SameAs(validatorPredicate));
      Assert.That(
          removingPropertyValidatorRegistration.RemovingPropertyValidationRuleCollector,
          Is.SameAs(_removingPropertyValidationRuleCollector));
    }

    [Test]
    public void RegisterValidator_WithMultipleValidators_AddsAllValidatorsToCollector ()
    {
      _removingPropertyValidationRuleCollector.RegisterValidator(typeof(StubPropertyValidator), null, null);
      _removingPropertyValidationRuleCollector.RegisterValidator(typeof(StubPropertyValidator), null, null);
      _removingPropertyValidationRuleCollector.RegisterValidator(typeof(NotEmptyOrWhitespaceValidator), null, null);

      Assert.That(
          _removingPropertyValidationRuleCollector.Validators.Select(v => v.ValidatorType),
          Is.EqualTo(new[] { typeof(StubPropertyValidator), typeof(StubPropertyValidator), typeof(NotEmptyOrWhitespaceValidator) }));
    }

    [Test]
    public void RegisterValidator_ValidatorTypeDoesNotImplementIPropertyValidator_ThrowsArgumentException ()
    {
      Assert.That(
          () => _removingPropertyValidationRuleCollector.RegisterValidator(typeof(Customer), typeof(CustomerValidationRuleCollector1), null),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'validatorType' is a 'Remotion.Validation.UnitTests.TestDomain.Customer', "
                  + "which cannot be assigned to type 'Remotion.Validation.Validators.IPropertyValidator'.",
                  "validatorType"));
    }

    [Test]
    public void RegisterValidator_CollectorTypeDoesNotImplementIValidationRuleCollector_ThrowsArgumentException ()
    {
      Assert.That(
          () => _removingPropertyValidationRuleCollector.RegisterValidator(
              typeof(StubPropertyValidator),
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
      var result = _removingPropertyValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo("RemovingPropertyValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }
  }
}
