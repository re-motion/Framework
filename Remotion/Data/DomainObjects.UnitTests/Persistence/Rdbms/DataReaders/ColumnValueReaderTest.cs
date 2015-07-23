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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class ColumnValueReaderTest
  {
    private IDataReader _dataReaderStub;
    private IColumnOrdinalProvider _columnOrdinalProviderStub;
    private IStorageTypeInformation _storageTypeInformationStrictMock;
    private ColumnDefinition _columnDefinition;

    private ColumnValueReader _columnValueReader;

    [SetUp]
    public void SetUp ()
    {
      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _columnOrdinalProviderStub = MockRepository.GenerateStub<IColumnOrdinalProvider>();
      _storageTypeInformationStrictMock = MockRepository.GenerateStrictMock<IStorageTypeInformation>();
      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn (storageTypeInformation: _storageTypeInformationStrictMock);

      _columnValueReader = new ColumnValueReader (_dataReaderStub, _columnOrdinalProviderStub);
    }

    [Test]
    public void GetValueForColumn ()
    {
      _columnOrdinalProviderStub.Stub (stub => stub.GetOrdinal (_columnDefinition, _dataReaderStub)).Return (17);

      _storageTypeInformationStrictMock
          .Expect (mock => mock.Read (_dataReaderStub, 17))
          .Return ("value");
      _storageTypeInformationStrictMock.Replay();

      var result = _columnValueReader.GetValueForColumn (_columnDefinition);

      _storageTypeInformationStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("value"));
    }
  }
}