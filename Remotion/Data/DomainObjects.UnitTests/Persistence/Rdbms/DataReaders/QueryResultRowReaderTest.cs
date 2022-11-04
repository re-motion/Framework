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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class QueryResultRowReaderTest
  {
    private Mock<IStorageTypeInformationProvider> _storageTypeInformationProviderStub;
    private QueryResultRowReader _queryResultRowReader;
    private Mock<IDataReader> _dataReaderStrictMock;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
      _dataReaderStrictMock = new Mock<IDataReader>(MockBehavior.Strict);

      _queryResultRowReader = new QueryResultRowReader(_storageTypeInformationProviderStub.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_queryResultRowReader.StorageTypeInformationProvider, Is.SameAs(_storageTypeInformationProviderStub.Object));
    }

    [Test]
    public void Read ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(true).Verifiable();

      var result = _queryResultRowReader.Read(_dataReaderStrictMock.Object);

      _dataReaderStrictMock.Verify();
      Assert.That(result, Is.Not.Null);
      CheckQueryResultRow(result);
    }

    [Test]
    public void Read_NoData ()
    {
      _dataReaderStrictMock.Setup(mock => mock.Read()).Returns(false).Verifiable();

      var result = _queryResultRowReader.Read(_dataReaderStrictMock.Object);

      _dataReaderStrictMock.Verify();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void ReadSequence ()
    {
      _dataReaderStrictMock
          .SetupSequence(mock => mock.Read())
          .Returns(true).Returns(true)
          .Returns(true)
          .Returns(false);

      var result = _queryResultRowReader.ReadSequence(_dataReaderStrictMock.Object).OfType<QueryResultRow>().ToList();

      _dataReaderStrictMock.Verify(mock => mock.Read(), Times.Exactly(4));
      Assert.That(result, Is.Not.Null);
      Assert.That(result.Count, Is.EqualTo(3));
      CheckQueryResultRow(result[0]);
      CheckQueryResultRow(result[1]);
      CheckQueryResultRow(result[2]);
    }

    private void CheckQueryResultRow (IQueryResultRow result)
    {
      Assert.That(result, Is.TypeOf(typeof(QueryResultRow)));
      Assert.That(((QueryResultRow)result).StorageTypeInformationProvider, Is.SameAs(_storageTypeInformationProviderStub.Object));
      Assert.That(((QueryResultRow)result).DataReader, Is.SameAs(_dataReaderStrictMock.Object));
    }

  }
}
