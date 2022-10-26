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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class ObjectIDReaderTest : StandardMappingTest
  {
    private Mock<IDataReader> _dataReaderStrictMock;
    private Mock<IRdbmsStoragePropertyDefinition> _idPropertyStrictMock;
    private Mock<IColumnOrdinalProvider> _columnOrdinalProviderStub;

    private ObjectIDReader _reader;

    private ObjectID _objectID;

    public override void SetUp ()
    {
      base.SetUp();

      _dataReaderStrictMock = new Mock<IDataReader>(MockBehavior.Strict);
      _idPropertyStrictMock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);
      _columnOrdinalProviderStub = new Mock<IColumnOrdinalProvider>();

      _reader = new ObjectIDReader(_idPropertyStrictMock.Object, _columnOrdinalProviderStub.Object);

      _objectID = new ObjectID("Order", Guid.NewGuid());
    }

    [Test]
    public void Read_DataReaderReadFalse ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(false).Verifiable();
      ReplayAll();

      var result = _reader.Read(_dataReaderStrictMock.Object);

      VerifyAll();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(true).Verifiable();
      _idPropertyStrictMock
          .Setup(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(_objectID)
          .Verifiable();
      ReplayAll();

      var result = _reader.Read(_dataReaderStrictMock.Object);

      VerifyAll();
      Assert.That(result, Is.SameAs(_objectID));
    }

    [Test]
    public void Read_DataReaderReadTrue_Null ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(true).Verifiable();
      _idPropertyStrictMock.Setup(mock => mock.CombineValue(It.IsAny<IColumnValueProvider>())).Returns((object)null).Verifiable();
      ReplayAll();

      var result = _reader.Read(_dataReaderStrictMock.Object);

      VerifyAll();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void ReadSequence ()
    {
      var objectID2 = new ObjectID("OrderItem", Guid.NewGuid());
      _dataReaderStrictMock
          .SetupSequence(mock => mock.Read())
          .Returns(true)
          .Returns(true)
          .Returns(true)
          .Returns(false);
      _idPropertyStrictMock
          .SetupSequence(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(_objectID)
          .Returns((object)null)
          .Returns(objectID2);
      ReplayAll();

      var result = _reader.ReadSequence(_dataReaderStrictMock.Object).ToArray();

      VerifyAll();
      Assert.That(result.Length, Is.EqualTo(3));
      Assert.That(result[0], Is.SameAs(_objectID));
      Assert.That(result[1], Is.Null);
      Assert.That(result[2], Is.SameAs(objectID2));
    }
    [Test]
    public void ReadSequence_NoData ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(false).Verifiable();
      ReplayAll();

      var result = _reader.ReadSequence(_dataReaderStrictMock.Object).ToArray();

      VerifyAll();
      Assert.That(result, Is.Empty);
    }

    private void ReplayAll ()
    {
    }

    private void VerifyAll ()
    {
      _dataReaderStrictMock.Verify();
      _idPropertyStrictMock.Verify();
    }

    private ColumnValueReader MatchColumnValueReader ()
    {
      return Match<IColumnValueProvider>.Create<ColumnValueReader>(
          r => r.DataReader == _dataReaderStrictMock.Object
               && r.ColumnOrdinalProvider == _columnOrdinalProviderStub.Object);
    }
  }
}
