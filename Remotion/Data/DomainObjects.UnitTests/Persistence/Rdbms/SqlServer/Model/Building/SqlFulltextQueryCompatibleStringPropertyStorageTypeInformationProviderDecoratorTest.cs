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
using System.Data;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Development.UnitTesting.ObjectMothers;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Model.Building
{
  [TestFixture]
  public class SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecoratorTest
  {
    [Test]
    public void GetStorageTypeForID ()
    {
      var expectedValue = StorageTypeInformationObjectMother.CreateStorageTypeInformation();
      var innerStub = new Mock<IStorageTypeInformationProvider>();

      var isStorageTypeNullable = BooleanObjectMother.GetRandomBoolean();
      innerStub.Setup(_ => _.GetStorageTypeForID(isStorageTypeNullable)).Returns(expectedValue);

      var decorator = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(innerStub.Object);

      Assert.That(decorator.GetStorageTypeForID(isStorageTypeNullable), Is.SameAs(expectedValue));
    }

    [Test]
    public void GetStorageTypeForClassID ()
    {
      var expectedValue = StorageTypeInformationObjectMother.CreateStorageTypeInformation();
      var innerStub = new Mock<IStorageTypeInformationProvider>();

      var isStorageTypeNullable = BooleanObjectMother.GetRandomBoolean();
      innerStub.Setup(_ => _.GetStorageTypeForClassID(isStorageTypeNullable)).Returns(expectedValue);

      var decorator = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(innerStub.Object);

      Assert.That(decorator.GetStorageTypeForClassID(isStorageTypeNullable), Is.SameAs(expectedValue));
    }

    [Test]
    public void GetStorageTypeForSerializedObjectID ()
    {
      var expectedValue = StorageTypeInformationObjectMother.CreateStorageTypeInformation();
      var innerStub = new Mock<IStorageTypeInformationProvider>();

      var isStorageTypeNullable = BooleanObjectMother.GetRandomBoolean();
      innerStub.Setup(_ => _.GetStorageTypeForSerializedObjectID(isStorageTypeNullable)).Returns(expectedValue);

      var decorator = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(innerStub.Object);

      Assert.That(decorator.GetStorageTypeForSerializedObjectID(isStorageTypeNullable), Is.SameAs(expectedValue));
    }

    [Test]
    public void GetStorageTypeForTimestamp ()
    {
      var expectedValue = StorageTypeInformationObjectMother.CreateStorageTypeInformation();
      var innerStub = new Mock<IStorageTypeInformationProvider>();

      var isStorageTypeNullable = BooleanObjectMother.GetRandomBoolean();
      innerStub.Setup(_ => _.GetStorageTypeForTimestamp(isStorageTypeNullable)).Returns(expectedValue);

      var decorator = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(innerStub.Object);

      Assert.That(decorator.GetStorageTypeForTimestamp(isStorageTypeNullable), Is.SameAs(expectedValue));
    }

    [Test]
    [TestCase(
        DbType.AnsiString,
        100,
        TestName = "GetStorageType_ForPropertyDefinition_WithStorageTypeLength_AndAnsiString_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.AnsiStringFixedLength,
        100,
        TestName = "GetStorageType_ForPropertyDefinition_WithStorageTypeLength_AndAnsiStringFixedLength_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.String,
        100,
        TestName = "GetStorageType_ForPropertyDefinition_WithStorageTypeLength_AndString_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.StringFixedLength,
        100,
        TestName = "GetStorageType_ForPropertyDefinition_WithStorageTypeLength_AndStringFixedLength_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.Int32,
        null,
        TestName = "GetStorageType_ForPropertyDefinition_WithoutStorageTypeLength_AndOtherDbType_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.AnsiStringFixedLength,
        null,
        TestName = "GetStorageType_ForPropertyDefinition_WithoutStorageTypeLength_AndAnsiStringFixedLength_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.StringFixedLength,
        null,
        TestName = "GetStorageType_ForPropertyDefinition_WithoutStorageTypeLength_AndStringFixedLength_ReturnsOriginalStorageTypeInformation")]
    public void GetStorageType_ForPropertyDefinition_ReturnsOriginalStorageTypeInformation (
        DbType storageDbType,
        int? storageTypeLength)
    {
      var expectedValue = StorageTypeInformationObjectMother.CreateStorageTypeInformation(
          storageDbType: storageDbType,
          storageTypeLength: storageTypeLength);
      var innerStub = new Mock<IStorageTypeInformationProvider>();

      var propertyDefinition = CreatePropertyDefinition();
      var forceNullable = BooleanObjectMother.GetRandomBoolean();
      innerStub.Setup(_ => _.GetStorageType(propertyDefinition, forceNullable)).Returns(expectedValue);

      var decorator = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(innerStub.Object);

      Assert.That(decorator.GetStorageType(propertyDefinition, forceNullable), Is.SameAs(expectedValue));
    }

    [Test]
    [TestCase(DbType.AnsiString, 8000, TestName = "GetStorageType_ForPropertyDefinition_WithoutStorageTypeLength_AndAnsiString_ReturnsDecoratedStorageTypeInformation")]
    [TestCase(DbType.String, 4000, TestName = "GetStorageType_ForPropertyDefinition_WithoutStorageTypeLength_AndString_ReturnsDecoratedStorageTypeInformation")]
    public void GetStorageType_ForPropertyDefinition_WithoutStorageTypeLength_ReturnsDecoratedStorageTypeInformation (
        DbType storageDbType,
        int supportedMaxLength)
    {
      var expectedValue = StorageTypeInformationObjectMother.CreateStorageTypeInformation(storageDbType: storageDbType, storageTypeLength: null);
      var innerStub = new Mock<IStorageTypeInformationProvider>();

      var propertyDefinition = CreatePropertyDefinition();
      var forceNullable = BooleanObjectMother.GetRandomBoolean();
      innerStub.Setup(_ => _.GetStorageType(propertyDefinition, forceNullable)).Returns(expectedValue);

      var decorator = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(innerStub.Object);

      var result = decorator.GetStorageType(propertyDefinition, forceNullable);

      Assert.That(result, Is.InstanceOf<SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator>());
      var decoratedTypeInfo = (SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator)result;
      Assert.That(decoratedTypeInfo.FulltextCompatibleMaxLength, Is.EqualTo(supportedMaxLength));
      Assert.That(decoratedTypeInfo.InnerStorageTypeInformation, Is.SameAs(expectedValue));
    }

    [Test]
    [TestCase(DbType.AnsiString, 100, TestName = "GetStorageType_ForType_WithStorageTypeLength_AndAnsiString_ReturnsOriginalStorageTypeInformation")]
    [TestCase(DbType.AnsiStringFixedLength, 100, TestName = "GetStorageType_ForType_WithStorageTypeLength_AndAnsiStringFixedLength_ReturnsOriginalStorageTypeInformation")]
    [TestCase(DbType.String, 100, TestName = "GetStorageType_ForType_WithStorageTypeLength_AndString_ReturnsOriginalStorageTypeInformation")]
    [TestCase(DbType.StringFixedLength, 100, TestName = "GetStorageType_ForType_WithStorageTypeLength_AndStringFilxedLength_ReturnsOriginalStorageTypeInformation")]
    [TestCase(DbType.Int32, null, TestName = "GetStorageType_ForType_WithoutStorageTypeLength_AndOtherDbType_ReturnsOriginalStorageTypeInformation")]
    [TestCase(DbType.AnsiStringFixedLength, null, TestName = "GetStorageType_ForType_WithoutStorageTypeLength_AndAnsiStringFixedLength_ReturnsOriginalStorageTypeInformation")]
    [TestCase(DbType.StringFixedLength, null, TestName = "GetStorageType_ForType_WithoutStorageTypeLength_AndStringFixedLength_ReturnsOriginalStorageTypeInformation")]
    public void GetStorageType_ForType_ReturnsOriginalStorageTypeInformation (
        DbType storageDbType,
        int? storageTypeLength)
    {
      var expectedValue = StorageTypeInformationObjectMother.CreateStorageTypeInformation(
          storageDbType: storageDbType,
          storageTypeLength: storageTypeLength);
      var innerStub = new Mock<IStorageTypeInformationProvider>();

      var type = typeof(object);
      innerStub.Setup(_ => _.GetStorageType(type)).Returns(expectedValue);

      var decorator = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(innerStub.Object);

      Assert.That(decorator.GetStorageType(type), Is.SameAs(expectedValue));
    }

    [Test]
    [TestCase(DbType.AnsiString, 8000, TestName = "GetStorageType_ForType_WithoutStorageTypeLength_AndAnsiString_ReturnsDecoratedStorageTypeInformation")]
    [TestCase(DbType.String, 4000, TestName = "GetStorageType_ForType_WithoutStorageTypeLength_AndString_ReturnsDecoratedStorageTypeInformation")]
    public void GetStorageType_ForType_WithoutStorageTypeLength_ReturnsDecoratedStorageTypeInformation (DbType storageDbType, int supportedMaxLength)
    {
      var expectedValue = StorageTypeInformationObjectMother.CreateStorageTypeInformation(storageDbType: storageDbType, storageTypeLength: null);
      var innerStub = new Mock<IStorageTypeInformationProvider>();

      var type = typeof(object);
      innerStub.Setup(_ => _.GetStorageType(type)).Returns(expectedValue);

      var decorator = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(innerStub.Object);

      var result = decorator.GetStorageType(type);

      Assert.That(result, Is.InstanceOf<SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator>());
      var decoratedTypeInfo = (SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator)result;
      Assert.That(decoratedTypeInfo.FulltextCompatibleMaxLength, Is.EqualTo(supportedMaxLength));
      Assert.That(decoratedTypeInfo.InnerStorageTypeInformation, Is.SameAs(expectedValue));
    }

    [Test]
    [TestCase(
        DbType.AnsiString,
        100,
        "value",
        TestName = "GetStorageType_FoValue_WithStorageTypeLength_AndAnsiString_AndValueNotNull_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.AnsiStringFixedLength,
        100,
        "value",
        TestName = "GetStorageType_FoValue_WithStorageTypeLength_AndAnsiStringFixedLength_AndValueNotNull_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.String,
        100,
        "value",
        TestName = "GetStorageType_ForValue_WithStorageTypeLength_AndString_AndValueNotNull_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.StringFixedLength,
        100,
        "value",
        TestName = "GetStorageType_ForValue_WithStorageTypeLength_AndStringFixedLength_AndValueNotNull_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.Int32,
        null,
        5,
        TestName = "GetStorageType_ForValue_WithoutStorageTypeLength_AndOtherDbType_AndValueNotNull_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.Int32,
        null,
        null,
        TestName = "GetStorageType_ForValue_WithoutStorageTypeLength_AndOtherDbType_AndValueNull_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.AnsiStringFixedLength,
        null,
        null,
        TestName = "GetStorageType_ForValue_WithoutStorageTypeLength_AndAnsiStringFixedLength_AndValueNull_ReturnsOriginalStorageTypeInformation")]
    [TestCase(
        DbType.StringFixedLength,
        null,
        null,
        TestName = "GetStorageType_ForValue_WithoutStorageTypeLength_AndStringFixedLength_AndValueNull_ReturnsOriginalStorageTypeInformation")]
    public void GetStorageType_ForValue_ReturnsOriginalStorageTypeInformation (
        DbType storageDbType,
        int? storageTypeLength,
        object value)
    {
      var expectedValue = StorageTypeInformationObjectMother.CreateStorageTypeInformation(
          storageDbType: storageDbType,
          storageTypeLength: storageTypeLength);
      var innerStub = new Mock<IStorageTypeInformationProvider>();

      innerStub.Setup(_ => _.GetStorageType(value)).Returns(expectedValue);

      var decorator = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(innerStub.Object);

      Assert.That(decorator.GetStorageType(value), Is.SameAs(expectedValue));
    }

    [Test]
    [TestCase(DbType.AnsiString, 8000, "value", TestName = "GetStorageType_ForValue_WithoutStorageTypeLength_AndAnsiString_AndValueNotNull_ReturnsDecoratedStorageTypeInformation")]
    [TestCase(DbType.String, 4000, "value", TestName = "GetStorageType_ForValue_WithoutStorageTypeLength_AndString_AndValueNotNull_ReturnsDecoratedStorageTypeInformation")]
    [TestCase(DbType.AnsiString, 8000, null, TestName = "GetStorageType_ForValue_WithoutStorageTypeLength_AndAnsiString_AndValueNull_ReturnsDecoratedStorageTypeInformation")]
    [TestCase(DbType.String, 4000, null, TestName = "GetStorageType_ForValue_WithoutStorageTypeLength_AndString_AndValueNull_ReturnsDecoratedStorageTypeInformation")]
    public void GetStorageType_ForValue_WithoutStorageTypeLength_ReturnsDecoratedStorageTypeInformation (
        DbType storageDbType,
        int supportedMaxLength,
        object value)
    {
      var expectedValue = StorageTypeInformationObjectMother.CreateStorageTypeInformation(storageDbType: storageDbType, storageTypeLength: null);
      var innerStub = new Mock<IStorageTypeInformationProvider>();

      innerStub.Setup(_ => _.GetStorageType(value)).Returns(expectedValue);

      var decorator = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(innerStub.Object);

      var result = decorator.GetStorageType(value);

      Assert.That(result, Is.InstanceOf<SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator>());
      var decoratedTypeInfo = (SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator)result;
      Assert.That(decoratedTypeInfo.FulltextCompatibleMaxLength, Is.EqualTo(supportedMaxLength));
      Assert.That(decoratedTypeInfo.InnerStorageTypeInformation, Is.SameAs(expectedValue));
    }

    private PropertyDefinition CreatePropertyDefinition ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition();
      return PropertyDefinitionObjectMother.CreateForFakePropertyInfo(
          typeDefinition,
          "Name",
          false,
          typeof(object),
          true,
          null,
          StorageClass.Persistent);
    }
  }
}
