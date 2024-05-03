using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications;

[TestFixture]
public class SqlTableValuedParameterComparedColumnSpecificationTest
{
  private ColumnDefinition _columnDefinition;
  private object[] _guidCollection;
  private Mock<ISqlDialect> _sqlDialectMock;
  private SqlCommand _sqlCommand;
  private SqlParameter _sqlParameter;

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

    _sqlCommand = new SqlCommand();
    _sqlParameter = _sqlCommand.CreateParameter();

    _sqlDialectMock = new Mock<ISqlDialect>(MockBehavior.Strict);
  }

  [Test]
  public void AddParameter_WithGuidColumn_AddsTableValuedParameter ()
  {
    _sqlDialectMock.Setup(_ => _.GetParameterName(_columnDefinition.Name)).Returns("@param");
    _sqlDialectMock.Setup(_ => _.CreateDataParameter(_sqlCommand, _columnDefinition.StorageTypeInfo, "@param", It.IsAny<object>())).Returns(_sqlParameter);

    var specification = new SqlTableValuedParameterComparedColumnSpecification(_columnDefinition, _guidCollection);
    specification.AddParameters(_sqlCommand, _sqlDialectMock.Object);

    Assert.That(_sqlCommand.Parameters.Count, Is.EqualTo(1));
    Assert.That(_sqlCommand.Parameters[0], Is.SameAs(_sqlParameter));

    Assert.That(_sqlParameter.SqlDbType, Is.EqualTo(SqlDbType.Structured));
    Assert.That(_sqlParameter.TypeName, Is.EqualTo("TVP_Guid"));
    Assert.That(_sqlParameter.Value, Is.InstanceOf<SqlTableValuedParameterValue>());

    var tvpValue = _sqlParameter.Value.As<SqlTableValuedParameterValue>();

    var sqlMetadata = tvpValue.ColumnMetaData.Single();
    Assert.That(sqlMetadata.Name, Is.EqualTo("Value"));
    Assert.That(sqlMetadata.SqlDbType, Is.EqualTo(SqlDbType.UniqueIdentifier));

    var items = tvpValue.Select(record => record.GetGuid(0));
    Assert.That(items, Is.EquivalentTo(_guidCollection));
  }

  [Test]
  public void AddParameters_WithEmptyCollection_AddsDBNull ()
  {
    _sqlDialectMock.Setup(_ => _.GetParameterName(_columnDefinition.Name)).Returns("@param");
    _sqlDialectMock.Setup(_ => _.CreateDataParameter(_sqlCommand, _columnDefinition.StorageTypeInfo, "@param", It.IsAny<object>())).Returns(_sqlParameter);

    var specification = new SqlTableValuedParameterComparedColumnSpecification(_columnDefinition, Array.Empty<object>());
    specification.AddParameters(_sqlCommand, _sqlDialectMock.Object);

    Assert.That(_sqlCommand.Parameters.Count, Is.EqualTo(1));
    Assert.That(_sqlCommand.Parameters[0], Is.SameAs(_sqlParameter));

    Assert.That(_sqlParameter.SqlDbType, Is.EqualTo(SqlDbType.Structured));
    Assert.That(_sqlParameter.TypeName, Is.EqualTo("TVP_Guid"));
    Assert.That(_sqlParameter.Value, Is.EqualTo(DBNull.Value));
  }

  [Test]
  public void AppendComparisons ()
  {
    _sqlDialectMock.Setup(_ => _.DelimitIdentifier("Value")).Returns("[delimitedValue]");
    _sqlDialectMock.Setup(_ => _.DelimitIdentifier(_columnDefinition.Name)).Returns("[delimitedColumn]");
    _sqlDialectMock.Setup(_ => _.GetParameterName(_columnDefinition.Name)).Returns("pColumn");

    var statementBuilder = new StringBuilder();
    var specification = new SqlTableValuedParameterComparedColumnSpecification(_columnDefinition, _guidCollection);
    specification.AppendComparisons(statementBuilder, _sqlCommand, _sqlDialectMock.Object);

    Assert.That(statementBuilder.ToString(), Is.EqualTo("[delimitedColumn] IN (SELECT [delimitedValue] FROM pColumn)"));
  }
}
