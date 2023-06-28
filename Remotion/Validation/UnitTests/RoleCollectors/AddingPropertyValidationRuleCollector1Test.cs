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
using System.Linq.Expressions;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class AddingPropertyValidationRuleCollector1Test
  {
    private Expression<Func<Customer, string>> _userNameExpression;
    private AddingPropertyValidationRuleCollector<Customer, string> _addingPropertyValidationRuleCollector;
    private IPropertyInformation _property;
    private Mock<IPropertyValidatorExtractor> _propertyValidatorExtractorMock;
    private StubPropertyValidator _stubPropertyValidator1;
    private NotEmptyOrWhitespaceValidator _stubPropertyValidator2;
    private NotEqualValidator _stubPropertyValidator3;

    [SetUp]
    public void SetUp ()
    {
      _property = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("UserName"));

      _userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.UserName);

      _stubPropertyValidator1 = new StubPropertyValidator();
      _stubPropertyValidator2 = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));
      _stubPropertyValidator3 = new NotEqualValidator("gfsf", new InvariantValidationMessage("Fake Message"));

      _propertyValidatorExtractorMock = new Mock<IPropertyValidatorExtractor>(MockBehavior.Strict);

      _addingPropertyValidationRuleCollector = AddingPropertyValidationRuleCollector.Create(_userNameExpression, typeof(CustomerValidationRuleCollector1));
    }

    [Test]
    public void Initialization_PropertyDeclaredInSameClass ()
    {
      var propertyInfo = ((PropertyInfoAdapter)_addingPropertyValidationRuleCollector.Property).PropertyInfo;
      Assert.That(_addingPropertyValidationRuleCollector.Property.Equals(_property), Is.True);
      Assert.That(_addingPropertyValidationRuleCollector.Property, Is.EqualTo(_property));
      Assert.That(propertyInfo.DeclaringType, Is.EqualTo(typeof(Customer)));
      Assert.That(propertyInfo.ReflectedType, Is.EqualTo(typeof(Customer)));
      Assert.That(_addingPropertyValidationRuleCollector.CollectorType, Is.EqualTo(typeof(CustomerValidationRuleCollector1)));
      Assert.That(_addingPropertyValidationRuleCollector.Validators.Any(), Is.False);
      Assert.That(_addingPropertyValidationRuleCollector.IsRemovable, Is.False);
      Assert.That(_addingPropertyValidationRuleCollector.ValidatedType, Is.EqualTo(typeof(Customer)));
    }

    [Test]
    public void Initialization_CollectorTypeDoesNotImplementIValidationRuleCollector_ThrowsArgumentException ()
    {
      Assert.That(
          () => new AddingPropertyValidationRuleCollector<Customer, string>(_property, _ => "", typeof(Customer)),
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
          .Setup(e => e.CreateValidationMessageForPropertyValidator(It.IsAny<NotNullValidator>(), _property))
          .Returns(new InvariantValidationMessage("expectedMessage"));

      Func<PropertyValidationRuleInitializationParameters, IPropertyValidator> validatorFactory = param => new NotNullValidator(param.ValidationMessage);

      _addingPropertyValidationRuleCollector.RegisterValidator(validatorFactory);
      var result = ((IAddingPropertyValidationRuleCollector)_addingPropertyValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub.Object);

      var notNullValidators = ((IPropertyValidationRule)result).Validators.Cast<NotNullValidator>().ToArray();
      Assert.That(notNullValidators.Length, Is.EqualTo(1));
      Assert.That(notNullValidators[0].ValidationMessage.ToString(), Is.EqualTo("expectedMessage"));
    }

    [Test]
    public void CreateValidationRule_WhenCalledTwice_InitializesDeferredInitializationValidationMessagesOnlyForNewlyRegisteredValidators ()
    {
      var validationMessageFactoryStub1 = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub1
          .Setup(e => e.CreateValidationMessageForPropertyValidator(It.IsAny<NotNullValidator>(), _property))
          .Returns(new InvariantValidationMessage("expectedMessage1"));

      var validationMessageFactoryStub2 = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub2
          .Setup(e => e.CreateValidationMessageForPropertyValidator(It.IsAny<NotNullValidator>(), _property))
          .Returns(new InvariantValidationMessage("expectedMessage2"));

      Func<PropertyValidationRuleInitializationParameters, IPropertyValidator> validatorFactory = param => new NotNullValidator(param.ValidationMessage);

      _addingPropertyValidationRuleCollector.RegisterValidator(validatorFactory);
      var result1 = ((IAddingPropertyValidationRuleCollector)_addingPropertyValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub1.Object);

      var notNullValidators1 = ((IPropertyValidationRule)result1).Validators.Cast<NotNullValidator>().ToArray();
      Assert.That(notNullValidators1.Length, Is.EqualTo(1));
      Assert.That(notNullValidators1[0].ValidationMessage.ToString(), Is.EqualTo("expectedMessage1"));

      _addingPropertyValidationRuleCollector.RegisterValidator(validatorFactory);
      var result2 = ((IAddingPropertyValidationRuleCollector)_addingPropertyValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub2.Object);

      var notNullValidators2 = ((IPropertyValidationRule)result2).Validators.Cast<NotNullValidator>().ToArray();
      Assert.That(notNullValidators2.Length, Is.EqualTo(2));
      Assert.That(notNullValidators2[0].ValidationMessage.ToString(), Is.EqualTo("expectedMessage1"));
      Assert.That(notNullValidators2[1].ValidationMessage.ToString(), Is.EqualTo("expectedMessage2"));
    }

    [Test]
    public void CreateValidationRule_IgnoresValidationMessagesForValidatorsWithoutADeferredInitializationValidationMessage ()
    {
      var validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub
          .Setup(e => e.CreateValidationMessageForPropertyValidator(It.IsAny<NotNullValidator>(), _property))
          .Returns(new InvariantValidationMessage("unexpectedMessage"));

      Func<PropertyValidationRuleInitializationParameters, IPropertyValidator> validatorFactory = _ => new NotNullValidator(new InvariantValidationMessage("expectedMessage"));

      _addingPropertyValidationRuleCollector.RegisterValidator(validatorFactory);
      var result = ((IAddingPropertyValidationRuleCollector)_addingPropertyValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub.Object);

      var propertyValidators = ((IPropertyValidationRule)result).Validators.Cast<NotNullValidator>().ToArray();
      Assert.That(propertyValidators.Length, Is.EqualTo(1));
      Assert.That(propertyValidators[0].ValidationMessage.ToString(), Is.EqualTo("expectedMessage"));
    }

    [Test]
    public void CreateValidationRule_WithValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
      var validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(It.IsAny<NotNullValidator>(), _property))
          .Returns((ValidationMessage)null);

      Func<PropertyValidationRuleInitializationParameters, IPropertyValidator> validatorFactory = param => new NotNullValidator(param.ValidationMessage);

      _addingPropertyValidationRuleCollector.RegisterValidator(validatorFactory);

      Assert.That(
          () => ((IAddingPropertyValidationRuleCollector)_addingPropertyValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The IValidationMessageFactory did not return a result for 'NotNullValidator' applied to "
                  + "property 'UserName' on type 'Remotion.Validation.UnitTests.TestDomain.Customer'."));
    }

    [Test]
    public void CreateValidationRule_WithConditionNotNull_InitializesConditionForCreatedPropertyValidationRule ()
    {
      var validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
      validationMessageFactoryStub
          .Setup(e => e.CreateValidationMessageForPropertyValidator(It.IsAny<NotNullValidator>(), _property))
          .Returns(new InvariantValidationMessage("expectedMessage"));

      Func<PropertyValidationRuleInitializationParameters, IPropertyValidator> validatorFactory = param => new NotNullValidator(param.ValidationMessage);
      Func<Customer, bool> predicate = _ => true;

      _addingPropertyValidationRuleCollector.RegisterValidator(validatorFactory);
      _addingPropertyValidationRuleCollector.SetCondition(predicate);

      var result = ((IAddingPropertyValidationRuleCollector)_addingPropertyValidationRuleCollector).CreateValidationRule(validationMessageFactoryStub.Object);

      Assert.That(((PropertyValidationRule<Customer, string>)result).Condition, Is.SameAs(predicate));
    }

    [Test]
    public void SetCondition ()
    {
      Func<Customer, bool> predicate = _ => true;
      _addingPropertyValidationRuleCollector.SetCondition(predicate);

      Assert.That(_addingPropertyValidationRuleCollector.Condition, Is.SameAs(predicate));
    }

    [Test]
    public void SetCondition_WithInvalidTypeOfPredicate_ThrowsArgumentException ()
    {
      Func<Person, bool> predicate = _ => true;

      Assert.That(
          () => _addingPropertyValidationRuleCollector.SetCondition(predicate),
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

      _addingPropertyValidationRuleCollector.SetCondition(predicateOne);
      _addingPropertyValidationRuleCollector.SetCondition(predicateTwo);

      Assert.That(_addingPropertyValidationRuleCollector.Condition, Is.SameAs(predicateTwo));
    }

    [Test]
    public void RegisterValidator ()
    {
      _addingPropertyValidationRuleCollector.RegisterValidator(_ => _stubPropertyValidator1);
      _addingPropertyValidationRuleCollector.RegisterValidator(_ => _stubPropertyValidator2);

      Assert.That(_addingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo(2));
      Assert.That(
          _addingPropertyValidationRuleCollector.Validators,
          Is.EquivalentTo(new IPropertyValidator[] { _stubPropertyValidator1, _stubPropertyValidator2 }));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_IsRemovableTrue ()
    {
      _addingPropertyValidationRuleCollector.SetRemovable();
      Assert.That(_addingPropertyValidationRuleCollector.IsRemovable, Is.True);
      _addingPropertyValidationRuleCollector.RegisterValidator(_ => _stubPropertyValidator1);
      _addingPropertyValidationRuleCollector.RegisterValidator(_ => _stubPropertyValidator2);
      _addingPropertyValidationRuleCollector.RegisterValidator(_ => _stubPropertyValidator3);
      Assert.That(_addingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo(3));

      _propertyValidatorExtractorMock
          .Setup(
              mock => mock.ExtractPropertyValidatorsToRemove(_addingPropertyValidationRuleCollector))
          .Returns(new IPropertyValidator[] { _stubPropertyValidator1, _stubPropertyValidator3 })
          .Verifiable();

      _addingPropertyValidationRuleCollector.ApplyRemoveValidatorRegistrations(_propertyValidatorExtractorMock.Object);

      _propertyValidatorExtractorMock.Verify();
      Assert.That(_addingPropertyValidationRuleCollector.Validators, Is.EqualTo(new[] { _stubPropertyValidator2 }));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_IsRemovableFalseAndNoValidatorsToRemove_NoExceptionIsThrown ()
    {
      Assert.That(_addingPropertyValidationRuleCollector.IsRemovable, Is.False);
      _addingPropertyValidationRuleCollector.RegisterValidator(_ => _stubPropertyValidator1);
      Assert.That(_addingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo(1));

      _propertyValidatorExtractorMock
          .Setup(
              stub => stub.ExtractPropertyValidatorsToRemove(_addingPropertyValidationRuleCollector))
          .Returns(new IPropertyValidator[0]);

      _addingPropertyValidationRuleCollector.ApplyRemoveValidatorRegistrations(_propertyValidatorExtractorMock.Object);

      Assert.That(_addingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo(1));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_IsRemovableFalseAndValidatorsToRemove_ExceptionIsThrown ()
    {
      _addingPropertyValidationRuleCollector.RegisterValidator(_ => _stubPropertyValidator1);
      _addingPropertyValidationRuleCollector.RegisterValidator(_ => _stubPropertyValidator2);
      _addingPropertyValidationRuleCollector.RegisterValidator(_ => _stubPropertyValidator3);
      Assert.That(_addingPropertyValidationRuleCollector.Validators.Count(), Is.EqualTo(3));

      _propertyValidatorExtractorMock
          .Setup(
              stub => stub.ExtractPropertyValidatorsToRemove(_addingPropertyValidationRuleCollector))
          .Returns(new IPropertyValidator[] { _stubPropertyValidator1, _stubPropertyValidator3 });

      Assert.That(
          () => _addingPropertyValidationRuleCollector.ApplyRemoveValidatorRegistrations(_propertyValidatorExtractorMock.Object),
          Throws.TypeOf<ValidationConfigurationException>().And.Message.EqualTo(
              "Attempted to remove non-removable validator(s) 'StubPropertyValidator, NotEqualValidator' on property "
              + "'Remotion.Validation.UnitTests.TestDomain.Customer.UserName'."));
    }

    [Test]
    public void ToString_NotRemovable ()
    {
      var result = _addingPropertyValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo("AddingPropertyValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_IsRemovable ()
    {
      _addingPropertyValidationRuleCollector.SetRemovable();
      var result = _addingPropertyValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo(
              "AddingPropertyValidationRuleCollector (REMOVABLE): Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_WithoutCondition ()
    {
      var result = _addingPropertyValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo(
              "AddingPropertyValidationRuleCollector: Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_WithCondition ()
    {
      _addingPropertyValidationRuleCollector.SetCondition((Customer o) => true);
      var result = _addingPropertyValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo(
              "AddingPropertyValidationRuleCollector (CONDITIONAL): Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_WithConditionAndIsRemovable ()
    {
      _addingPropertyValidationRuleCollector.SetCondition((Customer o) => true);
      _addingPropertyValidationRuleCollector.SetRemovable();
      var result = _addingPropertyValidationRuleCollector.ToString();

      Assert.That(
          result,
          Is.EqualTo(
              "AddingPropertyValidationRuleCollector (CONDITIONAL, REMOVABLE): Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }
  }
}
