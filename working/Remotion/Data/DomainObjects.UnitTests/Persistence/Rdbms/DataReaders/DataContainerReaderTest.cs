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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class DataContainerReaderTest : StandardMappingTest
  {
    private IDataReader _dataReaderStrictMock;
    private IRdbmsStoragePropertyDefinition _idPropertyStrictMock;
    private IRdbmsStoragePropertyDefinition _timestampPropertyStrictMock;
    private IRdbmsStoragePropertyDefinition _fileNamePropertyStrictMock;
    private IRdbmsStoragePropertyDefinition _orderPropertyStrictMock;

    private IColumnOrdinalProvider _ordinalProviderStub;
    private IRdbmsPersistenceModelProvider _persistenceModelProviderStub;
    private IDataContainerValidator _dataContainerValidatorStub;

    private DataContainerReader _dataContainerReader;

    private object _fakeTimestamp;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataReaderStrictMock = MockRepository.GenerateStrictMock<IDataReader>();
      _idPropertyStrictMock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition>();
      _timestampPropertyStrictMock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition>();
      _fileNamePropertyStrictMock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition> ();
      _orderPropertyStrictMock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition> ();
      
      _ordinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();
      _persistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();
      _dataContainerValidatorStub = MockRepository.GenerateStub<IDataContainerValidator>();

      _dataContainerReader = new DataContainerReader (
          _idPropertyStrictMock,
          _timestampPropertyStrictMock,
          _ordinalProviderStub,
          _persistenceModelProviderStub,
          _dataContainerValidatorStub);

      _fakeTimestamp = new object();
    }

    [Test]
    public void Read_DataReaderReadFalse ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read()).Return (false);
      _dataReaderStrictMock.Replay();

      var result = _dataContainerReader.Read (_dataReaderStrictMock);

      _dataReaderStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void Read_DataReaderReadTrue_ValueIDNotNull ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read()).Return (true);

      StubPersistenceModelProviderForProperty (typeof (OrderTicket), "FileName", _fileNamePropertyStrictMock);
      StubPersistenceModelProviderForProperty (typeof (OrderTicket), "Order", _orderPropertyStrictMock);
      
      ExpectPropertyCombinesForOrderTicket (DomainObjectIDs.OrderTicket1, 17, "abc", DomainObjectIDs.Order1);
      ReplayAll();
      
      var dataContainer = _dataContainerReader.Read (_dataReaderStrictMock);

      _persistenceModelProviderStub.AssertWasNotCalled (
          provider => provider.GetStoragePropertyDefinition (GetPropertyDefinition (typeof (OrderTicket), "Int32TransactionProperty")));
      VerifyAll();
      Assert.That (dataContainer, Is.Not.Null);
      CheckLoadedDataContainer(dataContainer, DomainObjectIDs.OrderTicket1, 17, "abc", DomainObjectIDs.Order1);
      _dataContainerValidatorStub.AssertWasCalled (_ => _.Validate (dataContainer));
    }

    [Test]
    public void Read_DataReaderReadTrue_ValueIDNull ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (true);

      _idPropertyStrictMock
          .Stub (stub => stub.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (null);
      ReplayAll();

      var dataContainer = _dataContainerReader.Read (_dataReaderStrictMock);

      VerifyAll();
      Assert.That (dataContainer, Is.Null);
    }

    [Test]
    [ExpectedException(typeof(RdbmsProviderException), ExpectedMessage =
      "Error while reading property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName' of object "
      + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid': TestException")]
    public void Read_DataReaderReadTrue_ThrowsException ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read ()).Return (true);

      _idPropertyStrictMock
          .Stub (stub => stub.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (DomainObjectIDs.OrderTicket1);
      _timestampPropertyStrictMock
          .Stub (stub => stub.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (_fakeTimestamp);
      ReplayAll();

      var propertyDefinition = GetPropertyDefinition (typeof(OrderTicket), "FileName");
      _persistenceModelProviderStub
        .Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition))
        .Throw (new InvalidOperationException ("TestException"));
      
      _dataContainerReader.Read (_dataReaderStrictMock);
    }

    [Test]
    public void ReadSequence_DataReaderReadFalse ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read()).Return (false);

      var result = _dataContainerReader.ReadSequence (_dataReaderStrictMock);

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void ReadSequence_DataReaderReadTrue ()
    {
      StubPersistenceModelProviderForProperty (typeof (OrderTicket), "FileName", _fileNamePropertyStrictMock);
      StubPersistenceModelProviderForProperty (typeof (OrderTicket), "Order", _orderPropertyStrictMock);

      ExpectPropertyCombinesForOrderTicket (DomainObjectIDs.OrderTicket1, 0, "first", DomainObjectIDs.Order1);
      ExpectPropertyCombinesForOrderTicket (DomainObjectIDs.OrderTicket2, 1, "second", DomainObjectIDs.Order3);
      ExpectPropertyCombinesForOrderTicket (DomainObjectIDs.OrderTicket3, 2, "third", DomainObjectIDs.Order4);

      _dataReaderStrictMock.Expect (mock => mock.Read()).Return (true).Repeat.Times (3);
      _dataReaderStrictMock.Expect (mock => mock.Read()).Return (false);
      ReplayAll();

      var result = _dataContainerReader.ReadSequence (_dataReaderStrictMock).ToArray ();

      VerifyAll();
      Assert.That (result.Length, Is.EqualTo (3));

      CheckLoadedDataContainer (result[0], DomainObjectIDs.OrderTicket1, 0, "first", DomainObjectIDs.Order1);
      CheckLoadedDataContainer (result[1], DomainObjectIDs.OrderTicket2, 1, "second", DomainObjectIDs.Order3);
      CheckLoadedDataContainer (result[2], DomainObjectIDs.OrderTicket3, 2, "third", DomainObjectIDs.Order4);

      _dataContainerValidatorStub.AssertWasCalled (_ => _.Validate (result[0]));
      _dataContainerValidatorStub.AssertWasCalled (_ => _.Validate (result[1]));
      _dataContainerValidatorStub.AssertWasCalled (_ => _.Validate (result[2]));
    }

    [Test]
    public void ReadSequence_DataReaderReadTrue_NullIDIsReturned ()
    {
      _dataReaderStrictMock.Expect (mock => mock.Read()).Return (true).Repeat.Once();
      _dataReaderStrictMock.Expect (mock => mock.Read()).Return (false);
      
      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (null);
      ReplayAll();
      
      var result = _dataContainerReader.ReadSequence (_dataReaderStrictMock).ToArray();

      VerifyAll();
      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0], Is.Null);
    }

    private void ExpectPropertyCombinesForOrderTicket (ObjectID objectID, object timestamp, string fileName, ObjectID order)
    {
      _idPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (objectID)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Repeat.Once ();
      _timestampPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (timestamp)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Repeat.Once ();
      _fileNamePropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .Return (fileName)
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Repeat.Once ();
      _orderPropertyStrictMock
          .Expect (mock => mock.CombineValue (Arg<IColumnValueProvider>.Is.Anything))
          .WhenCalled (mi => CheckColumnValueReader ((ColumnValueReader) mi.Arguments[0]))
          .Return (order)
          .Repeat.Once ();
    }

    private void CheckColumnValueReader (ColumnValueReader columnValueReader)
    {
      Assert.That (columnValueReader.DataReader, Is.SameAs (_dataReaderStrictMock));
      Assert.That (columnValueReader.ColumnOrdinalProvider, Is.SameAs (_ordinalProviderStub));
    }

    private void StubPersistenceModelProviderForProperty (
        Type declaringType, string shortPropertyName, IRdbmsStoragePropertyDefinition storagePropertyDefinitionStub)
    {
      var propertyDefinition = GetPropertyDefinition (declaringType, shortPropertyName);
      _persistenceModelProviderStub.Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition)).Return (storagePropertyDefinitionStub);
    }

    private void CheckLoadedDataContainer (DataContainer dataContainer, ObjectID expectedID, int expectedTimestamp, string expectedFileName, ObjectID expectedOrder)
    {
      Assert.That (dataContainer.ID, Is.EqualTo (expectedID));
      Assert.That (dataContainer.Timestamp, Is.EqualTo (expectedTimestamp));

      Assert.That (dataContainer.GetValue (GetPropertyDefinition (typeof (OrderTicket), "FileName"), ValueAccess.Original), Is.EqualTo (expectedFileName));
      Assert.That (dataContainer.GetValue (GetPropertyDefinition (typeof (OrderTicket), "FileName")), Is.EqualTo (expectedFileName));
      Assert.That (dataContainer.GetValue (GetPropertyDefinition (typeof (OrderTicket), "Order"), ValueAccess.Original), Is.EqualTo (expectedOrder));
      Assert.That (dataContainer.GetValue (GetPropertyDefinition (typeof (OrderTicket), "Order")), Is.EqualTo (expectedOrder));
      Assert.That (dataContainer.GetValue (GetPropertyDefinition (typeof (OrderTicket), "Int32TransactionProperty"), ValueAccess.Original), Is.EqualTo (0));
      Assert.That (dataContainer.GetValue (GetPropertyDefinition (typeof (OrderTicket), "Int32TransactionProperty")), Is.EqualTo (0));
    }

    private void ReplayAll()
    {
      _dataReaderStrictMock.Replay();
      _idPropertyStrictMock.Replay();
      _timestampPropertyStrictMock.Replay();
      _fileNamePropertyStrictMock.Replay();
      _orderPropertyStrictMock.Replay();
    }

    private void VerifyAll()
    {
      _dataReaderStrictMock.VerifyAllExpectations();
      _idPropertyStrictMock.VerifyAllExpectations();
      _timestampPropertyStrictMock.VerifyAllExpectations();
      _fileNamePropertyStrictMock.VerifyAllExpectations();
      _orderPropertyStrictMock.VerifyAllExpectations();
    }
  }
}