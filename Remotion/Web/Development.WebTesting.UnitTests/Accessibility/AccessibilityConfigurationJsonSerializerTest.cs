using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.Accessibility.Implementation;

namespace Remotion.Web.Development.WebTesting.UnitTests.Accessibility
{
  public class AccessibilityConfigurationJsonSerializerTest
  {
    [Test]
    public void Serialize_WCAG20A_AddsCorrectLevel ()
    {
      const string expected =
          "{iframes: false, xpath: true, frameWaitTime: 13000, absolutePaths: true, restoreScroll: true, runOnly: {type:\"tag\",values:[\"wcag2a\"]}}";

      var configuration = new AccessibilityConfiguration (
          AccessibilityConformanceLevel.Wcag20_ConformanceLevelA,
          false,
          TimeSpan.FromSeconds (13),
          true,
          true);

      var result = AccessibilityConfigurationJsonSerializer.Serialize (configuration);

      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void Serialize_WCAG20AA_AddsCorrectLevelAndAllBelow ()
    {
      const string expected =
          "{iframes: false, xpath: true, frameWaitTime: 13000, absolutePaths: true, restoreScroll: true, runOnly: {type:\"tag\",values:[\"wcag2aa\",\"wcag2a\"]}}";

      var configuration = new AccessibilityConfiguration (
          AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA,
          false,
          TimeSpan.FromSeconds (13),
          true,
          true);

      var result = AccessibilityConfigurationJsonSerializer.Serialize (configuration);

      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void Serialize_WCAG21A_AddsCorrectLevelAndAllBelowAndExperimental ()
    {
      const string expected =
          "{iframes: false, xpath: true, frameWaitTime: 13000, absolutePaths: true, restoreScroll: true, runOnly: {type:\"tag\",values:[\"wcag21a\",\"wcag2a\",\"experimental\"]}}";

      var configuration = new AccessibilityConfiguration (
          AccessibilityConformanceLevel.Wcag21_ConformanceLevelA,
          false,
          TimeSpan.FromSeconds (13),
          true,
          true);

      var result = AccessibilityConfigurationJsonSerializer.Serialize (configuration);

      Assert.That (result, Is.EqualTo (expected));
    }

    [Test]
    public void Serialize_WCAG21AA_AddsCorrectLevelAndAllBelowAndExperimental ()
    {
      const string expected =
          "{iframes: false, xpath: true, frameWaitTime: 13000, absolutePaths: true, restoreScroll: true, runOnly: {type:\"tag\",values:[\"wcag21aa\",\"wcag2a\",\"wcag21a\",\"wcag2aa\",\"experimental\"]}}";

      var configuration = new AccessibilityConfiguration (
          AccessibilityConformanceLevel.Wcag21_ConformanceLevelDoubleA,
          false,
          TimeSpan.FromSeconds (13),
          true,
          true);

      var result = AccessibilityConfigurationJsonSerializer.Serialize (configuration);

      Assert.That (result, Is.EqualTo (expected));
    }
  }
}