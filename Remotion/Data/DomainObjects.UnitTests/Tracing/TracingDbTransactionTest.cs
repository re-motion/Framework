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
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class TracingDbTransactionTest
  {
    private Mock<IDbTransaction> _innerTransactionMock;
    private Mock<IPersistenceExtension> _extensionMock;
    private Guid _connectionID;
    private IDbTransaction _transaction;

    [SetUp]
    public void SetUp ()
    {
      _innerTransactionMock = new Mock<IDbTransaction> (MockBehavior.Strict);
      _extensionMock = new Mock<IPersistenceExtension> (MockBehavior.Strict);
      _connectionID = Guid.NewGuid();

      _transaction = new TracingDbTransaction(_innerTransactionMock.Object, _extensionMock.Object, _connectionID);
    }

    [Test]
    public void GetWrappedInstance ()
    {
      var result = ((TracingDbTransaction)_transaction).WrappedInstance;

      Assert.That(result, Is.EqualTo(_innerTransactionMock.Object));
    }

    [Test]
    public void GetConnectionID ()
    {
      var result = ((TracingDbTransaction)_transaction).ConnectionID;

      Assert.That(result, Is.EqualTo(_connectionID));
    }

    [Test]
    public void GetTransactionID ()
    {
      var result = ((TracingDbTransaction)_transaction).TransactionID;

      Assert.That(result, Is.TypeOf(typeof(Guid)));
    }

    [Test]
    public void Dispose ()
    {
      var sequence = new MockSequence();
      _innerTransactionMock.InSequence (sequence).Setup (mock => mock.Dispose()).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.TransactionDisposed (_connectionID)).Verifiable();

      _transaction.Dispose();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void Dispose_DisposedTransaction ()
    {
      var sequence = new MockSequence();
      _innerTransactionMock.InSequence (sequence).Setup (mock => mock.Dispose()).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.TransactionDisposed (_connectionID)).Verifiable();
      _innerTransactionMock.InSequence (sequence).Setup (mock => mock.Dispose()).Verifiable();

      _transaction.Dispose();
      _transaction.Dispose();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void Commit ()
    {
      var sequence = new MockSequence();
      _innerTransactionMock.InSequence (sequence).Setup (mock => mock.Commit()).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.TransactionCommitted (_connectionID)).Verifiable();

      _transaction.Commit();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void Commit_DisposedTransaction ()
    {
      var sequence = new MockSequence();
      _innerTransactionMock.InSequence (sequence).Setup (mock => mock.Dispose()).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.TransactionDisposed (_connectionID)).Verifiable();
      _innerTransactionMock.InSequence (sequence).Setup (mock => mock.Commit()).Verifiable();

      _transaction.Dispose();
      _transaction.Commit();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void Rollback ()
    {
      var sequence = new MockSequence();
      _innerTransactionMock.InSequence (sequence).Setup (mock => mock.Rollback()).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.TransactionRolledBack (_connectionID)).Verifiable();

      _transaction.Rollback();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void Rollback_DisposedTransaction ()
    {
      var sequence = new MockSequence();
      _innerTransactionMock.InSequence (sequence).Setup (mock => mock.Dispose()).Verifiable();
      _extensionMock.InSequence (sequence).Setup (mock => mock.TransactionDisposed (_connectionID)).Verifiable();
      _innerTransactionMock.InSequence (sequence).Setup (mock => mock.Rollback()).Verifiable();

      _transaction.Dispose();
      _transaction.Rollback();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetConnection ()
    {
      var dbConnection = new Mock<IDbConnection> (MockBehavior.Strict);
      _innerTransactionMock.Setup (mock => mock.Connection).Returns (dbConnection.Object).Verifiable();

      var result = _transaction.Connection;

      Assert.That(result, Is.EqualTo(dbConnection.Object));
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      dbConnection.Verify();
    }

    [Test]
    public void GetIsolationLevel ()
    {
      var isolationLevel = IsolationLevel.Chaos;
      _innerTransactionMock.Setup (mock => mock.IsolationLevel).Returns (isolationLevel).Verifiable();

      var result = _transaction.IsolationLevel;

      Assert.That(result, Is.EqualTo(isolationLevel));
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
    }
  }
}
