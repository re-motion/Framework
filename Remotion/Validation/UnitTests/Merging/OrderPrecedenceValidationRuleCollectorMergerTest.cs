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
using Moq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Providers;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class OrderPrecedenceValidationRuleCollectorMergerTest
  {
    private ValidationRuleCollectorMergerBase _merger;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollector1;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector1;
    private Mock<IValidationRuleCollector> _validationRuleCollectorStub1;
    private Mock<IValidationRuleCollector> _validationRuleCollectorStub2;
    private Expression<Func<Customer, string>> _firstNameExpression;
    private Expression<Func<Customer, string>> _lastNameExpression;
    private NotNullValidator _notNullValidator;
    private NotEmptyOrWhitespaceValidator _notEmptyOrWhitespaceValidator;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector2;
    private NotEqualValidator _notEqualValidator;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector3;
    private MaximumLengthValidator _maximumLengthValidator;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollector2;
    private MinimumLengthValidator _minimumLengthValidator;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollector3;
    private Mock<IValidationRuleCollector> _validationRuleCollectorStub3;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector4;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollector4;
    private Mock<IPropertyValidatorExtractorFactory> _propertyValidatorExtractorFactoryMock;
    private Mock<IPropertyValidatorExtractor> _propertyValidatorExtractorMock;
    private Mock<IObjectValidatorExtractorFactory> _objectValidatorExtractorFactoryMock;
    private Mock<IObjectValidatorExtractor> _objectValidatorExtractorMock;

    [SetUp]
    public void SetUp ()
    {
      _notEmptyOrWhitespaceValidator = new NotEmptyOrWhitespaceValidator(new InvariantValidationMessage("Fake Message"));
      _notNullValidator = new NotNullValidator(new InvariantValidationMessage("Fake Message"));
      _notEqualValidator = new NotEqualValidator("test", new InvariantValidationMessage("Fake Message"));
      _maximumLengthValidator = new MaximumLengthValidator(30, new InvariantValidationMessage("Fake Message"));
      _minimumLengthValidator = new MinimumLengthValidator(5, new InvariantValidationMessage("Fake Message"));

      _validationRuleCollectorStub1 = new Mock<IValidationRuleCollector>();
      _validationRuleCollectorStub2 = new Mock<IValidationRuleCollector>();
      _validationRuleCollectorStub3 = new Mock<IValidationRuleCollector>();

      _firstNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.FirstName);
      _lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.LastName);

      _addingPropertyValidationRuleCollector1 = AddingPropertyValidationRuleCollector.Create(_firstNameExpression, _validationRuleCollectorStub1.Object.GetType());
      _addingPropertyValidationRuleCollector1.SetRemovable();
      _addingPropertyValidationRuleCollector1.RegisterValidator(_ => _notEmptyOrWhitespaceValidator);
      _addingPropertyValidationRuleCollector1.RegisterValidator(_ => _notNullValidator);
      _addingPropertyValidationRuleCollector1.RegisterValidator(_ => _notEqualValidator);

      _addingPropertyValidationRuleCollector2 = AddingPropertyValidationRuleCollector.Create(_lastNameExpression, _validationRuleCollectorStub2.Object.GetType());
      _addingPropertyValidationRuleCollector2.SetRemovable();
      _addingPropertyValidationRuleCollector2.RegisterValidator(_ => _maximumLengthValidator);

      _addingPropertyValidationRuleCollector3 = AddingPropertyValidationRuleCollector.Create(_lastNameExpression, _validationRuleCollectorStub2.Object.GetType());
      _addingPropertyValidationRuleCollector3.SetRemovable();
      _addingPropertyValidationRuleCollector3.RegisterValidator(_ => _minimumLengthValidator);

      _addingPropertyValidationRuleCollector4 = AddingPropertyValidationRuleCollector.Create(_lastNameExpression, _validationRuleCollectorStub2.Object.GetType());
      _addingPropertyValidationRuleCollector4.SetRemovable();
      _addingPropertyValidationRuleCollector4.RegisterValidator(_ => _notNullValidator);

      _removingPropertyValidationRuleCollector1 = RemovingPropertyValidationRuleCollector.Create(_firstNameExpression, typeof(CustomerValidationRuleCollector1));
      _removingPropertyValidationRuleCollector1.RegisterValidator(typeof(NotEmptyOrWhitespaceValidator), null, null);

      _removingPropertyValidationRuleCollector2 = RemovingPropertyValidationRuleCollector.Create(_firstNameExpression, typeof(CustomerValidationRuleCollector1));
      _removingPropertyValidationRuleCollector2.RegisterValidator(typeof(NotNullValidator), _validationRuleCollectorStub1.Object.GetType(), null);

      _removingPropertyValidationRuleCollector3 = RemovingPropertyValidationRuleCollector.Create(_firstNameExpression, typeof(CustomerValidationRuleCollector1));
      _removingPropertyValidationRuleCollector3.RegisterValidator(typeof(NotNullValidator), _validationRuleCollectorStub1.Object.GetType(), null);

      _removingPropertyValidationRuleCollector4 = RemovingPropertyValidationRuleCollector.Create(_lastNameExpression, typeof(CustomerValidationRuleCollector1));
      _removingPropertyValidationRuleCollector4.RegisterValidator(typeof(MaximumLengthValidator), null, null);

      _propertyValidatorExtractorFactoryMock = new Mock<IPropertyValidatorExtractorFactory>(MockBehavior.Strict);
      _propertyValidatorExtractorMock = new Mock<IPropertyValidatorExtractor>(MockBehavior.Strict);

      _objectValidatorExtractorFactoryMock = new Mock<IObjectValidatorExtractorFactory>(MockBehavior.Strict);
      _objectValidatorExtractorMock = new Mock<IObjectValidatorExtractor>(MockBehavior.Strict);

      _merger = new OrderPrecedenceValidationRuleCollectorMerger(_propertyValidatorExtractorFactoryMock.Object, _objectValidatorExtractorFactoryMock.Object);
    }

    [Test]
    public void Merge_RemovePropertyValidatorWithNoCollectorTypeDefined_ValidatorIsRemoved ()
    {
      _validationRuleCollectorStub1.Setup(stub => stub.AddedPropertyRules).Returns(new[] { _addingPropertyValidationRuleCollector1 });
      _validationRuleCollectorStub1.Setup(stub => stub.RemovedPropertyRules).Returns(new IRemovingPropertyValidationRuleCollector[0]);

      _validationRuleCollectorStub2.Setup(stub => stub.AddedPropertyRules).Returns(new[] { _addingPropertyValidationRuleCollector2, _addingPropertyValidationRuleCollector3 });
      _validationRuleCollectorStub2.Setup(stub => stub.RemovedPropertyRules).Returns(new[] { _removingPropertyValidationRuleCollector1 });

      _validationRuleCollectorStub1.Setup(stub => stub.AddedObjectRules).Returns(new IAddingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Setup(stub => stub.AddedObjectRules).Returns(new IAddingObjectValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Setup(stub => stub.RemovedObjectRules).Returns(new IRemovingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Setup(stub => stub.RemovedObjectRules).Returns(new IRemovingObjectValidationRuleCollector[0]);

      _propertyValidatorExtractorFactoryMock
          .Setup(
              mock =>
                  mock.Create(
                      It.Is<IEnumerable<RemovingPropertyValidatorRegistration>>(
                          c => c.Count() == 1 && c.ToArray()[0].ValidatorType == typeof(NotEmptyOrWhitespaceValidator)),
                      It.IsNotNull<ILogContext>()))
          .Returns(_propertyValidatorExtractorMock.Object)
          .Verifiable();

      _propertyValidatorExtractorMock
          .Setup(mock => mock.ExtractPropertyValidatorsToRemove(_addingPropertyValidationRuleCollector1))
          .Returns(new[] { _notEmptyOrWhitespaceValidator })
          .Verifiable();

      var result =
          _merger.Merge(
              new[]
              {
                  new[] { new ValidationRuleCollectorInfo(_validationRuleCollectorStub1.Object, typeof(ApiBasedValidationRuleCollectorProvider)) },
                  new[] { new ValidationRuleCollectorInfo(_validationRuleCollectorStub2.Object, typeof(ApiBasedValidationRuleCollectorProvider)) }
              }).CollectedPropertyValidationRules.ToArray();

      _propertyValidatorExtractorFactoryMock.Verify();
      _propertyValidatorExtractorMock.Verify();
      Assert.That(result.Count(), Is.EqualTo(3));
      Assert.That(result[0].Validators, Is.EquivalentTo(new IPropertyValidator[] { _notNullValidator, _notEqualValidator }));
      Assert.That(result[1].Validators, Is.EquivalentTo(new IPropertyValidator[] { _maximumLengthValidator }));
      Assert.That(result[2].Validators, Is.EquivalentTo(new IPropertyValidator[] { _minimumLengthValidator }));
    }

    [Test]
    public void Merge_RemovePropertyValidatorWithAlreadyRegisteredCollectorTypeDefined_ValidatorIsRemoved ()
    {
      _validationRuleCollectorStub1.Setup(stub => stub.AddedPropertyRules).Returns(new[] { _addingPropertyValidationRuleCollector1 });
      _validationRuleCollectorStub2.Setup(stub => stub.AddedPropertyRules).Returns(new IAddingPropertyValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Setup(stub => stub.RemovedPropertyRules).Returns(new IRemovingPropertyValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Setup(stub => stub.RemovedPropertyRules).Returns(new[] { _removingPropertyValidationRuleCollector2 });

      _validationRuleCollectorStub1.Setup(stub => stub.AddedObjectRules).Returns(new IAddingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Setup(stub => stub.AddedObjectRules).Returns(new IAddingObjectValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Setup(stub => stub.RemovedObjectRules).Returns(new IRemovingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Setup(stub => stub.RemovedObjectRules).Returns(new IRemovingObjectValidationRuleCollector[0]);

      _propertyValidatorExtractorFactoryMock
          .Setup(
              mock =>
                  mock.Create(
                      It.Is<IEnumerable<RemovingPropertyValidatorRegistration>>(
                          c => c.Count() == 1 && c.ToArray()[0].ValidatorType == typeof(NotNullValidator)),
                      It.IsNotNull<ILogContext>()))
          .Returns(_propertyValidatorExtractorMock.Object)
          .Verifiable();

      _propertyValidatorExtractorMock
          .Setup(mock => mock.ExtractPropertyValidatorsToRemove(_addingPropertyValidationRuleCollector1))
          .Returns(new[] { _notNullValidator })
          .Verifiable();

      var result =
          _merger.Merge(
              new[]
              {
                  new[] { new ValidationRuleCollectorInfo(_validationRuleCollectorStub1.Object, typeof(ApiBasedValidationRuleCollectorProvider)) },
                  new[] { new ValidationRuleCollectorInfo(_validationRuleCollectorStub2.Object, typeof(ApiBasedValidationRuleCollectorProvider)) }
              }).CollectedPropertyValidationRules.ToArray();

      _propertyValidatorExtractorFactoryMock.Verify();
      _propertyValidatorExtractorMock.Verify();
      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result[0].Validators, Is.EquivalentTo(new IPropertyValidator[] { _notEmptyOrWhitespaceValidator, _notEqualValidator }));
    }

    [Test]
    public void Merge_RemovePropertyValidator_AllRemoveRulesInSameGroupAreAppliedBeforeAllAddedRulesInSameGroup ()
    {
      _validationRuleCollectorStub1
          .Setup(stub => stub.AddedPropertyRules)
          .Returns(new[] { _addingPropertyValidationRuleCollector1, _addingPropertyValidationRuleCollector2 });
      _validationRuleCollectorStub2
          .Setup(stub => stub.AddedPropertyRules)
          .Returns(new[] { _addingPropertyValidationRuleCollector3 });
      _validationRuleCollectorStub3
          .Setup(stub => stub.AddedPropertyRules)
          .Returns(new[] { _addingPropertyValidationRuleCollector4 });

      _validationRuleCollectorStub1
          .Setup(stub => stub.RemovedPropertyRules)
          .Returns(new IRemovingPropertyValidationRuleCollector[0]);
      _validationRuleCollectorStub2
          .Setup(stub => stub.RemovedPropertyRules)
          .Returns(new[] { _removingPropertyValidationRuleCollector1, _removingPropertyValidationRuleCollector4 });
      _validationRuleCollectorStub3
          .Setup(stub => stub.RemovedPropertyRules)
          .Returns(new[] { _removingPropertyValidationRuleCollector2 });

      _validationRuleCollectorStub1
          .Setup(stub => stub.AddedObjectRules)
          .Returns(new IAddingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2
          .Setup(stub => stub.AddedObjectRules)
          .Returns(new IAddingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub3
          .Setup(stub => stub.AddedObjectRules)
          .Returns(new IAddingObjectValidationRuleCollector[0]);

      _validationRuleCollectorStub1
          .Setup(stub => stub.RemovedObjectRules)
          .Returns(new IRemovingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2
          .Setup(stub => stub.RemovedObjectRules)
          .Returns(new IRemovingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub3
          .Setup(stub => stub.RemovedObjectRules)
          .Returns(new IRemovingObjectValidationRuleCollector[0]);

      _propertyValidatorExtractorFactoryMock
          .Setup(mock => mock.Create(It.IsAny<IEnumerable<RemovingPropertyValidatorRegistration>>(), It.IsNotNull<ILogContext>()))
          .Returns(_propertyValidatorExtractorMock.Object)
          .Verifiable();

      _propertyValidatorExtractorMock
          .Setup(mock => mock.ExtractPropertyValidatorsToRemove(_addingPropertyValidationRuleCollector1))
          .Returns(new IPropertyValidator[] { _notEmptyOrWhitespaceValidator, _notNullValidator })
          .Verifiable();
      _propertyValidatorExtractorMock
          .Setup(mock => mock.ExtractPropertyValidatorsToRemove(_addingPropertyValidationRuleCollector2))
          .Returns(new[] { _maximumLengthValidator })
          .Verifiable();

      var result =
          _merger.Merge(
              new[]
              {
                  new[] { new ValidationRuleCollectorInfo(_validationRuleCollectorStub1.Object, typeof(ApiBasedValidationRuleCollectorProvider)) },
                  new[]
                  {
                      new ValidationRuleCollectorInfo(_validationRuleCollectorStub2.Object, typeof(ApiBasedValidationRuleCollectorProvider)),
                      new ValidationRuleCollectorInfo(_validationRuleCollectorStub3.Object, typeof(ApiBasedValidationRuleCollectorProvider))
                  }
              }).CollectedPropertyValidationRules.ToArray();

      _propertyValidatorExtractorFactoryMock.Verify();
      _propertyValidatorExtractorMock.Verify(mock => mock.ExtractPropertyValidatorsToRemove(_addingPropertyValidationRuleCollector2), Times.Once());
      Assert.That(result.Count(), Is.EqualTo(4));
      Assert.That(result[0].Validators, Is.EquivalentTo(new IPropertyValidator[] { _notEqualValidator }));
      Assert.That(result[1].Validators.Any(), Is.False);
      Assert.That(result[2].Validators, Is.EquivalentTo(new IPropertyValidator[] { _minimumLengthValidator }));
      Assert.That(result[3].Validators, Is.EquivalentTo(new IPropertyValidator[] { _notNullValidator }));
    }

    [Test]
    public void Merge_RemovePropertyValidator_NoPropertyValidatorsToRemove_NoValidatorIsRemoved ()
    {
      _validationRuleCollectorStub1.Setup(stub => stub.AddedPropertyRules).Returns(new[] { _addingPropertyValidationRuleCollector1 });
      _validationRuleCollectorStub2.Setup(stub => stub.AddedPropertyRules).Returns(new IAddingPropertyValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Setup(stub => stub.RemovedPropertyRules).Returns(new IRemovingPropertyValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Setup(stub => stub.RemovedPropertyRules).Returns(new[] { _removingPropertyValidationRuleCollector3 });

      _validationRuleCollectorStub1.Setup(stub => stub.AddedObjectRules).Returns(new IAddingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Setup(stub => stub.AddedObjectRules).Returns(new IAddingObjectValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Setup(stub => stub.RemovedObjectRules).Returns(new IRemovingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Setup(stub => stub.RemovedObjectRules).Returns(new IRemovingObjectValidationRuleCollector[0]);

      _propertyValidatorExtractorFactoryMock
          .Setup(mock => mock.Create(It.IsAny<IEnumerable<RemovingPropertyValidatorRegistration>>(), It.IsNotNull<ILogContext>()))
          .Returns(_propertyValidatorExtractorMock.Object)
          .Verifiable();

      _propertyValidatorExtractorMock
          .Setup(mock => mock.ExtractPropertyValidatorsToRemove(_addingPropertyValidationRuleCollector1))
          .Returns(new IPropertyValidator[0])
          .Verifiable();

      var result =
          _merger.Merge(
              new[]
              {
                  new[] { new ValidationRuleCollectorInfo(_validationRuleCollectorStub1.Object, typeof(ApiBasedValidationRuleCollectorProvider)) },
                  new[] { new ValidationRuleCollectorInfo(_validationRuleCollectorStub2.Object, typeof(ApiBasedValidationRuleCollectorProvider)) }
              }).CollectedPropertyValidationRules.ToArray();

      _propertyValidatorExtractorFactoryMock.Verify();
      _propertyValidatorExtractorMock.Verify();
      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result[0].Validators, Is.EquivalentTo(new IPropertyValidator[] { _notEmptyOrWhitespaceValidator, _notNullValidator, _notEqualValidator }));
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void Merge_RemoveObjectValidatorWithNoCollectorTypeDefined_ValidatorIsRemoved ()
    {
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void Merge_RemoveObjectValidatorWithAlreadyRegisteredCollectorTypeDefined_ValidatorIsRemoved ()
    {
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void Merge_RemoveObjectValidator_AllRemoveRulesInSameGroupAreAppliedBeforeAllAddedRulesInSameGroup ()
    {
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void Merge_RemoveObjectValidator_NoPropertyValidatorsToRemove_NoValidatorIsRemoved ()
    {
    }
  }
}
