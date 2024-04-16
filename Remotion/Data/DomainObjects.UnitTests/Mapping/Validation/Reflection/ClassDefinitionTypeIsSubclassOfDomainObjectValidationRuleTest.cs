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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class ClassDefinitionTypeIsSubclassOfDomainObjectValidationRuleTest : ValidationRuleTestBase
  {
    private ClassDefinitionTypeIsSubclassOfDomainObjectValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new ClassDefinitionTypeIsSubclassOfDomainObjectValidationRule();
    }

    [Test]
    public void ClassDefinitionTypeIsSubClassOfDomainObject ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(DerivedValidationDomainObjectClass));

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassDefinitionTypeIsNoSubClassOfDomainObject ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition("String", typeof(string));

      var validationResult = _validationRule.Validate(typeDefinition);

      var expectedMessage = "Type 'String' of class 'String' is not assignable to 'DomainObject'.\r\n\r\nDeclaring type: System.String";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }
  }
}
