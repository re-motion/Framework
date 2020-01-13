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
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestDomain.Validators;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class ObjectValidatorExtractorTest
  {
    private ObjectValidatorExtractor _extractor;
    private IObjectValidator _stubObjectValidator1;
    private IObjectValidator _stubObjectValidator2;
    private IObjectValidator _stubObjectValidator3;
    private ValidatorRegistration _validatorRegistration1a;
    private ValidatorRegistration _validatorRegistration2a;
    private ValidatorRegistration _validatorRegistration2b;
    private ValidatorRegistration _validatorRegistration2c;
    private ObjectValidatorRegistrationWithContext _registrationWithContext1;
    private ObjectValidatorRegistrationWithContext _registrationWithContext2;
    private ObjectValidatorRegistrationWithContext _registrationWithContext3;
    private ObjectValidatorRegistrationWithContext _registrationWithContext4;
    private ObjectValidatorRegistrationWithContext _registrationWithContext5;
    private IRemovingObjectValidationRuleCollector _removingObjectValidationRuleCollectorStub1;
    private IRemovingObjectValidationRuleCollector _removingObjectValidationRuleCollectorStub2;
    private IRemovingObjectValidationRuleCollector _removingObjectValidationRuleCollectorStub3;
    private ILogContext _logContextMock;

    [SetUp]
    public void SetUp ()
    {
      _validatorRegistration1a = new ValidatorRegistration (typeof (FakeCustomerValidator), null);
      _validatorRegistration2a = new ValidatorRegistration (typeof (StubObjectValidator), typeof (CustomerValidationRuleCollector1));
      _validatorRegistration2b = new ValidatorRegistration (typeof (StubObjectValidator), typeof (CustomerValidationRuleCollector2));
      _validatorRegistration2c = new ValidatorRegistration (typeof (StubObjectValidator), null);

      _removingObjectValidationRuleCollectorStub1 = MockRepository.GenerateStub<IRemovingObjectValidationRuleCollector>();
      _removingObjectValidationRuleCollectorStub1.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create (typeof (Customer)));
      _removingObjectValidationRuleCollectorStub2 = MockRepository.GenerateStub<IRemovingObjectValidationRuleCollector>();
      _removingObjectValidationRuleCollectorStub2.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create(typeof (Customer)));
      _removingObjectValidationRuleCollectorStub3 = MockRepository.GenerateStub<IRemovingObjectValidationRuleCollector>();
      _removingObjectValidationRuleCollectorStub3.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create(typeof (Employee)));

      _registrationWithContext1 = new ObjectValidatorRegistrationWithContext (_validatorRegistration1a, _removingObjectValidationRuleCollectorStub1);
      _registrationWithContext2 = new ObjectValidatorRegistrationWithContext (_validatorRegistration2a, _removingObjectValidationRuleCollectorStub1);
      _registrationWithContext3 = new ObjectValidatorRegistrationWithContext (_validatorRegistration2b, _removingObjectValidationRuleCollectorStub2);
      _registrationWithContext4 = new ObjectValidatorRegistrationWithContext (_validatorRegistration1a, _removingObjectValidationRuleCollectorStub1);
      _registrationWithContext5 = new ObjectValidatorRegistrationWithContext (_validatorRegistration2c, _removingObjectValidationRuleCollectorStub3);

      _stubObjectValidator1 = new FakeCustomerValidator(); //extracted
      _stubObjectValidator2 = new StubObjectValidator(); //extracted
      _stubObjectValidator3 = MockRepository.GenerateStub<IObjectValidator>(); //not extracted

      _logContextMock = MockRepository.GenerateStrictMock<ILogContext>();

      _extractor = new ObjectValidatorExtractor (
          new[]
          {
              _registrationWithContext1, _registrationWithContext2, _registrationWithContext3, _registrationWithContext4, 
              _registrationWithContext5
          },
          _logContextMock);
    }

    [Test]
    public void ExtractObjectValidatorsToRemove ()
    {
      var addingObjectValidationRuleCollector = MockRepository.GenerateStub<IAddingObjectValidationRuleCollector>();
      addingObjectValidationRuleCollector.Stub (stub => stub.Validators)
          .Return (new[] { _stubObjectValidator1, _stubObjectValidator3, _stubObjectValidator2 });
      addingObjectValidationRuleCollector.Stub (stub => stub.CollectorType).Return (typeof (CustomerValidationRuleCollector1));
      addingObjectValidationRuleCollector.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create(typeof (Customer)));

      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IObjectValidator>.Is.Same (_stubObjectValidator1),
                  Arg<ObjectValidatorRegistrationWithContext[]>.List.Equal (new[] { _registrationWithContext1, _registrationWithContext4 }),
                  Arg<IAddingObjectValidationRuleCollector>.Is.Same (addingObjectValidationRuleCollector))).Repeat.Once();
      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IObjectValidator>.Is.Same (_stubObjectValidator2),
                  Arg<ObjectValidatorRegistrationWithContext[]>.List.Equal (new[] { _registrationWithContext2 }),
                  Arg<IAddingObjectValidationRuleCollector>.Is.Same (addingObjectValidationRuleCollector))).Repeat.Once ();

      var result = _extractor.ExtractObjectValidatorsToRemove (addingObjectValidationRuleCollector).ToArray();

      _logContextMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (new[] { _stubObjectValidator1, _stubObjectValidator2 }));
    }
  }
}