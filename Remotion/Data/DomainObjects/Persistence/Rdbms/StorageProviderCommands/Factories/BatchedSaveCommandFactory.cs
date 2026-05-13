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
using Remotion.Data.DomainObjects.Persistence.SortingOptimization;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;

/// <summary>
/// The <see cref="BatchedSaveCommandFactory"/> is responsible to create batched save commands for a relational database.
/// </summary>
public class BatchedSaveCommandFactory : ISaveCommandFactory
{
  private class InsertTableManipulationDataContainerAccessor : ITableManipulationDataContainerAccessor
  {
    private readonly DataContainer _dataContainer;
    private readonly bool _hasFollowingUpdateForForeignKeys;

    public InsertTableManipulationDataContainerAccessor (DataContainer dataContainer, bool hasFollowingUpdateForForeignKeys)
    {
      ArgumentNullException.ThrowIfNull(dataContainer);

      _dataContainer = dataContainer;
      _hasFollowingUpdateForForeignKeys = hasFollowingUpdateForForeignKeys;
    }

    public ObjectID GetID () => _dataContainer.ID;

    public object GetTimestamp () => throw new NotSupportedException($"{nameof(InsertTableManipulationDataContainerAccessor)} does not support {nameof(GetTimestamp)}");

    public object? GetValue (PropertyDefinition propertyDefinition)
    {
      if (_hasFollowingUpdateForForeignKeys && propertyDefinition.IsObjectID)
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
    private readonly bool _isDataContainerNewOrDeleted;

    public UpdateTableManipulationDataContainerAccessor (DataContainer dataContainer)
    {
      ArgumentNullException.ThrowIfNull(dataContainer);

      _dataContainer = dataContainer;
      _isDataContainerNewOrDeleted = dataContainer.State.IsNew || dataContainer.State.IsDeleted;
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
      // In case the DataContainer is new or deleted we do not need to set optional values
      // either they have been set by a preceding insert or the object will be deleted
      if (_isDataContainerNewOrDeleted)
        return false;

      return _dataContainer.HasValueChanged(propertyDefinition);
    }
  }

  private class DataContainerGroup
  {
    public DataContainerGroup (TableDefinition tableDefinition)
    {
      ArgumentNullException.ThrowIfNull(tableDefinition);

      TableDefinition = tableDefinition;
    }
    public TableDefinition TableDefinition { get; }
    public List<DataContainer> ForInsert { get; } = new();
    public List<DataContainer> ForUpdate { get; } = new();
    public List<DataContainer> ForDelete { get; } = new();
    public HashSet<ObjectID> InsertDataContainerIDsWithForeignKeyUpdate { get; } = new();
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

    var enumeratedDataContainer = dataContainers.ToList();
    if (enumeratedDataContainer.Count == 0)
      return new CompoundRdbmsProviderCommand([]);

    var groupedByPersistenceModelProvider = enumeratedDataContainer.Select(d => d.ClassDefinition)
       .Distinct()
       .GroupBy(d => GetTableDefinition(d).PersistenceModelSortingProvider)
       .ToList();

    if (groupedByPersistenceModelProvider.Count > 1)
      throw new InvalidOperationException($"Multiple {nameof(IRdbmsPersistenceModelProvider)} found in the current set of data containers.");

    if (groupedByPersistenceModelProvider.Count == 0)
      throw new InvalidOperationException($"No {nameof(IRdbmsPersistenceModelProvider)} found in the current set of data containers.");

    return CreateForSave(enumeratedDataContainer, groupedByPersistenceModelProvider[0].Key);
  }

  /// <summary>
  ///   This method is used for testing.
  /// </summary>
  private IRdbmsProviderCommand CreateForSave (IEnumerable<DataContainer> dataContainers, IPersistenceModelSortingProvider persistenceModelSortingProvider)
  {
    return new CompoundRdbmsProviderCommand(CreateCommands(dataContainers.ToDictionary(k => k.ID, v => v), persistenceModelSortingProvider));
  }

  private TableDefinition GetTableDefinition (ClassDefinition classDefinition)
  {
    return InlineRdbmsStorageEntityDefinitionVisitor.Visit<TableDefinition>(
        (IRdbmsStorageEntityDefinition)classDefinition.StorageEntityDefinition,
        (table, _) => table,
        (filterView, continuation) => continuation(filterView.BaseEntity),
        (unionView, _) => { throw new InvalidOperationException($"Could not determine {nameof(TableDefinition)} for {classDefinition.ID}"); },
        (emptyView, _) => { throw new InvalidOperationException($"Could not determine {nameof(TableDefinition)} for {classDefinition.ID}"); });
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

    return dataContainer.State.IsChanged && dataContainer.State.IsPersistentDataChanged;
  }

  private IEnumerable<IRdbmsProviderCommand> CreateCommands (
      IReadOnlyDictionary<ObjectID, DataContainer> dataContainers,
      IPersistenceModelSortingProvider persistenceModelSortingProvider)
  {
    var groupedDataContainers = GetGroupedAndOrderedDataContainer(dataContainers, persistenceModelSortingProvider);

    var allDataContainersForLocking = new List<DataContainer>(dataContainers.Count);
    var lockCommandSpecifications = new List<IBatchedCommandSpecification>(groupedDataContainers.Count);

    var allDataContainersForInsert = new List<DataContainer>(dataContainers.Count);
    var insertCommandSpecifications = new List<IBatchedCommandSpecification>(groupedDataContainers.Count);

    var allDataContainersForUpdate = new List<DataContainer>(dataContainers.Count);
    var updateCommandSpecifications = new List<IBatchedCommandSpecification>(groupedDataContainers.Count);

    var allDataContainersForDelete = new List<DataContainer>(dataContainers.Count);
    var deleteCommandSpecifications = new List<IBatchedCommandSpecification>(groupedDataContainers.Count);

    foreach (var dataContainerGroup in groupedDataContainers)
    {
      var dataContainersForLock = dataContainerGroup.ForUpdate
          .Union(dataContainerGroup.ForDelete)
          .Where(d => !d.State.IsNew)
          .ToArray();
      if (dataContainersForLock.Length > 0)
      {
        allDataContainersForLocking.AddRange(dataContainersForLock);
        lockCommandSpecifications.Add(CreateLockCommandSpecification(dataContainerGroup.TableDefinition, dataContainersForLock));
      }

      if (dataContainerGroup.ForInsert.Count > 0)
      {
        allDataContainersForInsert.AddRange(dataContainerGroup.ForInsert);
        insertCommandSpecifications.Add(CreateInsertCommandSpecification(dataContainerGroup));
      }

      if (dataContainerGroup.ForUpdate.Count > 0)
      {
        allDataContainersForUpdate.AddRange(dataContainerGroup.ForUpdate);
        updateCommandSpecifications.Add(CreateUpdateCommandSpecification(dataContainerGroup));
      }

      if (dataContainerGroup.ForDelete.Count > 0)
      {
        allDataContainersForDelete.AddRange(dataContainerGroup.ForDelete);
        deleteCommandSpecifications.Add(CreateDeleteCommandSpecification(dataContainerGroup));
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
      // for deletes we need to reverse the order because if we delete an object which is the foreign key
      // of another object which we want to delete we need to delete the object with the foreign key first.
      deleteCommandSpecifications.Reverse();
      var deleteCommandBuilder = _dbCommandBuilderFactory.CreateForBatchedDelete(deleteCommandSpecifications);
      yield return new BatchedObjectsRdbmsProviderCommand(deleteCommandBuilder, allDataContainersForDelete);
    }
  }

  private List<DataContainerGroup> GetGroupedAndOrderedDataContainer (
      IReadOnlyDictionary<ObjectID, DataContainer> dataContainers,
      IPersistenceModelSortingProvider persistenceModelSortingProvider)
  {
    var group = new Dictionary<TableDefinition, DataContainerGroup>();

    foreach (var dataContainer in dataContainers.Values)
    {
      var tableDefinition = _tableDefinitionFinder.GetTableDefinition(dataContainer.ID);
      if (!group.TryGetValue(tableDefinition, out var currentGroup))
      {
        currentGroup = new DataContainerGroup(tableDefinition);
        group.Add(tableDefinition, currentGroup);
      }

      if (ShouldCreateInsertCommand(dataContainer))
      {
        currentGroup.ForInsert.Add(dataContainer);
        if (IsSubsequentUpdateRequiredForInsert(tableDefinition, dataContainer, dataContainers, persistenceModelSortingProvider))
        {
          currentGroup.ForUpdate.Add(dataContainer);
          currentGroup.InsertDataContainerIDsWithForeignKeyUpdate.Add(dataContainer.ID);
        }
      }
      else if (ShouldCreateDeleteCommand(dataContainer))
      {
        currentGroup.ForDelete.Add(dataContainer);
        if (IsPrecedingUpdateRequiredForDelete(tableDefinition, dataContainer, dataContainers, persistenceModelSortingProvider))
          currentGroup.ForUpdate.Add(dataContainer);
      }
      else if (ShouldCreateUpdateCommand(dataContainer))
      {
        currentGroup.ForUpdate.Add(dataContainer);
      }
    }

    return group.Values.OrderBy(g => persistenceModelSortingProvider.GetSortPosition(g.TableDefinition)).ToList();
  }

  private bool IsSubsequentUpdateRequiredForInsert (
      TableDefinition tableDefinition,
      DataContainer dataContainer,
      IReadOnlyDictionary<ObjectID, DataContainer> dataContainers,
      IPersistenceModelSortingProvider persistenceModelSortingProvider)
  {
    // if the class definition of the data container has not been ordered correctly we
    // must check if any set property would require a following update statement to set foreign keys
    if (persistenceModelSortingProvider.HasBeenSortedCorrectly(tableDefinition))
      return false;

    var foreignKeyRelevantProperties = persistenceModelSortingProvider.GetForeignKeyRelevantPropertyDefinitions(dataContainer.ClassDefinition);
    foreach (var propertyDefinition in foreignKeyRelevantProperties)
    {
      // if the value is null we don't care because we are sure that we can insert null values
      var pointingToID = (ObjectID?)dataContainer.GetValueWithoutEvents(propertyDefinition);
      if (pointingToID == null)
        continue;

      // if the object is present in the current data containers we need to check if 
      // it is new. If new we need a following update statement, and we stop looking for others
      if (dataContainers.TryGetValue(pointingToID, out var foundDataContainer) && foundDataContainer.State.IsNew)
        return true;
    }

    return false;
  }

  private bool IsPrecedingUpdateRequiredForDelete (
      TableDefinition tableDefinition,
      DataContainer dataContainer,
      IReadOnlyDictionary<ObjectID, DataContainer> dataContainers,
      IPersistenceModelSortingProvider persistenceModelSortingProvider)
  {
    if (persistenceModelSortingProvider.HasBeenSortedCorrectly(tableDefinition))
      return false;

    var foreignKeyRelevantProperties = persistenceModelSortingProvider.GetForeignKeyRelevantPropertyDefinitions(dataContainer.ClassDefinition);
    foreach (var propertyDefinition in foreignKeyRelevantProperties)
    {
      // if the value is null we don't care because we are sure that we can delete it.
      var pointingToID = (ObjectID?)dataContainer.GetValueWithoutEvents(propertyDefinition, ValueAccess.Original);
      if (pointingToID == null)
        continue;

      // if the object is present in the current data containers we need to check if 
      // it is deleted. If deleted we need an update statement, and we stop looking for others
      if (dataContainers.TryGetValue(pointingToID, out var foundDataContainer) && foundDataContainer.State.IsDeleted)
        return true;
    }

    return false;
  }

  private IBatchedCommandSpecification CreateLockCommandSpecification (TableDefinition tableDefinition, DataContainer[] dataContainers)
  {
    return CreateCommandSpecification(
        tableDefinition,
        dataContainers,
        (provider, classDefinition) => provider.GetLockRecordDefinition(classDefinition),
        d => new LockOrDeleteTableManipulationDataContainerAccessor(d));
  }

  private IBatchedCommandSpecification CreateInsertCommandSpecification (DataContainerGroup dataContainerGroup)
  {
    return CreateCommandSpecification(
        dataContainerGroup.TableDefinition,
        dataContainerGroup.ForInsert,
        (provider, classDefinition) => provider.GetInsertRecordDefinition(classDefinition),
        d => new InsertTableManipulationDataContainerAccessor(d, dataContainerGroup.InsertDataContainerIDsWithForeignKeyUpdate.Contains(d.ID)));
  }

  private IBatchedCommandSpecification CreateUpdateCommandSpecification (DataContainerGroup dataContainerGroup)
  {
    return CreateCommandSpecification(
        dataContainerGroup.TableDefinition,
        dataContainerGroup.ForUpdate,
        (provider, classDefinition) => provider.GetUpdateRecordDefinition(classDefinition),
        d => new UpdateTableManipulationDataContainerAccessor(d));
  }

  private IBatchedCommandSpecification CreateDeleteCommandSpecification (DataContainerGroup dataContainerGroup)
  {
    return CreateCommandSpecification(
        dataContainerGroup.TableDefinition,
        dataContainerGroup.ForDelete,
        (provider, classDefinition) => provider.GetDeleteRecordDefinition(classDefinition),
        d => new LockOrDeleteTableManipulationDataContainerAccessor(d));
  }

  private IBatchedCommandSpecification CreateCommandSpecification (
      TableDefinition tableDefinition,
      IReadOnlyList<DataContainer> dataContainers,
      Func<ITableManipulationRecordDefinitionProvider, ClassDefinition, RecordDefinition> getRecordDefinitionFunc,
      Func<DataContainer, ITableManipulationDataContainerAccessor> tableManipulationDataContainerAccessorFactory)
  {
    var parameterDefinition = new MultiClassTableValuedDataParameterDefinition(
        _tableManipulationRecordDefinitionProvider,
        getRecordDefinitionFunc,
        tableManipulationDataContainerAccessorFactory);
    var specification = new BatchedCommandSpecification(tableDefinition, parameterDefinition, dataContainers);
    return specification;
  }
}
