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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping.Validation;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Validation
{
  [TestFixture]
  public class ClassAboveTableIsAbstractValidationRuleTest : ValidationRuleTestBase
  {
    private ClassAboveTableIsAbstractValidationRule _validationRule;
    private ClassDefinition _concreteClassDefinition;

    private ClassDefinition _abstractClassDefinition;
    private TableDefinition _tableDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private EmptyViewDefinition _emptyViewDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new ClassAboveTableIsAbstractValidationRule();
      _abstractClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(DerivedValidationDomainObjectClass), isAbstract: true);
      _concreteClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(DerivedValidationDomainObjectClass), isAbstract: false);
      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition("DefaultStorageProvider");
      _tableDefinition = TableDefinitionObjectMother.Create(storageProviderDefinition, new EntityNameDefinition(null, "TableName"));
      _unionViewDefinition = UnionViewDefinitionObjectMother.Create(storageProviderDefinition);
      _emptyViewDefinition = EmptyViewDefinitionObjectMother.Create(storageProviderDefinition);
    }

    [Test]
    public void ClassTypeUnresolved_UnionViewDefinition_NotAbstract ()
    {
      var classDefinition = new ClassDefinitionWithUnresolvedType(
          "NonAbstractClassHasEntityNameDomainObject",
          typeof(DerivedValidationDomainObjectClass),
          false,
          null,
          new Mock<IPersistentMixinFinder>().Object,
          new Mock<IDomainObjectCreator>().Object);
      classDefinition.SetStorageEntity(_unionViewDefinition);

      var validationResult = _validationRule.Validate(classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_NoRdbmsStorageEntity_NotAbstract ()
    {
      var nonRdbmsStorageEntity = new Mock<IStorageEntityDefinition>();
      _concreteClassDefinition.SetStorageEntity(nonRdbmsStorageEntity.Object);

      var validationResult = _validationRule.Validate(_concreteClassDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_NoUnionViewDefinition_Abstract ()
    {
      _abstractClassDefinition.SetStorageEntity(_tableDefinition);

      var validationResult = _validationRule.Validate(_abstractClassDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_UnionViewDefinition_Abstract ()
    {
      _abstractClassDefinition.SetStorageEntity(_unionViewDefinition);

      var validationResult = _validationRule.Validate(_abstractClassDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_EmptyViewDefinition_Abstract ()
    {
      _abstractClassDefinition.SetStorageEntity(_emptyViewDefinition);

      var validationResult = _validationRule.Validate(_abstractClassDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_StorageEntityDefinitionIsNotSet ()
    {
      var validationResult = _validationRule.Validate(_abstractClassDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_UnionViewDefinition_NotAbstract ()
    {
      _concreteClassDefinition.SetStorageEntity(_unionViewDefinition);

      var validationResult = _validationRule.Validate(_concreteClassDefinition);

      var expectedMessage = "Neither class 'DerivedValidationDomainObjectClass' nor its base classes are mapped to a table. "
                            + "Make class 'DerivedValidationDomainObjectClass' abstract or define a table for it or one of its base classes.\r\n\r\n"
                            +
                            "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void ClassTypeResolved_EmptyViewDefinition_NotAbstract ()
    {
      _concreteClassDefinition.SetStorageEntity(_emptyViewDefinition);

      var validationResult = _validationRule.Validate(_concreteClassDefinition);

      var expectedMessage = "Neither class 'DerivedValidationDomainObjectClass' nor its base classes are mapped to a table. "
                            + "Make class 'DerivedValidationDomainObjectClass' abstract or define a table for it or one of its base classes.\r\n\r\n"
                            +
                            "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass";
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
