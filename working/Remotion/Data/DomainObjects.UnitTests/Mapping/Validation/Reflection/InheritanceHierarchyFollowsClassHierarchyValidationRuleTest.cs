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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Reflection
{
  [TestFixture]
  public class InheritanceHierarchyFollowsClassHierarchyValidationRuleTest : ValidationRuleTestBase
  {
    private InheritanceHierarchyFollowsClassHierarchyValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new InheritanceHierarchyFollowsClassHierarchyValidationRule();
    }

    [Test]
    public void ClassDefinitionWithoutBaseClass ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (BaseOfBaseValidationDomainObjectClass));

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassDefinitionWithBaseClass_ClassTypeIsDerivedFromBaseClassType ()
    {
      var baseType = typeof (BaseOfBaseValidationDomainObjectClass);
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: baseType);
      var derivedType = typeof (BaseValidationDomainObjectClass);
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: derivedType, baseClass: baseClassDefinition);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassDefinitionWithBaseClass_ClassTypeIsNotDerivedFromBaseClassType ()
    {
      var baseType = typeof (BaseOfBaseValidationDomainObjectClass);
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("Base", baseType);
      var derivedType = typeof (BaseValidationDomainObjectClass);
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (id: "Derived", classType: derivedType, baseClass: baseClassDefinition);
      PrivateInvoke.SetNonPublicField (classDefinition, "_classType", typeof (ClassOutOfInheritanceHierarchy));

      var validationResult = _validationRule.Validate (classDefinition);

      Assert.That (validationResult.IsValid, Is.False);
      var expectedMessage =
         @"Type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.ClassOutOfInheritanceHierarchy, Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*' of class 'Derived' is not derived from type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass, Remotion.Data.DomainObjects.UnitTests, Version=.*, Culture=.*, PublicKeyToken=.*' of base class 'Base'\.";
      Assert.That (validationResult.Message, Is.StringMatching (expectedMessage));
    }
  }
}