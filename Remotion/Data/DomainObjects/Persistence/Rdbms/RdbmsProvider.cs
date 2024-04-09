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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class RdbmsProvider : StorageProvider, IRdbmsProviderCommandExecutionContext
  {
    private readonly string _connectionString;
    private readonly IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderCommandFactory;
    private readonly Func<IDbConnection> _connectionFactory;

    private TracingDbConnection? _connection;
    private TracingDbTransaction? _transaction;

    public RdbmsProvider (
        RdbmsProviderDefinition definition,
        string connectionString,
        IPersistenceExtension persistenceExtension,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> storageProviderCommandFactory,
        Func<IDbConnection> connectionFactory)
        : base(definition, persistenceExtension)
    {
      ArgumentUtility.CheckNotNullOrEmpty("connectionString", connectionString);
      ArgumentUtility.CheckNotNull("storageProviderCommandFactory", storageProviderCommandFactory);
      ArgumentUtility.CheckNotNull("connectionFactory", connectionFactory);

      if (connectionString != definition.ConnectionString && connectionString != definition.ReadOnlyConnectionString)
      {
        throw CreateArgumentException(
            "connectionString",
            "The connection string '{0}' is not defined by provider '{1}'",
            connectionString,
            definition.Name);
      }

      _connectionString = connectionString;
      _storageProviderCommandFactory = storageProviderCommandFactory;
      _connectionFactory = connectionFactory;
    }

    public new RdbmsProviderDefinition StorageProviderDefinition
    {
      get
      {
        // CheckDisposed is not necessary here, because StorageProvider.StorageProviderDefinition already checks this.
        return (RdbmsProviderDefinition)base.StorageProviderDefinition;
      }
    }

    [MemberNotNullWhen(true, nameof(_connection))]
    public virtual bool IsConnected
    {
      get
      {
        if (_connection == null)
          return false;

        return _connection.State != ConnectionState.Closed;
      }
    }

    public string ConnectionString
    {
      get { return _connectionString; }
    }

    public TracingDbConnection? Connection
    {
      get
      {
        CheckDisposed();
        return _connection;
      }
    }

    public TracingDbTransaction? Transaction
    {
      get
      {
        CheckDisposed();
        return _transaction;
      }
    }

    public virtual IsolationLevel IsolationLevel
    {
      get { return IsolationLevel.Serializable; }
    }

    protected IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> StorageProviderCommandFactory
    {
      get { return _storageProviderCommandFactory; }
    }

    public override void Dispose ()
    {
      if (!IsDisposed)
      {
        try
        {
          DisposeTransaction();
          DisposeConnection();
        }
        finally
        {
          base.Dispose();
        }
      }
    }

    [MemberNotNull(nameof(_connection))]
    public virtual void Connect ()
    {
      CheckDisposed();

      if (!IsConnected)
      {
        _connection = new TracingDbConnection(CreateConnection(), PersistenceExtension);
        if (string.IsNullOrEmpty(_connection.ConnectionString))
          _connection.ConnectionString = _connectionString;

        try
        {
          _connection.Open();
        }
        catch (Exception e)
        {
          throw CreateRdbmsProviderException(e, "Error while opening connection.");
        }
      }
    }

    public virtual void Disconnect ()
    {
      Dispose();
    }

    public override void BeginTransaction ()
    {
      CheckDisposed();

      Connect();

      if (_transaction != null)
        throw new InvalidOperationException("Cannot call BeginTransaction when a transaction is already in progress.");

      try
      {
        _transaction = _connection.BeginTransaction(IsolationLevel);
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException(e, "Error while executing BeginTransaction.");
      }
    }

    public override void Commit ()
    {
      CheckDisposed();

      if (_transaction == null)
        throw new InvalidOperationException("Commit cannot be called without calling BeginTransaction first.");

      try
      {
        _transaction.Commit();
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException(e, "Error while executing Commit.");
      }
      finally
      {
        DisposeTransaction();
      }
    }

    public override void Rollback ()
    {
      CheckDisposed();

      if (_transaction == null)
        throw new InvalidOperationException("Rollback cannot be called without calling BeginTransaction first.");

      try
      {
        _transaction.Rollback();
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException(e, "Error while executing Rollback.");
      }
      finally
      {
        DisposeTransaction();
      }
    }

    public override IEnumerable<DataContainer?> ExecuteCollectionQuery (IQuery query)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("query", query);
      CheckQuery(query, QueryType.Collection, "query");

      Connect();

      var command = _storageProviderCommandFactory.CreateForDataContainerQuery(query);
      var dataContainers = command.Execute(this);

      var checkedSequence = CheckForDuplicates(dataContainers, "database query");
      return checkedSequence.ToArray();
    }

    public override IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("query", query);
      CheckQuery(query, QueryType.Custom, "query");

      Connect();

      var command = _storageProviderCommandFactory.CreateForCustomQuery(query);
      return command.Execute(this);
    }

    public override object? ExecuteScalarQuery (IQuery query)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("query", query);
      CheckQuery(query, QueryType.Scalar, "query");

      Connect();

      var command = _storageProviderCommandFactory.CreateForScalarQuery(query);
      return command.Execute(this);
    }

    public override ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("id", id);
      CheckStorageProvider(id, "id");

      Connect();

      var command = _storageProviderCommandFactory.CreateForSingleIDLookup(id);
      return command.Execute(this);
    }

    public override IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IReadOnlyCollection<ObjectID> ids)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("ids", ids);

      Connect();

      var checkedIDs = ids.Select(id => CheckStorageProvider(id, "ids"));
      var command = _storageProviderCommandFactory.CreateForSortedMultiIDLookup(checkedIDs);
      return command.Execute(this);
    }

    public override IEnumerable<DataContainer> LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition,
        SortExpressionDefinition? sortExpressionDefinition,
        ObjectID relatedID)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull("relatedID", relatedID);
      CheckClassDefinition(relationEndPointDefinition.ClassDefinition, "classDefinition");

      Connect();

      if (relationEndPointDefinition.PropertyDefinition.StorageClass == StorageClass.Transaction)
        return new DataContainerCollection();

      var storageProviderCommand = _storageProviderCommandFactory.CreateForRelationLookup(
          relationEndPointDefinition,
          relatedID,
          sortExpressionDefinition);
      var dataContainers = storageProviderCommand.Execute(this);

      var checkedSequence = CheckForNulls(CheckForDuplicates(dataContainers, "relation lookup"), "relation lookup");
      return new DataContainerCollection(checkedSequence, true);
    }

    public override void Save (IReadOnlyCollection<DataContainer> dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      Connect();

      var saveCommand = _storageProviderCommandFactory.CreateForSave(dataContainers);
      saveCommand.Execute(this);
    }

    public override void UpdateTimestamps (IReadOnlyCollection<DataContainer> dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      Connect();

      var objectIds = dataContainers.Select(dc => dc.ID);
      var multiTimestampLookupCommand = _storageProviderCommandFactory.CreateForMultiTimestampLookup(objectIds);
      var timestampDictionary = multiTimestampLookupCommand.Execute(this).ToDictionary(result => result.ObjectID, result => result.LocatedObject);

      foreach (var dataContainer in dataContainers)
      {
        if (!timestampDictionary.TryGetValue(dataContainer.ID, out var timestamp))
          throw new RdbmsProviderException(string.Format("No timestamp found for object '{0}'.", dataContainer.ID));

        // TODO RM-8523: guard against null-values of timestamp

        dataContainer.SetTimestamp(timestamp);
      }
    }

    public override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      CheckClassDefinition(classDefinition, "classDefinition");

      return new ObjectID(classDefinition.ID, Guid.NewGuid());
    }

    public virtual TracingDbCommand CreateDbCommand ()
    {
      CheckDisposed();

      if (!IsConnected)
        throw new InvalidOperationException("Connect must be called before a command can be created.");

      var command = _connection.CreateCommand();

      try
      {
        command.SetInnerConnection(_connection);
        command.SetInnerTransaction(_transaction);
      }
      catch (Exception)
      {
        command.Dispose();
        throw;
      }

      return command;
    }

    IDbCommand IRdbmsProviderCommandExecutionContext.CreateDbCommand ()
    {
      return CreateDbCommand();
    }

    public virtual IDataReader ExecuteReader (IDbCommand command, CommandBehavior behavior)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("command", command);
      ArgumentUtility.CheckValidEnumValue("behavior", behavior);

      try
      {
        return command.ExecuteReader(behavior);
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException(e, "Error while executing SQL command: " + e.Message);
      }
    }

    public virtual object? ExecuteScalar (IDbCommand command)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("command", command);

      try
      {
        return command.ExecuteScalar();
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException(e, "Error while executing SQL command: " + e.Message);
      }
    }

    public virtual int ExecuteNonQuery (IDbCommand command)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("command", command);

      try
      {
        return command.ExecuteNonQuery();
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException(e, "Error while executing SQL command: " + e.Message);
      }
    }

    protected IDbConnection CreateConnection ()
    {
      return _connectionFactory();
    }

    protected RdbmsProviderException CreateRdbmsProviderException (Exception innerException, string formatString, params object[] args)
    {
      var message = string.Format(formatString, args);
      return new RdbmsProviderException(message, innerException);
    }

    private void DisposeTransaction ()
    {
      if (_transaction != null)
        _transaction.Dispose();

      _transaction = null;
    }

    private void DisposeConnection ()
    {
      if (_connection != null)
        _connection.Close();

      _connection = null;
    }

    private ObjectID CheckStorageProvider (ObjectID id, string argumentName)
    {
      if (id.StorageProviderDefinition != StorageProviderDefinition)
      {
        throw CreateArgumentException(
            argumentName,
            "The StorageProviderID '{0}' of the provided ObjectID '{1}' does not match with this StorageProvider's ID '{2}'.",
            id.StorageProviderDefinition.Name,
            id,
            StorageProviderDefinition.Name);
      }
      return id;
    }

    private void CheckClassDefinition (ClassDefinition classDefinition, string argumentName)
    {
      if (classDefinition.StorageEntityDefinition.StorageProviderDefinition != StorageProviderDefinition)
      {
        throw CreateArgumentException(
            argumentName,
            "The StorageProviderID '{0}' of the provided ClassDefinition does not match with this StorageProvider's ID '{1}'.",
            classDefinition.StorageEntityDefinition.StorageProviderDefinition.Name,
            StorageProviderDefinition.Name);
      }
    }

    private IEnumerable<DataContainer?> CheckForDuplicates (IEnumerable<DataContainer?> dataContainers, string operation)
    {
      var loadedIDs = new HashSet<ObjectID>();
      foreach (var dataContainer in dataContainers)
      {
        if (dataContainer != null)
        {
          if (loadedIDs.Contains(dataContainer.ID))
          {
            var message = string.Format("A {0} returned duplicates of object '{1}', which is not allowed.", operation, dataContainer.ID);
            throw new RdbmsProviderException(message);
          }
          loadedIDs.Add(dataContainer.ID);
        }

        yield return dataContainer;
      }
    }

    private IEnumerable<DataContainer> CheckForNulls (IEnumerable<DataContainer?> dataContainers, string operation)
    {
      foreach (var dataContainer in dataContainers)
      {
        if (dataContainer == null)
          throw new RdbmsProviderException(string.Format("A {0} returned a NULL ID, which is not allowed.", operation));

        yield return dataContainer;
      }
    }
  }
}
