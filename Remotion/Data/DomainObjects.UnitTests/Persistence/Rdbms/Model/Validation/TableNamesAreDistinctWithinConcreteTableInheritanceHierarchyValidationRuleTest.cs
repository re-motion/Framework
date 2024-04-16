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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping.Validation;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Validation
{
  [TestFixture]
  public class TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRuleTest : ValidationRuleTestBase
  {
    private TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule _validationRule;
    private ClassDefinition _baseOfBaseClass;
    private ClassDefinition _derivedBaseClass1;
    private ClassDefinition _derivedClass;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule();

      _baseOfBaseClass = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(BaseOfBaseValidationDomainObjectClass));
      _derivedBaseClass1 = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(BaseValidationDomainObjectClass), baseClass: _baseOfBaseClass);
      _derivedClass = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(DerivedValidationDomainObjectClass), baseClass: _derivedBaseClass1);

      _baseOfBaseClass.SetDerivedClasses(new[] { _derivedBaseClass1 });
      _derivedBaseClass1.SetDerivedClasses(new[] { _derivedClass });
      _derivedClass.SetDerivedClasses(new ClassDefinition[0]);

      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition("DefaultStorageProvider");
      _tableDefinition1 = TableDefinitionObjectMother.Create(storageProviderDefinition, new EntityNameDefinition(null, "TableName1"));
      _tableDefinition2 = TableDefinitionObjectMother.Create(storageProviderDefinition, new EntityNameDefinition(null, "TableName2"));
      _unionViewDefinition = UnionViewDefinitionObjectMother.Create(storageProviderDefinition);
    }

    [Test]
    public void NoInheritanceRoot ()
    {
      var validationResult = _validationRule.Validate(_derivedBaseClass1);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_NoTableDefinition ()
    {
      _baseOfBaseClass.SetStorageEntity(_unionViewDefinition);
      _derivedBaseClass1.SetStorageEntity(_unionViewDefinition);
      _derivedClass.SetStorageEntity(_unionViewDefinition);

      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      AssertMappingValidationResult(validationResult, true, null);
    }


    [Test]
    public void InheritanceRoot_UniqueTableDefinition ()
    {
      _baseOfBaseClass.SetStorageEntity(_tableDefinition1);
      _derivedBaseClass1.SetStorageEntity(_unionViewDefinition);
      _derivedClass.SetStorageEntity(_unionViewDefinition);

      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_DifferentTableNameInDerivedClass ()
    {
      _baseOfBaseClass.SetStorageEntity(_tableDefinition1);
      _derivedBaseClass1.SetStorageEntity(_tableDefinition2);
      _derivedClass.SetStorageEntity(_unionViewDefinition);

      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_StorageEntityDefinitionIsNotSet ()
    {
      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InheritanceRoot_SameTableNameInDerivedClass ()
    {
      _baseOfBaseClass.SetStorageEntity(_tableDefinition1);
      _derivedBaseClass1.SetStorageEntity(_tableDefinition1);
      _derivedClass.SetStorageEntity(_unionViewDefinition);

      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      var expectedMessage =
          "At least two classes in different inheritance branches derived from abstract class 'BaseOfBaseValidationDomainObjectClass' "
          + "specify the same entity name 'TableName1', which is not allowed.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_SameTableNameInDerivedDerivedClass ()
    {
      _baseOfBaseClass.SetStorageEntity(_tableDefinition1);
      _derivedBaseClass1.SetStorageEntity(_unionViewDefinition);
      _derivedClass.SetStorageEntity(_tableDefinition1);

      var validationResult = _validationRule.Validate(_baseOfBaseClass);

      var expectedMessage =
          "At least two classes in different inheritance branches derived from abstract class 'BaseOfBaseValidationDomainObjectClass' "
          + "specify the same entity name 'TableName1', which is not allowed.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void IgnoresArgumentsOfTypeOtherThanClassDefinition ()
    {
      var typeDefinitionForUnresolvedRelationPropertyType = new TypeDefinitionForUnresolvedRelationPropertyType(typeof(string), new NullPropertyInformation());
      var validationResult = _validationRule.Validate(typeDefinitionForUnresolvedRelationPropertyType);

      AssertMappingValidationResult(validationResult, true, null);
    }
  }
}
