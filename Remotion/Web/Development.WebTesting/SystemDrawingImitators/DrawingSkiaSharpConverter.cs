// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Drawing;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

public static class DrawingSkiaSharpConverter
{
  public static Color ToColor (this SKColor color)
  {
    return Color.FromArgb(
        color.Alpha,
        color.Red,
        color.Green,
        color.Blue
    );
  }

  public static SKColor ToSKColor (this Color color)
  {
    return new SKColor(
        color.R,
        color.G,
        color.B,
        color.A
    );
  }

  public static SKPoint ToSKPoint (this Point point)
  {
    return new SKPoint(point.X, point.Y);
  }

  public static SKRect ToSkRect (this Rectangle rectangle)
  {
    return new SKRect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
  }
}
