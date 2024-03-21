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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class ReflectionBasedStorageNameProviderTest
  {
    private ReflectionBasedStorageNameProvider _provider;
    private ClassDefinition _classDefinition;
    private ClassDefinition _classDefinitionWithoutTable;

    [SetUp]
    public void SetUp ()
    {
      _provider = new ReflectionBasedStorageNameProvider();
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition("Company", classType: typeof(Company), baseClass: null);
      _classDefinitionWithoutTable = ClassDefinitionObjectMother.CreateClassDefinition("Partner", classType: typeof(Partner), baseClass: null);
    }

    [Test]
    public void IDColumnName ()
    {
      Assert.That(_provider.GetIDColumnName(), Is.EqualTo("ID"));
    }

    [Test]
    public void ClassIDColumnName ()
    {
      Assert.That(_provider.GetClassIDColumnName(), Is.EqualTo("ClassID"));
    }

    [Test]
    public void TimestampColumnName ()
    {
      Assert.That(_provider.GetTimestampColumnName(), Is.EqualTo("Timestamp"));
    }

    [Test]
    public void GetTableName_ClassHasDBTableAttributeWithoutName_ReturnsClassIDName ()
    {
      var result = _provider.GetTableName(_classDefinition).EntityName;

      Assert.That(result, Is.EqualTo("Company"));
    }

    [Test]
    public void GetTableName_ClassHasDBTableAttributeWithtName_ReturnsAttributeName ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(ClassHavingStorageSpecificIdentifierAttribute), baseClass: null);

      var result = _provider.GetTableName(classDefinition).EntityName;

      Assert.That(result, Is.EqualTo("ClassHavingStorageSpecificIdentifierAttributeTable"));
    }

    [Test]
    public void GetTableName_ClassHasNoDBTableAttribute_ReturnsNull ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Folder), baseClass: null);

      var result = _provider.GetTableName(classDefinition);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetViewName ()
    {
      var result = _provider.GetViewName(_classDefinition).EntityName;

      Assert.That(result, Is.EqualTo("CompanyView"));
    }

    [Test]
    public void GetColumnName_PropertyWithIStorageSpecificIdentifierAttribute_ReturnsNameFromAttribute ()
    {
      var classWithAllDataTypesDefinition =
          TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(ClassWithAllDataTypes), baseClass: null);
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(
          classWithAllDataTypesDefinition, typeof(ClassWithAllDataTypes), "BooleanProperty");

      var result = _provider.GetColumnName(propertyDefinition);

      Assert.That(result, Is.EqualTo("Boolean"));
    }

    [Test]
    public void GetColumnName_PropertyWithoutIStorageSpecificIdentifierAttribute_ReturnsPropertyName ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(Distributor), baseClass: null);
      typeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(typeDefinition, typeof(Distributor), "NumberOfShops");

      var result = _provider.GetColumnName(propertyDefinition);

      Assert.That(result, Is.EqualTo("NumberOfShops"));
    }

    [Test]
    public void GetRelationColumnName ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(FileSystemItem), baseClass: null);
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(typeDefinition, typeof(FileSystemItem), "ParentFolder");
      var relationDefinition = new RelationEndPointDefinition(propertyDefinition, true);

      var result = _provider.GetRelationColumnName(relationDefinition);

      Assert.That(result, Is.EqualTo("ParentFolderID"));
    }

    [Test]
    public void GetRelationColumnName_PropertyWithIStorageSpecificIdentifierAttribute_ReturnsNameFromAttribute ()
    {
      var typeDefinition =
          TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(FileSystemItem), baseClass: null);
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(typeDefinition, typeof(FileSystemItem), "ParentFolder2");
      var relationDefinition = new RelationEndPointDefinition(propertyDefinition, true);

      var result = _provider.GetRelationColumnName(relationDefinition);

      Assert.That(result, Is.EqualTo("ParentFolderRelation"));
    }

    [Test]
    public void GetRelationClassIDColumnName ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(FileSystemItem), baseClass: null);
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(typeDefinition, typeof(FileSystemItem), "ParentFolder");
      var relationDefinition = new RelationEndPointDefinition(propertyDefinition, true);

      var result = _provider.GetRelationClassIDColumnName(relationDefinition);

      Assert.That(result, Is.EqualTo("ParentFolderIDClassID"));
    }

    [Test]
    public void GetPrimaryKeyName ()
    {
      var result = _provider.GetPrimaryKeyConstraintName(_classDefinition);

      Assert.That(result, Is.EqualTo("PK_Company"));
    }

    [Test]
    public void GetPrimaryKeyName_WithoutTableName ()
    {
      Assert.That(
          () => _provider.GetPrimaryKeyConstraintName(_classDefinitionWithoutTable),
          Throws.Exception.TypeOf<MappingException>()
              .With.Message.EqualTo("Class 'Partner' cannot not define a primary key constraint because no table name has been defined."));
    }

    [Test]
    public void GetForeignKeyConstraintName_One ()
    {
      var columnDefinition = ColumnDefinitionObjectMother.CreateColumn("FakeColumn");

      var result = _provider.GetForeignKeyConstraintName(_classDefinition, new[] { columnDefinition });

      Assert.That(result, Is.EqualTo("FK_Company_FakeColumn"));
    }

    [Test]
    public void GetForeignKeyConstraintName_Many ()
    {
      var columnDefinition1 = ColumnDefinitionObjectMother.CreateColumn("FakeColumn1");
      var columnDefinition2 = ColumnDefinitionObjectMother.CreateColumn("FakeColumn2");

      var result = _provider.GetForeignKeyConstraintName(_classDefinition, new[] { columnDefinition1, columnDefinition2 });

      Assert.That(result, Is.EqualTo("FK_Company_FakeColumn1_FakeColumn2"));
    }

    [Test]
    public void GetForeignKeyConstraintName_WithoutTableName ()
    {
      var columnDefinition = ColumnDefinitionObjectMother.CreateColumn("FakeColumn");

      Assert.That(
          () => _provider.GetForeignKeyConstraintName(_classDefinitionWithoutTable, new[] { columnDefinition }),
          Throws.Exception.TypeOf<MappingException>()
              .With.Message.EqualTo("Class 'Partner' cannot not define a foreign key constraint because no table name has been defined."));
    }
  }
}
