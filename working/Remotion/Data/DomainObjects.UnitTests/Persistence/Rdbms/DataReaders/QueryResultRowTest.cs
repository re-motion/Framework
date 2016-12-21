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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class QueryResultRowTest
  {
    private IDataReader _dataReaderStub;
    private IStorageTypeInformationProvider _storageTypeInformationProviderStub;
    private QueryResultRow _queryResultRow;
    private IStorageTypeInformation _storageTypeInformationMock;

    [SetUp]
    public void SetUp ()
    {
      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _storageTypeInformationProviderStub = MockRepository.GenerateStub<IStorageTypeInformationProvider>();
      _queryResultRow = new QueryResultRow (_dataReaderStub, _storageTypeInformationProviderStub);

      _storageTypeInformationMock = MockRepository.GenerateStrictMock<IStorageTypeInformation>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_queryResultRow.DataReader, Is.SameAs (_dataReaderStub));
      Assert.That (_queryResultRow.StorageTypeInformationProvider, Is.SameAs (_storageTypeInformationProviderStub));
    }

    [Test]
    public void ValueCount ()
    {
      int fakeFieldCount = 10;
      _dataReaderStub.Stub (stub => stub.FieldCount).Return (fakeFieldCount);

      var result = _queryResultRow.ValueCount;

      Assert.That (result, Is.EqualTo (fakeFieldCount));
    }

    [Test]
    public void GetRawValue ()
    {
      var value1 = "hallo";
      var value2 = true;
      var value3 = 45;

      _dataReaderStub.Stub (stub => stub.GetValue (0)).Return (value1);
      _dataReaderStub.Stub (stub => stub.GetValue (1)).Return (value2);
      _dataReaderStub.Stub (stub => stub.GetValue (2)).Return (value3);

      Assert.That( _queryResultRow.GetRawValue (0), Is.EqualTo(value1));
      Assert.That (_queryResultRow.GetRawValue (1), Is.EqualTo (value2));
      Assert.That (_queryResultRow.GetRawValue (2), Is.EqualTo (value3));
    }

    [Test]
    public void GetConvertedValue_ValidType ()
    {
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageType (typeof (string))).Return (_storageTypeInformationMock);

      var fakeResult = "fake";
      _storageTypeInformationMock.Expect (mock => mock.Read (_dataReaderStub, 1)).Return (fakeResult);

      var result = _queryResultRow.GetConvertedValue (1, typeof (string));

      Assert.That (result, Is.EqualTo (fakeResult));
      _storageTypeInformationMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException(typeof(NotSupportedException), ExpectedMessage = "Type not supported.")]
    public void GetConvertedValue_ThrowsNotSupportedException_TypeNotObjectID ()
    {
      _storageTypeInformationProviderStub
        .Stub (stub => stub.GetStorageType (typeof (int)))
        .Throw(new NotSupportedException("Type not supported."));

      _queryResultRow.GetConvertedValue (1, typeof (int));
    }

    [Test]
    [ExpectedException(typeof(NotSupportedException), ExpectedMessage = 
      "Type 'ObjectID' ist not supported by this storage provider.\r\n"
      + "Please select the ID and ClassID values separately, then create an ObjectID with it in memory "
      + "(e.g., 'select new ObjectID (o.ID.ClassID, o.ID.Value)').")]
    public void GetConvertedValue_ThrowsNotSupportedException_TypeObjectID ()
    {
      _storageTypeInformationProviderStub
        .Stub (stub => stub.GetStorageType (typeof (ObjectID)))
        .Throw (new NotSupportedException ("Type not supported."));

      _queryResultRow.GetConvertedValue (1, typeof (ObjectID));
    }

    [Test]
    public void GetConvertedValue_GenericOverload_DelegatesToNoGenericOverload ()
    {
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageType (typeof (string))).Return (_storageTypeInformationMock);

      var fakeResult = "fake";
      _storageTypeInformationMock.Expect (mock => mock.Read (_dataReaderStub, 1)).Return (fakeResult);
      _storageTypeInformationMock.Replay ();

      var result = _queryResultRow.GetConvertedValue<string> (1);

      Assert.That (result, Is.EqualTo (fakeResult));
      _storageTypeInformationMock.VerifyAllExpectations ();
    }
  }
}