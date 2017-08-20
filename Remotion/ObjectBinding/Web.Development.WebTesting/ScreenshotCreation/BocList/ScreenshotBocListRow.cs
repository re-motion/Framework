﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList
{
  /// <summary>
  /// Marker class for the screenshot fluent API. Represents a <see cref="BocListControlObjectBase{TRowControlObject,TCellControlObject}"/> row.
  /// </summary>
  public class ScreenshotBocListRow<TList, TRow, TCell> : ISelfResolvable
      where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
      where TRow : ControlObject, IControlObjectWithCells<TCell>
      where TCell : ControlObject
  {
    private readonly IFluentScreenshotElement<ScreenshotBocList<TList, TRow, TCell>> _fluentList;
    private readonly IFluentScreenshotElement<TRow> _fluentRow;

    public ScreenshotBocListRow (IFluentScreenshotElement<ScreenshotBocList<TList, TRow, TCell>> fluentList, IFluentScreenshotElement<TRow> fluentRow)
    {
      _fluentList = fluentList;
      _fluentRow = fluentRow;
    }

    public ScreenshotBocListFluentRowCellSelector<TList, TRow, TCell> GetCellSelector ()
    {
      return new ScreenshotBocListFluentRowCellSelector<TList, TRow, TCell> (_fluentList, _fluentRow);
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveBrowserCoordinates ()
    {
      return _fluentRow.ResolveBrowserCoordinates();
    }

    /// <inheritdoc />
    public ResolvedScreenshotElement ResolveDesktopCoordinates (IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull ("locator", locator);

      return _fluentRow.ResolveDesktopCoordinates (locator);
    }
  }
}