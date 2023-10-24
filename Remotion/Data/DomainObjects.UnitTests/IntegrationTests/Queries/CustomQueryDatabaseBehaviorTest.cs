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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Queries
{
  [TestFixture]
  public class CustomQueryDatabaseBehaviorTest : QueryTestBase
  {
    private ServiceLocatorScope _serviceLocatorScope;
    private Mock<IPersistenceExtension> _persistenceExtensionMock;

    public override void SetUp ()
    {
      base.SetUp();

      var persistenceExtensionFactoryStub = new Mock<IPersistenceExtensionFactory>();
      _persistenceExtensionMock = new Mock<IPersistenceExtension>();

      persistenceExtensionFactoryStub
          .Setup(stub => stub.CreatePersistenceExtensions(TestableClientTransaction.ID))
          .Returns(new[] { _persistenceExtensionMock.Object });

      var locator = DefaultServiceLocator.Create();
      locator.RegisterSingle<IPersistenceExtensionFactory>(() => persistenceExtensionFactoryStub.Object);
      _serviceLocatorScope = new ServiceLocatorScope(locator);
    }

    public override void TearDown ()
    {
      _serviceLocatorScope.Dispose();
      base.TearDown();
    }

    [Test]
    public void CompleteIteration_CompletelyExecutesQuery ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomQuery"));

      QueryManager.GetCustom(query, QueryResultRowTestHelper.ExtractRawValues).ToList();

      _persistenceExtensionMock.Verify(mock => mock.ConnectionOpened(It.IsAny<Guid>()), Times.AtLeastOnce());
      _persistenceExtensionMock.Verify(
          mock => mock.QueryExecuting(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()),
          Times.AtLeastOnce());
      _persistenceExtensionMock.Verify(mock => mock.QueryExecuted(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan>()), Times.AtLeastOnce());
      _persistenceExtensionMock.Verify(mock => mock.QueryCompleted(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.AtLeastOnce());
      _persistenceExtensionMock.Verify(mock => mock.ConnectionClosed(It.IsAny<Guid>()), Times.AtLeastOnce());
    }

    [Test]
    public void NoIteration_DoesNotOpenConnection ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomQuery"));

      QueryManager.GetCustom(query, QueryResultRowTestHelper.ExtractRawValues);

      _persistenceExtensionMock.Verify(mock => mock.ConnectionOpened(It.IsAny<Guid>()), Times.Never());
      _persistenceExtensionMock.Verify(
          mock => mock.QueryExecuting(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()),
          Times.Never());
      _persistenceExtensionMock.Verify(mock => mock.QueryExecuted(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan>()), Times.Never());
      _persistenceExtensionMock.Verify(mock => mock.QueryCompleted(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.Never());
      _persistenceExtensionMock.Verify(mock => mock.ConnectionClosed(It.IsAny<Guid>()), Times.Never());
    }

    [Test]
    public void DuringIteration_QueryStaysActive ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomQuery"));

      var result = QueryManager.GetCustom(query, QueryResultRowTestHelper.ExtractRawValues);

      using (var iterator = result.GetEnumerator())
      {
        iterator.MoveNext();

        _persistenceExtensionMock.Verify(mock => mock.ConnectionOpened(It.IsAny<Guid>()), Times.AtLeastOnce());
        _persistenceExtensionMock.Verify(
            mock => mock.QueryExecuting(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()),
            Times.AtLeastOnce());
        _persistenceExtensionMock.Verify(mock => mock.QueryExecuted(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan>()), Times.AtLeastOnce());
        _persistenceExtensionMock.Verify(mock => mock.QueryCompleted(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.Never());
        _persistenceExtensionMock.Verify(mock => mock.ConnectionClosed(It.IsAny<Guid>()), Times.Never());
      }

      _persistenceExtensionMock.Verify(mock => mock.QueryCompleted(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.AtLeastOnce());
      _persistenceExtensionMock.Verify(mock => mock.ConnectionClosed(It.IsAny<Guid>()), Times.AtLeastOnce());
    }
  }
}
