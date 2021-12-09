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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class CheckForClassIDIsValidValidationRuleTest : ValidationRuleTestBase
  {
    private CheckForClassIDIsValidValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new CheckForClassIDIsValidValidationRule();
    }

    [Test]
    [TestCase("a")]
    [TestCase("ä")]
    [TestCase("_")]
    [TestCase("aä")]
    [TestCase("a_")]
    [TestCase("a1")]
    public void ValidClassID (string classID)
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(id: classID);

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    [TestCase("a|")]
    [TestCase("1a")]
    [TestCase("a.a")]
    [TestCase("a a")]
    [TestCase("a+a")]
    public void InvalidClassID (string classID)
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(id: classID);

      var validationResult = _validationRule.Validate(typeDefinition);

      var expectedMessage =
          string.Format(
              "The Class-ID '{0}' is not valid. Valid Class-IDs must start with a letter or underscore and containing only letters, digits, and underscores.\r\n\r\n"
              + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order",
              classID);
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void IntegrationTestForObjectIDStringSerializer ()
    {
      var classID = "Class" + ObjectIDStringSerializer.Delimiter + "End";
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(id: classID);

      var validationResult = _validationRule.Validate(typeDefinition);

      var expectedMessage =
          string.Format(
              "The Class-ID '{0}' is not valid. Valid Class-IDs must start with a letter or underscore and containing only letters, digits, and underscores.\r\n\r\n"
              + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order",
              classID);
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void IgnoresArgumentsOfTypeOtherThanClassDefinition ()
    {
      var typeDefinition = new TypeDefinitionForUnresolvedRelationPropertyType(typeof(string), new NullPropertyInformation());

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }
  }
}
