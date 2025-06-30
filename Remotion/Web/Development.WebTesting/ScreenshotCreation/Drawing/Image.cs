// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using System.IO;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

/// <summary>
/// Provides functionality to load and inspect an image.
/// Use <see cref="Canvas"/> to modify the image.
/// </summary>
public class Image : IDisposable
{
  public static Image FromFile (string filePath)
  {
    ArgumentException.ThrowIfNullOrEmpty(filePath);

    var skBitmap = SKBitmap.Decode(filePath);
    return new Image(skBitmap);
  }

  public static Image FromStream (Stream stream)
  {
    ArgumentNullException.ThrowIfNull(stream);

    var skBitmap = SKBitmap.Decode(stream);
    return new Image(skBitmap);
  }

  public SKBitmap SkiaBitmap { get; }

  public Size Size => new(Width, Height);

  public int Height => SkiaBitmap.Height;

  public int Width => SkiaBitmap.Width;

  public Image (int width, int height)
  {
    SkiaBitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
  }

  public Image (SKBitmap skiaBitmap)
  {
    ArgumentNullException.ThrowIfNull(skiaBitmap);

    SkiaBitmap = skiaBitmap;
  }

  public Image Clone ()
  {
    var encodedBitmap = SKImage.FromBitmap(SkiaBitmap).Encode(SKEncodedImageFormat.Png, 100);
    var newBitmap = SKBitmap.FromImage(SKImage.FromEncodedData(encodedBitmap));

    return new Image(newBitmap);
  }

  public Color GetPixel (int x, int y)
  {
    return SkiaBitmap.GetPixel(x, y).ToColor();
  }

  public void SetPixel (int x, int y, Color color)
  {
    SkiaBitmap.SetPixel(x, y, color.ToSkColor());
  }

  public void Save (string filePath)
  {
    ArgumentException.ThrowIfNullOrEmpty(filePath);

    var imageData = SKImage.FromBitmap(SkiaBitmap).Encode(SKEncodedImageFormat.Png, 100);
    using var fileStream = File.Create(filePath);
    imageData.SaveTo(fileStream);
  }

  public byte[] SaveToByteArray ()
  {
    var imageData = SKImage.FromBitmap(SkiaBitmap).Encode(SKEncodedImageFormat.Png, 100);
    using var memoryStream = new MemoryStream();
    imageData.SaveTo(memoryStream);

    return memoryStream.ToArray();
  }

  public void Dispose ()
  {
    SkiaBitmap.Dispose();
  }
}
