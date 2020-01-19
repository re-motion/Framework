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
using Remotion.Utilities;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.MetaValidation
{
  [TestFixture]
  public class PropertyMetaValidationRuleValidatorTest
  {
    private IValidationRuleCollector _collectorStub1;
    private IValidationRuleCollector _collectorStub2;
    private PropertyMetaValidationRuleValidator _validator;
    private IPropertyMetaValidationRuleCollector _propertyMetaValidationRuleCollectorStub1;
    private IPropertyMetaValidationRuleCollector _propertyMetaValidationRuleCollectorStub2;
    private IPropertyMetaValidationRuleCollector _propertyMetaValidationRuleCollectorStub3;
    private IPropertyMetaValidationRuleCollector _propertyMetaValidationRuleCollectorStub4;
    private IPropertyValidator _propertyValidatorStub1;
    private IPropertyValidator _propertyValidatorStub2;
    private IPropertyValidator _propertyValidatorStub3;
    private IPropertyValidator _propertyValidatorStub4;
    private IPropertyValidator _propertyValidatorStub5;
    private ISystemPropertyMetaValidationRuleProvider _systemPropertyMetaValidationRuleProviderStub;
    private ISystemPropertyMetaValidationRuleProviderFactory _systemPropertyMetaRuleProviderFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _collectorStub1 = MockRepository.GenerateStub<IValidationRuleCollector>();
      _collectorStub2 = MockRepository.GenerateStub<IValidationRuleCollector>();

      _propertyMetaValidationRuleCollectorStub1 = MockRepository.GenerateStub<IPropertyMetaValidationRuleCollector>();
      _propertyMetaValidationRuleCollectorStub2 = MockRepository.GenerateStub<IPropertyMetaValidationRuleCollector>();
      _propertyMetaValidationRuleCollectorStub3 = MockRepository.GenerateStub<IPropertyMetaValidationRuleCollector>();
      _propertyMetaValidationRuleCollectorStub4 = MockRepository.GenerateStub<IPropertyMetaValidationRuleCollector>();

      _collectorStub1.Stub (stub => stub.PropertyMetaValidationRules)
          .Return (new[] { _propertyMetaValidationRuleCollectorStub1, _propertyMetaValidationRuleCollectorStub2 });
      _collectorStub2.Stub (stub => stub.PropertyMetaValidationRules).Return (new[] { _propertyMetaValidationRuleCollectorStub3 });

      _propertyValidatorStub1 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub2 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub3 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub4 = MockRepository.GenerateStub<IPropertyValidator>();
      _propertyValidatorStub5 = MockRepository.GenerateStub<IPropertyValidator>();

      _systemPropertyMetaRuleProviderFactoryStub = MockRepository.GenerateStub<ISystemPropertyMetaValidationRuleProviderFactory> ();
      _systemPropertyMetaValidationRuleProviderStub = MockRepository.GenerateStub<ISystemPropertyMetaValidationRuleProvider>();

      _validator =
          new PropertyMetaValidationRuleValidator (
              new[]
              {
                  _propertyMetaValidationRuleCollectorStub1, _propertyMetaValidationRuleCollectorStub2,
                  _propertyMetaValidationRuleCollectorStub3, _propertyMetaValidationRuleCollectorStub4
              },
              _systemPropertyMetaRuleProviderFactoryStub);
    }

    [Test]
    public void Validate ()
    {
      var userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.UserName);
      var lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Person, string> (c => c.LastName);
      var otherPropertyExpression = ExpressionHelper.GetTypedMemberExpression<Person, DateTime> (c => c.Birthday);
      var filteredPropertyStub = MockRepository.GenerateStub<IPropertyInformation>();

      var propertyRule1 = AddingPropertyValidationRuleCollector.Create (userNameExpression, typeof (CustomerValidationRuleCollector1));
      var propertyRule2 = AddingPropertyValidationRuleCollector.Create (lastNameExpression, typeof (CustomerValidationRuleCollector1));
      var propertyRule3 = AddingPropertyValidationRuleCollector.Create (lastNameExpression, typeof (CustomerValidationRuleCollector2));
      var filteredPropertyRuleStub = MockRepository.GenerateStub<IAddingPropertyValidationRuleCollector>();
      filteredPropertyRuleStub.Stub (_ => _.Property).Return (filteredPropertyStub);
      filteredPropertyRuleStub.Stub (_ => _.Validators).Return (new[] { MockRepository.GenerateStub<IPropertyValidator>() });

      propertyRule1.RegisterValidator (_ => _propertyValidatorStub1);
      propertyRule1.RegisterValidator (_ => _propertyValidatorStub2);
      propertyRule2.RegisterValidator (_ => _propertyValidatorStub3);
      propertyRule2.RegisterValidator (_ => _propertyValidatorStub4);
      propertyRule3.RegisterValidator (_ => _propertyValidatorStub5);

      var systemMetaValidationRuleMock1 = MockRepository.GenerateStrictMock<IPropertyMetaValidationRule>();
      var systemMetaValidationRuleMock2 = MockRepository.GenerateStrictMock<IPropertyMetaValidationRule>();

      _systemPropertyMetaRuleProviderFactoryStub.Stub (stub => stub.Create (Arg<IPropertyInformation>.Is.Anything)).Return (_systemPropertyMetaValidationRuleProviderStub);
      _systemPropertyMetaValidationRuleProviderStub.Stub (stub => stub.GetSystemPropertyMetaValidationRules())
          .Return (new[] { systemMetaValidationRuleMock1, systemMetaValidationRuleMock2 });

      var metaValidationRuleMock1 = MockRepository.GenerateStrictMock<IPropertyMetaValidationRule>();
      var metaValidationRuleMock2 = MockRepository.GenerateStrictMock<IPropertyMetaValidationRule>();
      var metaValidationRuleMock3 = MockRepository.GenerateStrictMock<IPropertyMetaValidationRule>();
      var metaValidationRuleMock4 = MockRepository.GenerateStrictMock<IPropertyMetaValidationRule>();

      _propertyMetaValidationRuleCollectorStub1.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty (userNameExpression)));
      _propertyMetaValidationRuleCollectorStub1.Stub (stub => stub.MetaValidationRules).Return (new[] { metaValidationRuleMock1, metaValidationRuleMock2 });

      _propertyMetaValidationRuleCollectorStub2.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty (lastNameExpression)));
      _propertyMetaValidationRuleCollectorStub2.Stub (stub => stub.MetaValidationRules).Return (new[] { metaValidationRuleMock3 });

      _propertyMetaValidationRuleCollectorStub3.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty (lastNameExpression)));
      _propertyMetaValidationRuleCollectorStub3.Stub (stub => stub.MetaValidationRules).Return (new IPropertyMetaValidationRule[0]);

      _propertyMetaValidationRuleCollectorStub4.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty (otherPropertyExpression)));
      _propertyMetaValidationRuleCollectorStub4.Stub (stub => stub.MetaValidationRules).Return (new[] { metaValidationRuleMock4 });

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

      systemMetaValidationRuleMock1
          .Expect (mock =>mock.Validate (Arg<IEnumerable<IPropertyValidator>>.List.Equal (new IPropertyValidator[0])))
          .Return (new MetaValidationRuleValidationResult[0]);
      systemMetaValidationRuleMock2
          .Expect (mock =>mock.Validate (Arg<IEnumerable<IPropertyValidator>>.List.Equal (new IPropertyValidator[0])))
          .Return (new MetaValidationRuleValidationResult[0]);
      metaValidationRuleMock4
          .Expect (mock => mock.Validate (Arg<IEnumerable<IPropertyValidator>>.List.Equal (new IPropertyValidator[0])))
          .Return (new MetaValidationRuleValidationResult[0]);

      var result = _validator.Validate (new [] { propertyRule1, propertyRule2, filteredPropertyRuleStub, propertyRule3 }).ToArray();

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