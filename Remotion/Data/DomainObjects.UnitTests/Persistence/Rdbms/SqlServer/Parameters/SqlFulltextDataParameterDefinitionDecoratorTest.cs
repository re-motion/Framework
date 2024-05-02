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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Parameters;

[TestFixture]
public class SqlFulltextDataParameterDefinitionDecoratorTest
{
  private static IEnumerable<object> GetDummyValues ()
  {
    yield return "Dummy";
    yield return "Dummy".ToCharArray();
  }

  [Test]
  [TestCaseSource(nameof(GetDummyValues))]
  public void MaxSize_AnsiString_SizeSetTo8000 (object value)
  {
    var dbCommandStub = new Mock<IDbCommand>();

    var dataParameterMock = new Mock<IDbDataParameter>();
    dataParameterMock.Setup(_ => _.Value).Returns(value);
    dataParameterMock.Setup(_ => _.DbType).Returns(DbType.AnsiString);
    dataParameterMock.SetupProperty(_ => _.Size);
    dataParameterMock.Object.Size = -1;

    var parameterDefinitionMock = new Mock<IDataParameterDefinition>(MockBehavior.Strict);
    parameterDefinitionMock.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, "dummy", value)).Returns(dataParameterMock.Object);

    var decorator = new SqlFulltextDataParameterDefinitionDecorator(parameterDefinitionMock.Object);

    var result = decorator.CreateDataParameter(dbCommandStub.Object, "dummy", value);

    Assert.That(result, Is.SameAs(dataParameterMock.Object));
    Assert.That(result.Value, Is.EqualTo(value));
    Assert.That(result.DbType, Is.EqualTo(DbType.AnsiString));
    Assert.That(result.Size, Is.EqualTo(8000));
  }

  [Test]
  [TestCaseSource(nameof(GetDummyValues))]
  public void MaxSize_String_SizeSetTo4000 (object value)
  {
    var dbCommandStub = new Mock<IDbCommand>();

    var dataParameterStub = new Mock<IDbDataParameter>();
    dataParameterStub.Setup(_ => _.Value).Returns(value);
    dataParameterStub.Setup(_ => _.DbType).Returns(DbType.String);
    dataParameterStub.SetupProperty(_ => _.ParameterName);
    dataParameterStub.SetupProperty(_ => _.Size);
    dataParameterStub.Object.Size = -1;

    var parameterDefinitionMock = new Mock<IDataParameterDefinition>(MockBehavior.Strict);
    parameterDefinitionMock.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, "dummy", value)).Returns(dataParameterStub.Object);

    var decorator = new SqlFulltextDataParameterDefinitionDecorator(parameterDefinitionMock.Object);

    var result = decorator.CreateDataParameter(dbCommandStub.Object, "dummy", value);

    Assert.That(result, Is.SameAs(dataParameterStub.Object));
    Assert.That(result.Value, Is.EqualTo(value));
    Assert.That(result.DbType, Is.EqualTo(DbType.String));
    Assert.That(result.Size, Is.EqualTo(4000));
  }

  private static IEnumerable<object> GetValuesLength8001 ()
  {
    yield return new string('x', 8001);
    yield return new string('x', 8001).ToCharArray();
  }

  [Test]
  [TestCaseSource(nameof(GetValuesLength8001))]
  public void MaxSize_Over8000LengthAnsiString_SizeUnchanged (object value)
  {
    var dbCommandStub = new Mock<IDbCommand>();

    var dataParameterStub = new Mock<IDbDataParameter>();
    dataParameterStub.Setup(_ => _.Value).Returns(value);
    dataParameterStub.Setup(_ => _.DbType).Returns(DbType.AnsiString);
    dataParameterStub.SetupProperty(_ => _.Size);
    dataParameterStub.Object.Size = -1;

    var parameterDefinitionMock = new Mock<IDataParameterDefinition>(MockBehavior.Strict);
    parameterDefinitionMock.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, "dummy", value)).Returns(dataParameterStub.Object);

    var decorator = new SqlFulltextDataParameterDefinitionDecorator(parameterDefinitionMock.Object);

    var result = decorator.CreateDataParameter(dbCommandStub.Object, "dummy", value);

    Assert.That(result, Is.SameAs(dataParameterStub.Object));
    Assert.That(result.Value, Is.EqualTo(value));
    Assert.That(result.DbType, Is.EqualTo(DbType.AnsiString));
    Assert.That(result.Size, Is.EqualTo(-1));
  }

  private static IEnumerable<object> GetValuesLength4001 ()
  {
    yield return new string('x', 4001);
    yield return new string('x', 4001).ToCharArray();
  }

  [Test]
  [TestCaseSource(nameof(GetValuesLength4001))]
  public void MaxSize_Over4000LengthString_SizeUnchanged (object value)
  {
    var dbCommandStub = new Mock<IDbCommand>();

    var dataParameterStub = new Mock<IDbDataParameter>();
    dataParameterStub.Setup(_ => _.Value).Returns(value);
    dataParameterStub.Setup(_ => _.DbType).Returns(DbType.String);
    dataParameterStub.SetupProperty(_ => _.Size);
    dataParameterStub.Object.Size = -1;

    var parameterDefinitionMock = new Mock<IDataParameterDefinition>(MockBehavior.Strict);
    parameterDefinitionMock.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, "dummy", value)).Returns(dataParameterStub.Object);

    var decorator = new SqlFulltextDataParameterDefinitionDecorator(parameterDefinitionMock.Object);

    var result = decorator.CreateDataParameter(dbCommandStub.Object, "dummy", value);

    Assert.That(result, Is.SameAs(dataParameterStub.Object));
    Assert.That(result.Value, Is.EqualTo(value));
    Assert.That(result.DbType, Is.EqualTo(DbType.String));
    Assert.That(result.Size, Is.EqualTo(-1));
  }

  [Test]
  public void MaxSize_Binary_SizeUnchanged ()
  {
    var dbCommandStub = new Mock<IDbCommand>();

    var dataParameterStub = new Mock<IDbDataParameter>();
    var dummyValue = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
    dataParameterStub.Setup(_ => _.Value).Returns(dummyValue);
    dataParameterStub.Setup(_ => _.DbType).Returns(DbType.Binary);
    dataParameterStub.SetupProperty(_ => _.Size);
    dataParameterStub.Object.Size = -1;

    var parameterDefinitionMock = new Mock<IDataParameterDefinition>(MockBehavior.Strict);
    parameterDefinitionMock.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, "dummy", dummyValue)).Returns(dataParameterStub.Object);

    var decorator = new SqlFulltextDataParameterDefinitionDecorator(parameterDefinitionMock.Object);

    var result = decorator.CreateDataParameter(dbCommandStub.Object, "dummy", dummyValue);

    Assert.That(result, Is.SameAs(dataParameterStub.Object));
    Assert.That(result.Value, Is.EqualTo(dummyValue));
    Assert.That(result.DbType, Is.EqualTo(DbType.Binary));
    Assert.That(result.Size, Is.EqualTo(-1));
  }
}
