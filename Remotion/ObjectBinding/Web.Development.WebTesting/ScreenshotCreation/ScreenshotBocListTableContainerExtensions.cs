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
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Provides fluent extension methods for controlling the <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/> table.
  /// </summary>
  public static class ScreenshotBocListTableContainerExtensions
  {
    /// <summary>
    /// Starts the fluent selection for a specific cell in the specified <paramref name="fluentColumn"/>.
    /// </summary>
    public static ScreenshotBocListFluentColumnCellSelector<TList, TRow, TCell> GetCell<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListColumn<TList, TRow, TCell>> fluentColumn)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentColumn", fluentColumn);

      return fluentColumn.Target.GetCellSelector();
    }

    /// <summary>
    /// Selects a cell in the specified <paramref name="fluentColumn"/> with the specified <paramref name="rowItemID"/>.
    /// </summary>
    public static FluentControlHostScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> GetCell<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListColumn<TList, TRow, TCell>> fluentColumn,
        [NotNull] string rowItemID)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentColumn", fluentColumn);
      ArgumentUtility.CheckNotNull ("rowItemID", rowItemID);

      return
          new FluentControlHostScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> (
              fluentColumn.Target.GetCellSelector().WithItemID (rowItemID));
    }

    /// <summary>
    /// Selects a cell in the specified <paramref name="fluentColumn"/> with the specified <paramref name="oneBasedRowIndex"/>.
    /// </summary>
    public static FluentControlHostScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> GetCell<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListColumn<TList, TRow, TCell>> fluentColumn,
        int oneBasedRowIndex)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentColumn", fluentColumn);

      return
          new FluentControlHostScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> (
              fluentColumn.Target.GetCellSelector().WithIndex (oneBasedRowIndex));
    }

    /// <summary>
    /// Starts the fluent selection for a specific cell in the specified <paramref name="fluentRow"/>.
    /// </summary>
    public static ScreenshotBocListFluentRowCellSelector<TList, TRow, TCell> GetCell<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListRow<TList, TRow, TCell>> fluentRow)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentRow", fluentRow);

      return fluentRow.Target.GetCellSelector();
    }

    /// <summary>
    /// Selects a cell in the specified <paramref name="fluentRow"/> with the specified <paramref name="columnItemID"/>.
    /// </summary>
    public static FluentControlHostScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> GetCell<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListRow<TList, TRow, TCell>> fluentRow,
        [NotNull] string columnItemID)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentRow", fluentRow);
      ArgumentUtility.CheckNotNull ("columnItemID", columnItemID);

      return
          new FluentControlHostScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> (
              fluentRow.Target.GetCellSelector().WithItemID (columnItemID));
    }

    /// <summary>
    /// Selects a cell in the specified <paramref name="fluentRow"/> with the specified <paramref name="oneBasedColumnIndex"/>.
    /// </summary>
    public static FluentControlHostScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> GetCell<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListRow<TList, TRow, TCell>> fluentRow,
        int oneBasedColumnIndex)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentRow", fluentRow);

      return
          new FluentControlHostScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> (
              fluentRow.Target.GetCellSelector().WithIndex (oneBasedColumnIndex));
    }

    /// <summary>
    /// Starts the fluent selection for a specific column in the specified <paramref name="fluentTableContainer"/>.
    /// </summary>
    public static ScreenshotBocListFluentColumnSelector<TList, TRow, TCell> GetColumn<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListTableContainer<TList, TRow, TCell>> fluentTableContainer,
        bool includeHeader = true)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentTableContainer", fluentTableContainer);

      return new ScreenshotBocListFluentColumnSelector<TList, TRow, TCell> (fluentTableContainer.Target.FluentList, includeHeader);
    }

    /// <summary>
    /// Selects a column in the specified <paramref name="fluentTableContainer"/> with the specified <paramref name="columnItemID"/>.
    /// </summary>
    public static FluentScreenshotElement<ScreenshotBocListColumn<TList, TRow, TCell>> GetColumn<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListTableContainer<TList, TRow, TCell>> fluentTableContainer,
        [NotNull] string columnItemID,
        bool includeHeader = true)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentTableContainer", fluentTableContainer);
      ArgumentUtility.CheckNotNull ("columnItemID", columnItemID);

      return fluentTableContainer.GetColumn (includeHeader).WithItemID (columnItemID);
    }

    /// <summary>
    /// Selects a column in the specified <paramref name="fluentTableContainer"/> with the specified <paramref name="oneBasedColumnIndex"/>.
    /// </summary>
    public static FluentScreenshotElement<ScreenshotBocListColumn<TList, TRow, TCell>> GetColumn<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListTableContainer<TList, TRow, TCell>> fluentTableContainer,
        int oneBasedColumnIndex,
        bool includeHeader = true)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentTableContainer", fluentTableContainer);

      return fluentTableContainer.GetColumn (includeHeader).WithIndex (oneBasedColumnIndex);
    }

    /// <summary>
    /// Starts the fluent selection for a navigating the header row of the table.
    /// </summary>
    public static IFluentScreenshotElement<ScreenshotBocListHeaderRow<TList, TRow, TCell>> GetHeaderRow<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListTableContainer<TList, TRow, TCell>> fluentTableContainer)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentTableContainer", fluentTableContainer);

      var element = fluentTableContainer.Target.Element.FindCss (".bocListTable .bocListTableHead", Options.NoWait);
      if (!element.Exists (Options.NoWait))
        throw new MissingHtmlException ("Could not find the header row.");

      return
          SelfResolvableFluentScreenshot.Create (
              new ScreenshotBocListHeaderRow<TList, TRow, TCell> (fluentTableContainer.Target.FluentList, element.ForElementScopeScreenshot()), ElementVisibility.PartiallyVisible);
    }

    /// <summary>
    /// Starts the fluent selection for a specific row in the specified <paramref name="fluentTableContainer"/>.
    /// </summary>
    public static ScreenshotBocListFluentRowSelector<TList, TRow, TCell> GetRow<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListTableContainer<TList, TRow, TCell>> fluentTableContainer)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentTableContainer", fluentTableContainer);

      return new ScreenshotBocListFluentRowSelector<TList, TRow, TCell> (fluentTableContainer.Target.FluentList);
    }

    /// <summary>
    /// Selects a row in the specified <paramref name="fluentTableContainer"/> with the specified <paramref name="rowItemID"/>.
    /// </summary>
    public static FluentScreenshotElement<ScreenshotBocListRow<TList, TRow, TCell>> GetRow<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListTableContainer<TList, TRow, TCell>> fluentTableContainer,
        [NotNull] string rowItemID)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentTableContainer", fluentTableContainer);
      ArgumentUtility.CheckNotNull ("rowItemID", rowItemID);

      return fluentTableContainer.GetRow().WithItemID (rowItemID);
    }

    /// <summary>
    /// Selects a row in the specified <paramref name="fluentTableContainer"/> with the specified <paramref name="oneBasedRowIndex"/>.
    /// </summary>
    public static FluentScreenshotElement<ScreenshotBocListRow<TList, TRow, TCell>> GetRow<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListTableContainer<TList, TRow, TCell>> fluentTableContainer,
        int oneBasedRowIndex)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentTableContainer", fluentTableContainer);

      return fluentTableContainer.GetRow().WithIndex (oneBasedRowIndex);
    }

    /// <summary>
    /// Returns the amount of rows currently displayed in the table.
    /// </summary>
    public static int GetRowCount<TList, TRow, TCell> (
        [NotNull] this IFluentScreenshotElement<ScreenshotBocListTableContainer<TList, TRow, TCell>> fluentTableContainer)
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      return fluentTableContainer.Target.List.GetNumberOfRows();
    }
  }
}