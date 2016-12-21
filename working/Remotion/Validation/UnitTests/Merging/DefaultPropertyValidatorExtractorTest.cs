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
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class DefaultPropertyValidatorExtractorTest
  {
    private ValidatorRegistration _validatorRegistration1;
    private ValidatorRegistration _validatorRegistration2;
    private ValidatorRegistration _validatorRegistration3;
    private PropertyValidatorExtractor _extractor;
    private StubPropertyValidator _stubPropertyValidator1;
    private NotEmptyValidator _stubPropertyValidator2;
    private NotEqualValidator _stubPropertyValidator3;
    private ValidatorRegistration _validatorRegistration4;
    private LengthValidator _stubPropertyValidator4;
    private ValidatorRegistrationWithContext _registrationWithContext1;
    private ValidatorRegistrationWithContext _registrationWithContext2;
    private ValidatorRegistrationWithContext _registrationWithContext3;
    private ValidatorRegistrationWithContext _registrationWithContext4;
    private ValidatorRegistration _validatorRegistration5;
    private ILogContext _logContextMock;
    private ValidatorRegistrationWithContext _registrationWithContext5;
    private ValidatorRegistrationWithContext _registrationWithContext6;
    private IRemovingComponentPropertyRule _removingPropertyRuleStub1;
    private ValidatorRegistration _validatorRegistration6;
    private IRemovingComponentPropertyRule _removingPropertyRuleStub2;
    private ValidatorRegistrationWithContext _registrationWithContext7;

    [SetUp]
    public void SetUp ()
    {
      _validatorRegistration1 = new ValidatorRegistration (typeof (NotEmptyValidator), null);
      _validatorRegistration2 = new ValidatorRegistration (typeof (NotEqualValidator), typeof (CustomerValidationCollector1));
      _validatorRegistration3 = new ValidatorRegistration (typeof (NotNullValidator), null);
      _validatorRegistration4 = new ValidatorRegistration (typeof (LengthValidator), typeof (CustomerValidationCollector2));
      _validatorRegistration5 = new ValidatorRegistration (typeof (NotEqualValidator), typeof (CustomerValidationCollector2));
      _validatorRegistration6 = new ValidatorRegistration (typeof (LengthValidator), null);

      _removingPropertyRuleStub1 = MockRepository.GenerateStub<IRemovingComponentPropertyRule>();
      _removingPropertyRuleStub1.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("LastName")));
      _removingPropertyRuleStub2 = MockRepository.GenerateStub<IRemovingComponentPropertyRule>();
      _removingPropertyRuleStub2.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("FirstName")));

      _registrationWithContext1 = new ValidatorRegistrationWithContext (_validatorRegistration1, _removingPropertyRuleStub1);
      _registrationWithContext2 = new ValidatorRegistrationWithContext (_validatorRegistration2, _removingPropertyRuleStub1);
      _registrationWithContext3 = new ValidatorRegistrationWithContext (_validatorRegistration3, _removingPropertyRuleStub1);
      _registrationWithContext4 = new ValidatorRegistrationWithContext (_validatorRegistration4, _removingPropertyRuleStub1);
      _registrationWithContext5 = new ValidatorRegistrationWithContext (_validatorRegistration5, _removingPropertyRuleStub1);
      _registrationWithContext6 = new ValidatorRegistrationWithContext (_validatorRegistration1, _removingPropertyRuleStub1);
      _registrationWithContext7 = new ValidatorRegistrationWithContext (_validatorRegistration6, _removingPropertyRuleStub2);
          //other property -> filtered!

      _stubPropertyValidator1 = new StubPropertyValidator(); //not extracted
      _stubPropertyValidator2 = new NotEmptyValidator (null); //extracted
      _stubPropertyValidator3 = new NotEqualValidator ("gfsf"); //extracted
      _stubPropertyValidator4 = new LengthValidator (0, 10); //not extracted

      _logContextMock = MockRepository.GenerateStrictMock<ILogContext>();

      _extractor = new PropertyValidatorExtractor (
          new[]
          {
              _registrationWithContext1, _registrationWithContext2, _registrationWithContext3, _registrationWithContext4,
              _registrationWithContext5, _registrationWithContext6, _registrationWithContext7
          },
          _logContextMock);
    }

    [Test]
    public void ExtractPropertyValidatorsToRemove ()
    {
      var addingComponentPropertyRule = MockRepository.GenerateStub<IAddingComponentPropertyRule>();
      addingComponentPropertyRule.Stub (stub => stub.Validators)
          .Return (
              new IPropertyValidator[]
              { _stubPropertyValidator1, _stubPropertyValidator2, _stubPropertyValidator3, _stubPropertyValidator4 });
      addingComponentPropertyRule.Stub (stub => stub.CollectorType).Return (typeof (CustomerValidationCollector1));
      addingComponentPropertyRule.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("LastName")));

      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IPropertyValidator>.Is.Same (_stubPropertyValidator2),
                  Arg<ValidatorRegistrationWithContext[]>.List.Equal (new[] { _registrationWithContext1, _registrationWithContext6 }),
                  Arg<IValidationRule>.Is.Same (addingComponentPropertyRule))).Repeat.Once();
      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IPropertyValidator>.Is.Same (_stubPropertyValidator3),
                  Arg<ValidatorRegistrationWithContext[]>.List.Equal (new[] { _registrationWithContext2 }),
                  Arg<IValidationRule>.Is.Same (addingComponentPropertyRule))).Repeat.Once();

      var result = _extractor.ExtractPropertyValidatorsToRemove (addingComponentPropertyRule).ToArray();

      _logContextMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (new IPropertyValidator[] { _stubPropertyValidator2, _stubPropertyValidator3 }));
    }
  }
}