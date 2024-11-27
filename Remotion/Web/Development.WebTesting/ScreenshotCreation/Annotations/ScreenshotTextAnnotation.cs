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
  /// Draws text around the annotation target using the specified properties.
  /// </summary>
  public class ScreenshotTextAnnotation : IScreenshotAnnotation
  {
    private const float c_maxLayoutMeasure = 5000f;

    private readonly Brush? _backgroundBrush;
    private readonly string _content;
    private readonly ContentAlignment _contentAlignment;
    private readonly Font _font;
    private readonly Brush _foregroundBrush;
    private readonly float _maxHeight;
    private readonly float _maxWidth;
    private readonly WebPadding _padding;
    private readonly StringFormat _stringFormat;

    public ScreenshotTextAnnotation (
        [NotNull] string content,
        [NotNull] Font font,
        [NotNull] Brush foregroundBrush,
        [CanBeNull] Brush? backgroundBrush,
        [NotNull] StringFormat stringFormat,
        ContentAlignment contentAlignment,
        WebPadding padding,
        float? maxWidth,
        float? maxHeight)
    {
      ArgumentUtility.CheckNotNull("content", content);
      ArgumentUtility.CheckNotNull("font", font);
      ArgumentUtility.CheckNotNull("foregroundBrush", foregroundBrush);
      ArgumentUtility.CheckNotNull("stringFormat", stringFormat);

      _content = content;
      _font = font;
      _foregroundBrush = foregroundBrush;
      _backgroundBrush = backgroundBrush;
      _stringFormat = stringFormat;
      _contentAlignment = contentAlignment;
      _padding = padding;
      _maxWidth = maxWidth ?? c_maxLayoutMeasure;
      _maxHeight = maxHeight ?? c_maxLayoutMeasure;
    }

    /// <summary>
    /// The <see cref="Brush"/> used to fill the background of the text,
    /// or <see langword="null" /> if the background should not be filled.
    /// </summary>
    [CanBeNull]
    public Brush? BackgroundBrush
    {
      get { return _backgroundBrush; }
    }

    /// <summary>
    /// The text content that will be used to draw the text annotation.
    /// </summary>
    [NotNull]
    public string Content
    {
      get { return _content; }
    }

    /// <summary>
    /// The <see cref="ContentAlignment"/> that will be used to position the text relative to the annotated object.
    /// </summary>
    public ContentAlignment ContentAlignment
    {
      get { return _contentAlignment; }
    }

    /// <summary>
    /// The <see cref="Font"/> that will be used to draw the text.
    /// </summary>
    [NotNull]
    public Font Font
    {
      get { return _font; }
    }

    /// <summary>
    /// The <see cref="Brush"/> that will be used to draw the text.
    /// </summary>
    [NotNull]
    public Brush ForegroundBrush
    {
      get { return _foregroundBrush; }
    }

    /// <summary>
    /// The maximum height in pixels, that the text bounding box is allowed to have.
    /// </summary>
    public float MaxHeight
    {
      get { return _maxHeight; }
    }

    /// <summary>
    /// The maximum width in pixels, that the text bounding box is allowed to have.
    /// </summary>
    public float MaxWidth
    {
      get { return _maxWidth; }
    }

    /// <summary>
    /// The padding that will be applied to the text after applying the <see cref="ContentAlignment"/>.
    /// </summary>
    public WebPadding Padding
    {
      get { return _padding; }
    }

    /// <summary>
    /// The <see cref="StringFormat"/> that will be used to draw the text.
    /// </summary>
    [NotNull]
    public StringFormat StringFormat
    {
      get { return _stringFormat; }
    }

    /// <inheritdoc />
    public void Draw (Graphics graphics, ResolvedScreenshotElement resolvedScreenshotElement)
    {
      ArgumentUtility.CheckNotNull("graphics", graphics);
      ArgumentUtility.CheckNotNull("resolvedScreenshotElement", resolvedScreenshotElement);

      var size = graphics.MeasureString(_content, _font, new SizeF(_maxWidth, _maxHeight));
      var position = PositionAndApplyPadding(resolvedScreenshotElement.ElementBounds, size.Width, size.Height);
      var layout = new Rectangle(
          (int)Math.Round(position.X),
          (int)Math.Round(position.Y),
          (int)Math.Round(size.Width) + 1,
          (int)Math.Round(size.Height) + 1);

      if (_backgroundBrush != null)
        graphics.FillRectangle(_backgroundBrush, layout);

      graphics.DrawString(_content, _font, _foregroundBrush, layout, _stringFormat);
    }

    /// <summary>
    /// Positions the text according to <see cref="ContentAlignment"/> and the specified <paramref name="elementBounds"/>,
    /// <paramref name="contentWidth"/> and <paramref name="contentHeight"/>.
    /// </summary>
    private PointF PositionAndApplyPadding (Rectangle elementBounds, float contentWidth, float contentHeight)
    {
      switch (_contentAlignment)
      {
        case ContentAlignment.TopLeft:
          return new PointF(
              elementBounds.X - contentWidth - _padding.Right,
              elementBounds.Y - contentHeight - _padding.Bottom);
        case ContentAlignment.TopCenter:
          return new PointF(
              elementBounds.X + (elementBounds.Width - contentWidth) / 2,
              elementBounds.Y - contentHeight - _padding.Bottom);
        case ContentAlignment.TopRight:
          return new PointF(
              elementBounds.Right + _padding.Left,
              elementBounds.Y - contentHeight - _padding.Bottom);
        case ContentAlignment.MiddleLeft:
          return new PointF(
              elementBounds.X - contentWidth - _padding.Right,
              elementBounds.Y + (elementBounds.Height - contentHeight) / 2);
        case ContentAlignment.MiddleCenter:
          return new PointF(
              elementBounds.X + (elementBounds.Width - contentWidth) / 2,
              elementBounds.Y + (elementBounds.Height - contentHeight) / 2);
        case ContentAlignment.MiddleRight:
          return new PointF(
              elementBounds.Right + _padding.Left,
              elementBounds.Y + (elementBounds.Height - contentHeight) / 2);
        case ContentAlignment.BottomLeft:
          return new PointF(
              elementBounds.X - contentWidth - _padding.Right,
              elementBounds.Bottom + _padding.Top);
        case ContentAlignment.BottomCenter:
          return new PointF(
              elementBounds.X + (elementBounds.Width - contentWidth) / 2,
              elementBounds.Bottom + _padding.Top);
        case ContentAlignment.BottomRight:
          return new PointF(
              elementBounds.Right + _padding.Left,
              elementBounds.Bottom + _padding.Top);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
