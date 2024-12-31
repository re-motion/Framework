// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

// imitates System.Drawing.SolidBrush
public class SolidBrush : Brush
{
  public SolidBrush (Color color)
  {
    Paint = new SKPaint
            {
                Color = ColorConverter.ToSKColor(color),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
  }

  public SolidBrush (SKColor color)
  {
    Paint = new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
  }

  public Color Color
  {
    get
    {
      if (Paint == null)
        return Color.Transparent;

      return ColorConverter.ToColor(Paint.Color);
    }
  }

  public override object Clone ()
  {
    return new SolidBrush(Color);
  }
}
