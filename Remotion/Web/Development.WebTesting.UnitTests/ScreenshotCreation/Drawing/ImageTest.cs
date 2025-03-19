// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Drawing;
using System.IO;
using NUnit.Framework;
using Remotion.Development.UnitTesting.IO;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

namespace Remotion.Web.Development.WebTesting.UnitTests.ScreenshotCreation.Drawing;

[TestFixture]
public class ImageTest
{
  [Test]
  public void FromFile ()
  {
    var savedScreenshotBytes = SavedTestScreenshots.GetSavedScreenshotForTest("BasicColors.png");
    using var tempFile = new TempFile();
    tempFile.WriteAllBytes(savedScreenshotBytes);

    using var image = Image.FromFile(tempFile.FileName);

    Assert.That(image.Width, Is.EqualTo(2));
    Assert.That(image.Height, Is.EqualTo(3));
  }

  [Test]
  public void FromStream ()
  {
    var savedScreenshotBytes = SavedTestScreenshots.GetSavedScreenshotForTest("BasicColors.png");
    var memoryStream = new MemoryStream(savedScreenshotBytes, writable: false);

    using var image = Image.FromStream(memoryStream);

    Assert.That(image.Width, Is.EqualTo(2));
    Assert.That(image.Height, Is.EqualTo(3));
  }

  [Test]
  public void Construction ()
  {
    using var image = new Image(10, 13);

    Assert.That(image.Width, Is.EqualTo(10));
    Assert.That(image.Height, Is.EqualTo(13));
    Assert.That(image.Size, Is.EqualTo(new Size(10, 13)));
    Assert.That(image.SkiaBitmap, Is.Not.Null);
  }

  [Test]
  public void Clone ()
  {
    using var image = new Image(10, 13);

    var clone = image.Clone();
    Assert.That(clone.Width, Is.EqualTo(10));
    Assert.That(clone.Height, Is.EqualTo(13));

    Assert.That(clone, Is.Not.SameAs(image));
    Assert.That(clone.SkiaBitmap, Is.Not.SameAs(image.SkiaBitmap));
  }

  [Test]
  public void GetPixel ()
  {
    var savedScreenshotBytes = SavedTestScreenshots.GetSavedScreenshotForTest("BasicColors.png");
    var memoryStream = new MemoryStream(savedScreenshotBytes, writable: false);
    using var image = Image.FromStream(memoryStream);

    Assert.That(image.Width, Is.EqualTo(2));
    Assert.That(image.Height, Is.EqualTo(3));

    Assert.That(image.GetPixel(0, 0), Is.EqualTo(Color.FromArgb(255, 255, 0, 0)));
    Assert.That(image.GetPixel(1, 0), Is.EqualTo(Color.FromArgb(255, 0, 0, 255)));
    Assert.That(image.GetPixel(0, 1), Is.EqualTo(Color.FromArgb(255, 0, 255, 0)));
    Assert.That(image.GetPixel(1, 1), Is.EqualTo(Color.FromArgb(255, 255, 255, 255)));
    Assert.That(image.GetPixel(0, 2), Is.EqualTo(Color.FromArgb(255, 0, 0, 0)));
    Assert.That(image.GetPixel(1, 2), Is.EqualTo(Color.FromArgb(0, 0, 0, 0)));
  }

  [Test]
  public void SetPixel ()
  {
    using var image = new Image(2, 1);

    image.SetPixel(0, 0, Color.Red);

    Assert.That(image.GetPixel(0, 0), Is.EqualTo(Color.FromArgb(byte.MaxValue, 0, 0)));
    Assert.That(image.GetPixel(1, 0), Is.EqualTo(Color.FromArgb(0, 0, 0, 0)));
  }

  [Test]
  public void Save ()
  {
    using var image = new Image(10, 13);
    using var tempFile = new TempFile();

    image.SetPixel(5, 5, Color.FromArgb(255, 20, 30, 40));
    image.Save(tempFile.FileName);

    Assert.That(File.Exists(tempFile.FileName), Is.True);
    Assert.That(File.ReadAllBytes(tempFile.FileName).Length, Is.GreaterThan(0));

    using var loadedImage = Image.FromFile(tempFile.FileName);
    Assert.That(loadedImage.Width, Is.EqualTo(10));
    Assert.That(loadedImage.Height, Is.EqualTo(13));
    Assert.That(image.GetPixel(5, 5), Is.EqualTo(Color.FromArgb(255, 20, 30, 40)));
  }

  [Test]
  public void SaveToByteArray ()
  {
    using var image = new Image(10, 13);

    image.SetPixel(5, 5, Color.FromArgb(10, 20, 30, 40));
    var imageBytes = image.SaveToByteArray();

    Assert.That(imageBytes.Length, Is.GreaterThan(0));

    var memoryStream = new MemoryStream(imageBytes, writable: false);
    using var loadedImage = Image.FromStream(memoryStream);
    Assert.That(loadedImage.Width, Is.EqualTo(10));
    Assert.That(loadedImage.Height, Is.EqualTo(13));
    Assert.That(image.GetPixel(5, 5), Is.EqualTo(Color.FromArgb(10, 20, 30, 40)));
  }
}
