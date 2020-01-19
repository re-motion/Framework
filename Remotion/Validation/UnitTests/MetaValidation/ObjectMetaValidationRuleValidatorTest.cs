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
using Remotion.Logging;
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
  public class ObjectMetaValidationRuleValidatorTest
  {
    private IValidationRuleCollector _collectorStub1;
    private IValidationRuleCollector _collectorStub2;
    private ObjectMetaValidationRuleValidator _validator;
    private IObjectMetaValidationRuleCollector _objectMetaValidationRuleCollectorStub1;
    private IObjectMetaValidationRuleCollector _objectMetaValidationRuleCollectorStub2;
    private IObjectMetaValidationRuleCollector _objectMetaValidationRuleCollectorStub3;
    private IObjectMetaValidationRuleCollector _objectMetaValidationRuleCollectorStub4;
    private IObjectValidator _objectValidatorStub1;
    private IObjectValidator _objectValidatorStub2;
    private IObjectValidator _objectValidatorStub3;
    private IObjectValidator _objectValidatorStub4;
    private IObjectValidator _objectValidatorStub5;

    [SetUp]
    public void SetUp ()
    {
      _collectorStub1 = MockRepository.GenerateStub<IValidationRuleCollector>();
      _collectorStub2 = MockRepository.GenerateStub<IValidationRuleCollector>();

      _objectMetaValidationRuleCollectorStub1 = MockRepository.GenerateStub<IObjectMetaValidationRuleCollector>();
      _objectMetaValidationRuleCollectorStub2 = MockRepository.GenerateStub<IObjectMetaValidationRuleCollector>();
      _objectMetaValidationRuleCollectorStub3 = MockRepository.GenerateStub<IObjectMetaValidationRuleCollector>();
      _objectMetaValidationRuleCollectorStub4 = MockRepository.GenerateStub<IObjectMetaValidationRuleCollector>();

      _collectorStub1.Stub (stub => stub.ObjectMetaValidationRules)
          .Return (new[] { _objectMetaValidationRuleCollectorStub1, _objectMetaValidationRuleCollectorStub2 });
      _collectorStub2.Stub (stub => stub.ObjectMetaValidationRules).Return (new[] { _objectMetaValidationRuleCollectorStub3 });

      _objectValidatorStub1 = MockRepository.GenerateStub<IObjectValidator>();
      _objectValidatorStub2 = MockRepository.GenerateStub<IObjectValidator>();
      _objectValidatorStub3 = MockRepository.GenerateStub<IObjectValidator>();
      _objectValidatorStub4 = MockRepository.GenerateStub<IObjectValidator>();
      _objectValidatorStub5 = MockRepository.GenerateStub<IObjectValidator>();

      _validator = new ObjectMetaValidationRuleValidator (
          new[]
          {
              _objectMetaValidationRuleCollectorStub1, _objectMetaValidationRuleCollectorStub2, _objectMetaValidationRuleCollectorStub3,
              _objectMetaValidationRuleCollectorStub4
          });
    }

    [Test]
    public void Validate ()
    {
      var filteredTypeStub = MockRepository.GenerateStub<ITypeInformation>();
      var objectRule1 = AddingObjectValidationRuleCollector.Create<Customer> (typeof (CustomerValidationRuleCollector1));
      var objectRule2 = AddingObjectValidationRuleCollector.Create<Person> (typeof (CustomerValidationRuleCollector1));
      var objectRule3 = AddingObjectValidationRuleCollector.Create<Person> (typeof (CustomerValidationRuleCollector2));
      var filteredObjectRule = MockRepository.GenerateStub<IAddingObjectValidationRuleCollector>();
      filteredObjectRule.Stub (_ => _.ValidatedType).Return (filteredTypeStub);
      filteredObjectRule.Stub (_ => _.Validators).Return (new[] { MockRepository.GenerateStub<IObjectValidator>() });

      objectRule1.RegisterValidator (_ => _objectValidatorStub1);
      objectRule1.RegisterValidator (_ => _objectValidatorStub2);
      objectRule2.RegisterValidator (_ => _objectValidatorStub3);
      objectRule2.RegisterValidator (_ => _objectValidatorStub4);
      objectRule3.RegisterValidator (_ => _objectValidatorStub5);

      var metaValidationRuleMock1 = MockRepository.GenerateStrictMock<IObjectMetaValidationRule>();
      var metaValidationRuleMock2 = MockRepository.GenerateStrictMock<IObjectMetaValidationRule>();
      var metaValidationRuleMock3 = MockRepository.GenerateStrictMock<IObjectMetaValidationRule>();
      var metaValidationRuleMock4 = MockRepository.GenerateStrictMock<IObjectMetaValidationRule>();

      _objectMetaValidationRuleCollectorStub1.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create (typeof (Customer)));
      _objectMetaValidationRuleCollectorStub1.Stub (stub => stub.MetaValidationRules)
          .Return (new[] { metaValidationRuleMock1, metaValidationRuleMock2 });

      _objectMetaValidationRuleCollectorStub2.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create (typeof (Person)));
      _objectMetaValidationRuleCollectorStub2.Stub (stub => stub.MetaValidationRules).Return (new[] { metaValidationRuleMock3 });

      _objectMetaValidationRuleCollectorStub3.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create (typeof (Person)));
      _objectMetaValidationRuleCollectorStub3.Stub (stub => stub.MetaValidationRules).Return (new IObjectMetaValidationRule[0]);

      _objectMetaValidationRuleCollectorStub4.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create (typeof (Employee)));
      _objectMetaValidationRuleCollectorStub4.Stub (stub => stub.MetaValidationRules).Return (new[] { metaValidationRuleMock4 });

      var resultItem1 = MetaValidationRuleValidationResult.CreateValidResult();
      var resultItem2 = MetaValidationRuleValidationResult.CreateValidResult();
      var resultItem3 = MetaValidationRuleValidationResult.CreateInvalidResult ("Error Mock 2");
      var resultItem4 = MetaValidationRuleValidationResult.CreateValidResult();

      metaValidationRuleMock1
          .Expect (mock => mock.Validate (Arg<IEnumerable<IObjectValidator>>.List.Equal (new[] { _objectValidatorStub1, _objectValidatorStub2 })))
          .Return (
              new[]
              {
                  resultItem1
              });
      metaValidationRuleMock2
          .Expect (mock => mock.Validate (Arg<IEnumerable<IObjectValidator>>.List.Equal (new[] { _objectValidatorStub1, _objectValidatorStub2 })))
          .Return (
              new[]
              {
                  resultItem2,
                  resultItem3
              });
      metaValidationRuleMock3
          .Expect (
              mock => mock.Validate (
                  Arg<IEnumerable<IObjectValidator>>.List.Equal (new[] { _objectValidatorStub3, _objectValidatorStub4, _objectValidatorStub5 })))
          .Return (new[] { resultItem4 });
      metaValidationRuleMock4.Expect (mock => mock.Validate (Arg<IEnumerable<IObjectValidator>>.List.Equal (new IObjectValidator[0])))
          .Return (new MetaValidationRuleValidationResult[0]);

      var result = _validator.Validate (new[] { objectRule1, objectRule2, filteredObjectRule, objectRule3 }).ToArray();

      metaValidationRuleMock1.VerifyAllExpectations();
      metaValidationRuleMock2.VerifyAllExpectations();
      metaValidationRuleMock3.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { resultItem1, resultItem2, resultItem3, resultItem4 }));
    }
  }
}