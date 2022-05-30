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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class QueryResultRowTest
  {
    private Mock<IDataReader> _dataReaderStub;
    private Mock<IStorageTypeInformationProvider> _storageTypeInformationProviderStub;
    private QueryResultRow _queryResultRow;
    private Mock<IStorageTypeInformation> _storageTypeInformationMock;

    [SetUp]
    public void SetUp ()
    {
      _dataReaderStub = new Mock<IDataReader>();
      _storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
      _queryResultRow = new QueryResultRow(_dataReaderStub.Object, _storageTypeInformationProviderStub.Object);

      _storageTypeInformationMock = new Mock<IStorageTypeInformation>(MockBehavior.Strict);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_queryResultRow.DataReader, Is.SameAs(_dataReaderStub.Object));
      Assert.That(_queryResultRow.StorageTypeInformationProvider, Is.SameAs(_storageTypeInformationProviderStub.Object));
    }

    [Test]
    public void ValueCount ()
    {
      int fakeFieldCount = 10;
      _dataReaderStub.Setup(stub => stub.FieldCount).Returns(fakeFieldCount);

      var result = _queryResultRow.ValueCount;

      Assert.That(result, Is.EqualTo(fakeFieldCount));
    }

    [Test]
    public void GetRawValue_WithString ()
    {
      var value = "hallo";

      _dataReaderStub.Setup(stub => stub.GetValue(0)).Returns(value);

      Assert.That(_queryResultRow.GetRawValue(0), Is.EqualTo(value));
    }

    [Test]
    public void GetRawValue_WithInt32 ()
    {
      var value = true;

      _dataReaderStub.Setup(stub => stub.GetValue(1)).Returns(value);

      Assert.That(_queryResultRow.GetRawValue(1), Is.EqualTo(value));
    }

    [Test]
    public void GetRawValue_WithBoolean ()
    {
      var value = 45;

      _dataReaderStub.Setup(stub => stub.GetValue(2)).Returns(value);

      Assert.That(_queryResultRow.GetRawValue(2), Is.EqualTo(value));
    }

    [Test]
    public void GetRawValue_WithDBNull ()
    {
      _dataReaderStub.Setup(stub => stub.GetValue(3)).Returns(DBNull.Value);

      Assert.That(_queryResultRow.GetRawValue(3), Is.EqualTo(DBNull.Value));
    }

    [Test]
    public void GetRawValue_WithNull ()
    {
      _dataReaderStub.Setup(stub => stub.GetValue(4)).Returns((object)null);

      Assert.That(_queryResultRow.GetRawValue(4), Is.EqualTo(null));
    }

    [Test]
    public void GetConvertedValue_ValidType ()
    {
      _storageTypeInformationProviderStub.Setup(stub => stub.GetStorageType(typeof(string))).Returns(_storageTypeInformationMock.Object);

      var fakeResult = "fake";
      _storageTypeInformationMock.Setup(mock => mock.Read(_dataReaderStub.Object, 1)).Returns(fakeResult).Verifiable();

      var result = _queryResultRow.GetConvertedValue(1, typeof(string));

      Assert.That(result, Is.EqualTo(fakeResult));
      _storageTypeInformationMock.Verify();
    }

    [Test]
    public void GetConvertedValue_ThrowsNotSupportedException_TypeNotObjectID ()
    {
      _storageTypeInformationProviderStub
          .Setup(stub => stub.GetStorageType(typeof(int)))
          .Throws(new NotSupportedException("Type not supported."));
      Assert.That(
          () => _queryResultRow.GetConvertedValue(1, typeof(int)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Type not supported."));
    }

    [Test]
    public void GetConvertedValue_ThrowsNotSupportedException_TypeObjectID ()
    {
      _storageTypeInformationProviderStub
          .Setup(stub => stub.GetStorageType(typeof(ObjectID)))
          .Throws(new NotSupportedException("Type not supported."));
      Assert.That(
          () => _queryResultRow.GetConvertedValue(1, typeof(ObjectID)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Type 'ObjectID' ist not supported by this storage provider.\r\n"
                  + "Please select the ID and ClassID values separately, then create an ObjectID with it in memory "
                  + "(e.g., 'select new ObjectID (o.ID.ClassID, o.ID.Value)')."));
    }

    [Test]
    public void GetConvertedValue_GenericOverloadWithReferenceType_DelegatesToNoGenericOverload ()
    {
      _storageTypeInformationProviderStub.Setup(stub => stub.GetStorageType(typeof(string))).Returns(_storageTypeInformationMock.Object);

      var fakeResult = "fake";
      _storageTypeInformationMock.Setup(mock => mock.Read(_dataReaderStub.Object, 1)).Returns(fakeResult).Verifiable();

      string result = _queryResultRow.GetConvertedValue<string>(1);

      Assert.That(result, Is.EqualTo(fakeResult));
      _storageTypeInformationMock.Verify();
    }

    [Test]
    public void GetConvertedValue_GenericOverloadWithNullableValueType_DelegatesToNoGenericOverload ()
    {
      _storageTypeInformationProviderStub.Setup(stub => stub.GetStorageType(typeof(int?))).Returns(_storageTypeInformationMock.Object);

      _storageTypeInformationMock.Setup(mock => mock.Read(_dataReaderStub.Object, 1)).Returns(13).Verifiable();

      int? result = _queryResultRow.GetConvertedValue<int?>(1);

      Assert.That(result, Is.EqualTo(13));
      _storageTypeInformationMock.Verify();
    }

    [Test]
    public void GetConvertedValue_GenericOverloadWithNullableValueTypeAndNullValue_DelegatesToNoGenericOverload ()
    {
      _storageTypeInformationProviderStub.Setup(stub => stub.GetStorageType(typeof(int?))).Returns(_storageTypeInformationMock.Object);

      _storageTypeInformationMock.Setup(mock => mock.Read(_dataReaderStub.Object, 1)).Returns((object)null).Verifiable();

      int? result = _queryResultRow.GetConvertedValue<int?>(1);

      Assert.That(result, Is.EqualTo(null));
      _storageTypeInformationMock.Verify();
    }

    [Test]
    public void GetConvertedValue_GenericOverloadWithValueType_DelegatesToNoGenericOverload ()
    {
      _storageTypeInformationProviderStub.Setup(stub => stub.GetStorageType(typeof(int))).Returns(_storageTypeInformationMock.Object);

      _storageTypeInformationMock.Setup(mock => mock.Read(_dataReaderStub.Object, 1)).Returns(13).Verifiable();

      int result = _queryResultRow.GetConvertedValue<int>(1);

      Assert.That(result, Is.EqualTo(13));
      _storageTypeInformationMock.Verify();
    }

    [Test]
    public void GetConvertedValue_GenericOverloadWithValueTypeAndNullValue_DelegatesToNoGenericOverload ()
    {
      _storageTypeInformationProviderStub.Setup(stub => stub.GetStorageType(typeof(int))).Returns(_storageTypeInformationMock.Object);

      _storageTypeInformationMock.Setup(mock => mock.Read(_dataReaderStub.Object, 3)).Returns((object)null).Verifiable();

      Assert.That(
          () => _queryResultRow.GetConvertedValue<int>(3),
          Throws.Exception.TypeOf<InvalidCastException>()
              .With.Message.EqualTo(
                  "Type parameter 'T' is a value type ('System.Int32') but the result at position '3' is null. Use 'System.Nullable<System.Int32>' instead as type parameter."));

      _storageTypeInformationMock.Verify();
    }
  }
}
