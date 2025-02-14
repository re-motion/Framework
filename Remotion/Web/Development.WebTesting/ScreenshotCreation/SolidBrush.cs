// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Skia;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation;

/// <summary>
/// Imitates System.Drawing.SolidBrush
/// </summary>
public class SolidBrush : Brush
{
  public SolidBrush (Color color)
  {
    Paint = new SKPaint
            {
                Color = color.ToSkColor(),
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
  public override object Clone ()
  {
    return new SolidBrush(Paint.Color.ToColor());
  }
}
