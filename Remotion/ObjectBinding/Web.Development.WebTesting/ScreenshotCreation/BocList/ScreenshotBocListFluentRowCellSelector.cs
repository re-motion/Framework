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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent.Selectors;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList
{
  public class ScreenshotBocListFluentRowCellSelector<TList, TRow, TCell>
      : IFluentItemIDSelector<ScreenshotBocListCell<TList, TRow, TCell>>,
          IFluentIndexSelector<ScreenshotBocListCell<TList, TRow, TCell>>,
          IFluentTitleSelector<ScreenshotBocListCell<TList, TRow, TCell>>,
          IFluentTitleContainsSelector<ScreenshotBocListCell<TList, TRow, TCell>>
      where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
      where TRow : ControlObject, IBocListRowControlObject<TCell>
      where TCell : ControlObject
  {
    private readonly IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> _fluentList;
    private readonly IFluentScreenshotElement<TRow> _fluentRow;

    public ScreenshotBocListFluentRowCellSelector (
        [NotNull] IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
        [NotNull] IFluentScreenshotElement<TRow> fluentRow)
    {
      ArgumentUtility.CheckNotNull ("fluentList", fluentList);
      ArgumentUtility.CheckNotNull ("fluentRow", fluentRow);

      _fluentList = fluentList;
      _fluentRow = fluentRow;
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNull ("itemID", itemID);

      return
          SelfResolvableFluentScreenshot.Create (
              new ScreenshotBocListCell<TList, TRow, TCell> (_fluentList, _fluentRow.Target.GetCell (itemID).ForControlObjectScreenshot()),
              ElementVisibility.PartiallyVisible);
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> WithIndex (int oneBasedIndex)
    {
      ArgumentUtility.CheckNotNull ("oneBasedIndex", oneBasedIndex);

      return
          SelfResolvableFluentScreenshot.Create (
              new ScreenshotBocListCell<TList, TRow, TCell> (_fluentList, _fluentRow.Target.GetCell (oneBasedIndex).ForControlObjectScreenshot()),
              ElementVisibility.PartiallyVisible);
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> WithTitle (string title)
    {
      ArgumentUtility.CheckNotNull ("title", title);

      return
          SelfResolvableFluentScreenshot.Create (
              new ScreenshotBocListCell<TList, TRow, TCell> (_fluentList, _fluentRow.Target.GetCell().WithColumnTitle (title).ForControlObjectScreenshot()),
              ElementVisibility.PartiallyVisible);
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ScreenshotBocListCell<TList, TRow, TCell>> WithTitleContains (string content)
    {
      ArgumentUtility.CheckNotNull ("content", content);

      return
          SelfResolvableFluentScreenshot.Create (
              new ScreenshotBocListCell<TList, TRow, TCell> (
                  _fluentList,
                  _fluentRow.Target.GetCell().WithColumnTitleContains (content).ForControlObjectScreenshot()),
              ElementVisibility.PartiallyVisible);
    }
  }
}