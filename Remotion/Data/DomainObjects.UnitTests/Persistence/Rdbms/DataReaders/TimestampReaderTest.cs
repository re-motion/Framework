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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class TimestampReaderTest : StandardMappingTest
  {
    private IDataReader _dataReaderStrictMock;
    private IRdbmsStoragePropertyDefinition _idPropertyStrictMock;
    private IRdbmsStoragePropertyDefinition _timestampStrictMock;

    private IColumnOrdinalProvider _columnOrdinalProviderStub;

    private TimestampReader _reader;
    
    private ObjectID _fakeObjectIDResult;
    private object _fakeTimestampResult;

    public override void SetUp ()
    {
      base.SetUp();

      _dataReaderStrictMock = MockRepository.GenerateStrictMock<IDataReader> ();
      _idPropertyStrictMock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition>();
      _timestampStrictMock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition>();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();

      _reader = new TimestampReader (_idPropertyStrictMock, _timestampStrictMock, _columnOrdinalProviderStub);

      _fakeObjectIDResult = new ObjectID(typeof (Order), Guid.NewGuid ());
      _fakeTimestampResult = new object();
    }

    [Test]
    public void Read_DataReaderReadFalse ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (false);
      ReplayAll();

      var result = _reader.Read (_dataReaderStrictMock);

      VerifyAll();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue_NullValue ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (true).Repeat.Once ();
      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (null)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]));
      ReplayAll();

      var result = _reader.Read (_dataReaderStrictMock);

      VerifyAll();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue_NonNullValue ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (true).Repeat.Once ();
      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (_fakeObjectIDResult)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]));
      _timestampStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (_fakeTimestampResult)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]));
      ReplayAll();
      
      var result = _reader.Read (_dataReaderStrictMock);

      VerifyAll();
      Assert.That (result, Is.EqualTo (Tuple.Create(_fakeObjectIDResult, _fakeTimestampResult)));
    }

    [Test]
    public void ReadSequence ()
    {
      var fakeObjectIDResult2 = new ObjectID(typeof (OrderItem), Guid.NewGuid ());
      var fakeTimestampResult2 = new object ();

      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (true).Repeat.Times (3);
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (false).Repeat.Once ();

      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (_fakeObjectIDResult)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Repeat.Once ();
      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (null)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Repeat.Once ();
      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (fakeObjectIDResult2)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Repeat.Once ();
      _timestampStrictMock
        .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
        .Return (_fakeTimestampResult)
        .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
        .Repeat.Once();
      _timestampStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
        .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Return (fakeTimestampResult2)
          .Repeat.Once();
      ReplayAll();
      
      var result = _reader.ReadSequence (_dataReaderStrictMock).ToArray ();

      VerifyAll();
      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0], Is.EqualTo (Tuple.Create (_fakeObjectIDResult, _fakeTimestampResult)));
      Assert.That (result[1], Is.Null);
      Assert.That (result[2], Is.EqualTo(Tuple.Create (fakeObjectIDResult2, fakeTimestampResult2)));
    }

    [Test]
    public void ReadSequence_NoData ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (false).Repeat.Once ();
      ReplayAll();

      var result = _reader.ReadSequence (_dataReaderStrictMock).ToArray();

      VerifyAll();
      Assert.That (result, Is.Empty);
    }

    private void ReplayAll()
    {
      _dataReaderStrictMock.Replay();
      _idPropertyStrictMock.Replay();
      _timestampStrictMock.Replay();
    }

    private void VerifyAll()
    {
      _dataReaderStrictMock.VerifyAllExpectations();
      _idPropertyStrictMock.VerifyAllExpectations();
      _timestampStrictMock.VerifyAllExpectations();
    }

    private void CheckColumnValueReader (ColumnValueReader columnValueReader)
    {
      Assert.That (columnValueReader.DataReader, Is.SameAs (_dataReaderStrictMock));
      Assert.That (columnValueReader.ColumnOrdinalProvider, Is.SameAs (_columnOrdinalProviderStub));
    }
 
  }
}