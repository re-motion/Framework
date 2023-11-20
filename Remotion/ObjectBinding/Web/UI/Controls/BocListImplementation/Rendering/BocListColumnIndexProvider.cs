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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for providing a column index for a specific <see cref="BocColumnDefinition"/> from a set of column definitions.
  /// </summary>
  public class BocListColumnIndexProvider : IBocListColumnIndexProvider
  {
    private readonly IReadOnlyList<BocColumnRenderer> _columnDefinitions;

    // Create the lookup lazily as most of the times it won't be used as there are no validation failures to render
    private Dictionary<BocColumnDefinition, BocColumnRenderer>? _lookup;

    public BocListColumnIndexProvider (IReadOnlyList<BocColumnRenderer> columnDefinitions)
    {
      ArgumentUtility.CheckNotNull(nameof(columnDefinitions), columnDefinitions);

      _columnDefinitions = columnDefinitions;
    }

    /// <inheritdoc />
    public int GetColumnIndex (BocColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(columnDefinition), columnDefinition);

      return GetBocColumnRenderer(columnDefinition).ColumnIndex;
    }

    /// <inheritdoc />
    public int GetVisibleColumnIndex (BocColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(columnDefinition), columnDefinition);

      return GetBocColumnRenderer(columnDefinition).VisibleColumnIndex;
    }

    private BocColumnRenderer GetBocColumnRenderer (BocColumnDefinition columnDefinition)
    {
      _lookup ??= _columnDefinitions.ToDictionary(e => e.ColumnDefinition, e => e);
      return _lookup.TryGetValue(columnDefinition, out var columnRenderer)
          ? columnRenderer
          : throw new InvalidOperationException(
              $"Could not find a {nameof(BocColumnRenderer)} for the {nameof(BocColumnDefinition)} with item ID '{columnDefinition.ItemID}'.");
    }
  }
}
