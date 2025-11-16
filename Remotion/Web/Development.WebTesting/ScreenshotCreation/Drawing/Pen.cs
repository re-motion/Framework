// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

/// <summary>
/// Defines how to draw lines.
/// </summary>
public class Pen
{
  public SKPaint SkiaPaint { get; }

  public Pen (Color color, float width = 1.0f)
      : this(color.ToSkColor(), width)
  {
  }

  public Pen (SKColor color, float width = 1.0f)
      : this(new SKPaint { Style = SKPaintStyle.Stroke, Color = color, StrokeWidth = width })
  {
  }

  public Pen (SKPaint skiaPaint)
  {
    ArgumentNullException.ThrowIfNull(skiaPaint);

    SkiaPaint = skiaPaint;
  }

  public Color Color => SkiaPaint.Color.ToColor();

  public float Width => SkiaPaint.StrokeWidth;
}
