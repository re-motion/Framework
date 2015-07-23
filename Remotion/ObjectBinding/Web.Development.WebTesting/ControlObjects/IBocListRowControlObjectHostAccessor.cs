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
using Coypu;
using JetBrains.Annotations;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Accessor methods for <see cref="BocListRowControlObject"/> and <see cref="BocListEditableRowControlObject"/>.
  /// </summary>
  public interface IBocListRowControlObjectHostAccessor
  {
    /// <summary>
    /// Returns the scope of the parent control.
    /// </summary>
    ElementScope ParentScope { get; }

    /// <summary>
    /// Returns the one-based column index of the given <paramref name="columnItemID"/> within the parent control.
    /// </summary>
    int GetColumnIndexForItemID ([NotNull] string columnItemID);

    /// <summary>
    /// Returns the one-based column index of the given <paramref name="columnTitle"/> within the parent control.
    /// </summary>
    int GetColumnIndexForTitle ([NotNull] string columnTitle);

    /// <summary>
    /// Returns the one-based column index of the given <paramref name="columnTitleContains"/> within the parent control.
    /// </summary>
    int GetColumnIndexForTitleContains ([NotNull] string columnTitleContains);

    /// <summary>
    /// Returns the zero-based absolute row index of the first row on the current page.
    /// </summary>
    int GetZeroBasedAbsoluteRowIndexOfFirstRow ();
  }
}