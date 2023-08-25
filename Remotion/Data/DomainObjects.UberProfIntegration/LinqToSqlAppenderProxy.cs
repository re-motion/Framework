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
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UberProfIntegration
{
  /// <summary>
  /// Implements a strong-typed wrapper for <b><a href="http://l2sprof.com/">Linq to Sql Profiler</a></b>. (Tested for build 661)
  /// </summary>
  /// <remarks>
  /// The wrapper uses runtime-binding to redirect the calls to Linq to Sql Profiler's API. This removes the static dependcy on Linq to Sql Profiler.
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  [Serializable]
  public sealed class LinqToSqlAppenderProxy
#pragma warning disable SYSLIB0050
      : IObjectReference
#pragma warning restore SYSLIB0050
  {
    private static readonly DoubleCheckedLockingContainer<LinqToSqlAppenderProxy> s_instance =
        new DoubleCheckedLockingContainer<LinqToSqlAppenderProxy>(() => new LinqToSqlAppenderProxy(
            "re-store ClientTransaction",
            Type.GetType("HibernatingRhinos.Profiler.Appender.LinqToSql.LinqToSqlProfiler, HibernatingRhinos.Profiler.Appender", throwOnError: true, ignoreCase: false)!,
            Type.GetType("HibernatingRhinos.Profiler.Appender.LinqToSql.LinqToSqlAppender, HibernatingRhinos.Profiler.Appender", throwOnError: true, ignoreCase: false)!));

    public static LinqToSqlAppenderProxy Instance => s_instance.Value;

    [NonSerialized]
    private readonly object _linqToSqlAppender;

    [NonSerialized]
    private readonly Action<Guid> _connectionStarted;

    [NonSerialized]
    private readonly Action<Guid> _connectionDisposed;

    [NonSerialized]
    private readonly Action<Guid, Guid, int> _statementRowCount;

    [NonSerialized]
    private readonly Action<Guid, Exception> _statementError;

    [NonSerialized]
    private readonly Action<Guid, long, int?> _commandDurationAndRowCount;

    [NonSerialized]
    private readonly Action<Guid, Guid, string> _statementExecuted;

    [NonSerialized]
    private readonly Action<Guid, IsolationLevel> _transactionBegan;

    [NonSerialized]
    private readonly Action<Guid> _transactionCommit;

    [NonSerialized]
    private readonly Action<Guid> _transactionDisposed;

    [NonSerialized]
    private readonly Action<Guid> _transactionRolledBack;

    private LinqToSqlAppenderProxy (string name, Type linqToSqlProfilerType, Type linqToSqlAppenderType)
    {
      ArgumentUtility.CheckNotNull("name", name);

      var initialize = LateBoundDelegateFactory.CreateDelegate<Action>(linqToSqlProfilerType, "Initialize");
      initialize();

      var getAppender = LateBoundDelegateFactory.CreateDelegate(
          linqToSqlProfilerType, "GetAppender", typeof(Func<,>).MakeGenericType(typeof(string), linqToSqlAppenderType));
      _linqToSqlAppender = Assertion.IsNotNull(getAppender.DynamicInvoke(name), "getAppender.DynamicInvoke(\"{0}\") != null", name);

      _connectionStarted = LateBoundDelegateFactory.CreateDelegate<Action<Guid>>(_linqToSqlAppender, "ConnectionStarted");
      _connectionDisposed = LateBoundDelegateFactory.CreateDelegate<Action<Guid>>(_linqToSqlAppender, "ConnectionDisposed");
      _statementRowCount = LateBoundDelegateFactory.CreateDelegate<Action<Guid, Guid, int>>(_linqToSqlAppender, "StatementRowCount");
      _statementError = LateBoundDelegateFactory.CreateDelegate<Action<Guid, Exception>>(_linqToSqlAppender, "StatementError");
      _commandDurationAndRowCount = LateBoundDelegateFactory.CreateDelegate<Action<Guid, long, int?>>(_linqToSqlAppender, "CommandDurationAndRowCount");
      _statementExecuted = LateBoundDelegateFactory.CreateDelegate<Action<Guid, Guid, string>>(_linqToSqlAppender, "StatementExecuted");
      _transactionBegan = LateBoundDelegateFactory.CreateDelegate<Action<Guid, IsolationLevel>>(_linqToSqlAppender, "TransactionBegan");
      _transactionCommit = LateBoundDelegateFactory.CreateDelegate<Action<Guid>>(_linqToSqlAppender, "TransactionCommit");
      _transactionDisposed = LateBoundDelegateFactory.CreateDelegate<Action<Guid>>(_linqToSqlAppender, "TransactionDisposed");
      _transactionRolledBack = LateBoundDelegateFactory.CreateDelegate<Action<Guid>>(_linqToSqlAppender, "TransactionRolledBack");
    }

    public object LinqToSqlAppender
    {
      get { return _linqToSqlAppender; }
    }

    public void ConnectionStarted (Guid sessionID)
    {
      _connectionStarted(sessionID);
    }

    public void ConnectionDisposed (Guid sessionID)
    {
      _connectionDisposed(sessionID);
    }

    public void StatementRowCount (Guid sessionID, Guid queryID, int rowCount)
    {
      _statementRowCount(sessionID, queryID, rowCount);
    }

    public void StatementError (Guid sessionID, Exception e)
    {
      _statementError(sessionID, e);
    }

    public void CommandDurationAndRowCount (Guid sessionID, long milliseconds, int? rowCount)
    {
      _commandDurationAndRowCount(sessionID, milliseconds, rowCount);
    }

    public void StatementExecuted (Guid sessionID, Guid queryID, string statement)
    {
      _statementExecuted(sessionID, queryID, statement);
    }

    public void TransactionBegan (Guid sessionID, IsolationLevel isolationLevel)
    {
      _transactionBegan(sessionID, isolationLevel);
    }

    public void TransactionCommit (Guid sessionID)
    {
      _transactionCommit(sessionID);
    }

    public void TransactionDisposed (Guid sessionID)
    {
      _transactionDisposed(sessionID);
    }

    public void TransactionRolledBack (Guid sessionID)
    {
      _transactionRolledBack(sessionID);
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return Instance;
    }
  }
}
