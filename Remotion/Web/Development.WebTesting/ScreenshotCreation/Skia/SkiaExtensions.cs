// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using System.IO;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using Remotion.Utilities;
using SkiaSharp;
using Color = System.Drawing.Color;
using SizeF = System.Drawing.SizeF;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Skia;

/// <summary>
/// Provides extension methods for working with SkiaSharp bitmaps, colors, and drawing utilities.
/// </summary>
public static class SkiaExtensions
{
  /// <summary>
  /// Creates a deep copy of the specified <see cref="SKBitmap"/>.
  /// </summary>
  /// <param name="bitmap">The <see cref="SKBitmap"/> to clone.</param>
  /// <returns>A new <see cref="SKBitmap"/> that is a copy of the original.</returns>
  public static SKBitmap Clone (this SKBitmap bitmap)
  {
    ArgumentUtility.CheckNotNull(nameof(bitmap), bitmap);
    var encodedBitmap = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
    return SKBitmap.FromImage(SKImage.FromEncodedData(encodedBitmap));
  }

  /// <summary>
  /// Draws a string on the specified <see cref="SkiaCanvas"/> with given font and brush properties.
  /// </summary>
  /// <param name="canvas">The canvas on which to draw.</param>
  /// <param name="text">The text to draw.</param>
  /// <param name="font">The font to use.</param>
  /// <param name="brush">The brush for coloring the text.</param>
  /// <param name="rectangle">The rectangle defining the text boundaries.</param>
  /// <param name="horizontalAlignment">The horizontal alignment of the text.</param>
  /// <param name="verticalAlignment">The vertical alignment of the text.</param>
  /// <param name="wrapLines">Indicates whether the text should wrap within the given rectangle.</param>
  public static void DrawString (
      this SkiaCanvas canvas,
      string text,
      Font font,
      Brush brush,
      Rectangle rectangle,
      HorizontalAlignment horizontalAlignment,
      VerticalAlignment verticalAlignment,
      bool wrapLines)
  {
    ArgumentUtility.CheckNotNull(nameof(canvas), canvas);
    ArgumentUtility.CheckNotNull(nameof(text), text);
    ArgumentUtility.CheckNotNull(nameof(font), font);
    ArgumentUtility.CheckNotNull(nameof(brush), brush);

    canvas.Font = new Microsoft.Maui.Graphics.Font(font.Typeface.FamilyName);
    canvas.FontSize = (int)Math.Round(font.Size);
    canvas.FontColor = brush.Paint.Color.AsColor();
    canvas.DrawString(
        text,
        rectangle.X,
        rectangle.Y,
        rectangle.Width,
        rectangle.Height,
        horizontalAlignment,
        verticalAlignment,
        wrapLines ? TextFlow.ClipBounds : TextFlow.OverflowBounds);
  }

  /// <summary>
  /// Measures the size of a given string when drawn with a specified font and layout area.
  /// </summary>
  /// <param name="canvas">The canvas used for measurement.</param>
  /// <param name="text">The text to measure.</param>
  /// <param name="font">The font used for measurement.</param>
  /// <param name="layoutArea">The maximum Size of the Area (pass 0 if you dont have a maximum).</param>
  /// <param name="wrapLines">Indicates whether text wrapping should be considered.</param>
  /// <returns>The measured size of the text.</returns>
  public static SizeF MeasureString (this SkiaCanvas canvas, string? text, Font font, SizeF layoutArea, bool wrapLines = true)
  {
    ArgumentUtility.CheckNotNull(nameof(canvas), canvas);
    ArgumentUtility.CheckNotNull(nameof(font), font);

    var paint = new SKPaint
                {
                    TextSize = font.Size,
                    IsAntialias = true,
                    Typeface = font.Typeface,
                    TextEncoding = SKTextEncoding.Utf16 // Match SKFont default
                };
    if (string.IsNullOrEmpty(text))
      return new SizeF(0, 0);

    var widthLimit = layoutArea.Width <= 0 ? float.PositiveInfinity : layoutArea.Width;
    var heightLimit = layoutArea.Height <= 0 ? float.PositiveInfinity : layoutArea.Height;

    var spaceWidth = paint.MeasureText(" ");
    var maxWidth = 0f;
    var height = paint.FontSpacing;

    var currentWidth = 0f;
    foreach (var word in text?.Split(' ') ?? [])
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

  /// <summary>
  /// Saves the bitmap to the specified file in the specified format and quality.
  /// </summary>
  /// <param name="bitmap">The bitmap to save.</param>
  /// <param name="path">The file path to save the bitmap.</param>
  /// <param name="format">The image format.</param>
  /// <param name="quality">The image quality (1-100).</param>
  /// <param name="fileMode">The file mode for saving.</param>
  public static void Save (this SKBitmap bitmap, string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100, FileMode fileMode = FileMode.Create)
  {
    ArgumentUtility.CheckNotNull(nameof(bitmap), bitmap);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(path), path);
    ArgumentOutOfRangeException.ThrowIfLessThan(quality, 1);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(quality, 100);

    var imageData = SKImage.FromBitmap(bitmap).Encode(format, quality);
    using var fileStream = new FileStream(path, fileMode);
    imageData.SaveTo(fileStream);
  }

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
