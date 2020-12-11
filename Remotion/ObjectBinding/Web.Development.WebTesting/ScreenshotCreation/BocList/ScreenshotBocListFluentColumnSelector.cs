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
using System.Linq;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent.Selectors;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList
{
  public class ScreenshotBocListFluentColumnSelector<TList, TRow, TCell>
      : IFluentItemIDSelector<ScreenshotBocListColumn<TList, TRow, TCell>>,
          IFluentIndexSelector<ScreenshotBocListColumn<TList, TRow, TCell>>,
          IFluentTitleSelector<ScreenshotBocListColumn<TList, TRow, TCell>>,
          IFluentTitleContainsSelector<ScreenshotBocListColumn<TList, TRow, TCell>>
      where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
      where TRow : ControlObject, IBocListRowControlObject<TCell>
      where TCell : ControlObject
  {
    private readonly IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> _fluentList;
    private readonly bool _includeHeader;

    public ScreenshotBocListFluentColumnSelector (
        [NotNull] IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
        bool includeHeader)
    {
      ArgumentUtility.CheckNotNull ("fluentList", fluentList);

      _fluentList = fluentList;
      _includeHeader = includeHeader;
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ScreenshotBocListColumn<TList, TRow, TCell>> WithItemID (string itemID)
    {
      var columns = _fluentList.Target.List.GetColumnDefinitions();
      var column = columns.Where (c => c.ItemID == itemID).Take (2).ToArray();

      if (column.Length == 0)
        throw AssertionExceptionUtility.CreateExpectationException (_fluentList.Target.List.Driver, "Could not find a column with the specified item ID '{0}'.", itemID);
      if (column.Length > 1)
        throw AssertionExceptionUtility.CreateExpectationException (_fluentList.Target.List.Driver, "There are multiple columns with the same item ID '{0}'.", itemID);

      return SelfResolvableFluentScreenshot.Create (
          new ScreenshotBocListColumn<TList, TRow, TCell> (_fluentList, column[0].Index, _includeHeader),
          ElementVisibility.PartiallyVisible);
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ScreenshotBocListColumn<TList, TRow, TCell>> WithIndex (int oneBasedIndex)
    {
      var columns = _fluentList.Target.List.GetColumnDefinitions();
      var column = columns.Where (c => c.Index == oneBasedIndex).Take (2).ToArray();

      if (column.Length == 0)
        throw AssertionExceptionUtility.CreateExpectationException (_fluentList.Target.List.Driver, "Could not find a column with the specified index '{0}'.", oneBasedIndex);
      if (column.Length > 1)
        throw AssertionExceptionUtility.CreateExpectationException (_fluentList.Target.List.Driver, "There are multiple columns with the same index '{0}'.", oneBasedIndex);

      return SelfResolvableFluentScreenshot.Create (
          new ScreenshotBocListColumn<TList, TRow, TCell> (_fluentList, column[0].Index, _includeHeader),
          ElementVisibility.PartiallyVisible);
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ScreenshotBocListColumn<TList, TRow, TCell>> WithTitle (string title)
    {
      var columns = _fluentList.Target.List.GetColumnDefinitions();
      var column = columns.Where (c => c.Title == title).Take (2).ToArray();

      if (column.Length == 0)
        throw AssertionExceptionUtility.CreateExpectationException (_fluentList.Target.List.Driver, "Could not find a column with the specified title '{0}'.", title);
      if (column.Length > 1)
        throw AssertionExceptionUtility.CreateExpectationException (_fluentList.Target.List.Driver, "There are multiple columns with the same title '{0}'.", title);

      return SelfResolvableFluentScreenshot.Create (
          new ScreenshotBocListColumn<TList, TRow, TCell> (_fluentList, column[0].Index, _includeHeader),
          ElementVisibility.PartiallyVisible);
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ScreenshotBocListColumn<TList, TRow, TCell>> WithTitleContains (string content)
    {
      var columns = _fluentList.Target.List.GetColumnDefinitions();
      var column = columns.Where (c => c.Title.Contains (content)).Take (2).ToArray();
      if (column.Length == 0)
        throw AssertionExceptionUtility.CreateExpectationException (_fluentList.Target.List.Driver, "Could not find a column where the title contains '{0}'.", content);
      if (column.Length > 1)
        throw AssertionExceptionUtility.CreateExpectationException (_fluentList.Target.List.Driver, "There are multiple columns where the title contains '{0}'.", content);

      return SelfResolvableFluentScreenshot.Create (
          new ScreenshotBocListColumn<TList, TRow, TCell> (_fluentList, column[0].Index, _includeHeader),
          ElementVisibility.PartiallyVisible);
    }
  }
}