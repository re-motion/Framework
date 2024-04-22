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
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class ComparedColumnsSpecificationTest
  {
    private ColumnDefinition _column1;
    private object _value1;
    private Mock<IStorageTypeInformation> _storageTypeInformationMock1;

    private ColumnDefinition _column2;
    private object _value2;
    private Mock<IStorageTypeInformation> _storageTypeInformationMock2;

    private StringBuilder _statement;
    private Mock<IDataParameterCollection> _parametersCollectionMock;
    private Mock<IDbCommand> _commandStub;
    private Mock<ISqlDialect> _sqlDialectStub;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationMock1 = new Mock<IStorageTypeInformation>(MockBehavior.Strict);
      _column1 = new ColumnDefinition("First", _storageTypeInformationMock1.Object, false);
      _value1 = 17;

      _storageTypeInformationMock2 = new Mock<IStorageTypeInformation>(MockBehavior.Strict);
      _column2 = new ColumnDefinition("Second", _storageTypeInformationMock2.Object, false);
      _value2 = 18;

      _statement = new StringBuilder();

      _parametersCollectionMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);
      _commandStub = new Mock<IDbCommand>();
      _commandStub.Setup(_ => _.Parameters).Returns(_parametersCollectionMock.Object);

      _sqlDialectStub = new Mock<ISqlDialect>();
    }

    [Test]
    public void Initialization_Empty ()
    {
      Assert.That(
          () => new ComparedColumnsSpecification(Enumerable.Empty<ColumnValue>()),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
                  "The sequence of compared column values must contain at least one element.", "comparedColumnValues"));
    }

    [Test]
    public void Initialization_NonEmpty ()
    {
      var specification = new ComparedColumnsSpecification(new[] { new ColumnValue(_column1, _value1) });
      Assert.That(specification.ComparedColumnValues, Is.EqualTo(new[] { new ColumnValue(_column1, _value1) }));
    }

    [Test]
    public void AddParameters_OneValue ()
    {
      var specification = new ComparedColumnsSpecification(new[] { new ColumnValue(_column1, _value1) });

      var parameterStub = new Mock<IDbDataParameter>();

      _parametersCollectionMock.Setup(_ => _.Add(parameterStub.Object)).Returns(0).Verifiable();

      _sqlDialectStub.Setup(_ => _.GetParameterName("First")).Returns("pFirst");
      _sqlDialectStub
          .Setup(_ => _.CreateDataParameter(_commandStub.Object, _storageTypeInformationMock1.Object, "pFirst", _value1))
          .Returns(parameterStub.Object);

      specification.AddParameters(_commandStub.Object, _sqlDialectStub.Object);

      _parametersCollectionMock.Verify();
      _storageTypeInformationMock1.Verify();
    }

    [Test]
    public void AddParameters_MultipleValues ()
    {
      var columnValue1 = new ColumnValue(_column1, _value1);
      var columnValue2 = new ColumnValue(_column2, _value2);
      var specification = new ComparedColumnsSpecification(new[] { columnValue1, columnValue2 });

      var parameterStub1 = new Mock<IDbDataParameter>();
      var parameterStub2 = new Mock<IDbDataParameter>();

      _parametersCollectionMock.Setup(_ => _.Add(parameterStub1.Object)).Returns(0).Verifiable();
      _parametersCollectionMock.Setup(_ => _.Add(parameterStub2.Object)).Returns(1).Verifiable();

      _sqlDialectStub.Setup(_ => _.GetParameterName("First")).Returns("pFirst");
      _sqlDialectStub.Setup(_ => _.GetParameterName("Second")).Returns("pSecond");
      _sqlDialectStub
          .Setup(_ => _.CreateDataParameter(_commandStub.Object, _storageTypeInformationMock1.Object, "pFirst", _value1))
          .Returns(parameterStub1.Object);
      _sqlDialectStub
          .Setup(_ => _.CreateDataParameter(_commandStub.Object, _storageTypeInformationMock2.Object, "pSecond", _value2))
          .Returns(parameterStub2.Object);

      specification.AddParameters(_commandStub.Object, _sqlDialectStub.Object);

      _parametersCollectionMock.Verify();
      _storageTypeInformationMock1.Verify();
      _storageTypeInformationMock2.Verify();
    }

    [Test]
    public void AppendComparisons_OneValue ()
    {
      var specification = new ComparedColumnsSpecification(new[] { new ColumnValue(_column1, _value1) });

      _statement.Append("<existingtext>");

      _sqlDialectStub.Setup(_ => _.GetParameterName("First")).Returns("pFirst");
      _sqlDialectStub.Setup(_ => _.DelimitIdentifier("First")).Returns("[First]");

      specification.AppendComparisons(_statement, _commandStub.Object, _sqlDialectStub.Object);

      _parametersCollectionMock.Verify();
      _storageTypeInformationMock1.Verify();

      Assert.That(_statement.ToString(), Is.EqualTo("<existingtext>[First] = pFirst"));
    }

    [Test]
    public void AppendComparisons_MultipleValues_EmptyParameterDictionary ()
    {
      var columnValue1 = new ColumnValue(_column1, _value1);
      var columnValue2 = new ColumnValue(_column2, _value2);
      var specification = new ComparedColumnsSpecification(new[] { columnValue1, columnValue2 });

      _statement.Append("<existingtext>");

      _sqlDialectStub.Setup(_ => _.GetParameterName("First")).Returns("pFirst");
      _sqlDialectStub.Setup(_ => _.GetParameterName("Second")).Returns("pSecond");
      _sqlDialectStub.Setup(_ => _.DelimitIdentifier("First")).Returns("[First]");
      _sqlDialectStub.Setup(_ => _.DelimitIdentifier("Second")).Returns("[Second]");

      specification.AppendComparisons(_statement, _commandStub.Object, _sqlDialectStub.Object);

      Assert.That(_statement.ToString(), Is.EqualTo("<existingtext>[First] = pFirst AND [Second] = pSecond"));
    }
  }
}
