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
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class TracingDataReaderTest
  {
    private Mock<IDataReader> _innerDataReader;
    private Mock<IPersistenceExtension> _extensionMock;
    private Guid _connectionID;
    private Guid _queryID;
    private TracingDataReader _dataReader;

    [SetUp]
    public void SetUp ()
    {
      _innerDataReader = new Mock<IDataReader>(MockBehavior.Strict);
      _extensionMock = new Mock<IPersistenceExtension>(MockBehavior.Strict);
      _connectionID = Guid.NewGuid();
      _queryID = Guid.NewGuid();

      _dataReader = new TracingDataReader(_innerDataReader.Object, _extensionMock.Object, _connectionID, _queryID);
    }

    [Test]
    public void GetName ()
    {
      var i = 5;
      _innerDataReader.Setup(mock => mock.GetName(i)).Returns("test").Verifiable();

      var result = _dataReader.GetName(i);

      Assert.That(result, Is.EqualTo("test"));
      _innerDataReader.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetDataTypeName ()
    {
      var i = 5;
      _innerDataReader.Setup(mock => mock.GetDataTypeName(i)).Returns("test").Verifiable();

      var result = _dataReader.GetDataTypeName(i);

      Assert.That(result, Is.EqualTo("test"));
      _innerDataReader.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetFieldType ()
    {
      var i = 5;
      var expectedType = typeof(string);

      _innerDataReader.Setup(mock => mock.GetFieldType(i)).Returns(expectedType).Verifiable();

      var result = _dataReader.GetFieldType(i);

      Assert.That(result, Is.EqualTo(expectedType));
      _innerDataReader.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetValue ()
    {
      var i = 5;
      var o = "test";
      _innerDataReader.Setup(mock => mock.GetValue(i)).Returns(o).Verifiable();

      var result = _dataReader.GetValue(i);

      Assert.That(result, Is.EqualTo(o));
      _innerDataReader.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetValues ()
    {
      var i = 5;
      object[] values = new object[] { "1", 2, "3" };
      _innerDataReader.Setup(mock => mock.GetValues(values)).Returns(i).Verifiable();

      var result = _dataReader.GetValues(values);

      Assert.That(result, Is.EqualTo(i));
      _innerDataReader.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetOrdinal ()
    {
      var i = 5;
      var name = "test";
      _innerDataReader.Setup(mock => mock.GetOrdinal(name)).Returns(i).Verifiable();

      var result = _dataReader.GetOrdinal(name);

      Assert.That(result, Is.EqualTo(i));
      _innerDataReader.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetBoolean ()
    {
      var i = 5;
      _innerDataReader.Setup(mock => mock.GetBoolean(i)).Returns(true).Verifiable();

      var result = _dataReader.GetBoolean(i);

      Assert.That(result, Is.True);
      _innerDataReader.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetByte ()
    {
      var i = 5;
      var b = new Byte();

      _innerDataReader.Setup(mock => mock.GetByte(i)).Returns(b).Verifiable();

      var result = _dataReader.GetByte(i);

      Assert.That(result, Is.EqualTo(b));
      _innerDataReader.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetBytes ()
    {
      int i = 5;
      long fieldOffset = 10;
      byte[] buffer = new byte[] { new byte(), new byte() };
      int bufferoffset = 1;
      int length = 128;
      long assumedResult = 10;

      _innerDataReader.Setup(mock => mock.GetBytes(i, fieldOffset, buffer, bufferoffset, length)).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
      _innerDataReader.Verify();
      _extensionMock.Verify();

      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void TracingDataReaderWithInt ()
    {
      var i = 5;
      object o = "5";
      _innerDataReader.Setup(mock => mock[i]).Returns(o).Verifiable();

      var result = _dataReader[i];

      _innerDataReader.Verify();
      _extensionMock.Verify();

      Assert.That(result, Is.EqualTo(o));
    }

    [Test]
    public void TracingDataReaderWithString ()
    {
      var i = "test";
      object o = "5";
      _innerDataReader.Setup(mock => mock[i]).Returns(o).Verifiable();

      var result = _dataReader[i];

      _innerDataReader.Verify();
      _extensionMock.Verify();

      Assert.That(result, Is.EqualTo(o));
    }

    [Test]
    public void GetWrappedInstance ()
    {
      var wrappedInstance = _dataReader.WrappedInstance;

      Assert.That(wrappedInstance, Is.EqualTo(_innerDataReader.Object));
    }

    [Test]
    public void GetConnectionId ()
    {
      var connectionID = _dataReader.ConnectionID;

      Assert.That(connectionID, Is.EqualTo(_connectionID));
    }

    [Test]
    public void GetQueryID ()
    {
      var queryID = _dataReader.QueryID;

      Assert.That(queryID, Is.EqualTo(_queryID));
    }

    [Test]
    public void GetPersistenceListener ()
    {
      var persistenceListener = _dataReader.PersistenceExtension;

      Assert.That(persistenceListener, Is.EqualTo(_extensionMock.Object));
    }

    [Test]
    public void GetChar ()
    {
      var i = 5;
      var c = 't';
      _innerDataReader.Setup(mock => mock.GetChar(i)).Returns(c).Verifiable();

      var result = _dataReader.GetChar(i);

      Assert.That(result, Is.EqualTo(c));
      _innerDataReader.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetChars ()
    {
      int assumedResult = 2000;
      int i = 5;
      long fieldoffset = 5;
      char[] buffer = new char[] { 'a', 'b' };
      int bufferoffset = 10;
      int length = 128;

      _innerDataReader.Setup(mock => mock.GetChars(i, fieldoffset, buffer, bufferoffset, length)).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetGuid ()
    {
      var assumedGuid = Guid.NewGuid();
      int i = 5;

      _innerDataReader.Setup(mock => mock.GetGuid(i)).Returns(assumedGuid).Verifiable();

      var result = _dataReader.GetGuid(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedGuid));
    }

    [Test]
    public void GetInt16 ()
    {
      int i = 5;
      short assumedResult = 1;

      _innerDataReader.Setup(mock => mock.GetInt16(i)).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetInt16(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetInt32 ()
    {
      int i = 5;
      int assumedResult = 1;

      _innerDataReader.Setup(mock => mock.GetInt32(i)).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetInt32(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetInt64 ()
    {
      int i = 5;
      long assumedResult = 1;

      _innerDataReader.Setup(mock => mock.GetInt64(i)).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetInt64(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetFloat ()
    {
      int i = 5;
      float assumedResult = 1;

      _innerDataReader.Setup(mock => mock.GetFloat(i)).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetFloat(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetDouble ()
    {
      int i = 5;
      double assumedResult = 1.0;

      _innerDataReader.Setup(mock => mock.GetDouble(i)).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetDouble(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetString ()
    {
      int i = 5;
      string assumedResult = "test";

      _innerDataReader.Setup(mock => mock.GetString(i)).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetString(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetDecimal ()
    {
      int i = 5;
      decimal assumedResult = 1;

      _innerDataReader.Setup(mock => mock.GetDecimal(i)).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetDecimal(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetDateTime ()
    {
      int i = 5;
      DateTime assumedResult = new DateTime();

      _innerDataReader.Setup(mock => mock.GetDateTime(i)).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetDateTime(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetData ()
    {
      var dataReaderMock = new Mock<IDataReader>(MockBehavior.Strict);
      int i = 5;
      _innerDataReader.Setup(mock => mock.GetData(i)).Returns(dataReaderMock.Object).Verifiable();

      var result = _dataReader.GetData(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      dataReaderMock.Verify();
      Assert.That(result, Is.EqualTo(dataReaderMock.Object));
    }

    [Test]
    public void IsDBNull ()
    {
      var assumedResult = true;
      int i = 5;
      _innerDataReader.Setup(mock => mock.IsDBNull(i)).Returns(assumedResult).Verifiable();

      var result = _dataReader.IsDBNull(i);

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void FieldCount ()
    {
      int assumedResult = 5;
      _innerDataReader.Setup(mock => mock.FieldCount).Returns(assumedResult).Verifiable();

      var result = _dataReader.FieldCount;

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void NextResult ()
    {
      var assumedResult = true;
      _innerDataReader.Setup(mock => mock.NextResult()).Returns(assumedResult).Verifiable();

      var result = _dataReader.NextResult();

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetSchemaTable ()
    {
      DataTable assumedResult = new DataTable();
      _innerDataReader.Setup(mock => mock.GetSchemaTable()).Returns(assumedResult).Verifiable();

      var result = _dataReader.GetSchemaTable();

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void Depth ()
    {
      int assumedResult = 5;
      _innerDataReader.Setup(mock => mock.Depth).Returns(assumedResult).Verifiable();

      var result = _dataReader.Depth;

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void IsClosed ()
    {
      var assumedResult = true;
      _innerDataReader.Setup(mock => mock.IsClosed).Returns(assumedResult).Verifiable();

      var result = _dataReader.IsClosed;

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void RecordsAffected ()
    {
      int assumedResult = 5;
      _innerDataReader.Setup(mock => mock.RecordsAffected).Returns(assumedResult).Verifiable();

      var result = _dataReader.RecordsAffected;

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void Dispose ()
    {
      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryCompleted(_connectionID, _queryID, It.Is<TimeSpan>(p => p > TimeSpan.Zero), 0))
          .Verifiable();
      _innerDataReader.InVerifiableSequence(sequence).Setup(mock => mock.Dispose()).Verifiable();

      _dataReader.Dispose();
      _innerDataReader.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Close ()
    {
      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryCompleted(_connectionID, _queryID, It.Is<TimeSpan>(p => p > TimeSpan.Zero), 0))
          .Verifiable();
      _innerDataReader.InVerifiableSequence(sequence).Setup(mock => mock.Close()).Verifiable();

      _dataReader.Close();
      _innerDataReader.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CloseAndDispose ()
    {
      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryCompleted(_connectionID,  _queryID, It.Is<TimeSpan>(p => p > TimeSpan.Zero), 0))
          .Verifiable();
      _innerDataReader.InVerifiableSequence(sequence).Setup(mock => mock.Close()).Verifiable();
      _innerDataReader.InVerifiableSequence(sequence).Setup(mock => mock.Dispose()).Verifiable();

      _dataReader.Close();
      _dataReader.Dispose();

      _innerDataReader.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Read_HasRecord ()
    {
      _innerDataReader.Setup(mock => mock.Read()).Returns(true).Verifiable();

      var hasRecord = _dataReader.Read();

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(hasRecord, Is.True);
    }

    [Test]
    public void Read_NoRecord ()
    {
      _innerDataReader.Setup(mock => mock.Read()).Returns(false).Verifiable();

      var hasRecord = _dataReader.Read();

      _innerDataReader.Verify();
      _extensionMock.Verify();
      Assert.That(hasRecord, Is.False);
    }

    [Test]
    public void ReadAndClose ()
    {
      var sequence = new VerifiableSequence();
      _innerDataReader.InVerifiableSequence(sequence).Setup(mock => mock.Read()).Returns(true).Verifiable();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryCompleted(_connectionID, _queryID, It.Is<TimeSpan>(p => p > TimeSpan.Zero), 1))
          .Verifiable();
      _innerDataReader.InVerifiableSequence(sequence).Setup(mock => mock.Close()).Verifiable();

      _dataReader.Read();
      _dataReader.Close();

      _innerDataReader.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

  }
}
