// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.IntegrationTests;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.IntegrationTests.ContentSecurityPolicy;

/// <summary>
/// Test the rendering components related to CSP using a custom test site.
/// The basic setup is as follows:
///  - The test page contains a log element in which output is collected
///  - Different JS sources (inline scripts, startup scripts, controls, etc.) write a custom message to the output
///  - Tests enable/disable CSP and check what elements log in the output
///  - Depending on the CSP settings some elements get blocked and a console error is asserted in the test
/// </summary>
[TestFixture]
public class CspRenderTest : IntegrationTest
{
  private enum CspMode
  {
    Off,
    Enabled,
    ReportOnly
  }

  private class CspRenderPageObject : PageObject
  {
    public CspRenderPageObject ([NotNull] PageObjectContext context)
        : base(context)
    {
    }

    public string[] GetOutput ()
    {
      var text = Context.Scope.FindId("output").Text;
      return text.Split(';')
          .Where(e => !string.IsNullOrWhiteSpace(e))
          .Select(e => e.Trim())
          .ToArray();
    }

    public void TriggerAsyncPostback ()
    {
        var syncDate = Context.Scope.FindId("syncDate").Text;
        Context.Scope.FindId("asyncPostback").Click();
        RetryUntilTimeout.Run(
                Logger,
                () => Assert.That(Context.Scope.FindId("asyncDate").Text, Is.Not.EqualTo(syncDate)));
    }

    /// <summary>
    /// Triggers an inline event handler rendered by the CspRenderTestControl.
    /// We can't trigger this automatically, so we sadly need an explicit call in each of the tests.
    /// </summary>
    public void ClickTestLink ()
    {
        Context.Scope.FindId("myTestLink").Click();
    }
  }

  /// <summary>
  /// Tests that all the different script sources log correctly when there is no CSP enabled.
  /// </summary>
  [Test]
  public void NoCspHeader ()
  {
    var home = Start(CspMode.Off);

    home.ClickTestLink();

    Assert.That(
        home.GetOutput(),
        Is.EquivalentTo(
                new[]
                {
                        "INLINE EVENT HANDLER",
                        "INLINE SCRIPT",
                        "REGISTERED STARTUP SCRIPT",
                        "CONTROL INLINE ATTRIBUTE",
                        "CONTROL INLINE SCRIPT"
                }));

    AssertErrors(home, errorCount: 0, reportOnlyErrors: 0);
  }

  /// <summary>
  /// Tests that enabling CSP blocks the inline script controls on page load.
  /// </summary>
  [Test]
  public void WithCspHeader ()
  {
    var home = Start(CspMode.Enabled);

    home.ClickTestLink();

    Assert.That(
        home.GetOutput(),
        Is.EquivalentTo(
                new[]
                {
                        "REGISTERED STARTUP SCRIPT",
                        "CONTROL INLINE ATTRIBUTE",
                        "CONTROL INLINE SCRIPT"
                }));

    AssertErrors(home, errorCount: 2, reportOnlyErrors: 0);
  }

  /// <summary>
  /// Tests that report-only works correctly. That is, it does not block but reports the errors.
  /// </summary>
  [Test]
  public void WithCspReportOnlyHeader ()
  {
    var home = Start(CspMode.ReportOnly);

    home.ClickTestLink();

    Assert.That(
        home.GetOutput(),
        Is.EquivalentTo(
                new[]
                {
                        "INLINE EVENT HANDLER",
                        "INLINE SCRIPT",
                        "REGISTERED STARTUP SCRIPT",
                        "CONTROL INLINE ATTRIBUTE",
                        "CONTROL INLINE SCRIPT"
                }));

    AssertErrors(home, errorCount: 2, reportOnlyErrors: 2);
  }

  /// <summary>
  /// Tests that triggering an async postback without CSP will execute the relevant startup script once more.
  /// </summary>
  /// <remarks>
  /// The inline script in the control does not execute again because of the way ASP.NET sets the new HTML content.
  /// </remarks>
  [Test]
  public void AsyncPostBack_WithNoCsp ()
  {
    var home = Start(CspMode.Off);

    home.TriggerAsyncPostback();
    home.ClickTestLink();

    Assert.That(
        home.GetOutput(),
        Is.EquivalentTo(
                new[]
                {
                        "INLINE EVENT HANDLER",
                        "INLINE SCRIPT",
                        "REGISTERED STARTUP SCRIPT",
                        "REGISTERED STARTUP SCRIPT",
                        "CONTROL INLINE ATTRIBUTE",
                        "CONTROL INLINE SCRIPT"
                }));

    AssertErrors(home, errorCount: 0, reportOnlyErrors: 0);
  }

  /// <summary>
  /// Tests that triggering an async postback with CSP enabled will execute the relevant startup script once more,
  /// while blocking the inline scripts.
  /// </summary>
  /// <remarks>
  /// The inline script in the control does not execute again because of the way ASP.NET sets the new HTML content.
  /// </remarks>
  [Test]
  public void AsyncPostBack_WithCsp ()
  {
    var home = Start(CspMode.Enabled);

    home.TriggerAsyncPostback();
    home.ClickTestLink();

    Assert.That(
        home.GetOutput(),
        Is.EquivalentTo(
                new[]
                {
                        "REGISTERED STARTUP SCRIPT",
                        "REGISTERED STARTUP SCRIPT",
                        "CONTROL INLINE SCRIPT"
                }));

    AssertErrors(home, errorCount: 3, reportOnlyErrors: 0);
  }

  private void AssertErrors (PageObject page, int errorCount, int reportOnlyErrors)
  {
      var errors = new List<string>();
      RetryUntilTimeout.Run(
              page.Logger,
              () =>
              {
                  errors.AddRange(GetErrors(page));
                  if (errors.Count < errorCount)
                      throw new InvalidOperationException($"Expected {errorCount} errors but got only {errors.Count}.");
              });

    Assert.That(
        errors.Count,
        Is.EqualTo(errorCount));
    Assert.That(
        FilterForReportOnly(errors).Length,
        Is.EqualTo(reportOnlyErrors));
  }

  private string[] GetErrors (PageObject pageObject)
  {
    var browserLogs = pageObject.Context.Browser.GetBrowserLogs();
    return browserLogs
        .Where(e => e.Level == LogLevel.Severe)
        .Where(e => e.Message.Contains("Content Security Policy") || e.Message.Contains("Content-Security-Policy"))
        .Select(e => e.Message)
        .ToArray();
  }

  private string[] FilterForReportOnly (IReadOnlyList<string> errors)
  {
    return errors
        .Where(e => e.Contains("[Report Only]") || e.Contains("(Report-Only policy)"))
        .ToArray();
  }

  private CspRenderPageObject Start (CspMode cspMode)
  {
    return Start<CspRenderPageObject>($"ContentSecurityPolicy/CspRenderTest.aspx?cspMode={cspMode}");
  }
}
