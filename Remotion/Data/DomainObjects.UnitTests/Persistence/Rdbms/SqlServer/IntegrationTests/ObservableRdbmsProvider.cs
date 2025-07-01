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
using System.Data.Common;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  public class ObservableRdbmsProvider : RdbmsProvider
  {
    public interface ICommandExecutionListener
    {
      void OnExecuteReader (DbCommand command, CommandBehavior behavior);
      void OnExecuteScalar (DbCommand command);
      void OnExecuteNonQuery (DbCommand command);
    }

    private readonly ICommandExecutionListener _listener;

    public ObservableRdbmsProvider (
        RdbmsProviderDefinition definition,
        string connectionString,
        IPersistenceExtension persistenceExtension,
        IRdbmsProviderCommandFactory rdbmsProviderCommandFactory,
        Func<DbConnection> connectionFactory,
        ICommandExecutionListener listener)
        : base(definition, connectionString, persistenceExtension, rdbmsProviderCommandFactory, connectionFactory)
    {
      ArgumentNullException.ThrowIfNull(listener);
      _listener = listener;
    }

    public override DbDataReader ExecuteReader (DbCommand command, CommandBehavior behavior)
    {
      _listener.OnExecuteReader(command, behavior);
      return base.ExecuteReader(command, behavior);
    }

    public override object ExecuteScalar (DbCommand command)
    {
      _listener.OnExecuteScalar(command);
      return base.ExecuteScalar(command);
    }

    public override int ExecuteNonQuery (DbCommand command)
    {
      _listener.OnExecuteNonQuery(command);
      return base.ExecuteNonQuery(command);
    }
  }
}
