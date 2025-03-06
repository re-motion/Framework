// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

// imitates System.Drawing.Pen
public class Pen
{
  public Color Color { get; set; }

  private float _width;

  public float Width
  {
    get { return _width; }
    set
    {
      if (value <= 0)
        throw new ArgumentOutOfRangeException(nameof(value), "New width must be greater than zero.");

      _width = value;
    }
  }

  public SKPaint Paint => new()
                          {
                              Style = SKPaintStyle.Stroke,
                              Color = Color.ToSkColor(),
                              StrokeWidth = _width
                          };

  public Pen (Color color, float width = 1.0f)
  {
    if (width <= 0)
      throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");

    Color = color;
    Width = width;
  }

  public Pen (SKColor color, float width = 1.0f)
  {
    if (width <= 0)
      throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");

    Color = color.ToColor();
    Width = width;
  }
}
