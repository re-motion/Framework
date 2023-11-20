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
using Remotion.Utilities;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.RuleBuilders
{
  [TestFixture]
  public class AddingPropertyValidationRuleBuilderTest
  {
    private Mock<IAddingPropertyValidationRuleCollector> _addingPropertyValidationRuleCollectorMock;
    private AddingPropertyValidationRuleBuilder<Customer, string> _addingPropertyValidationBuilder;
    private Mock<IPropertyValidator> _propertyValidatorStub;
    private Mock<IPropertyMetaValidationRuleCollector> _propertyMetaValidationRuleCollectorMock;

    [SetUp]
    public void SetUp ()
    {
      _addingPropertyValidationRuleCollectorMock = new Mock<IAddingPropertyValidationRuleCollector>(MockBehavior.Strict);
      _addingPropertyValidationRuleCollectorMock
          .Setup(stub => stub.Property)
          .Returns(PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Customer _) => _.UserName)));

      _propertyMetaValidationRuleCollectorMock = new Mock<IPropertyMetaValidationRuleCollector>(MockBehavior.Strict);
      _addingPropertyValidationRuleCollectorMock
          .Setup(stub => stub.Property)
          .Returns(PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Customer _) => _.UserName)));

      _addingPropertyValidationBuilder = new AddingPropertyValidationRuleBuilder<Customer, string>(
          _addingPropertyValidationRuleCollectorMock.Object,
          _propertyMetaValidationRuleCollectorMock.Object);

      _propertyValidatorStub = new Mock<IPropertyValidator>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_addingPropertyValidationBuilder.AddingPropertyValidationRuleCollector, Is.SameAs(_addingPropertyValidationRuleCollectorMock.Object));
      Assert.That(_addingPropertyValidationBuilder.PropertyMetaValidationRuleCollector, Is.SameAs(_propertyMetaValidationRuleCollectorMock.Object));
    }

    [Test]
    public void SetValidator ()
    {
      Func<PropertyValidationRuleInitializationParameters, IPropertyValidator> validatorFactory = _ => _propertyValidatorStub.Object;
      _addingPropertyValidationRuleCollectorMock
          .Setup(mock => mock.RegisterValidator(validatorFactory))
          .Verifiable();

      ((IAddingPropertyValidationRuleBuilder<Customer, string>)_addingPropertyValidationBuilder).SetValidator(validatorFactory);

      _addingPropertyValidationRuleCollectorMock.Verify();
    }

    [Test]
    public void CanBeRemoved ()
    {
      _addingPropertyValidationRuleCollectorMock
          .Setup(mock => mock.SetRemovable())
          .Verifiable();

      _addingPropertyValidationBuilder.CanBeRemoved();

      _addingPropertyValidationRuleCollectorMock.Verify();
    }

    [Test]
    public void AddMetaValidationRule ()
    {
      var metaValidationRuleStub = new Mock<IPropertyMetaValidationRule>();

      _propertyMetaValidationRuleCollectorMock
          .Setup(mock => mock.RegisterMetaValidationRule(metaValidationRuleStub.Object))
          .Verifiable();

      _addingPropertyValidationBuilder.AddMetaValidationRule(metaValidationRuleStub.Object);

      _propertyMetaValidationRuleCollectorMock.Verify();
    }

    [Test]
    public void AddMetaValidationRule_FuncOverload ()
    {
      var fakeValidationResult = MetaValidationRuleValidationResult.CreateValidResult();

      IPropertyMetaValidationRule actualRule = null;
      _propertyMetaValidationRuleCollectorMock
          .Setup(mock => mock.RegisterMetaValidationRule(It.IsAny<IPropertyMetaValidationRule>()))
          .Callback((IPropertyMetaValidationRule r) => actualRule = r);

      _addingPropertyValidationBuilder.AddMetaValidationRule(v => fakeValidationResult);

      Assert.That(actualRule, Is.TypeOf<DelegatePropertyMetaValidationRule<IPropertyValidator>>());
      Assert.That(actualRule.Validate(new IPropertyValidator[0]), Is.EquivalentTo(new [] { fakeValidationResult }));
    }

    [Test]
    public void AddMetaValidationRule_ExpressionOverload_IsValidIsTrue ()
    {
      IPropertyMetaValidationRule actualRule = null;
      _propertyMetaValidationRuleCollectorMock
          .Setup(mock => mock.RegisterMetaValidationRule(It.IsAny<IPropertyMetaValidationRule>()))
          .Callback((IPropertyMetaValidationRule r) => actualRule = r);

      _addingPropertyValidationBuilder.AddMetaValidationRule<IPropertyValidator>(v => true);

      Assert.That(actualRule, Is.TypeOf<DelegatePropertyMetaValidationRule<IPropertyValidator>>());
      Assert.That(actualRule.Validate(new IPropertyValidator[0]), Has.Exactly(1).With.Property(nameof(MetaValidationRuleValidationResult.IsValid)).True);
    }

    [Test]
    public void AddMetaValidationRule_ExpressionOverload_IsValidIsFalse ()
    {
      IPropertyMetaValidationRule actualRule = null;
      _propertyMetaValidationRuleCollectorMock
          .Setup(mock => mock.RegisterMetaValidationRule(It.IsAny<IPropertyMetaValidationRule>()))
          .Callback((IPropertyMetaValidationRule r) => actualRule = r);

      _addingPropertyValidationBuilder.AddMetaValidationRule<IPropertyValidator>(v => false);

      Assert.That(actualRule, Is.TypeOf<DelegatePropertyMetaValidationRule<IPropertyValidator>>());
      Assert.That(actualRule.Validate(new IPropertyValidator[0]), Has.Exactly(1).Items);
      Assert.That(actualRule.Validate(new IPropertyValidator[0]), Has.All.With.Property(nameof(MetaValidationRuleValidationResult.IsValid)).False);
      Assert.That(
          actualRule.Validate(new IPropertyValidator[0]),
          Has.All.With.Property(nameof(MetaValidationRuleValidationResult.Message))
              .EqualTo(
                  "Meta validation rule 'v => False' failed for validator 'Remotion.Validation.Validators.IPropertyValidator' "
                  + "on property 'Remotion.Validation.UnitTests.TestDomain.Customer.UserName'."));
    }

    [Test]
    public void SetCondition ()
    {
      Func<Customer, bool> predicate = _ => true;

      _addingPropertyValidationRuleCollectorMock
          .Setup(mock => mock.SetCondition(predicate))
          .Verifiable();

      _addingPropertyValidationBuilder.SetCondition(predicate);

      _addingPropertyValidationRuleCollectorMock.Verify();
    }
  }
}
