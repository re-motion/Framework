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
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.RuleBuilders
{
  [TestFixture]
  public class RemovingPropertyValidationRuleBuilderTest
  {
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollectorMock;
    private RemovingPropertyValidationRuleBuilder<Customer, string> _addingPropertyValidationBuilder;

    [SetUp]
    public void SetUp ()
    {
      _removingPropertyValidationRuleCollectorMock = MockRepository.GenerateStrictMock<IRemovingPropertyValidationRuleCollector>();
      _addingPropertyValidationBuilder = new RemovingPropertyValidationRuleBuilder<Customer, string> (_removingPropertyValidationRuleCollectorMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_addingPropertyValidationBuilder.RemovingPropertyValidationRuleCollector, Is.SameAs (_removingPropertyValidationRuleCollectorMock));
    }

    [Test]
    public void RegisterValidator ()
    {
      _removingPropertyValidationRuleCollectorMock.Expect (mock => mock.RegisterValidator (typeof (StubPropertyValidator), null, null));

      _addingPropertyValidationBuilder.Validator (typeof (StubPropertyValidator), null, null);

      _removingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterValidator_WithGenericValidatorType ()
    {
      _removingPropertyValidationRuleCollectorMock.Expect (mock => mock.RegisterValidator (typeof (StubPropertyValidator), null, null));

      _addingPropertyValidationBuilder.Validator<StubPropertyValidator> (null);

      _removingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterValidator_WithGenericValidatorType_WithPredicate ()
    {
      var expectedValidator = new StubPropertyValidator();
      var expectedPredicateResult = BooleanObjectMother.GetRandomBoolean();
      StubPropertyValidator actualValidator = null;
      Func<StubPropertyValidator, bool> predicate = validator =>
      {
        actualValidator = validator;
        return expectedPredicateResult;
      };
      Func<IPropertyValidator, bool> actualPredicate = null;

      _removingPropertyValidationRuleCollectorMock
          .Stub (
              mock => mock.RegisterValidator (
                  Arg.Is (typeof (StubPropertyValidator)),
                  Arg<Type>.Is.Null,
                  Arg<Func<IPropertyValidator, bool>>.Is.NotNull))
          .WhenCalled (mi => actualPredicate = (Func<IPropertyValidator, bool>) mi.Arguments[2]);

      _addingPropertyValidationBuilder.Validator<StubPropertyValidator> (predicate);

      Assert.That (actualPredicate, Is.Not.Null);

      var actualPredicateResult = actualPredicate (expectedValidator);

      Assert.That (actualPredicateResult, Is.EqualTo (expectedPredicateResult));
      Assert.That (actualValidator, Is.SameAs (expectedValidator));
    }

    [Test]
    public void RegisterValidator_WithGenericValidatorType_WithPredicate_WithInvalidValidatorType_ThrowsArgumentException ()
    {
      Func<IPropertyValidator, bool> actualPredicate = null;

      _removingPropertyValidationRuleCollectorMock
          .Stub (
              mock => mock.RegisterValidator (
                  Arg.Is (typeof (StubPropertyValidator)),
                  Arg<Type>.Is.Null,
                  Arg<Func<IPropertyValidator, bool>>.Is.NotNull))
          .WhenCalled (mi => actualPredicate = (Func<IPropertyValidator, bool>) mi.Arguments[2]);

      _addingPropertyValidationBuilder.Validator<StubPropertyValidator> (validator => true);

      Assert.That (actualPredicate, Is.Not.Null);

      var notEmptyValidator = new NotEmptyValidator (new InvariantValidationMessage ("Foo"));
      Assert.That (() => actualPredicate (notEmptyValidator), Throws.ArgumentException);
    }

    [Test]
    public void RegisterValidator_WithCollectorType ()
    {
      _removingPropertyValidationRuleCollectorMock.Expect (
          mock => mock.RegisterValidator (typeof (StubPropertyValidator), typeof (CustomerValidationRuleCollector1), null));

      _addingPropertyValidationBuilder.Validator (typeof (StubPropertyValidator), typeof (CustomerValidationRuleCollector1), null);

      _removingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterValidator_WithPredicate ()
    {
      Func<IPropertyValidator, bool> predicate = _ => false;

      _removingPropertyValidationRuleCollectorMock.Expect (
          mock => mock.RegisterValidator (typeof (StubPropertyValidator), null, predicate));

      _addingPropertyValidationBuilder.Validator (typeof (StubPropertyValidator), null, predicate);

      _removingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterValidator_WithGenericCollectorType ()
    {
      _removingPropertyValidationRuleCollectorMock.Expect (
          mock => mock.RegisterValidator (typeof (StubPropertyValidator), typeof (CustomerValidationRuleCollector1), null));

      _addingPropertyValidationBuilder.Validator<StubPropertyValidator, CustomerValidationRuleCollector1> (null);

      _removingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterValidator_WithGenericCollectorType_WithPredicate ()
    {
      var expectedValidator = new StubPropertyValidator();
      var expectedPredicateResult = BooleanObjectMother.GetRandomBoolean();
      StubPropertyValidator actualValidator = null;
      Func<StubPropertyValidator, bool> predicate = validator =>
      {
        actualValidator = validator;
        return expectedPredicateResult;
      };
      Func<IPropertyValidator, bool> actualPredicate = null;

      _removingPropertyValidationRuleCollectorMock
          .Stub (
              mock => mock.RegisterValidator (
                  Arg.Is (typeof (StubPropertyValidator)),
                  Arg.Is (typeof (CustomerValidationRuleCollector1)),
                  Arg<Func<IPropertyValidator, bool>>.Is.NotNull))
          .WhenCalled (mi => actualPredicate = (Func<IPropertyValidator, bool>) mi.Arguments[2]);

      _addingPropertyValidationBuilder.Validator<StubPropertyValidator, CustomerValidationRuleCollector1> (predicate);

      Assert.That (actualPredicate, Is.Not.Null);

      var actualPredicateResult = actualPredicate (expectedValidator);

      Assert.That (actualPredicateResult, Is.EqualTo (expectedPredicateResult));
      Assert.That (actualValidator, Is.SameAs (expectedValidator));
    }

    [Test]
    public void RegisterValidator_WithGenericCollectorType_WithPredicate_WithInvalidValidatorType_ThrowsArgumentException ()
    {
      Func<IPropertyValidator, bool> actualPredicate = null;

      _removingPropertyValidationRuleCollectorMock
          .Stub (
              mock => mock.RegisterValidator (
                  Arg.Is (typeof (StubPropertyValidator)),
                  Arg.Is (typeof (CustomerValidationRuleCollector1)),
                  Arg<Func<IPropertyValidator, bool>>.Is.NotNull))
          .WhenCalled (mi => actualPredicate = (Func<IPropertyValidator, bool>) mi.Arguments[2]);

      _addingPropertyValidationBuilder.Validator<StubPropertyValidator, CustomerValidationRuleCollector1> (validator => true);

      Assert.That (actualPredicate, Is.Not.Null);

      var notEmptyValidator = new NotEmptyValidator (new InvariantValidationMessage ("Foo"));
      Assert.That (() => actualPredicate (notEmptyValidator), Throws.ArgumentException);
    }
  }
}