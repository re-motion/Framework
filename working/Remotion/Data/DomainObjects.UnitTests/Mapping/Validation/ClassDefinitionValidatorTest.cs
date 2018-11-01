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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation
{
  [TestFixture]
  public class ClassDefinitionValidatorTest
  {
    private ClassDefinition _classDefinition1;
    private ClassDefinition _classDefinition2;
    private ClassDefinition _classDefinition3;
    private MappingValidationResult _fakeValidMappingValidationResult;
    private MappingValidationResult _fakeInvalidMappingValidationResult;
    private IClassDefinitionValidationRule _validationRuleMock1;
    private IClassDefinitionValidationRule _validationRuleMock2;
    private IClassDefinitionValidationRule _validationRuleMock3;

    [SetUp]
    public void SetUp ()
    {
      _classDefinition1 = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (DerivedValidationDomainObjectClass));
      _classDefinition2 = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (DerivedValidationDomainObjectClass));
      _classDefinition3 = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (DerivedValidationDomainObjectClass));

      _validationRuleMock1 = MockRepository.GenerateStrictMock<IClassDefinitionValidationRule> ();
      _validationRuleMock2 = MockRepository.GenerateStrictMock<IClassDefinitionValidationRule> ();
      _validationRuleMock3 = MockRepository.GenerateStrictMock<IClassDefinitionValidationRule> ();

      _fakeValidMappingValidationResult = MappingValidationResult.CreateValidResult();
      _fakeInvalidMappingValidationResult = MappingValidationResult.CreateInvalidResult("Test");
    }

    [Test]
    public void ValidateWithOneRuleAndClassDefinition_ValidResult ()
    {
      var validator = new ClassDefinitionValidator (_validationRuleMock1);

      _validationRuleMock1.Expect (mock => mock.Validate (_classDefinition1)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock1.Replay();

      var mappingValidationResults = validator.Validate (new[] { _classDefinition1 }).ToArray();

      _validationRuleMock1.VerifyAllExpectations ();
      Assert.That (validator.ValidationRules.Count, Is.EqualTo (1));
      Assert.That (mappingValidationResults.Length, Is.EqualTo (0));
    }

    [Test]
    public void ValidateWithOneRuleAndClassDefinition_InvalidResult ()
    {
      var validator = new ClassDefinitionValidator (_validationRuleMock1);

      _validationRuleMock1.Expect (mock => mock.Validate (_classDefinition1)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock1.Replay ();

      var mappingValidationResults = validator.Validate (new[] { _classDefinition1 }).ToArray ();

      _validationRuleMock1.VerifyAllExpectations ();
      Assert.That (validator.ValidationRules.Count, Is.EqualTo (1));
      Assert.That (mappingValidationResults.Length, Is.EqualTo (1));
      Assert.That (mappingValidationResults[0], Is.SameAs(_fakeInvalidMappingValidationResult));
    }

    [Test]
    public void ValidateWithSeveralRulesAndClassDefinitions_ValidResult ()
    {
      var validator = new ClassDefinitionValidator (_validationRuleMock1, _validationRuleMock2, _validationRuleMock3);

      _validationRuleMock1.Expect (mock => mock.Validate (_classDefinition1)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock1.Expect (mock => mock.Validate (_classDefinition2)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock1.Expect (mock => mock.Validate (_classDefinition3)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock2.Expect (mock => mock.Validate (_classDefinition1)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock2.Expect (mock => mock.Validate (_classDefinition2)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock2.Expect (mock => mock.Validate (_classDefinition3)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock3.Expect (mock => mock.Validate (_classDefinition1)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock3.Expect (mock => mock.Validate (_classDefinition2)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock3.Expect (mock => mock.Validate (_classDefinition3)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock1.Replay ();

      var mappingValidationResults = validator.Validate (new[] { _classDefinition1, _classDefinition2, _classDefinition3 }).ToArray ();

      _validationRuleMock1.VerifyAllExpectations ();
      Assert.That (validator.ValidationRules.Count, Is.EqualTo (3));
      Assert.That (mappingValidationResults.Length, Is.EqualTo (0));
    }

    [Test]
    public void ValidateWithSeveralRulesAndClassDefinitions_InvalidResult ()
    {
      var validator = new ClassDefinitionValidator (_validationRuleMock1, _validationRuleMock2, _validationRuleMock3);

      _validationRuleMock1.Expect (mock => mock.Validate (_classDefinition1)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock1.Expect (mock => mock.Validate (_classDefinition2)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock1.Expect (mock => mock.Validate (_classDefinition3)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock2.Expect (mock => mock.Validate (_classDefinition1)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock2.Expect (mock => mock.Validate (_classDefinition2)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock2.Expect (mock => mock.Validate (_classDefinition3)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock3.Expect (mock => mock.Validate (_classDefinition1)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock3.Expect (mock => mock.Validate (_classDefinition2)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock3.Expect (mock => mock.Validate (_classDefinition3)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock1.Replay ();

      var mappingValidationResults = validator.Validate (new[] { _classDefinition1, _classDefinition2, _classDefinition3 }).ToArray ();

      _validationRuleMock1.VerifyAllExpectations ();
      Assert.That (validator.ValidationRules.Count, Is.EqualTo (3));
      Assert.That (mappingValidationResults.Length, Is.EqualTo (9));
      Assert.That (mappingValidationResults[0], Is.SameAs (_fakeInvalidMappingValidationResult));
      Assert.That (mappingValidationResults[1], Is.SameAs (_fakeInvalidMappingValidationResult));
      Assert.That (mappingValidationResults[2], Is.SameAs (_fakeInvalidMappingValidationResult));
      Assert.That (mappingValidationResults[3], Is.SameAs (_fakeInvalidMappingValidationResult));
      Assert.That (mappingValidationResults[4], Is.SameAs (_fakeInvalidMappingValidationResult));
      Assert.That (mappingValidationResults[5], Is.SameAs (_fakeInvalidMappingValidationResult));
      Assert.That (mappingValidationResults[6], Is.SameAs (_fakeInvalidMappingValidationResult));
      Assert.That (mappingValidationResults[7], Is.SameAs (_fakeInvalidMappingValidationResult));
      Assert.That (mappingValidationResults[8], Is.SameAs (_fakeInvalidMappingValidationResult));
    }
  }
}