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
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Providers;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class OrderPrecedenceValidationRuleCollectorMergerTest
  {
    private ValidationRuleCollectorMergerBase _merger;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollector1;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector1;
    private IValidationRuleCollector _validationRuleCollectorStub1;
    private IValidationRuleCollector _validationRuleCollectorStub2;
    private Expression<Func<Customer, string>> _firstNameExpression;
    private Expression<Func<Customer, string>> _lastNameExpression;
    private NotNullValidator _notNullValidator;
    private NotEmptyValidator _notEmptyValidator;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector2;
    private NotEqualValidator _notEqualValidator;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector3;
    private MaximumLengthValidator _maximumLengthValidator;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollector2;
    private LengthValidator _minimumLengthValidator;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollector3;
    private IValidationRuleCollector _validationRuleCollectorStub3;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollector4;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollector4;
    private IPropertyValidatorExtractorFactory _propertyValidatorExtractorFactoryMock;
    private IPropertyValidatorExtractor _propertyValidatorExtractorMock;
    private IObjectValidatorExtractorFactory _objectValidatorExtractorFactoryMock;
    private IObjectValidatorExtractor _objectValidatorExtractorMock;

    [SetUp]
    public void SetUp ()
    {
      _notEmptyValidator = new NotEmptyValidator (new InvariantValidationMessage ("Fake Message"));
      _notNullValidator = new NotNullValidator (new InvariantValidationMessage ("Fake Message"));
      _notEqualValidator = new NotEqualValidator ("test", new InvariantValidationMessage ("Fake Message"));
      _maximumLengthValidator = new MaximumLengthValidator (30, new InvariantValidationMessage ("Fake Message"));
      _minimumLengthValidator = new MinimumLengthValidator (5, new InvariantValidationMessage ("Fake Message"));

      _validationRuleCollectorStub1 = MockRepository.GenerateStub<IValidationRuleCollector>();
      _validationRuleCollectorStub2 = MockRepository.GenerateStub<IValidationRuleCollector>();
      _validationRuleCollectorStub3 = MockRepository.GenerateStub<IValidationRuleCollector>();

      _firstNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.FirstName);
      _lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName);

      _addingPropertyValidationRuleCollector1 = AddingPropertyValidationRuleCollector.Create (_firstNameExpression, _validationRuleCollectorStub1.GetType());
      _addingPropertyValidationRuleCollector1.SetRemovable();
      _addingPropertyValidationRuleCollector1.RegisterValidator (_ => _notEmptyValidator);
      _addingPropertyValidationRuleCollector1.RegisterValidator (_ => _notNullValidator);
      _addingPropertyValidationRuleCollector1.RegisterValidator (_ => _notEqualValidator);

      _addingPropertyValidationRuleCollector2 = AddingPropertyValidationRuleCollector.Create (_lastNameExpression, _validationRuleCollectorStub2.GetType());
      _addingPropertyValidationRuleCollector2.SetRemovable();
      _addingPropertyValidationRuleCollector2.RegisterValidator (_ => _maximumLengthValidator);

      _addingPropertyValidationRuleCollector3 = AddingPropertyValidationRuleCollector.Create (_lastNameExpression, _validationRuleCollectorStub2.GetType());
      _addingPropertyValidationRuleCollector3.SetRemovable();
      _addingPropertyValidationRuleCollector3.RegisterValidator (_ => _minimumLengthValidator);

      _addingPropertyValidationRuleCollector4 = AddingPropertyValidationRuleCollector.Create (_lastNameExpression, _validationRuleCollectorStub2.GetType());
      _addingPropertyValidationRuleCollector4.SetRemovable();
      _addingPropertyValidationRuleCollector4.RegisterValidator (_ => _notNullValidator);

      _removingPropertyValidationRuleCollector1 = RemovingPropertyValidationRuleCollector.Create (_firstNameExpression, typeof (CustomerValidationRuleCollector1));
      _removingPropertyValidationRuleCollector1.RegisterValidator (typeof (NotEmptyValidator));

      _removingPropertyValidationRuleCollector2 = RemovingPropertyValidationRuleCollector.Create (_firstNameExpression, typeof (CustomerValidationRuleCollector1));
      _removingPropertyValidationRuleCollector2.RegisterValidator (typeof (NotNullValidator), _validationRuleCollectorStub1.GetType());

      _removingPropertyValidationRuleCollector3 = RemovingPropertyValidationRuleCollector.Create (_firstNameExpression, typeof (CustomerValidationRuleCollector1));
      _removingPropertyValidationRuleCollector3.RegisterValidator (typeof (NotNullValidator), typeof (string)); //Unknown collector type!

      _removingPropertyValidationRuleCollector4 = RemovingPropertyValidationRuleCollector.Create (_lastNameExpression, typeof (CustomerValidationRuleCollector1));
      _removingPropertyValidationRuleCollector4.RegisterValidator (typeof (MaximumLengthValidator));

      _propertyValidatorExtractorFactoryMock = MockRepository.GenerateStrictMock<IPropertyValidatorExtractorFactory>();
      _propertyValidatorExtractorMock = MockRepository.GenerateStrictMock<IPropertyValidatorExtractor>();

      _objectValidatorExtractorFactoryMock = MockRepository.GenerateStrictMock<IObjectValidatorExtractorFactory>();
      _objectValidatorExtractorMock = MockRepository.GenerateStrictMock<IObjectValidatorExtractor>();

      _merger = new OrderPrecedenceValidationRuleCollectorMerger (_propertyValidatorExtractorFactoryMock, _objectValidatorExtractorFactoryMock);
    }

    [Test]
    public void Merge_RemovePropertyValidatorWithNoCollectorTypeDefined_ValidatorIsRemoved ()
    {
      _validationRuleCollectorStub1.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyValidationRuleCollector1 });
      _validationRuleCollectorStub1.Stub (stub => stub.RemovedPropertyRules).Return (new IRemovingPropertyValidationRuleCollector[0]);

      _validationRuleCollectorStub2.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyValidationRuleCollector2, _addingPropertyValidationRuleCollector3 });
      _validationRuleCollectorStub2.Stub (stub => stub.RemovedPropertyRules).Return (new[] { _removingPropertyValidationRuleCollector1 });

      _validationRuleCollectorStub1.Stub (stub => stub.AddedObjectRules).Return (new IAddingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.AddedObjectRules).Return (new IAddingObjectValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Stub (stub => stub.RemovedObjectRules).Return (new IRemovingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.RemovedObjectRules).Return (new IRemovingObjectValidationRuleCollector[0]);

      _propertyValidatorExtractorFactoryMock
          .Expect (
              mock =>
                  mock.Create (
                      Arg<IEnumerable<PropertyValidatorRegistrationWithContext>>.Matches (
                          c => c.Count() == 1 && c.ToArray()[0].ValidatorRegistration.ValidatorType == typeof (NotEmptyValidator)),
                      Arg<ILogContext>.Is.NotNull))
          .Return (_propertyValidatorExtractorMock);

      _propertyValidatorExtractorMock
          .Expect (mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyValidationRuleCollector1))
          .Return (new[] { _notEmptyValidator });

      var result =
          _merger.Merge (
              new[]
              {
                  new[] { new ValidationRuleCollectorInfo (_validationRuleCollectorStub1, typeof (ApiBasedValidationRuleCollectorProvider)) },
                  new[] { new ValidationRuleCollectorInfo (_validationRuleCollectorStub2, typeof (ApiBasedValidationRuleCollectorProvider)) }
              }).CollectedPropertyValidationRules.ToArray();

      _propertyValidatorExtractorFactoryMock.VerifyAllExpectations();
      _propertyValidatorExtractorMock.VerifyAllExpectations();
      Assert.That (result.Count(), Is.EqualTo (3));
      Assert.That (result[0].Validators, Is.EquivalentTo (new IPropertyValidator[] { _notNullValidator, _notEqualValidator }));
      Assert.That (result[1].Validators, Is.EquivalentTo (new IPropertyValidator[] { _maximumLengthValidator }));
      Assert.That (result[2].Validators, Is.EquivalentTo (new IPropertyValidator[] { _minimumLengthValidator }));
    }

    [Test]
    public void Merge_RemovePropertyValidatorWithAlreadyRegisteredCollectorTypeDefined_ValidatorIsRemoved ()
    {
      _validationRuleCollectorStub1.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyValidationRuleCollector1 });
      _validationRuleCollectorStub2.Stub (stub => stub.AddedPropertyRules).Return (new IAddingPropertyValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Stub (stub => stub.RemovedPropertyRules).Return (new IRemovingPropertyValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.RemovedPropertyRules).Return (new[] { _removingPropertyValidationRuleCollector2 });

      _validationRuleCollectorStub1.Stub (stub => stub.AddedObjectRules).Return (new IAddingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.AddedObjectRules).Return (new IAddingObjectValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Stub (stub => stub.RemovedObjectRules).Return (new IRemovingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.RemovedObjectRules).Return (new IRemovingObjectValidationRuleCollector[0]);

      _propertyValidatorExtractorFactoryMock
          .Expect (
              mock =>
                  mock.Create (
                      Arg<IEnumerable<PropertyValidatorRegistrationWithContext>>.Matches (
                          c => c.Count() == 1 && c.ToArray()[0].ValidatorRegistration.ValidatorType == typeof (NotNullValidator)),
                      Arg<ILogContext>.Is.NotNull))
          .Return (_propertyValidatorExtractorMock);

      _propertyValidatorExtractorMock
          .Expect (mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyValidationRuleCollector1))
          .Return (new[] { _notNullValidator });

      var result =
          _merger.Merge (
              new[]
              {
                  new[] { new ValidationRuleCollectorInfo (_validationRuleCollectorStub1, typeof (ApiBasedValidationRuleCollectorProvider)) },
                  new[] { new ValidationRuleCollectorInfo (_validationRuleCollectorStub2, typeof (ApiBasedValidationRuleCollectorProvider)) }
              }).CollectedPropertyValidationRules.ToArray();

      _propertyValidatorExtractorFactoryMock.VerifyAllExpectations();
      _propertyValidatorExtractorMock.VerifyAllExpectations();
      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0].Validators, Is.EquivalentTo (new IPropertyValidator[] { _notEmptyValidator, _notEqualValidator }));
    }

    [Test]
    public void Merge_RemovePropertyValidator_AllRemoveRulesInSameGroupAreAppliedBeforeAllAddedRulesInSameGroup ()
    {
      _validationRuleCollectorStub1.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyValidationRuleCollector1, _addingPropertyValidationRuleCollector2 });
      _validationRuleCollectorStub2.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyValidationRuleCollector3 });
      _validationRuleCollectorStub3.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyValidationRuleCollector4 });

      _validationRuleCollectorStub1.Stub (stub => stub.RemovedPropertyRules).Return (new IRemovingPropertyValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.RemovedPropertyRules).Return (new[] { _removingPropertyValidationRuleCollector1, _removingPropertyValidationRuleCollector4 });
      _validationRuleCollectorStub3.Stub (stub => stub.RemovedPropertyRules).Return (new[] { _removingPropertyValidationRuleCollector2 });

      _validationRuleCollectorStub1.Stub (stub => stub.AddedObjectRules).Return (new IAddingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.AddedObjectRules).Return (new IAddingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub3.Stub (stub => stub.AddedObjectRules).Return (new IAddingObjectValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Stub (stub => stub.RemovedObjectRules).Return (new IRemovingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.RemovedObjectRules).Return (new IRemovingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub3.Stub (stub => stub.RemovedObjectRules).Return (new IRemovingObjectValidationRuleCollector[0]);

      _propertyValidatorExtractorFactoryMock
          .Expect (mock => mock.Create (Arg<IEnumerable<PropertyValidatorRegistrationWithContext>>.Is.Anything, Arg<ILogContext>.Is.NotNull))
          .Return (_propertyValidatorExtractorMock);

      _propertyValidatorExtractorMock
          .Expect (mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyValidationRuleCollector1))
          .Return (new IPropertyValidator[] { _notEmptyValidator, _notNullValidator }).Repeat.Once();
      _propertyValidatorExtractorMock
          .Expect (mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyValidationRuleCollector2))
          .Return (new[] { _maximumLengthValidator }).Repeat.Once();

      var result =
          _merger.Merge (
              new[]
              {
                  new[] { new ValidationRuleCollectorInfo (_validationRuleCollectorStub1, typeof (ApiBasedValidationRuleCollectorProvider)) },
                  new[]
                  {
                      new ValidationRuleCollectorInfo (_validationRuleCollectorStub2, typeof (ApiBasedValidationRuleCollectorProvider)),
                      new ValidationRuleCollectorInfo (_validationRuleCollectorStub3, typeof (ApiBasedValidationRuleCollectorProvider))
                  }
              }).CollectedPropertyValidationRules.ToArray();

      _propertyValidatorExtractorFactoryMock.VerifyAllExpectations();
      _propertyValidatorExtractorMock.VerifyAllExpectations();
      Assert.That (result.Count(), Is.EqualTo (4));
      Assert.That (result[0].Validators, Is.EquivalentTo (new IPropertyValidator[] { _notEqualValidator }));
      Assert.That (result[1].Validators.Any(), Is.False);
      Assert.That (result[2].Validators, Is.EquivalentTo (new IPropertyValidator[] { _minimumLengthValidator }));
      Assert.That (result[3].Validators, Is.EquivalentTo (new IPropertyValidator[] { _notNullValidator }));
    }

    [Test]
    public void Merge_RemovePropertyValidator_NoPropertyValidatorsToRemove_NoValidatorIsRemoved ()
    {
      _validationRuleCollectorStub1.Stub (stub => stub.AddedPropertyRules).Return (new[] { _addingPropertyValidationRuleCollector1 });
      _validationRuleCollectorStub2.Stub (stub => stub.AddedPropertyRules).Return (new IAddingPropertyValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Stub (stub => stub.RemovedPropertyRules).Return (new IRemovingPropertyValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.RemovedPropertyRules).Return (new[] { _removingPropertyValidationRuleCollector3 });

      _validationRuleCollectorStub1.Stub (stub => stub.AddedObjectRules).Return (new IAddingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.AddedObjectRules).Return (new IAddingObjectValidationRuleCollector[0]);

      _validationRuleCollectorStub1.Stub (stub => stub.RemovedObjectRules).Return (new IRemovingObjectValidationRuleCollector[0]);
      _validationRuleCollectorStub2.Stub (stub => stub.RemovedObjectRules).Return (new IRemovingObjectValidationRuleCollector[0]);

      _propertyValidatorExtractorFactoryMock
          .Expect (mock => mock.Create (Arg<IEnumerable<PropertyValidatorRegistrationWithContext>>.Is.Anything, Arg<ILogContext>.Is.NotNull))
          .Return (_propertyValidatorExtractorMock);

      _propertyValidatorExtractorMock
          .Expect (mock => mock.ExtractPropertyValidatorsToRemove (_addingPropertyValidationRuleCollector1))
          .Return (new IPropertyValidator[0]);

      var result =
          _merger.Merge (
              new[]
              {
                  new[] { new ValidationRuleCollectorInfo (_validationRuleCollectorStub1, typeof (ApiBasedValidationRuleCollectorProvider)) },
                  new[] { new ValidationRuleCollectorInfo (_validationRuleCollectorStub2, typeof (ApiBasedValidationRuleCollectorProvider)) }
              }).CollectedPropertyValidationRules.ToArray();

      _propertyValidatorExtractorFactoryMock.VerifyAllExpectations();
      _propertyValidatorExtractorMock.VerifyAllExpectations();
      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0].Validators, Is.EquivalentTo (new IPropertyValidator[] { _notEmptyValidator, _notNullValidator, _notEqualValidator }));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void Merge_RemoveObjectValidatorWithNoCollectorTypeDefined_ValidatorIsRemoved ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void Merge_RemoveObjectValidatorWithAlreadyRegisteredCollectorTypeDefined_ValidatorIsRemoved ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void Merge_RemoveObjectValidator_AllRemoveRulesInSameGroupAreAppliedBeforeAllAddedRulesInSameGroup ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void Merge_RemoveObjectValidator_NoPropertyValidatorsToRemove_NoValidatorIsRemoved ()
    {
    }
  }
}