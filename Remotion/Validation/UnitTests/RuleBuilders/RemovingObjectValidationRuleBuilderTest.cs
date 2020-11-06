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
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestDomain.Validators;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.RuleBuilders
{
  [TestFixture]
  public class RemovingObjectValidationRuleBuilderTest
  {
    private IRemovingObjectValidationRuleCollector _removingObjectValidationRuleCollectorMock;
    private RemovingObjectValidationRuleBuilder<Customer> _addingObjectValidationBuilder;

    [SetUp]
    public void SetUp ()
    {
      _removingObjectValidationRuleCollectorMock = MockRepository.GenerateStrictMock<IRemovingObjectValidationRuleCollector>();
      _addingObjectValidationBuilder = new RemovingObjectValidationRuleBuilder<Customer> (_removingObjectValidationRuleCollectorMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_addingObjectValidationBuilder.RemovingObjectValidationRuleCollector, Is.SameAs (_removingObjectValidationRuleCollectorMock));
    }

    [Test]
    public void RegisterValidator ()
    {
      _removingObjectValidationRuleCollectorMock.Expect (mock => mock.RegisterValidator (typeof (StubObjectValidator), null, null));

      _addingObjectValidationBuilder.Validator (typeof (StubObjectValidator), null, null);

      _removingObjectValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterValidator_WithGenericValidatorType ()
    {
      _removingObjectValidationRuleCollectorMock.Expect (mock => mock.RegisterValidator (typeof (StubObjectValidator), null, null));

      _addingObjectValidationBuilder.Validator<StubObjectValidator> (null);

      _removingObjectValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterValidator_WithGenericValidatorType_WithPredicate ()
    {
      var expectedValidator = new StubObjectValidator();
      var expectedPredicateResult = BooleanObjectMother.GetRandomBoolean();
      StubObjectValidator actualValidator = null;
      Func<StubObjectValidator, bool> predicate = validator =>
      {
        actualValidator = validator;
        return expectedPredicateResult;
      };
      Func<IObjectValidator, bool> actualPredicate = null;

      _removingObjectValidationRuleCollectorMock
          .Stub (
              mock => mock.RegisterValidator (
                  Arg.Is (typeof (StubObjectValidator)),
                  Arg<Type>.Is.Null,
                  Arg<Func<IObjectValidator, bool>>.Is.NotNull))
          .WhenCalled (mi => actualPredicate = (Func<IObjectValidator, bool>) mi.Arguments[2]);

      _addingObjectValidationBuilder.Validator<StubObjectValidator> (predicate);

      Assert.That (actualPredicate, Is.Not.Null);

      var actualPredicateResult = actualPredicate (expectedValidator);

      Assert.That (actualPredicateResult, Is.EqualTo (expectedPredicateResult));
      Assert.That (actualValidator, Is.SameAs (expectedValidator));
    }

    [Test]
    public void RegisterValidator_WithGenericValidatorType_WithPredicate_WithInvalidValidatorType_ThrowsArgumentException ()
    {
      Func<IObjectValidator, bool> actualPredicate = null;

      _removingObjectValidationRuleCollectorMock
          .Stub (
              mock => mock.RegisterValidator (
                  Arg.Is (typeof (StubObjectValidator)),
                  Arg<Type>.Is.Null,
                  Arg<Func<IObjectValidator, bool>>.Is.NotNull))
          .WhenCalled (mi => actualPredicate = (Func<IObjectValidator, bool>) mi.Arguments[2]);

      _addingObjectValidationBuilder.Validator<StubObjectValidator> (validator => true);

      Assert.That (actualPredicate, Is.Not.Null);

      var otherValidator = new FakeCustomerValidator();
      Assert.That (() => actualPredicate (otherValidator), Throws.ArgumentException);
    }

    [Test]
    public void RegisterValidator_WithCollectorType ()
    {
      _removingObjectValidationRuleCollectorMock.Expect (
          mock => mock.RegisterValidator (typeof (StubObjectValidator), typeof (CustomerValidationRuleCollector1), null));

      _addingObjectValidationBuilder.Validator (typeof (StubObjectValidator), typeof (CustomerValidationRuleCollector1), null);

      _removingObjectValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterValidator_WithPredicate ()
    {
      Func<IObjectValidator, bool> predicate = _ => false;

      _removingObjectValidationRuleCollectorMock.Expect (
          mock => mock.RegisterValidator (typeof (StubObjectValidator), null, predicate));

      _addingObjectValidationBuilder.Validator (typeof (StubObjectValidator), null, predicate);

      _removingObjectValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterValidator_WithGenericCollectorType ()
    {
      _removingObjectValidationRuleCollectorMock.Expect (
          mock => mock.RegisterValidator (typeof (StubObjectValidator), typeof (CustomerValidationRuleCollector1), null));

      _addingObjectValidationBuilder.Validator<StubObjectValidator, CustomerValidationRuleCollector1> (null);

      _removingObjectValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void RegisterValidator_WithGenericCollectorType_WithPredicate ()
    {
      var expectedValidator = new StubObjectValidator();
      var expectedPredicateResult = BooleanObjectMother.GetRandomBoolean();
      StubObjectValidator actualValidator = null;
      Func<StubObjectValidator, bool> predicate = validator =>
      {
        actualValidator = validator;
        return expectedPredicateResult;
      };
      Func<IObjectValidator, bool> actualPredicate = null;

      _removingObjectValidationRuleCollectorMock
          .Stub (
              mock => mock.RegisterValidator (
                  Arg.Is (typeof (StubObjectValidator)),
                  Arg.Is (typeof (CustomerValidationRuleCollector1)),
                  Arg<Func<IObjectValidator, bool>>.Is.NotNull))
          .WhenCalled (mi => actualPredicate = (Func<IObjectValidator, bool>) mi.Arguments[2]);

      _addingObjectValidationBuilder.Validator<StubObjectValidator, CustomerValidationRuleCollector1> (predicate);

      Assert.That (actualPredicate, Is.Not.Null);

      var actualPredicateResult = actualPredicate (expectedValidator);

      Assert.That (actualPredicateResult, Is.EqualTo (expectedPredicateResult));
      Assert.That (actualValidator, Is.SameAs (expectedValidator));
    }

    [Test]
    public void RegisterValidator_WithGenericCollectorType_WithPredicate_WithInvalidValidatorType_ThrowsArgumentException ()
    {
      Func<IObjectValidator, bool> actualPredicate = null;

      _removingObjectValidationRuleCollectorMock
          .Stub (
              mock => mock.RegisterValidator (
                  Arg.Is (typeof (StubObjectValidator)),
                  Arg.Is (typeof (CustomerValidationRuleCollector1)),
                  Arg<Func<IObjectValidator, bool>>.Is.NotNull))
          .WhenCalled (mi => actualPredicate = (Func<IObjectValidator, bool>) mi.Arguments[2]);

      _addingObjectValidationBuilder.Validator<StubObjectValidator, CustomerValidationRuleCollector1> (validator => true);

      Assert.That (actualPredicate, Is.Not.Null);

      var otherValidator = new FakeCustomerValidator();
      Assert.That (() => actualPredicate (otherValidator), Throws.ArgumentException);
    }
  }
}