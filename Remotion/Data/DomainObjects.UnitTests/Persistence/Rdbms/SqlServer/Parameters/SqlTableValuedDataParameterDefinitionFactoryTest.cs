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
#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Parameters;

[TestFixture]
public class SqlTableValuedDataParameterDefinitionFactoryTest
{
  private static IEnumerable<(object, Type, bool, IStorageTypeInformation)> GetTestCasesForCreateParameterDefinition ()
  {
    yield return (Array.Empty<int>(), typeof(int), false, StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
    yield return (Array.AsReadOnly(Array.Empty<int>()), typeof(int), false, StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
    yield return (new ArrayList(), typeof(object), false, StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
    yield return (new List<int>(), typeof(int), false, StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
    yield return (Mock.Of<ICollection>(), typeof(object), false, StorageTypeInformationObjectMother.CreateStorageTypeInformation());
    yield return (Mock.Of<ICollection<DateTime>>(), typeof(DateTime), false, StorageTypeInformationObjectMother.CreateDateTimeStorageTypeInformation());
    yield return (Mock.Of<IReadOnlyCollection<Guid>>(), typeof(Guid), false, StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation());
    yield return (Mock.Of<ISet<string>>(), typeof(string), true, StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation());
#if NET5_0_OR_GREATER
    yield return (Mock.Of<IReadOnlySet<int>>(), typeof(int), true, StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
#endif
  }

  [Test]
  [TestCaseSource(nameof(GetTestCasesForCreateParameterDefinition))]
  public void CreateParameterDefinition ((object value, Type dotNetType, bool isDistinct, IStorageTypeInformation storageTypeInformation) testCase)
  {
    var query = Mock.Of<IQuery>();
    var queryParameter = new QueryParameter("Dummy", testCase.value);

    var columnDefinition = new ColumnDefinition("Value", testCase.storageTypeInformation, false);
    var storagePropertyDefinition = new SimpleStoragePropertyDefinition(testCase.dotNetType, columnDefinition);
    var tableTypeDefinition = new TableTypeDefinition(
        new EntityNameDefinition(null, "Test"),
        new[]
        {
            storagePropertyDefinition
        },
        testCase.isDistinct
            ? [ new UniqueConstraintDefinition($"UQ_Test_Value", true, new[] { columnDefinition }) ]
            : Array.Empty<ITableConstraintDefinition>());

    var propertyDefinitions = new[] { RecordPropertyDefinition.ScalarAsValue(storagePropertyDefinition) };

    var recordDefinition = new RecordDefinition("Test", tableTypeDefinition, propertyDefinitions);

    var recordDefinitionFactory = new Mock<IQueryParameterRecordDefinitionFinder>();
    recordDefinitionFactory
        .Setup(_ => _.GetRecordDefinition(queryParameter, query))
        .Returns(recordDefinition);

    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    storageTypeInformationProviderStub
        .Setup(_ => _.GetStorageType(testCase.dotNetType))
        .Returns(testCase.storageTypeInformation);
    storageTypeInformationProviderStub
        .Setup(_ => _.GetStorageType(It.Is<Type>(t => t != testCase.dotNetType)))
        .Returns(StorageTypeInformationObjectMother.CreateStorageTypeInformation());

    var nextFactoryStub = new Mock<IDataParameterDefinitionFactory>();
    var factory = new SqlTableValuedDataParameterDefinitionFactory(recordDefinitionFactory.Object, nextFactoryStub.Object);

    var result = factory.CreateDataParameterDefinition(queryParameter, query);

    Assert.That(result, Is.InstanceOf<SqlTableValuedDataParameterDefinition>());
    Assert.That(result.As<SqlTableValuedDataParameterDefinition>().RecordDefinition, Is.EqualTo(recordDefinition));
  }

  private static IEnumerable<object?> GetNonCollectionValues ()
  {
    yield return null;
    yield return "StringValue";
    yield return "CharArray".ToCharArray();
    yield return new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
    yield return Mock.Of<IEnumerable>();
  }

  [Test]
  [TestCaseSource(nameof(GetNonCollectionValues))]
  public void CreateParameterDefinition_WithNonCollection_ReturnsResultOfNextFactory (object value)
  {
    var query = Mock.Of<IQuery>();
    var queryParameter = new QueryParameter("SingleValue", value);

    var definitionStub = Mock.Of<IDataParameterDefinition>();

    var nextFactoryStub = new Mock<IDataParameterDefinitionFactory>();
    nextFactoryStub.Setup(_ => _.CreateDataParameterDefinition(queryParameter, query)).Returns(definitionStub);

    var queryParameterRecordDefinitionFactory = Mock.Of<IQueryParameterRecordDefinitionFinder>();

    var factory = new SqlTableValuedDataParameterDefinitionFactory(queryParameterRecordDefinitionFactory, nextFactoryStub.Object);
    var result = factory.CreateDataParameterDefinition(queryParameter, query);
    Assert.That(result, Is.SameAs(definitionStub));
  }
}
