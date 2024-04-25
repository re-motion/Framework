// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Accessibility.Implementation;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.Accessibility
{
  /// <summary>
  /// Can analyze webpage with the given settings.
  /// </summary>
  public class AccessibilityAnalyzer
  {
    private static readonly TimeSpan s_defaultMaximumTimeToWaitForFrame = TimeSpan.FromMilliseconds(3000);

    /// <summary>
    /// Creates an <see cref="AccessibilityAnalyzer"/> that uses an instance of <see cref="IWebDriver"/> for both <see cref="IWebDriver"/>
    /// and <see cref="IJavaScriptExecutor"/>.
    /// </summary>
    [NotNull]
    public static AccessibilityAnalyzer CreateForWebDriver (
        [NotNull] IWebDriver webDriver,
        [NotNull] IAxeResultParser axeResultParser,
        [NotNull] IAccessibilityConfiguration configuration,
        [NotNull] IAxeSourceProvider sourceProvider,
        [NotNull] IAccessibilityResultMapper mapper,
        [NotNull] ILog logger)
    {
      ArgumentUtility.CheckNotNull("webDriver", webDriver);
      ArgumentUtility.CheckNotNull("axeResultParser", axeResultParser);
      ArgumentUtility.CheckNotNull("configuration", configuration);
      ArgumentUtility.CheckNotNull("sourceProvider", sourceProvider);
      ArgumentUtility.CheckNotNull("mapper", mapper);
      ArgumentUtility.CheckNotNull("logger", logger);

      return new AccessibilityAnalyzer(webDriver, (IJavaScriptExecutor)webDriver, axeResultParser, configuration, sourceProvider, mapper, logger);
    }

    /// <summary>
    /// Creates an <see cref="AccessibilityAnalyzer"/> that uses an instance of <see cref="RemoteWebDriver"/> for both <see cref="IWebDriver"/>
    /// and <see cref="IJavaScriptExecutor"/>.
    /// </summary>
    [Obsolete(
        "Use CreateForWebDriver(IWebDriver, IAxeResultParser, IAccessibilityConfiguration, IAxeSourceProvider, IAccessibilityResultMapper, ILog) instead. (Version: 3.20.0)",
        false)]
    [NotNull]
    public static AccessibilityAnalyzer CreateForRemoteWebDriver (
        [NotNull] RemoteWebDriver remoteWebDriver,
        [NotNull] IAxeResultParser axeResultParser,
        [NotNull] IAccessibilityConfiguration configuration,
        [NotNull] IAxeSourceProvider sourceProvider,
        [NotNull] IAccessibilityResultMapper mapper,
        [NotNull] ILog logger)
    {
      ArgumentUtility.CheckNotNull("remoteWebDriver", remoteWebDriver);
      ArgumentUtility.CheckNotNull("axeResultParser", axeResultParser);
      ArgumentUtility.CheckNotNull("configuration", configuration);
      ArgumentUtility.CheckNotNull("sourceProvider", sourceProvider);
      ArgumentUtility.CheckNotNull("mapper", mapper);
      ArgumentUtility.CheckNotNull("logger", logger);

      return new AccessibilityAnalyzer(remoteWebDriver, remoteWebDriver, axeResultParser, configuration, sourceProvider, mapper, logger);
    }

    /// <summary>
    /// Indicates whether IFrames are included.
    /// </summary>
    public bool IncludeIFrames { get; set; }

    /// <summary>
    /// Timeout of IFrames.
    /// </summary>
    public TimeSpan IFrameTimeout { get; set; }

    private IAxeSourceProvider AxeSourceProvider { get; }
    private IJavaScriptExecutor JsExecutor { get; }
    private IWebDriver WebDriver { get; }
    private IAxeResultParser AxeResultParser { get; }
    private IAccessibilityConfiguration Configuration { get; }
    private IAccessibilityResultMapper Mapper { get; }
    private ILog Logger { get; }
    private List<string> ExcludedElements { get; } = new List<string>();
    private List<string> ExcludedRules { get; } = new List<string>();

    protected AccessibilityAnalyzer (
        [NotNull] IWebDriver webDriver,
        [NotNull] IJavaScriptExecutor jsExecutor,
        [NotNull] IAxeResultParser axeResultParser,
        [NotNull] IAccessibilityConfiguration configuration,
        [NotNull] IAxeSourceProvider axeSourceProvider,
        [NotNull] IAccessibilityResultMapper mapper,
        [NotNull] ILog logger)
    {
      ArgumentUtility.CheckNotNull("webDriver", webDriver);
      ArgumentUtility.CheckNotNull("axeResultParser", axeResultParser);
      ArgumentUtility.CheckNotNull("configuration", configuration);
      ArgumentUtility.CheckNotNull("axeSourceProvider", axeSourceProvider);
      ArgumentUtility.CheckNotNull("mapper", mapper);
      ArgumentUtility.CheckNotNull("logger", logger);

      WebDriver = webDriver;
      JsExecutor = jsExecutor;
      AxeResultParser = axeResultParser;
      Configuration = configuration;
      IncludeIFrames = configuration.IncludeIFrames;
      IFrameTimeout = configuration.IFrameTimeout;
      AxeSourceProvider = axeSourceProvider;
      Mapper = mapper;
      Logger = logger;
    }

    /// <summary>
    /// Ignore the given <see cref="ControlObject"/> during accessibility analysis.
    /// </summary>
    /// <param name="element"><see cref="ControlObject"/> to ignore.</param>
    public void IgnoreControlObject ([NotNull] ControlObject element)
    {
      ArgumentUtility.CheckNotNull("element", element);

      IgnoreCssSelector($"#{element.GetHtmlID()}");
    }

    /// <summary>
    /// Ignore <see cref="ControlObject"/> with given css selector during the accessibility analysis.
    /// Does not work with the <c>html</c> element.
    /// </summary>
    /// <param name="cssSelector">CSS selector of the <see cref="ControlObject"/>.</param>
    public void IgnoreCssSelector ([NotNull] string cssSelector)
    {
      ArgumentUtility.CheckNotNullOrEmpty("cssSelector", cssSelector);

      ExcludedElements.Add(cssSelector);
    }

    /// <summary>
    /// Accessibility rule to ignore during the accessibility analysis.
    /// </summary>
    /// <param name="rule">The <see cref="AccessibilityRuleID"/> to ignore when executing the analysis.</param>
    public void IgnoreRule (AccessibilityRuleID rule)
    {
      ExcludedRules.Add(AccessibilityRuleIDConverter.ConvertToString(rule));
    }

    /// <summary>
    /// Analyzes specific <see cref="ControlObject"/> on Webpage.
    /// </summary>
    /// <param name="controlObject"><see cref="ControlObject"/> to analyze.</param>
    /// <param name="timeout">
    /// Maximum wait time after which an iframe on the page must be visible to be analyzed. If <see langword="null"/>, a default value of 3000 ms is used.
    /// </param>
    /// <returns> Result of the analysis.</returns>
    [NotNull]
    public AccessibilityResult Analyze ([NotNull] ControlObject controlObject, [CanBeNull] TimeSpan? timeout = null)
    {
      ArgumentUtility.CheckNotNull("controlObject", controlObject);

      return Analyze($"#{controlObject.GetHtmlID()}", timeout);
    }

    /// <summary>
    /// Analyzes a specific element on webpage.
    /// </summary>
    /// <param name="cssSelector">CSS selector of the <see cref="ControlObject"/> to analyze.</param>
    /// <param name="timeout">
    /// Maximum wait time after which an iframe on the page must be visible to be analyzed. If <see langword="null"/>, a default value of 3000 ms is used.
    /// </param>
    /// <returns> Result of the analysis.</returns>
    [NotNull]
    public AccessibilityResult Analyze ([NotNull] string cssSelector, [CanBeNull] TimeSpan? timeout = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty("cssSelector", cssSelector);

      return GetAccessibilityResult(cssSelector, timeout ?? s_defaultMaximumTimeToWaitForFrame);
    }

    /// <summary>
    /// Analyzes all content on a webpage.
    /// </summary>
    /// <param name="timeout">
    /// Maximum wait time after which an iframe on the page must be visible to be analyzed. If <see langword="null"/>, a default value of 3000 ms is used.
    /// </param>
    /// <remarks>
    /// Switches to the outermost frame before the analysis and back to
    /// the original frame after the analysis completed.
    /// </remarks>
    /// <returns> Results of the analysis.</returns>
    [NotNull]
    public AccessibilityResult Analyze ([CanBeNull] TimeSpan? timeout = null)
    {
      var outerFrame = (string)JsExecutor.ExecuteScript("return self.name;");
      if (outerFrame != "")
        WebDriver.SwitchTo().DefaultContent();

      var result = GetAccessibilityResult(null, timeout ?? s_defaultMaximumTimeToWaitForFrame);
      if (outerFrame != "")
        WebDriver.SwitchTo().Frame(outerFrame);

      return result;
    }

    private AccessibilityResult GetAccessibilityResult (string? cssSelector, TimeSpan timeout)
    {
      var source = AxeSourceProvider.GetSource();

      if (!AxeIsInjected())
      {
        using (new PerformanceTimer(Logger, "aXe has been injected."))
        {
          InjectAxeSource(source, timeout);
        }
      }

      var axeRunFunctionCall = BuildAxeRunFunctionCall(cssSelector);

      string result;
      using (new PerformanceTimer(Logger, "Accessibility analysis has been performed."))
      {
        result = (string)JsExecutor.ExecuteAsyncScript(axeRunFunctionCall);

        if (result == null)
          throw new InvalidOperationException("Could not obtain accessibility analysis result.");
      }

      var parsedResult = AxeResultParser.Parse(result);

      return Mapper.Map(parsedResult);
    }

    private bool AxeIsInjected ()
    {
      return (bool)JsExecutor.ExecuteScript("return (typeof axe !== 'undefined')");
    }

    /// <summary>
    /// Injects Axe into every iframe manually because axe-core cannot do it itself.
    /// </summary>
    /// <param name="source">Source of axe.min.js to inject.</param>
    /// <param name="timeout">Maximum wait time after which an iframe must be visible such that Axe can be injected.</param>
    /// <remarks>
    /// Source: https://www.deque.com/blog/writing-automated-tests-accessibility/
    /// </remarks>
    private void InjectIntoIFrames (string source, TimeSpan timeout)
    {
      foreach (var frame in WebDriver.FindElements(By.TagName("iframe")))
      {
        frame.WaitUntilFrameIsVisible(timeout);
        WebDriver.SwitchTo().Frame(frame);

        if (!IsEmptyFrame() && !AxeIsInjected())
          InjectAxeSource(source, timeout);

        WebDriver.SwitchTo().DefaultContent();
      }
    }

    private bool IsEmptyFrame ()
    {
      return !WebDriver.FindElements(By.XPath("/html/body/*")).Any();
    }

    private void InjectAxeSource (string source, TimeSpan timeout)
    {
      JsExecutor.ExecuteScript(source);

      if (Configuration.IncludeIFrames)
        InjectIntoIFrames(source, timeout);
    }

    private string BuildAxeRunFunctionCall (string? cssToInclude)
    {
      var stringBuilder = new StringBuilder();

      stringBuilder.Append("var callback = arguments[0];axe.run(");

      AppendAxeRunFunctionCallContext(stringBuilder, cssToInclude);
      stringBuilder.Append(AccessibilityConfigurationJsonSerializer.Serialize(Configuration));
      if (ExcludedRules.Any())
        AppendExcludeRules(stringBuilder);
      stringBuilder.Append(",function (err, results) {callback(JSON.stringify(results));});");
      return stringBuilder.ToString();
    }

    private void AppendExcludeRules (StringBuilder stringBuilder)
    {
      stringBuilder.Length--;
      stringBuilder.Append(",rules:{");
      stringBuilder.Append(ExcludedRules.Select(x => '"' + x + "\": {enabled: false}").Aggregate((i, j) => i + ",\n" + j));
      stringBuilder.Append("}}");
    }

    private void AppendAxeRunFunctionCallContext (StringBuilder stringBuilder, string? cssToInclude)
    {
      if (cssToInclude == null && !ExcludedElements.Any())
      {
        stringBuilder.Append("document,");
      }
      else
      {
        stringBuilder.Append('{');
        AppendAxeRunFunctionCallContextInclude(stringBuilder, cssToInclude);
        AppendAxeRunFunctionCallContextExclude(stringBuilder, cssToInclude);
        stringBuilder.Append("},");
      }
    }

    private void AppendAxeRunFunctionCallContextInclude (StringBuilder stringBuilder, string? cssToInclude)
    {
      if (cssToInclude != null)
        stringBuilder.Append($"include: [['{cssToInclude}']]");
    }

    private void AppendAxeRunFunctionCallContextExclude (StringBuilder stringBuilder, string? cssToInclude)
    {
      if (ExcludedElements.Count != 0)
      {
        if (cssToInclude != null)
          stringBuilder.Append(',');

        stringBuilder.Append("exclude: [");
        stringBuilder.Append(string.Join(", ", ExcludedElements.Select(s => $"['{s}']")));
        stringBuilder.Append(']');
      }
    }
  }
}
