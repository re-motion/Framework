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
using System.Reflection;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.MetaValidation
{
  [TestFixture]
  public class MetaRulesValidatorTest
  {
    private IComponentValidationCollector _collectorStub1;
    private IComponentValidationCollector _collectorStub2;
    private MetaRulesValidator _validator;
    private IAddingComponentPropertyMetaValidationRule _propertyMetaValidationRuleStub1;
    private IAddingComponentPropertyMetaValidationRule _propertyMetaValidationRuleStub2;
    private IAddingComponentPropertyMetaValidationRule _propertyMetaValidationRuleStub3;
    private IPropertyValidator _propertyValidatorStub1;
    private IPropertyValidator _propertyValidatorStub2;
    private IPropertyValidator _propertyValidatorStub3;
    private IPropertyValidator _propertyValidatorStub4;
    private IPropertyValidator _propertyValidatorStub5;
    private ISystemMetaValidationRulesProvider _systemMetaRulesProviderStub;
    private ISystemMetaValidationRulesProviderFactory _systemMetaRulesProviderFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _collectorStub1 = MockRepository.GenerateStub<IComponentValidationCollector>();
      _collectorStub2 = MockRepository.GenerateStub<IComponentValidationCollector>();

      _propertyMetaValidationRuleStub1 = MockRepository.GenerateStub<IAddingComponentPropertyMetaValidationRule>();
      _propertyMetaValidationRuleStub2 = MockRepository.GenerateStub<IAddingComponentPropertyMetaValidationRule>();
      _propertyMetaValidationRuleStub3 = MockRepository.GenerateStub<IAddingComponentPropertyMetaValidationRule>();

      _collectorStub1.Stub (stub => stub.AddedPropertyMetaValidationRules)
          .Return (new[] { _propertyMetaValidationRuleStub1, _propertyMetaValidationRuleStub2 });
      _collectorStub2.Stub (stub => stub.AddedPropertyMetaValidationRules).Return (new[] { _propertyMetaValidationRuleStub3 });

      _propertyValidatorStub1 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub2 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub3 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub4 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub5 = MockRepository.GenerateStub<IPropertyValidator>();

      _systemMetaRulesProviderFactoryStub = MockRepository.GenerateStub<ISystemMetaValidationRulesProviderFactory> ();
      _systemMetaRulesProviderStub = MockRepository.GenerateStub<ISystemMetaValidationRulesProvider>();

      _validator =
          new MetaRulesValidator (
              new[] { _propertyMetaValidationRuleStub1, _propertyMetaValidationRuleStub2, _propertyMetaValidationRuleStub3 },
              _systemMetaRulesProviderFactoryStub);
    }

    [Test]
    public void Validate ()
    {
      var userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.UserName);
      var lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Person, string> (c => c.LastName);

      var propertyRule1 = AddingComponentPropertyRule.Create (userNameExpression, typeof (CustomerValidationCollector1));
      var propertyRule2 = AddingComponentPropertyRule.Create (lastNameExpression, typeof (CustomerValidationCollector1));
      var propertyRule3 = AddingComponentPropertyRule.Create (lastNameExpression, typeof (CustomerValidationCollector2));
      var filteredPropertyRule = MockRepository.GenerateStub<IValidationRule>();

      propertyRule1.AddValidator (_propertyValidatorStub1);
      propertyRule1.AddValidator (_propertyValidatorStub2);
      propertyRule2.AddValidator (_propertyValidatorStub3);
      propertyRule2.AddValidator (_propertyValidatorStub4);
      propertyRule3.AddValidator (_propertyValidatorStub5);

      var systemMetaValidationRuleMock1 = MockRepository.GenerateStrictMock<IMetaValidationRule>();
      var systemMetaValidationRuleMock2 = MockRepository.GenerateStrictMock<IMetaValidationRule>();

      _systemMetaRulesProviderFactoryStub.Stub (stub => stub.Create (Arg<IPropertyInformation>.Is.Anything)).Return (_systemMetaRulesProviderStub);
      _systemMetaRulesProviderStub.Stub (stub => stub.GetSystemMetaValidationRules())
          .Return (new[] { systemMetaValidationRuleMock1, systemMetaValidationRuleMock2 });

      var metaValidationRuleMock1 = MockRepository.GenerateStrictMock<IMetaValidationRule>();
      var metaValidationRuleMock2 = MockRepository.GenerateStrictMock<IMetaValidationRule>();
      var metaValidationRuleMock3 = MockRepository.GenerateStrictMock<IMetaValidationRule>();

      _propertyMetaValidationRuleStub1.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create((PropertyInfo) userNameExpression.GetMember()));
      _propertyMetaValidationRuleStub1.Stub (stub => stub.MetaValidationRules).Return (new[] { metaValidationRuleMock1, metaValidationRuleMock2 });

      _propertyMetaValidationRuleStub2.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create ((PropertyInfo) lastNameExpression.GetMember ()));
      _propertyMetaValidationRuleStub2.Stub (stub => stub.MetaValidationRules).Return (new[] { metaValidationRuleMock3 });

      _propertyMetaValidationRuleStub3.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create ((PropertyInfo) lastNameExpression.GetMember ()));
      _propertyMetaValidationRuleStub3.Stub (stub => stub.MetaValidationRules).Return (new IMetaValidationRule[0]);

      systemMetaValidationRuleMock1
          .Expect (
              mock => mock.Validate (Arg<IEnumerable<IPropertyValidator>>.List.Equal (new[] { _propertyValidatorStub1, _propertyValidatorStub2 })))
          .Return (new[] { MetaValidationRuleValidationResult.CreateInvalidResult ("Error System Mock 1") });
      systemMetaValidationRuleMock2
          .Expect (
              mock => mock.Validate (Arg<IEnumerable<IPropertyValidator>>.List.Equal (new[] { _propertyValidatorStub1, _propertyValidatorStub2 })))
          .Return (new[] { MetaValidationRuleValidationResult.CreateValidResult() });
      metaValidationRuleMock1
          .Expect (
              mock => mock.Validate (Arg<IEnumerable<IPropertyValidator>>.List.Equal (new[] { _propertyValidatorStub1, _propertyValidatorStub2 })))
          .Return (new[] { MetaValidationRuleValidationResult.CreateValidResult() });
      metaValidationRuleMock2
          .Expect (
              mock => mock.Validate (Arg<IEnumerable<IPropertyValidator>>.List.Equal (new[] { _propertyValidatorStub1, _propertyValidatorStub2 })))
          .Return (
              new[]
              { MetaValidationRuleValidationResult.CreateValidResult(), MetaValidationRuleValidationResult.CreateInvalidResult ("Error Mock 2") });

      systemMetaValidationRuleMock1
          .Expect (
              mock =>
                  mock.Validate (
                      Arg<IEnumerable<IPropertyValidator>>.List.Equal (
                          new[] { _propertyValidatorStub3, _propertyValidatorStub4, _propertyValidatorStub5 })))
          .Return (new[] { MetaValidationRuleValidationResult.CreateValidResult() });
      systemMetaValidationRuleMock2
          .Expect (
              mock =>
                  mock.Validate (
                      Arg<IEnumerable<IPropertyValidator>>.List.Equal (
                          new[] { _propertyValidatorStub3, _propertyValidatorStub4, _propertyValidatorStub5 })))
          .Return (new[] { MetaValidationRuleValidationResult.CreateValidResult() });
      metaValidationRuleMock3
          .Expect (
              mock =>
                  mock.Validate (
                      Arg<IEnumerable<IPropertyValidator>>.List.Equal (
                          new[] { _propertyValidatorStub3, _propertyValidatorStub4, _propertyValidatorStub5 })))
          .Return (new[] { MetaValidationRuleValidationResult.CreateValidResult() });

      var result = _validator.Validate (new[] { propertyRule1, propertyRule2, filteredPropertyRule, propertyRule3 }).ToArray();

      systemMetaValidationRuleMock1.VerifyAllExpectations();
      systemMetaValidationRuleMock2.VerifyAllExpectations();
      metaValidationRuleMock1.VerifyAllExpectations();
      metaValidationRuleMock2.VerifyAllExpectations();
      metaValidationRuleMock3.VerifyAllExpectations();
      Assert.That (result.Count(), Is.EqualTo (8));
      Assert.That (result[0].IsValid, Is.False);
      Assert.That (result[1].IsValid, Is.True);
      Assert.That (result[2].IsValid, Is.True);
      Assert.That (result[3].IsValid, Is.True);
      Assert.That (result[4].IsValid, Is.False);
      Assert.That (result[5].IsValid, Is.True);
      Assert.That (result[6].IsValid, Is.True);
      Assert.That (result[7].IsValid, Is.True);
    }
  }
}