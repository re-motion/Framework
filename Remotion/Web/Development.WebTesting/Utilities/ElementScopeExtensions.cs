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
using System.Collections.Generic;
using System.Threading;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Extension methods for <see cref="ElementScope"/>.
  /// </summary>
  public static class ElementScopeExtensions
  {
    /// <summary>
    /// Returns the current scrolling position of <paramref name="element"/>.
    /// </summary>
    public static Point GetScrollPosition ([NotNull] this ElementScope element)
    {
      ArgumentUtility.CheckNotNull("element", element);

      var driver = ((IWrapsDriver)element.Native).WrappedDriver;
      var jsExecutor = (IJavaScriptExecutor)driver;

      var rawData =
          (IReadOnlyList<object>)jsExecutor.ExecuteScript("return [arguments[0].scrollLeft, arguments[0].scrollTop];", (IWebElement)element.Native);
      return new Point((int)(long)rawData[0], (int)(long)rawData[1]);
    }

    /// <summary>
    /// Moves the scrollbars of <paramref name="element"/> so that (<paramref name="x"/>, <paramref name="y"/>) are at the top left corner of the image.
    /// </summary>
    /// <remarks>
    /// This methods sets the <c>scrollLeft</c> and <c>scrollTop</c> values of the element and therefore:
    ///  - Values smaller than <c>0</c> are set to <c>0</c>.
    ///  - Values that are bigger than the max. scroll amount are set to the max. scroll amount.
    /// </remarks>
    public static void ScrollTo ([NotNull] this ElementScope element, int x, int y)
    {
      ArgumentUtility.CheckNotNull("element", element);

      var driver = ((IWrapsDriver)element.Native).WrappedDriver;
      var jsExecutor = (IJavaScriptExecutor)driver;

      jsExecutor.ExecuteScript("arguments[0].scrollLeft=arguments[1];arguments[0].scrollTop=arguments[2];", (IWebElement)element.Native, x, y);

      // Wait a bit for the browser to catch up (redraw at the correct position)
      Thread.Sleep(50);
    }

    /// <summary>
    /// Moves the scrollbars of <paramref name="element"/> so that <paramref name="target"/> is in the visible area.
    /// </summary>
    public static void ScrollToElement (
        [NotNull] this ElementScope element,
        [NotNull] ElementScope target,
        ContentAlignment? alignment = null,
        WebPadding? padding = null)
    {
      ArgumentUtility.CheckNotNull("element", element);
      ArgumentUtility.CheckNotNull("target", target);

      var elementBounds = ElementScopeResolver.Instance.ResolveBrowserCoordinates(element).ElementBounds;
      var targetBounds = ElementScopeResolver.Instance.ResolveBrowserCoordinates(target).ElementBounds;

      var position = GetScrollPoint(
          elementBounds.Size,
          new Rectangle(targetBounds.Location - new Size(elementBounds.Location) + new Size(element.GetScrollPosition()), targetBounds.Size),
          alignment ?? ContentAlignment.TopLeft,
          padding ?? WebPadding.None);

      element.ScrollTo((int)Math.Round(position.X), (int)Math.Round(position.Y));
    }

    private static PointF GetScrollPoint (Size scrollContainerSize, Rectangle targetElementBounds, ContentAlignment alignment, WebPadding padding)
    {
      switch (alignment)
      {
        case ContentAlignment.TopLeft:
          return new PointF(
              targetElementBounds.X - 1 - padding.Left,
              targetElementBounds.Y - 1 - padding.Top);
        case ContentAlignment.TopCenter:
          return new PointF(
              targetElementBounds.X - (scrollContainerSize.Width - targetElementBounds.Width) / 2f,
              targetElementBounds.Y - 1 - padding.Top);
        case ContentAlignment.TopRight:
          return new PointF(
              targetElementBounds.X - (scrollContainerSize.Width - targetElementBounds.Width) + 1 + padding.Right,
              targetElementBounds.Y - 1 - padding.Top);
        case ContentAlignment.MiddleLeft:
          return new PointF(
              targetElementBounds.X - 1 - padding.Left,
              targetElementBounds.Y - (scrollContainerSize.Height - targetElementBounds.Height) / 2f);
        case ContentAlignment.MiddleCenter:
          return new PointF(
              targetElementBounds.X - (scrollContainerSize.Width - targetElementBounds.Width) / 2f,
              targetElementBounds.Y - (scrollContainerSize.Height - targetElementBounds.Height) / 2f);
        case ContentAlignment.MiddleRight:
          return new PointF(
              targetElementBounds.X - (scrollContainerSize.Width - targetElementBounds.Width) + 1 + padding.Right,
              targetElementBounds.Y - (scrollContainerSize.Height - targetElementBounds.Height) / 2f);
        case ContentAlignment.BottomLeft:
          return new PointF(
              targetElementBounds.X - 1 - padding.Left,
              targetElementBounds.Y - (scrollContainerSize.Width - targetElementBounds.Width) + 1 + padding.Bottom);
        case ContentAlignment.BottomCenter:
          return new PointF(
              targetElementBounds.X - (scrollContainerSize.Width - targetElementBounds.Width) / 2f,
              targetElementBounds.Y - (scrollContainerSize.Width - targetElementBounds.Width) + 1 + padding.Bottom);
        case ContentAlignment.BottomRight:
          return new PointF(
              targetElementBounds.X - (scrollContainerSize.Width - targetElementBounds.Width) + 1 + padding.Right,
              targetElementBounds.Y - (scrollContainerSize.Width - targetElementBounds.Width) + 1 + padding.Bottom);
        default:
          throw new ArgumentOutOfRangeException("alignment", alignment, null);
      }
    }
  }
}
