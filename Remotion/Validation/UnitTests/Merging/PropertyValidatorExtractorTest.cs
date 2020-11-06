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
    private PropertyValidatorExtractor _extractor;
    private StubPropertyValidator _stubPropertyValidator1;
    private NotEmptyValidator _stubPropertyValidator2;
    private NotEqualValidator _stubPropertyValidator3;
    private LengthValidator _stubPropertyValidator4;
    private StubPropertyValidator _stubPropertyValidator5;
    private ILogContext _logContextMock;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollectorStub1;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollectorStub2;
    private IRemovingPropertyValidationRuleCollector _removingPropertyValidationRuleCollectorStub3;
    private RemovingPropertyValidatorRegistration _removingPropertyValidatorRegistration1;
    private RemovingPropertyValidatorRegistration _removingPropertyValidatorRegistration2;
    private RemovingPropertyValidatorRegistration _removingPropertyValidatorRegistration3;
    private RemovingPropertyValidatorRegistration _removingPropertyValidatorRegistration4;
    private RemovingPropertyValidatorRegistration _removingPropertyValidatorRegistration5;
    private RemovingPropertyValidatorRegistration _removingPropertyValidatorRegistration6;
    private RemovingPropertyValidatorRegistration _removingPropertyValidatorRegistration7;
    private RemovingPropertyValidatorRegistration _removingPropertyValidatorRegistration8;
    private RemovingPropertyValidatorRegistration _removingPropertyValidatorRegistration9;
    private RemovingPropertyValidatorRegistration _removingPropertyValidatorRegistration10;

    [SetUp]
    public void SetUp ()
    {
      _stubPropertyValidator1 = new StubPropertyValidator(); //not extracted
      _stubPropertyValidator2 = new NotEmptyValidator (new InvariantValidationMessage ("Fake Message")); //extracted
      _stubPropertyValidator3 = new NotEqualValidator ("gfsf", new InvariantValidationMessage ("Fake Message")); //extracted
      _stubPropertyValidator4 = new LengthValidator (0, 10, new InvariantValidationMessage ("Fake Message")); //not extracted
      _stubPropertyValidator5 = new StubPropertyValidator(); //extracted

      var registration1 = new {ValidatorType = typeof (NotEmptyValidator), CollectorTypeToRemoveFrom = (Type) null };
      var registration2 = new {ValidatorType = typeof (NotEqualValidator), CollectorTypeToRemoveFrom = typeof (CustomerValidationRuleCollector1) };
      var registration3 = new {ValidatorType = typeof (NotNullValidator), CollectorTypeToRemoveFrom = (Type) null };
      var registration4 = new {ValidatorType = typeof (LengthValidator), CollectorTypeToRemoveFrom = typeof (CustomerValidationRuleCollector2) };
      var registration5 = new {ValidatorType = typeof (NotEqualValidator), CollectorTypeToRemoveFrom = typeof (CustomerValidationRuleCollector2) };
      var registration6 = new {ValidatorType = typeof (LengthValidator), CollectorTypeToRemoveFrom = (Type) null };

      _removingPropertyValidationRuleCollectorStub1 = MockRepository.GenerateStub<IRemovingPropertyValidationRuleCollector>();
      _removingPropertyValidationRuleCollectorStub1.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create (typeof (Customer).GetProperty ("LastName")));
      _removingPropertyValidationRuleCollectorStub2 = MockRepository.GenerateStub<IRemovingPropertyValidationRuleCollector>();
      _removingPropertyValidationRuleCollectorStub2.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("FirstName")));
      _removingPropertyValidationRuleCollectorStub3 = MockRepository.GenerateStub<IRemovingPropertyValidationRuleCollector> ();
      _removingPropertyValidationRuleCollectorStub3.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create (typeof (SpecialCustomer2).GetProperty ("LastName")));

      _removingPropertyValidatorRegistration1 = new RemovingPropertyValidatorRegistration (registration1.ValidatorType, registration1.CollectorTypeToRemoveFrom, null, _removingPropertyValidationRuleCollectorStub1);
      _removingPropertyValidatorRegistration2 = new RemovingPropertyValidatorRegistration (registration2.ValidatorType, registration2.CollectorTypeToRemoveFrom, null, _removingPropertyValidationRuleCollectorStub1);
      _removingPropertyValidatorRegistration3 = new RemovingPropertyValidatorRegistration (registration3.ValidatorType, registration3.CollectorTypeToRemoveFrom, null, _removingPropertyValidationRuleCollectorStub1);
      _removingPropertyValidatorRegistration4 = new RemovingPropertyValidatorRegistration (registration4.ValidatorType, registration4.CollectorTypeToRemoveFrom, null, _removingPropertyValidationRuleCollectorStub1);
      _removingPropertyValidatorRegistration5 = new RemovingPropertyValidatorRegistration (registration5.ValidatorType, registration5.CollectorTypeToRemoveFrom, null, _removingPropertyValidationRuleCollectorStub1);
      _removingPropertyValidatorRegistration6 = new RemovingPropertyValidatorRegistration (registration1.ValidatorType, registration1.CollectorTypeToRemoveFrom, null, _removingPropertyValidationRuleCollectorStub1);
      _removingPropertyValidatorRegistration7 = new RemovingPropertyValidatorRegistration (registration6.ValidatorType, registration6.CollectorTypeToRemoveFrom, null, _removingPropertyValidationRuleCollectorStub2);
      _removingPropertyValidatorRegistration8 = new RemovingPropertyValidatorRegistration (registration4.ValidatorType, registration4.CollectorTypeToRemoveFrom, null, _removingPropertyValidationRuleCollectorStub3);
      _removingPropertyValidatorRegistration9 = new RemovingPropertyValidatorRegistration (registration4.ValidatorType, registration4.CollectorTypeToRemoveFrom, null, _removingPropertyValidationRuleCollectorStub1);
      _removingPropertyValidatorRegistration10 = new RemovingPropertyValidatorRegistration (typeof (StubPropertyValidator), null, v => ReferenceEquals (v, _stubPropertyValidator5), _removingPropertyValidationRuleCollectorStub1);

      //other property -> filtered!

      _logContextMock = MockRepository.GenerateStrictMock<ILogContext>();

      _extractor = new PropertyValidatorExtractor (
          new[]
          {
              _removingPropertyValidatorRegistration1, _removingPropertyValidatorRegistration2, _removingPropertyValidatorRegistration3, _removingPropertyValidatorRegistration4,
              _removingPropertyValidatorRegistration5, _removingPropertyValidatorRegistration6, _removingPropertyValidatorRegistration7, _removingPropertyValidatorRegistration8,
              _removingPropertyValidatorRegistration9, _removingPropertyValidatorRegistration10
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
              { _stubPropertyValidator1, _stubPropertyValidator2, _stubPropertyValidator3, _stubPropertyValidator4, _stubPropertyValidator5 });
      addingComponentPropertyRule.Stub (stub => stub.CollectorType).Return (typeof (CustomerValidationRuleCollector1));
      addingComponentPropertyRule.Stub (stub => stub.Property).Return (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("LastName")));

      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IPropertyValidator>.Is.Same (_stubPropertyValidator2),
                  Arg<RemovingPropertyValidatorRegistration[]>.List.Equal (new[] { _removingPropertyValidatorRegistration1, _removingPropertyValidatorRegistration6 }),
                  Arg<IAddingPropertyValidationRuleCollector>.Is.Same (addingComponentPropertyRule))).Repeat.Once();
      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IPropertyValidator>.Is.Same (_stubPropertyValidator3),
                  Arg<RemovingPropertyValidatorRegistration[]>.List.Equal (new[] { _removingPropertyValidatorRegistration2 }),
                  Arg<IAddingPropertyValidationRuleCollector>.Is.Same (addingComponentPropertyRule))).Repeat.Once();
      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IPropertyValidator>.Is.Same (_stubPropertyValidator5),
                  Arg<RemovingPropertyValidatorRegistration[]>.List.Equal (new[] { _removingPropertyValidatorRegistration10 }),
                  Arg<IAddingPropertyValidationRuleCollector>.Is.Same (addingComponentPropertyRule))).Repeat.Once();

      var result = _extractor.ExtractPropertyValidatorsToRemove (addingComponentPropertyRule).ToArray();

      _logContextMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (new IPropertyValidator[] { _stubPropertyValidator2, _stubPropertyValidator3, _stubPropertyValidator5 }));
    }

    [Test]
    public void ExtractPropertyValidatorsToRemove_OnlyRemovesValidatorsOfNewDefinedPropertyOfDerivedClass ()
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
                  Arg<RemovingPropertyValidatorRegistration[]>.List.Equal (new[] { _removingPropertyValidatorRegistration8 }),
                  Arg<IAddingPropertyValidationRuleCollector>.Is.Same (addingComponentPropertyRule))).Repeat.Once ();

      var result = _extractor.ExtractPropertyValidatorsToRemove (addingComponentPropertyRule).ToArray ();
      _logContextMock.VerifyAllExpectations ();

      Assert.That (result, Is.EqualTo (new IPropertyValidator[] { _stubPropertyValidator4 }));
    }
  }
}