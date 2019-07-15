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
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent.Selectors;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList
{
  public class ScreenshotBocListFluentHeaderCellSelector<TList, TRow, TCell>
      : IFluentItemIDSelector<ElementScope>,
          IFluentIndexSelector<ElementScope>,
          IFluentTitleSelector<ElementScope>,
          IFluentTitleContainsSelector<ElementScope>
      where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
      where TRow : ControlObject, IBocListRowControlObject<TCell>
      where TCell : ControlObject
  {
    private readonly IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> _fluentList;
    private readonly IFluentScreenshotElement<ElementScope> _fluentElement;

    public ScreenshotBocListFluentHeaderCellSelector (
        [NotNull] IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
        [NotNull] IFluentScreenshotElement<ElementScope> fluentElement)
    {
      ArgumentUtility.CheckNotNull ("fluentList", fluentList);
      ArgumentUtility.CheckNotNull ("fluentElement", fluentElement);

      _fluentList = fluentList;
      _fluentElement = fluentElement;
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ElementScope> WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNull ("itemID", itemID);

      var columns = _fluentList.Target.List.GetColumnDefinitions().Where (c => c.ItemID == itemID).Take (2).ToArray();

      if (columns.Length == 0)
        throw new MissingHtmlException (String.Format ("Could not find a header row with the specified item ID '{0}'.", itemID));
      if (columns.Length > 1)
        throw new AmbiguousException (String.Format ("There are multiple header rows with the same item ID '{0}'.", itemID));

      var element = _fluentElement.Target.FindTagWithAttribute (
          "th",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          columns[0].Index.ToString());

      return FluentUtility.CreateFluentElementScope (element, ElementVisibility.PartiallyVisible);
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ElementScope> WithIndex (int oneBasedIndex)
    {
      var element = _fluentElement.Target.FindTagWithAttribute (
          "th",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          oneBasedIndex.ToString());

      return FluentUtility.CreateFluentElementScope (element, ElementVisibility.PartiallyVisible);
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ElementScope> WithTitle (string title)
    {
      ArgumentUtility.CheckNotNull ("title", title);

      var columns = _fluentList.Target.List.GetColumnDefinitions().Where (c => c.Title == title).Take (2).ToArray();

      if (columns.Length == 0)
        throw new MissingHtmlException (String.Format ("Could not find a header row with the specified title '{0}'.", title));
      if (columns.Length > 1)
        throw new AmbiguousException (String.Format ("There are multiple header rows with the same title '{0}'.", title));

      var element = _fluentElement.Target.FindTagWithAttribute (
          "th",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          columns[0].Index.ToString());

      return FluentUtility.CreateFluentElementScope (element, ElementVisibility.PartiallyVisible);
    }

    /// <inheritdoc />
    public FluentScreenshotElement<ElementScope> WithTitleContains (string content)
    {
      ArgumentUtility.CheckNotNull ("content", content);

      var columns = _fluentList.Target.List.GetColumnDefinitions().Where (c => c.Title.Contains (content)).Take (2).ToArray();

      if (columns.Length == 0)
        throw new MissingHtmlException (String.Format ("Could not find a header row where the title contains '{0}'.", content));
      if (columns.Length > 1)
        throw new AmbiguousException (String.Format ("There are multiple header rows where title contain '{0}'.", content));

      var element = _fluentElement.Target.FindTagWithAttribute (
          "th",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          columns[0].Index.ToString());

      return FluentUtility.CreateFluentElementScope (element, ElementVisibility.PartiallyVisible);
    }
  }
}