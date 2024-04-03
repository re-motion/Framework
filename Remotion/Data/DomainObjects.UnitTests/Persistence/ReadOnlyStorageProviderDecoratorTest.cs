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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  public class ReadOnlyStorageProviderDecoratorTest
  {
    private static QueryType[] s_readOnlyTestCases = { QueryType.CollectionReadOnly, QueryType.CustomReadOnly, QueryType.ScalarReadOnly };
    private static QueryType[] s_readWriteTestCases = { QueryType.CollectionReadWrite, QueryType.CustomReadWrite, QueryType.ScalarReadWrite };

    [Test]
    [TestCaseSource(nameof(s_readOnlyTestCases))]
    public void ExecuteCollectionQuery_WithReadOnlyQueryType_CallsInnerStorageProvider (QueryType queryType)
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(_ => _.QueryType).Returns(queryType);
      var innerReadOnlyStorageProviderMock = new Mock<IReadOnlyStorageProvider>();
      var readOnlyStorageProviderDecorator = new ReadOnlyStorageProviderDecorator(innerReadOnlyStorageProviderMock.Object);
      var expectedResult = new DataContainer[1];
      innerReadOnlyStorageProviderMock.Setup(_ => _.ExecuteCollectionQuery(queryStub.Object)).Returns(expectedResult);

      var result = readOnlyStorageProviderDecorator.ExecuteCollectionQuery(queryStub.Object);

      Assert.That(result, Is.SameAs(expectedResult));
    }

    [Test]
    [TestCaseSource(nameof(s_readOnlyTestCases))]
    public void ExecuteCustomQuery_WithReadOnlyQueryType_CallsInnerStorageProvider (QueryType queryType)
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(_ => _.QueryType).Returns(queryType);
      var innerReadOnlyStorageProviderStub = new Mock<IReadOnlyStorageProvider>();
      var readOnlyStorageProviderDecorator = new ReadOnlyStorageProviderDecorator(innerReadOnlyStorageProviderStub.Object);
      var expectedResult = new IQueryResultRow[1];
      innerReadOnlyStorageProviderStub.Setup(_ => _.ExecuteCustomQuery(queryStub.Object)).Returns(expectedResult);

      var result = readOnlyStorageProviderDecorator.ExecuteCustomQuery(queryStub.Object);

      Assert.That(result, Is.SameAs(expectedResult));
    }

    [Test]
    [TestCaseSource(nameof(s_readOnlyTestCases))]
    public void ExecuteScalarQuery_WithReadOnlyQueryType_CallsInnerStorageProvider (QueryType queryType)
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(_ => _.QueryType).Returns(queryType);
      var innerReadOnlyStorageProviderStub = new Mock<IReadOnlyStorageProvider>();
      var readOnlyStorageProviderDecorator = new ReadOnlyStorageProviderDecorator(innerReadOnlyStorageProviderStub.Object);
      innerReadOnlyStorageProviderStub.Setup(_ => _.ExecuteScalarQuery(queryStub.Object)).Returns("Successful!");

      var result = readOnlyStorageProviderDecorator.ExecuteScalarQuery(queryStub.Object);

      Assert.That(result, Is.EqualTo("Successful!"));
    }

    [Test]
    [TestCaseSource(nameof(s_readWriteTestCases))]
    public void ExecuteCollectionQuery_WithReadWriteQueryType_ThrowsArgumentException (QueryType queryType)
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(_ => _.QueryType).Returns(queryType);
      var innerReadOnlyStorageProviderStub = new Mock<IReadOnlyStorageProvider>();
      var readOnlyStorageProviderDecorator = new ReadOnlyStorageProviderDecorator(innerReadOnlyStorageProviderStub.Object);

      Assert.That(
          () => readOnlyStorageProviderDecorator.ExecuteCollectionQuery(queryStub.Object),
          Throws.InstanceOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo(
              $"Query '{queryStub.Object.ID}' has query type '{queryStub.Object.QueryType}' which cannot be used in a read-only query scenario.",
              "query"));
    }

    [Test]
    [TestCaseSource(nameof(s_readWriteTestCases))]
    public void ExecuteScalarQuery_WithReadWriteQueryType_ThrowsArgumentException (QueryType queryType)
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(_ => _.QueryType).Returns(queryType);
      var innerReadOnlyStorageProviderStub = new Mock<IReadOnlyStorageProvider>();
      var readOnlyStorageProviderDecorator = new ReadOnlyStorageProviderDecorator(innerReadOnlyStorageProviderStub.Object);

      Assert.That(
          () => readOnlyStorageProviderDecorator.ExecuteCollectionQuery(queryStub.Object),
          Throws.InstanceOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo(
              $"Query '{queryStub.Object.ID}' has query type '{queryStub.Object.QueryType}' which cannot be used in a read-only query scenario.",
              "query"));
    }

    [Test]
    [TestCaseSource(nameof(s_readWriteTestCases))]
    public void ExecuteCustomQuery_WithReadWriteQueryType_ThrowsArgumentException (QueryType queryType)
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(_ => _.QueryType).Returns(queryType);
      var innerReadOnlyStorageProviderStub = new Mock<IReadOnlyStorageProvider>();
      var readOnlyStorageProviderDecorator = new ReadOnlyStorageProviderDecorator(innerReadOnlyStorageProviderStub.Object);

      Assert.That(
          () => readOnlyStorageProviderDecorator.ExecuteCollectionQuery(queryStub.Object),
          Throws.InstanceOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo(
              $"Query '{queryStub.Object.ID}' has query type '{queryStub.Object.QueryType}' which cannot be used in a read-only query scenario.",
              "query"));
    }
  }
}
