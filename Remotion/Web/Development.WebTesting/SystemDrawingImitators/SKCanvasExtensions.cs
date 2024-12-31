// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Drawing;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

public static class SKCanvasExtensions
{
  public static void DrawLines (this SKCanvas canvas, Pen pen, Point[] points)
  {
    for (int i = 1; i < points.Length; i++)
    {
      var p0 = DrawingSkiaSharpConverter.ToSKPoint(points[i - 1]);
      var p1 = DrawingSkiaSharpConverter.ToSKPoint(points[i]);

      canvas.DrawLine(p0, p1, pen.Paint);
    }
  }
}
