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
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class CompoundPersistenceExtensionTest
  {
    private Mock<IPersistenceExtension> _innerPersistenceListener1;
    private Mock<IPersistenceExtension> _innerPersistenceListener2;
    private IPersistenceExtension _extension;
    private List<IPersistenceExtension> _listeners;

    [SetUp]
    public void SetUp ()
    {
      _innerPersistenceListener1 = new Mock<IPersistenceExtension> (MockBehavior.Strict); //add second listener
      _innerPersistenceListener2 = new Mock<IPersistenceExtension> (MockBehavior.Strict);
      _listeners = new List<IPersistenceExtension> { _innerPersistenceListener1.Object, _innerPersistenceListener2.Object };

      _extension = new CompoundPersistenceExtension(_listeners);
    }

    [Test]
    public void ConnectionOpened ()
    {
      var connectionID = Guid.NewGuid();
      _innerPersistenceListener1.Setup (mock => mock.ConnectionOpened (connectionID)).Verifiable();
      _innerPersistenceListener2.Setup (mock => mock.ConnectionOpened (connectionID)).Verifiable();

      _extension.ConnectionOpened(connectionID);

      _innerPersistenceListener1.Verify();
      _innerPersistenceListener2.Verify();
    }

    [Test]
    public void ConnectionClosed ()
    {
      var connectionID = Guid.NewGuid();
      _innerPersistenceListener1.Setup (mock => mock.ConnectionClosed (connectionID)).Verifiable();
      _innerPersistenceListener2.Setup (mock => mock.ConnectionClosed (connectionID)).Verifiable();

      _extension.ConnectionClosed(connectionID);

      _innerPersistenceListener1.Verify();
      _innerPersistenceListener2.Verify();
    }

    [Test]
    public void TransactionBegan ()
    {
      var connectionID = Guid.NewGuid();
      var isolationLevel = IsolationLevel.Chaos;
      _innerPersistenceListener1.Setup (mock => mock.TransactionBegan (connectionID, isolationLevel)).Verifiable();
      _innerPersistenceListener2.Setup (mock => mock.TransactionBegan (connectionID, isolationLevel)).Verifiable();

      _extension.TransactionBegan(connectionID, isolationLevel);

      _innerPersistenceListener1.Verify();
      _innerPersistenceListener2.Verify();
    }

    [Test]
    public void TransactionCommitted ()
    {
      var connectionID = Guid.NewGuid();
      _innerPersistenceListener1.Setup (mock => mock.TransactionCommitted (connectionID)).Verifiable();
      _innerPersistenceListener2.Setup (mock => mock.TransactionCommitted (connectionID)).Verifiable();

      _extension.TransactionCommitted(connectionID);

      _innerPersistenceListener1.Verify();
      _innerPersistenceListener2.Verify();
    }

    [Test]
    public void TransactionRolledBack ()
    {
      var connectionID = Guid.NewGuid();
      _innerPersistenceListener1.Setup (mock => mock.TransactionRolledBack (connectionID)).Verifiable();
      _innerPersistenceListener2.Setup (mock => mock.TransactionRolledBack (connectionID)).Verifiable();

      _extension.TransactionRolledBack(connectionID);

      _innerPersistenceListener1.Verify();
      _innerPersistenceListener2.Verify();
    }

    [Test]
    public void TransactionDisposed ()
    {
      var connectionID = Guid.NewGuid();
      _innerPersistenceListener1.Setup (mock => mock.TransactionDisposed (connectionID)).Verifiable();
      _innerPersistenceListener2.Setup (mock => mock.TransactionDisposed (connectionID)).Verifiable();

      _extension.TransactionDisposed(connectionID);

      _innerPersistenceListener1.Verify();
      _innerPersistenceListener2.Verify();
    }

    [Test]
    public void QueryExecuting ()
    {
      var connectionID = Guid.NewGuid();
      var queryID = Guid.NewGuid();
      var commandText = "commandText";
      var parameters = new Mock<IDictionary<string, object>> (MockBehavior.Strict);

      _innerPersistenceListener1.Setup (mock => mock.QueryExecuting (connectionID, queryID, commandText, parameters.Object)).Verifiable();
      _innerPersistenceListener2.Setup (mock => mock.QueryExecuting (connectionID, queryID, commandText, parameters.Object)).Verifiable();

      _extension.QueryExecuting(connectionID, queryID, commandText, parameters.Object);

      _innerPersistenceListener1.Verify();
      _innerPersistenceListener2.Verify();
      parameters.Verify();
    }

    [Test]
    public void QueryExecuted ()
    {
      var connectionID = Guid.NewGuid();
      var queryID = Guid.NewGuid();
      var durationOfQueryExecution = new TimeSpan();

      _innerPersistenceListener1.Setup (mock => mock.QueryExecuted (connectionID, queryID, durationOfQueryExecution)).Verifiable();
      _innerPersistenceListener2.Setup (mock => mock.QueryExecuted (connectionID, queryID, durationOfQueryExecution)).Verifiable();

      _extension.QueryExecuted(connectionID, queryID, durationOfQueryExecution);

      _innerPersistenceListener1.Verify();
      _innerPersistenceListener2.Verify();
    }

    [Test]
    public void QueryCompleted ()
    {
      var connectionID = Guid.NewGuid();
      var queryID = Guid.NewGuid();
      var durationOfDataRead = new TimeSpan();
      var rowCount = 6;

      _innerPersistenceListener1.Setup (mock => mock.QueryCompleted (connectionID, queryID, durationOfDataRead, rowCount)).Verifiable();
      _innerPersistenceListener2.Setup (mock => mock.QueryCompleted (connectionID, queryID, durationOfDataRead, rowCount)).Verifiable();

      _extension.QueryCompleted(connectionID, queryID, durationOfDataRead, rowCount);

      _innerPersistenceListener1.Verify();
      _innerPersistenceListener2.Verify();
    }

    [Test]
    public void QueryError ()
    {
      var connectionID = Guid.NewGuid();
      var queryID = Guid.NewGuid();
      Exception ex = new Exception();

      _innerPersistenceListener1.Setup (mock => mock.QueryError (connectionID, queryID, ex)).Verifiable();
      _innerPersistenceListener2.Setup (mock => mock.QueryError (connectionID, queryID, ex)).Verifiable();

      _extension.QueryError(connectionID, queryID, ex);

      _innerPersistenceListener1.Verify();
      _innerPersistenceListener2.Verify();
    }

    [Test]
    public void IsNull ()
    {
      var result = _extension.IsNull;
      Assert.That(result, Is.False);
    }
  }
}
