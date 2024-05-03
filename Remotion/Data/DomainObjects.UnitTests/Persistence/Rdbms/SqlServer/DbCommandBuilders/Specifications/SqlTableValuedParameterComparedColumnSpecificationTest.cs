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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications;

[TestFixture]
public class SqlTableValuedParameterComparedColumnSpecificationTest
{
  private ColumnDefinition _columnDefinition;
  private object[] _guidCollection;
  private Mock<ISqlDialect> _sqlDialectMock;
  private SqlCommand _sqlCommand;
  private SqlTableValuedDataParameterDefinition _dataParameterDefinition;

  [SetUp]
  public void SetUp ()
  {
    var storageTypeInfo = StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation();
    _columnDefinition = new ColumnDefinition("Column", storageTypeInfo, false);
    _guidCollection =
    [
        new Guid("C1BF6D17-6D4B-468D-928B-5E0D7A42F275"),
        new Guid("22858375-AEB1-4F4F-9138-16F02DA3BAA0"),
        new Guid("D47EF6FE-F64D-4EAE-B70F-2E06814F7674")
    ];

    var storagePropertyStub = new Mock<IRdbmsStoragePropertyDefinition>();
    storagePropertyStub.Setup(_ => _.GetColumns()).Returns(new[] { _columnDefinition });
    storagePropertyStub.Setup(_ => _.SplitValue(It.IsAny<object>())).Returns((object value) => new[] { new ColumnValue(_columnDefinition, value) });

    var tableTypeDefinition = new TableTypeDefinition(
        new EntityNameDefinition(null, "GuidTableType"),
        [ storagePropertyStub.Object ],
        Array.Empty<ITableConstraintDefinition>());

    var recordDefinition = new RecordDefinition("GuidRecord", tableTypeDefinition, [new FakeRecordPropertyDefinition(storagePropertyStub.Object)]);

    _dataParameterDefinition = new SqlTableValuedDataParameterDefinition(recordDefinition);

    _sqlCommand = new SqlCommand();

    _sqlDialectMock = new Mock<ISqlDialect>(MockBehavior.Strict);
  }

  [Test]
  public void Initialize ()
  {
    var values = new object[] { 42, 17, 4 };
    var specification = new SqlTableValuedParameterComparedColumnSpecification(_columnDefinition, values, _dataParameterDefinition);

    Assert.That(specification.ComparedColumnDefinition, Is.SameAs(_columnDefinition));
    Assert.That(specification.DataParameterDefinition, Is.SameAs(_dataParameterDefinition));
    Assert.That(specification.ObjectValues, Is.EqualTo(values));
  }

  [Test]
  public void AddParameters_WithGuidColumn_AddsTableValuedParameter ()
  {
    _sqlDialectMock.Setup(_ => _.GetParameterName(_columnDefinition.Name)).Returns("@param");

    var specification = new SqlTableValuedParameterComparedColumnSpecification(_columnDefinition, _guidCollection, _dataParameterDefinition);
    specification.AddParameters(_sqlCommand, _sqlDialectMock.Object);

    Assert.That(_sqlCommand.Parameters.Count, Is.EqualTo(1));

    var sqlParameter = _sqlCommand.Parameters[0];
    Assert.That(sqlParameter.SqlDbType, Is.EqualTo(SqlDbType.Structured));
    Assert.That(sqlParameter.TypeName, Is.EqualTo("GuidTableType"));
    Assert.That(sqlParameter.Value, Is.InstanceOf<SqlTableValuedParameterValue>());

    var tvpValue = sqlParameter.Value.As<SqlTableValuedParameterValue>();

    var sqlMetadata = tvpValue.ColumnMetaData.Single();
    Assert.That(sqlMetadata.Name, Is.EqualTo("Column"));
    Assert.That(sqlMetadata.SqlDbType, Is.EqualTo(SqlDbType.UniqueIdentifier));

    var items = tvpValue.Select(record => record.GetGuid(0));
    Assert.That(items, Is.EquivalentTo(_guidCollection));
  }

  [Test]
  public void AddParameters_WithEmptyCollection_AddsNullValue ()
  {
    _sqlDialectMock.Setup(_ => _.GetParameterName(_columnDefinition.Name)).Returns("@param");

    var specification = new SqlTableValuedParameterComparedColumnSpecification(_columnDefinition, Array.Empty<object>(), _dataParameterDefinition);
    specification.AddParameters(_sqlCommand, _sqlDialectMock.Object);

    Assert.That(_sqlCommand.Parameters.Count, Is.EqualTo(1));

    var sqlParameter = _sqlCommand.Parameters[0];
    Assert.That(sqlParameter.SqlDbType, Is.EqualTo(SqlDbType.Structured));
    Assert.That(sqlParameter.TypeName, Is.EqualTo("GuidTableType"));
    Assert.That(sqlParameter.Value, Is.EqualTo(null));
  }

  private static IEnumerable<Guid[]> GetTestCasesFor_AppendComparisons ()
  {
    yield return [];
    yield return
    [
        new Guid("C1BF6D17-6D4B-468D-928B-5E0D7A42F275"),
        new Guid("22858375-AEB1-4F4F-9138-16F02DA3BAA0"),
        new Guid("D47EF6FE-F64D-4EAE-B70F-2E06814F7674")
    ];
  }

  [Test]
  [TestCaseSource(nameof(GetTestCasesFor_AppendComparisons))]
  public void AppendComparisons (Guid[] values)
  {
    _sqlDialectMock.Setup(_ => _.DelimitIdentifier("Value")).Returns("[delimitedValue]");
    _sqlDialectMock.Setup(_ => _.DelimitIdentifier(_columnDefinition.Name)).Returns("[delimitedColumn]");
    _sqlDialectMock.Setup(_ => _.GetParameterName(_columnDefinition.Name)).Returns("pColumn");

    var statementBuilder = new StringBuilder();
    var specification = new SqlTableValuedParameterComparedColumnSpecification(_columnDefinition, _guidCollection, _dataParameterDefinition);
    specification.AppendComparisons(statementBuilder, _sqlCommand, _sqlDialectMock.Object);

    Assert.That(statementBuilder.ToString(), Is.EqualTo("[delimitedColumn] IN (SELECT [delimitedValue] FROM pColumn)"));
  }
}
