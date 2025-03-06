// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Resources;

namespace Remotion.Web.Development.WebTesting.UnitTests.Resources;

[TestFixture]
public class LiberationSansTest
{
  [Test]
  public void CanLoadFonts ()
  {
    Assert.That(
        () => LiberationsSans.Bold().SkiaFont.Typeface.FamilyName,
        Is.EqualTo("Liberation Sans"));
    Assert.That(
        () => LiberationsSans.BoldItalic().SkiaFont.Typeface.FamilyName,
        Is.EqualTo("Liberation Sans"));
    Assert.That(
        () => LiberationsSans.Italic().SkiaFont.Typeface.FamilyName,
        Is.EqualTo("Liberation Sans"));
    Assert.That(
        () => LiberationsSans.Regular().SkiaFont.Typeface.FamilyName,
        Is.EqualTo("Liberation Sans"));
  }

  [Test]
  public void DisposingFontDoesNotBreakFutureInstances ()
  {
    LiberationsSans.Regular().Dispose();

    Assert.That(
        () => LiberationsSans.Regular().SkiaFont.Typeface.FamilyName,
        Is.EqualTo("Liberation Sans"));
  }
}
