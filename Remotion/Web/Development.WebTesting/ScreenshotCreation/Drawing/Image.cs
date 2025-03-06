// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using System.IO;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

public class Image : IDisposable
{
  public static Image FromFile (string filePath)
  {
    var skBitmap = SKBitmap.Decode(filePath);
    return new Image(skBitmap);
  }

  public static Image FromStream (Stream stream)
  {
    var skBitmap = SKBitmap.Decode(stream);
    return new Image(skBitmap);
  }

  public SKBitmap SkiaBitmap { get; }

  public Size Size => new(Width, Height);

  public int Height => SkiaBitmap.Height;

  public int Width => SkiaBitmap.Width;

  public Image (int width, int height)
  {
    SkiaBitmap = new SKBitmap(width, height);
  }

  public Image (SKBitmap skiaBitmap)
  {
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

  public void Save (string filePath)
  {
    var imageData = SKImage.FromBitmap(SkiaBitmap).Encode(SKEncodedImageFormat.Png, 100);
    using var fileStream = File.Create(filePath);
    imageData.SaveTo(fileStream);
  }

  public void Dispose ()
  {
    SkiaBitmap.Dispose();
  }
}
