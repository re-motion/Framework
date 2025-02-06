// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

/// <summary>
/// Imitates System.Drawing.SolidBrush
/// </summary>
public class SolidBrush : Brush
{
  public SolidBrush (Color color)
  {
    Paint = new SKPaint
            {
                Color = color.ToSKColor(),
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

      return Paint.Color.ToColor();
    }
  }

  public override object Clone ()
  {
    return new SolidBrush(Color);
  }
}
