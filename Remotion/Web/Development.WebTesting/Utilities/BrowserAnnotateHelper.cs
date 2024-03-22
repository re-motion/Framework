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
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Provides helper methods for annotating elements.
  /// </summary>
  public class BrowserAnnotateHelper
  {
    public readonly IBrowserConfiguration BrowserConfiguration;

    public BrowserAnnotateHelper (IBrowserConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull("configuration", configuration);

      BrowserConfiguration = configuration;
    }

    /// <summary>
    /// Draws a tooltip with the specified <paramref name="content"/> at the mouse cursor position.
    /// </summary>
    public IFluentScreenshotElement<Rectangle> DrawCursorTooltip (
        [NotNull] ScreenshotBuilder builder,
        [NotNull] IBrowserSession browserSession,
        [NotNull] string content,
        ScreenshotTooltipStyle? style = null,
        WebPadding? padding = null,
        TooltipPositioning? positioning = null,
        bool? wrapLines = null,
        Size? maximumSize = null)
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("browserSession", browserSession);
      ArgumentUtility.CheckNotNull("content", content);

      var seleniumDriver = (IWebDriver)browserSession.Driver.Native;

      var clonedStyle = (style ?? BrowserConfiguration.TooltipStyle).Clone(positioning: positioning, wrapLines: wrapLines, maximumSize: maximumSize);
      var browserContentBounds = BrowserConfiguration.Locator.GetBrowserContentBounds(seleniumDriver).Location;

      // Offset the position of the cursor to translate it to the browser coordinate system.
      var cursorPosition = Cursor.Position;
      cursorPosition.Offset(-browserContentBounds.X, -browserContentBounds.Y);

      var tooltipAnnotation = new ScreenshotTooltipAnnotation(
          content,
          clonedStyle,
          padding ?? new WebPadding(0, 20, 0, 25));
      builder.Annotate(new Rectangle(cursorPosition, new Size(1, 1)), new RectangleResolver(seleniumDriver), tooltipAnnotation);

      return new FluentScreenshotElement<Rectangle>(tooltipAnnotation.TooltipBounds, new RectangleResolver(seleniumDriver));
    }

    /// <summary>
    /// Draws the tooltip associated with the specified <paramref name="controlObject"/>.
    /// </summary>
    public IFluentScreenshotElement<Rectangle> DrawTooltip (
        [NotNull] ScreenshotBuilder builder,
        [NotNull] ControlObject controlObject,
        ScreenshotTooltipStyle? style = null,
        WebPadding? padding = null,
        TooltipPositioning? positioning = null,
        bool? wrapLines = null,
        Size? maximumSize = null)
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("controlObject", controlObject);

      return DrawTooltip(builder, (IWebElement)controlObject.Scope.Native, style, padding, positioning, wrapLines, maximumSize);
    }

    /// <summary>
    /// Draws the tooltip associated with the specified <paramref name="element"/>.
    /// </summary>
    public IFluentScreenshotElement<Rectangle> DrawTooltip (
        [NotNull] ScreenshotBuilder builder,
        [NotNull] ElementScope element,
        ScreenshotTooltipStyle? style = null,
        WebPadding? padding = null,
        TooltipPositioning? positioning = null,
        bool? wrapLines = null,
        Size? maximumSize = null)
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("element", element);

      return DrawTooltip(builder, (IWebElement)element.Native, style, padding, positioning, wrapLines, maximumSize);
    }

    /// <summary>
    /// Draws the tooltip associated with the specified <paramref name="webElement"/>.
    /// </summary>
    public IFluentScreenshotElement<Rectangle> DrawTooltip (
        [NotNull] ScreenshotBuilder builder,
        [NotNull] IWebElement webElement,
        ScreenshotTooltipStyle? style = null,
        WebPadding? padding = null,
        TooltipPositioning? positioning = null,
        bool? wrapLines = null,
        Size? maximumSize = null)
    {
      ArgumentUtility.CheckNotNull("builder", builder);
      ArgumentUtility.CheckNotNull("webElement", webElement);

      var title = webElement.GetAttribute("title");
      if (title == null)
        throw new InvalidOperationException("Can not display the tooltip for the specified element as it has no title attribute.");

      var clonedStyle = (style ?? BrowserConfiguration.TooltipStyle).Clone(positioning: positioning, wrapLines: wrapLines, maximumSize: maximumSize);

      var tooltipAnnotation = new ScreenshotTooltipAnnotation(
          title,
          clonedStyle,
          padding ?? new WebPadding(0, 20, 0, 20));
      builder.Annotate(
          new Rectangle(
              WebElementResolver.Instance.ResolveBrowserCoordinates(webElement).ElementBounds.Location,
              new Size(1, 1)),
          new RectangleResolver(((IWrapsDriver)webElement).WrappedDriver),
          tooltipAnnotation);

      return new FluentScreenshotElement<Rectangle>(tooltipAnnotation.TooltipBounds, new RectangleResolver(((IWrapsDriver)webElement).WrappedDriver));
    }
  }
}
