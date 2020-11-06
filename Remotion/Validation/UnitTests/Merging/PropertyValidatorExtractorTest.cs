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
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class PropertyValidatorExtractorTest
  {
    private RemovingValidatorRegistration _removingValidatorRegistration1;
    private RemovingValidatorRegistration _removingValidatorRegistration2;
    private RemovingValidatorRegistration _removingValidatorRegistration3;
    private PropertyValidatorExtractor _extractor;
    private StubPropertyValidator _stubPropertyValidator1;
    private NotEmptyValidator _stubPropertyValidator2;
    private NotEqualValidator _stubPropertyValidator3;
    private RemovingValidatorRegistration _removingValidatorRegistration4;
    private LengthValidator _stubPropertyValidator4;
    private PropertyValidatorRegistrationWithContext _registrationWithContext1;
    private PropertyValidatorRegistrationWithContext _registrationWithContext2;
    private PropertyValidatorRegistrationWithContext _registrationWithContext3;
    private PropertyValidatorRegistrationWithContext _registrationWithContext4;
    private RemovingValidatorRegistration _removingValidatorRegistration5;
    private ILogContext _logContextMock;
    private PropertyValidatorRegistrationWithContext _registrationWithContext5;
    private PropertyValidatorRegistrationWithContext _registrationWithContext6;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollectorStub1;
    private RemovingValidatorRegistration _removingValidatorRegistration6;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollectorStub2;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollectorStub3;
    private PropertyValidatorRegistrationWithContext _registrationWithContext7;
    private PropertyValidatorRegistrationWithContext _registrationWithContext8;
    private PropertyValidatorRegistrationWithContext _registrationWithContext9;

    [SetUp]
    public void SetUp ()
    {
      _removingValidatorRegistration1 = new RemovingValidatorRegistration (typeof (NotEmptyValidator), null);
      _removingValidatorRegistration2 = new RemovingValidatorRegistration (typeof (NotEqualValidator), typeof (CustomerValidationRuleCollector1));
      _removingValidatorRegistration3 = new RemovingValidatorRegistration (typeof (NotNullValidator), null);
      _removingValidatorRegistration4 = new RemovingValidatorRegistration (typeof (LengthValidator), typeof (CustomerValidationRuleCollector2));
      _removingValidatorRegistration5 = new RemovingValidatorRegistration (typeof (NotEqualValidator), typeof (CustomerValidationRuleCollector2));
      _removingValidatorRegistration6 = new RemovingValidatorRegistration (typeof (LengthValidator), null);

      _removingPropertyValidationRuleCollectorStub1 = MockRepository.GenerateStub<IRemovingPropertyValidationRuleCollector>();
      _removingPropertyValidationRuleCollectorStub1.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create (typeof (Customer).GetProperty ("LastName")));
      _removingPropertyValidationRuleCollectorStub2 = MockRepository.GenerateStub<IRemovingPropertyValidationRuleCollector>();
      _removingPropertyValidationRuleCollectorStub2.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("FirstName")));
      _removingPropertyValidationRuleCollectorStub3 = MockRepository.GenerateStub<IRemovingPropertyValidationRuleCollector> ();
      _removingPropertyValidationRuleCollectorStub3.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create (typeof (SpecialCustomer2).GetProperty ("LastName")));

      _registrationWithContext1 = new PropertyValidatorRegistrationWithContext (_removingValidatorRegistration1, _removingPropertyValidationRuleCollectorStub1);
      _registrationWithContext2 = new PropertyValidatorRegistrationWithContext (_removingValidatorRegistration2, _removingPropertyValidationRuleCollectorStub1);
      _registrationWithContext3 = new PropertyValidatorRegistrationWithContext (_removingValidatorRegistration3, _removingPropertyValidationRuleCollectorStub1);
      _registrationWithContext4 = new PropertyValidatorRegistrationWithContext (_removingValidatorRegistration4, _removingPropertyValidationRuleCollectorStub1);
      _registrationWithContext5 = new PropertyValidatorRegistrationWithContext (_removingValidatorRegistration5, _removingPropertyValidationRuleCollectorStub1);
      _registrationWithContext6 = new PropertyValidatorRegistrationWithContext (_removingValidatorRegistration1, _removingPropertyValidationRuleCollectorStub1);
      _registrationWithContext7 = new PropertyValidatorRegistrationWithContext (_removingValidatorRegistration6, _removingPropertyValidationRuleCollectorStub2);
      _registrationWithContext8 = new PropertyValidatorRegistrationWithContext (_removingValidatorRegistration4, _removingPropertyValidationRuleCollectorStub3);
      _registrationWithContext9 = new PropertyValidatorRegistrationWithContext (_removingValidatorRegistration4, _removingPropertyValidationRuleCollectorStub1);

      //other property -> filtered!

      _stubPropertyValidator1 = new StubPropertyValidator(); //not extracted
      _stubPropertyValidator2 = new NotEmptyValidator (new InvariantValidationMessage ("Fake Message")); //extracted
      _stubPropertyValidator3 = new NotEqualValidator ("gfsf", new InvariantValidationMessage ("Fake Message")); //extracted
      _stubPropertyValidator4 = new LengthValidator (0, 10, new InvariantValidationMessage ("Fake Message")); //not extracted

      _logContextMock = MockRepository.GenerateStrictMock<ILogContext>();

      _extractor = new PropertyValidatorExtractor (
          new[]
          {
              _registrationWithContext1, _registrationWithContext2, _registrationWithContext3, _registrationWithContext4,
              _registrationWithContext5, _registrationWithContext6, _registrationWithContext7, _registrationWithContext8,
              _registrationWithContext9
          },
          _logContextMock);
    }

    [Test]
    public void ExtractPropertyValidatorsToRemove ()
    {
      var addingComponentPropertyRule = MockRepository.GenerateStub<IAddingPropertyValidationRuleCollector>();
      addingComponentPropertyRule.Stub (stub => stub.Validators)
          .Return (
              new IPropertyValidator[]
              { _stubPropertyValidator1, _stubPropertyValidator2, _stubPropertyValidator3, _stubPropertyValidator4 });
      addingComponentPropertyRule.Stub (stub => stub.CollectorType).Return (typeof (CustomerValidationRuleCollector1));
      addingComponentPropertyRule.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("LastName")));

      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IPropertyValidator>.Is.Same (_stubPropertyValidator2),
                  Arg<PropertyValidatorRegistrationWithContext[]>.List.Equal (new[] { _registrationWithContext1, _registrationWithContext6 }),
                  Arg<IAddingPropertyValidationRuleCollector>.Is.Same (addingComponentPropertyRule))).Repeat.Once();
      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IPropertyValidator>.Is.Same (_stubPropertyValidator3),
                  Arg<PropertyValidatorRegistrationWithContext[]>.List.Equal (new[] { _registrationWithContext2 }),
                  Arg<IAddingPropertyValidationRuleCollector>.Is.Same (addingComponentPropertyRule))).Repeat.Once();

      var result = _extractor.ExtractPropertyValidatorsToRemove (addingComponentPropertyRule).ToArray();

      _logContextMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (new IPropertyValidator[] { _stubPropertyValidator2, _stubPropertyValidator3 }));
    }

    [Test]
    public void ExtractPropertyValidatorsToRemove_OnlyRemovesValidatorsOfNewDefinedPropertyOfDerrivedClass ()
    {
      var addingComponentPropertyRule = MockRepository.GenerateStub<IAddingPropertyValidationRuleCollector> ();
      addingComponentPropertyRule.Stub (stub => stub.Validators)
          .Return (
              new IPropertyValidator[] { _stubPropertyValidator1, _stubPropertyValidator2, _stubPropertyValidator3, _stubPropertyValidator4 });
      addingComponentPropertyRule.Stub (stub => stub.CollectorType).Return (typeof (CustomerValidationRuleCollector2));
      addingComponentPropertyRule.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create (typeof (SpecialCustomer2).GetProperty ("LastName")));

      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IPropertyValidator>.Is.Same (_stubPropertyValidator4),
                  Arg<PropertyValidatorRegistrationWithContext[]>.List.Equal (new[] { _registrationWithContext8 }),
                  Arg<IAddingPropertyValidationRuleCollector>.Is.Same (addingComponentPropertyRule))).Repeat.Once ();

      var result = _extractor.ExtractPropertyValidatorsToRemove (addingComponentPropertyRule).ToArray ();
      _logContextMock.VerifyAllExpectations ();

      Assert.That (result, Is.EqualTo (new IPropertyValidator[] { _stubPropertyValidator4 }));
    }
  }
}