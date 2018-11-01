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
using Remotion.Data.DomainObjects.Tracing;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class TracingDbTransactionTest
  {
    private MockRepository _mockRepository;
    private IDbTransaction _innerTransactionMock;
    private IPersistenceExtension _extensionMock;
    private Guid _connectionID;
    private IDbTransaction _transaction;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _innerTransactionMock = _mockRepository.StrictMock<IDbTransaction>();
      _extensionMock = _mockRepository.StrictMock<IPersistenceExtension> ();
      _connectionID = Guid.NewGuid ();

      _transaction = new TracingDbTransaction (_innerTransactionMock, _extensionMock, _connectionID);
    }

    [Test]
    public void GetWrappedInstance ()
    {
      var result = ((TracingDbTransaction)_transaction).WrappedInstance;

      Assert.That (result, Is.EqualTo (_innerTransactionMock));
    }

    [Test]
    public void GetConnectionID ()
    {
      var result = ((TracingDbTransaction) _transaction).ConnectionID;

      Assert.That (result, Is.EqualTo (_connectionID));
    }

    [Test]
    public void GetTransactionID ()
    {
      var result = ((TracingDbTransaction) _transaction).TransactionID;

      Assert.That (result, Is.TypeOf (typeof (Guid)));
    }

    [Test]
    public void Dispose ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Dispose());
        _extensionMock.Expect (mock => mock.TransactionDisposed (_connectionID));
      }
      _mockRepository.ReplayAll();

      _transaction.Dispose();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Dispose_DisposedTransaction ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Dispose ());
        _extensionMock.Expect (mock => mock.TransactionDisposed (_connectionID));
        _innerTransactionMock.Expect (mock => mock.Dispose ());
      }
      _mockRepository.ReplayAll ();

      _transaction.Dispose ();
      _transaction.Dispose ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Commit ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Commit ());
        _extensionMock.Expect (mock => mock.TransactionCommitted (_connectionID));
      }
      _mockRepository.ReplayAll();

      _transaction.Commit();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Commit_DisposedTransaction ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Dispose());
        _extensionMock.Expect (mock => mock.TransactionDisposed (_connectionID));
        _innerTransactionMock.Expect (mock => mock.Commit ());
        
      }
      _mockRepository.ReplayAll ();

      _transaction.Dispose();
      _transaction.Commit ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Rollback ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Rollback ());
        _extensionMock.Expect (mock => mock.TransactionRolledBack (_connectionID));
      }
      _mockRepository.ReplayAll ();

      _transaction.Rollback ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Rollback_DisposedTransaction ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerTransactionMock.Expect (mock => mock.Dispose ());
        _extensionMock.Expect (mock => mock.TransactionDisposed (_connectionID));
        _innerTransactionMock.Expect (mock => mock.Rollback ());

      }
      _mockRepository.ReplayAll ();

      _transaction.Dispose ();
      _transaction.Rollback ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetConnection ()
    {
      var dbConnection = _mockRepository.StrictMock<IDbConnection>();
      _innerTransactionMock.Expect (mock => mock.Connection).Return (dbConnection);
      _mockRepository.ReplayAll();

      var result = _transaction.Connection;

      Assert.That (result, Is.EqualTo (dbConnection));
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetIsolationLevel ()
    {
      var isolationLevel = IsolationLevel.Chaos;
      _innerTransactionMock.Expect (mock => mock.IsolationLevel).Return (isolationLevel);
      _mockRepository.ReplayAll ();

      var result = _transaction.IsolationLevel;

      Assert.That (result, Is.EqualTo (isolationLevel));
      _mockRepository.VerifyAll ();
    }
  }
}