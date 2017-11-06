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
using System.Drawing;
using JetBrains.Annotations;
using OpenQA.Selenium.Internal;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList
{
  /// <summary>
  /// Marker class for the screenshot fluent API. Represents a <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/> column.
  /// </summary>
  public class ScreenshotBocListColumn<TList, TRow, TCell> : ISelfResolvable
      where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
      where TRow : ControlObject, IControlObjectWithCells<TCell>
      where TCell : ControlObject
  {
    private readonly IFluentScreenshotElement<ScreenshotBocList<TList, TRow, TCell>> _fluentList;
    private readonly int _columnIndex;
    private readonly bool _includeHeader;

    public ScreenshotBocListColumn (
        [NotNull] IFluentScreenshotElement<ScreenshotBocList<TList, TRow, TCell>> fluentList,
        int columnIndex,
        bool includeHeader)
    {
      ArgumentUtility.CheckNotNull ("fluentList", fluentList);

      _fluentList = fluentList;
      _columnIndex = columnIndex;
      _includeHeader = includeHeader;
    }

    public ScreenshotBocListFluentColumnCellSelector<TList, TRow, TCell> GetCellSelector ()
    {
      return new ScreenshotBocListFluentColumnCellSelector<TList, TRow, TCell> (_fluentList, _columnIndex);
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveBrowserCoordinates ()
    {
      return ResolveInformation (CoordinateSystem.Browser, Point.Empty);
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveDesktopCoordinates (IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull ("locator", locator);

      var window = locator.GetBrowserContentBounds (((IWrapsDriver) _fluentList.Target.List.Scope.Native).WrappedDriver);
      return ResolveInformation (CoordinateSystem.Desktop, window.Location);
    }

    private ResolvedScreenshotElement ResolveInformation (CoordinateSystem coordinateSystem, Point offset)
    {
      var container = _fluentList.GetTableContainer();

      var firstElement = ((IFluentScreenshotElement) container.GetRow (1).GetCell (_columnIndex)).ResolveBrowserCoordinates();
      ResolvedScreenshotElement from;
      Rectangle? parent;
      if (_includeHeader)
      {
        from = ((IFluentScreenshotElement) container.GetHeaderRow().GetCell (_columnIndex)).ResolveBrowserCoordinates();
        parent = ((IFluentScreenshotElement) container).ResolveBrowserCoordinates().ElementBounds;
      }
      else
      {
        from = firstElement;
        parent = from.ParentBounds;
      }

      var to = ((IFluentScreenshotElement) container.GetRow (container.GetRowCount()).GetCell (_columnIndex)).ResolveBrowserCoordinates();

      ElementVisibility visibility;
      if (firstElement.ElementVisibility == ElementVisibility.FullyVisible && from.ElementVisibility == ElementVisibility.FullyVisible)
        visibility = ElementVisibility.FullyVisible;
      else
        visibility = ElementVisibility.PartiallyVisible;

      var columnBounds = new Rectangle (
          to.ElementBounds.X,
          from.ElementBounds.Y,
          to.ElementBounds.Width,
          to.ElementBounds.Y - from.ElementBounds.Y + to.ElementBounds.Height);

      columnBounds.Offset (offset);
      if (parent.HasValue)
      {
        var parentBounds = parent.Value;
        parentBounds.Offset (offset);
        parent = parentBounds;
      }

      return new ResolvedScreenshotElement (coordinateSystem, columnBounds, visibility, parent);
    }
  }
}