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
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations
{
  /// <summary>
  /// Draws a tooltip with the specified <see cref="ScreenshotTooltipStyle"/>.
  /// </summary>
  public class ScreenshotTooltipAnnotation : IScreenshotAnnotation
  {
    private readonly string _content;
    private readonly ScreenshotTooltipStyle _style;
    private readonly WebPadding _padding;
    private Rectangle? _tooltipBounds;

    public ScreenshotTooltipAnnotation (
        [NotNull] string content,
        [NotNull] ScreenshotTooltipStyle style,
        WebPadding padding)
    {
      ArgumentUtility.CheckNotNull("content", content);
      ArgumentUtility.CheckNotNull("style", style);

      _content = content;
      _style = style;
      _padding = padding;
    }

    /// <summary>
    /// The content string of the tooltip.
    /// </summary>
    public string Content
    {
      get { return _content; }
    }

    /// <summary>
    /// The <see cref="ScreenshotTooltipStyle"/> which will be used for drawing the style.
    /// </summary>
    public ScreenshotTooltipStyle Style
    {
      get { return _style; }
    }

    /// <summary>
    /// The bounds of the tooltip, available after the tooltip has been drawn.
    /// </summary>
    /// <exception cref="InvalidOperationException">The tooltip has not been drawn before accessing <see cref="TooltipBounds"/>.</exception>
    public Rectangle TooltipBounds
    {
      get
      {
        if (!_tooltipBounds.HasValue)
          throw new InvalidOperationException("The tooltip needs to be drawn before its bounds can be accessed.");

        return _tooltipBounds.Value;
      }
    }

    /// <inheritdoc />
    public void Draw (Graphics graphics, ResolvedScreenshotElement resolvedScreenshotElement)
    {
      ArgumentUtility.CheckNotNull("graphics", graphics);
      ArgumentUtility.CheckNotNull("resolvedScreenshotElement", resolvedScreenshotElement);

      // Prepare the StringFormat for laying out the text
      var stringFormat = new StringFormat();
      if (!_style.WrapLines)
      {
        stringFormat.FormatFlags = StringFormatFlags.NoWrap;
        stringFormat.Trimming = StringTrimming.EllipsisCharacter;
      }
      else
      {
        stringFormat.FormatFlags = StringFormatFlags.LineLimit;
        stringFormat.Trimming = StringTrimming.EllipsisWord;
      }

      // Calculate the maximum size of the tooltip
      var border = (int)Math.Round(_style.Border.Width);
      var maximumSize = new Size(
          2 * border + _style.ContentPadding.Horizontal + _style.MaximumSize.Width,
          2 * border + _style.ContentPadding.Vertical + _style.MaximumSize.Height);

      // calculate the layout of the content
      var layoutArea = maximumSize - new Size(border * 2 + _style.ContentPadding.Horizontal, border * 2 + _style.ContentPadding.Vertical);

      // Measure how much space the text needs
      var contentSizeF = graphics.MeasureString(_content, _style.Font, layoutArea, stringFormat);
      var contentSize = new Size((int)Math.Ceiling(contentSizeF.Width) + 1, (int)Math.Ceiling(contentSizeF.Height));

      // Calculate the bounds of the tooltip with border
      var desktopOffset = new Size(1 + _style.ContentPadding.Left, 1 + _style.ContentPadding.Top);
      var tooltipBounds = _style.ContentPadding.Apply(new Rectangle(resolvedScreenshotElement.ElementBounds.Location, contentSize));
      tooltipBounds.Offset(_style.ContentPadding.Left, _style.ContentPadding.Top);

      // Make sure the tooltip is as least as big as the minimum size
      if (tooltipBounds.Width < _style.MinimumSize.Width)
        tooltipBounds.Width = _style.MinimumSize.Width;
      if (tooltipBounds.Height < _style.MinimumSize.Height)
        tooltipBounds.Height = _style.MinimumSize.Height;

      // Reposition the tooltip based on the content alignment
      tooltipBounds.Location = PositionTooltipWithAlignment(_style.Positioning, resolvedScreenshotElement.ElementBounds, tooltipBounds);

      // Remember the original tooltip bounds needed to annotate the tooltip
      var originalTooltipLocation = PositionTooltipWithAlignment(_style.Positioning, resolvedScreenshotElement.UnresolvedBounds, tooltipBounds);
      _tooltipBounds = new Rectangle(originalTooltipLocation, tooltipBounds.Size);

      // Draw the box of the tooltip
      var boxAnnotation = new ScreenshotBoxAnnotation(_style.Border, WebPadding.None, _style.BackgroundBrush);
      boxAnnotation.Draw(graphics, new ResolvedScreenshotElement(CoordinateSystem.Browser, tooltipBounds, ElementVisibility.FullyVisible, null, _tooltipBounds.Value));

      // Draw the text content
      graphics.DrawString(
          _content,
          _style.Font,
          _style.ForegroundBrush,
          new Rectangle(tooltipBounds.Location + desktopOffset, contentSize),
          stringFormat);
    }

    private Point PositionTooltipWithAlignment (TooltipPositioning alignment, Rectangle element, Rectangle tooltip)
    {
      var centerX = element.X + element.Width / 2;

      switch (alignment)
      {
        case TooltipPositioning.TopLeft:
          return new Point(centerX - tooltip.Width + _padding.Left - _padding.Right, element.Y - _padding.Top - tooltip.Height);
        case TooltipPositioning.TopCenter:
          return new Point(centerX - tooltip.Width / 2, element.Y - _padding.Top - tooltip.Height);
        case TooltipPositioning.TopRight:
          return new Point(centerX + _padding.Left - _padding.Right, element.Y - _padding.Top - tooltip.Height);
        case TooltipPositioning.BottomLeft:
          return new Point(centerX - tooltip.Width + _padding.Left - _padding.Right, element.Bottom + _padding.Bottom);
        case TooltipPositioning.BottomCenter:
          return new Point(centerX - tooltip.Width / 2, element.Bottom + _padding.Bottom);
        case TooltipPositioning.BottomRight:
          return new Point(centerX + _padding.Left - _padding.Right, element.Bottom + _padding.Bottom);
        default:
          throw new ArgumentOutOfRangeException("alignment", alignment, null);
      }
    }
  }
}
