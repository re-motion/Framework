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
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class CheckForTypeNotFoundTypeDefinitionValidationRuleTest : ValidationRuleTestBase
  {
    private CheckForTypeNotFoundTypeDefinitionValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new CheckForTypeNotFoundTypeDefinitionValidationRule();
    }

    [Test]
    public void RelationDefinitionWithNoTypeNotFoundClassDefinition ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(DerivedValidationDomainObjectClass));
      var endPoint = new AnonymousRelationEndPointDefinition(typeDefinition);
      var relationDefinition = new RelationDefinition("ID", endPoint, endPoint);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void RelationDefinitionWithTypeNotFoundClassDefinition ()
    {
      var classDefinition = new TypeDefinitionForUnresolvedRelationPropertyType(
          typeof(ClassOutOfInheritanceHierarchy),
          PropertyInfoAdapter.Create(typeof(DerivedValidationDomainObjectClass).GetProperty("Property")));
      var endPoint = new VirtualObjectRelationEndPointDefinition(
          classDefinition,
          "RelationProperty",
          false,
          PropertyInfoAdapter.Create(typeof(DerivedValidationDomainObjectClass).GetProperty("Property")));
      var relationDefinition = new RelationDefinition("ID", endPoint, endPoint);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "The relation property 'Property' has return type 'String', which is not a part of the mapping. "
          + "Relation properties must not point to types above the inheritance root.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
          + "Property: Property";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }
  }
}
