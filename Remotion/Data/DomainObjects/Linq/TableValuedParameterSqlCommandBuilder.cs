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
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.Linq;

/// <summary>
/// An <see cref="SqlCommandBuilder"/> that uses SQL Server's "table-valued parameters" feature to handle <see cref="ConstantCollectionExpression"/>s.
/// </summary>
public class TableValuedParameterSqlCommandBuilder : SqlCommandBuilder
{
  /// <summary>
  /// Creates a new <see cref="SqlCommandBuilder"/> instance that uses a table-valued parameter (instead of single-element parameters) for a collection that has at least
  /// <paramref name="tableValuedParameterThreshold"/> elements.
  /// </summary>
  /// <exception cref="ArgumentOutOfRangeException">If <paramref name="tableValuedParameterThreshold"/> is 0 or negative.</exception>
  public TableValuedParameterSqlCommandBuilder (int tableValuedParameterThreshold)
  {
    if (tableValuedParameterThreshold <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(tableValuedParameterThreshold), "Threshold must be greater than 0.");
    }

    TableValuedParameterThreshold = tableValuedParameterThreshold;
  }

  public int TableValuedParameterThreshold { get; }

  protected override void AppendNonEmptyCollection (ConstantCollectionExpression collectionExpression)
  {
    ArgumentNullException.ThrowIfNull(collectionExpression);

    if (GetCount(collectionExpression.Collection, TableValuedParameterThreshold) < TableValuedParameterThreshold)
    {
      base.AppendNonEmptyCollection(collectionExpression);
    }
    else
    {
      Append("SELECT [Value] FROM ");
      AppendParameter(collectionExpression.Collection);
    }
    static int GetCount (object enumerable, int threshold)
    {
      if (enumerable is ICollection collection)
        return collection.Count;

      if (enumerable is IReadOnlyCollection<object> readOnlyCollection)
        return readOnlyCollection.Count;

      if (enumerable.GetType().CanAscribeTo(typeof(ICollection<>)) || enumerable.GetType().CanAscribeTo(typeof(IReadOnlyCollection<>)))
        return ((IEnumerable)enumerable).Cast<object>().Take(threshold).Count();

      throw new NotSupportedException($"ConstantCollectionExpression for a collection of type {enumerable.GetType().FullName} is not supported.");
    }
  }
}
