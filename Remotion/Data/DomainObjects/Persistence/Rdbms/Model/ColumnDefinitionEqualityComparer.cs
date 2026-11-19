// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

/// <summary>
///   Provides equality comparison logic for <see cref="ColumnDefinition" /> instances,
///   based on the column name and whether the column is part of the primary key.
/// </summary>
internal class ColumnDefinitionEqualityComparer : IEqualityComparer<ColumnDefinition>
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
