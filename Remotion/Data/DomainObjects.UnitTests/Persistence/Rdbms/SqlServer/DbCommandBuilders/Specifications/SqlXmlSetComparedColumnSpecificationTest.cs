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
using System.Text;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class SqlXmlSetComparedColumnSpecificationTest
  {
    private ColumnDefinition _columnDefinition;
    private string _objectValue1;
    private int _objectValue2;
    private object _objectValue3;
    private SqlXmlSetComparedColumnSpecification _specification;
    private StringBuilder _statement;
    private Mock<IDataParameterCollection> _parametersCollectionMock;
    private Mock<IDbCommand> _commandStub;
    private Mock<ISqlDialect> _sqlDialectStub;
    private Mock<IDbDataParameter> _parameterStub;

    [SetUp]
    public void SetUp ()
    {
      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn("Column", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation());
      _objectValue1 = "<Test1";
      _objectValue2 = 689;
      _objectValue3 = true;

      _specification = new SqlXmlSetComparedColumnSpecification(_columnDefinition, new[] { _objectValue1, _objectValue2, _objectValue3 });

      _statement = new StringBuilder();

      _parametersCollectionMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);

      _commandStub = new Mock<IDbCommand>();
      _commandStub.Setup(stub => stub.Parameters).Returns(_parametersCollectionMock.Object);

      _parameterStub = new Mock<IDbDataParameter>();
      _parameterStub.SetupAllProperties();
      _commandStub.Setup(stub => stub.CreateParameter()).Returns(_parameterStub.Object);

      _sqlDialectStub = new Mock<ISqlDialect>();
      _sqlDialectStub.Setup(stub => stub.StatementDelimiter).Returns("delimiter");
    }

    [Test]
    public void AddParameters ()
    {
      _sqlDialectStub.Setup(stub => stub.GetParameterName("Column")).Returns("pColumn");

      _parametersCollectionMock.Setup(mock => mock.Add(_parameterStub.Object)).Returns(0).Verifiable();

      _specification.AddParameters(_commandStub.Object, _sqlDialectStub.Object);

      _parametersCollectionMock.Verify();

      Assert.That(_parameterStub.Object.Value, Is.EqualTo("<L><I>&lt;Test1</I><I>689</I><I>True</I></L>"));
      Assert.That(_parameterStub.Object.DbType, Is.EqualTo(DbType.Xml));
      Assert.That(_parameterStub.Object.ParameterName, Is.EqualTo("pColumn"));
    }

    [Test]
    public void AddParameters_NullValue ()
    {
      _specification = new SqlXmlSetComparedColumnSpecification(_columnDefinition, new[] { _objectValue1, null, _objectValue3 });
      Assert.That(
          () => _specification.AddParameters(_commandStub.Object, _sqlDialectStub.Object),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "SQL Server cannot represent NULL values in an XML data type."));
    }

    [Test]
    public void AppendComparisons ()
    {
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Column")).Returns("[delimited Column]");
      _sqlDialectStub.Setup(stub => stub.GetParameterName("Column")).Returns("pColumn");

      _specification.AppendComparisons(_statement, _commandStub.Object, _sqlDialectStub.Object);

      Assert.That(_statement.ToString(), Is.EqualTo("[delimited Column] IN (SELECT T.c.value('.', 'varchar(100)') FROM pColumn.nodes('/L/I') T(c))"));
    }
  }
}
