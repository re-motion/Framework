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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting
{
  public static class BocListRowSortExtension
  {
    public static IEnumerable<BocListRow> OrderBy (this IEnumerable<BocListRow> rows, BocListSortingOrderEntry[] sortingOrder)
    {
      ArgumentUtility.CheckNotNull ("rows", rows);
      ArgumentUtility.CheckNotNull ("sortingOrder", sortingOrder);

      return rows.OrderBy (r => r, new CompoundComparer<BocListRow> (sortingOrder.GetComparers()));
    }

    private static IEnumerable<IComparer<BocListRow>> GetComparers (this IEnumerable<BocListSortingOrderEntry> sortingOrder)
    {
      return sortingOrder
          .Where (entry => entry.Direction != SortingDirection.None)
          .Select (CreateComparer);
    }

    private static IComparer<BocListRow> CreateComparer (BocListSortingOrderEntry entry)
    {
      var baseComparer = entry.Column.CreateCellValueComparer();

      if (entry.Direction == SortingDirection.Descending)
        return new InvertedComparerDecorator<BocListRow> (baseComparer);

      return baseComparer;
    }
  }
}