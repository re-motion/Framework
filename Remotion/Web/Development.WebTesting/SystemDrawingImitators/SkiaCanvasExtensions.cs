// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using System.IO;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using SkiaSharp;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

public static class SkiaCanvasExtensions
{
  public static void DrawLines (this SkiaCanvas canvas, Pen pen, Point[] points)
  {
    for (int i = 1; i < points.Length; i++)
    {
      var p0 = points[i - 1].ToSKPoint();
      var p1 = points[i].ToSKPoint();

      canvas.Canvas.DrawLine(p0, p1, pen.Paint);
    }
  }

  public static void FillRectangle (this SkiaCanvas canvas, Brush brush, Rectangle rectangle)
  {
    canvas.Canvas.DrawRect(rectangle.ToSkRect(), brush.Paint);
  }

  public static SizeF MeasureString (this SkiaCanvas canvas, string? text, SKFont font)
    => MeasureString(canvas, text, font, new SizeF(0, 0));

  public static SizeF MeasureString (this SkiaCanvas canvas, string? text, SKFont font, Size layoutArea, bool wrapLines)
    => MeasureString(canvas, text, font, new SizeF(layoutArea.Width, layoutArea.Height), wrapLines);

  public static SizeF MeasureString (this SkiaCanvas canvas, string? text, SKFont font, SizeF layoutArea, bool wrapLines = true)
  {
    var paint = new SKPaint
                {
                    TextSize = font.Size,
                    IsAntialias = true,
                    Typeface = font.Typeface,
                    TextEncoding = SKTextEncoding.Utf16 // Match SKFont default
                };

    var widthLimit = layoutArea.Width == 0 ? layoutArea.Width : float.PositiveInfinity;
    var heightLimit = layoutArea.Height == 0 ? layoutArea.Height : float.PositiveInfinity;

    var spaceWidth = paint.MeasureText(" ");
    var maxWidth = 0f;
    var height = paint.FontSpacing;

    var currentWidth = 0f;
    foreach (var word in text?.Split(' ') ?? [])
    {
      var wordWidth = paint.MeasureText(word) + spaceWidth;

      if (currentWidth + wordWidth > widthLimit && wrapLines)
      {
        // new line
        height += paint.FontSpacing;
        currentWidth = wordWidth;
      }

      currentWidth += wordWidth;
      maxWidth = Math.Max(maxWidth, currentWidth);
    }

    maxWidth = Math.Min(maxWidth, widthLimit);
    height = Math.Min(height, heightLimit);

    return new SizeF(maxWidth, height);
  }

  public static SkiaCanvas FromImage (SKImage? image)
  {
    return FromBitmap(SKBitmap.FromImage(image));
  }

  public static SkiaCanvas FromBitmap (SKBitmap? bitmap)
  {
    return new SkiaCanvas { Canvas = new SKCanvas(bitmap) };
  }

  public static void FillEllipse (this SkiaCanvas canvas, Brush brush, Rectangle ellipseBounds)
  {
    canvas.Canvas.DrawOval(ellipseBounds.ToSkRect(), brush.Paint);
  }

  public static void FillEllipse (this SkiaCanvas canvas, Brush brush, int x, int y, int width, int height)
  {
    var rect = new SKRect(x, y, x + width, y + height);
    canvas.Canvas.DrawOval(rect, brush.Paint);
  }

  public static void DrawEllipse (this SkiaCanvas canvas, Pen pen, Rectangle ellipseBounds)
  {
    canvas.Canvas.DrawOval(ellipseBounds.X, ellipseBounds.Y, ellipseBounds.Width, ellipseBounds.Height, pen.Paint);
  }

  public static void DrawString (
      this SkiaCanvas canvas,
      string? text,
      SKFont? font,
      Brush brush,
      Rectangle rectangle,
      ContentAlignment contentAlignment = ContentAlignment.MiddleCenter,
      bool wrapLines = true)
  {
    HorizontalAlignment horizAlignment;
    VerticalAlignment vertAlignment;

    switch (contentAlignment)
    {
      case ContentAlignment.TopLeft:
        horizAlignment = HorizontalAlignment.Left;
        vertAlignment = VerticalAlignment.Top;
        break;
      case ContentAlignment.TopCenter:
        horizAlignment = HorizontalAlignment.Center;
        vertAlignment = VerticalAlignment.Top;
        break;
      case ContentAlignment.TopRight:
        horizAlignment = HorizontalAlignment.Right;
        vertAlignment = VerticalAlignment.Top;
        break;
      case ContentAlignment.MiddleLeft:
        horizAlignment = HorizontalAlignment.Left;
        vertAlignment = VerticalAlignment.Center;
        break;
      case ContentAlignment.MiddleCenter:
        horizAlignment = HorizontalAlignment.Center;
        vertAlignment = VerticalAlignment.Center;
        break;
      case ContentAlignment.MiddleRight:
        horizAlignment = HorizontalAlignment.Right;
        vertAlignment = VerticalAlignment.Center;
        break;
      case ContentAlignment.BottomLeft:
        horizAlignment = HorizontalAlignment.Left;
        vertAlignment = VerticalAlignment.Bottom;
        break;
      case ContentAlignment.BottomCenter:
        horizAlignment = HorizontalAlignment.Center;
        vertAlignment = VerticalAlignment.Bottom;
        break;
      case ContentAlignment.BottomRight:
        horizAlignment = HorizontalAlignment.Right;
        vertAlignment = VerticalAlignment.Bottom;
        break;
      default:
        horizAlignment = HorizontalAlignment.Center;
        vertAlignment = VerticalAlignment.Center;
        break;
    }

    canvas.Font = font.ToMauiFont();
    canvas.FontColor = brush.Paint?.Color.AsColor();
    canvas.DrawString(
        text,
        rectangle.X,
        rectangle.Y,
        rectangle.Width,
        rectangle.Height,
        horizAlignment,
        vertAlignment,
        wrapLines ? TextFlow.ClipBounds : TextFlow.OverflowBounds);
  }

  public static void SaveAsPng (this SkiaCanvas canvas, string path, int width, int height)
  {
    var surface = SKSurface.Create(new SKImageInfo(width, height));
    canvas.Canvas.DrawSurface(surface, new SKPoint(0, 0));
    using var image = surface.Snapshot();
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    using var stream = new FileStream(path, FileMode.Create);
    data.SaveTo(stream);
  }
}
