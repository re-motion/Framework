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
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class ObjectIDReaderTest : StandardMappingTest
  {
    private IDataReader _dataReaderStrictMock;
    private IRdbmsStoragePropertyDefinition _idPropertyStrictMock;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;

    private ObjectIDReader _reader;

    private ObjectID _objectID;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataReaderStrictMock = MockRepository.GenerateStrictMock<IDataReader>();
      _idPropertyStrictMock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition> ();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();

      _reader = new ObjectIDReader (_idPropertyStrictMock, _columnOrdinalProviderStub);

      _objectID = new ObjectID("Order", Guid.NewGuid());
    }

    [Test]
    public void Read_DataReaderReadFalse ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read()).Return (false);
      ReplayAll();

      var result = _reader.Read (_dataReaderStrictMock);

      VerifyAll();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (true).Repeat.Once ();
      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (_objectID)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]));
      ReplayAll();

      var result = _reader.Read (_dataReaderStrictMock);

      VerifyAll();
      Assert.That (result, Is.SameAs (_objectID));
    }

    [Test]
    public void Read_DataReaderReadTrue_Null ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (true).Repeat.Once ();
      _idPropertyStrictMock.Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything)).Return (null);
      ReplayAll();

      var result = _reader.Read (_dataReaderStrictMock);

      VerifyAll();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void ReadSequence ()
    {
      var objectID2 = new ObjectID("OrderItem", Guid.NewGuid ());
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (true).Repeat.Times (3);
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (false).Repeat.Once ();
      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (_objectID)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Repeat.Once ();
      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (null)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Repeat.Once ();
      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (objectID2)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Repeat.Once ();
      ReplayAll();

      var result = _reader.ReadSequence (_dataReaderStrictMock).ToArray ();

      VerifyAll();
      Assert.That (result.Length, Is.EqualTo (3));
      Assert.That (result[0], Is.SameAs (_objectID));
      Assert.That (result[1], Is.Null);
      Assert.That (result[2], Is.SameAs (objectID2));
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
    }

    private void VerifyAll()
    {
      _dataReaderStrictMock.VerifyAllExpectations();
      _idPropertyStrictMock.VerifyAllExpectations();
    }

    private void CheckColumnValueReader (ColumnValueReader columnValueReader)
    {
      Assert.That (columnValueReader.DataReader, Is.SameAs (_dataReaderStrictMock));
      Assert.That (columnValueReader.ColumnOrdinalProvider, Is.SameAs (_columnOrdinalProviderStub));
    }
  }
}