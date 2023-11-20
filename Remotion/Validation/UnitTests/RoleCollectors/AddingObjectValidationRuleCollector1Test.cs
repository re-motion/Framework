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
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestDomain.Validators;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class AddingObjectValidationRuleCollector1Test
  {
    private AddingObjectValidationRuleCollector<Customer> _addingObjectValidationRuleCollector;
    private Mock<IObjectValidatorExtractor> _objectValidatorExtractorMock;
    private IObjectValidator _stubObjectValidator1;
    private Mock<IObjectValidator> _stubObjectValidator2;
    private IObjectValidator _stubObjectValidator3;
    private ITypeInformation _validatedType;

    [SetUp]
    public void SetUp ()
    {
      _addingObjectValidationRuleCollector = AddingObjectValidationRuleCollector.Create<Customer>(typeof(CustomerValidationRuleCollector1));
      _objectValidatorExtractorMock = new Mock<IObjectValidatorExtractor>();
      _stubObjectValidator1 = new StubObjectValidator();
      _stubObjectValidator2 = new Mock<IObjectValidator>();
      _stubObjectValidator3 = new FakeCustomerValidator();
      _validatedType = TypeAdapter.Create(typeof(Customer));
    }

    [Test]
    public void Initialization_CollectorTypeDoesNotImplementIValidationRuleCollector_ThrowsArgumentException ()
    {
      Assert.That(
          () => new AddingObjectValidationRuleCollector<Customer>(typeof(Customer)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'collectorType' is a 'Remotion.Validation.UnitTests.TestDomain.Customer', "
                  + "which cannot be assigned to type 'Remotion.Validation.IValidationRuleCollector'.",
                  "collectorType"));
    }

    [Test]
    public void CreateValidationRule_InitializesDeferredInitializationValidationMessages ()
    {
      var validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub
          .Setup(e => e.CreateValidationMessageForObjectValidator(It.IsAny<IObjectValidator>(), _validatedType))
          .Returns(new InvariantValidationMessage("expectedMessage"));

      Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactory = param => new FakeObjectValidator(param.ValidationMessage);

      _addingObjectValidationRuleCollector.RegisterValidator(validatorFactory);
      var result = ((IAddingObjectValidationRuleCollector)_addingObjectValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub.Object);

      var fakeObjectValidators = ((ObjectValidationRule<Customer>)result).Validators.Cast<FakeObjectValidator>().ToArray();
      Assert.That(fakeObjectValidators.Length, Is.EqualTo(1));
      Assert.That(fakeObjectValidators[0].ValidationMessage.ToString(), Is.EqualTo("expectedMessage"));
    }

    [Test]
    public void CreateValidationRule_WhenCalledTwice_InitializesDeferredInitializationValidationMessagesOnlyForNewlyRegisteredValidators ()
    {
      var validationMessageFactoryStub1 = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub1
          .Setup(e => e.CreateValidationMessageForObjectValidator(It.IsAny<IObjectValidator>(), _validatedType))
          .Returns(new InvariantValidationMessage("expectedMessage1"));

      var validationMessageFactoryStub2 = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub2
          .Setup(e => e.CreateValidationMessageForObjectValidator(It.IsAny<IObjectValidator>(), _validatedType))
          .Returns(new InvariantValidationMessage("expectedMessage2"));

      Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactory = param => new FakeObjectValidator(param.ValidationMessage);

      _addingObjectValidationRuleCollector.RegisterValidator(validatorFactory);
      var result1 = ((IAddingObjectValidationRuleCollector)_addingObjectValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub1.Object);

      var fakeObjectValidators1 = ((ObjectValidationRule<Customer>)result1).Validators.Cast<FakeObjectValidator>().ToArray();
      Assert.That(fakeObjectValidators1.Length, Is.EqualTo(1));
      Assert.That(fakeObjectValidators1[0].ValidationMessage.ToString(), Is.EqualTo("expectedMessage1"));

      _addingObjectValidationRuleCollector.RegisterValidator(validatorFactory);
      var result2 = ((IAddingObjectValidationRuleCollector)_addingObjectValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub2.Object);

      var fakeObjectValidators2 = ((ObjectValidationRule<Customer>)result2).Validators.Cast<FakeObjectValidator>().ToArray();
      Assert.That(fakeObjectValidators2.Length, Is.EqualTo(2));
      Assert.That(fakeObjectValidators2[0].ValidationMessage.ToString(), Is.EqualTo("expectedMessage1"));
      Assert.That(fakeObjectValidators2[1].ValidationMessage.ToString(), Is.EqualTo("expectedMessage2"));
    }

    [Test]
    public void CreateValidationRule_IgnoresValidationMessagesForValidatorsWithoutADeferredInitializationValidationMessage ()
    {
      var validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub
          .Setup(e => e.CreateValidationMessageForObjectValidator(It.IsAny<IObjectValidator>(), _validatedType))
          .Returns(new InvariantValidationMessage("unexpectedMessage"));

      Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactory = _ => new FakeObjectValidator(new InvariantValidationMessage("expectedMessage"));

      (_addingObjectValidationRuleCollector).RegisterValidator(validatorFactory);
      var result = ((IAddingObjectValidationRuleCollector)_addingObjectValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub.Object);

      var fakeObjectValidators = ((ObjectValidationRule<Customer>)result).Validators.Cast<FakeObjectValidator>().ToArray();
      Assert.That(fakeObjectValidators.Length, Is.EqualTo(1));
      Assert.That(fakeObjectValidators[0].ValidationMessage.ToString(), Is.EqualTo("expectedMessage"));
    }

    [Test]
    public void CreateValidationRule_WithValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
      var validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub
          .Setup(_ => _.CreateValidationMessageForObjectValidator(It.IsAny<IObjectValidator>(), _validatedType))
          .Returns((ValidationMessage)null);

      Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactory = param => new FakeObjectValidator(param.ValidationMessage);

      _addingObjectValidationRuleCollector.RegisterValidator(validatorFactory);

      Assert.That(
          () => ((IAddingObjectValidationRuleCollector)_addingObjectValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The IValidationMessageFactory did not return a result for 'FakeObjectValidator' applied to "
                  + "type 'Remotion.Validation.UnitTests.TestDomain.Customer'."));
    }

    [Test]
    public void CreateValidationRule_WithConditionNotNull_InitializesConditionForCreatedObjectValidationRule ()
    {
      var validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub
          .Setup(e => e.CreateValidationMessageForObjectValidator(It.IsAny<IObjectValidator>(), _validatedType))
          .Returns(new InvariantValidationMessage("expectedMessage"));

      Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactory = param => new FakeObjectValidator(param.ValidationMessage);
      Func<Customer, bool> predicate = _ => true;

      _addingObjectValidationRuleCollector.RegisterValidator(validatorFactory);
      _addingObjectValidationRuleCollector.SetCondition(predicate);

      var result = ((IAddingObjectValidationRuleCollector)_addingObjectValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub.Object);

      Assert.That(((ObjectValidationRule<Customer>)result).Condition, Is.SameAs(predicate));
    }

    [Test]
    public void SetCondition ()
    {
      Func<Customer, bool> predicate = _ => true;
      _addingObjectValidationRuleCollector.SetCondition(predicate);

      Assert.That(_addingObjectValidationRuleCollector.Condition, Is.SameAs(predicate));
    }

    [Test]
    public void SetCondition_WithInvalidTypeOfPredicate_ThrowsArgumentException ()
    {
      Func<Person, bool> predicate = _ => true;

      Assert.That(
          () => _addingObjectValidationRuleCollector.SetCondition(predicate),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The type 'Remotion.Validation.UnitTests.TestDomain.Person' of the predicate does not match the "
                  + "type 'Remotion.Validation.UnitTests.TestDomain.Customer' of the validation rule.", "predicate"));
    }

    [Test]
    public void SetCondition_Twice_UsesNewCondition ()
    {
      Func<Customer, bool> predicateOne = _ => true;
      Func<Customer, bool> predicateTwo = _ => false;

      _addingObjectValidationRuleCollector.SetCondition(predicateOne);
      _addingObjectValidationRuleCollector.SetCondition(predicateTwo);

      Assert.That(_addingObjectValidationRuleCollector.Condition, Is.SameAs(predicateTwo));
    }

    [Test]
    public void RegisterValidator ()
    {
      _addingObjectValidationRuleCollector.RegisterValidator(_ => _stubObjectValidator1);
      _addingObjectValidationRuleCollector.RegisterValidator(_ => _stubObjectValidator3);

      Assert.That(_addingObjectValidationRuleCollector.Validators.Count(), Is.EqualTo(2));
      Assert.That(
          _addingObjectValidationRuleCollector.Validators,
          Is.EquivalentTo(new[] { _stubObjectValidator1, _stubObjectValidator3 }));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_IsRemovableTrue ()
    {
      _addingObjectValidationRuleCollector.SetRemovable();
      Assert.That(_addingObjectValidationRuleCollector.IsRemovable, Is.True);
      _addingObjectValidationRuleCollector.RegisterValidator(_ => _stubObjectValidator1);
      _addingObjectValidationRuleCollector.RegisterValidator(_ => _stubObjectValidator2.Object);
      _addingObjectValidationRuleCollector.RegisterValidator(_ => _stubObjectValidator3);
      Assert.That(_addingObjectValidationRuleCollector.Validators.Count(), Is.EqualTo(3));

      _objectValidatorExtractorMock
          .Setup(
              mock => mock.ExtractObjectValidatorsToRemove(_addingObjectValidationRuleCollector))
          .Returns(new[] { _stubObjectValidator1, _stubObjectValidator3 })
          .Verifiable();

      _addingObjectValidationRuleCollector.ApplyRemoveValidatorRegistrations(_objectValidatorExtractorMock.Object);

      _objectValidatorExtractorMock.Verify();
      Assert.That(_addingObjectValidationRuleCollector.Validators, Is.EqualTo(new[] { _stubObjectValidator2.Object }));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_IsRemovableFalseAndNoValidatorsToRemove_NoExceptionIsThrown ()
    {
      Assert.That(_addingObjectValidationRuleCollector.IsRemovable, Is.False);
      _addingObjectValidationRuleCollector.RegisterValidator(_ => _stubObjectValidator1);
      Assert.That(_addingObjectValidationRuleCollector.Validators.Count(), Is.EqualTo(1));

      _objectValidatorExtractorMock
          .Setup(
              stub => stub.ExtractObjectValidatorsToRemove(_addingObjectValidationRuleCollector))
          .Returns(new IObjectValidator[0]);

      _addingObjectValidationRuleCollector.ApplyRemoveValidatorRegistrations(_objectValidatorExtractorMock.Object);

      Assert.That(_addingObjectValidationRuleCollector.Validators.Count(), Is.EqualTo(1));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_IsRemovableFalseAndValidatorsToRemove_ExceptionIsThrown ()
    {
      _addingObjectValidationRuleCollector.RegisterValidator(_ => _stubObjectValidator1);
      _addingObjectValidationRuleCollector.RegisterValidator(_ => _stubObjectValidator2.Object);
      _addingObjectValidationRuleCollector.RegisterValidator(_ => _stubObjectValidator3);
      Assert.That(_addingObjectValidationRuleCollector.Validators.Count(), Is.EqualTo(3));

      _objectValidatorExtractorMock
          .Setup(
              stub => stub.ExtractObjectValidatorsToRemove(_addingObjectValidationRuleCollector))
          .Returns(new[] { _stubObjectValidator1, _stubObjectValidator3 });

      Assert.That(
          () => _addingObjectValidationRuleCollector.ApplyRemoveValidatorRegistrations(_objectValidatorExtractorMock.Object),
          Throws.TypeOf<ValidationConfigurationException>()
              .And.Message.EqualTo(
                  "Attempted to remove non-removable validator(s) 'StubObjectValidator, FakeCustomerValidator' on type "
                  + "'Remotion.Validation.UnitTests.TestDomain.Customer'."));
    }

    [Test]
    public void ToString_NotRemovable ()
    {
      var result = _addingObjectValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo("AddingObjectValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer"));
    }

    [Test]
    public void ToString_IsRemovable ()
    {
      _addingObjectValidationRuleCollector.SetRemovable();
      var result = _addingObjectValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo(
              "AddingObjectValidationRuleCollector (REMOVABLE): Remotion.Validation.UnitTests.TestDomain.Customer"));
    }

    [Test]
    public void ToString_WithoutCondition ()
    {
      var result = _addingObjectValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo(
              "AddingObjectValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer"));
    }

    [Test]
    public void ToString_WithCondition ()
    {
      _addingObjectValidationRuleCollector.SetCondition((Customer o) => true);
      var result = _addingObjectValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo(
              "AddingObjectValidationRuleCollector (CONDITIONAL): Remotion.Validation.UnitTests.TestDomain.Customer"));
    }

    [Test]
    public void ToString_WithConditionAndIsRemovable ()
    {
      _addingObjectValidationRuleCollector.SetCondition((Customer o) => true);
      _addingObjectValidationRuleCollector.SetRemovable();
      var result = _addingObjectValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo(
              "AddingObjectValidationRuleCollector (CONDITIONAL, REMOVABLE): Remotion.Validation.UnitTests.TestDomain.Customer"));
    }
  }
}
