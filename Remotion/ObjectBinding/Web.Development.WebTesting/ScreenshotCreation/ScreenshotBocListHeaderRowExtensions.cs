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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides fluent extension methods for navigating a <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/> table header-row.
  /// </summary>
  public static class ScreenshotBocListHeaderRowExtensions
  {
    /// <summary>
    /// Starts the fluent selection for a specific cell in the header row.
    /// </summary>
    public static ScreenshotBocListFluentHeaderCellSelector<TList, TRow, TCell> GetCell<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocListHeaderRow<TList, TRow, TCell>> fluentHeaderRow)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull("fluentHeaderRow", fluentHeaderRow);

      return fluentHeaderRow.Target.GetCellSelector();
    }

    /// <summary>
    /// Selects a cell in the header row with the specified <paramref name="itemID"/>.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetCell<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocListHeaderRow<TList, TRow, TCell>> fluentHeaderRow,
        [NotNull] string itemID)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull("fluentHeaderRow", fluentHeaderRow);
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);

      return fluentHeaderRow.Target.GetCellSelector().WithItemID(itemID);
    }

    /// <summary>
    /// Selects a cell in the header row with the specified <paramref name="oneBasedIndex"/>.
    /// </summary>
    public static FluentScreenshotElement<ElementScope> GetCell<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocListHeaderRow<TList, TRow, TCell>> fluentHeaderRow,
        int oneBasedIndex)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull("fluentHeaderRow", fluentHeaderRow);

      return fluentHeaderRow.Target.GetCellSelector().WithIndex(oneBasedIndex);
    }

    /// <summary>
    /// Returns the error marker for this header row.
    /// </summary>
    /// <exception cref="InvalidOperationException">The error marker could not be found.</exception>
    public static FluentScreenshotElement<ElementScope> GetErrorMarker<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElementWithCovariance<ScreenshotBocListHeaderRow<TList, TRow, TCell>> fluentRow)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IBocListRowControlObject<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull("fluentRow", fluentRow);

      var result = fluentRow.Target.Element.FindCss("th.bocListTitleCellValidationFailureIndicator img", Options.NoWait);
      if (!result.Exists(Options.NoWait))
        throw new InvalidOperationException("Can not find a error marker for this row.");

      return result.ForElementScopeScreenshot();
    }
  }
}
