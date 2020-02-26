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
using NUnit.Framework;
using OpenQA.Selenium;
using log4net;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.Accessibility.AxeJsonResultDtos;
using Remotion.Web.Development.WebTesting.Accessibility.Implementation;
using Rhino.Mocks;

namespace Remotion.Web.Development.WebTesting.UnitTests.Accessibility
{
  [TestFixture]
  public class AccessibilityAnalyzerTest
  {
    private IAxeSourceProvider _sourceProviderStub;
    private IAxeResultParser _resultParserStub;
    private IAccessibilityResultMapper _resultMapperStub;
    private IAccessibilityConfiguration _configStub;
    private IJavaScriptExecutor _jsExecutorStub;
    private IWebDriver _webDriverStub;
    private ILog _loggerStub;

    [SetUp]
    public void SetUp ()
    {
      _jsExecutorStub = MockRepository.GenerateStub<IJavaScriptExecutor>();
      _sourceProviderStub = MockRepository.GenerateStub<IAxeSourceProvider>();
      _resultParserStub = MockRepository.GenerateStub<IAxeResultParser>();
      _configStub = MockRepository.GenerateStub<IAccessibilityConfiguration>();
      _webDriverStub = MockRepository.GenerateStub<IWebDriver>();
      _resultMapperStub = MockRepository.GenerateStub<IAccessibilityResultMapper>();
      _loggerStub = MockRepository.GenerateStub<ILog>();
    }

    [Test]
    public void CreateAnalyzer_CopiesGivenConfig ()
    {
      _configStub.Stub (_ => _.IFrameTimeout).Return (TimeSpan.FromSeconds (5));
      _configStub.Stub (_ => _.IncludeIFrames).Return (true);
      var analyzer = CreateAccessibilityAnalyzer();

      Assert.That (analyzer.IncludeIFrames, Is.True);
      Assert.That (analyzer.IFrameTimeout, Is.EqualTo (TimeSpan.FromSeconds (5)));
    }

    [Test]
    public void Initialize_ChangesDefaultConfiguration ()
    {
      _configStub.Stub (_ => _.IFrameTimeout).Return (TimeSpan.FromSeconds (5));
      _configStub.Stub (_ => _.IncludeIFrames).Return (true);
      var analyzer = CreateAccessibilityAnalyzer();

      analyzer.IFrameTimeout = TimeSpan.FromSeconds (15);
      analyzer.IncludeIFrames = false;

      Assert.That (analyzer.IncludeIFrames, Is.False);
      Assert.That (analyzer.IFrameTimeout, Is.EqualTo (TimeSpan.FromSeconds (15)));
    }

    [Test]
    public void Analyze_EmptyId_ThrowsException ()
    {
      var analyzer = CreateAccessibilityAnalyzer();

      Assert.That (() => analyzer.Analyze (""), Throws.ArgumentException);
    }

    [Test]
    public void Analyze_ExecuteAsyncReturnsNull_ThrowsInvalidOperationException ()
    {
      _jsExecutorStub.Stub (_ => _.ExecuteAsyncScript (null, null)).IgnoreArguments().Return (null);
      _jsExecutorStub.Stub (_ => _.ExecuteScript ("return (typeof axe !== 'undefined')")).Return (true);
      _jsExecutorStub.Stub (_ => _.ExecuteScript ("return self.name;")).Return ("TestFrame");
      _webDriverStub.Stub (_ => _.FindElements (null)).IgnoreArguments().Return (new List<IWebElement>().AsReadOnly());
      _webDriverStub.Stub (_ => _.SwitchTo().DefaultContent());
      _configStub.Stub (_ => _.ConformanceLevel).Return (AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA);
      var analyzer = CreateAccessibilityAnalyzer();

      Assert.That (
          () => analyzer.Analyze(),
          Throws.TypeOf (typeof (InvalidOperationException)).With.Message.EqualTo ("Could not obtain accessibility analysis result."));
    }

    [Test]
    public void Analyze_IEPath ()
    {
      var axeResult = new AxeResult();
      var accessibilityResult = new AccessibilityResult (
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
      _jsExecutorStub.Stub (_ => _.ExecuteAsyncScript (null, null)).IgnoreArguments().Return (null).Repeat.Once();
      _jsExecutorStub.Stub (_ => _.ExecuteAsyncScript (null, null)).IgnoreArguments().Return ("{}");
      _jsExecutorStub.Stub (_ => _.ExecuteScript ("return (typeof axe !== 'undefined')")).Return (true);
      _jsExecutorStub.Stub (_ => _.ExecuteScript ("return self.name;")).Return ("TestFrame");
      _webDriverStub.Stub (_ => _.FindElements (null)).IgnoreArguments().Return (new List<IWebElement>().AsReadOnly());
      _webDriverStub.Stub (_ => _.SwitchTo().DefaultContent());
      _resultParserStub.Stub (_ => _.Parse ("{}")).Return (axeResult);
      _resultMapperStub.Stub (_ => _.Map (axeResult)).Return (accessibilityResult);
      _configStub.Stub (_ => _.ConformanceLevel).Return (AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA);
      var analyzer = CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze();

      Assert.That (result, Is.SameAs (accessibilityResult));
    }

    [Test]
    public void Analyze_LogsInjectionTime ()
    {
      var parsedAxeResult = new AxeResult();
      const string axeSource = "AxeSource";
      const string axeResult = "AxeResult";
      _sourceProviderStub.Stub (_ => _.GetSource()).Return (axeSource);
      _jsExecutorStub.Stub (_ => _.ExecuteScript ("return (typeof axe !== 'undefined')")).Return (false);
      _jsExecutorStub.Stub (_ => _.ExecuteScript (axeSource)).Return (true);
      _jsExecutorStub.Stub (_ => _.ExecuteAsyncScript (null, null)).IgnoreArguments().Return (axeResult);
      _resultParserStub.Stub (_ => _.Parse (axeResult)).Return (parsedAxeResult);
      _resultMapperStub.Stub (_ => _.Map (parsedAxeResult)).Return (null);
      _configStub.Stub (_ => _.ConformanceLevel).Return (AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA);
      var analyzer = CreateAccessibilityAnalyzer();

      analyzer.Analyze ("SomeCssSelector");

      _loggerStub.AssertWasCalled (_ => _.DebugFormat (Arg<string>.Is.Equal ("aXe has been injected. [took: {0}]"), Arg<TimeSpan>.Is.Anything));
    }

    [Test]
    public void Analyze_LogsAnalysisTime ()
    {
      var parsedAxeResult = new AxeResult();
      const string axeSource = "AxeSource";
      const string axeResult = "AxeResult";
      _sourceProviderStub.Stub (_ => _.GetSource()).Return (axeSource);
      _jsExecutorStub.Stub (_ => _.ExecuteScript ("return (typeof axe !== 'undefined')")).Return (false);
      _jsExecutorStub.Stub (_ => _.ExecuteScript (axeSource)).Return (true);
      _jsExecutorStub.Stub (_ => _.ExecuteAsyncScript (null, null)).IgnoreArguments().Return (axeResult);
      _resultParserStub.Stub (_ => _.Parse (axeResult)).Return (parsedAxeResult);
      _resultMapperStub.Stub (_ => _.Map (parsedAxeResult)).Return (null);
      _configStub.Stub (_ => _.ConformanceLevel).Return (AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA);
      var analyzer = CreateAccessibilityAnalyzer();

      analyzer.Analyze ("SomeCssSelector");

      _loggerStub.AssertWasCalled (_ => _.DebugFormat (Arg<string>.Is.Equal ("Accessibility analysis has been performed. [took: {0}]"), Arg<TimeSpan>.Is.Anything));
    }

    private TestableAccessibilityAnalyzer CreateAccessibilityAnalyzer ()
    {
      return new TestableAccessibilityAnalyzer (
          _webDriverStub,
          _jsExecutorStub,
          _resultParserStub,
          _configStub,
          _sourceProviderStub,
          _resultMapperStub,
          _loggerStub);
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
          ILog logger)
          : base (webDriver, jsExecutor, resultParser, configuration, sourceProvider, mapper, logger)
      {
      }
    }
  }
}