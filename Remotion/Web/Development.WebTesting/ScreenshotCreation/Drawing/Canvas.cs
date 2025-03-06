// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using Microsoft.Maui.Graphics.Skia;
using Remotion.Utilities;
using SkiaSharp;
using SizeF = System.Drawing.SizeF;
using MauiHorizontalAlignment = Microsoft.Maui.Graphics.HorizontalAlignment;
using MauiVerticalAlignment = Microsoft.Maui.Graphics.VerticalAlignment;
using MauiTextFlow = Microsoft.Maui.Graphics.TextFlow;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

public class Canvas : IDisposable
{
  public static Canvas FromImage (Image layerImage)
  {
    return new Canvas(
        new SkiaCanvas
        {
            Canvas = new SKCanvas(layerImage.SkiaBitmap)
        });
  }

  public SkiaCanvas SkiaCanvas { get; }

  public Canvas (SkiaCanvas skiaCanvas)
  {
    SkiaCanvas = skiaCanvas;
  }

  public void Flush ()
  {
    SkiaCanvas.Canvas.Flush();
  }

  public void SetTransform (int x, int y)
  {
    var translationMatrix = SKMatrix.CreateTranslation(x, y);
    SkiaCanvas.Canvas.SetMatrix(translationMatrix);
  }

  public void DrawEllipse (Pen borderPen, Rectangle ellipseBounds)
  {
    SkiaCanvas.Canvas.DrawOval(ellipseBounds.ToSkRect(), borderPen.Paint);
  }

  public void FillEllipse (Brush backgroundBrush, Rectangle ellipseBounds)
  {
    SkiaCanvas.Canvas.DrawOval(ellipseBounds.ToSkRect(), backgroundBrush.Paint);
  }

  /// <summary>
  /// Draws a string on the specified <see cref="SkiaCanvas"/> with given font and brush properties.
  /// </summary>
  /// <param name="text">The text to draw.</param>
  /// <param name="font">The font to use.</param>
  /// <param name="brush">The brush for coloring the text.</param>
  /// <param name="rectangle">The rectangle defining the text boundaries.</param>
  /// <param name="stringFormat">The alignment and overflow options for drawing the string.</param>
  public void DrawString (
      string text,
      Font font,
      Brush brush,
      Rectangle rectangle,
      StringFormat stringFormat)
  {
    ArgumentUtility.CheckNotNull(nameof(text), text);
    ArgumentUtility.CheckNotNull(nameof(font), font);
    ArgumentUtility.CheckNotNull(nameof(brush), brush);
    ArgumentUtility.CheckNotNull(nameof(stringFormat), stringFormat);

    DrawString(
        text,
        font,
        brush,
        rectangle,
        stringFormat.HorizontalAlignment,
        stringFormat.VerticalAlignment,
        stringFormat.TextFlow == TextFlow.ClipBounds);
  }

  /// <summary>
  /// Draws a string on the specified <see cref="SkiaCanvas"/> with given font and brush properties.
  /// </summary>
  /// <param name="text">The text to draw.</param>
  /// <param name="font">The font to use.</param>
  /// <param name="brush">The brush for coloring the text.</param>
  /// <param name="rectangle">The rectangle defining the text boundaries.</param>
  /// <param name="horizontalAlignment">The horizontal alignment of the text.</param>
  /// <param name="verticalAlignment">The vertical alignment of the text.</param>
  /// <param name="wrapLines">Indicates whether the text should wrap within the given rectangle.</param>
  public void DrawString (
      string text,
      Font font,
      Brush brush,
      Rectangle rectangle,
      HorizontalAlignment horizontalAlignment,
      VerticalAlignment verticalAlignment,
      bool wrapLines)
  {
    ArgumentUtility.CheckNotNull(nameof(text), text);
    ArgumentUtility.CheckNotNull(nameof(font), font);
    ArgumentUtility.CheckNotNull(nameof(brush), brush);

    SkiaCanvas.Font = new Microsoft.Maui.Graphics.Font(font.Typeface.FamilyName);
    SkiaCanvas.FontSize = (int)Math.Round(font.Size);
    SkiaCanvas.FontColor = brush.Paint.Color.AsColor();
    SkiaCanvas.DrawString(
        text,
        rectangle.X,
        rectangle.Y,
        rectangle.Width,
        rectangle.Height,
        ToMauiHorizontalAlignment(horizontalAlignment),
        ToMauiVerticalAlignment(verticalAlignment),
        wrapLines ? MauiTextFlow.ClipBounds : MauiTextFlow.OverflowBounds);
  }

  public void DrawRectangle (Rectangle borderBounds, Pen pen)
  {
    SkiaCanvas.Canvas.DrawRect(borderBounds.ToSkRect(), pen.Paint);
  }

  public void FillRectangle (Brush backgroundBrush, Rectangle annotationBounds)
  {
    SkiaCanvas.Canvas.DrawRect(annotationBounds.ToSkRect(), backgroundBrush.Paint);
  }

  public void DrawImage (Image layerImage, Point point)
  {
    SkiaCanvas.Canvas.DrawBitmap(layerImage.SkiaBitmap, new SKPoint(point.X, point.Y));
  }

  public void DrawImage (Image layerImage, Rectangle newImageBounds, Rectangle normalizedCroppingRectangle)
  {
    SkiaCanvas.Canvas.DrawBitmap(layerImage.SkiaBitmap, normalizedCroppingRectangle.ToSkRect(), newImageBounds.ToSkRect());
  }

  /// <summary>
  /// Measures the size of a given string when drawn with a specified font and layout area.
  /// </summary>
  /// <param name="text">The text to measure.</param>
  /// <param name="font">The font used for measurement.</param>
  /// <param name="layoutArea">The maximum Size of the Area (pass 0 if you dont have a maximum).</param>
  /// <param name="wrapLines">Indicates whether text wrapping should be considered.</param>
  /// <returns>The measured size of the text.</returns>
  public SizeF MeasureString (string? text, Font font, SizeF? layoutArea = null, bool wrapLines = true)
  {
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

    var widthLimit = layoutArea is { Width: > 0 } ? layoutArea.Value.Width : float.PositiveInfinity;
    var heightLimit = layoutArea is { Height: > 0 } ? layoutArea.Value.Height : float.PositiveInfinity;

    var spaceWidth = paint.MeasureText(" ");
    var maxWidth = 0f;
    var height = paint.FontSpacing;

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

  private MauiVerticalAlignment ToMauiVerticalAlignment (VerticalAlignment verticalAlignment)
  {
    return verticalAlignment switch
    {
        VerticalAlignment.Top => MauiVerticalAlignment.Top,
        VerticalAlignment.Center => MauiVerticalAlignment.Center,
        VerticalAlignment.Bottom => MauiVerticalAlignment.Bottom,
        _ => throw new ArgumentOutOfRangeException(nameof(verticalAlignment), verticalAlignment, null)
    };
  }

  private MauiHorizontalAlignment ToMauiHorizontalAlignment (HorizontalAlignment horizontalAlignment)
  {
    return horizontalAlignment switch
    {
        HorizontalAlignment.Left => MauiHorizontalAlignment.Left,
        HorizontalAlignment.Center => MauiHorizontalAlignment.Center,
        HorizontalAlignment.Right => MauiHorizontalAlignment.Right,
        HorizontalAlignment.Justified => MauiHorizontalAlignment.Justified,
        _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null)
    };
  }

  public void Dispose ()
  {
    SkiaCanvas.Dispose();
  }
}
