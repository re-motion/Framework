// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Drawing;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Resources;

namespace Remotion.Web.Development.WebTesting.UnitTests.ScreenshotCreation.Drawing;

[TestFixture]
public class FontTest
{
  [TestCase(null, 0, 0, false, 0, 0)]
  [TestCase(" ", 0, 0, false, 5.55f, 13.15f)]
  [TestCase("Test text", 0, 0, false, 41.12f, 13.15f)]
  [TestCase("Test text", 40, 0, false, 40f, 13.15f)]
  [TestCase("Test text", 40, 0, true, 22.22f, 24.65f)]
  [TestCase("Test text", 40, 12, true, 22.22f, 12f)]
  [Test]
  public void MeasureString (
      [CanBeNull] string text,
      int layoutWidth,
      int layoutHeight,
      bool wrapLines,
      float expectedWidth,
      float expectedHeight)
  {
    var font = LiberationsSans.Regular(10f);

    SizeF? layoutArea = null;
    if (layoutWidth != 0 || layoutHeight != 0)
      layoutArea = new SizeF(layoutWidth, layoutHeight);

    var result = font.MeasureString(
        text,
        layoutArea,
        wrapLines);

    Assert.Multiple(
        () =>
        {
          Assert.That(result.Width, Is.EqualTo(expectedWidth).Within(0.01));
          Assert.That(result.Height, Is.EqualTo(expectedHeight).Within(0.01));
        });
  }
}
