// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

namespace Remotion.Web.Development.WebTesting.UnitTests.ScreenshotCreation.Drawing;

[TestFixture]
public class CanvasTest
{
  [Test]
  public void FromImage ()
  {
    using var image = new Image(10, 10);
    using var canvas = Canvas.FromImage(image);

    Assert.That(canvas.SkiaCanvas, Is.Not.Null);
    Assert.That(canvas.SkiaCanvas.Canvas, Is.Not.Null);
  }

  [Test]
  public void SetTransform ()
  {
    using var image = new Image(10, 10);
    using var canvas = Canvas.FromImage(image);

    canvas.DrawRectangle(Pens.Red, new Rectangle(1, 0, 5, 6));
    canvas.SetTransform(2, 1);

    canvas.DrawRectangle(Pens.Blue, new Rectangle(1, 0, 5, 6));

    CompareToSavedImage(image, "SetTransform.png");
  }

  [Test]
  public void DrawEllipse ()
  {
    using var image = new Image(40, 40);
    using var canvas = Canvas.FromImage(image);

    canvas.DrawEllipse(Pens.Blue, new Rectangle(5, 10, 30, 20));

    CompareToSavedImage(image, "DrawEllipse.png");
  }

  [Test]
  public void FillEllipse ()
  {
    using var image = new Image(40, 40);
    using var canvas = Canvas.FromImage(image);

    canvas.FillEllipse(Brushes.Blue, new Rectangle(5, 10, 30, 20));

    CompareToSavedImage(image, "FillEllipse.png");
  }

  [Test]
  public void DrawRectangle ()
  {
    using var image = new Image(40, 40);
    using var canvas = Canvas.FromImage(image);

    canvas.DrawRectangle(Pens.Blue, new Rectangle(5, 10, 30, 20));

    CompareToSavedImage(image, "DrawRectangle.png");
  }

  [Test]
  public void FillRectangle ()
  {
    using var image = new Image(40, 40);
    using var canvas = Canvas.FromImage(image);

    canvas.FillRectangle(Brushes.Blue, new Rectangle(5, 10, 30, 20));

    CompareToSavedImage(image, "FillRectangle.png");
  }

  [Test]
  public void DrawImage ()
  {
    using var starImage = GetStarImage();

    using var image = new Image(40, 40);
    using var canvas = Canvas.FromImage(image);

    canvas.DrawImage(starImage, new Point(10, 5));

    CompareToSavedImage(image, "DrawImage.png");
  }

  [Test]
  public void DrawImage_WithCroppingRectangles ()
  {
    using var starImage = GetStarImage();

    using var image = new Image(40, 40);
    using var canvas = Canvas.FromImage(image);

    canvas.DrawImage(
        starImage,
        new Rectangle(10, 5, 30, 30),
        new Rectangle(0, 0, 15, 15));

    CompareToSavedImage(image, "DrawImage_WithCroppingRectangles.png");
  }

  private Image GetStarImage ()
  {
    var savedScreenshotBytes = SavedTestScreenshots.GetSavedScreenshotForTest("star.png");
    var memoryStream = new MemoryStream(savedScreenshotBytes, writable: false);
    return Image.FromStream(memoryStream);
  }

  private void CompareToSavedImage (Image left, string savedImageName)
  {
    Directory.CreateDirectory("ActualImages");
    var outputImagePath = $"ActualImages/{savedImageName}";
    left.Save(outputImagePath);

    var savedScreenshotBytes = SavedTestScreenshots.GetSavedScreenshotForTest(savedImageName);
    var memoryStream = new MemoryStream(savedScreenshotBytes, writable: false);
    using var right = Image.FromStream(memoryStream);

    Assert.That(left.Width, Is.EqualTo(right.Width));
    Assert.That(left.Height, Is.EqualTo(right.Height));

    for (var x = 0; x < left.Width; x++)
    {
      for (var y = 0; y < left.Height; y++)
      {
        if (left.GetPixel(x, y) != right.GetPixel(x, y))
        {
          Assert.Fail($"Images do not match at point {x},{y}.\r\n   Actual: {left.GetPixel(x, y)}\r\n Expected: {right.GetPixel(x, y)}\r\n Image location: '{outputImagePath}'");
        }
      }
    }
  }
}
