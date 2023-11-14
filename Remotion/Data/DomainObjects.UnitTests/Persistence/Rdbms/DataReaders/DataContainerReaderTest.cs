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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class DataContainerReaderTest : StandardMappingTest
  {
    private Mock<IDataReader> _dataReaderStrictMock;
    private Mock<IRdbmsStoragePropertyDefinition> _idPropertyStrictMock;
    private Mock<IRdbmsStoragePropertyDefinition> _timestampPropertyStrictMock;
    private Mock<IRdbmsStoragePropertyDefinition> _fileNamePropertyStrictMock;
    private Mock<IRdbmsStoragePropertyDefinition> _orderPropertyStrictMock;

    private Mock<IColumnOrdinalProvider> _ordinalProviderStub;
    private Mock<IRdbmsPersistenceModelProvider> _persistenceModelProviderStub;
    private Mock<IDataContainerValidator> _dataContainerValidatorStub;

    private DataContainerReader _dataContainerReader;

    private object _fakeTimestamp;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataReaderStrictMock = new Mock<IDataReader>(MockBehavior.Strict);
      _idPropertyStrictMock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);
      _timestampPropertyStrictMock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);
      _fileNamePropertyStrictMock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);
      _orderPropertyStrictMock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);

      _ordinalProviderStub = new Mock<IColumnOrdinalProvider>();
      _persistenceModelProviderStub = new Mock<IRdbmsPersistenceModelProvider>();
      _dataContainerValidatorStub = new Mock<IDataContainerValidator>();

      _dataContainerReader = new DataContainerReader(
          _idPropertyStrictMock.Object,
          _timestampPropertyStrictMock.Object,
          _ordinalProviderStub.Object,
          _persistenceModelProviderStub.Object,
          _dataContainerValidatorStub.Object);

      _fakeTimestamp = new object();
    }

    [Test]
    public void Read_DataReaderReadFalse ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(false).Verifiable();

      var result = _dataContainerReader.Read(_dataReaderStrictMock.Object);

      _dataReaderStrictMock.Verify();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue_ValueIDNotNull ()
    {
      StubPersistenceModelProviderForProperty(typeof(OrderTicket), "FileName", _fileNamePropertyStrictMock.Object);
      StubPersistenceModelProviderForProperty(typeof(OrderTicket), "Order", _orderPropertyStrictMock.Object);

      var sequence = new VerifiableSequence();
      _dataReaderStrictMock.InVerifiableSequence(sequence).Setup(mock => mock.Read()).Returns(true).Verifiable();
      _idPropertyStrictMock.InVerifiableSequence(sequence).Setup(mock => mock.CombineValue(MatchColumnValueReader())).Returns(DomainObjectIDs.OrderTicket1).Verifiable();
      _timestampPropertyStrictMock.InVerifiableSequence(sequence).Setup(mock => mock.CombineValue(MatchColumnValueReader())).Returns(17).Verifiable();
      _fileNamePropertyStrictMock.InVerifiableSequence(sequence).Setup(mock => mock.CombineValue(MatchColumnValueReader())).Returns("abc").Verifiable();
      _orderPropertyStrictMock.InVerifiableSequence(sequence).Setup(mock => mock.CombineValue(MatchColumnValueReader())).Returns(DomainObjectIDs.Order1).Verifiable();

      var dataContainer = _dataContainerReader.Read(_dataReaderStrictMock.Object);

      _persistenceModelProviderStub.Verify(
          provider => provider.GetStoragePropertyDefinition(GetPropertyDefinition(typeof(OrderTicket), "Int32TransactionProperty")),
          Times.Never());
      VerifyAll();
      sequence.Verify();
      Assert.That(dataContainer, Is.Not.Null);
      CheckLoadedDataContainer(dataContainer, DomainObjectIDs.OrderTicket1, 17, "abc", DomainObjectIDs.Order1);
      _dataContainerValidatorStub.Verify(_ => _.Validate(dataContainer), Times.AtLeastOnce());
    }

    [Test]
    public void Read_DataReaderReadTrue_ValueIDNull ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(true).Verifiable();

      _idPropertyStrictMock
          .Setup(stub => stub.CombineValue(It.IsAny<IColumnValueProvider>()))
          .Returns((object)null);

      var dataContainer = _dataContainerReader.Read(_dataReaderStrictMock.Object);

      VerifyAll();
      Assert.That(dataContainer, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue_ThrowsException ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(true).Verifiable();

      _idPropertyStrictMock
          .Setup(stub => stub.CombineValue(It.IsAny<IColumnValueProvider>()))
          .Returns(DomainObjectIDs.OrderTicket1);
      _timestampPropertyStrictMock
          .Setup(stub => stub.CombineValue(It.IsAny<IColumnValueProvider>()))
          .Returns(_fakeTimestamp);

      var propertyDefinition = GetPropertyDefinition(typeof(OrderTicket), "FileName");
      _persistenceModelProviderStub
          .Setup(stub => stub.GetStoragePropertyDefinition(propertyDefinition))
          .Throws(new InvalidOperationException("TestException"));
      Assert.That(
          () => _dataContainerReader.Read(_dataReaderStrictMock.Object),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo(
                  "Error while reading property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName' of object "
                  + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid': TestException"));
    }

    [Test]
    public void ReadSequence_DataReaderReadFalse ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(false).Verifiable();

      var result = _dataContainerReader.ReadSequence(_dataReaderStrictMock.Object);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void ReadSequence_DataReaderReadTrue ()
    {
      StubPersistenceModelProviderForProperty(typeof(OrderTicket), "FileName", _fileNamePropertyStrictMock.Object);
      StubPersistenceModelProviderForProperty(typeof(OrderTicket), "Order", _orderPropertyStrictMock.Object);

      var idPropertyQueue = new Queue<ObjectID>(new[] { DomainObjectIDs.OrderTicket1, DomainObjectIDs.OrderTicket2, DomainObjectIDs.OrderTicket3 });
      var timestampPropertyQueue = new Queue<object>(new object[] { 0, 1, 2 });
      var fileNamePropertyQueue = new Queue<string>(new[] { "first", "second", "third" });
      var orderPropertyQueue = new Queue<ObjectID>(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4 });

      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(() => idPropertyQueue.Count > 0).Verifiable();

      _idPropertyStrictMock.Setup(mock => mock.CombineValue(MatchColumnValueReader())).Returns(() => idPropertyQueue.Dequeue()).Verifiable();
      _timestampPropertyStrictMock.Setup(mock => mock.CombineValue(MatchColumnValueReader())).Returns(() => timestampPropertyQueue.Dequeue()).Verifiable();
      _fileNamePropertyStrictMock.Setup(mock => mock.CombineValue(MatchColumnValueReader())).Returns(() => fileNamePropertyQueue.Dequeue()).Verifiable();
      _orderPropertyStrictMock.Setup(mock => mock.CombineValue(MatchColumnValueReader())).Returns(() => orderPropertyQueue.Dequeue()).Verifiable();

      var result = _dataContainerReader.ReadSequence(_dataReaderStrictMock.Object).ToArray();

      VerifyAll();
      Assert.That(result.Length, Is.EqualTo(3));

      CheckLoadedDataContainer(result[0], DomainObjectIDs.OrderTicket1, 0, "first", DomainObjectIDs.Order1);
      CheckLoadedDataContainer(result[1], DomainObjectIDs.OrderTicket2, 1, "second", DomainObjectIDs.Order3);
      CheckLoadedDataContainer(result[2], DomainObjectIDs.OrderTicket3, 2, "third", DomainObjectIDs.Order4);

      _dataContainerValidatorStub.Verify(_ => _.Validate(result[0]), Times.AtLeastOnce());
      _dataContainerValidatorStub.Verify(_ => _.Validate(result[1]), Times.AtLeastOnce());
      _dataContainerValidatorStub.Verify(_ => _.Validate(result[2]), Times.AtLeastOnce());
    }

    [Test]
    public void ReadSequence_DataReaderReadTrue_NullIDIsReturned ()
    {
      _dataReaderStrictMock
          .SetupSequence(mock => mock.Read())
          .Returns(true)
          .Returns(false);

      _idPropertyStrictMock
          .Setup(mock => mock.CombineValue(It.IsAny<IColumnValueProvider>()))
          .Returns((object)null)
          .Verifiable();

      var result = _dataContainerReader.ReadSequence(_dataReaderStrictMock.Object).ToArray();

      VerifyAll();
      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.Null);
    }

    private void ExpectPropertyCombinesForOrderTicket (MockSequence sequence, ObjectID objectID, object timestamp, string fileName, ObjectID order)
    {
      _idPropertyStrictMock
          .InSequence(sequence)
          .Setup(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(objectID)
          .Verifiable();
      _timestampPropertyStrictMock
          .InSequence(sequence)
          .Setup(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(timestamp)
          .Verifiable();
      _fileNamePropertyStrictMock
          .InSequence(sequence)
          .Setup(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(fileName)
          .Verifiable();
      _orderPropertyStrictMock
          .InSequence(sequence)
          .Setup(mock => mock.CombineValue(MatchColumnValueReader()))
          .Returns(order)
          .Verifiable();
    }

    private ColumnValueReader MatchColumnValueReader ()
    {
      return Match<IColumnValueProvider>.Create<ColumnValueReader>(
          r => r.DataReader == _dataReaderStrictMock.Object
               && r.ColumnOrdinalProvider == _ordinalProviderStub.Object);
    }

    private void StubPersistenceModelProviderForProperty (
        Type declaringType, string shortPropertyName, IRdbmsStoragePropertyDefinition storagePropertyDefinitionStub)
    {
      var propertyDefinition = GetPropertyDefinition(declaringType, shortPropertyName);
      _persistenceModelProviderStub.Setup(stub => stub.GetStoragePropertyDefinition(propertyDefinition)).Returns(storagePropertyDefinitionStub);
    }

    private void CheckLoadedDataContainer (DataContainer dataContainer, ObjectID expectedID, int expectedTimestamp, string expectedFileName, ObjectID expectedOrder)
    {
      Assert.That(dataContainer.ID, Is.EqualTo(expectedID));
      Assert.That(dataContainer.Timestamp, Is.EqualTo(expectedTimestamp));

      Assert.That(dataContainer.GetValue(GetPropertyDefinition(typeof(OrderTicket), "FileName"), ValueAccess.Original), Is.EqualTo(expectedFileName));
      Assert.That(dataContainer.GetValue(GetPropertyDefinition(typeof(OrderTicket), "FileName")), Is.EqualTo(expectedFileName));
      Assert.That(dataContainer.GetValue(GetPropertyDefinition(typeof(OrderTicket), "Order"), ValueAccess.Original), Is.EqualTo(expectedOrder));
      Assert.That(dataContainer.GetValue(GetPropertyDefinition(typeof(OrderTicket), "Order")), Is.EqualTo(expectedOrder));
      Assert.That(dataContainer.GetValue(GetPropertyDefinition(typeof(OrderTicket), "Int32TransactionProperty"), ValueAccess.Original), Is.EqualTo(0));
      Assert.That(dataContainer.GetValue(GetPropertyDefinition(typeof(OrderTicket), "Int32TransactionProperty")), Is.EqualTo(0));
    }

    private void VerifyAll ()
    {
      _dataReaderStrictMock.Verify();
      _idPropertyStrictMock.Verify();
      _timestampPropertyStrictMock.Verify();
      _fileNamePropertyStrictMock.Verify();
      _orderPropertyStrictMock.Verify();
    }
  }
}
