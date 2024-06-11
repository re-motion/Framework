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
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Finds the matching <see cref="IRdbmsStructuredTypeDefinition"/> for a collection of scalar values.  
  /// </summary>
  public class StructuredTypeDefinitionFinderForCollectionOfScalars : IRdbmsStructuredTypeDefinitionFinder
  {
    public ISingleScalarStructuredTypeDefinitionProvider SingleScalarStructuredTypeProvider { get; }

    public StructuredTypeDefinitionFinderForCollectionOfScalars (ISingleScalarStructuredTypeDefinitionProvider singleScalarStructuredTypeProvider)
    {
      ArgumentUtility.CheckNotNull(nameof(singleScalarStructuredTypeProvider), singleScalarStructuredTypeProvider);
      SingleScalarStructuredTypeProvider = singleScalarStructuredTypeProvider;
    }

    /// <summary>
    /// Gets the matching <see cref="IRdbmsStructuredTypeDefinition"/> for the <paramref name="queryParameter"/>'s <see cref="QueryParameter.Value"/>,
    /// as long as that parameter value is a collection of scalar items.
    /// </summary>
    /// <param name="queryParameter">The <see cref="QueryParameter"/> for whose <see cref="QueryParameter.Value"/> to find a matching <see cref="IRdbmsStructuredTypeDefinition"/>.
    /// </param>
    /// <param name="query">The <see cref="IQuery"/> that contains the <paramref name="queryParameter"/>, for additional context.</param>
    /// <returns>An <see cref="IRdbmsStructuredTypeDefinition"/> that can be used to transmit the <paramref name="queryParameter"/>'s <see cref="QueryParameter.Value"/>,
    /// or <see langword="null"/> if that value is not a collection.</returns>
    /// <exception cref="NotSupportedException">If the <paramref name="queryParameter"/>'s <see cref="QueryParameter.Value"/> is a collection, but the item type is not
    /// supported by the <see cref="SingleScalarStructuredTypeProvider"/>.</exception>
    public IRdbmsStructuredTypeDefinition? GetTypeDefinition (QueryParameter queryParameter, IQuery query)
    {
      ArgumentUtility.CheckNotNull(nameof(queryParameter), queryParameter);
      ArgumentUtility.CheckNotNull(nameof(queryParameter), queryParameter);

      var (itemType, isDistinct) = GetCollectionInfo(queryParameter.Value);
      if (itemType != null)
        return SingleScalarStructuredTypeProvider.GetStructuredTypeDefinition(itemType, isDistinct);

      return null;
    }

    private (Type? itemType, bool isDistinct) GetCollectionInfo (object? value)
    {
      (Type? itemType, bool isDistinct) result = (null, false);
      if (value == null)
        return result;

      if (value is string or char[] or byte[])
        return result;

      if (value is not IEnumerable)
        return result;

      if (value is ICollection)
        result.itemType = typeof(object);

      var type = value.GetType();
      if (type.CanAscribeTo(typeof(ICollection<>)))
      {
        result.itemType = type.GetAscribedGenericArguments(typeof(ICollection<>)).Single();
        result.isDistinct = type.CanAscribeTo(typeof(ISet<>));
      }
      else if (type.CanAscribeTo(typeof(IReadOnlyCollection<>)))
      {
        result.itemType = type.GetAscribedGenericArguments(typeof(IReadOnlyCollection<>)).Single();
#if NET5_0_OR_GREATER
        result.isDistinct = type.CanAscribeTo(typeof(IReadOnlySet<>));
#endif
      }

      return result;
    }
  }
}
