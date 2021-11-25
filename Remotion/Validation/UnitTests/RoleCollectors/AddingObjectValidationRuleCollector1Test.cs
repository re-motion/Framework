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
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.RuleCollectors;
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
    private IAddingObjectValidationRuleCollector _addingObjectValidationRuleCollector;
    private Mock<IObjectValidatorExtractor> _objectValidatorExtractorMock;
    private IObjectValidator _stubObjectValidator1;
    private Mock<IObjectValidator> _stubObjectValidator2;
    private IObjectValidator _stubObjectValidator3;

    [SetUp]
    public void SetUp ()
    {
      _addingObjectValidationRuleCollector = AddingObjectValidationRuleCollector.Create<Customer>(typeof(CustomerValidationRuleCollector1));
      _objectValidatorExtractorMock = new Mock<IObjectValidatorExtractor>();
      _stubObjectValidator1 = new StubObjectValidator();
      _stubObjectValidator2 = new Mock<IObjectValidator>();
      _stubObjectValidator3 = new FakeCustomerValidator();
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void Initialization_CollectorTypeDoesNotImplementIValidationRuleCollector_ThrowsArgumentException ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_InitializesDeferredInitializationValidationMessages ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_WhenCalledTwice_InitializesDeferredInitializationValidationMessagesOnlyForNewlyRegisteredValidators ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_IgnoresValidationMessagesForValidatorsWithoutADeferredInitializationValidationMessage ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_WithValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationRule_WithConditionNotNull_InitializesConditionForCreatedObjectValidationRule ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void SetCondition ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void SetCondition_Twice_UsesNewCondition ()
    {
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
