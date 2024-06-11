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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;

/// <summary>
/// Provides the <see cref="TableTypeDefinition"/>s that contain a single column for a scalar value.
/// </summary>
public class SingleScalarSqlTableTypeDefinitionProvider : ISingleScalarStructuredTypeDefinitionProvider
{
  public IStorageTypeInformationProvider StorageTypeInformationProvider { get; }
  private readonly ConcurrentDictionary<(Type dotNetType, bool withUniqueConstraint), Lazy<IRdbmsStructuredTypeDefinition>> _repository = new();

  // This type does not have values, so that we get an IStorageTypeInformation for varchar(max) 
  private class DummyExtensibleEnum : ExtensibleEnum<DummyExtensibleEnum>
  {
    public DummyExtensibleEnum (string id)
        : base(id)
    {
    }
  }

  public SingleScalarSqlTableTypeDefinitionProvider (IStorageTypeInformationProvider storageTypeInformationProvider)
  {
    ArgumentUtility.CheckNotNull(nameof(storageTypeInformationProvider), storageTypeInformationProvider);
    StorageTypeInformationProvider = storageTypeInformationProvider;
  }

  /// <summary>
  /// Gets a single-column <see cref="TableTypeDefinition"/> for each scalar type; where possible, an additional <see cref="TableTypeDefinition"/>
  /// with a <see cref="UniqueConstraintDefinition"/> is also returned.
  /// </summary>
  /// <remarks>
  /// A <see cref="UniqueConstraintDefinition"/> is only possible on columns that are up to 900 bytes wide. For example, varchar(max) or varbinary(max) are too wide to have
  /// a unique constraint.
  /// </remarks>
  public IEnumerable<IRdbmsStructuredTypeDefinition> GetAllStructuredTypeDefinitions ()
  {
    yield return GetStructuredTypeDefinition(typeof(string), false);
    yield return GetStructuredTypeDefinition(typeof(byte[]), false);
    yield return GetStructuredTypeDefinition(typeof(DummyExtensibleEnum), false);

    yield return GetStructuredTypeDefinition(typeof(bool), false);
    yield return GetStructuredTypeDefinition(typeof(bool), true);
    yield return GetStructuredTypeDefinition(typeof(byte), false);
    yield return GetStructuredTypeDefinition(typeof(byte), true);
    yield return GetStructuredTypeDefinition(typeof(short), false);
    yield return GetStructuredTypeDefinition(typeof(short), true);
    yield return GetStructuredTypeDefinition(typeof(int), false);
    yield return GetStructuredTypeDefinition(typeof(int), true);
    yield return GetStructuredTypeDefinition(typeof(long), false);
    yield return GetStructuredTypeDefinition(typeof(long), true);

    yield return GetStructuredTypeDefinition(typeof(decimal), false);
    yield return GetStructuredTypeDefinition(typeof(decimal), true);
    yield return GetStructuredTypeDefinition(typeof(float), false);
    yield return GetStructuredTypeDefinition(typeof(float), true);
    yield return GetStructuredTypeDefinition(typeof(double), false);
    yield return GetStructuredTypeDefinition(typeof(double), true);

    yield return GetStructuredTypeDefinition(typeof(DateTime), false);
    yield return GetStructuredTypeDefinition(typeof(DateTime), true);
    yield return GetStructuredTypeDefinition(typeof(Guid), false);
    yield return GetStructuredTypeDefinition(typeof(Guid), true);
  }

  /// <summary>
  /// Gets the <see cref="TableTypeDefinition"/> for the given <paramref name="dotNetType"/> and <paramref name="forDistinctValues"/> flag.
  /// </summary>
  /// <param name="dotNetType">The scalar <see cref="Type"/> of the single column.</param>
  /// <param name="forDistinctValues">Determines whether the <see cref="TableTypeDefinition"/> should have a <see cref="UniqueConstraintDefinition"/> on its single column.</param>
  /// <remarks>
  /// A <see cref="UniqueConstraintDefinition"/> is only possible on columns that are up to 900 bytes wide. For example, varchar(max) or varbinary(max) are too wide to have
  /// a unique constraint.
  /// </remarks>
  public IRdbmsStructuredTypeDefinition GetStructuredTypeDefinition (Type dotNetType, bool forDistinctValues)
  {
    ArgumentUtility.CheckNotNull(nameof(dotNetType), dotNetType);

    if (typeof(IExtensibleEnum).IsAssignableFrom(dotNetType))
      dotNetType = typeof(DummyExtensibleEnum); // ExtensibleEnum types have different varchar sizes -> always use our dummy in order to get varchar(max)

    var cachedValue = _repository.GetOrAdd(
        (dotNetType, forDistinctValues),
        definition => new Lazy<IRdbmsStructuredTypeDefinition>(
            () =>
            {
              if (!NullableTypeUtility.IsNullableType(definition.dotNetType))
                definition.dotNetType = typeof(Nullable<>).MakeGenericType(definition.dotNetType);

              var storageTypeInformation = StorageTypeInformationProvider.GetStorageType(definition.dotNetType);
              return CreateTypeDefinition(storageTypeInformation, definition.withUniqueConstraint);
            },
            LazyThreadSafetyMode.ExecutionAndPublication));
    return cachedValue.Value;
  }

  private static TableTypeDefinition CreateTypeDefinition (IStorageTypeInformation storageTypeInformation, bool withUniqueConstraint)
  {
    var dbType = storageTypeInformation.StorageDbType;
    var typeNameDefinition = new EntityNameDefinition(null, $"TVP_{dbType}{(withUniqueConstraint ? "_Distinct" : null)}");
    var columnDefinition = new ColumnDefinition("Value", storageTypeInformation, false);
    var propertyDefinitions = new[] { new SimpleStoragePropertyDefinition(storageTypeInformation.DotNetType, columnDefinition) };

    var tableConstraintDefinitions = Array.Empty<ITableConstraintDefinition>();
    if (withUniqueConstraint)
    {
      tableConstraintDefinitions =
      [
          new UniqueConstraintDefinition($"UQ_{typeNameDefinition.EntityName}_{columnDefinition.Name}", true, new[] { columnDefinition })
      ];
    }

    return new TableTypeDefinition(
        typeNameDefinition,
        propertyDefinitions,
        tableConstraintDefinitions);
  }
}
