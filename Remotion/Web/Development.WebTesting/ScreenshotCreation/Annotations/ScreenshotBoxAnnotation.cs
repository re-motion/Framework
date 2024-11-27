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
using Microsoft.Maui.Graphics;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.SystemDrawingImitators;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations
{
  /// <summary>
  /// Draws a box around the annotation target with the specified <see cref="Pen"/> and <see cref="Padding"/>.
  /// </summary>
  public class ScreenshotBoxAnnotation : IScreenshotAnnotation
  {
    private readonly Brush? _backgroundBrush;
    private readonly WebPadding _padding;
    private readonly Pen _pen;

    public ScreenshotBoxAnnotation ([NotNull] Pen pen, WebPadding padding, [CanBeNull] Brush? backgroundBrush)
    {
      ArgumentUtility.CheckNotNull("pen", pen);

      _pen = pen;
      _padding = padding;
      _backgroundBrush = backgroundBrush;
    }

    /// <summary>
    /// The <see cref="Brush"/> that will be used to fill the inside of the box annotation, 
    /// or <see langword="null" /> if the inside of the box should not be filled.
    /// </summary>
    [CanBeNull]
    public Brush? BackgroundBrush
    {
      get { return _backgroundBrush; }
    }

    /// <summary>
    /// The <see cref="WebPadding"/> that will be applied to the element before drawing the box annotation.
    /// </summary>
    public WebPadding Padding
    {
      get { return _padding; }
    }

    /// <summary>
    /// The pen that will be used to draw the box annotation.
    /// </summary>
    [NotNull]
    public Pen Pen
    {
      get { return _pen; }
    }

    /// <inheritdoc />
    public void Draw (Graphics graphics, ResolvedScreenshotElement resolvedScreenshotElement)
    {
      ArgumentUtility.CheckNotNull("graphics", graphics);
      ArgumentUtility.CheckNotNull("resolvedScreenshotElement", resolvedScreenshotElement);

      // Calculate the bound of the annotation with padding
      var annotationBounds = _padding.Apply(resolvedScreenshotElement.ElementBounds);

      // Draw the background if there is one
      if (_backgroundBrush != null)
        graphics.FillRectangle(_backgroundBrush, annotationBounds);

      // Apply the padding for the border
      var border = (int)Math.Floor(_pen.Width);
      var xyOffset = (border - 1) / 2;
      var whOffset = border / 2;
      var borderBounds = new WebPadding(xyOffset + 1, xyOffset + 1, whOffset, whOffset).Apply(annotationBounds);

      // Draw the border by drawing 5 lines. GDI+ is somehow
      // unable to draw rectangles in certain situations
      graphics.DrawLines(
          _pen,
          new[]
          {
              borderBounds.Location,
              borderBounds.Location + new Size(borderBounds.Width, 0),
              borderBounds.Location + borderBounds.Size,
              borderBounds.Location + new Size(0, borderBounds.Height),
              borderBounds.Location,
              borderBounds.Location + new Size(borderBounds.Width, 0)
          });
    }
  }
}
