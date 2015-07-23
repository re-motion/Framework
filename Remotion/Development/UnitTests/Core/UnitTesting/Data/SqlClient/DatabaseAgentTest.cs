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
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Data.SqlClient
{
  [TestFixture]
  public class DatabaseAgentTest
  {
    private MockRepository _mockRepository;
    private IDbCommand _commandMock;
    private IDbConnection _connectionStub;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();

      _connectionStub = _mockRepository.Stub<IDbConnection> ();
      _commandMock = _mockRepository.StrictMock<IDbCommand> ();

      SetupResult.For (_connectionStub.CreateCommand ()).Return (_commandMock);
    }

    [Test]
    public void ExecuteScalarCommand ()
    {
      SetupCommandExpectations ("my command", null, delegate { Expect.Call (_commandMock.ExecuteScalar ()).Return ("foo"); });

      _mockRepository.ReplayAll();

      TestableDatabaseAgent agent = new TestableDatabaseAgent (_connectionStub);
      object result = agent.ExecuteScalarCommand ("my command");
      Assert.That (result, Is.EqualTo ("foo"));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteCommand_ReturnsCount ()
    {
      SetupCommandExpectations ("my command", null, delegate { Expect.Call (_commandMock.ExecuteNonQuery()).Return (15); });

      _mockRepository.ReplayAll ();

      TestableDatabaseAgent agent = new TestableDatabaseAgent (_connectionStub);
      int result = agent.ExecuteCommand ("my command");
      Assert.That (result, Is.EqualTo (15));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteBatchString ()
    {
      SetupCommandExpectations ("ABCDEFG", null, delegate { Expect.Call (_commandMock.ExecuteNonQuery ()).Return (5); });
      
      _mockRepository.ReplayAll ();

      TestableDatabaseAgent agent = new TestableDatabaseAgent (_connectionStub);
      int result = agent.ExecuteBatchString (_connectionStub, "ABCDEFG", null);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (5));
    }

    [Test]
    public void ExecuteBatchString_StringIsSplitted ()
    {
      SetupCommandExpectations ("ABC", null, delegate { Expect.Call (_commandMock.ExecuteNonQuery ()).Return (10); });
      SetupCommandExpectations ("GFE", null, delegate { Expect.Call (_commandMock.ExecuteNonQuery ()).Return (20); });
      
      _mockRepository.ReplayAll ();

      TestableDatabaseAgent agent = new TestableDatabaseAgent (_connectionStub);
      int result = agent.ExecuteBatchString (_connectionStub, "ABC\nGO\nGFE", null);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (30));
    }

    [Test]
    public void ExecuteBatchString_EmptyLines ()
    {
      SetupCommandExpectations ("ABC", null, delegate { Expect.Call (_commandMock.ExecuteNonQuery ()).Return (10); });
      SetupCommandExpectations ("GFE", null, delegate { Expect.Call (_commandMock.ExecuteNonQuery ()).Return (20); });

      _mockRepository.ReplayAll ();

      TestableDatabaseAgent agent = new TestableDatabaseAgent (_connectionStub);
      int result = agent.ExecuteBatchString (_connectionStub, "ABC\n\nGO\n\n\nGFE\n\n", null);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (30));
    }

    private void SetupCommandExpectations (string commandText, IDbTransaction transaction, Action actualCommandExpectation)
    {
      using (_mockRepository.Ordered ())
      {
        using (_mockRepository.Unordered ())
        {
          _commandMock.CommandType = CommandType.Text;
          _commandMock.CommandText = commandText;
          _commandMock.Transaction = transaction;
        }

        actualCommandExpectation ();
        _commandMock.Dispose ();
      }
    }
  }
}
