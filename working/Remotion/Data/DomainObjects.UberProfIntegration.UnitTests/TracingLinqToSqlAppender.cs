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
using System.Text;

namespace Remotion.Data.DomainObjects.UberProfIntegration.UnitTests
{
  public class TracingLinqToSqlAppender : MockableLinqToSqlAppender.ILinqToSqlAppender
  {
    private readonly StringBuilder _log = new StringBuilder ();

    public string TraceLog
    {
      get { return _log.ToString(); }
    }

    public void ConnectionStarted (Guid sessionID)
    {
      _log.AppendFormat ("ConnectionStarted ({0})", sessionID);
      _log.AppendLine();
    }

    public void ConnectionDisposed (Guid sessionID)
    {
      _log.AppendFormat ("ConnectionDisposed ({0})", sessionID);
      _log.AppendLine ();
    }

    public void StatementRowCount (Guid sessionID, Guid queryID, int rowCount)
    {
      _log.AppendFormat ("StatementRowCount ({0}, {1}, {2})", sessionID, queryID, rowCount);
      _log.AppendLine ();
    }

    public void StatementError (Guid sessionID, Exception e)
    {
      _log.AppendFormat ("ConnectionDisposed ({0}, {1})", sessionID, e);
      _log.AppendLine ();
    }

    public void CommandDurationAndRowCount (Guid sessionID, long milliseconds, int? rowCount)
    {
      _log.AppendFormat ("CommandDurationAndRowCount ({0}, {1}, {2})", sessionID, milliseconds, (object) rowCount ?? "<null>");
      _log.AppendLine ();
    }

    public void StatementExecuted (Guid sessionID, Guid queryID, string statement)
    {
      _log.AppendFormat ("StatementExecuted ({0}, {1}, {2})", sessionID, queryID, statement);
      _log.AppendLine ();
    }

    public void TransactionBegan (Guid sessionID, IsolationLevel isolationLevel)
    {
      _log.AppendFormat ("TransactionBegan ({0}, {1})", sessionID, isolationLevel);
      _log.AppendLine ();
    }

    public void TransactionCommit (Guid sessionID)
    {
      _log.AppendFormat ("TransactionCommit ({0})", sessionID);
      _log.AppendLine ();
    }

    public void TransactionDisposed (Guid sessionID)
    {
      _log.AppendFormat ("TransactionDisposed ({0})", sessionID);
      _log.AppendLine ();
    }

    public void TransactionRolledBack (Guid sessionID)
    {
      _log.AppendFormat ("TransactionRolledBack ({0})", sessionID);
      _log.AppendLine ();
    }
  }
}