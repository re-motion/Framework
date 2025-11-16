// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Drawing;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

namespace Remotion.Web.Development.WebTesting.UnitTests.ScreenshotCreation.Drawing;

[TestFixture]
public class SkiaExtensionsTest
{
  [Test]
  public void ColorConversions ()
  {
    var color = Color.FromArgb(255, 100, 150, 200);
    var skColor = color.ToSkColor();
    var convertedBack = skColor.ToColor();

    Assert.That(convertedBack, Is.EqualTo(color));
  }

  [Test]
  public void RectangleConversions ()
  {
    var rect = new Rectangle(10, 20, 30, 40);
    var skRect = rect.ToSkRect();
    var convertedBack = skRect.ToRectangle();

    Assert.That(convertedBack, Is.EqualTo(rect));
  }
}
