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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRuleTest : ValidationRuleTestBase
  {
    private MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule(
          new ReflectionBasedMemberInformationNameResolver(),
          new PropertyMetadataReflector());
    }

    [Test]
    public void ClassDefinitionWithUnresolvedType ()
    {
      var type = typeof(BaseMappingAttributesClass);
      var classDefinition = new ClassDefinitionWithUnresolvedType(
          "Test", type, true, null, new Mock<IPersistentMixinFinder>().Object, new Mock<IDomainObjectCreator>().Object);
      Assert.That(
          () => _validationRule.Validate(classDefinition),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
                  + "MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.BaseMappingAttributesClass' is not resolved."));
    }

    [Test]
    public void OriginalPropertyDeclaration ()
    {
      var type = typeof(BaseMappingAttributesClass);
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: type);

      var validationResult = _validationRule.Validate(typeDefinition).First();

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void NonOriginalPropertiesDeclarationWithMappingAttribute_NoInheritanceRoot ()
    {
      var type = typeof(DerivedClassWithMappingAttribute);
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: type);

      var validationResult = _validationRule.Validate(typeDefinition).Where(r=>!r.IsValid).ToArray();

      var expectedMessage1 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.DerivedClassWithMappingAttribute\r\n"
        + "Property: Property1";
      var expectedMessage2 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.DerivedClassWithMappingAttribute\r\n"
        + "Property: Property3";
      Assert.That(validationResult.Length, Is.EqualTo(2));
      AssertMappingValidationResult(validationResult[0], false, expectedMessage1);
      AssertMappingValidationResult(validationResult[1], false, expectedMessage2);
    }

    [Test]
    public void NonOriginalPropertiesDeclarationWithMappingAttribute_InheritanceRoot ()
    {
      var type = typeof(InheritanceRootDerivedMappingAttributesClass);
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: type);

      var validationResult = _validationRule.Validate(typeDefinition).Where(r => !r.IsValid).ToArray();

      var expectedMessage1 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.InheritanceRootDerivedMappingAttributesClass\r\n"
        + "Property: Property1";
      var expectedMessage2 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.InheritanceRootDerivedMappingAttributesClass\r\n"
        + "Property: Property3";
      Assert.That(validationResult.Length, Is.EqualTo(2));
      AssertMappingValidationResult(validationResult[0], false, expectedMessage1);
      AssertMappingValidationResult(validationResult[1], false, expectedMessage2);
    }

    [Test]
    [Ignore("TODO RM-4449: Utilities.ReflectionUtility.IsOriginalDeclaration does not work for Mixins")]
    public void NonOriginalPropertiesDeclarationWithMappingAttributeOnMixin_NoInheritanceRoot ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(ClassUsingMixinPropertiesNoInheritanceRoot));

      var validationResult = _validationRule.Validate(typeDefinition).ToArray();

      var expectedMessage1 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.InheritanceRootDerivedMappingAttributesClass\r\n"
        + "Property: Property1";
      var expectedMessage2 =
        "The 'StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definition.\r\n\r\n"
        + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule.InheritanceRootDerivedMappingAttributesClass\r\n"
        + "Property: Property3";
      Assert.That(validationResult.Length, Is.EqualTo(2));
      AssertMappingValidationResult(validationResult[0], false, expectedMessage1);
      AssertMappingValidationResult(validationResult[1], false, expectedMessage2);
    }
  }
}
