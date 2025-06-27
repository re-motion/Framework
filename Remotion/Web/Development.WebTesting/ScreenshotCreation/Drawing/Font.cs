// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using Remotion.Utilities;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

/// <summary>
/// Defines a format for text, including font face, size, and style.
/// </summary>
public class Font : IDisposable
{
  public SKFont SkiaFont { get; }

  public Font (string fontFamilyName, float size = 12f)
  {
    ArgumentException.ThrowIfNullOrEmpty(fontFamilyName);

    var fontFamily = SKTypeface.FromFamilyName(fontFamilyName);
    if (fontFamily == null || !fontFamily.FamilyName.Equals(fontFamilyName, StringComparison.OrdinalIgnoreCase))
      throw new InvalidOperationException($"Cannot find font '{fontFamilyName}' (Fallback would be '{fontFamily?.FamilyName}').");

    SkiaFont = new SKFont(fontFamily, size);
  }

  public Font (SKTypeface typeface, float size = 12f)
      : this(new SKFont(typeface, size))
  {
  }

  public Font (SKFont skiaFont)
  {
    ArgumentNullException.ThrowIfNull(skiaFont);

    SkiaFont = skiaFont;
  }

  public float Size => SkiaFont.Size;

  /// <summary>
  /// Measures the size of a given string when drawn with a specified font and layout area.
  /// </summary>
  /// <param name="text">The text to measure.</param>
  /// <param name="layoutArea">The maximum Size of the area, or <see langword="null" /> if there is no layout area.</param>
  /// <param name="wrapLines">Indicates whether text wrapping should be considered.</param>
  /// <returns>The measured size of the text.</returns>
  public SizeF MeasureString (string? text, SizeF? layoutArea = null, bool wrapLines = true)
  {
    var paint = new SKPaint
                {
                    TextSize = SkiaFont.Size,
                    IsAntialias = true,
                    Typeface = SkiaFont.Typeface,
                    TextEncoding = SKTextEncoding.Utf16 // Match SKFont default
                };
    if (string.IsNullOrEmpty(text))
      return new SizeF(0, 0);

    var widthLimit = layoutArea is { Width: > 0 } ? layoutArea.Value.Width : float.PositiveInfinity;
    var heightLimit = layoutArea is { Height: > 0 } ? layoutArea.Value.Height : float.PositiveInfinity;

    var spaceWidth = paint.MeasureText(" ");
    var maxWidth = 0f;
    var height = Math.Abs(SkiaFont.Metrics.Top) + SkiaFont.Metrics.Bottom + SkiaFont.Metrics.Leading;

    var currentWidth = 0f;
    foreach (var word in text.Split(' '))
    {
      var wordWidth = paint.MeasureText(word) + spaceWidth;

      if (currentWidth + wordWidth > widthLimit && wrapLines)
      {
        // new line
        height += paint.FontSpacing;
        currentWidth = 0;
      }

      currentWidth += wordWidth;
      maxWidth = Math.Max(maxWidth, currentWidth);
    }

    maxWidth = Math.Min(maxWidth, widthLimit);
    height = Math.Min(height, heightLimit);

    return new SizeF(maxWidth, height);
  }

  public void Dispose ()
  {
    SkiaFont.Dispose();
  }
}
