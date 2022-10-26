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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class TimestampReaderTest : StandardMappingTest
  {
    private Mock<IDataReader> _dataReaderStrictMock;
    private Mock<IRdbmsStoragePropertyDefinition> _idPropertyStrictMock;
    private Mock<IRdbmsStoragePropertyDefinition> _timestampStrictMock;

    private Mock<IColumnOrdinalProvider> _columnOrdinalProviderStub;

    private TimestampReader _reader;

    private ObjectID _fakeObjectIDResult;
    private object _fakeTimestampResult;

    public override void SetUp ()
    {
      base.SetUp();

      _dataReaderStrictMock = new Mock<IDataReader>(MockBehavior.Strict);
      _idPropertyStrictMock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);
      _timestampStrictMock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);
      _columnOrdinalProviderStub = new Mock<IColumnOrdinalProvider>();

      _reader = new TimestampReader(_idPropertyStrictMock.Object, _timestampStrictMock.Object, _columnOrdinalProviderStub.Object);

      _fakeObjectIDResult = new ObjectID(typeof(Order), Guid.NewGuid());
      _fakeTimestampResult = new object();
    }

    [Test]
    public void Read_DataReaderReadFalse ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(false).Verifiable();

      var result = _reader.Read(_dataReaderStrictMock.Object);

      VerifyAll();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue_NullValueForObjectID ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(true).Verifiable();
      _idPropertyStrictMock
          .Setup(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns((object)null)
          .Verifiable();

      var result = _reader.Read(_dataReaderStrictMock.Object);

      VerifyAll();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue_NonNullValue ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(true).Verifiable();
      _idPropertyStrictMock
          .Setup(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(_fakeObjectIDResult)
          .Verifiable();
      _timestampStrictMock
          .Setup(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(_fakeTimestampResult)
          .Verifiable();

      var result = _reader.Read(_dataReaderStrictMock.Object);

      VerifyAll();
      Assert.That(result, Is.EqualTo(Tuple.Create(_fakeObjectIDResult, _fakeTimestampResult)));
    }

    [Test]
    public void Read_DataReaderReadTrue_NullValueForTimestamp ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(true).Verifiable();
      _idPropertyStrictMock
          .Setup(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(_fakeObjectIDResult)
          .Verifiable();
      _timestampStrictMock
          .Setup(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns((object)null)
          .Verifiable();

      Assert.That(
          () => _reader.Read(_dataReaderStrictMock.Object),
          Throws.TypeOf<RdbmsProviderException>().With.Message.EqualTo("No timestamp was read for object '" + _fakeObjectIDResult + "'."));

      VerifyAll();
    }

    [Test]
    public void ReadSequence ()
    {
      var fakeObjectIDResult2 = new ObjectID(typeof(OrderItem), Guid.NewGuid());
      var fakeTimestampResult2 = new object();

      _dataReaderStrictMock
          .SetupSequence(mock => mock.Read())
          .Returns(true)
          .Returns(true)
          .Returns(true)
          .Returns(false);
      _idPropertyStrictMock
          .SetupSequence(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(_fakeObjectIDResult)
          .Returns((object)null)
          .Returns(fakeObjectIDResult2);
      _timestampStrictMock
          .SetupSequence(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(_fakeTimestampResult)
          // NOP
          .Returns(fakeTimestampResult2);

      var result = _reader.ReadSequence(_dataReaderStrictMock.Object).ToArray();

      VerifyAll();
      Assert.That(result.Length, Is.EqualTo(3));
      Assert.That(result[0], Is.EqualTo(Tuple.Create(_fakeObjectIDResult, _fakeTimestampResult)));
      Assert.That(result[1], Is.Null);
      Assert.That(result[2], Is.EqualTo(Tuple.Create(fakeObjectIDResult2, fakeTimestampResult2)));
    }

    [Test]
    public void ReadSequence_NoData ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(false).Verifiable();

      var result = _reader.ReadSequence(_dataReaderStrictMock.Object).ToArray();

      VerifyAll();
      Assert.That(result, Is.Empty);
    }

    private void VerifyAll ()
    {
      _dataReaderStrictMock.Verify();
      _idPropertyStrictMock.Verify();
      _timestampStrictMock.Verify();
    }

    private ColumnValueReader MatchColumnValueReader ()
    {
      return Match<IColumnValueProvider>.Create<ColumnValueReader>(
          r => r.DataReader == _dataReaderStrictMock.Object
               && r.ColumnOrdinalProvider == _columnOrdinalProviderStub.Object);
    }

  }
}
