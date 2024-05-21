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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

/// <summary>
/// SQL-Server-based implementation that can create <see cref="IDataParameterDefinition"/> instances that uses table-valued parameters to handle parameter values that are
/// collections (<see cref="ICollection"/>, <see cref="ICollection{T}"/>, and <see cref="IReadOnlyCollection{T}"/>).
/// </summary>
public class SqlTableValuedDataParameterDefinitionFactory : IDataParameterDefinitionFactory
{
  public IStorageTypeInformationProvider StorageTypeInformationProvider { get; }
  public IDataParameterDefinitionFactory NextDataParameterDefinitionFactory { get; }

  public SqlTableValuedDataParameterDefinitionFactory (IStorageTypeInformationProvider storageTypeInformationProvider, IDataParameterDefinitionFactory nextDataParameterDefinitionFactory)
  {
    ArgumentUtility.CheckNotNull(nameof(storageTypeInformationProvider), storageTypeInformationProvider);
    ArgumentUtility.CheckNotNull(nameof(nextDataParameterDefinitionFactory), nextDataParameterDefinitionFactory);

    StorageTypeInformationProvider = storageTypeInformationProvider;
    NextDataParameterDefinitionFactory = nextDataParameterDefinitionFactory;
  }

  /// <summary>
  /// Creates an <see cref="IDataParameterDefinition"/> from a <paramref name="queryParameter"/> with a collection <see cref="QueryParameter.Value"/>; for other values,
  /// responsibility is passed to the <see cref="NextDataParameterDefinitionFactory"/>.
  /// </summary>
  public IDataParameterDefinition CreateDataParameterDefinition (QueryParameter queryParameter)
  {
    ArgumentUtility.CheckNotNull(nameof(queryParameter), queryParameter);

    if (TryGetCollectionInfo(queryParameter.Value, out var itemType, out var isDistinct))
    {
      var storageTypeInformation = StorageTypeInformationProvider.GetStorageType(itemType);
      return new SqlTableValuedDataParameterDefinition(storageTypeInformation, isDistinct);
    }
    return NextDataParameterDefinitionFactory.CreateDataParameterDefinition(queryParameter);
  }

  private bool TryGetCollectionInfo (object? value, [MaybeNullWhen(false)] out Type itemType, out bool isDistinct)
  {
    itemType = null!;
    isDistinct = false;

    if (value == null)
      return false;

    if (value is string or char[] or byte[])
      return false;

    if (value is not IEnumerable)
      return false;

    if (value is ICollection)
      itemType = typeof(object);

    var type = value.GetType();
    if (type.CanAscribeTo(typeof(ICollection<>)))
    {
      itemType = type.GetAscribedGenericArguments(typeof(ICollection<>)).Single();
      isDistinct = type.CanAscribeTo(typeof(ISet<>));
    }
    else if (type.CanAscribeTo(typeof(IReadOnlyCollection<>)))
    {
      itemType = type.GetAscribedGenericArguments(typeof(IReadOnlyCollection<>)).Single();
#if NET5_0_OR_GREATER
      isDistinct = type.CanAscribeTo(typeof(IReadOnlySet<>));
#endif
    }

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    return itemType != null;
  }
}
