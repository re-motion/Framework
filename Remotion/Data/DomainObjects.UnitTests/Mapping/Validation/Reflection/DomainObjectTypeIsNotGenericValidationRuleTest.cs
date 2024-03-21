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
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.DomainObjectTypeIsNotGenericValidationRule;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class DomainObjectTypeIsNotGenericValidationRuleTest : ValidationRuleTestBase
  {
    private DomainObjectTypeIsNotGenericValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new DomainObjectTypeIsNotGenericValidationRule();
    }

    [Test]
    public void NoGenericType ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(NonGenericTypeDomainObject));

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void IsGenericType_IsNotDomainObjectBase ()
    {
      var type = typeof(GenericTypeDomainObject<string>);
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(type);

      var validationResult = _validationRule.Validate(typeDefinition);

      var expectedMessage = "Generic domain objects are not supported.\r\n\r\n"
        +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
        +"DomainObjectTypeIsNotGenericValidationRule.GenericTypeDomainObject`1[System.String]";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

  }
}
