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
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.SystemDrawingImitators;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations
{
  /// <summary>
  /// Draws a badge (circle/ellipse with text) centered into the target element.
  /// </summary>
  public class ScreenshotBadgeAnnotation : IScreenshotAnnotation
  {
    private readonly Brush? _backgroundBrush;
    private readonly Pen _borderPen;
    private readonly string _content;
    private readonly Brush _contentBrush;
    private readonly WebPadding _contentPadding;
    private readonly Font _font;
    private readonly bool _forceCircle;
    private readonly System.Drawing.Size _translation;

    public ScreenshotBadgeAnnotation (
        [NotNull] string content,
        WebPadding contentPadding,
        [NotNull] Font font,
        [NotNull] Brush contentBrush,
        [NotNull] Pen borderPen,
        [CanBeNull] Brush? backgroundBrush,
        System.Drawing.Size translation,
        bool forceCircle)
    {
      ArgumentUtility.CheckNotNull("content", content);
      ArgumentUtility.CheckNotNull("font", font);
      ArgumentUtility.CheckNotNull("contentBrush", contentBrush);
      ArgumentUtility.CheckNotNull("borderPen", borderPen);

      _content = content;
      _contentPadding = contentPadding;
      _font = font;
      _contentBrush = contentBrush;
      _borderPen = borderPen;
      _backgroundBrush = backgroundBrush;
      _translation = translation;
      _forceCircle = forceCircle;
    }

    /// <summary>
    /// <see cref="Brush"/> that will be used to fill the background of the badge, 
    /// or <see langword="null" /> if the background should not be filled.
    /// </summary>
    [CanBeNull]
    public Brush? BackgroundBrush
    {
      get { return _backgroundBrush; }
    }

    /// <summary>
    /// <see cref="Pen"/> used to outline the drawn badge.
    /// </summary>
    public Pen BorderPen
    {
      get { return _borderPen; }
    }

    /// <summary>
    /// The content of the badge.
    /// </summary>
    public string Content
    {
      get { return _content; }
    }

    /// <summary>
    /// <see cref="Brush"/> used to draw the <see cref="Content"/> of the badge.
    /// </summary>
    public Brush ContentBrush
    {
      get { return _contentBrush; }
    }

    /// <summary>
    /// Padding used to position the <see cref="Content"/> inside of the ellipse/circle.
    /// </summary>
    /// <remarks>
    /// Changing the <see cref="ContentPadding"/> does not effect the positioning of the badge.
    /// The badge will always be positioned by using a center point.
    /// </remarks>
    public WebPadding ContentPadding
    {
      get { return _contentPadding; }
    }

    /// <summary>
    /// The <see cref="System.Drawing.Font"/> that will be used to draw the <see cref="Content"/>.
    /// </summary>
    public Font Font
    {
      get { return _font; }
    }

    /// <summary>
    /// <see langword="true" /> if the badge should always be drawing using a circle instead of an ellipse.
    /// </summary>
    public bool ForceCircle
    {
      get { return _forceCircle; }
    }

    /// <summary>
    /// Translation vector of the center point.
    /// </summary>
    public System.Drawing.Size Translation
    {
      get { return _translation; }
    }

    /// <inheritdoc />
    public void Draw (Graphics graphics, ResolvedScreenshotElement resolvedScreenshotElement)
    {
      ArgumentUtility.CheckNotNull("graphics", graphics);
      ArgumentUtility.CheckNotNull("resolvedScreenshotElement", resolvedScreenshotElement);

      var elementBounds = resolvedScreenshotElement.ElementBounds;
      var centerPoint = new Point(
          elementBounds.X + elementBounds.Width / 2 + _translation.Width,
          elementBounds.Y + elementBounds.Height / 2 + _translation.Height);

      var textSizeF = graphics.MeasureString(Content, Font);
      var textSize = new Size((int)Math.Ceiling(textSizeF.Width), (int)Math.Ceiling(textSizeF.Height));

      var textBound = new Rectangle(centerPoint.X - textSize.Width / 2, centerPoint.Y - textSize.Height / 2, textSize.Width, textSize.Height);
      var ellipseBounds = ContentPadding.Apply(textBound);

      if (ForceCircle && ellipseBounds.Width != ellipseBounds.Height)
      {
        var difference = ellipseBounds.Width - ellipseBounds.Height;
        if (difference > 0)
        {
          ellipseBounds.Height += difference;
          ellipseBounds.Y -= difference / 2;
        }
        else
        {
          ellipseBounds.Width += -difference;
          ellipseBounds.X -= -difference / 2;
        }
      }

      if (BackgroundBrush != null)
        graphics.FillEllipse(BackgroundBrush, ellipseBounds);
      graphics.DrawString(Content, Font, ContentBrush, textBound);
      graphics.DrawEllipse(BorderPen, ellipseBounds);
    }
  }
}
