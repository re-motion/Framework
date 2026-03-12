// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

/// <summary>
/// Provides the <see cref="RecordDefinition"/>s for TVPs that can be used to efficiently batch table manipulations.
/// TVPs for lock and delete are shared between tables, while a TVP is created for insert and update per table.
/// </summary>
public class TableManipulationRecordDefinitionProvider : ITableManipulationRecordDefinitionProvider
{
  public const string IsSetColumnPostFix = "__IsSet";
  public class UnknownRecordPropertyDefinition : RecordPropertyDefinition
  {
    public UnknownRecordPropertyDefinition (IRdbmsStoragePropertyDefinition storagePropertyDefinition, Func<object, object?> getValue)
        : base("Unknown", storagePropertyDefinition, getValue)
    {
    }
  }

  public class TableManipulationRecordDefinition : RecordDefinition
  {
    public TableManipulationRecordDefinition (string name, IRdbmsStructuredTypeDefinition structuredTypeDefinition, IReadOnlyCollection<RecordPropertyDefinition> propertyDefinitions)
        : base(name, structuredTypeDefinition, propertyDefinitions)
    {
    }

    public override object[] GetColumnValues (object item)
    {
      ArgumentNullException.ThrowIfNull(item);

      // It is possible that we get NULL for a non-nullable type therefore ConvertToStorageType can fail.
      // This happens when multiple classes are stored in the same table.
      // eg: Class A and B : A where B introduces a new int property called IntPropOnB.
      // If we now try to get all values for an instance of Class A we get a column for IntPropOnB but Class A will always return null.
      // Because the property is typed to System.Int32 and null cannot be converted to System.In32 therefore ConvertToStorageType throws an exception.
      // But instead we just want DBNull.Value because the column in the database has to be nullable.

      var convertedValues = new List<object>(PropertyDefinitions.Count * 2);
      foreach (var recordPropertyDefinition in PropertyDefinitions)
      {
        var columns = recordPropertyDefinition.StoragePropertyDefinition.SplitValue(recordPropertyDefinition.GetValue(item));
        // TODO: RM-8491 Possibly remove this check when RM-8491 is fixed
        if (recordPropertyDefinition is UnknownRecordPropertyDefinition)
          convertedValues.AddRange(columns.Select(cv => cv.Value ?? DBNull.Value));
        else
          convertedValues.AddRange(columns.Select(cv => cv.Column.StorageTypeInfo.ConvertToStorageType(cv.Value)));
      }

      return convertedValues.ToArray();
    }
  }

  private delegate RecordPropertyDefinition RecordPropertyDefinitionFactory (
      IRdbmsStoragePropertyDefinition property,
      IReadOnlyDictionary<IStoragePropertyDefinition, PropertyDefinition> propertyDefinitionLookup);

  private class ColumnDefinitionEqualityComparer : IEqualityComparer<ColumnDefinition>
  {
    public bool Equals (ColumnDefinition? x, ColumnDefinition? y)
    {
      if (x == null && y == null)
        return true;

      if (x == null)
        return false;

      if (y == null)
        return false;

      return string.Equals(x.Name, y.Name, StringComparison.Ordinal) && x.IsPartOfPrimaryKey == y.IsPartOfPrimaryKey;
    }

    public int GetHashCode (ColumnDefinition obj)
    {
      return HashCode.Combine(obj.Name, obj.IsPartOfPrimaryKey);
    }
  }

  private class StoragePropertyDefinitionEqualityComparer : IEqualityComparer<IStoragePropertyDefinition>
  {
    private readonly ColumnDefinitionEqualityComparer _columnDefinitionEqualityComparer = new();

    public bool Equals (IStoragePropertyDefinition? x, IStoragePropertyDefinition? y)
    {
      if (x == null && y == null)
        return true;

      if (x == null)
        return false;

      if (y == null)
        return false;

      if (ReferenceEquals(x, y))
        return true;

      if (x is not IRdbmsStoragePropertyDefinition xRdbms || y is not IRdbmsStoragePropertyDefinition yRdbms)
        throw new UnreachableException("This should be unreachable, because every implementation compared here should implement IRdbmsStoragePropertyDefinition");

      var xCols = xRdbms.GetColumns();
      var yCols = yRdbms.GetColumns();
      return xCols.SequenceEqual(yCols, _columnDefinitionEqualityComparer);

    }

    public int GetHashCode (IStoragePropertyDefinition obj)
    {
      var hashCode = new HashCode();
      hashCode.Add(obj.GetType().GetHashCode());

      if (obj is not IRdbmsStoragePropertyDefinition rdbmsStoragePropertyDefinition)
        return hashCode.ToHashCode();

      foreach (var column in rdbmsStoragePropertyDefinition.GetColumns())
      {
        hashCode.Add(column.Name.GetHashCode());
      }

      return hashCode.ToHashCode();
    }
  }

  /// <summary>
  /// Represents a column in the TVP and handles the creation of <see cref="RecordPropertyDefinition"/>.
  /// Use the static factory methods to create instances.
  /// </summary>
  private record TvpColumnMetadata (IRdbmsStoragePropertyDefinition Property, RecordPropertyDefinitionFactory RecordPropertyDefinitionFactory)
  {
    /// <summary>
    /// Creates a <see cref="TvpColumnMetadata"/> for an ObjectID property and reads the whole object ID.
    /// </summary>
    public static TvpColumnMetadata CreateForIDColumn (TableDefinition tableDefinition)
    {
      ArgumentNullException.ThrowIfNull(tableDefinition);

      return new TvpColumnMetadata(
           tableDefinition.ObjectIDProperty,
           static (property, _) =>
           {
             return new RecordPropertyDefinition(
                 "ID", // Note: Name here is cosmetic and not used anywhere
                 property,
                 o => ((ITableManipulationDataContainerAccessor)o).GetID());
           });
    }

    /// <summary>
    /// Creates a <see cref="TvpColumnMetadata"/> for an ObjectID property and only retrieves the value part of it.
    /// </summary>
    public static TvpColumnMetadata CreateForIDValueColumn (IRdbmsStoragePropertyDefinition property)
    {
      ArgumentNullException.ThrowIfNull(property);

      return new TvpColumnMetadata(
            property,
            static (property, _) =>
            {
              return new RecordPropertyDefinition(
                  "ID", // Note: Name here is cosmetic and not used anywhere
                  property,
                  o => ((ITableManipulationDataContainerAccessor)o).GetID().Value);
            });
    }

    public static TvpColumnMetadata CreateForTimestampColumn (IRdbmsStoragePropertyDefinition property)
    {
      ArgumentNullException.ThrowIfNull(property);

      return new TvpColumnMetadata(
          property,
          static (property, _) =>
          {
            return new RecordPropertyDefinition(
                "Timestamp", // Note: Name here is cosmetic and not used anywhere
                property,
                o => ((ITableManipulationDataContainerAccessor)o).GetTimestamp());
          });
    }

    public static TvpColumnMetadata CreateForDataColumn (IRdbmsStoragePropertyDefinition property)
    {
      ArgumentNullException.ThrowIfNull(property);

      return new TvpColumnMetadata(
          property,
          static (property, propertyLookup) =>
          {
            // If we can't find the property in the lookup then it belongs to another ClassDefinition in the inheritance hierarchy
            if (propertyLookup.TryGetValue(property, out var propertyDefinition))
            {
              return new RecordPropertyDefinition(
                  propertyDefinition.PropertyName,
                  property,
                  o => ((ITableManipulationDataContainerAccessor)o).GetValue(propertyDefinition));
            }
            else
            {
              return new UnknownRecordPropertyDefinition(
                  property,
                  _ => null);
            }
          });
    }

    public static (TvpColumnMetadata DataProperty, TvpColumnMetadata IsDataSetProperty) CreateForOptionalDataColumn (
        IStorageTypeInformationProvider storageTypeInformationProvider,
        SimpleStoragePropertyDefinition property,
        object? defaultValue)
    {
      ArgumentNullException.ThrowIfNull(storageTypeInformationProvider);
      ArgumentNullException.ThrowIfNull(property);

      var dataProperty = new TvpColumnMetadata(
          property,
          (property, propertyLookup) =>
          {
            // If we can't find the property in the lookup then it belongs to another ClassDefinition in the inheritance hierarchy
            if (propertyLookup.TryGetValue(property, out var propertyDefinition))
            {
              return new RecordPropertyDefinition(
                  propertyDefinition.PropertyName,
                  property,
                  o => ((ITableManipulationDataContainerAccessor)o).GetOptionalValue(propertyDefinition, defaultValue));
            }
            else
            {
              return new UnknownRecordPropertyDefinition(
                  property,
                  _ => null);
            }
          });

      var columnDefinition = new ColumnDefinition(
          $"{property.ColumnDefinition.Name}{IsSetColumnPostFix}",
          storageTypeInformationProvider.GetStorageType(typeof(bool)),
          false);
      var isDataSetPropertyDefinition = new SimpleStoragePropertyDefinition(typeof(bool), columnDefinition);
      var isDataSetProperty = new TvpColumnMetadata(
          isDataSetPropertyDefinition,
          (isSetProperty, propertyLookup) =>
          {
            // If we can't find the property in the lookup then it belongs to another ClassDefinition in the inheritance hierarchy
            if (propertyLookup.TryGetValue(property, out var propertyDefinition))
            {
              return new RecordPropertyDefinition(
                  columnDefinition.Name,
                  isSetProperty,
                  o => ((ITableManipulationDataContainerAccessor)o).IsOptionalValueSet(propertyDefinition));
            }
            else
            {
              return new UnknownRecordPropertyDefinition(
                  isSetProperty,
                  _ => false);
            }
          });

      return (dataProperty, isDataSetProperty);
    }
  }

  private record TvpTableTypeDefinitions (
      TableTypeDefinition InsertTableTypeDefinition,
      ImmutableArray<TvpColumnMetadata> InsertColumns,
      TableTypeDefinition UpdateTableTypeDefinition,
      ImmutableArray<TvpColumnMetadata> UpdateColumns);

  private record TvpRecordDefinitions (
      RecordDefinition InsertRecordDefinition,
      RecordDefinition UpdateRecordDefinition);

  private static readonly FrozenDictionary<Type, object> s_optionalColumnsDefaultValues = FrozenDictionary.Create<Type, object>(
  [
      new KeyValuePair<Type, object>(typeof(string), string.Empty),
      new KeyValuePair<Type, object>(typeof(byte[]), Array.Empty<byte>())
  ]);

  private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;
  private readonly IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;
  private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;

  private readonly ConcurrentDictionary<TableDefinition, Lazy<TvpTableTypeDefinitions>> _tableTypeDefinitions = new();
  private readonly ConcurrentDictionary<ClassDefinition, Lazy<TvpRecordDefinitions?>> _recordDefinitions = new();

  private readonly Lazy<RecordDefinition> _deleteLazyRecordDefinition;
  private readonly Lazy<RecordDefinition> _lockLazyRecordDefinition;

  public TableManipulationRecordDefinitionProvider (
      IStorageTypeInformationProvider storageTypeInformationProvider,
      IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
      IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider)
  {
    ArgumentNullException.ThrowIfNull(storageTypeInformationProvider);
    ArgumentNullException.ThrowIfNull(rdbmsPersistenceModelProvider);

    _storageTypeInformationProvider = storageTypeInformationProvider;
    _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
    _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;

    _deleteLazyRecordDefinition = new Lazy<RecordDefinition>(CreateDeleteRecordDefinition, LazyThreadSafetyMode.ExecutionAndPublication);
    _lockLazyRecordDefinition = new Lazy<RecordDefinition>(CreateLockRecordDefinition, LazyThreadSafetyMode.ExecutionAndPublication);
  }

  /// <inheritdoc />
  public RecordDefinition GetDeleteRecordDefinition (ClassDefinition classDefinition)
  {
    ArgumentNullException.ThrowIfNull(classDefinition);

    return _deleteLazyRecordDefinition.Value;
  }

  /// <inheritdoc />
  public RecordDefinition GetInsertRecordDefinition (ClassDefinition classDefinition)
  {
    ArgumentNullException.ThrowIfNull(classDefinition);

    return GetOrCreateTvpRecordDefinitions(classDefinition)?.InsertRecordDefinition
           ?? throw new InvalidOperationException($"No TVP record definition could be found for class '{classDefinition.ID}'.");
  }

  /// <inheritdoc />
  public RecordDefinition GetLockRecordDefinition (ClassDefinition classDefinition)
  {
    ArgumentNullException.ThrowIfNull(classDefinition);

    return _lockLazyRecordDefinition.Value;
  }

  /// <inheritdoc />
  public RecordDefinition GetUpdateRecordDefinition (ClassDefinition classDefinition)
  {
    ArgumentNullException.ThrowIfNull(classDefinition);

    return GetOrCreateTvpRecordDefinitions(classDefinition)?.UpdateRecordDefinition
           ?? throw new InvalidOperationException($"No TVP record definition could be found for class '{classDefinition.ID}'.");
  }

  private TvpRecordDefinitions? GetOrCreateTvpRecordDefinitions (ClassDefinition classDefinition)
  {
    return _recordDefinitions.GetOrAdd(
            classDefinition,
            value => { return new Lazy<TvpRecordDefinitions?>(() => CreateTvpRecordDefinitions(value), LazyThreadSafetyMode.ExecutionAndPublication); })
        .Value;
  }

  private TvpRecordDefinitions? CreateTvpRecordDefinitions (ClassDefinition classDefinition)
  {
    var storageEntityDefinition = _rdbmsPersistenceModelProvider.GetEntityDefinition(classDefinition);

    // For classes that share their table with other classes, the storage entity definition is going to be a filter view.
    // We can follow this filter view to the corresponding table definition.
    var tableDefinition = InlineRdbmsStorageEntityDefinitionVisitor.Visit<TableDefinition?>(
        storageEntityDefinition,
        (table, continuation) => table,
        (filterView, continuation) => continuation(filterView.BaseEntity),
        (unionView, continuation) => null,
        (emptyView, continuation) => null);

    // No TableDefinition -> the ClassDefinition does not correspond to a table, and we don't generate TVPs
    if (tableDefinition == null)
      return null;

    // Multiple class definitions might use the same table for storage, so we need to ensure that they share their TableTypeDefinitions.
    // RecordDefinitions, on the other hand, are created per ClassDefinition.
    var tableTypeDefinitions = _tableTypeDefinitions.GetOrAdd(
            tableDefinition,
            value => { return new Lazy<TvpTableTypeDefinitions>(() => CreateTableTypeDefinition(value), LazyThreadSafetyMode.ExecutionAndPublication); })
        .Value;

    // We can't navigate from rdbms property to mapping property so we create a reverse lookup using the ClassDefinition.
    // The assumption is that this is enough to find our own properties. Properties from sibling ClassDefinitions are not included but also not relevant.
    // We need a custom IEqualityComparer because the StoragePropertyDefinition is not always the same instance as the one in the table definition.
    // This is caused by IRdbmsStoragePropertyDefinition.UnifyWithEquivalentProperties which creates new instances.
    var propertyLookup = classDefinition
        .GetPropertyDefinitions()
        .Where(pd => pd.StorageClass == StorageClass.Persistent)
        .ToDictionary(e => e.StoragePropertyDefinition, e => e, new StoragePropertyDefinitionEqualityComparer());

    var insertRecordDefinition = CreateRecordDefinition(tableTypeDefinitions.InsertTableTypeDefinition, tableTypeDefinitions.InsertColumns, propertyLookup);
    var updateRecordDefinition = CreateRecordDefinition(tableTypeDefinitions.UpdateTableTypeDefinition, tableTypeDefinitions.UpdateColumns, propertyLookup);

    return new TvpRecordDefinitions(
        insertRecordDefinition,
        updateRecordDefinition);
  }

  private RecordDefinition CreateDeleteRecordDefinition ()
  {
    ImmutableArray<TvpColumnMetadata> columns =
    [
        TvpColumnMetadata.CreateForIDValueColumn(_infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition().ValueProperty)
    ];
    var tableTypeDefinition = CreateTableTypeDefinition(
        "TVP_AllTables_Delete",
        columns);

    return CreateRecordDefinition(
        tableTypeDefinition,
        columns,
        ReadOnlyDictionary<IStoragePropertyDefinition, PropertyDefinition>.Empty);
  }

  private RecordDefinition CreateLockRecordDefinition ()
  {
    ImmutableArray<TvpColumnMetadata> columns =
    [
        TvpColumnMetadata.CreateForIDValueColumn(_infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition().ValueProperty),
        TvpColumnMetadata.CreateForTimestampColumn(_infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition())
    ];
    var tableTypeDefinition = CreateTableTypeDefinition(
        "TVP_AllTables_Lock",
        columns);

    return CreateRecordDefinition(
        tableTypeDefinition,
        columns,
        ReadOnlyDictionary<IStoragePropertyDefinition, PropertyDefinition>.Empty);
  }

  private TableManipulationRecordDefinition CreateRecordDefinition (TableTypeDefinition tableTypeDefinition, ImmutableArray<TvpColumnMetadata> columns, IReadOnlyDictionary<IStoragePropertyDefinition, PropertyDefinition> propertyLookup)
  {
    return new TableManipulationRecordDefinition(
        tableTypeDefinition.TypeName.EntityName,
        tableTypeDefinition,
        columns.Select(e => e.RecordPropertyDefinitionFactory(e.Property, propertyLookup)).ToArray());
  }

  private TvpTableTypeDefinitions CreateTableTypeDefinition (TableDefinition tableDefinition)
  {
    var insertColumns = GetInsertTvpColumns(tableDefinition).ToImmutableArray();
    var insertTableDefinition = CreateTableTypeDefinition($"TVP_{tableDefinition.TableName.EntityName}_Insert", insertColumns);

    var updateColumns = GetUpdateTvpColumns(tableDefinition).ToImmutableArray();
    var updateTableDefinition = CreateTableTypeDefinition($"TVP_{tableDefinition.TableName.EntityName}_Update", updateColumns);

    return new TvpTableTypeDefinitions(
        insertTableDefinition,
        insertColumns,
        updateTableDefinition,
        updateColumns);
  }

  private TableTypeDefinition CreateTableTypeDefinition (string name, ImmutableArray<TvpColumnMetadata> columns)
  {
    return new TableTypeDefinition(
        new EntityNameDefinition(null, name),
        columns.Select(e => e.Property).ToArray(),
        []);
  }

  private IEnumerable<TvpColumnMetadata> GetInsertTvpColumns (TableDefinition tableDefinition)
  {
    yield return TvpColumnMetadata.CreateForIDColumn(tableDefinition);
    foreach (var dataProperty in tableDefinition.DataProperties)
      yield return TvpColumnMetadata.CreateForDataColumn(dataProperty);
  }

  private IEnumerable<TvpColumnMetadata> GetUpdateTvpColumns (TableDefinition tableDefinition)
  {
    yield return TvpColumnMetadata.CreateForIDColumn(tableDefinition);
    foreach (var dataProperty in tableDefinition.DataProperties)
    {
      if (dataProperty is SimpleStoragePropertyDefinition simpleDataProperty
          && s_optionalColumnsDefaultValues.TryGetValue(simpleDataProperty.ColumnDefinition.StorageTypeInfo.DotNetType, out var defaultValue))
      {
        var (data, isDataSet) = TvpColumnMetadata.CreateForOptionalDataColumn(
            _storageTypeInformationProvider,
            simpleDataProperty,
            defaultValue);

        yield return data;
        yield return isDataSet;
      }
      else
      {
        yield return TvpColumnMetadata.CreateForDataColumn(dataProperty);
      }
    }
  }
}
