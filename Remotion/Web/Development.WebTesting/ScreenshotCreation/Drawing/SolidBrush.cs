// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

/// <summary>
/// Draws an area with a solid color.
/// </summary>
public class SolidBrush : Brush
{
  public SolidBrush (Color color)
      : this(color.ToSkColor())
  {
  }

  public SolidBrush (SKColor color)
      : base(new SKPaint { Color = color, Style = SKPaintStyle.Fill })
  {
  }
}
