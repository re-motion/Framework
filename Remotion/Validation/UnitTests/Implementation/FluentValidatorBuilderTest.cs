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
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  //TODO RM-5960: Rename to CollectorBasedValidatorBuilderTest
  [TestFixture]
  public class FluentValidatorBuilderTest
  {
    private IValidationCollectorProvider _validationCollectorProviderMock;
    private FluentValidatorBuilder _validatorBuilder;
    private IComponentValidationCollector _componentValidationCollectorStub1;
    private IComponentValidationCollector _componentValidationCollectorStub2;
    private IValidationCollectorMerger _validationCollectorMergerMock;
    private IComponentValidationCollector _componentValidationCollectorStub3;

    private IAddingComponentPropertyRule[] _fakeAddingComponentPropertyRulesResult;
    private IAddingComponentPropertyRule _addingComponentPropertyRuleStub1;
    private IAddingComponentPropertyRule _addingComponentPropertyRuleStub2;
    private IAddingComponentPropertyRule _addingComponentPropertyRuleStub3;
    private IAddingComponentPropertyRule _addingComponentPropertyRuleStub4;
    private ValidationCollectorInfo _validationCollectorInfo1;
    private ValidationCollectorInfo _validationCollectorInfo2;
    private ValidationCollectorInfo _validationCollectorInfo3;
    private IMetaRulesValidatorFactory _metaRulesValidatorFactoryStub;
    private IMetaRuleValidator _metaRuleValidatorMock;
    private MetaValidationRuleValidationResult _validMetaValidationResult1;
    private MetaValidationRuleValidationResult _validMetaValidationResult2;
    private MetaValidationRuleValidationResult _invalidMetaValidationResult1;
    private MetaValidationRuleValidationResult _invalidMetaValidationResult2;
    private IAddingComponentPropertyMetaValidationRule _metaValidationRule1Stub;
    private IAddingComponentPropertyMetaValidationRule _metaValidationRule2Stub;
    private IAddingComponentPropertyMetaValidationRule _metaValidationRule3Stub;
    private IMemberInformationNameResolver _memberInformationNameResolverMock;
    private ICollectorValidator _collectorValidatorMock;
    private ValidationCollectorMergeResult _fakeValidationCollectorMergeResult;
    private IValidationMessageFactory _validationMessageFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _validationCollectorProviderMock = MockRepository.GenerateStrictMock<IValidationCollectorProvider>();
      _validationCollectorMergerMock = MockRepository.GenerateStrictMock<IValidationCollectorMerger>();
      _metaRulesValidatorFactoryStub = MockRepository.GenerateStub<IMetaRulesValidatorFactory>();
      _metaRuleValidatorMock = MockRepository.GenerateStrictMock<IMetaRuleValidator>();
      _memberInformationNameResolverMock = MockRepository.GenerateStrictMock<IMemberInformationNameResolver>();
      _collectorValidatorMock = MockRepository.GenerateStrictMock<ICollectorValidator> ();
      _validationMessageFactoryStub = MockRepository.GenerateStub<IValidationMessageFactory>();

      _metaValidationRule1Stub = MockRepository.GenerateStub<IAddingComponentPropertyMetaValidationRule>();
      _metaValidationRule2Stub = MockRepository.GenerateStub<IAddingComponentPropertyMetaValidationRule>();
      _metaValidationRule3Stub = MockRepository.GenerateStub<IAddingComponentPropertyMetaValidationRule>();

      _componentValidationCollectorStub1 = MockRepository.GenerateStub<IComponentValidationCollector>();
      _componentValidationCollectorStub1.Stub (stub => stub.AddedPropertyMetaValidationRules).Return (new[] { _metaValidationRule1Stub });
      _componentValidationCollectorStub2 = MockRepository.GenerateStub<IComponentValidationCollector>();
      _componentValidationCollectorStub2.Stub (stub => stub.AddedPropertyMetaValidationRules)
          .Return (new[] { _metaValidationRule2Stub, _metaValidationRule3Stub });
      _componentValidationCollectorStub3 = MockRepository.GenerateStub<IComponentValidationCollector>();
      _componentValidationCollectorStub3.Stub (stub => stub.AddedPropertyMetaValidationRules)
          .Return (new IAddingComponentPropertyMetaValidationRule[0]);

      _validationCollectorInfo1 = new ValidationCollectorInfo (
          _componentValidationCollectorStub1,
          typeof (ApiBasedComponentValidationCollectorProvider));
      _validationCollectorInfo2 = new ValidationCollectorInfo (
          _componentValidationCollectorStub2,
          typeof (ApiBasedComponentValidationCollectorProvider));
      _validationCollectorInfo3 = new ValidationCollectorInfo (
          _componentValidationCollectorStub3,
          typeof (ApiBasedComponentValidationCollectorProvider));

      _addingComponentPropertyRuleStub1 = MockRepository.GenerateStub<IAddingComponentPropertyRule>();
      _addingComponentPropertyRuleStub2 = MockRepository.GenerateStub<IAddingComponentPropertyRule>();
      _addingComponentPropertyRuleStub3 = AddingComponentPropertyRule.Create (
          ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.FirstName),
          typeof (IComponentValidationCollector));
      _addingComponentPropertyRuleStub4 = AddingComponentPropertyRule.Create (
          ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName),
          typeof (IComponentValidationCollector));

      _fakeAddingComponentPropertyRulesResult = new[]
                                  {
                                      _addingComponentPropertyRuleStub1,
                                      _addingComponentPropertyRuleStub2,
                                      _addingComponentPropertyRuleStub3,
                                      _addingComponentPropertyRuleStub4
                                  };
      _fakeValidationCollectorMergeResult = new ValidationCollectorMergeResult (
          _fakeAddingComponentPropertyRulesResult,
          MockRepository.GenerateStub<ILogContext>());

      _validatorBuilder = new FluentValidatorBuilder (
          _validationCollectorProviderMock,
          _validationCollectorMergerMock,
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
      Assert.That (_validatorBuilder.ComponentValidationCollectorProvider, Is.SameAs (_validationCollectorProviderMock));
      Assert.That (_validatorBuilder.ValidationCollectorMerger, Is.SameAs (_validationCollectorMergerMock));
    }

    [Test]
    public void BuildValidator ()
    {
      ExpectMocks();

      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componentValidationCollectorStub1)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componentValidationCollectorStub2)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componentValidationCollectorStub3)).Repeat.Once ();

      _metaRuleValidatorMock
          .Expect (mock => mock.Validate (Arg<IAddingComponentPropertyRule[]>.List.Equal (_fakeAddingComponentPropertyRulesResult)))
          .Return (new[] { _validMetaValidationResult1, _validMetaValidationResult2 });

      var validationRuleStub1 = MockRepository.GenerateStub<IValidationRule>();
      var validationRuleStub2 = MockRepository.GenerateStub<IValidationRule>();
      _addingComponentPropertyRuleStub1.Stub (_ => _.CreateValidationRule (_validationMessageFactoryStub)).Return (validationRuleStub1);
      _addingComponentPropertyRuleStub2.Stub (_ => _.CreateValidationRule (_validationMessageFactoryStub)).Return (validationRuleStub2);

      var result = _validatorBuilder.BuildValidator (typeof (SpecialCustomer1));

      _validationCollectorProviderMock.VerifyAllExpectations();
      _validationCollectorMergerMock.VerifyAllExpectations();
      _metaRuleValidatorMock.VerifyAllExpectations();
      _memberInformationNameResolverMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (Validator)));
      var validator = (Validator) result;
      var validationRules = validator.ValidationRules.ToArray();
      Assert.That (validationRules.Length, Is.EqualTo (4));
      Assert.That (validationRules[0], Is.SameAs (validationRuleStub1));
      Assert.That (validationRules[1], Is.SameAs (validationRuleStub2));
      Assert.That (validationRules[2].Property, Is.SameAs (_addingComponentPropertyRuleStub3.Property));
      Assert.That (validationRules[3].Property, Is.SameAs (_addingComponentPropertyRuleStub4.Property));
    }

    [Test]
    public void BuildValidator_ExtensionMethod ()
    {
      ExpectMocks();

      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componentValidationCollectorStub1)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componentValidationCollectorStub2)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componentValidationCollectorStub3)).Repeat.Once ();

      _metaRuleValidatorMock
          .Expect (mock => mock.Validate (Arg<IAddingComponentPropertyRule[]>.List.Equal (_fakeAddingComponentPropertyRulesResult)))
          .Return (new[] { _validMetaValidationResult1, _validMetaValidationResult2 });

      var result = _validatorBuilder.BuildValidator<SpecialCustomer1>();

      _validationCollectorProviderMock.VerifyAllExpectations();
      _validationCollectorMergerMock.VerifyAllExpectations();
      _metaRuleValidatorMock.VerifyAllExpectations();
      _memberInformationNameResolverMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (TypedValidatorDecorator<SpecialCustomer1>)));
    }

    [Test]
    public void BuildValidator_MetaValidationFailed ()
    {
      ExpectMocks();

      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componentValidationCollectorStub1)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componentValidationCollectorStub2)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componentValidationCollectorStub3)).Repeat.Once ();

      _metaRuleValidatorMock
          .Expect (mock => mock.Validate (_fakeAddingComponentPropertyRulesResult))
          .Return (new[] { _validMetaValidationResult1, _invalidMetaValidationResult1, _validMetaValidationResult2, _invalidMetaValidationResult2 });

      Assert.That (
          () => _validatorBuilder.BuildValidator<SpecialCustomer1>(),
          Throws.TypeOf<ValidationConfigurationException>().And.Message.EqualTo ("Error1\r\n----------\r\nError2"));
    }

    private void ExpectMocks ()
    {
      var validationCollectorInfos = new[] { new[] { _validationCollectorInfo1, _validationCollectorInfo3 }, new[] { _validationCollectorInfo2 } };
      _validationCollectorProviderMock
          .Expect (mock => mock.GetValidationCollectors (new[] { typeof (SpecialCustomer1) }))
          .Return (validationCollectorInfos);

      _validationCollectorMergerMock
          .Expect (
              mock => mock.Merge (
                  Arg<IEnumerable<IEnumerable<ValidationCollectorInfo>>>.Matches (
                      c =>
                          c.SelectMany (g => g).ElementAt (0).Equals (_validationCollectorInfo1) &&
                          c.SelectMany (g => g).ElementAt (1).Equals (_validationCollectorInfo3) &&
                          c.SelectMany (g => g).ElementAt (2).Equals (_validationCollectorInfo2))))
          .Return (_fakeValidationCollectorMergeResult);

      _metaRulesValidatorFactoryStub
          .Stub (
              stub =>
                  stub.CreateMetaRuleValidator (
                      Arg<IAddingComponentPropertyMetaValidationRule[]>.List.Equal (
                          new[] { _metaValidationRule1Stub, _metaValidationRule2Stub, _metaValidationRule3Stub })))
          .Return (_metaRuleValidatorMock);
    }
  }
}