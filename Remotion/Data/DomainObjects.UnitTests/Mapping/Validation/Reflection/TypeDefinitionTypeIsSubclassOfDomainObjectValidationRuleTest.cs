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

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class TypeDefinitionTypeIsSubclassOfDomainObjectValidationRuleTest : ValidationRuleTestBase
  {
    private interface INonDomainObject
    {
    }

    private interface IOrder : IDomainObject
    {
    }

    private class NonDomainObject
    {
    }

    private class Order : DomainObject
    {
    }

    private TypeDefinitionTypeIsSubtypeOfDomainObjectValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new TypeDefinitionTypeIsSubtypeOfDomainObjectValidationRule();
    }

    [Test]
    public void ClassDefinitionTypeIsSubTypeOfDomainObject ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Order));

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassDefinitionTypeIsNoSubTypeOfDomainObject ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(NonDomainObject));

      var validationResult = _validationRule.Validate(typeDefinition);

      var expectedMessage =
          "Type 'NonDomainObject' is not assignable to 'IDomainObject'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection.TypeDefinitionTypeIsSubclassOfDomainObjectValidationRuleTest+NonDomainObject";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void InterfaceDefinitionTypeIsSubTypeOfDomainObject ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateInterfaceDefinition(typeof(IOrder));

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InterfaceDefinitionTypeIsNoSubTypeOfDomainObject ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateInterfaceDefinition(typeof(INonDomainObject));

      var validationResult = _validationRule.Validate(typeDefinition);

      var expectedMessage =
          "Type 'INonDomainObject' is not assignable to 'IDomainObject'.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection.TypeDefinitionTypeIsSubclassOfDomainObjectValidationRuleTest+INonDomainObject";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }
  }
}
