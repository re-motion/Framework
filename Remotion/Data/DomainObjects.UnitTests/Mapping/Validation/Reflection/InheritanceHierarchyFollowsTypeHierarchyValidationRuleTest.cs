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
  public class InheritanceHierarchyFollowsTypeHierarchyValidationRuleTest : ValidationRuleTestBase
  {
    private interface ISeparateInterface
    {
    }

    private interface IBaseInterface
    {
    }

    private interface ISubInterface : IBaseInterface
    {
    }

    private class SeparateClass
    {
    }

    private class BaseClass
    {
    }

    private class SubClass : BaseClass
    {
    }

    private InheritanceHierarchyFollowsTypeHierarchyValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new InheritanceHierarchyFollowsTypeHierarchyValidationRule();
    }

    [Test]
    public void ClassDefinitionWithoutBaseClass ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(SeparateClass));

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassDefinitionWithBaseClass_TypeIsDerivedFromBaseType ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(BaseClass));
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(SubClass), baseClass: baseClassDefinition);

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassDefinitionWithBaseClass_TypeIsNotDerivedFromBaseType ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition("Base", typeof(SeparateClass));
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(id: "Derived", classType: typeof(SubClass), baseClass: baseClassDefinition);

      var validationResult = _validationRule.Validate(typeDefinition);

      Assert.That(validationResult.IsValid, Is.False);
      var expectedMessageRegex =
          @"Type '.*?\.InheritanceHierarchyFollowsTypeHierarchyValidationRuleTest\+SubClass, Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*'"
          + @" of class 'Derived' is not derived from type '.*?\.InheritanceHierarchyFollowsTypeHierarchyValidationRuleTest\+SeparateClass, Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*'"
          + " of base class 'Base'.";
      Assert.That(validationResult.Message, Does.Match(expectedMessageRegex));
    }

    [Test]
    public void InterfaceDefinitionWithoutExtendedInterface ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateInterfaceDefinition(typeof(ISeparateInterface));

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InterfaceDefinitionWithExtendedInterface_TypeIsDerivedFromBaseType ()
    {
      var baseInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(IBaseInterface));
      var typeDefinition = TypeDefinitionObjectMother.CreateInterfaceDefinition(typeof(ISubInterface), extendedInterfaces: new[] { baseInterfaceDefinition });

      var validationResult = _validationRule.Validate(typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InterfaceDefinitionWithExtendedInterface_TypeIsNotDerivedFromBaseType ()
    {
      var baseInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ISeparateInterface));
      var typeDefinition = TypeDefinitionObjectMother.CreateInterfaceDefinition(typeof(IBaseInterface), extendedInterfaces: new[] { baseInterfaceDefinition });

      var validationResult = _validationRule.Validate(typeDefinition);

      Assert.That(validationResult.IsValid, Is.False);
      var expectedMessageRegex =
          @"Type '.*?\.InheritanceHierarchyFollowsTypeHierarchyValidationRuleTest\+IBaseInterface, Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*'"
          + @" does not extend interface '.*?\.InheritanceHierarchyFollowsTypeHierarchyValidationRuleTest\+ISeparateInterface, Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*'";
      Assert.That(validationResult.Message, Does.Match(expectedMessageRegex));
    }
  }
}
