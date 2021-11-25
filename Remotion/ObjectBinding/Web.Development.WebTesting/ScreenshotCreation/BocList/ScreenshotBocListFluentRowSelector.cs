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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent.Selectors;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList
{
  public class ScreenshotBocListFluentRowSelector<TList, TRow, TCell>
      : IFluentItemIDSelector<ScreenshotBocListRow<TList, TRow, TCell>>,
          IFluentIndexSelector<ScreenshotBocListRow<TList, TRow, TCell>>
      where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
      where TRow : ControlObject, IBocListRowControlObject<TCell>
      where TCell : ControlObject
  {
    private readonly IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> _fluentList;

    public ScreenshotBocListFluentRowSelector (IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList)
    {
      _fluentList = fluentList;
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ScreenshotBocListRow<TList, TRow, TCell>> WithItemID (string itemID)
    {
      var row = _fluentList.Target.List.GetRow(itemID).ForControlObjectScreenshot();

      return SelfResolvableFluentScreenshot.Create(
          new ScreenshotBocListRow<TList, TRow, TCell>(_fluentList, row),
          ElementVisibility.PartiallyVisible);
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ScreenshotBocListRow<TList, TRow, TCell>> WithIndex (int oneBasedIndex)
    {
      var row = _fluentList.Target.List.GetRow(oneBasedIndex).ForControlObjectScreenshot();

      return SelfResolvableFluentScreenshot.Create(
          new ScreenshotBocListRow<TList, TRow, TCell>(_fluentList, row),
          ElementVisibility.PartiallyVisible);
    }
  }
}