// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
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

  public static SizeF MeasureString (this SkiaCanvas canvas, string? text, SKFont font, Size layoutArea, object stringFormat) //TODO: add support for string formatting
    => MeasureString(canvas, text, font, new SizeF(layoutArea.Width, layoutArea.Height));

  public static SizeF MeasureString (this SkiaCanvas canvas, string? text, SKFont font, SizeF layoutArea)
  {
    var paint = new SKPaint
                {
                    TextSize = font.Size,
                    IsAntialias = true
                };

    var maxLayoutWidth = layoutArea.Width;
    var spaceWidth = paint.MeasureText(" ");
    var maxWidth = 0f;
    var maxHeigth = paint.TextSize;
    var totalX = 0f;

    foreach (string word in text?.Split(' ') ?? [])
    {
      float wordWidth = paint.MeasureText(word);
      totalX = totalX + wordWidth + spaceWidth;
      if (totalX > maxLayoutWidth)
      {
        // new line
        if (totalX > maxWidth) maxWidth = totalX;
        maxHeigth += paint.FontSpacing;
        totalX = 0f;
      }
    }

    return new SizeF(maxWidth, maxHeigth);
  }

  public static SkiaCanvas FromImage (SKImage? image)
  {
    return FromBitmap(SKBitmap.FromImage(image));
  }

  public static SkiaCanvas FromBitmap (SKBitmap? bitmap)
  {
    return new SkiaCanvas { Canvas = new SKCanvas(bitmap) };
  }

  public static SKImage Clone (this SKImage? image)
  {
    return SKImage.FromEncodedData(image?.EncodedData);
  }

  public static void FillEllipse (this SkiaCanvas canvas, Brush brush, int x, int y, int width, int height)
  {
    var rect = new SKRect(x, y, x + width, y + height);
    canvas.Canvas.DrawOval(rect, brush.Paint);
  }

  public static void DrawString (this SkiaCanvas canvas, string? text, SKFont? font, Brush brush, Rectangle rectangle, object stringFormat) //TODO: add support for string formatting
  {
    canvas.Font = font.ToMauiFont();
    canvas.FontColor = brush.Paint?.Color.AsColor();
    canvas.DrawString(text, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, HorizontalAlignment.Center, VerticalAlignment.Center);
  }
}
