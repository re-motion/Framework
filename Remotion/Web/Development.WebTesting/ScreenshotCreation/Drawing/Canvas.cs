// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using Microsoft.Maui.Graphics.Skia;
using SkiaSharp;
using MauiHorizontalAlignment = Microsoft.Maui.Graphics.HorizontalAlignment;
using MauiVerticalAlignment = Microsoft.Maui.Graphics.VerticalAlignment;
using MauiTextFlow = Microsoft.Maui.Graphics.TextFlow;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

/// <summary>
/// Provides functionality to draw shapes and text to an <see cref="Image"/>.
/// </summary>
public class Canvas : IDisposable
{
  public static Canvas FromImage (Image layerImage)
  {
    ArgumentNullException.ThrowIfNull(layerImage);

    return new Canvas(
        new SkiaCanvas
        {
            Canvas = new SKCanvas(layerImage.SkiaBitmap)
        });
  }

  public SkiaCanvas SkiaCanvas { get; }

  public Canvas (SkiaCanvas skiaCanvas)
  {
    ArgumentNullException.ThrowIfNull(skiaCanvas);

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
    ArgumentNullException.ThrowIfNull(borderPen);

    SkiaCanvas.Canvas.DrawOval(ellipseBounds.ToSkRect(), borderPen.SkiaPaint);
  }

  public void FillEllipse (Brush backgroundBrush, Rectangle ellipseBounds)
  {
    ArgumentNullException.ThrowIfNull(backgroundBrush);

    SkiaCanvas.Canvas.DrawOval(ellipseBounds.ToSkRect(), backgroundBrush.SkiaPaint);
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
    ArgumentNullException.ThrowIfNull(text);
    ArgumentNullException.ThrowIfNull(font);
    ArgumentNullException.ThrowIfNull(brush);
    ArgumentNullException.ThrowIfNull(stringFormat);

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
    ArgumentNullException.ThrowIfNull(text);
    ArgumentNullException.ThrowIfNull(font);
    ArgumentNullException.ThrowIfNull(brush);

    SkiaCanvas.Font = new Microsoft.Maui.Graphics.Font(font.SkiaFont.Typeface.FamilyName);
    SkiaCanvas.FontSize = (int)Math.Round(font.Size);
    SkiaCanvas.FontColor = brush.SkiaPaint.Color.AsColor();
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

  public void DrawRectangle (Pen pen, Rectangle borderBounds)
  {
    ArgumentNullException.ThrowIfNull(pen);

    SkiaCanvas.Canvas.DrawRect(borderBounds.ToSkRect(), pen.SkiaPaint);
  }

  public void FillRectangle (Brush backgroundBrush, Rectangle annotationBounds)
  {
    ArgumentNullException.ThrowIfNull(backgroundBrush);

    SkiaCanvas.Canvas.DrawRect(annotationBounds.ToSkRect(), backgroundBrush.SkiaPaint);
  }

  public void DrawImage (Image layerImage, Point point)
  {
    ArgumentNullException.ThrowIfNull(layerImage);

    SkiaCanvas.Canvas.DrawBitmap(layerImage.SkiaBitmap, new SKPoint(point.X, point.Y));
  }

  public void DrawImage (Image layerImage, Rectangle newImageBounds, Rectangle normalizedCroppingRectangle)
  {
    ArgumentNullException.ThrowIfNull(layerImage);

    SkiaCanvas.Canvas.DrawBitmap(layerImage.SkiaBitmap, normalizedCroppingRectangle.ToSkRect(), newImageBounds.ToSkRect());
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
