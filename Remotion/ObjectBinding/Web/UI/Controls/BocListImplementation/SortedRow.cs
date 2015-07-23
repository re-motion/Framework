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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation
{
  /// <summary>
  /// Encapsulates a <see cref="BocListRow"/> and its position in the sorted list.
  /// </summary>
  public class SortedRow
  {
    private readonly BocListRow _valueRow;
    private readonly int _sortedIndex;

    public SortedRow (BocListRow valueRow, int sortedIndex)
    {
      ArgumentUtility.CheckNotNull ("valueRow", valueRow);
      if (sortedIndex < 0)
        throw new ArgumentOutOfRangeException ("sortedIndex", sortedIndex, "Value cannot be negative");

      _valueRow = valueRow;
      _sortedIndex = sortedIndex;
    }

    public BocListRow ValueRow
    {
      get { return _valueRow; }
    }

    public int SortedIndex
    {
      get { return _sortedIndex; }
    }
  }
}