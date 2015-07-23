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
using FluentValidation;
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.RuleBuilders
{
  [TestFixture]
  public class AddingComponentRuleBuilderTest
  {
    private IAddingComponentPropertyRule _addingComponentPropertyRuleMock;
    private AddingComponentRuleBuilder<Customer, string> _addingComponentBuilder;
    private IPropertyValidator _propertyValidatorStub;
    private IAddingComponentPropertyMetaValidationRule _addingComponentPropertyMetaValidationRuleMock;

    [SetUp]
    public void SetUp ()
    {
      _addingComponentPropertyRuleMock = MockRepository.GenerateStrictMock<IAddingComponentPropertyRule>();
      _addingComponentPropertyRuleMock.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName")));

      _addingComponentPropertyMetaValidationRuleMock = MockRepository.GenerateStrictMock<IAddingComponentPropertyMetaValidationRule>();
      _addingComponentPropertyRuleMock.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName")));

      _addingComponentBuilder = new AddingComponentRuleBuilder<Customer, string> (
          _addingComponentPropertyRuleMock,
          _addingComponentPropertyMetaValidationRuleMock);

      _propertyValidatorStub = MockRepository.GenerateStub<IPropertyValidator>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_addingComponentBuilder.AddingComponentPropertyRule, Is.SameAs (_addingComponentPropertyRuleMock));
      Assert.That (_addingComponentBuilder.AddingMetaValidationPropertyRule, Is.SameAs (_addingComponentPropertyMetaValidationRuleMock));
    }

    [Test]
    public void SetValidator ()
    {
      _addingComponentPropertyRuleMock.Expect (mock => mock.RegisterValidator (_propertyValidatorStub));

      ((IRuleBuilder<Customer, string>) _addingComponentBuilder).SetValidator (_propertyValidatorStub);

      _addingComponentPropertyRuleMock.VerifyAllExpectations();
    }

    [Test]
    public void NotRemovable ()
    {
      _addingComponentPropertyRuleMock.Expect (mock => mock.SetHardConstraint());

      _addingComponentBuilder.NotRemovable();

      _addingComponentPropertyRuleMock.VerifyAllExpectations();
    }

    [Test]
    public void AddMetaValidationRule ()
    {
      var metaValidationRuleStub = MockRepository.GenerateStub<IMetaValidationRule>();

      _addingComponentPropertyMetaValidationRuleMock.Expect (mock => mock.RegisterMetaValidationRule (metaValidationRuleStub));

      _addingComponentBuilder.AddMetaValidationRule (metaValidationRuleStub);

      _addingComponentPropertyMetaValidationRuleMock.VerifyAllExpectations();
    }

    [Test]
    public void AddMetaValidationRule_FuncOverload ()
    {
      var fakeValidationResult = MetaValidationRuleValidationResult.CreateValidResult();

      _addingComponentPropertyMetaValidationRuleMock.Expect (
          mock => mock.RegisterMetaValidationRule (
              Arg<IMetaValidationRule>.Matches (
                  rule =>
                      rule.GetType() == typeof (DelegateMetaValidationRule<IPropertyValidator>) &&
                      rule.Validate (new IPropertyValidator[0]).First() == fakeValidationResult)));

      _addingComponentBuilder.AddMetaValidationRule (v => fakeValidationResult);

      _addingComponentPropertyMetaValidationRuleMock.VerifyAllExpectations();
    }

    [Test]
    public void AddMetaValidationRule_ExpressionOverload_IsValidIsTrue ()
    {
      _addingComponentPropertyMetaValidationRuleMock.Expect (
          mock => mock.RegisterMetaValidationRule (
              Arg<IMetaValidationRule>.Matches (
                  rule =>
                      rule.GetType() == typeof (DelegateMetaValidationRule<IPropertyValidator>)
                      && rule.Validate (new IPropertyValidator[0]).First().IsValid)));

      _addingComponentBuilder.AddMetaValidationRule<IPropertyValidator> (v => true);

      _addingComponentPropertyMetaValidationRuleMock.VerifyAllExpectations();
    }

    [Test]
    public void AddMetaValidationRule_ExpressionOverload_IsValidIsFalse ()
    {
      _addingComponentPropertyMetaValidationRuleMock.Expect (
          mock => mock.RegisterMetaValidationRule (
              Arg<IMetaValidationRule>.Matches (
                  rule =>
                      rule.GetType() == typeof (DelegateMetaValidationRule<IPropertyValidator>) &&
                      !rule.Validate (new IPropertyValidator[0]).First().IsValid &&
                      rule.Validate (new IPropertyValidator[0]).First().Message
                      == "Meta validation rule 'v => False' failed for validator 'FluentValidation.Validators.IPropertyValidator' "
                      + "on property 'Remotion.Validation.UnitTests.TestDomain.Customer.UserName'.")));

      _addingComponentBuilder.AddMetaValidationRule<IPropertyValidator> (v => false);

      _addingComponentPropertyMetaValidationRuleMock.VerifyAllExpectations();
    }
  }
}