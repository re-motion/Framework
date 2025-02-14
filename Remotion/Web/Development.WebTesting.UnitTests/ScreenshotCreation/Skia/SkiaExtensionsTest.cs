// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using System.IO;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Skia;
using SkiaSharp;
using Color = System.Drawing.Color;
using Font = Remotion.Web.Development.WebTesting.ScreenshotCreation.Font;
using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;

namespace Remotion.Web.Development.WebTesting.UnitTests.ScreenshotCreation.Skia;

[TestFixture]
public class SkiaExtensionsTest
{
  private static readonly byte[] s_orangeTestOnTransparentBackgroundImageAsBytes =
  [
      137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 50, 0, 0, 0, 50, 8, 6, 0, 0, 0, 30, 63, 136, 177, 0, 0, 0, 4, 115, 66, 73, 84, 8,
      8, 8, 8, 124, 8, 100, 136, 0, 0, 1, 134, 73, 68, 65, 84, 104, 129, 237, 213, 49, 110, 194, 48, 20, 6, 224, 223, 86, 114, 136, 74, 193, 32, 70, 134, 228,
      6, 160, 150, 123, 148, 10, 6, 98, 245, 14, 81, 219, 59, 160, 164, 67, 145, 232, 61, 26, 137, 35, 196, 67, 71, 20, 66, 37, 14, 145, 200, 238, 128, 76,
      41, 148, 182, 3, 81, 150, 247, 109, 206, 123, 182, 19, 251, 197, 6, 8, 33, 132, 16, 66, 8, 33, 132, 16, 66, 8, 33, 132, 16, 66, 8, 33, 23, 192, 234, 24,
      52, 11, 197, 27, 99, 184, 57, 23, 119, 120, 217, 233, 205, 182, 235, 58, 230, 174, 141, 146, 94, 148, 73, 177, 186, 248, 184, 211, 86, 95, 73, 97, 108,
      155, 95, 122, 130, 166, 56, 77, 78, 158, 77, 197, 45, 227, 88, 216, 182, 209, 24, 5, 207, 197, 171, 109, 191, 223, 95, 181, 43, 237, 230, 251, 184, 65,
      26, 36, 197, 80, 133, 173, 23, 48, 54, 6, 0, 187, 43, 141, 237, 136, 146, 94, 4, 142, 7, 63, 46, 152, 31, 23, 204, 225, 101, 135, 113, 44, 212, 180,
      213, 183, 57, 149, 118, 115, 163, 49, 178, 57, 140, 233, 37, 0, 248, 201, 102, 2, 109, 6, 0, 96, 99, 13, 150, 22, 127, 116, 121, 121, 109, 91, 189, 217,
      118, 13, 99, 230, 96, 24, 1, 187, 221, 0, 0, 215, 41, 151, 54, 199, 143, 63, 158, 206, 141, 214, 72, 105, 217, 85, 175, 180, 155, 43, 41, 190, 197, 140,
      65, 10, 236, 62, 44, 11, 69, 90, 105, 55, 207, 66, 145, 6, 73, 49, 252, 109, 204, 70, 255, 145, 191, 142, 97, 251, 242, 153, 20, 43, 37, 133, 129, 49,
      115, 63, 217, 76, 126, 202, 109, 164, 180, 28, 167, 90, 3, 64, 85, 57, 237, 255, 228, 7, 113, 209, 133, 54, 3, 48, 54, 182, 37, 119, 172, 145, 15, 217,
      255, 15, 156, 45, 15, 159, 43, 233, 69, 182, 236, 118, 247, 132, 23, 217, 152, 1, 235, 236, 251, 226, 107, 49, 108, 126, 45, 55, 251, 241, 203, 25, 240,
      187, 32, 46, 186, 39, 177, 131, 99, 20, 248, 58, 94, 129, 211, 163, 23, 56, 45, 197, 227, 254, 132, 144, 243, 62, 1, 10, 83, 161, 56, 48, 255, 100, 255,
      0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130
  ];

  [Test]
  public void Clone_CreatesExactCopy ()
  {
    using var data = new MemoryStream(s_orangeTestOnTransparentBackgroundImageAsBytes);
    using var image = SKImage.FromEncodedData(data);
    using var bitmap = SKBitmap.FromImage(image);

    var clone = bitmap.Clone();

    using var resultImage = SKImage.FromBitmap(bitmap);
    using var resultData = image.Encode(SKEncodedImageFormat.Png, 100);
    var resultBytes = data.ToArray();

    Assert.That(resultBytes, Is.EqualTo(s_orangeTestOnTransparentBackgroundImageAsBytes));
    Assert.That(bitmap != clone);
  }

  [Test]
  public void DrawString_RendersExpectedOutput ()
  {
    var canvas = new SkiaCanvas();
    var bitmap = new SKBitmap(50, 50);
    var surface = new SKCanvas(bitmap);
    canvas.Canvas = surface;

    var font = new Font(SKTypeface.FromFamilyName("SansSerif"), 16);
    var brush = new SolidBrush(Color.Chocolate);
    var rect = new Rectangle(0, 0, 50, 50);

    canvas.DrawString("Test", font, brush, rect, HorizontalAlignment.Right, VerticalAlignment.Bottom, true);

    using var image = SKImage.FromBitmap(bitmap);
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    var bytes = data.ToArray();

    //var debug = bytes.Aggregate("", (current, item) => current + item + ", ");
    //bitmap.Save(@"C:\Temp\test.png");
    //throw new NotImplementedException(debug);

    Assert.That(bytes, Is.EqualTo(s_orangeTestOnTransparentBackgroundImageAsBytes));
  }

  [Test]
  public void MeasureString_ReturnsExpectedResult ()
  {
    var canvas = new SkiaCanvas();
    var font = new Font(SKTypeface.FromFamilyName("Arial"));
    var layoutArea = new SizeF(50, 60);

    var resultF = canvas.MeasureString("Hello World", font, layoutArea);
    var result = new Size((int)resultF.Width, (int)resultF.Height);

    Assert.That(result.Width, Is.EqualTo(34));
    Assert.That(result.Height, Is.EqualTo(27));
  }

  [Test]
  public void MeasureString_NoWrap_ReturnsExpectedResult ()
  {
    var canvas = new SkiaCanvas();
    var font = new Font(SKTypeface.FromFamilyName("Arial"));
    var layoutArea = new SizeF(50, 60);

    var resultF = canvas.MeasureString("Hello World", font, layoutArea, wrapLines: false);
    var result = new Size((int)resultF.Width, (int)resultF.Height);

    Assert.That(result.Width, Is.EqualTo(50));
    Assert.That(result.Height, Is.EqualTo(13));
  }

  [Test]
  public void MeasureString_LongText_ReturnsExpectedResult ()
  {
    var canvas = new SkiaCanvas();
    var font = new Font(SKTypeface.FromFamilyName("Arial"));
    var layoutArea = new SizeF(50, 60);

    var resultF = canvas.MeasureString("Hello World, this is a long text that should wrap", font, layoutArea);
    var result = new Size((int)resultF.Width, (int)resultF.Height);

    Assert.That(result.Width, Is.EqualTo(48));
    Assert.That(result.Height, Is.EqualTo(60));
  }

  [Test]
  public void MeasureString_NoText_ReturnsZeroSize ()
  {
    var canvas = new SkiaCanvas();
    var font = new Font(SKTypeface.FromFamilyName("Arial"));
    var layoutArea = new SizeF(50, 60);

    var resultF = canvas.MeasureString("", font, layoutArea);
    var result = new Size((int)resultF.Width, (int)resultF.Height);

    Assert.That(result.Width, Is.EqualTo(0));
    Assert.That(result.Height, Is.EqualTo(0));
  }

  [Test]
  public void MeasureString_NullText_ReturnsZeroSize ()
  {
    var canvas = new SkiaCanvas();
    var font = new Font(SKTypeface.FromFamilyName("Arial"));
    var layoutArea = new SizeF(50, 60);

    var resultF = canvas.MeasureString(null, font, layoutArea);
    var result = new Size((int)resultF.Width, (int)resultF.Height);

    Assert.That(result.Width, Is.EqualTo(0));
    Assert.That(result.Height, Is.EqualTo(0));
  }

  [Test]
  public void MeasureString_UnlimitedLayoutArea_ReturnsExpectedResult ()
  {
    var canvas = new SkiaCanvas();
    var font = new Font(SKTypeface.FromFamilyName("Arial"));
    var layoutArea = new SizeF(0, 0);

    var resultF = canvas.MeasureString("Hello World", font, layoutArea);
    var result = new Size((int)resultF.Width, (int)resultF.Height);

    Assert.That(result.Width, Is.EqualTo(65));
    Assert.That(result.Height, Is.EqualTo(13));
  }

  [Test]
  public void Save_CreatesFileSuccessfully ()
  {
    var bitmap = new SKBitmap(10, 10);
    var path = Path.GetTempFileName();

    bitmap.Save(path);

    Assert.That(File.Exists(path), Is.True);
  }

  [Test]
  public void ColorConversions_WorkCorrectly ()
  {
    var color = Color.FromArgb(255, 100, 150, 200);
    var skColor = color.ToSkColor();
    var convertedBack = skColor.ToColor();

    Assert.That(convertedBack, Is.EqualTo(color));
  }

  [Test]
  public void RectangleConversions_WorkCorrectly ()
  {
    var rect = new Rectangle(10, 20, 30, 40);
    var skRect = rect.ToSkRect();
    var convertedBack = skRect.ToRectangle();

    Assert.That(convertedBack, Is.EqualTo(rect));
  }
}
