// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using Microsoft.Maui.Graphics.Skia;
using SkiaSharp;

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

  public static SizeF MeasureString (this SkiaCanvas canvas, string? text, SKFont font, Size layoutArea)
    => MeasureString(canvas, text, font, new SizeF(layoutArea.Width, layoutArea.Height));

  public static SizeF MeasureString (this SkiaCanvas canvas, string? text, SKFont font, SizeF layoutArea)
  {
    font.MeasureText();
    return new SizeF(rect.Width, rect.Height);
  }
}
