// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Drawing;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

public static class ColorConverter
{
  public static Color ToColor (SKColor color)
  {
    return Color.FromArgb(
        color.Alpha,
        color.Red,
        color.Green,
        color.Blue
    );
  }

  public static SKColor ToSKColor (Color color)
  {
    return new SKColor(
        color.R,
        color.G,
        color.B,
        color.A
    );
  }
}
