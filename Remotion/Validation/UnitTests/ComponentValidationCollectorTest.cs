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
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestHelpers;

namespace Remotion.Validation.UnitTests
{
  [TestFixture]
  public class ComponentValidationCollectorTest
  {
    private Expression<Func<Customer, string>> _firstNameExpression;
    private Expression<Func<Customer, string>> _lastNameExpression;
    private Expression<Func<Customer, Address>> _addressExpression;
    private TestableComponentValidationCollector<Customer> _customerValidationCollector;

    [SetUp]
    public void SetUp ()
    {
      _firstNameExpression = c => c.FirstName;
      _lastNameExpression = c => c.LastName;
      _addressExpression = c => c.BillingAddress;
      _customerValidationCollector = new TestableComponentValidationCollector<Customer>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_customerValidationCollector.ValidatedType, Is.EqualTo (typeof (Customer)));
      Assert.That (_customerValidationCollector.AddedPropertyRules.Count(), Is.EqualTo (0));
      Assert.That (_customerValidationCollector.AddedPropertyMetaValidationRules.Count(), Is.EqualTo (0));
      Assert.That (_customerValidationCollector.RemovedPropertyRules.Count(), Is.EqualTo (0));
    }

    [Test]
    public void AddRule_OneRule ()
    {
      var result = _customerValidationCollector.AddRule (_firstNameExpression);

      Assert.That (_customerValidationCollector.AddedPropertyRules.Count(), Is.EqualTo (1));
      Assert.That (_customerValidationCollector.AddedPropertyMetaValidationRules.Count(), Is.EqualTo (1));
      Assert.That (_customerValidationCollector.RemovedPropertyRules.Count(), Is.EqualTo (0));

      var propertyRule = _customerValidationCollector.AddedPropertyRules.First();
      var metaValidationPropertyRule = _customerValidationCollector.AddedPropertyMetaValidationRules.First();

      Assert.That (result, Is.TypeOf (typeof (AddingComponentRuleBuilder<Customer, string>)));
      var resultAsComponentRuleBuilder = (AddingComponentRuleBuilder<Customer, string>) result;
      Assert.That (resultAsComponentRuleBuilder.AddingComponentPropertyRule.CollectorType, Is.SameAs (_customerValidationCollector.GetType()));
      Assert.That (resultAsComponentRuleBuilder.AddingComponentPropertyRule, Is.SameAs (propertyRule));
      Assert.That (resultAsComponentRuleBuilder.AddingComponentPropertyRule.Validators.Count(), Is.EqualTo (0));
      Assert.That (resultAsComponentRuleBuilder.AddingMetaValidationPropertyRule.CollectorType, Is.SameAs (_customerValidationCollector.GetType()));
      Assert.That (resultAsComponentRuleBuilder.AddingMetaValidationPropertyRule, Is.SameAs (metaValidationPropertyRule));
    }

    [Test]
    public void AddRule_ThreeRules ()
    {
      var result1 = _customerValidationCollector.AddRule (_firstNameExpression);
      var result2 = _customerValidationCollector.AddRule (_addressExpression);
      var result3 = _customerValidationCollector.AddRule (_lastNameExpression);

      Assert.That (_customerValidationCollector.AddedPropertyRules.Count(), Is.EqualTo (3));
      Assert.That (_customerValidationCollector.AddedPropertyMetaValidationRules.Count(), Is.EqualTo (3));
      Assert.That (_customerValidationCollector.RemovedPropertyRules.Count(), Is.EqualTo (0));
      Assert.That (result1, Is.TypeOf (typeof (AddingComponentRuleBuilder<Customer, string>)));
      Assert.That (result2, Is.TypeOf (typeof (AddingComponentRuleBuilder<Customer, Address>)));
      Assert.That (result3, Is.TypeOf (typeof (AddingComponentRuleBuilder<Customer, string>)));
    }

    [Test]
    public void RemoveRule_OneRule ()
    {
      var result = _customerValidationCollector.RemoveRule (_firstNameExpression);

      Assert.That (_customerValidationCollector.RemovedPropertyRules.Count(), Is.EqualTo (1));
      Assert.That (_customerValidationCollector.AddedPropertyRules.Count(), Is.EqualTo (0));

      var propertyRule = _customerValidationCollector.RemovedPropertyRules.First();
      Assert.That (result, Is.TypeOf (typeof (RemovingComponentRuleBuilder<Customer, string>)));
      var resultAsComponentRuleBuilder = (RemovingComponentRuleBuilder<Customer, string>) result;
      Assert.That (resultAsComponentRuleBuilder.RemovingComponentPropertyRule.CollectorType, Is.SameAs (_customerValidationCollector.GetType()));
      Assert.That (resultAsComponentRuleBuilder.RemovingComponentPropertyRule, Is.SameAs (propertyRule));
      Assert.That (resultAsComponentRuleBuilder.RemovingComponentPropertyRule.Validators.Count(), Is.EqualTo (0));
    }

    [Test]
    public void RemoveRule_ThreeRules ()
    {
      var result1 = _customerValidationCollector.RemoveRule (_firstNameExpression);
      var result2 = _customerValidationCollector.RemoveRule (_addressExpression);
      var result3 = _customerValidationCollector.RemoveRule (_lastNameExpression);

      Assert.That (_customerValidationCollector.RemovedPropertyRules.Count(), Is.EqualTo (3));
      Assert.That (_customerValidationCollector.AddedPropertyRules.Count(), Is.EqualTo (0));
      Assert.That (_customerValidationCollector.AddedPropertyMetaValidationRules.Count(), Is.EqualTo (0));
      Assert.That (result1, Is.TypeOf (typeof (RemovingComponentRuleBuilder<Customer, string>)));
      Assert.That (result2, Is.TypeOf (typeof (RemovingComponentRuleBuilder<Customer, Address>)));
      Assert.That (result3, Is.TypeOf (typeof (RemovingComponentRuleBuilder<Customer, string>)));
    }

    [Test]
    public void When_NoRuleIsAdded ()
    {
      _customerValidationCollector.When (c => c.FirstName == "Test", () => { });

      Assert.That (_customerValidationCollector.AddedPropertyRules.Count(), Is.EqualTo (0));
      Assert.That (_customerValidationCollector.AddedPropertyMetaValidationRules.Count(), Is.EqualTo (0));
      Assert.That (_customerValidationCollector.RemovedPropertyRules.Count(), Is.EqualTo (0));
    }

    [Test]
    public void When_TwoRulesWithSeveralValidatorsAreAdded_ConditionIsApplied ()
    {
      var stubPropertyValidator1 = new StubPropertyValidator();
      var stubPropertyValidator2 = new StubPropertyValidator();
      var stubPropertyValidator3 = new StubPropertyValidator();
      Assert.That (_customerValidationCollector.AddedPropertyRules.Count(), Is.EqualTo (0));
      Assert.That (_customerValidationCollector.AddedPropertyMetaValidationRules.Count(), Is.EqualTo (0));
      var customer1 = new Customer { FirstName = "Test" };
      var customer2 = new Customer { FirstName = "NoTest" };

      _customerValidationCollector.When (
          c => c.FirstName == "Test",
          () =>
          {
            _customerValidationCollector.AddRule (_firstNameExpression).SetValidator (stubPropertyValidator1).SetValidator (stubPropertyValidator2);
            _customerValidationCollector.AddRule (_addressExpression).SetValidator (stubPropertyValidator3);
          });

      Assert.That (_customerValidationCollector.AddedPropertyMetaValidationRules.Count(), Is.EqualTo (2));
      var addedPropertyRules = _customerValidationCollector.AddedPropertyRules.ToArray();
      Assert.That (addedPropertyRules.Count(), Is.EqualTo (2));
      var propertyValidators = addedPropertyRules.SelectMany (pr => pr.Validators).ToArray();
      Assert.That (propertyValidators.Count(), Is.EqualTo (3));
      foreach (var propertyValidator in propertyValidators)
      {
        Assert.That (propertyValidator.GetType(), Is.EqualTo (typeof (DelegatingValidator)));
        var condition = (Func<object, bool>) PrivateInvoke.GetNonPublicField (propertyValidator, "condition");
        Assert.That (condition (customer1), Is.True);
        Assert.That (condition (customer2), Is.False);
      }

      Assert.That (_customerValidationCollector.RemovedPropertyRules.Count(), Is.EqualTo (0));
    }

    [Test]
    public void When_RemovedPropertyRules_InvalidOperationExceptionIsThrown ()
    {
      Assert.Throws<InvalidOperationException> (
          () => _customerValidationCollector.When (
              c => c.FirstName == "Test",
              () =>
                  _customerValidationCollector.RemoveRule (_firstNameExpression).Validator<StubPropertyValidator>()),
          "Conditions are not allowed for removing validation rules registrations.");
    }

    [Test]
    public void Unless_TwoRulesWithSeveralValidatorsAreAdded_ConditionIsApplied ()
    {
      var stubPropertyValidator1 = new StubPropertyValidator();
      var stubPropertyValidator2 = new StubPropertyValidator();
      var stubPropertyValidator3 = new StubPropertyValidator();
      Assert.That (_customerValidationCollector.AddedPropertyRules.Count(), Is.EqualTo (0));
      Assert.That (_customerValidationCollector.AddedPropertyMetaValidationRules.Count(), Is.EqualTo (0));

      var customer1 = new Customer { FirstName = "Test" };
      var customer2 = new Customer { FirstName = "NoTest" };

      _customerValidationCollector.Unless (
          c => c.FirstName == "Test",
          () =>
          {
            _customerValidationCollector.AddRule (_firstNameExpression).SetValidator (stubPropertyValidator1).SetValidator (stubPropertyValidator2);
            _customerValidationCollector.AddRule (_addressExpression).SetValidator (stubPropertyValidator3);
          });

      Assert.That (_customerValidationCollector.AddedPropertyMetaValidationRules.Count(), Is.EqualTo (2));
      var addedPropertyRules = _customerValidationCollector.AddedPropertyRules.ToArray();
      Assert.That (addedPropertyRules.Count(), Is.EqualTo (2));
      var propertyValidators = addedPropertyRules.SelectMany (pr => pr.Validators).ToArray();
      Assert.That (propertyValidators.Count(), Is.EqualTo (3));
      foreach (var propertyValidator in propertyValidators)
      {
        Assert.That (propertyValidator.GetType(), Is.EqualTo (typeof (DelegatingValidator)));
        var condition = (Func<object, bool>) PrivateInvoke.GetNonPublicField (propertyValidator, "condition");
        Assert.That (condition (customer1), Is.False);
        Assert.That (condition (customer2), Is.True);
      }

      Assert.That (_customerValidationCollector.RemovedPropertyRules.Count(), Is.EqualTo (0));
    }
  }
}