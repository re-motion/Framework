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
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.RuleBuilders
{
  [TestFixture]
  public class AddingPropertyValidationRuleBuilderTest
  {
    private IAddingPropertyValidationRuleCollector _addingPropertyValidationRuleCollectorMock;
    private AddingPropertyValidationRuleBuilder<Customer, string> _addingPropertyValidationBuilder;
    private IPropertyValidator _propertyValidatorStub;
    private IPropertyMetaValidationRuleCollector _propertyMetaValidationRuleCollectorMock;

    [SetUp]
    public void SetUp ()
    {
      _addingPropertyValidationRuleCollectorMock = MockRepository.GenerateStrictMock<IAddingPropertyValidationRuleCollector>();
      _addingPropertyValidationRuleCollectorMock.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName")));

      _propertyMetaValidationRuleCollectorMock = MockRepository.GenerateStrictMock<IPropertyMetaValidationRuleCollector>();
      _addingPropertyValidationRuleCollectorMock.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName")));

      _addingPropertyValidationBuilder = new AddingPropertyValidationRuleBuilder<Customer, string> (
          _addingPropertyValidationRuleCollectorMock,
          _propertyMetaValidationRuleCollectorMock);

      _propertyValidatorStub = MockRepository.GenerateStub<IPropertyValidator>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_addingPropertyValidationBuilder.AddingPropertyValidationRuleCollector, Is.SameAs (_addingPropertyValidationRuleCollectorMock));
      Assert.That (_addingPropertyValidationBuilder.PropertyMetaValidationRuleCollector, Is.SameAs (_propertyMetaValidationRuleCollectorMock));
    }

    [Test]
    public void SetValidator ()
    {
      Func<PropertyRuleInitializationParameters, IPropertyValidator> validatorFactory = _ => _propertyValidatorStub;
      _addingPropertyValidationRuleCollectorMock.Expect (mock => mock.RegisterValidator (validatorFactory));

      ((IAddingPropertyValidationRuleBuilder<Customer, string>) _addingPropertyValidationBuilder).SetValidator (validatorFactory);

      _addingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void NotRemovable ()
    {
      _addingPropertyValidationRuleCollectorMock.Expect (mock => mock.SetHardConstraint());

      _addingPropertyValidationBuilder.NotRemovable();

      _addingPropertyValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void AddMetaValidationRule ()
    {
      var metaValidationRuleStub = MockRepository.GenerateStub<IMetaValidationRule>();

      _propertyMetaValidationRuleCollectorMock.Expect (mock => mock.RegisterMetaValidationRule (metaValidationRuleStub));

      _addingPropertyValidationBuilder.AddMetaValidationRule (metaValidationRuleStub);

      _propertyMetaValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void AddMetaValidationRule_FuncOverload ()
    {
      var fakeValidationResult = MetaValidationRuleValidationResult.CreateValidResult();

      _propertyMetaValidationRuleCollectorMock.Expect (
          mock => mock.RegisterMetaValidationRule (
              Arg<IMetaValidationRule>.Matches (
                  rule =>
                      rule.GetType() == typeof (DelegateMetaValidationRule<IPropertyValidator>) &&
                      rule.Validate (new IPropertyValidator[0]).First() == fakeValidationResult)));

      _addingPropertyValidationBuilder.AddMetaValidationRule (v => fakeValidationResult);

      _propertyMetaValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void AddMetaValidationRule_ExpressionOverload_IsValidIsTrue ()
    {
      _propertyMetaValidationRuleCollectorMock.Expect (
          mock => mock.RegisterMetaValidationRule (
              Arg<IMetaValidationRule>.Matches (
                  rule =>
                      rule.GetType() == typeof (DelegateMetaValidationRule<IPropertyValidator>)
                      && rule.Validate (new IPropertyValidator[0]).First().IsValid)));

      _addingPropertyValidationBuilder.AddMetaValidationRule<IPropertyValidator> (v => true);

      _propertyMetaValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    public void AddMetaValidationRule_ExpressionOverload_IsValidIsFalse ()
    {
      _propertyMetaValidationRuleCollectorMock.Expect (
          mock => mock.RegisterMetaValidationRule (
              Arg<IMetaValidationRule>.Matches (
                  rule =>
                      rule.GetType() == typeof (DelegateMetaValidationRule<IPropertyValidator>) &&
                      !rule.Validate (new IPropertyValidator[0]).First().IsValid &&
                      rule.Validate (new IPropertyValidator[0]).First().Message
                      == "Meta validation rule 'v => False' failed for validator 'Remotion.Validation.Validators.IPropertyValidator' "
                      + "on property 'Remotion.Validation.UnitTests.TestDomain.Customer.UserName'.")));

      _addingPropertyValidationBuilder.AddMetaValidationRule<IPropertyValidator> (v => false);

      _propertyMetaValidationRuleCollectorMock.VerifyAllExpectations();
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void SetCondition ()
    {
    }
  }
}