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

namespace Remotion.Data.DomainObjects.UberProfIntegration.UnitTests
{
  public class MockableLinqToSqlAppender
  {
    public interface ILinqToSqlAppender
    {
      void ConnectionStarted (Guid sessionID);
      void ConnectionDisposed (Guid sessionID);
      void StatementRowCount (Guid sessionID, Guid queryID, int rowCount);
      void StatementError (Guid sessionID, Exception e);
      void CommandDurationAndRowCount (Guid sessionID, long milliseconds, int? rowCount);
      void StatementExecuted (Guid sessionID, Guid queryID, string statement);
      void TransactionBegan (Guid sessionID, IsolationLevel isolationLevel);
      void TransactionCommit (Guid sessionID);
      void TransactionDisposed (Guid sessionID);
      void TransactionRolledBack (Guid sessionID);
    }

    private readonly string _name;

    public MockableLinqToSqlAppender (string name)
    {
      _name = name;
    }

    public string Name
    {
      get { return _name; }
    }

    public ILinqToSqlAppender AppenderMock { get; set; }

    public void ConnectionStarted (Guid sessionID)
    {
      if (AppenderMock != null)
        AppenderMock.ConnectionStarted (sessionID);
    }

    public void ConnectionDisposed (Guid sessionID)
    {
      if (AppenderMock != null)
        AppenderMock.ConnectionDisposed (sessionID);
    }

    public void StatementRowCount (Guid sessionID, Guid queryID, int rowCount)
    {
      if (AppenderMock != null)
        AppenderMock.StatementRowCount (sessionID, queryID, rowCount);
    }

    public void StatementError (Guid sessionID, Exception e)
    {
      if (AppenderMock != null)
        AppenderMock.StatementError (sessionID, e);
    }

    public void CommandDurationAndRowCount (Guid sessionID, long milliseconds, int? rowCount)
    {
      if (AppenderMock != null)
        AppenderMock.CommandDurationAndRowCount (sessionID, milliseconds, rowCount);
    }

    public void StatementExecuted (Guid sessionID, Guid queryID, string statement)
    {
      if (AppenderMock != null)
        AppenderMock.StatementExecuted (sessionID, queryID, statement);
    }

    public void TransactionBegan (Guid sessionID, IsolationLevel isolationLevel)
    {
      if (AppenderMock != null)
        AppenderMock.TransactionBegan (sessionID, isolationLevel);
    }

    public void TransactionCommit (Guid sessionID)
    {
      if (AppenderMock != null)
        AppenderMock.TransactionCommit (sessionID);
    }

    public void TransactionDisposed (Guid sessionID)
    {
      if (AppenderMock != null)
        AppenderMock.TransactionDisposed (sessionID);
    }

    public void TransactionRolledBack (Guid sessionID)
    {
      if (AppenderMock != null)
        AppenderMock.TransactionRolledBack (sessionID);
    }
  }
}