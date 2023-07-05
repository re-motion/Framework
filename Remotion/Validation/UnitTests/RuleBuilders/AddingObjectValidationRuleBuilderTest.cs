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
using Moq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.RuleBuilders
{
  [TestFixture]
  public class AddingObjectValidationRuleBuilderTest
  {
    private Mock<IAddingObjectValidationRuleCollector> _addingObjectValidationRuleCollectorMock;
    private AddingObjectValidationRuleBuilder<Customer> _addingObjectValidationBuilder;
    private Mock<IObjectValidator> _objectValidatorStub;
    private Mock<IObjectMetaValidationRuleCollector> _objectMetaValidationRuleCollectorMock;

    [SetUp]
    public void SetUp ()
    {
      _addingObjectValidationRuleCollectorMock = new Mock<IAddingObjectValidationRuleCollector>(MockBehavior.Strict);
      _addingObjectValidationRuleCollectorMock
          .Setup(stub => stub.ValidatedType)
          .Returns(TypeAdapter.Create(typeof(Customer)));

      _objectMetaValidationRuleCollectorMock = new Mock<IObjectMetaValidationRuleCollector>(MockBehavior.Strict);
      _addingObjectValidationRuleCollectorMock
          .Setup(stub => stub.ValidatedType)
          .Returns(TypeAdapter.Create(typeof(Customer)));

      _addingObjectValidationBuilder = new AddingObjectValidationRuleBuilder<Customer>(
          _addingObjectValidationRuleCollectorMock.Object,
          _objectMetaValidationRuleCollectorMock.Object);

      _objectValidatorStub = new Mock<IObjectValidator>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_addingObjectValidationBuilder.AddingObjectValidationRuleCollector, Is.SameAs(_addingObjectValidationRuleCollectorMock.Object));
      Assert.That(_addingObjectValidationBuilder.ObjectMetaValidationRuleCollector, Is.SameAs(_objectMetaValidationRuleCollectorMock.Object));
    }

    [Test]
    public void SetValidator ()
    {
      Func<ObjectValidationRuleInitializationParameters, IObjectValidator> validatorFactory = _ => _objectValidatorStub.Object;
      _addingObjectValidationRuleCollectorMock
          .Setup(mock => mock.RegisterValidator(validatorFactory))
          .Verifiable();

      ((IAddingObjectValidationRuleBuilder<Customer>)_addingObjectValidationBuilder).SetValidator(validatorFactory);

      _addingObjectValidationRuleCollectorMock.Verify();
    }

    [Test]
    public void CanBeRemoved ()
    {
      _addingObjectValidationRuleCollectorMock
          .Setup(mock => mock.SetRemovable())
          .Verifiable();

      _addingObjectValidationBuilder.CanBeRemoved();

      _addingObjectValidationRuleCollectorMock.Verify();
    }

    [Test]
    public void AddMetaValidationRule ()
    {
      var metaValidationRuleStub = new Mock<IObjectMetaValidationRule>();

      _objectMetaValidationRuleCollectorMock
          .Setup(mock => mock.RegisterMetaValidationRule(metaValidationRuleStub.Object))
          .Verifiable();

      _addingObjectValidationBuilder.AddMetaValidationRule(metaValidationRuleStub.Object);

      _objectMetaValidationRuleCollectorMock.Verify();
    }

    [Test]
    public void AddMetaValidationRule_FuncOverload ()
    {
      var fakeValidationResult = MetaValidationRuleValidationResult.CreateValidResult();

      IObjectMetaValidationRule actualRule = null;
      _objectMetaValidationRuleCollectorMock
          .Setup(mock => mock.RegisterMetaValidationRule(It.IsAny<IObjectMetaValidationRule>()))
          .Callback((IObjectMetaValidationRule r) => actualRule = r);

      _addingObjectValidationBuilder.AddMetaValidationRule(v => fakeValidationResult);

      Assert.That(actualRule, Is.TypeOf<DelegateObjectMetaValidationRule<IObjectValidator>>());
      Assert.That(actualRule.Validate(new IObjectValidator[0]), Is.EquivalentTo(new [] { fakeValidationResult }));
    }

    [Test]
    public void AddMetaValidationRule_ExpressionOverload_IsValidIsTrue ()
    {
      IObjectMetaValidationRule actualRule = null;
      _objectMetaValidationRuleCollectorMock
          .Setup(mock => mock.RegisterMetaValidationRule(It.IsAny<IObjectMetaValidationRule>()))
          .Callback((IObjectMetaValidationRule r) => actualRule = r);

      _addingObjectValidationBuilder.AddMetaValidationRule<IObjectValidator>(v => true);

      Assert.That(actualRule, Is.TypeOf<DelegateObjectMetaValidationRule<IObjectValidator>>());
      Assert.That(actualRule.Validate(new IObjectValidator[0]), Has.Exactly(1).With.Property(nameof(MetaValidationRuleValidationResult.IsValid)).True);
    }

    [Test]
    public void AddMetaValidationRule_ExpressionOverload_IsValidIsFalse ()
    {
      IObjectMetaValidationRule actualRule = null;
      _objectMetaValidationRuleCollectorMock
          .Setup(mock => mock.RegisterMetaValidationRule(It.IsAny<IObjectMetaValidationRule>()))
          .Callback((IObjectMetaValidationRule r) => actualRule = r);

      _addingObjectValidationBuilder.AddMetaValidationRule<IObjectValidator>(v => false);

      Assert.That(actualRule, Is.TypeOf<DelegateObjectMetaValidationRule<IObjectValidator>>());
      Assert.That(actualRule.Validate(new IObjectValidator[0]), Has.Exactly(1).Items);
      Assert.That(actualRule.Validate(new IObjectValidator[0]), Has.All.With.Property(nameof(MetaValidationRuleValidationResult.IsValid)).False);
      Assert.That(
          actualRule.Validate(new IObjectValidator[0]),
          Has.All.With.Property(nameof(MetaValidationRuleValidationResult.Message))
              .EqualTo(
                  "Meta validation rule 'v => False' failed for validator 'Remotion.Validation.Validators.IObjectValidator' "
                  + "on type 'Remotion.Validation.UnitTests.TestDomain.Customer'."));
    }

    [Test]
    public void SetCondition ()
    {
      Func<Customer, bool> predicate = _ => true;

      _addingObjectValidationRuleCollectorMock
          .Setup(mock => mock.SetCondition(predicate))
          .Verifiable();

      _addingObjectValidationBuilder.SetCondition(predicate);

      _addingObjectValidationRuleCollectorMock.Verify();
    }
  }
}
