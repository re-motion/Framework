// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

// imitates System.Drawing.SolidBrush
public class SolidBrush : Brush
{
  public SolidBrush (SKColor color)
  {
    Paint = new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
  }

  public SKColor Color => Paint?.Color ?? SKColor.Empty;

  public override object Clone ()
  {
    return new SolidBrush(Color);
  }
}
