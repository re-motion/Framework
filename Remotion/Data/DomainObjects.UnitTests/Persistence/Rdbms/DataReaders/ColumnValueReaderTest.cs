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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class ColumnValueReaderTest
  {
    private Mock<IDataReader> _dataReaderStub;
    private Mock<IColumnOrdinalProvider> _columnOrdinalProviderStub;
    private Mock<IStorageTypeInformation> _storageTypeInformationStrictMock;
    private ColumnDefinition _columnDefinition;

    private ColumnValueReader _columnValueReader;

    [SetUp]
    public void SetUp ()
    {
      _dataReaderStub = new Mock<IDataReader>();
      _columnOrdinalProviderStub = new Mock<IColumnOrdinalProvider>();
      _storageTypeInformationStrictMock = new Mock<IStorageTypeInformation> (MockBehavior.Strict);
      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn(storageTypeInformation: _storageTypeInformationStrictMock.Object);

      _columnValueReader = new ColumnValueReader(_dataReaderStub.Object, _columnOrdinalProviderStub.Object);
    }

    [Test]
    public void GetValueForColumn ()
    {
      _columnOrdinalProviderStub.Setup (stub => stub.GetOrdinal (_columnDefinition, _dataReaderStub.Object)).Returns (17);

      _storageTypeInformationStrictMock
          .Setup(mock => mock.Read(_dataReaderStub.Object, 17))
          .Returns("value")
          .Verifiable();

      var result = _columnValueReader.GetValueForColumn(_columnDefinition);

      _storageTypeInformationStrictMock.Verify();
      Assert.That(result, Is.EqualTo("value"));
    }
  }
}
