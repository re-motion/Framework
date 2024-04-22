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
using System.ComponentModel;
using System.Data;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer
{
  [TestFixture]
  public class SqlDialectTest
  {
    private SqlDialect _dialect;
    private Mock<TypeConverter> _typeConverterStub;

    [SetUp]
    public void SetUp ()
    {
      _dialect = new SqlDialect();
      _typeConverterStub = new Mock<TypeConverter>();
    }

    [Test]
    public void StatementDelimiter ()
    {
      Assert.That(_dialect.StatementDelimiter, Is.EqualTo(";"));
    }

    [Test]
    public void DelimitIdentifier ()
    {
      Assert.That(_dialect.DelimitIdentifier("x"), Is.EqualTo("[x]"));
    }

    [Test]
    public void GetParameterName ()
    {
      Assert.That(_dialect.GetParameterName("parameter"), Is.EqualTo("@parameter"));
      Assert.That(_dialect.GetParameterName("@parameter"), Is.EqualTo("@parameter"));
    }

    [Test]
    [CLSCompliant(false)]
    [TestCase(null, TestName = "CreateDataParameter_WithoutSize_DoesNotSetSizeOnParameter.")]
    [TestCase(-1, TestName = "CreateDataParameter_WithNegativeSize_SetsSizeOnParameter.")]
    [TestCase(0, TestName = "CreateDataParameter_WithSizeZero_SetsSizeOnParameter.")]
    [TestCase(1, TestName = "CreateDataParameter_WithPositiveSize_SetsSizeOnParameter.")]
    public void CreateDataParameter (int? storageTypeSize)
    {
      var commandStub = new Mock<IDbCommand>();
      var dataParameterMock = new Mock<IDbDataParameter>();

      var storageTypeInformation = new StorageTypeInformation(
          typeof(bool),
          "test",
          DbType.Boolean,
          false,
          storageTypeSize,
          typeof(int),
          _typeConverterStub.Object);

      _typeConverterStub.Setup(_ => _.ConvertTo(null, null, "value", storageTypeInformation.StorageType)).Returns("");

      commandStub.Setup(_ => _.CreateParameter()).Returns(dataParameterMock.Object);

      dataParameterMock.SetupSet(_ => _.ParameterName = "@dummy").Verifiable();
      dataParameterMock.SetupSet(_ => _.DbType = storageTypeInformation.StorageDbType).Verifiable();
      dataParameterMock.SetupSet(_ => _.Value = "").Verifiable();
      if (storageTypeSize.HasValue)
        dataParameterMock.SetupSet(_ => _.Size = storageTypeSize.Value).Verifiable();
      else
        dataParameterMock.SetupSet(_ => _.Size = It.IsAny<int>()).Throws(new AssertionException("Must not be called."));

      var result = _dialect.CreateDataParameter(commandStub.Object, storageTypeInformation, "@dummy", "value");

      dataParameterMock.Verify();

      Assert.That(result, Is.SameAs(dataParameterMock.Object));
    }

    [Test]
    public void CreateDataParameter_WithStringValueExceedingFixedSize_DoesNotInitializeSize ()
    {
      var commandStub = new Mock<IDbCommand>();
      var dataParameterMock = new Mock<IDbDataParameter>();

      var storageTypeInformation = new StorageTypeInformation(
          typeof(string),
          "test",
          DbType.String,
          false,
          5,
          typeof(string),
          _typeConverterStub.Object);

      _typeConverterStub.Setup(_ => _.ConvertTo(null, null, "value", storageTypeInformation.StorageType)).Returns("converted value");

      commandStub.Setup(_ => _.CreateParameter()).Returns(dataParameterMock.Object);

      dataParameterMock.SetupSet(_ => _.ParameterName = "@dummy").Verifiable();
      dataParameterMock.SetupSet(_ => _.DbType = storageTypeInformation.StorageDbType).Verifiable();
      dataParameterMock.SetupSet(_ => _.Value = "converted value").Verifiable();
      dataParameterMock.SetupSet(_ => _.Size = It.IsAny<int>()).Throws(new AssertionException("Must not be called."));

      var result = _dialect.CreateDataParameter(commandStub.Object, storageTypeInformation, "@dummy", "value");

      dataParameterMock.Verify();

      Assert.That(result, Is.SameAs(dataParameterMock.Object));
    }

    [Test]
    public void CreateDataParameter_WithCharArrayValueExceedingFixedSize_DoesNotInitializeSize ()
    {
      var commandStub = new Mock<IDbCommand>();
      var dataParameterMock = new Mock<IDbDataParameter>();

      var storageTypeInformation = new StorageTypeInformation(
          typeof(char[]),
          "test",
          DbType.AnsiString,
          false,
          5,
          typeof(char[]),
          _typeConverterStub.Object);

      var convertedValue = new char[10];
      _typeConverterStub.Setup(_ => _.ConvertTo(null, null, "value", storageTypeInformation.StorageType)).Returns(convertedValue);

      commandStub.Setup(_ => _.CreateParameter()).Returns(dataParameterMock.Object);

      dataParameterMock.SetupSet(_ => _.ParameterName = "@dummy").Verifiable();
      dataParameterMock.SetupSet(_ => _.DbType = storageTypeInformation.StorageDbType).Verifiable();
      dataParameterMock.SetupSet(_ => _.Value = convertedValue).Verifiable();
      dataParameterMock.SetupSet(_ => _.Size = It.IsAny<int>()).Throws(new AssertionException("Must not be called."));

      var result = _dialect.CreateDataParameter(commandStub.Object, storageTypeInformation, "@dummy","value");

      dataParameterMock.Verify();

      Assert.That(result, Is.SameAs(dataParameterMock.Object));
    }

    [Test]
    public void CreateDataParameter_WithByteArrayValueExceedingFixedSize_DoesNotInitializeSize ()
    {
      var commandStub = new Mock<IDbCommand>();
      var dataParameterMock = new Mock<IDbDataParameter>();

      var storageTypeInformation = new StorageTypeInformation(
          typeof(byte[]),
          "test",
          DbType.Binary,
          false,
          5,
          typeof(byte[]),
          _typeConverterStub.Object);

      var convertedValue = new byte[10];
      _typeConverterStub.Setup(_ => _.ConvertTo(null, null, "value", storageTypeInformation.StorageType)).Returns(convertedValue);

      commandStub.Setup(_ => _.CreateParameter()).Returns(dataParameterMock.Object);

      dataParameterMock.SetupSet(_ => _.ParameterName = "@dummy").Verifiable();
      dataParameterMock.SetupSet(_ => _.DbType = storageTypeInformation.StorageDbType).Verifiable();
      dataParameterMock.SetupSet(_ => _.Value = convertedValue).Verifiable();
      dataParameterMock.SetupSet(_ => _.Size = It.IsAny<int>()).Throws(new AssertionException("Must not be called."));

      var result = _dialect.CreateDataParameter(commandStub.Object, storageTypeInformation, "@dummy","value");

      dataParameterMock.Verify();

      Assert.That(result, Is.SameAs(dataParameterMock.Object));
    }

    [Test]
    public void CreateDataParameter_WithNullResult ()
    {
      var commandStub = new Mock<IDbCommand>();
      var dataParameterMock = new Mock<IDbDataParameter>();

      var storageTypeInformation = new StorageTypeInformation(typeof(bool), "test", DbType.Boolean, false, null, typeof(int), _typeConverterStub.Object);
      _typeConverterStub.Setup(_ => _.ConvertTo(null, null, "value", storageTypeInformation.StorageType)).Returns((object)null);

      commandStub.Setup(_ => _.CreateParameter()).Returns(dataParameterMock.Object);

      dataParameterMock.SetupSet(_ => _.ParameterName = "@dummy").Verifiable();
      dataParameterMock.SetupSet(_ => _.DbType = storageTypeInformation.StorageDbType).Verifiable();
      dataParameterMock.SetupSet(_ => _.Value = DBNull.Value).Verifiable();
      dataParameterMock.SetupSet(_ => _.Size = It.IsAny<int>()).Throws(new AssertionException("Must not be called."));

      var result = _dialect.CreateDataParameter(commandStub.Object, storageTypeInformation, "@dummy","value");

      dataParameterMock.Verify();

      Assert.That(result, Is.SameAs(dataParameterMock.Object));
    }

    [Test]
    public void CreateDataParameter_WithNullResult_AndParameterSize_SetsSize ()
    {
      var commandStub = new Mock<IDbCommand>();
      var dataParameterMock = new Mock<IDbDataParameter>();

      var storageTypeInformation = new StorageTypeInformation(
          typeof(byte[]),
          "test",
          DbType.Binary,
          false,
          5,
          typeof(byte[]),
          _typeConverterStub.Object);

      _typeConverterStub.Setup(_ => _.ConvertTo(null, null, "value", storageTypeInformation.StorageType)).Returns((object)null);

      commandStub.Setup(_ => _.CreateParameter()).Returns(dataParameterMock.Object);

      dataParameterMock.SetupSet(_ => _.ParameterName = "@dummy").Verifiable();
      dataParameterMock.SetupSet(_ => _.DbType = storageTypeInformation.StorageDbType).Verifiable();
      dataParameterMock.SetupSet(_ => _.Value = DBNull.Value).Verifiable();
      dataParameterMock.SetupSet(_ => _.Size = 5).Verifiable();

      var result = _dialect.CreateDataParameter(commandStub.Object, storageTypeInformation, "@dummy","value");

      dataParameterMock.Verify();

      Assert.That(result, Is.SameAs(dataParameterMock.Object));
    }

    [Test]
    public void CreateDataParameter_WithNullInput_NoSize ()
    {
      var commandStub = new Mock<IDbCommand>();
      var dataParameterMock = new Mock<IDbDataParameter>();

      var storageTypeInformation = new StorageTypeInformation(typeof(bool), "test", DbType.Boolean, false, null, typeof(int), _typeConverterStub.Object);
      _typeConverterStub.Setup(_ => _.ConvertTo(null, null, null, storageTypeInformation.StorageType)).Returns(DBNull.Value);

      commandStub.Setup(_ => _.CreateParameter()).Returns(dataParameterMock.Object);

      dataParameterMock.SetupSet(_ => _.ParameterName = "@dummy").Verifiable();
      dataParameterMock.SetupSet(_ => _.DbType = storageTypeInformation.StorageDbType).Verifiable();
      dataParameterMock.SetupSet(_ => _.Value = DBNull.Value).Verifiable();
      dataParameterMock.SetupSet(_ => _.Size = It.IsAny<int>()).Throws(new AssertionException("Must not be called."));

      var result = _dialect.CreateDataParameter(commandStub.Object, storageTypeInformation, "@dummy", null);

      dataParameterMock.Verify();

      Assert.That(result, Is.SameAs(dataParameterMock.Object));
    }

    [Test]
    public void CreateDataParameter_WithNullInput_WithSize ()
    {
      var commandStub = new Mock<IDbCommand>();
      var dataParameterMock = new Mock<IDbDataParameter>();

      var storageTypeInformation = new StorageTypeInformation(typeof(string), "test", DbType.String, false, 5, typeof(string), _typeConverterStub.Object);
      _typeConverterStub.Setup(_ => _.ConvertTo(null, null, null, storageTypeInformation.StorageType)).Returns(DBNull.Value);

      commandStub.Setup(_ => _.CreateParameter()).Returns(dataParameterMock.Object);

      dataParameterMock.SetupSet(_ => _.ParameterName = "@dummy").Verifiable();
      dataParameterMock.SetupSet(_ => _.DbType = storageTypeInformation.StorageDbType).Verifiable();
      dataParameterMock.SetupSet(_ => _.Value = DBNull.Value).Verifiable();
      dataParameterMock.SetupSet(_ => _.Size = 5).Verifiable();

      var result = _dialect.CreateDataParameter(commandStub.Object, storageTypeInformation, "@dummy", null);

      dataParameterMock.Verify();

      Assert.That(result, Is.SameAs(dataParameterMock.Object));
    }
  }
}
