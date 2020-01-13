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
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Providers;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class ValidationRuleCollectorBasedValidatorBuilderTest
  {
    private IValidationRuleCollectorProvider _validationRuleCollectorProviderMock;
    private ValidationRuleCollectorBasedValidatorBuilder _validatorBuilder;
    private IValidationRuleCollector _validationRuleCollectorStub1;
    private IValidationRuleCollector _validationRuleCollectorStub2;
    private IValidationRuleCollectorMerger _validationRuleCollectorMergerMock;
    private IValidationRuleCollector _validationRuleCollectorStub3;

    private IAddingPropertyValidationRuleCollector[] _fakeAddingPropertyValidationRulesCollectorResult;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollectorStub1;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollectorStub2;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollectorStub3;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollectorStub4;
    private IAddingObjectValidationRuleCollector[] _fakeAddingObjectValidationRulesCollectorResult;
    private IAddingObjectValidationRuleCollector _addingObjectValidationRuleCollectorStub1;
    private IAddingObjectValidationRuleCollector _addingObjectValidationRuleCollectorStub2;
    private IAddingObjectValidationRuleCollector _addingObjectValidationRuleCollectorStub3;
    private IAddingObjectValidationRuleCollector _addingObjectValidationRuleCollectorStub4;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo1;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo2;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo3;
    private IMetaRulesValidatorFactory _metaRulesValidatorFactoryStub;
    private IMetaRuleValidator _metaRuleValidatorMock;
    private MetaValidationRuleValidationResult _validMetaValidationResult1;
    private MetaValidationRuleValidationResult _validMetaValidationResult2;
    private MetaValidationRuleValidationResult _invalidMetaValidationResult1;
    private MetaValidationRuleValidationResult _invalidMetaValidationResult2;
    private IPropertyMetaValidationRuleCollector _metaValidationRule1Stub;
    private IPropertyMetaValidationRuleCollector _metaValidationRule2Stub;
    private IPropertyMetaValidationRuleCollector _metaValidationRule3Stub;
    private IMemberInformationNameResolver _memberInformationNameResolverMock;
    private IValidationRuleCollectorValidator _collectorValidatorMock;
    private ValidationCollectorMergeResult _fakeValidationCollectorMergeResult;
    private IValidationMessageFactory _validationMessageFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _validationRuleCollectorProviderMock = MockRepository.GenerateStrictMock<IValidationRuleCollectorProvider>();
      _validationRuleCollectorMergerMock = MockRepository.GenerateStrictMock<IValidationRuleCollectorMerger>();
      _metaRulesValidatorFactoryStub = MockRepository.GenerateStub<IMetaRulesValidatorFactory>();
      _metaRuleValidatorMock = MockRepository.GenerateStrictMock<IMetaRuleValidator>();
      _memberInformationNameResolverMock = MockRepository.GenerateStrictMock<IMemberInformationNameResolver>();
      _collectorValidatorMock = MockRepository.GenerateStrictMock<IValidationRuleCollectorValidator> ();
      _validationMessageFactoryStub = MockRepository.GenerateStub<IValidationMessageFactory>();

      _metaValidationRule1Stub = MockRepository.GenerateStub<IPropertyMetaValidationRuleCollector>();
      _metaValidationRule2Stub = MockRepository.GenerateStub<IPropertyMetaValidationRuleCollector>();
      _metaValidationRule3Stub = MockRepository.GenerateStub<IPropertyMetaValidationRuleCollector>();

      _validationRuleCollectorStub1 = MockRepository.GenerateStub<IValidationRuleCollector>();
      _validationRuleCollectorStub1.Stub (stub => stub.PropertyMetaValidationRules).Return (new[] { _metaValidationRule1Stub });
      _validationRuleCollectorStub2 = MockRepository.GenerateStub<IValidationRuleCollector>();
      _validationRuleCollectorStub2.Stub (stub => stub.PropertyMetaValidationRules)
          .Return (new[] { _metaValidationRule2Stub, _metaValidationRule3Stub });
      _validationRuleCollectorStub3 = MockRepository.GenerateStub<IValidationRuleCollector>();
      _validationRuleCollectorStub3.Stub (stub => stub.PropertyMetaValidationRules)
          .Return (new IPropertyMetaValidationRuleCollector[0]);

      _validationRuleCollectorInfo1 = new ValidationRuleCollectorInfo (
          _validationRuleCollectorStub1,
          typeof (ApiBasedValidationRuleCollectorProvider));
      _validationRuleCollectorInfo2 = new ValidationRuleCollectorInfo (
          _validationRuleCollectorStub2,
          typeof (ApiBasedValidationRuleCollectorProvider));
      _validationRuleCollectorInfo3 = new ValidationRuleCollectorInfo (
          _validationRuleCollectorStub3,
          typeof (ApiBasedValidationRuleCollectorProvider));

      _addingPropertyValidationRuleCollectorStub1 = MockRepository.GenerateStub<IAddingPropertyValidationRuleCollector>();
      _addingPropertyValidationRuleCollectorStub2 = MockRepository.GenerateStub<IAddingPropertyValidationRuleCollector>();
      _addingPropertyValidationRuleCollectorStub3 = AddingPropertyValidationRuleCollector.Create (
          ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.FirstName),
          typeof (IValidationRuleCollector));
      _addingPropertyValidationRuleCollectorStub4 = AddingPropertyValidationRuleCollector.Create (
          ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName),
          typeof (IValidationRuleCollector));

      _addingObjectValidationRuleCollectorStub1 = MockRepository.GenerateStub<IAddingObjectValidationRuleCollector>();
      _addingObjectValidationRuleCollectorStub2 = MockRepository.GenerateStub<IAddingObjectValidationRuleCollector>();
      _addingObjectValidationRuleCollectorStub3 = AddingObjectValidationRuleCollector.Create<Customer> (typeof (IValidationRuleCollector));
      _addingObjectValidationRuleCollectorStub4 = AddingObjectValidationRuleCollector.Create<Customer> (typeof (IValidationRuleCollector));

      _fakeAddingPropertyValidationRulesCollectorResult =
          new[]
          {
              _addingPropertyValidationRuleCollectorStub1,
              _addingPropertyValidationRuleCollectorStub2,
              _addingPropertyValidationRuleCollectorStub3,
              _addingPropertyValidationRuleCollectorStub4
          };
      _fakeAddingObjectValidationRulesCollectorResult =
          new[]
          {
              _addingObjectValidationRuleCollectorStub1,
              _addingObjectValidationRuleCollectorStub2,
              _addingObjectValidationRuleCollectorStub3,
              _addingObjectValidationRuleCollectorStub4
          };
      _fakeValidationCollectorMergeResult = new ValidationCollectorMergeResult (
          _fakeAddingPropertyValidationRulesCollectorResult,
          _fakeAddingObjectValidationRulesCollectorResult,
          MockRepository.GenerateStub<ILogContext>());

      _validatorBuilder = new ValidationRuleCollectorBasedValidatorBuilder (
          _validationRuleCollectorProviderMock,
          _validationRuleCollectorMergerMock,
          _metaRulesValidatorFactoryStub,
          _validationMessageFactoryStub,
          _memberInformationNameResolverMock,
          _collectorValidatorMock);

      _validMetaValidationResult1 = MetaValidationRuleValidationResult.CreateValidResult();
      _validMetaValidationResult2 = MetaValidationRuleValidationResult.CreateValidResult();
      _invalidMetaValidationResult1 = MetaValidationRuleValidationResult.CreateInvalidResult ("Error1");
      _invalidMetaValidationResult2 = MetaValidationRuleValidationResult.CreateInvalidResult ("Error2");
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_validatorBuilder.ValidationRuleCollectorProvider, Is.SameAs (_validationRuleCollectorProviderMock));
      Assert.That (_validatorBuilder.ValidationRuleCollectorMerger, Is.SameAs (_validationRuleCollectorMergerMock));
    }

    [Test]
    public void BuildValidator ()
    {
      ExpectMocks();

      _collectorValidatorMock.Expect (mock => mock.CheckValid (_validationRuleCollectorStub1)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_validationRuleCollectorStub2)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_validationRuleCollectorStub3)).Repeat.Once ();

      _metaRuleValidatorMock
          .Expect (mock => mock.Validate (Arg<IAddingPropertyValidationRuleCollector[]>.List.Equal (_fakeAddingPropertyValidationRulesCollectorResult)))
          .Return (new[] { _validMetaValidationResult1, _validMetaValidationResult2 });

      var validationRuleStub1 = MockRepository.GenerateStub<IValidationRule>();
      var validationRuleStub2 = MockRepository.GenerateStub<IValidationRule>();
      var validationRuleStub4 = MockRepository.GenerateStub<IValidationRule>();
      var validationRuleStub5 = MockRepository.GenerateStub<IValidationRule>();
      _addingPropertyValidationRuleCollectorStub1.Stub (_ => _.CreateValidationRule (_validationMessageFactoryStub)).Return (validationRuleStub1);
      _addingPropertyValidationRuleCollectorStub2.Stub (_ => _.CreateValidationRule (_validationMessageFactoryStub)).Return (validationRuleStub2);
      _addingObjectValidationRuleCollectorStub1.Stub (_ => _.CreateValidationRule (_validationMessageFactoryStub)).Return (validationRuleStub4);
      _addingObjectValidationRuleCollectorStub2.Stub (_ => _.CreateValidationRule (_validationMessageFactoryStub)).Return (validationRuleStub5);

      var result = _validatorBuilder.BuildValidator (typeof (SpecialCustomer1));

      _validationRuleCollectorProviderMock.VerifyAllExpectations();
      _validationRuleCollectorMergerMock.VerifyAllExpectations();
      _metaRuleValidatorMock.VerifyAllExpectations();
      _memberInformationNameResolverMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (Validator)));
      var validator = (Validator) result;
      var validationRules = validator.ValidationRules.ToArray();
      Assert.That (validationRules.Length, Is.EqualTo (8));
      Assert.That (validationRules[0], Is.SameAs (validationRuleStub1));
      Assert.That (validationRules[1], Is.SameAs (validationRuleStub2));
      Assert.That (validationRules[2], Is.InstanceOf<PropertyValidationRule<Customer, string>>());
      Assert.That (((IPropertyValidationRule) validationRules[2]).Property, Is.SameAs (_addingPropertyValidationRuleCollectorStub3.Property));
      Assert.That (validationRules[3], Is.InstanceOf<PropertyValidationRule<Customer, string>>());
      Assert.That (((IPropertyValidationRule) validationRules[3]).Property, Is.SameAs (_addingPropertyValidationRuleCollectorStub4.Property));
      Assert.That (validationRules[4], Is.SameAs (validationRuleStub4));
      Assert.That (validationRules[5], Is.SameAs (validationRuleStub5));
      Assert.That (validationRules[6], Is.InstanceOf<ObjectValidationRule<Customer>>());
      Assert.That (validationRules[7], Is.InstanceOf<ObjectValidationRule<Customer>>());
    }

    [Test]
    public void BuildValidator_ExtensionMethod ()
    {
      ExpectMocks();

      _collectorValidatorMock.Expect (mock => mock.CheckValid (_validationRuleCollectorStub1)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_validationRuleCollectorStub2)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_validationRuleCollectorStub3)).Repeat.Once ();

      _metaRuleValidatorMock
          .Expect (mock => mock.Validate (Arg<IAddingPropertyValidationRuleCollector[]>.List.Equal (_fakeAddingPropertyValidationRulesCollectorResult)))
          .Return (new[] { _validMetaValidationResult1, _validMetaValidationResult2 });

      var result = _validatorBuilder.BuildValidator<SpecialCustomer1>();

      _validationRuleCollectorProviderMock.VerifyAllExpectations();
      _validationRuleCollectorMergerMock.VerifyAllExpectations();
      _metaRuleValidatorMock.VerifyAllExpectations();
      _memberInformationNameResolverMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (TypedValidatorDecorator<SpecialCustomer1>)));
    }

    [Test]
    public void BuildValidator_MetaValidationFailed ()
    {
      ExpectMocks();

      _collectorValidatorMock.Expect (mock => mock.CheckValid (_validationRuleCollectorStub1)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_validationRuleCollectorStub2)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_validationRuleCollectorStub3)).Repeat.Once ();

      _metaRuleValidatorMock
          .Expect (mock => mock.Validate (_fakeAddingPropertyValidationRulesCollectorResult))
          .Return (new[] { _validMetaValidationResult1, _invalidMetaValidationResult1, _validMetaValidationResult2, _invalidMetaValidationResult2 });

      Assert.That (
          () => _validatorBuilder.BuildValidator<SpecialCustomer1>(),
          Throws.TypeOf<ValidationConfigurationException>().And.Message.EqualTo ("Error1\r\n----------\r\nError2"));
    }

    private void ExpectMocks ()
    {
      var validationCollectorInfos = new[] { new[] { _validationRuleCollectorInfo1, _validationRuleCollectorInfo3 }, new[] { _validationRuleCollectorInfo2 } };
      _validationRuleCollectorProviderMock
          .Expect (mock => mock.GetValidationRuleCollectors (new[] { typeof (SpecialCustomer1) }))
          .Return (validationCollectorInfos);

      _validationRuleCollectorMergerMock
          .Expect (
              mock => mock.Merge (
                  Arg<IEnumerable<IEnumerable<ValidationRuleCollectorInfo>>>.Matches (
                      c =>
                          c.SelectMany (g => g).ElementAt (0).Equals (_validationRuleCollectorInfo1) &&
                          c.SelectMany (g => g).ElementAt (1).Equals (_validationRuleCollectorInfo3) &&
                          c.SelectMany (g => g).ElementAt (2).Equals (_validationRuleCollectorInfo2))))
          .Return (_fakeValidationCollectorMergeResult);

      _metaRulesValidatorFactoryStub
          .Stub (
              stub =>
                  stub.CreateMetaRuleValidator (
                      Arg<IPropertyMetaValidationRuleCollector[]>.List.Equal (
                          new[] { _metaValidationRule1Stub, _metaValidationRule2Stub, _metaValidationRule3Stub })))
          .Return (_metaRuleValidatorMock);
    }
  }
}