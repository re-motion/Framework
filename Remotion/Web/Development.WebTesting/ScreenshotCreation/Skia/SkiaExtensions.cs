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

public static class SkiaExtensions
{
  public static SKBitmap Clone (this SKBitmap bitmap)
  {
    ArgumentUtility.CheckNotNull(nameof(bitmap), bitmap);
    var encodedBitmap = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
    return SKBitmap.FromImage(SKImage.FromEncodedData(encodedBitmap));
  }

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
    ArgumentUtility.CheckNotNull(nameof(canvas), canvas);

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

  public static Color ToColor (this SKColor color)
  {
    return Color.FromArgb(
        color.Alpha,
        color.Red,
        color.Green,
        color.Blue
    );
  }

  public static SKColor ToSkColor (this Color color)
  {
    return new SKColor(
        color.R,
        color.G,
        color.B,
        color.A
    );
  }

  public static SKRect ToSkRect (this Rectangle rectangle)
  {
    return new SKRect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
  }

  public static Rectangle ToRectangle (this SKRect rect)
  {
    return new Rectangle((int)rect.Left, (int)rect.Top, (int)(rect.Right - rect.Left), (int)(rect.Bottom - rect.Top));
  }
}
