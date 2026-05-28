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
    private readonly Dictionary<PropertyDefinition, ObjectID?> _objectIDValues;

    public InsertTableManipulationDataContainerAccessor (DataContainer dataContainer, Dictionary<PropertyDefinition, ObjectID?> objectIDValues)
    {
      ArgumentNullException.ThrowIfNull(dataContainer);

      _dataContainer = dataContainer;
      _objectIDValues = objectIDValues;
    }

    public ObjectID GetID () => _dataContainer.ID;

    public object GetTimestamp () => throw new NotSupportedException($"{nameof(InsertTableManipulationDataContainerAccessor)} does not support {nameof(GetTimestamp)}");

    public object? GetValue (PropertyDefinition propertyDefinition)
    {
      if (_objectIDValues.TryGetValue(propertyDefinition, out var value))
        return value;

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
    private readonly Dictionary<PropertyDefinition, ObjectID?>? _objectIDValues;

    public UpdateTableManipulationDataContainerAccessor (DataContainer dataContainer, Dictionary<PropertyDefinition, ObjectID?>? objectIDValues)
    {
      ArgumentNullException.ThrowIfNull(dataContainer);

      _dataContainer = dataContainer;
      _isDataContainerNewOrDeleted = dataContainer.State.IsNew || dataContainer.State.IsDeleted;
      _objectIDValues = objectIDValues;
    }

    public ObjectID GetID () => _dataContainer.ID;

    public object GetTimestamp () => _dataContainer.Timestamp!;

    public object? GetValue (PropertyDefinition propertyDefinition)
    {
      if (_objectIDValues?.TryGetValue(propertyDefinition, out var value) == true)
        return value;

      return _dataContainer.GetValueWithoutEvents(propertyDefinition);
    }

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
    public Dictionary<DataContainer, ITableManipulationDataContainerAccessor> InsertAccessors { get; } = new();
    public Dictionary<DataContainer, ITableManipulationDataContainerAccessor> UpdateAccessors { get; } = new();
    public Dictionary<DataContainer, ITableManipulationDataContainerAccessor> DeleteAccessors { get; } = new();
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

  protected virtual ITableManipulationDataContainerAccessor CreateLockDataContainerAccessor (DataContainer dataContainer)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    return new LockOrDeleteTableManipulationDataContainerAccessor(dataContainer);
  }

  protected virtual ITableManipulationDataContainerAccessor CreateInsertDataContainerAccessor (
      DataContainer dataContainer,
      Dictionary<PropertyDefinition, ObjectID?> objectIDValues)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);
    ArgumentNullException.ThrowIfNull(objectIDValues);

    return new InsertTableManipulationDataContainerAccessor(dataContainer, objectIDValues);
  }

  protected virtual ITableManipulationDataContainerAccessor CreateUpdateDataContainerAccessor (
      DataContainer dataContainer,
      Dictionary<PropertyDefinition, ObjectID?>? objectIDValues)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    return new UpdateTableManipulationDataContainerAccessor(dataContainer, objectIDValues);
  }

  protected virtual ITableManipulationDataContainerAccessor CreateDeleteDataContainerAccessor (DataContainer dataContainer)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    return new LockOrDeleteTableManipulationDataContainerAccessor(dataContainer);
  }

  private bool ShouldCreateInsertCommand (DataContainer dataContainer)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    return dataContainer.State.IsNew;
  }

  private bool ShouldCreateDeleteCommand (DataContainer dataContainer)
  {
    ArgumentNullException.ThrowIfNull(dataContainer);

    return dataContainer.State.IsDeleted;
  }

  private bool ShouldCreateUpdateCommand (DataContainer dataContainer)
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
      var dataContainersForLock = dataContainerGroup.UpdateAccessors.Keys
          .Where(d => !d.State.IsNew)
          .Union(dataContainerGroup.DeleteAccessors.Keys)
          .ToArray();
      if (dataContainersForLock.Length > 0)
      {
        allDataContainersForLocking.AddRange(dataContainersForLock);
        lockCommandSpecifications.Add(CreateLockCommandSpecification(dataContainerGroup.TableDefinition, dataContainersForLock));
      }

      if (dataContainerGroup.InsertAccessors.Count > 0)
      {
        allDataContainersForInsert.AddRange(dataContainerGroup.InsertAccessors.Keys);
        insertCommandSpecifications.Add(CreateInsertCommandSpecification(dataContainerGroup));
      }

      if (dataContainerGroup.UpdateAccessors.Count > 0)
      {
        allDataContainersForUpdate.AddRange(dataContainerGroup.UpdateAccessors.Keys);
        updateCommandSpecifications.Add(CreateUpdateCommandSpecification(dataContainerGroup));
      }

      if (dataContainerGroup.DeleteAccessors.Count > 0)
      {
        allDataContainersForDelete.AddRange(dataContainerGroup.DeleteAccessors.Keys);
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
        var accessors = CreateInsertAccessorsForDataContainer(dataContainer, dataContainers, persistenceModelSortingProvider);
        currentGroup.InsertAccessors.Add(dataContainer, accessors.insertAccessor);
        if (accessors.updateAccessor != null)
          currentGroup.UpdateAccessors.Add(dataContainer, accessors.updateAccessor);
      }
      else if (ShouldCreateDeleteCommand(dataContainer))
      {
        var accessors = CreateDeleteAccessors(dataContainer, dataContainers, persistenceModelSortingProvider);
        currentGroup.DeleteAccessors.Add(dataContainer, accessors.deleteAccessor);
        if (accessors.updateAccessor != null)
          currentGroup.UpdateAccessors.Add(dataContainer, accessors.updateAccessor);
      }
      else if (ShouldCreateUpdateCommand(dataContainer))
      {
        currentGroup.UpdateAccessors.Add(dataContainer, CreateUpdateDataContainerAccessor(dataContainer, null));
      }
    }

    return group.Values.OrderBy(g => persistenceModelSortingProvider.GetSortPosition(g.TableDefinition)).ToList();
  }

  private (ITableManipulationDataContainerAccessor insertAccessor, ITableManipulationDataContainerAccessor? updateAccessor) CreateInsertAccessorsForDataContainer (
      DataContainer dataContainer,
      IReadOnlyDictionary<ObjectID, DataContainer> dataContainers,
      IPersistenceModelSortingProvider persistenceModelSortingProvider)
  {
    var foreignKeyOptimizationObjectIDPropertySpecifications = persistenceModelSortingProvider.GetPropertySpecificationsForForeignKeyProperties(dataContainer.ClassDefinition);

    var insertValues = new Dictionary<PropertyDefinition, ObjectID?>(foreignKeyOptimizationObjectIDPropertySpecifications.Count);
    var updateValues = new Dictionary<PropertyDefinition, ObjectID?>(foreignKeyOptimizationObjectIDPropertySpecifications.Count);
    var requiresUpdate = false;
    var sortPosition = persistenceModelSortingProvider.GetSortPosition(dataContainer.ClassDefinition);

    foreach (var propertySpecification in foreignKeyOptimizationObjectIDPropertySpecifications)
    {
      var propertyValue = (ObjectID?)dataContainer.GetValueWithoutEvents(propertySpecification.PropertyDefinition);

      // the update values contain always all values because if we need an update we currently cannot skip
      // not updated values with TVPs because ObjectIDs are not optional.
      // we use update values because we already called GetValueWithoutEvents and have the correct value
      // so we do not have to call GetValueWithoutEvents later in UpdateTableManipulationDataContainerAccessor
      updateValues.Add(propertySpecification.PropertyDefinition, propertyValue);

      switch (propertySpecification.CycleBreakHint)
      {
        case ForeignKeyCycleBreakHint.PreferredBreak: // The application developer has decided that we do our magic
        case ForeignKeyCycleBreakHint.Automatic:
          if (!propertySpecification.HasForeignKeyConstraint || propertyValue == null)
          {
            // in case it is no foreign key, or has been ordered correct, or the value is null we can use the value for insert
            insertValues.Add(propertySpecification.PropertyDefinition, propertyValue);
          }
          else if (persistenceModelSortingProvider.GetSortPosition(propertyValue.ClassDefinition) > sortPosition
                   && dataContainers.TryGetValue(propertyValue, out var foundDataContainer)
                   && foundDataContainer.State.IsNew)
          {
            // if the value is not null we need to check if the object it is pointing to is in the current commit set and new
            // if present and new we need to insert null and use a following update
            requiresUpdate = true;
            insertValues.Add(propertySpecification.PropertyDefinition, null);
          }
          else
          {
            insertValues.Add(propertySpecification.PropertyDefinition, propertyValue);
          }
          break;
        case ForeignKeyCycleBreakHint.NeverBreak: // The application developer has decided to always insert values of this property, and if it fails it fails.
          insertValues.Add(propertySpecification.PropertyDefinition, propertyValue);
          break;
        case ForeignKeyCycleBreakHint.AlwaysBreak: // the application developer has decided to always insert nulls for this property and update them with a subsequent update.
          requiresUpdate = true;
          insertValues.Add(propertySpecification.PropertyDefinition, null);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    var insertAccessor = CreateInsertDataContainerAccessor(dataContainer, insertValues);
    ITableManipulationDataContainerAccessor? updateAccessor = null;
    if (requiresUpdate)
      updateAccessor = CreateUpdateDataContainerAccessor(dataContainer, updateValues);

    return (insertAccessor, updateAccessor);
  }

  private (ITableManipulationDataContainerAccessor deleteAccessor, ITableManipulationDataContainerAccessor? updateAccessor) CreateDeleteAccessors (
      DataContainer dataContainer,
      IReadOnlyDictionary<ObjectID, DataContainer> dataContainers,
      IPersistenceModelSortingProvider persistenceModelSortingProvider)
  {
    var foreignKeyOptimizationObjectIDPropertySpecifications = persistenceModelSortingProvider.GetPropertySpecificationsForForeignKeyProperties(dataContainer.ClassDefinition);

    var updateValues = new Dictionary<PropertyDefinition, ObjectID?>(foreignKeyOptimizationObjectIDPropertySpecifications.Count);
    var requiresUpdate = false;
    var sortPosition = persistenceModelSortingProvider.GetSortPosition(dataContainer.ClassDefinition);

    foreach (var propertySpecification in foreignKeyOptimizationObjectIDPropertySpecifications)
    {
      var originalPropertyValue = (ObjectID?)dataContainer.GetValueWithoutEvents(propertySpecification.PropertyDefinition, ValueAccess.Original);

      switch (propertySpecification.CycleBreakHint)
      {
        case ForeignKeyCycleBreakHint.PreferredBreak: // The application developer has decided that we do our magic
        case ForeignKeyCycleBreakHint.Automatic:
          if (propertySpecification.HasForeignKeyConstraint
              && originalPropertyValue != null
              && persistenceModelSortingProvider.GetSortPosition(originalPropertyValue.ClassDefinition) < sortPosition
              && dataContainers.TryGetValue(originalPropertyValue, out var foundDataContainer)
              && foundDataContainer.State.IsDeleted)
          {
            // if the value is not null we need to check if the object it is pointing to is in the current commit set and deleted
            // if present and deleted we need to update the value to null.
            requiresUpdate = true;
            updateValues.Add(propertySpecification.PropertyDefinition, null);
          }
          else
          {
            updateValues.Add(propertySpecification.PropertyDefinition, originalPropertyValue);
          }
          break;
        case ForeignKeyCycleBreakHint.NeverBreak: // The application developer has decided to never break this property, and if it fails it fails.
          updateValues.Add(propertySpecification.PropertyDefinition, originalPropertyValue);
          break;
        case ForeignKeyCycleBreakHint.AlwaysBreak: // the application developer has decided to always break this property and update them with a preceding update.
          requiresUpdate = true;
          updateValues.Add(propertySpecification.PropertyDefinition, null);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    var deleteAccessor = CreateDeleteDataContainerAccessor(dataContainer);
    ITableManipulationDataContainerAccessor? updateAccessor = null;
    if (requiresUpdate)
      updateAccessor = CreateUpdateDataContainerAccessor(dataContainer, updateValues);

    return (deleteAccessor, updateAccessor);
  }

  private IBatchedCommandSpecification CreateLockCommandSpecification (TableDefinition tableDefinition, DataContainer[] dataContainers)
  {
    return CreateCommandSpecification(
        tableDefinition,
        dataContainers,
        (provider, classDefinition) => provider.GetLockRecordDefinition(classDefinition),
        CreateLockDataContainerAccessor);
  }

  private IBatchedCommandSpecification CreateInsertCommandSpecification (DataContainerGroup dataContainerGroup)
  {
    return CreateCommandSpecification(
        dataContainerGroup.TableDefinition,
        dataContainerGroup.InsertAccessors.Keys,
        (provider, classDefinition) => provider.GetInsertRecordDefinition(classDefinition),
        d => dataContainerGroup.InsertAccessors[d]);
  }

  private IBatchedCommandSpecification CreateUpdateCommandSpecification (DataContainerGroup dataContainerGroup)
  {
    return CreateCommandSpecification(
        dataContainerGroup.TableDefinition,
        dataContainerGroup.UpdateAccessors.Keys,
        (provider, classDefinition) => provider.GetUpdateRecordDefinition(classDefinition),
        d => dataContainerGroup.UpdateAccessors[d]);
  }

  private IBatchedCommandSpecification CreateDeleteCommandSpecification (DataContainerGroup dataContainerGroup)
  {
    return CreateCommandSpecification(
        dataContainerGroup.TableDefinition,
        dataContainerGroup.DeleteAccessors.Keys,
        (provider, classDefinition) => provider.GetDeleteRecordDefinition(classDefinition),
        d => dataContainerGroup.DeleteAccessors[d]);
  }

  private IBatchedCommandSpecification CreateCommandSpecification (
      TableDefinition tableDefinition,
      IReadOnlyCollection<DataContainer> dataContainers,
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
