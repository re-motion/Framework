using System.Text;
using System.Xml;

namespace Remotion.Web.Development.WebTesting.Accessibility.Implementation
{
  public static class AccessibilityConfigurationJsonSerializer
  {
    public static string Serialize (IAccessibilityConfiguration configuration)
    {
      var sb = new StringBuilder();
      sb.Append ('{');
      sb.Append ($"iframes: {configuration.IncludeIFrames.ToString().ToLower()}");
      sb.Append ($", xpath: {configuration.EnableXPath.ToString().ToLower()}");
      sb.Append ($", frameWaitTime: {configuration.IFrameTimeout.TotalMilliseconds}");
      sb.Append ($", absolutePaths: {configuration.EnableAbsolutePaths.ToString().ToLower()}");
      sb.Append ($", restoreScroll: {configuration.EnableScrollToInitialPosition.ToString().ToLower()}");
      sb.Append (", runOnly: {type:\"tag\",values:[");
      sb.Append ($"\"{AccessibilityConformanceLevelConverter.ConvertToString (configuration.ConformanceLevel)}\"");

      if (configuration.ConformanceLevel == AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA)
        AppendWcag2A (sb);

      //axe version 3.2.2 requires the experimental tag for WCAG 2.1 A and WCAG 2.1 AA
      if (configuration.ConformanceLevel == AccessibilityConformanceLevel.Wcag21_ConformanceLevelA)
      {
        AppendWcag2A (sb);
        AppendExperimental (sb);
      }

      if (configuration.ConformanceLevel == AccessibilityConformanceLevel.Wcag21_ConformanceLevelDoubleA)
      {
        AppendWcag2A (sb);
        AppendWcag21A (sb);
        AppendWcag2DoubleA (sb);
        AppendExperimental (sb);
      }

      sb.Append ("]}}");

      return sb.ToString();

      void AppendWcag2A (StringBuilder stringBuilder) => stringBuilder.Append (",\"wcag2a\"");
      void AppendWcag21A (StringBuilder stringBuilder) => stringBuilder.Append (",\"wcag21a\"");
      void AppendWcag2DoubleA (StringBuilder stringBuilder) => stringBuilder.Append (",\"wcag2aa\"");
      void AppendExperimental (StringBuilder stringBuilder) => stringBuilder.Append (",\"experimental\"");
    }
  }
}