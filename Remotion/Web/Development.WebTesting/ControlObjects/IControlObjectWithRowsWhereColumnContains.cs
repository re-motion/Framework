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
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of rows with cells, e.g. a BOC list. The interface
  /// allows to query a row by the contents of a certain cell, e.g. "give me the row of the list where the column 'xyz' contains the text 'abc'".
  /// </summary>
  public interface IControlObjectWithRowsWhereColumnContains<TRowControlObject>
      where TRowControlObject : ControlObject
  {
    /// <summary>
    /// Start of the fluent interface for selecting a row.
    /// </summary>
    IFluentControlObjectWithRowsWhereColumnContains<TRowControlObject> GetRowWhere ();

    /// <summary>
    /// Short for explicitly implemented <see cref="IFluentControlObjectWithRowsWhereColumnContains{TRowControlObject}.ColumnWithItemIDContains"/>.
    /// </summary>
    TRowControlObject GetRowWhere ([NotNull] string columnItemID, [NotNull] string containsCellText);
  }

  /// <summary>
  /// Fluent interface for completing the <see cref="IControlObjectWithRowsWhereColumnContains{TCellControlObject}.GetRowWhere()"/> call.
  /// </summary>
  public interface IFluentControlObjectWithRowsWhereColumnContains<TRowControlObject>
      where TRowControlObject : ControlObject
  {
    /// <summary>
    /// Selects the row which contains the <paramref name="containsCellText"/> in the column given by <paramref name="itemID"/>.
    /// </summary>
    TRowControlObject ColumnWithItemIDContains ([NotNull] string itemID, [NotNull] string containsCellText);

    /// <summary>
    /// Selects the row which contains the <paramref name="containsCellText"/> in the column given by <paramref name="index"/>.
    /// </summary>
    TRowControlObject ColumnWithIndexContains (int index, [NotNull] string containsCellText);

    /// <summary>
    /// Selects the row which contains exactly the <paramref name="containsCellText"/> in the column given by <paramref name="title"/>.
    /// </summary>
    TRowControlObject ColumnWithTitleContainsExactly ([NotNull] string title, [NotNull] string containsCellText);

    /// <summary>
    /// Selects the row which contains the <paramref name="containsCellText"/> in the column given by <paramref name="title"/>.
    /// </summary>
    TRowControlObject ColumnWithTitleContains ([NotNull] string title, [NotNull] string containsCellText);
  }
}