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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

[TestFixture]
public class SingleScalarSqlTableTypeDefinitionProviderTest
{
  [Test]
  public void Initialize ()
  {
    var storageTypeInformationProvider = Mock.Of<IStorageTypeInformationProvider>();

    var singleScalarSqlTableTypeDefinitionProvider = new SingleScalarSqlTableTypeDefinitionProvider(storageTypeInformationProvider);

    Assert.That(singleScalarSqlTableTypeDefinitionProvider.StorageTypeInformationProvider, Is.SameAs(storageTypeInformationProvider));
  }

  [Test]
  public void GetAllStructuredTypeDefinitions_ReturnsAllTableTypes ()
  {
    var dummyExtensibleEnumType = typeof(SingleScalarSqlTableTypeDefinitionProvider).GetNestedType("DummyExtensibleEnum", BindingFlags.NonPublic);
    var typeMapping = new List<(Type dotNetType, bool withUniqueConstraint, IStorageTypeInformation storageTypeInfo)>
                      {
                          AddType<string>(DbType.String, false),
                          AddType<byte[]>(DbType.Binary, false),
                          Add(dummyExtensibleEnumType, DbType.AnsiString, false),
                          AddType<bool>(DbType.Boolean, false),
                          AddType<bool>(DbType.Boolean, true),
                          AddType<byte>(DbType.Byte, false),
                          AddType<byte>(DbType.Byte, true),
                          AddType<short>(DbType.Int16, false),
                          AddType<short>(DbType.Int16, true),
                          AddType<int>(DbType.Int32, false),
                          AddType<int>(DbType.Int32, true),
                          AddType<long>(DbType.Int64, false),
                          AddType<long>(DbType.Int64, true),
                          AddType<decimal>(DbType.Decimal, false),
                          AddType<decimal>(DbType.Decimal, true),
                          AddType<float>(DbType.Single, false),
                          AddType<float>(DbType.Single, true),
                          AddType<double>(DbType.Double, false),
                          AddType<double>(DbType.Double, true),
                          AddType<DateTime>(DbType.DateTime2, false),
                          AddType<DateTime>(DbType.DateTime2, true),
                          AddType<DateOnly>(DbType.Date, false),
                          AddType<DateOnly>(DbType.Date, true),
                          AddType<Guid>(DbType.Guid, false),
                          AddType<Guid>(DbType.Guid, true)
                      };

    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    foreach (var mapping in typeMapping)
      storageTypeInformationProviderStub.Setup(_ => _.GetStorageType(mapping.dotNetType)).Returns(mapping.storageTypeInfo);

    var repository = new SingleScalarSqlTableTypeDefinitionProvider(storageTypeInformationProviderStub.Object);
    var result = repository.GetAllStructuredTypeDefinitions().ToArray();

    Assert.That(result.Length, Is.EqualTo(typeMapping.Count));
    for (var i = 0; i < typeMapping.Count; i++)
      AssertTableType(typeMapping[i], result[i]);

    (Type dotNetType, bool withUniqueConstraint, IStorageTypeInformation storageTypeInfo) AddType<T> (DbType dbType, bool withUniqueConstraint)
    {
      return Add(typeof(T), dbType, withUniqueConstraint);
    }

    (Type dotNetType, bool withUniqueConstraint, IStorageTypeInformation storageTypeInfo) Add (Type type, DbType dbType, bool withUniqueConstraint)
    {
      if (!NullableTypeUtility.IsNullableType(type))
        type = typeof(Nullable<>).MakeGenericType(type);

      var storageTypeInformationStub = new Mock<IStorageTypeInformation>();
      storageTypeInformationStub.Setup(_ => _.DotNetType).Returns(type);
      storageTypeInformationStub.Setup(_ => _.StorageDbType).Returns(dbType);
      return (type, withUniqueConstraint, storageTypeInformationStub.Object);
    }

    void AssertTableType ((Type dotNetType, bool withUniqueConstraint, IStorageTypeInformation storageTypeInformation) expected, IRdbmsStructuredTypeDefinition actual)
    {
      var dotnetType = expected.dotNetType;
      var dbType = expected.storageTypeInformation.StorageDbType;
      var isDistinct = expected.withUniqueConstraint;
      Assert.That(actual, Is.InstanceOf<TableTypeDefinition>());
      var entityNameDefinition = actual.As<TableTypeDefinition>().TypeName;
      Assert.That(entityNameDefinition.EntityName, Is.EqualTo($"TVP_{dbType}{(isDistinct ? "_Distinct" : "")}"));
      Assert.That(entityNameDefinition.SchemaName, Is.Null);

      Assert.That(actual.Properties.Single(), Is.InstanceOf<SimpleStoragePropertyDefinition>());
      Assert.That(actual.Properties.Single().As<SimpleStoragePropertyDefinition>().PropertyType, Is.EqualTo(dotnetType));
      Assert.That(actual.Properties.Single().As<SimpleStoragePropertyDefinition>().ColumnDefinition.Name, Is.EqualTo("Value"));
      Assert.That(actual.Properties.Single().As<SimpleStoragePropertyDefinition>().ColumnDefinition.StorageTypeInfo.DotNetType, Is.EqualTo(dotnetType));

      if (isDistinct)
      {
        Assert.That(actual.As<TableTypeDefinition>().Constraints.Count, Is.EqualTo(1));
        var constraints = actual.As<TableTypeDefinition>().Constraints;
        Assert.That(constraints.Count, Is.EqualTo(1));

        var constraint = constraints.Single();
        Assert.That(constraint, Is.InstanceOf<UniqueConstraintDefinition>());
        Assert.That(constraint.ConstraintName, Is.EqualTo($"UQ_{entityNameDefinition.EntityName}_Value"));
        var columnDefinitions = constraint.As<UniqueConstraintDefinition>().Columns;
        Assert.That(columnDefinitions.Count, Is.EqualTo(1));
        Assert.That(columnDefinitions.Single(), Is.SameAs(actual.Properties.Single().As<SimpleStoragePropertyDefinition>().ColumnDefinition));
      }
      else
      {
        Assert.That(actual.As<TableTypeDefinition>().Constraints, Is.Empty);
      }
    }
  }

  [Test]
  [TestCase(typeof(string), DbType.String, false)]
  [TestCase(typeof(byte[]), DbType.Binary, false)]
  [TestCase(typeof(Color), DbType.AnsiString, false)]
  [TestCase(typeof(bool), DbType.Boolean, false)]
  [TestCase(typeof(bool?), DbType.Boolean, true)]
  [TestCase(typeof(byte), DbType.Byte, false)]
  [TestCase(typeof(byte?), DbType.Byte, true)]
  [TestCase(typeof(short), DbType.Int16, false)]
  [TestCase(typeof(short?), DbType.Int16, true)]
  [TestCase(typeof(int), DbType.Int32, false)]
  [TestCase(typeof(int?), DbType.Int32, true)]
  [TestCase(typeof(long), DbType.Int64, false)]
  [TestCase(typeof(long?), DbType.Int64, true)]
  [TestCase(typeof(decimal), DbType.Decimal, false)]
  [TestCase(typeof(decimal?), DbType.Decimal, true)]
  [TestCase(typeof(float), DbType.Single, false)]
  [TestCase(typeof(float?), DbType.Single, true)]
  [TestCase(typeof(double), DbType.Double, false)]
  [TestCase(typeof(double?), DbType.Double, true)]
  [TestCase(typeof(DateTime), DbType.DateTime2, false)]
  [TestCase(typeof(DateTime?), DbType.DateTime2, true)]
  [TestCase(typeof(DateOnly), DbType.Date, false)]
  [TestCase(typeof(DateOnly?), DbType.Date, true)]
  [TestCase(typeof(Guid), DbType.Guid, false)]
  [TestCase(typeof(Guid?), DbType.Guid, true)]
  [TestCase(typeof(ClassWithAllDataTypes.EnumType), DbType.AnsiString, false)]
  [TestCase(typeof(ClassWithAllDataTypes.EnumType?), DbType.AnsiString, true)]
  public void GetStructuredTypeDefinition_SupportsType (Type elementType, DbType dbType, bool withUniqueConstraint)
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation(storageDbType: dbType, dotNetType: elementType);

    var effectiveType = NullableTypeUtility.IsNullableType(elementType) ? elementType : typeof(Nullable<>).MakeGenericType(elementType);
    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    if (typeof(IExtensibleEnum).IsAssignableFrom(effectiveType))
    {
      var dummyExtensibleEnumType = typeof(SingleScalarSqlTableTypeDefinitionProvider).GetNestedType("DummyExtensibleEnum", BindingFlags.NonPublic);
      Assert.That(dummyExtensibleEnumType, Is.Not.Null);
      storageTypeInformationProviderStub.Setup(_ => _.GetStorageType(dummyExtensibleEnumType)).Returns(storageTypeInformation);
    }
    else
    {
      storageTypeInformationProviderStub.Setup(_ => _.GetStorageType(effectiveType)).Returns(storageTypeInformation);
    }

    var finder = new SingleScalarSqlTableTypeDefinitionProvider(storageTypeInformationProviderStub.Object);
    var typeDefinition = finder.GetStructuredTypeDefinition(elementType, withUniqueConstraint);
    Assert.That(typeDefinition, Is.Not.Null);
    Assert.That(typeDefinition.Properties.Count, Is.EqualTo(1));
    Assert.That(typeDefinition.Properties.Single(), Is.InstanceOf<SimpleStoragePropertyDefinition>());

    var property = typeDefinition.Properties.Single().As<SimpleStoragePropertyDefinition>();
    Assert.That(property.ColumnDefinition.Name, Is.EqualTo("Value"));
    Assert.That(property.ColumnDefinition.StorageTypeInfo, Is.SameAs(storageTypeInformation));
    Assert.That(property.ColumnDefinition.IsPartOfPrimaryKey, Is.False);

    Assert.That(typeDefinition, Is.InstanceOf<TableTypeDefinition>());
    var constraintDefinitions = typeDefinition.As<TableTypeDefinition>().Constraints;
    if (withUniqueConstraint)
    {
      Assert.That(constraintDefinitions.Count, Is.EqualTo(1));
      Assert.That(constraintDefinitions.Single(), Is.InstanceOf<UniqueConstraintDefinition>());

      var uniqueConstraint = constraintDefinitions.Single().As<UniqueConstraintDefinition>();
      Assert.That(uniqueConstraint.IsClustered, Is.True);
      Assert.That(uniqueConstraint.Columns, Is.EqualTo(new[] { property.ColumnDefinition }));
    }
    else
    {
      Assert.That(constraintDefinitions, Is.Empty);
    }
  }

  [TestCase(typeof(int?), typeof(int))]
  [TestCase(typeof(DateTime?), typeof(DateTime))]
  [TestCase(typeof(DateOnly?), typeof(DateOnly))]
  [TestCase(typeof(Guid?), typeof(Guid))]
  public void GetStructuredTypeDefinition_ForValueType_ReturnsValueForNullable (Type nullableType, Type underlyingType)
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation();

    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    storageTypeInformationProviderStub.Setup(_ => _.GetStorageType(nullableType)).Returns(storageTypeInformation);

    var finder = new SingleScalarSqlTableTypeDefinitionProvider(storageTypeInformationProviderStub.Object);
    var typeDefinition = finder.GetStructuredTypeDefinition(underlyingType, false);
    Assert.That(typeDefinition, Is.Not.Null);
    Assert.That(typeDefinition.Properties.Single().GetColumns().Single().StorageTypeInfo, Is.SameAs(storageTypeInformation));
  }

  [Test]
  [TestCase(typeof(int))]
  [TestCase(typeof(int?))]
  [TestCase(typeof(string))]
  [TestCase(typeof(IExtensibleEnum))]
  public void GetStructuredTypeDefinition_ReusesInstances (Type elementType)
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation();

    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    storageTypeInformationProviderStub.Setup(_ => _.GetStorageType(It.IsAny<Type>())).Returns(storageTypeInformation);

    var finder = new SingleScalarSqlTableTypeDefinitionProvider(storageTypeInformationProviderStub.Object);
    var typeDefinition = finder.GetStructuredTypeDefinition(elementType, false);
    Assert.That(typeDefinition, Is.Not.Null);

    var typeDefinition2 = finder.GetStructuredTypeDefinition(elementType, false);
    Assert.That(typeDefinition2, Is.SameAs(typeDefinition2));
  }
}
