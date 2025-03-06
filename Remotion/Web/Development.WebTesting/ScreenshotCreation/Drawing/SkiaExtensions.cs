// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using System.IO;
using Remotion.Utilities;
using SkiaSharp;
using Color = System.Drawing.Color;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

/// <summary>
/// Provides extension methods for working with SkiaSharp bitmaps, colors, and drawing utilities.
/// </summary>
public static class SkiaExtensions
{
  /// <summary>
  /// Converts an <see cref="SKColor"/> to a <see cref="Color"/>.
  /// </summary>
  /// <param name="color">The <see cref="SKColor"/> to convert.</param>
  /// <returns>The equivalent <see cref="Color"/>.</returns>
  public static Color ToColor (this SKColor color)
  {
    return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
  }

  /// <summary>
  /// Converts a <see cref="Color"/> to an <see cref="SKColor"/>.
  /// </summary>
  /// <param name="color">The <see cref="Color"/> to convert.</param>
  /// <returns>The equivalent <see cref="SKColor"/>.</returns>
  public static SKColor ToSkColor (this Color color)
  {
    return new SKColor(color.R, color.G, color.B, color.A);
  }

  /// <summary>
  /// Converts a <see cref="Rectangle"/> to an <see cref="SKRect"/>.
  /// </summary>
  /// <param name="rectangle">The <see cref="Rectangle"/> to convert.</param>
  /// <returns>The equivalent <see cref="SKRect"/>.</returns>
  public static SKRect ToSkRect (this Rectangle rectangle)
  {
    return new SKRect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
  }

  /// <summary>
  /// Converts an <see cref="SKRect"/> to a <see cref="Rectangle"/>.
  /// </summary>
  /// <param name="rect">The <see cref="SKRect"/> to convert.</param>
  /// <returns>The equivalent <see cref="Rectangle"/>.</returns>
  public static Rectangle ToRectangle (this SKRect rect)
  {
    return new Rectangle((int)rect.Left, (int)rect.Top, (int)(rect.Right - rect.Left), (int)(rect.Bottom - rect.Top));
  }
}
