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
using FluentValidation;
using FluentValidation.Internal;
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
  [TestFixture]
  public class FluentValidatorBuilderTest
  {
    private IValidationCollectorProvider _validationCollectorProviderMock;
    private FluentValidatorBuilder _fluentValidationBuilder;
    private IComponentValidationCollector _componenValidationCollectorStub1;
    private IComponentValidationCollector _componenValidationCollectorStub2;
    private IValidationCollectorMerger _validationCollectorMergerMock;
    private IComponentValidationCollector _componenValidationCollectorStub3;

    private IValidationRule[] _fakeValidationRuleResult;
    private IValidationRule _validationRuleStub1;
    private IValidationRule _validationRuleStub2;
    private PropertyRule _validationRuleStub3;
    private PropertyRule _validationRuleStub4;
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
    private IValidationRuleMetadataService _validationRuleGlobalizationServiceMock;
    private IMemberInformationNameResolver _memberInformationNameResolverMock;
    private ICollectorValidator _collectorValidatorMock;
    private ValidationCollectorMergeResult _fakeValidationCollectorMergeResult;

    [SetUp]
    public void SetUp ()
    {
      _validationCollectorProviderMock = MockRepository.GenerateStrictMock<IValidationCollectorProvider>();
      _validationCollectorMergerMock = MockRepository.GenerateStrictMock<IValidationCollectorMerger>();
      _metaRulesValidatorFactoryStub = MockRepository.GenerateStub<IMetaRulesValidatorFactory>();
      _metaRuleValidatorMock = MockRepository.GenerateStrictMock<IMetaRuleValidator>();
      _validationRuleGlobalizationServiceMock = MockRepository.GenerateStrictMock<IValidationRuleMetadataService>();
      _memberInformationNameResolverMock = MockRepository.GenerateStrictMock<IMemberInformationNameResolver>();
      _collectorValidatorMock = MockRepository.GenerateStrictMock<ICollectorValidator> ();

      _metaValidationRule1Stub = MockRepository.GenerateStub<IAddingComponentPropertyMetaValidationRule>();
      _metaValidationRule2Stub = MockRepository.GenerateStub<IAddingComponentPropertyMetaValidationRule>();
      _metaValidationRule3Stub = MockRepository.GenerateStub<IAddingComponentPropertyMetaValidationRule>();

      _componenValidationCollectorStub1 = MockRepository.GenerateStub<IComponentValidationCollector>();
      _componenValidationCollectorStub1.Stub (stub => stub.AddedPropertyMetaValidationRules).Return (new[] { _metaValidationRule1Stub });
      _componenValidationCollectorStub2 = MockRepository.GenerateStub<IComponentValidationCollector>();
      _componenValidationCollectorStub2.Stub (stub => stub.AddedPropertyMetaValidationRules)
          .Return (new[] { _metaValidationRule2Stub, _metaValidationRule3Stub });
      _componenValidationCollectorStub3 = MockRepository.GenerateStub<IComponentValidationCollector>();
      _componenValidationCollectorStub3.Stub (stub => stub.AddedPropertyMetaValidationRules)
          .Return (new IAddingComponentPropertyMetaValidationRule[0]);

      _validationCollectorInfo1 = new ValidationCollectorInfo (
          _componenValidationCollectorStub1,
          typeof (ApiBasedComponentValidationCollectorProvider));
      _validationCollectorInfo2 = new ValidationCollectorInfo (
          _componenValidationCollectorStub2,
          typeof (ApiBasedComponentValidationCollectorProvider));
      _validationCollectorInfo3 = new ValidationCollectorInfo (
          _componenValidationCollectorStub3,
          typeof (ApiBasedComponentValidationCollectorProvider));

      _validationRuleStub1 = MockRepository.GenerateStub<IValidationRule>();
      _validationRuleStub2 = MockRepository.GenerateStub<IValidationRule>();
      _validationRuleStub3 = PropertyRule.Create (ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.FirstName));
      _validationRuleStub4 = PropertyRule.Create (ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName));

      _fakeValidationRuleResult = new[] { _validationRuleStub1, _validationRuleStub2, _validationRuleStub3, _validationRuleStub4 };
      _fakeValidationCollectorMergeResult = new ValidationCollectorMergeResult (_fakeValidationRuleResult, MockRepository.GenerateStub<ILogContext>());

      _fluentValidationBuilder = new FluentValidatorBuilder (
          _validationCollectorProviderMock,
          _validationCollectorMergerMock,
          _metaRulesValidatorFactoryStub,
          _validationRuleGlobalizationServiceMock,
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
      Assert.That (_fluentValidationBuilder.ComponentValidationCollectorProvider, Is.SameAs (_validationCollectorProviderMock));
      Assert.That (_fluentValidationBuilder.ValidationCollectorMerger, Is.SameAs (_validationCollectorMergerMock));
    }

    [Test]
    public void BuildValidator ()
    {
      ExpectMocks();

      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componenValidationCollectorStub1)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componenValidationCollectorStub2)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componenValidationCollectorStub3)).Repeat.Once ();

      _metaRuleValidatorMock
          .Expect (mock => mock.Validate (Arg<IValidationRule[]>.List.Equal (_fakeValidationRuleResult)))
          .Return (new[] { _validMetaValidationResult1, _validMetaValidationResult2 });

      _memberInformationNameResolverMock
          .Expect (mock => mock.GetPropertyName (Arg<IPropertyInformation>.Matches (pi => pi.Name == "FirstName")))
          .Return ("FakeTechnicalPropertyName1");
      _memberInformationNameResolverMock
          .Expect (mock => mock.GetPropertyName (Arg<IPropertyInformation>.Matches (pi => pi.Name == "LastName")))
          .Return ("FakeTechnicalPropertyName2");

      _validationRuleGlobalizationServiceMock
          .Expect (mock => mock.ApplyMetadata (_validationRuleStub1, typeof (SpecialCustomer1)));
      _validationRuleGlobalizationServiceMock
          .Expect (mock => mock.ApplyMetadata (_validationRuleStub2, typeof (SpecialCustomer1)));
      _validationRuleGlobalizationServiceMock
          .Expect (mock => mock.ApplyMetadata (_validationRuleStub3, typeof (SpecialCustomer1)));
      _validationRuleGlobalizationServiceMock
          .Expect (mock => mock.ApplyMetadata (_validationRuleStub4, typeof (SpecialCustomer1)));

      var result = _fluentValidationBuilder.BuildValidator (typeof (SpecialCustomer1));

      _validationCollectorProviderMock.VerifyAllExpectations();
      _validationCollectorMergerMock.VerifyAllExpectations();
      _metaRuleValidatorMock.VerifyAllExpectations();
      _memberInformationNameResolverMock.VerifyAllExpectations();
      _validationRuleGlobalizationServiceMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (Validator)));
      var validator = (Validator) result;
      Assert.That (validator.ValidationRules, Is.EqualTo (_fakeValidationRuleResult));
      Assert.That (_validationRuleStub3.PropertyName, Is.EqualTo ("FakeTechnicalPropertyName1"));
      Assert.That (_validationRuleStub4.PropertyName, Is.EqualTo ("FakeTechnicalPropertyName2"));
    }

    [Test]
    public void BuildValidator_ExtensionMethod ()
    {
      ExpectMocks();

      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componenValidationCollectorStub1)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componenValidationCollectorStub2)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componenValidationCollectorStub3)).Repeat.Once ();

      _metaRuleValidatorMock
          .Expect (mock => mock.Validate (Arg<IValidationRule[]>.List.Equal (_fakeValidationRuleResult)))
          .Return (new[] { _validMetaValidationResult1, _validMetaValidationResult2 });

      _memberInformationNameResolverMock
          .Expect (mock => mock.GetPropertyName (Arg<IPropertyInformation>.Matches (pi => pi.Name == "FirstName")))
          .Return ("FakeTechnicalPropertyName1");
      _memberInformationNameResolverMock
          .Expect (mock => mock.GetPropertyName (Arg<IPropertyInformation>.Matches (pi => pi.Name == "LastName")))
          .Return ("FakeTechnicalPropertyName2");

      _validationRuleGlobalizationServiceMock
          .Expect (mock => mock.ApplyMetadata (_validationRuleStub1, typeof (SpecialCustomer1)));
      _validationRuleGlobalizationServiceMock
          .Expect (mock => mock.ApplyMetadata (_validationRuleStub2, typeof (SpecialCustomer1)));
      _validationRuleGlobalizationServiceMock
          .Expect (mock => mock.ApplyMetadata (_validationRuleStub3, typeof (SpecialCustomer1)));
      _validationRuleGlobalizationServiceMock
          .Expect (mock => mock.ApplyMetadata (_validationRuleStub4, typeof (SpecialCustomer1)));

      var result = _fluentValidationBuilder.BuildValidator<SpecialCustomer1>();

      _validationCollectorProviderMock.VerifyAllExpectations();
      _validationCollectorMergerMock.VerifyAllExpectations();
      _metaRuleValidatorMock.VerifyAllExpectations();
      _memberInformationNameResolverMock.VerifyAllExpectations();
      _validationRuleGlobalizationServiceMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (TypedValidatorDecorator<SpecialCustomer1>)));
      Assert.That (_validationRuleStub3.PropertyName, Is.EqualTo ("FakeTechnicalPropertyName1"));
      Assert.That (_validationRuleStub4.PropertyName, Is.EqualTo ("FakeTechnicalPropertyName2"));
    }

    [Test]
    public void BuildValidator_MetaValidationFailed ()
    {
      ExpectMocks();

      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componenValidationCollectorStub1)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componenValidationCollectorStub2)).Repeat.Once ();
      _collectorValidatorMock.Expect (mock => mock.CheckValid (_componenValidationCollectorStub3)).Repeat.Once ();

      _metaRuleValidatorMock
          .Expect (mock => mock.Validate (_fakeValidationRuleResult))
          .Return (new[] { _validMetaValidationResult1, _invalidMetaValidationResult1, _validMetaValidationResult2, _invalidMetaValidationResult2 });

      Assert.That (
          () => _fluentValidationBuilder.BuildValidator<SpecialCustomer1>(),
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