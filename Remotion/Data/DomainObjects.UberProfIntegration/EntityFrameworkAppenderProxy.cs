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
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UberProfIntegration
{
  /// <summary>
  /// Implements a strong-typed wrapper for <b><a href="https://hibernatingrhinos.com/products/efprof">Entity Framework Profiler</a></b>.
  /// </summary>
  /// <remarks>
  /// The wrapper uses runtime-binding to redirect the calls to Entity Framework Profiler's API. This removes the static dependency on Entity Framework Profiler.
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  [Serializable]
  public sealed class EntityFrameworkAppenderProxy
#pragma warning disable SYSLIB0050
      : IObjectReference
#pragma warning restore SYSLIB0050
  {
    private static readonly DoubleCheckedLockingContainer<EntityFrameworkAppenderProxy> s_instance =
        new DoubleCheckedLockingContainer<EntityFrameworkAppenderProxy>(
            () => new EntityFrameworkAppenderProxy(
                "re-store ClientTransaction",
                Type.GetType("HibernatingRhinos.Profiler.Appender.ProfilerInfrastructure, HibernatingRhinos.Profiler.Appender", throwOnError: true, ignoreCase: false)!,
                Type.GetType(
                    "HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkAppenderConfiguration, HibernatingRhinos.Profiler.Appender",
                    throwOnError: true,
                    ignoreCase: false)!,
                Type.GetType(
                    "HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkAppender, HibernatingRhinos.Profiler.Appender",
                    throwOnError: true,
                    ignoreCase: false)!));

    public static EntityFrameworkAppenderProxy Instance => s_instance.Value;

    [NonSerialized]
    private readonly object _entityFrameworkAppender;

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

    private EntityFrameworkAppenderProxy (
        string name,
        Type entityFrameworkProfilerType,
        Type entityFrameworkConfigurationType,
        Type entityFrameworkAppenderType)
    {
      ArgumentUtility.CheckNotNull("name", name);

      var configuration = Activator.CreateInstance(entityFrameworkConfigurationType);
      var initializeMethod = entityFrameworkProfilerType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static)!;
      initializeMethod.Invoke(null, new[] { configuration });

      _entityFrameworkAppender = Activator.CreateInstance(entityFrameworkAppenderType, name)!;

      _connectionStarted = LateBoundDelegateFactory.CreateDelegate<Action<Guid>>(_entityFrameworkAppender, "ConnectionStarted");
      _connectionDisposed = LateBoundDelegateFactory.CreateDelegate<Action<Guid>>(_entityFrameworkAppender, "ConnectionDisposed");
      _statementRowCount = LateBoundDelegateFactory.CreateDelegate<Action<Guid, Guid, int>>(_entityFrameworkAppender, "StatementRowCount");
      _statementError = LateBoundDelegateFactory.CreateDelegate<Action<Guid, Exception>>(_entityFrameworkAppender, "StatementError");
      _commandDurationAndRowCount = LateBoundDelegateFactory.CreateDelegate<Action<Guid, long, int?>>(_entityFrameworkAppender, "CommandDurationAndRowCount");
      _statementExecuted = LateBoundDelegateFactory.CreateDelegate<Action<Guid, Guid, string>>(_entityFrameworkAppender, "StatementExecuted");
      _transactionBegan = LateBoundDelegateFactory.CreateDelegate<Action<Guid, IsolationLevel>>(_entityFrameworkAppender, "TransactionBegan");
      _transactionCommit = LateBoundDelegateFactory.CreateDelegate<Action<Guid>>(_entityFrameworkAppender, "TransactionCommit");
      _transactionDisposed = LateBoundDelegateFactory.CreateDelegate<Action<Guid>>(_entityFrameworkAppender, "TransactionDisposed");
      _transactionRolledBack = LateBoundDelegateFactory.CreateDelegate<Action<Guid>>(_entityFrameworkAppender, "TransactionRolledBack");
    }

    public object EntityFrameworkAppender
    {
      get { return _entityFrameworkAppender; }
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
