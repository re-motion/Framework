// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using System.IO;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using SkiaSharp;
using SizeF = System.Drawing.SizeF;

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

public static class SkiaCanvasExtensions
{
  public static SizeF MeasureString (this SkiaCanvas canvas, string? text, SKFont font, SizeF layoutArea, bool wrapLines = true)
  {
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
        currentWidth = wordWidth;
      }

      currentWidth += wordWidth;
      maxWidth = Math.Max(maxWidth, currentWidth);
    }

    maxWidth = Math.Min(maxWidth, widthLimit);
    height = Math.Min(height, heightLimit);

    return new SizeF(maxWidth, height);
  }

  public static SkiaCanvas FromBitmap (SKBitmap? bitmap)
  {
    return new SkiaCanvas { Canvas = new SKCanvas(bitmap) };
  }

  public static void DrawString (
      this SkiaCanvas canvas,
      string text,
      SKFont font,
      Brush brush,
      Rectangle rectangle,
      HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment verticalAlignment = VerticalAlignment.Top,
      bool wrapLines = true)
  {
    canvas.Font = font.ToMauiFont();
    canvas.FontSize = font.ToMauiFont().Weight;
    canvas.FontColor = brush.Paint?.Color.AsColor();
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

  public static void Save (this SKBitmap bitmap, string path, FileMode mode = FileMode.Create)
  {
    var imageData = bitmap.ToSkImage().Encode();
    using var fileStream = new FileStream(path, mode);
    imageData.SaveTo(fileStream);
  }
}
