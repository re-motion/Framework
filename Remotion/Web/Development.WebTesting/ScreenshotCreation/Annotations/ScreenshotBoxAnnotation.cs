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
  /// Draws a box around the annotation target with the specified <see cref="Pen"/> and <see cref="Padding"/>.
  /// </summary>
  public class ScreenshotBoxAnnotation : IScreenshotAnnotation
  {
    private readonly Brush _backgroundBrush;
    private readonly WebPadding _padding;
    private readonly Pen _pen;

    public ScreenshotBoxAnnotation ([NotNull] Pen pen, WebPadding padding, [CanBeNull] Brush backgroundBrush)
    {
      ArgumentUtility.CheckNotNull ("pen", pen);

      _pen = pen;
      _padding = padding;
      _backgroundBrush = backgroundBrush;
    }

    /// <summary>
    /// The <see cref="Brush"/> that will be used to fill the inside of the box annotation, 
    /// or <see langword="null" /> if the inside of the box should not be filled.
    /// </summary>
    [CanBeNull]
    public Brush BackgroundBrush
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
      ArgumentUtility.CheckNotNull ("graphics", graphics);
      ArgumentUtility.CheckNotNull ("resolvedScreenshotElement", resolvedScreenshotElement);

      // Transforms the bounding rectangle of the element so that GDI+ draws
      // exactly around the element, respecting the thickness of the drawing
      // pen. The box will never touch the elements bounds (as long as no
      // negative padding is specified).
      var size = (int) Math.Floor (_pen.Width);
      var xyOffset = (size - 1) / 2;
      var whOffset = xyOffset + size / 2;

      var bounds = resolvedScreenshotElement.ElementBounds;
      var area = new Rectangle (bounds.X - 1 - xyOffset, bounds.Y - 1 - xyOffset, bounds.Width + 1 + whOffset, bounds.Height + 1 + whOffset);
      var areaWithPadding = _padding.Apply (area);

      // GDI+ is essentially unable to correctly draw rectangles
      // correctly, that's why we draw lines instead.
      graphics.DrawLines (
          _pen,
          new[]
          {
              areaWithPadding.Location,
              areaWithPadding.Location + new Size (areaWithPadding.Width, 0),
              areaWithPadding.Location + areaWithPadding.Size,
              areaWithPadding.Location + new Size (0, areaWithPadding.Height),
              areaWithPadding.Location,
              areaWithPadding.Location + new Size (areaWithPadding.Width, 0)
          });

      if (_backgroundBrush != null)
      {
        if (areaWithPadding.Width <= 2 || areaWithPadding.Height <= 2)
          return;

        var innerRectangle = new Rectangle (areaWithPadding.X + 1, areaWithPadding.Y + 1, areaWithPadding.Width - 1, areaWithPadding.Height - 1);
        graphics.FillRectangle (_backgroundBrush, innerRectangle);
      }
    }
  }
}