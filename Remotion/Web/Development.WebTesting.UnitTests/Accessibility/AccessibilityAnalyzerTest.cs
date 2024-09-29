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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.Accessibility.AxeJsonResultDtos;
using Remotion.Web.Development.WebTesting.Accessibility.Implementation;

namespace Remotion.Web.Development.WebTesting.UnitTests.Accessibility
{
  [TestFixture]
  public class AccessibilityAnalyzerTest
  {
    private Mock<IAxeSourceProvider> _sourceProviderStub;
    private Mock<IAxeResultParser> _resultParserStub;
    private Mock<IAccessibilityResultMapper> _resultMapperStub;
    private Mock<IAccessibilityConfiguration> _configStub;
    private Mock<IJavaScriptExecutor> _jsExecutorStub;
    private Mock<IWebDriver> _webDriverStub;
    private FakeLogCollector _fakeLogCollector;

    [SetUp]
    public void SetUp ()
    {
      _jsExecutorStub = new Mock<IJavaScriptExecutor>();
      _sourceProviderStub = new Mock<IAxeSourceProvider>();
      _resultParserStub = new Mock<IAxeResultParser>();
      _configStub = new Mock<IAccessibilityConfiguration>();
      _webDriverStub = new Mock<IWebDriver>();
      _resultMapperStub = new Mock<IAccessibilityResultMapper>();
      _fakeLogCollector = new FakeLogCollector();
    }

    [Test]
    public void CreateAnalyzer_CopiesGivenConfig ()
    {
      _configStub.Setup(_ => _.IFrameTimeout).Returns(TimeSpan.FromSeconds(5));
      _configStub.Setup(_ => _.IncludeIFrames).Returns(true);
      var analyzer = CreateAccessibilityAnalyzer();

      Assert.That(analyzer.IncludeIFrames, Is.True);
      Assert.That(analyzer.IFrameTimeout, Is.EqualTo(TimeSpan.FromSeconds(5)));
    }

    [Test]
    public void Initialize_ChangesDefaultConfiguration ()
    {
      _configStub.Setup(_ => _.IFrameTimeout).Returns(TimeSpan.FromSeconds(5));
      _configStub.Setup(_ => _.IncludeIFrames).Returns(true);
      var analyzer = CreateAccessibilityAnalyzer();

      analyzer.IFrameTimeout = TimeSpan.FromSeconds(15);
      analyzer.IncludeIFrames = false;

      Assert.That(analyzer.IncludeIFrames, Is.False);
      Assert.That(analyzer.IFrameTimeout, Is.EqualTo(TimeSpan.FromSeconds(15)));
    }

    [Test]
    public void Analyze_EmptyId_ThrowsException ()
    {
      var analyzer = CreateAccessibilityAnalyzer();

      Assert.That(() => analyzer.Analyze(""), Throws.ArgumentException);
    }

    [Test]
    public void Analyze_ExecuteAsyncReturnsNull_ThrowsInvalidOperationException ()
    {
      _jsExecutorStub.Setup(_ => _.ExecuteAsyncScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns((object)null);
      _jsExecutorStub.Setup(_ => _.ExecuteScript("return (typeof axe !== 'undefined')")).Returns(true);
      _jsExecutorStub.Setup(_ => _.ExecuteScript("return self.name;")).Returns("TestFrame");
      _webDriverStub.Setup(_ => _.FindElements(It.IsAny<By>())).Returns(new List<IWebElement>().AsReadOnly());
      _webDriverStub.Setup(_ => _.SwitchTo().DefaultContent());
      _configStub.Setup(_ => _.ConformanceLevel).Returns(AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA);
      var analyzer = CreateAccessibilityAnalyzer();

      Assert.That(
          () => analyzer.Analyze(),
          Throws.TypeOf(typeof(InvalidOperationException)).With.Message.EqualTo("Could not obtain accessibility analysis result."));
    }

    [Test]
    public void Analyze_IEPath ()
    {
      var axeResult = new AxeResult();
      var accessibilityResult = new AccessibilityResult(
          DateTime.MinValue,
          "url",
          "version",
          0,
          0,
          0,
          "orientationType",
          "userAgent",
          true,
          AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA,
          new List<AccessibilityRuleResult>());
      _jsExecutorStub.Setup(_ => _.ExecuteAsyncScript(It.IsAny<string>(),It.IsAny<object[]>())).Returns(null);
      _jsExecutorStub.Setup(_ => _.ExecuteAsyncScript(It.IsAny<string>(),It.IsAny<object[]>())).Returns("{}");
      _jsExecutorStub.Setup(_ => _.ExecuteScript("return (typeof axe !== 'undefined')")).Returns(true);
      _jsExecutorStub.Setup(_ => _.ExecuteScript("return self.name;")).Returns("TestFrame");
      _webDriverStub.Setup(_ => _.FindElements(It.IsAny<By>())).Returns(new List<IWebElement>().AsReadOnly());
      _webDriverStub.Setup(_ => _.SwitchTo().DefaultContent());
      _resultParserStub.Setup(_ => _.Parse("{}")).Returns(axeResult);
      _resultMapperStub.Setup(_ => _.Map(axeResult)).Returns(accessibilityResult);
      _configStub.Setup(_ => _.ConformanceLevel).Returns(AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA);
      var analyzer = CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze();

      Assert.That(result, Is.SameAs(accessibilityResult));
    }

    [Test]
    public void Analyze_LogsInjectionTime ()
    {
      var parsedAxeResult = new AxeResult();
      const string axeSource = "AxeSource";
      const string axeResult = "AxeResult";
      _sourceProviderStub.Setup(_ => _.GetSource()).Returns(axeSource);
      _jsExecutorStub.Setup(_ => _.ExecuteScript("return (typeof axe !== 'undefined')")).Returns(false);
      _jsExecutorStub.Setup(_ => _.ExecuteScript(axeSource)).Returns(true);
      _jsExecutorStub.Setup(_ => _.ExecuteAsyncScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(axeResult);
      _resultParserStub.Setup(_ => _.Parse(axeResult)).Returns(parsedAxeResult);
      _resultMapperStub.Setup(_ => _.Map(parsedAxeResult)).Returns((AccessibilityResult)null);
      _configStub.Setup(_ => _.ConformanceLevel).Returns(AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA);
      var analyzer = CreateAccessibilityAnalyzer();

      analyzer.Analyze("SomeCssSelector");

      var logRecords = _fakeLogCollector.GetSnapshot();
      var debugLogRecords = logRecords.Where(l => l.Level == Microsoft.Extensions.Logging.LogLevel.Debug).ToArray();
      Assert.That(debugLogRecords, Has.Some.With.Property(nameof(FakeLogRecord.Message)).Match(@"aXe has been injected. \[took: 00:00:00.000\d\d\d\d\]"));
    }

    [Test]
    public void Analyze_LogsAnalysisTime ()
    {
      var parsedAxeResult = new AxeResult();
      const string axeSource = "AxeSource";
      const string axeResult = "AxeResult";
      _sourceProviderStub.Setup(_ => _.GetSource()).Returns(axeSource);
      _jsExecutorStub.Setup(_ => _.ExecuteScript("return (typeof axe !== 'undefined')")).Returns(false);
      _jsExecutorStub.Setup(_ => _.ExecuteScript(axeSource)).Returns(true);
      _jsExecutorStub.Setup(_ => _.ExecuteAsyncScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(axeResult);
      _resultParserStub.Setup(_ => _.Parse(axeResult)).Returns(parsedAxeResult);
      _resultMapperStub.Setup(_ => _.Map(parsedAxeResult)).Returns((AccessibilityResult)null);
      _configStub.Setup(_ => _.ConformanceLevel).Returns(AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA);
      var analyzer = CreateAccessibilityAnalyzer();

      analyzer.Analyze("SomeCssSelector");

      var logRecords = _fakeLogCollector.GetSnapshot();
      var debugLogRecords = logRecords.Where(l => l.Level == Microsoft.Extensions.Logging.LogLevel.Debug).ToArray();
      Assert.That(debugLogRecords, Has.Some.With.Property(nameof(FakeLogRecord.Message)).Match(@"Accessibility analysis has been performed. \[took: 00:00:00.000\d\d\d\d\]"));
    }

    private TestableAccessibilityAnalyzer CreateAccessibilityAnalyzer ()
    {
      return new TestableAccessibilityAnalyzer(
          _webDriverStub.Object,
          _jsExecutorStub.Object,
          _resultParserStub.Object,
          _configStub.Object,
          _sourceProviderStub.Object,
          _resultMapperStub.Object,
          new FakeLogger(_fakeLogCollector));
    }

    private class TestableAccessibilityAnalyzer : AccessibilityAnalyzer
    {
      public TestableAccessibilityAnalyzer (
          IWebDriver webDriver,
          IJavaScriptExecutor jsExecutor,
          IAxeResultParser resultParser,
          IAccessibilityConfiguration configuration,
          IAxeSourceProvider sourceProvider,
          IAccessibilityResultMapper mapper,
          ILogger logger)
          : base(webDriver, jsExecutor, resultParser, configuration, sourceProvider, mapper, logger)
      {
      }
    }
  }
}
