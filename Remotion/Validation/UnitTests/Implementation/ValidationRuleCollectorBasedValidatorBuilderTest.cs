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
using Moq;
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

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class ValidationRuleCollectorBasedValidatorBuilderTest
  {
    private Mock<IValidationRuleCollectorProvider> _validationRuleCollectorProviderMock;
    private ValidationRuleCollectorBasedValidatorBuilder _validatorBuilder;
    private Mock<IValidationRuleCollector> _validationRuleCollectorStub1;
    private Mock<IValidationRuleCollector> _validationRuleCollectorStub2;
    private Mock<IValidationRuleCollectorMerger> _validationRuleCollectorMergerMock;
    private Mock<IValidationRuleCollector> _validationRuleCollectorStub3;

    private IAddingPropertyValidationRuleCollector[] _fakeAddingPropertyValidationRulesCollectorResult;
    private Mock<IAddingPropertyValidationRuleCollector> _addingPropertyValidationRuleCollectorStub1;
    private Mock<IAddingPropertyValidationRuleCollector> _addingPropertyValidationRuleCollectorStub2;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollectorStub3;
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollectorStub4;
    private IAddingObjectValidationRuleCollector[] _fakeAddingObjectValidationRulesCollectorResult;
    private Mock<IAddingObjectValidationRuleCollector> _addingObjectValidationRuleCollectorStub1;
    private Mock<IAddingObjectValidationRuleCollector> _addingObjectValidationRuleCollectorStub2;
    private IAddingObjectValidationRuleCollector _addingObjectValidationRuleCollectorStub3;
    private IAddingObjectValidationRuleCollector _addingObjectValidationRuleCollectorStub4;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo1;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo2;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo3;
    private Mock<IPropertyMetaValidationRuleValidatorFactory> _propertyMetaValidationRuleValidatorFactoryStub;
    private Mock<IPropertyMetaValidationRuleValidator> _propertyMetaValidationRuleValidatorMock;
    private Mock<IObjectMetaValidationRuleValidatorFactory> _objectMetaValidationRuleValidatorFactoryStub;
    private Mock<IObjectMetaValidationRuleValidator> _objectMetaValidationRuleValidatorMock;
    private MetaValidationRuleValidationResult _validMetaValidationResult1;
    private MetaValidationRuleValidationResult _validMetaValidationResult2;
    private MetaValidationRuleValidationResult _validMetaValidationResult3;
    private MetaValidationRuleValidationResult _validMetaValidationResult4;
    private MetaValidationRuleValidationResult _invalidMetaValidationResult1;
    private MetaValidationRuleValidationResult _invalidMetaValidationResult2;
    private MetaValidationRuleValidationResult _invalidMetaValidationResult3;
    private MetaValidationRuleValidationResult _invalidMetaValidationResult4;
    private Mock<IPropertyMetaValidationRuleCollector> _propertyMetaValidationRuleCollector1Stub;
    private Mock<IPropertyMetaValidationRuleCollector> _propertyMetaValidationRuleCollector2Stub;
    private Mock<IPropertyMetaValidationRuleCollector> _propertyMetaValidationRuleCollector3Stub;
    private Mock<IObjectMetaValidationRuleCollector> _objectMetaValidationRuleCollector1Stub;
    private Mock<IObjectMetaValidationRuleCollector> _objectMetaValidationRuleCollector2Stub;
    private Mock<IObjectMetaValidationRuleCollector> _objectMetaValidationRuleCollector3Stub;
    private Mock<IMemberInformationNameResolver> _memberInformationNameResolverMock;
    private Mock<IValidationRuleCollectorValidator> _collectorValidatorMock;
    private ValidationCollectorMergeResult _fakeValidationCollectorMergeResult;
    private Mock<IValidationMessageFactory> _validationMessageFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _validationRuleCollectorProviderMock = new Mock<IValidationRuleCollectorProvider>(MockBehavior.Strict);
      _validationRuleCollectorMergerMock = new Mock<IValidationRuleCollectorMerger>(MockBehavior.Strict);
      _propertyMetaValidationRuleValidatorFactoryStub = new Mock<IPropertyMetaValidationRuleValidatorFactory>();
      _propertyMetaValidationRuleValidatorMock = new Mock<IPropertyMetaValidationRuleValidator>(MockBehavior.Strict);
      _objectMetaValidationRuleValidatorFactoryStub = new Mock<IObjectMetaValidationRuleValidatorFactory>();
      _objectMetaValidationRuleValidatorMock = new Mock<IObjectMetaValidationRuleValidator>(MockBehavior.Strict);
      _memberInformationNameResolverMock = new Mock<IMemberInformationNameResolver>(MockBehavior.Strict);
      _collectorValidatorMock = new Mock<IValidationRuleCollectorValidator>(MockBehavior.Strict);
      _validationMessageFactoryStub = new Mock<IValidationMessageFactory>();

      _propertyMetaValidationRuleCollector1Stub = new Mock<IPropertyMetaValidationRuleCollector>();
      _propertyMetaValidationRuleCollector2Stub = new Mock<IPropertyMetaValidationRuleCollector>();
      _propertyMetaValidationRuleCollector3Stub = new Mock<IPropertyMetaValidationRuleCollector>();

      _objectMetaValidationRuleCollector1Stub = new Mock<IObjectMetaValidationRuleCollector>();
      _objectMetaValidationRuleCollector2Stub = new Mock<IObjectMetaValidationRuleCollector>();
      _objectMetaValidationRuleCollector3Stub = new Mock<IObjectMetaValidationRuleCollector>();

      _validationRuleCollectorStub1 = new Mock<IValidationRuleCollector>();
      _validationRuleCollectorStub1.Setup(stub => stub.PropertyMetaValidationRules).Returns(new[] { _propertyMetaValidationRuleCollector1Stub.Object });
      _validationRuleCollectorStub1.Setup(stub => stub.ObjectMetaValidationRules).Returns(new[] { _objectMetaValidationRuleCollector1Stub.Object });
      _validationRuleCollectorStub2 = new Mock<IValidationRuleCollector>();
      _validationRuleCollectorStub2
          .Setup(stub => stub.PropertyMetaValidationRules)
          .Returns(new[] { _propertyMetaValidationRuleCollector2Stub.Object, _propertyMetaValidationRuleCollector3Stub.Object });
      _validationRuleCollectorStub2
          .Setup(stub => stub.ObjectMetaValidationRules)
          .Returns(new[] { _objectMetaValidationRuleCollector2Stub.Object, _objectMetaValidationRuleCollector3Stub.Object });
      _validationRuleCollectorStub3 = new Mock<IValidationRuleCollector>();
      _validationRuleCollectorStub3.Setup(stub => stub.PropertyMetaValidationRules).Returns(new IPropertyMetaValidationRuleCollector[0]);
      _validationRuleCollectorStub3.Setup(stub => stub.ObjectMetaValidationRules).Returns(new IObjectMetaValidationRuleCollector[0]);

      _validationRuleCollectorInfo1 = new ValidationRuleCollectorInfo(
          _validationRuleCollectorStub1.Object,
          typeof(ApiBasedValidationRuleCollectorProvider));
      _validationRuleCollectorInfo2 = new ValidationRuleCollectorInfo(
          _validationRuleCollectorStub2.Object,
          typeof(ApiBasedValidationRuleCollectorProvider));
      _validationRuleCollectorInfo3 = new ValidationRuleCollectorInfo(
          _validationRuleCollectorStub3.Object,
          typeof(ApiBasedValidationRuleCollectorProvider));

      _addingPropertyValidationRuleCollectorStub1 = new Mock<IAddingPropertyValidationRuleCollector>();
      _addingPropertyValidationRuleCollectorStub2 = new Mock<IAddingPropertyValidationRuleCollector>();
      _addingPropertyValidationRuleCollectorStub3 = AddingPropertyValidationRuleCollector.Create(
          ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.FirstName),
          typeof(IValidationRuleCollector));
      _addingPropertyValidationRuleCollectorStub4 = AddingPropertyValidationRuleCollector.Create(
          ExpressionHelper.GetTypedMemberExpression<Customer, string>(c => c.LastName),
          typeof(IValidationRuleCollector));

      _addingObjectValidationRuleCollectorStub1 = new Mock<IAddingObjectValidationRuleCollector>();
      _addingObjectValidationRuleCollectorStub2 = new Mock<IAddingObjectValidationRuleCollector>();
      _addingObjectValidationRuleCollectorStub3 = AddingObjectValidationRuleCollector.Create<Customer>(typeof(IValidationRuleCollector));
      _addingObjectValidationRuleCollectorStub4 = AddingObjectValidationRuleCollector.Create<Customer>(typeof(IValidationRuleCollector));

      _fakeAddingPropertyValidationRulesCollectorResult =
          new[]
          {
              _addingPropertyValidationRuleCollectorStub1.Object,
              _addingPropertyValidationRuleCollectorStub2.Object,
              _addingPropertyValidationRuleCollectorStub3,
              _addingPropertyValidationRuleCollectorStub4
          };
      _fakeAddingObjectValidationRulesCollectorResult =
          new[]
          {
              _addingObjectValidationRuleCollectorStub1.Object,
              _addingObjectValidationRuleCollectorStub2.Object,
              _addingObjectValidationRuleCollectorStub3,
              _addingObjectValidationRuleCollectorStub4
          };
      _fakeValidationCollectorMergeResult = new ValidationCollectorMergeResult(
          _fakeAddingPropertyValidationRulesCollectorResult,
          _fakeAddingObjectValidationRulesCollectorResult,
          new Mock<ILogContext>().Object);

      _validatorBuilder = new ValidationRuleCollectorBasedValidatorBuilder(
          _validationRuleCollectorProviderMock.Object,
          _validationRuleCollectorMergerMock.Object,
          _propertyMetaValidationRuleValidatorFactoryStub.Object,
          _objectMetaValidationRuleValidatorFactoryStub.Object,
          _validationMessageFactoryStub.Object,
          _memberInformationNameResolverMock.Object,
          _collectorValidatorMock.Object);

      _validMetaValidationResult1 = MetaValidationRuleValidationResult.CreateValidResult();
      _validMetaValidationResult2 = MetaValidationRuleValidationResult.CreateValidResult();
      _validMetaValidationResult3 = MetaValidationRuleValidationResult.CreateValidResult();
      _validMetaValidationResult4 = MetaValidationRuleValidationResult.CreateValidResult();
      _invalidMetaValidationResult1 = MetaValidationRuleValidationResult.CreateInvalidResult("Error1");
      _invalidMetaValidationResult2 = MetaValidationRuleValidationResult.CreateInvalidResult("Error2");
      _invalidMetaValidationResult3 = MetaValidationRuleValidationResult.CreateInvalidResult("Error3");
      _invalidMetaValidationResult4 = MetaValidationRuleValidationResult.CreateInvalidResult("Error4");
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_validatorBuilder.ValidationRuleCollectorProvider, Is.SameAs(_validationRuleCollectorProviderMock.Object));
      Assert.That(_validatorBuilder.ValidationRuleCollectorMerger, Is.SameAs(_validationRuleCollectorMergerMock.Object));
    }

    [Test]
    public void BuildValidator ()
    {
      ExpectMocks();

      _collectorValidatorMock.Setup(mock => mock.CheckValid(_validationRuleCollectorStub1.Object)).Verifiable();
      _collectorValidatorMock.Setup(mock => mock.CheckValid(_validationRuleCollectorStub2.Object)).Verifiable();
      _collectorValidatorMock.Setup(mock => mock.CheckValid(_validationRuleCollectorStub3.Object)).Verifiable();

      _propertyMetaValidationRuleValidatorMock
          .Setup(mock => mock.Validate(_fakeAddingPropertyValidationRulesCollectorResult))
          .Returns(new[] { _validMetaValidationResult1, _validMetaValidationResult2 })
          .Verifiable();

      _objectMetaValidationRuleValidatorMock
          .Setup(mock => mock.Validate(_fakeAddingObjectValidationRulesCollectorResult))
          .Returns(new[] { _validMetaValidationResult3, _validMetaValidationResult4 })
          .Verifiable();

      var validationRuleStub1 = new Mock<IValidationRule>();
      var validationRuleStub2 = new Mock<IValidationRule>();
      var validationRuleStub4 = new Mock<IValidationRule>();
      var validationRuleStub5 = new Mock<IValidationRule>();
      _addingPropertyValidationRuleCollectorStub1.Setup(_ => _.CreateValidationRule(_validationMessageFactoryStub.Object)).Returns(validationRuleStub1.Object);
      _addingPropertyValidationRuleCollectorStub2.Setup(_ => _.CreateValidationRule(_validationMessageFactoryStub.Object)).Returns(validationRuleStub2.Object);
      _addingObjectValidationRuleCollectorStub1.Setup(_ => _.CreateValidationRule(_validationMessageFactoryStub.Object)).Returns(validationRuleStub4.Object);
      _addingObjectValidationRuleCollectorStub2.Setup(_ => _.CreateValidationRule(_validationMessageFactoryStub.Object)).Returns(validationRuleStub5.Object);

      var result = _validatorBuilder.BuildValidator(typeof(SpecialCustomer1));

      _validationRuleCollectorProviderMock.Verify();
      _validationRuleCollectorMergerMock.Verify();
      _propertyMetaValidationRuleValidatorMock.Verify();
      _objectMetaValidationRuleValidatorMock.Verify();
      _memberInformationNameResolverMock.Verify();
      Assert.That(result, Is.TypeOf(typeof(Validator)));
      var validator = (Validator)result;
      var validationRules = validator.ValidationRules.ToArray();
      Assert.That(validationRules.Length, Is.EqualTo(8));
      Assert.That(validationRules[0], Is.SameAs(validationRuleStub1.Object));
      Assert.That(validationRules[1], Is.SameAs(validationRuleStub2.Object));
      Assert.That(validationRules[2], Is.InstanceOf<PropertyValidationRule<Customer, string>>());
      Assert.That(((IPropertyValidationRule)validationRules[2]).Property, Is.SameAs(_addingPropertyValidationRuleCollectorStub3.Property));
      Assert.That(validationRules[3], Is.InstanceOf<PropertyValidationRule<Customer, string>>());
      Assert.That(((IPropertyValidationRule)validationRules[3]).Property, Is.SameAs(_addingPropertyValidationRuleCollectorStub4.Property));
      Assert.That(validationRules[4], Is.SameAs(validationRuleStub4.Object));
      Assert.That(validationRules[5], Is.SameAs(validationRuleStub5.Object));
      Assert.That(validationRules[6], Is.InstanceOf<ObjectValidationRule<Customer>>());
      Assert.That(validationRules[7], Is.InstanceOf<ObjectValidationRule<Customer>>());
    }

    [Test]
    public void BuildValidator_ExtensionMethod ()
    {
      ExpectMocks();

      _collectorValidatorMock.Setup(mock => mock.CheckValid(_validationRuleCollectorStub1.Object)).Verifiable();
      _collectorValidatorMock.Setup(mock => mock.CheckValid(_validationRuleCollectorStub2.Object)).Verifiable();
      _collectorValidatorMock.Setup(mock => mock.CheckValid(_validationRuleCollectorStub3.Object)).Verifiable();

      _propertyMetaValidationRuleValidatorMock
          .Setup(mock => mock.Validate(_fakeAddingPropertyValidationRulesCollectorResult))
          .Returns(new[] { _validMetaValidationResult1, _validMetaValidationResult2 })
          .Verifiable();

      _objectMetaValidationRuleValidatorMock
          .Setup(mock => mock.Validate(_fakeAddingObjectValidationRulesCollectorResult))
          .Returns(new[] { _validMetaValidationResult3, _validMetaValidationResult4 })
          .Verifiable();

      var result = _validatorBuilder.BuildValidator<SpecialCustomer1>();

      _validationRuleCollectorProviderMock.Verify();
      _validationRuleCollectorMergerMock.Verify();
      _propertyMetaValidationRuleValidatorMock.Verify();
      _objectMetaValidationRuleValidatorMock.Verify();
      _memberInformationNameResolverMock.Verify();
      Assert.That(result, Is.TypeOf(typeof(TypedValidatorDecorator<SpecialCustomer1>)));
    }

    [Test]
    public void BuildValidator_MetaValidationFailed ()
    {
      ExpectMocks();

      _collectorValidatorMock.Setup(mock => mock.CheckValid(_validationRuleCollectorStub1.Object)).Verifiable();
      _collectorValidatorMock.Setup(mock => mock.CheckValid(_validationRuleCollectorStub2.Object)).Verifiable();
      _collectorValidatorMock.Setup(mock => mock.CheckValid(_validationRuleCollectorStub3.Object)).Verifiable();

      _propertyMetaValidationRuleValidatorMock
          .Setup(mock => mock.Validate(_fakeAddingPropertyValidationRulesCollectorResult))
          .Returns(new[] { _validMetaValidationResult1, _invalidMetaValidationResult1, _validMetaValidationResult2, _invalidMetaValidationResult2 })
          .Verifiable();

      _objectMetaValidationRuleValidatorMock
          .Setup(mock => mock.Validate(_fakeAddingObjectValidationRulesCollectorResult))
          .Returns(new[] { _validMetaValidationResult3, _invalidMetaValidationResult3, _validMetaValidationResult4, _invalidMetaValidationResult4 })
          .Verifiable();

      Assert.That(
          () => _validatorBuilder.BuildValidator<SpecialCustomer1>(),
          Throws.TypeOf<ValidationConfigurationException>()
              .And.Message.EqualTo("Error1\r\n----------\r\nError2\r\n----------\r\nError3\r\n----------\r\nError4"));
    }

    private void ExpectMocks ()
    {
      var validationCollectorInfos = new[] { new[] { _validationRuleCollectorInfo1, _validationRuleCollectorInfo3 }, new[] { _validationRuleCollectorInfo2 } };
      _validationRuleCollectorProviderMock
          .Setup(mock => mock.GetValidationRuleCollectors(new[] { typeof(SpecialCustomer1) }))
          .Returns(validationCollectorInfos)
          .Verifiable();

      _validationRuleCollectorMergerMock
          .Setup(
              mock => mock.Merge(
                  It.Is<IEnumerable<IEnumerable<ValidationRuleCollectorInfo>>>(
                      c =>
                          c.SelectMany(g => g).ElementAt(0).Equals(_validationRuleCollectorInfo1) &&
                          c.SelectMany(g => g).ElementAt(1).Equals(_validationRuleCollectorInfo3) &&
                          c.SelectMany(g => g).ElementAt(2).Equals(_validationRuleCollectorInfo2))))
          .Returns(_fakeValidationCollectorMergeResult)
          .Verifiable();

      _propertyMetaValidationRuleValidatorFactoryStub
          .Setup(
              stub => stub.CreatePropertyMetaValidationRuleValidator(
                  new[]
                      {
                          _propertyMetaValidationRuleCollector1Stub.Object, _propertyMetaValidationRuleCollector2Stub.Object,
                          _propertyMetaValidationRuleCollector3Stub.Object
                      }))
          .Returns(_propertyMetaValidationRuleValidatorMock.Object);

      _objectMetaValidationRuleValidatorFactoryStub
          .Setup(
              stub => stub.CreateObjectMetaValidationRuleValidator(
                  new[]
                      {
                          _objectMetaValidationRuleCollector1Stub.Object, _objectMetaValidationRuleCollector2Stub.Object,
                          _objectMetaValidationRuleCollector3Stub.Object
                      }))
          .Returns(_objectMetaValidationRuleValidatorMock.Object);
    }
  }
}
