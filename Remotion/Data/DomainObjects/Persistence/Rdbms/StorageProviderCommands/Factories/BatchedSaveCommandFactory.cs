// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;

/// <summary>
/// The <see cref="BatchedSaveCommandFactory"/> is responsible to create batched save commands for a relational database.
/// </summary>
public class BatchedSaveCommandFactory : ISaveCommandFactory
{
  private class InsertTableManipulationDataContainerAccessor : ITableManipulationDataContainerAccessor
  {
    private readonly DataContainer _dataContainer;

    public InsertTableManipulationDataContainerAccessor (DataContainer dataContainer)
    {
      ArgumentNullException.ThrowIfNull(dataContainer);

      _dataContainer = dataContainer;
    }

    public ObjectID GetID () => _dataContainer.ID;

    public object GetTimestamp () => throw new NotSupportedException($"{nameof(InsertTableManipulationDataContainerAccessor)} does not support {nameof(GetTimestamp)}");

    public object? GetValue (PropertyDefinition propertyDefinition)
    {
      // TODO This ensures that no relation is inserted during insert but with RM-9647 this should be changed to a better logic
      if (propertyDefinition.IsObjectID)
        return null;

      return _dataContainer.GetValueWithoutEvents(propertyDefinition);
    }

    public object GetOptionalValue (PropertyDefinition propertyDefinition, object? defaultValue)
    {
      throw new NotSupportedException($"{nameof(InsertTableManipulationDataContainerAccessor)} does not support {nameof(GetOptionalValue)}");
    }

    public bool IsOptionalValueSet (PropertyDefinition propertyDefinition)
    {
      throw new NotSupportedException($"{nameof(InsertTableManipulationDataContainerAccessor)} does not support {nameof(IsOptionalValueSet)}");
    }
  }

  private class LockOrDeleteTableManipulationDataContainerAccessor : ITableManipulationDataContainerAccessor
  {
    private readonly DataContainer _dataContainer;

    public LockOrDeleteTableManipulationDataContainerAccessor (DataContainer dataContainer)
    {
      ArgumentNullException.ThrowIfNull(dataContainer);

      _dataContainer = dataContainer;
    }

    public ObjectID GetID () => _dataContainer.ID;

    public object GetTimestamp () => _dataContainer.Timestamp!;

    public object GetValue (PropertyDefinition propertyDefinition)
    {
      throw new NotSupportedException($"{nameof(LockOrDeleteTableManipulationDataContainerAccessor)} does not support {nameof(GetValue)}");
    }

    public object GetOptionalValue (PropertyDefinition propertyDefinition, object? defaultValue)
    {
      throw new NotSupportedException($"{nameof(LockOrDeleteTableManipulationDataContainerAccessor)} does not support {nameof(GetOptionalValue)}");
    }

    public bool IsOptionalValueSet (PropertyDefinition propertyDefinition)
    {
      throw new NotSupportedException($"{nameof(LockOrDeleteTableManipulationDataContainerAccessor)} does not support {nameof(IsOptionalValueSet)}");
    }
  }

  private class UpdateTableManipulationDataContainerAccessor : ITableManipulationDataContainerAccessor
  {
    private readonly DataContainer _dataContainer;
    private readonly bool _isNewDataContainer;

    public UpdateTableManipulationDataContainerAccessor (DataContainer dataContainer)
    {
      ArgumentNullException.ThrowIfNull(dataContainer);

      _dataContainer = dataContainer;
      _isNewDataContainer = dataContainer.State.IsNew;
    }

    public ObjectID GetID () => _dataContainer.ID;

    public object GetTimestamp () => _dataContainer.Timestamp!;

    public object? GetValue (PropertyDefinition propertyDefinition) => _dataContainer.GetValueWithoutEvents(propertyDefinition);

    public object? GetOptionalValue (PropertyDefinition propertyDefinition, object? defaultValue)
    {
      if (IsOptionalValueSet(propertyDefinition))
        return GetValue(propertyDefinition);

      return defaultValue;
    }

    public bool IsOptionalValueSet (PropertyDefinition propertyDefinition)
    {
      // In case the DataContainer is new we do not need to set optional values again
      // because they have already been set by the previous insert.
      if (_isNewDataContainer)
        return false;

      return _dataContainer.HasValueChanged(propertyDefinition);
    }
  }

  private class DataContainerGroup
  {
    public List<DataContainer> ForInsert { get; } = new();
    public List<DataContainer> ForUpdate { get; } = new();
    public List<DataContainer> ForDelete { get; } = new();
  }

  private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
  private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
  private readonly ITableDefinitionFinder _tableDefinitionFinder;
  private readonly ITableManipulationRecordDefinitionProvider _tableManipulationRecordDefinitionProvider;

  public BatchedSaveCommandFactory (
      IDbCommandBuilderFactory dbCommandBuilderFactory,
      IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
      ITableDefinitionFinder tableDefinitionFinder,
      ITableManipulationRecordDefinitionProvider tableManipulationRecordDefinitionProvider)
  {
    ArgumentNullException.ThrowIfNull(dbCommandBuilderFactory);
    ArgumentNullException.ThrowIfNull(rdbmsPersistenceModelProvider);
    ArgumentNullException.ThrowIfNull(tableDefinitionFinder);
    ArgumentNullException.ThrowIfNull(tableManipulationRecordDefinitionProvider);

    _dbCommandBuilderFactory = dbCommandBuilderFactory;
    _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
    _tableDefinitionFinder = tableDefinitionFinder;
    _tableManipulationRecordDefinitionProvider = tableManipulationRecordDefinitionProvider;
  }

  public IDbCommandBuilderFactory DbCommandBuilderFactory => _dbCommandBuilderFactory;

  public IRdbmsPersistenceModelProvider RdbmsPersistenceModelProvider => _rdbmsPersistenceModelProvider;

  public ITableDefinitionFinder TableDefinitionFinder => _tableDefinitionFinder;

  public virtual IRdbmsProviderCommand CreateForSave (IEnumerable<DataContainer> dataContainers)
  {
    ArgumentNullException.ThrowIfNull(dataContainers);

    return new CompoundRdbmsProviderCommand(CreateCommands(dataContainers));
  }

  protected virtual bool ShouldCreateInsertCommand (DataContainer dataContainer)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    return dataContainer.State.IsNew;
  }

  protected virtual bool ShouldCreateDeleteCommand (DataContainer dataContainer)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    return dataContainer.State.IsDeleted;
  }

  protected virtual bool ShouldCreateUpdateCommand (DataContainer dataContainer)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    if (dataContainer.State.IsChanged && dataContainer.State.IsPersistentDataChanged)
      return true;

    if (dataContainer.State.IsNew || dataContainer.State.IsDeleted)
    {
      // TODO: This will be removed/refactored when ordering tables is implemented because it is no longer required for new or deleted datacontainers
      return dataContainer.ClassDefinition.GetPropertyDefinitions().Any(pd => pd.StorageClass == StorageClass.Persistent && pd.IsObjectID);
    }

    return false;
  }

  private IEnumerable<IRdbmsProviderCommand> CreateCommands (IEnumerable<DataContainer> dataContainers)
  {
    var groupedDataContainers = GetGroupedDataContainer(dataContainers);

    var allDataContainersForLocking = new List<DataContainer>();
    var lockCommandSpecifications = new List<IBatchedCommandSpecification>();

    var allDataContainersForInsert = new List<DataContainer>();
    var insertCommandSpecifications = new List<IBatchedCommandSpecification>();

    var allDataContainersForUpdate = new List<DataContainer>();
    var updateCommandSpecifications = new List<IBatchedCommandSpecification>();

    var allDataContainersForDelete = new List<DataContainer>();
    var deleteCommandSpecifications = new List<IBatchedCommandSpecification>();

    foreach (var kvp in groupedDataContainers)
    {
      var tableDefinition = kvp.Key;
      var dataContainerGroup = kvp.Value;

      var dataContainersForLock = dataContainerGroup.ForUpdate
          .Union(dataContainerGroup.ForDelete)
          .Where(d => !d.State.IsNew)
          .ToArray();
      if (dataContainersForLock.Length > 0)
      {
        allDataContainersForLocking.AddRange(dataContainersForLock);
        lockCommandSpecifications.Add(CreateLockCommandSpecification(tableDefinition, dataContainersForLock));
      }

      if (dataContainerGroup.ForInsert.Count > 0)
      {
        allDataContainersForInsert.AddRange(dataContainerGroup.ForInsert);
        insertCommandSpecifications.Add(CreateInsertCommandSpecification(tableDefinition, dataContainerGroup.ForInsert));
      }

      if (dataContainerGroup.ForUpdate.Count > 0)
      {
        allDataContainersForUpdate.AddRange(dataContainerGroup.ForUpdate);
        updateCommandSpecifications.Add(CreateUpdateCommandSpecification(tableDefinition, dataContainerGroup.ForUpdate));
      }

      if (dataContainerGroup.ForDelete.Count > 0)
      {
        allDataContainersForDelete.AddRange(dataContainerGroup.ForDelete);
        deleteCommandSpecifications.Add(CreateDeleteCommandSpecification(tableDefinition, dataContainerGroup.ForDelete));
      }
    }

    // The order of the Commands is important!
    // 1. Locks
    // 2. Inserts
    // 3. Updates
    // 4. Deletes
    if (allDataContainersForLocking.Count > 0)
    {
      var lockCommandBuilder = _dbCommandBuilderFactory.CreateForBatchedLock(lockCommandSpecifications);
      yield return new BatchedLockRdbmsProviderCommand(lockCommandBuilder, allDataContainersForLocking);
    }

    if (allDataContainersForInsert.Count > 0)
    {
      var insertCommandBuilder = _dbCommandBuilderFactory.CreateForBatchedInsert(insertCommandSpecifications);
      yield return new BatchedObjectsRdbmsProviderCommand(insertCommandBuilder, allDataContainersForInsert);
    }

    if (allDataContainersForUpdate.Count > 0)
    {
      var updateCommandBuilder = _dbCommandBuilderFactory.CreateForBatchedUpdate(updateCommandSpecifications);
      yield return new BatchedObjectsRdbmsProviderCommand(updateCommandBuilder, allDataContainersForUpdate);
    }

    if (allDataContainersForDelete.Count > 0)
    {
      var deleteCommandBuilder = _dbCommandBuilderFactory.CreateForBatchedDelete(deleteCommandSpecifications);
      yield return new BatchedObjectsRdbmsProviderCommand(deleteCommandBuilder, allDataContainersForDelete);
    }
  }

  private IDictionary<TableDefinition, DataContainerGroup> GetGroupedDataContainer (IEnumerable<DataContainer> dataContainers)
  {
    var group = new Dictionary<TableDefinition, DataContainerGroup>();

    foreach (var dataContainer in dataContainers)
    {
      var tableDefinition = _tableDefinitionFinder.GetTableDefinition(dataContainer.ID);
      if (!group.TryGetValue(tableDefinition, out var currentGroup))
      {
        currentGroup = new DataContainerGroup();
        group.Add(tableDefinition, currentGroup);
      }

      if (ShouldCreateInsertCommand(dataContainer))
        currentGroup.ForInsert.Add(dataContainer);
      else if (ShouldCreateDeleteCommand(dataContainer))
        currentGroup.ForDelete.Add(dataContainer);

      if (ShouldCreateUpdateCommand(dataContainer))
        currentGroup.ForUpdate.Add(dataContainer);
    }

    return group;
  }

  private IBatchedCommandSpecification CreateLockCommandSpecification (TableDefinition tableDefinition, DataContainer[] dataContainers)
  {
    return CreateCommandSpecification(
        tableDefinition,
        dataContainers,
        (provider, classDefinition) => provider.GetLockRecordDefinition(classDefinition),
        d => new LockOrDeleteTableManipulationDataContainerAccessor(d));
  }

  private IBatchedCommandSpecification CreateInsertCommandSpecification (TableDefinition tableDefinition, List<DataContainer> dataContainers)
  {
    return CreateCommandSpecification(
        tableDefinition,
        dataContainers,
        (provider, classDefinition) => provider.GetInsertRecordDefinition(classDefinition),
        d => new InsertTableManipulationDataContainerAccessor(d));
  }

  private IBatchedCommandSpecification CreateUpdateCommandSpecification (TableDefinition tableDefinition, List<DataContainer> dataContainers)
  {
    return CreateCommandSpecification(
        tableDefinition,
        dataContainers,
        (provider, classDefinition) => provider.GetUpdateRecordDefinition(classDefinition),
        d => new UpdateTableManipulationDataContainerAccessor(d));
  }

  private IBatchedCommandSpecification CreateDeleteCommandSpecification (TableDefinition tableDefinition, List<DataContainer> dataContainers)
  {
    return CreateCommandSpecification(
        tableDefinition,
        dataContainers,
        (provider, classDefinition) => provider.GetDeleteRecordDefinition(classDefinition),
        d => new LockOrDeleteTableManipulationDataContainerAccessor(d));
  }

  private IBatchedCommandSpecification CreateCommandSpecification (
      TableDefinition tableDefinition,
      IReadOnlyList<DataContainer> dataContainers,
      Func<ITableManipulationRecordDefinitionProvider, ClassDefinition, RecordDefinition> getRecordDefinitionFunc,
      Func<DataContainer, ITableManipulationDataContainerAccessor> tableManipulationDataContainerAccessorFactory)
  {
    var parameterDefinition = new MultiClassTableValuedDataParameterDefinition(_tableManipulationRecordDefinitionProvider, getRecordDefinitionFunc, tableManipulationDataContainerAccessorFactory);
    var specification = new BatchedCommandSpecification(tableDefinition, parameterDefinition, dataContainers);
    return specification;
  }
}
