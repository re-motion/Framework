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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class TracingDataReaderTest
  {
    private MockRepository _mockRepository;
    private IDataReader _innerDataReader;
    private IPersistenceExtension _extensionMock;
    private Guid _connectionID;
    private Guid _queryID;
    private TracingDataReader _dataReader;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _innerDataReader = _mockRepository.StrictMock<IDataReader>();
      _extensionMock = _mockRepository.StrictMock<IPersistenceExtension>();
      _connectionID = Guid.NewGuid();
      _queryID = Guid.NewGuid();

      _dataReader = new TracingDataReader (_innerDataReader, _extensionMock, _connectionID, _queryID);
    }

    [Test]
    public void GetName ()
    {
      var i = 5;
      _innerDataReader.Expect (mock => mock.GetName (i)).Return ("test");
      _mockRepository.ReplayAll();

      var result = _dataReader.GetName (i);

      Assert.That (result, Is.EqualTo ("test"));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetDataTypeName ()
    {
      var i = 5;
      _innerDataReader.Expect (mock => mock.GetDataTypeName (i)).Return ("test");
      _mockRepository.ReplayAll();

      var result = _dataReader.GetDataTypeName (i);

      Assert.That (result, Is.EqualTo ("test"));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetFieldType ()
    {
      var i = 5;
      var expectedType = typeof (string);

      _innerDataReader.Expect (mock => mock.GetFieldType (i)).Return (expectedType);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetFieldType (i);

      Assert.That (result, Is.EqualTo (expectedType));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetValue ()
    {
      var i = 5;
      var o = "test";
      _innerDataReader.Expect (mock => mock.GetValue (i)).Return (o);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetValue (i);

      Assert.That (result, Is.EqualTo (o));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetValues ()
    {
      var i = 5;
      object[] values = new object[] { "1", 2, "3" };
      _innerDataReader.Expect (mock => mock.GetValues (values)).Return (i);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetValues (values);

      Assert.That (result, Is.EqualTo (i));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOrdinal ()
    {
      var i = 5;
      var name = "test";
      _innerDataReader.Expect (mock => mock.GetOrdinal (name)).Return (i);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetOrdinal (name);

      Assert.That (result, Is.EqualTo (i));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetBoolean ()
    {
      var i = 5;
      _innerDataReader.Expect (mock => mock.GetBoolean (i)).Return (true);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetBoolean (i);

      Assert.That (result, Is.True);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetByte ()
    {
      var i = 5;
      var b = new Byte();

      _innerDataReader.Expect (mock => mock.GetByte (i)).Return (b);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetByte (i);

      Assert.That (result, Is.EqualTo (b));
      _mockRepository.VerifyAll();
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

      _innerDataReader.Expect (mock => mock.GetBytes (i, fieldOffset, buffer, bufferoffset, length)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetBytes (i, fieldOffset, buffer, bufferoffset, length);
      _mockRepository.VerifyAll();

      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void TracingDataReaderWithInt ()
    {
      var i = 5;
      object o = "5";
      _innerDataReader.Expect (mock => mock[i]).Return (o);
      _mockRepository.ReplayAll();

      var result = _dataReader[i];

      _mockRepository.VerifyAll();

      Assert.That (result, Is.EqualTo (o));
    }

    [Test]
    public void TracingDataReaderWithString ()
    {
      var i = "test";
      object o = "5";
      _innerDataReader.Expect (mock => mock[i]).Return (o);
      _mockRepository.ReplayAll();

      var result = _dataReader[i];

      _mockRepository.VerifyAll();

      Assert.That (result, Is.EqualTo (o));
    }

    [Test]
    public void GetWrappedInstance ()
    {
      var wrappedInstance = _dataReader.WrappedInstance;

      Assert.That (wrappedInstance, Is.EqualTo (_innerDataReader));
    }

    [Test]
    public void GetConnectionId ()
    {
      var connectionID = _dataReader.ConnectionID;

      Assert.That (connectionID, Is.EqualTo (_connectionID));
    }

    [Test]
    public void GetQueryID ()
    {
      var queryID = _dataReader.QueryID;

      Assert.That (queryID, Is.EqualTo (_queryID));
    }

    [Test]
    public void GetPersistenceListener ()
    {
      var persistenceListener = _dataReader.PersistenceExtension;

      Assert.That (persistenceListener, Is.EqualTo (_extensionMock));
    }

    [Test]
    public void GetChar ()
    {
      var i = 5;
      var c = 't';
      _innerDataReader.Expect (mock => mock.GetChar (i)).Return (c);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetChar (i);

      Assert.That (result, Is.EqualTo (c));
      _mockRepository.VerifyAll();
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


      _innerDataReader.Expect (mock => mock.GetChars (i, fieldoffset, buffer, bufferoffset, length)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetChars (i, fieldoffset, buffer, bufferoffset, length);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetGuid ()
    {
      var assumedGuid = Guid.NewGuid();
      int i = 5;

      _innerDataReader.Expect (mock => mock.GetGuid (i)).Return (assumedGuid);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetGuid (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedGuid));
    }

    [Test]
    public void GetInt16 ()
    {
      int i = 5;
      short assumedResult = 1;

      _innerDataReader.Expect (mock => mock.GetInt16 (i)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetInt16 (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetInt32 ()
    {
      int i = 5;
      int assumedResult = 1;

      _innerDataReader.Expect (mock => mock.GetInt32 (i)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetInt32 (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetInt64 ()
    {
      int i = 5;
      long assumedResult = 1;

      _innerDataReader.Expect (mock => mock.GetInt64 (i)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetInt64 (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetFloat ()
    {
      int i = 5;
      float assumedResult = 1;

      _innerDataReader.Expect (mock => mock.GetFloat (i)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetFloat (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetDouble ()
    {
      int i = 5;
      double assumedResult = 1.0;

      _innerDataReader.Expect (mock => mock.GetDouble (i)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetDouble (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetString ()
    {
      int i = 5;
      string assumedResult = "test";

      _innerDataReader.Expect (mock => mock.GetString (i)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetString (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetDecimal ()
    {
      int i = 5;
      decimal assumedResult = 1;

      _innerDataReader.Expect (mock => mock.GetDecimal (i)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetDecimal (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetDateTime ()
    {
      int i = 5;
      DateTime assumedResult = new DateTime();

      _innerDataReader.Expect (mock => mock.GetDateTime (i)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetDateTime (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetData ()
    {
      var dataReaderMock = _mockRepository.StrictMock<IDataReader>();
      int i = 5;
      _innerDataReader.Expect (mock => mock.GetData (i)).Return (dataReaderMock);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetData (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (dataReaderMock));
    }

    [Test]
    public void IsDBNull ()
    {
      var assumedResult = true;
      int i = 5;
      _innerDataReader.Expect (mock => mock.IsDBNull (i)).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.IsDBNull (i);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void FieldCount ()
    {
      int assumedResult = 5;
      _innerDataReader.Expect (mock => mock.FieldCount).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.FieldCount;

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void NextResult ()
    {
      var assumedResult = true;
      _innerDataReader.Expect (mock => mock.NextResult()).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.NextResult();

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetSchemaTable ()
    {
      DataTable assumedResult = new DataTable();
      _innerDataReader.Expect (mock => mock.GetSchemaTable()).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.GetSchemaTable();

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void Depth ()
    {
      int assumedResult = 5;
      _innerDataReader.Expect (mock => mock.Depth).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.Depth;

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void IsClosed ()
    {
      var assumedResult = true;
      _innerDataReader.Expect (mock => mock.IsClosed).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.IsClosed;

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void RecordsAffected ()
    {
      int assumedResult = 5;
      _innerDataReader.Expect (mock => mock.RecordsAffected).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _dataReader.RecordsAffected;

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void Dispose ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.QueryCompleted (
                Arg<Guid>.Matches (p => p == _connectionID),
                Arg<Guid>.Matches (p => p == _queryID),
                Arg<TimeSpan>.Matches (p => p.Milliseconds > 0),
                Arg<int>.Matches (p => p == 0)));
        _innerDataReader.Expect (mock => mock.Dispose());
      }
      _mockRepository.ReplayAll();

      _dataReader.Dispose();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Close ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.QueryCompleted (
                Arg<Guid>.Matches (p => p == _connectionID),
                Arg<Guid>.Matches (p => p == _queryID),
                Arg<TimeSpan>.Matches (p => p.Milliseconds > 0),
                Arg<int>.Matches (p => p == 0)));
        _innerDataReader.Expect (mock => mock.Close());
      }
      _mockRepository.ReplayAll();

      _dataReader.Close();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void CloseAndDispose ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock =>
            mock.QueryCompleted (
                Arg<Guid>.Matches (p => p == _connectionID),
                Arg<Guid>.Matches (p => p == _queryID),
                Arg<TimeSpan>.Matches (p => p.Milliseconds > 0),
                Arg<int>.Matches (p => p == 0)));
        _innerDataReader.Expect (mock => mock.Close());
        _innerDataReader.Expect (mock => mock.Dispose());
      }
      _mockRepository.ReplayAll();

      _dataReader.Close();
      _dataReader.Dispose();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Read_HasRecord ()
    {
      _innerDataReader.Expect (mock => mock.Read()).Return (true);
      _mockRepository.ReplayAll();

      var hasRecord = _dataReader.Read();

      _mockRepository.VerifyAll();
      Assert.That (hasRecord, Is.True);
    }

    [Test]
    public void Read_NoRecord ()
    {
      _innerDataReader.Expect (mock => mock.Read()).Return (false);
      _mockRepository.ReplayAll();

      var hasRecord = _dataReader.Read();

      _mockRepository.VerifyAll();
      Assert.That (hasRecord, Is.False);
    }
    [Test]
    public void ReadAndClose ()
    {
      _innerDataReader.Expect (mock => mock.Read ()).Return (true);
      _extensionMock.Expect (
            mock =>
            mock.QueryCompleted (
                Arg<Guid>.Is.Equal (_connectionID),
                Arg<Guid>.Matches (p => p == _queryID),
                Arg<TimeSpan>.Matches (p => p.Milliseconds > 0),
                Arg<int>.Matches (p => p == 1)));
      _innerDataReader.Expect (mock => mock.Close ());
      _mockRepository.ReplayAll ();
      
      _dataReader.Read();
      _dataReader.Close();

      _mockRepository.VerifyAll ();
    }

  }
}