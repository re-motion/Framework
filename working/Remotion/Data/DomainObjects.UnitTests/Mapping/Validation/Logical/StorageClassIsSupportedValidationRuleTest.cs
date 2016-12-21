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

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class StorageClassIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private StorageClassIsSupportedValidationRule _validationRule;
    private Type _type;
    private ClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new StorageClassIsSupportedValidationRule();

      _type = typeof (DerivedValidationDomainObjectClass);
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (_type);
    }

    [Test]
    public void PropertyWithoutStorageClassAttribute ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _type, "Property");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassPersistent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _type, "PropertyWithStorageClassPersistent");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassTransaction ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _type, "PropertyWithStorageClassTransaction");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassNone ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo (_classDefinition, _type, "PropertyWithStorageClassNone");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      var expectedMessage = "Only StorageClass.Persistent and StorageClass.Transaction are supported for property 'PropertyWithStorageClassNone' of "
        +"class 'DerivedValidationDomainObjectClass'.\r\n\r\n"
        +"Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
        +"Property: PropertyWithStorageClassNone";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

  }
}