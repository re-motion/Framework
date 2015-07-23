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
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of cells, e.g. a row within a BOC list.
  /// </summary>
  public interface IControlObjectWithCells<TCellControlObject>
      where TCellControlObject : ControlObject
  {
    /// <summary>
    /// Start of the fluent interface for selecting a cell.
    /// </summary>
    IFluentControlObjectWithCells<TCellControlObject> GetCell ();

    /// <summary>
    /// Short for explicitly implemented <see cref="IFluentControlObjectWithCells{TCellControlObject}.WithColumnItemID"/>.
    /// </summary>
    TCellControlObject GetCell ([NotNull] string columnItemID);

    /// <summary>
    /// Short for explicitly implemented <see cref="IFluentControlObjectWithCells{TCellControlObject}.WithIndex"/>.
    /// </summary>
    TCellControlObject GetCell (int index);
  }

  /// <summary>
  /// Fluent interface for completing the <see cref="IControlObjectWithCells{TCellControlObject}.GetCell()"/> call.
  /// </summary>
  public interface IFluentControlObjectWithCells<TCellControlObject>
      where TCellControlObject : ControlObject
  {
    /// <summary>
    /// Selects the cell using the given <paramref name="columnItemID"/>.
    /// </summary>
    TCellControlObject WithColumnItemID ([NotNull] string columnItemID);

    /// <summary>
    /// Selects the cell using the given <paramref name="index"/>.
    /// </summary>
    TCellControlObject WithIndex (int index);

    /// <summary>
    /// Selects the cell using the given <paramref name="columnTitle"/>.
    /// </summary>
    TCellControlObject WithColumnTitle ([NotNull] string columnTitle);

    /// <summary>
    /// Selects the cell using the given <paramref name="columnTitleContains"/>.
    /// </summary>
    TCellControlObject WithColumnTitleContains ([NotNull] string columnTitleContains);
  }
}