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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class CompoundPersistenceExtensionTest
  {
    private MockRepository _mockRepository;
    private IPersistenceExtension _innerPersistenceListener1;
    private IPersistenceExtension _innerPersistenceListener2;
    private IPersistenceExtension _extension;
    private List<IPersistenceExtension> _listeners;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _innerPersistenceListener1 = _mockRepository.StrictMock<IPersistenceExtension>(); //add second listener
      _innerPersistenceListener2 = _mockRepository.StrictMock<IPersistenceExtension>();
      _listeners = new List<IPersistenceExtension> { _innerPersistenceListener1, _innerPersistenceListener2 };

      _extension = new CompoundPersistenceExtension (_listeners);
    }

    [Test]
    public void ConnectionOpened ()
    {
      var connectionID = Guid.NewGuid ();
      _innerPersistenceListener1.Expect (mock => mock.ConnectionOpened (connectionID));
      _innerPersistenceListener2.Expect (mock => mock.ConnectionOpened (connectionID));
      _mockRepository.ReplayAll ();

      _extension.ConnectionOpened (connectionID);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ConnectionClosed ()
    {
      var connectionID = Guid.NewGuid ();
      _innerPersistenceListener1.Expect (mock => mock.ConnectionClosed (connectionID));
      _innerPersistenceListener2.Expect (mock => mock.ConnectionClosed (connectionID));
      _mockRepository.ReplayAll ();

      _extension.ConnectionClosed (connectionID);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionBegan ()
    {
      var connectionID = Guid.NewGuid ();
      var isolationLevel = IsolationLevel.Chaos;
      _innerPersistenceListener1.Expect (mock => mock.TransactionBegan (connectionID, isolationLevel));
      _innerPersistenceListener2.Expect (mock => mock.TransactionBegan (connectionID, isolationLevel));
      _mockRepository.ReplayAll ();

      _extension.TransactionBegan (connectionID, isolationLevel);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionCommitted ()
    {
      var connectionID = Guid.NewGuid ();
      _innerPersistenceListener1.Expect (mock => mock.TransactionCommitted (connectionID));
      _innerPersistenceListener2.Expect (mock => mock.TransactionCommitted (connectionID));
      _mockRepository.ReplayAll ();

      _extension.TransactionCommitted (connectionID);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionRolledBack ()
    {
      var connectionID = Guid.NewGuid ();
      _innerPersistenceListener1.Expect (mock => mock.TransactionRolledBack (connectionID));
      _innerPersistenceListener2.Expect (mock => mock.TransactionRolledBack (connectionID));
      _mockRepository.ReplayAll ();

      _extension.TransactionRolledBack (connectionID);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TransactionDisposed ()
    {
      var connectionID = Guid.NewGuid ();
      _innerPersistenceListener1.Expect (mock => mock.TransactionDisposed (connectionID));
      _innerPersistenceListener2.Expect (mock => mock.TransactionDisposed (connectionID));
      _mockRepository.ReplayAll ();

      _extension.TransactionDisposed (connectionID);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void QueryExecuting ()
    {
      var connectionID = Guid.NewGuid ();
      var queryID = Guid.NewGuid();
      var commandText = "commandText";
      var parameters = _mockRepository.StrictMock<IDictionary<string, object>>();
      
      _innerPersistenceListener1.Expect (mock => mock.QueryExecuting (connectionID, queryID, commandText, parameters));
      _innerPersistenceListener2.Expect (mock => mock.QueryExecuting (connectionID, queryID, commandText, parameters));
      _mockRepository.ReplayAll ();

      _extension.QueryExecuting (connectionID, queryID, commandText, parameters);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void QueryExecuted ()
    {
      var connectionID = Guid.NewGuid ();
      var queryID = Guid.NewGuid ();
      var durationOfQueryExecution = new TimeSpan();

      _innerPersistenceListener1.Expect (mock => mock.QueryExecuted (connectionID, queryID, durationOfQueryExecution));
      _innerPersistenceListener2.Expect (mock => mock.QueryExecuted (connectionID, queryID, durationOfQueryExecution));
      _mockRepository.ReplayAll ();

      _extension.QueryExecuted (connectionID, queryID, durationOfQueryExecution);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void QueryCompleted ()
    {
      var connectionID = Guid.NewGuid ();
      var queryID = Guid.NewGuid ();
      var durationOfDataRead = new TimeSpan ();
      var rowCount = 6;

      _innerPersistenceListener1.Expect (mock => mock.QueryCompleted (connectionID, queryID, durationOfDataRead, rowCount));
      _innerPersistenceListener2.Expect (mock => mock.QueryCompleted (connectionID, queryID, durationOfDataRead, rowCount));
      _mockRepository.ReplayAll ();

      _extension.QueryCompleted (connectionID, queryID, durationOfDataRead, rowCount);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void QueryError ()
    {
      var connectionID = Guid.NewGuid ();
      var queryID = Guid.NewGuid ();
      Exception ex = new Exception();

      _innerPersistenceListener1.Expect (mock => mock.QueryError (connectionID, queryID, ex));
      _innerPersistenceListener2.Expect (mock => mock.QueryError (connectionID, queryID, ex));
      _mockRepository.ReplayAll ();

      _extension.QueryError (connectionID, queryID, ex);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void IsNull ()
    {
      var result = _extension.IsNull;
      Assert.That (result, Is.False);
    }
  }
}