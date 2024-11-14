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
using Coypu;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies
{
  /// <summary>
  /// Various helper methods for all WXE-based <see cref="ICompletionDetectionStrategy"/> implementations.
  /// </summary>
  internal static class WxeCompletionDetectionHelpers
  {
    private const string c_wxeFunctionToken = "WxeFunctionToken";
    private const string c_dmaWxePostBackSequenceNumberFieldId = "dmaWxePostBackSequenceNumberField";

    /// <summary>
    /// Returns the current WXE postback sequence number in the given <paramref name="context"/>.
    /// </summary>
    public static int GetWxePostBackSequenceNumber ([NotNull] PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      return int.Parse(context.Scope.FindId(c_dmaWxePostBackSequenceNumberFieldId).Value);
    }

    /// <summary>
    /// Returns the current WXE function token in the given <paramref name="context"/>.
    /// </summary>
    public static string GetWxeFunctionToken ([NotNull] PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      return context.Scope.FindId(c_wxeFunctionToken).Value;
    }

    /// <summary>
    /// Waits for the WXE postback sequence number in the given <paramref name="context"/> to reach the
    /// <paramref name="expectedWxePostBackSequenceNumber"/>.
    /// </summary>
    public static void WaitForExpectedWxePostBackSequenceNumber (
        [NotNull] ILogger logger,
        [NotNull] PageObjectContext context,
        int expectedWxePostBackSequenceNumber,
        TimeSpan? timeout = null)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      ArgumentUtility.CheckNotNull("context", context);

      int newWxePostBackSequenceNumber;
      var options = timeout.HasValue ? new Options { Timeout = timeout.Value } : null;

      try
      {
        EnsureWindowContext(context);

        newWxePostBackSequenceNumber = context.Window.Query(
            () => GetWxePostBackSequenceNumber(context),
            expectedWxePostBackSequenceNumber,
            options);
      }
      catch (Exception)
      {
        try
        {
          logger.LogDebug("Parameters: window: '{0}' scope: '{1}'.", context.Window.Title, GetPageTitle(context));
        }
        catch
        {
          // ignored
        }

        context.RequestErrorDetectionStrategy.CheckPageForError(context.Scope);

        throw;
      }

      logger.LogDebug("Parameters: window: '{0}' scope: '{1}'.", context.Window.Title, GetPageTitle(context));

      Assertion.IsTrue(
          newWxePostBackSequenceNumber == expectedWxePostBackSequenceNumber,
          string.Format("Expected WXE-PSN to be '{0}', but it actually is '{1}'", expectedWxePostBackSequenceNumber, newWxePostBackSequenceNumber));
    }

    /// <summary>
    /// Waits for the WXE postback sequence number in the given <paramref name="context"/> to increase from
    /// <paramref name="oldWxePostBackSequenceNumber"/> by <paramref name="expectedWxePostBackSequenceNumberIncrease"/>.
    /// </summary>
    public static void WaitForExpectedWxePostBackSequenceNumber (
        [NotNull] ILogger logger,
        [NotNull] PageObjectContext context,
        int oldWxePostBackSequenceNumber,
        int expectedWxePostBackSequenceNumberIncrease,
        TimeSpan? timeout = null)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      ArgumentUtility.CheckNotNull("context", context);

      var expectedWxePostBackSequenceNumber = oldWxePostBackSequenceNumber + expectedWxePostBackSequenceNumberIncrease;

      logger.LogDebug("State: previous WXE-PSN: {0}, expected WXE-PSN: {1}.", oldWxePostBackSequenceNumber, expectedWxePostBackSequenceNumber);

      WaitForExpectedWxePostBackSequenceNumber(logger, context, expectedWxePostBackSequenceNumber, timeout);
    }

    /// <summary>
    /// Waits for the WXE function token to change from <paramref name="oldWxeFunctionToken"/> to a new function token.
    /// </summary>
    public static void WaitForNewWxeFunctionToken (
        [NotNull] ILogger logger,
        [NotNull] PageObjectContext context,
        [NotNull] string oldWxeFunctionToken,
        TimeSpan? timeout = null)
    {
      ArgumentUtility.CheckNotNull("logger", logger);
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("oldWxeFunctionToken", oldWxeFunctionToken);

      logger.LogDebug("State: previous WXE-FT: {0}.", oldWxeFunctionToken);
      var options = timeout.HasValue ? new Options { Timeout = timeout.Value } : null;

      try
      {
        EnsureWindowContext(context);

        context.Window.Query(() => GetWxeFunctionToken(context) != oldWxeFunctionToken, true, options);
      }
      catch (Exception)
      {
        try
        {
          logger.LogDebug("Parameters: window: '{0}' scope: '{1}'.", context.Window.Title, GetPageTitle(context));
        }
        catch
        {
          // ignored
        }

        context.RequestErrorDetectionStrategy.CheckPageForError(context.Scope);

        throw;
      }

      logger.LogDebug("Parameters: window: '{0}' scope: '{1}'.", context.Window.Title, GetPageTitle(context));

      Assertion.IsTrue(
          GetWxeFunctionToken(context) != oldWxeFunctionToken,
          string.Format("Expected WXE-FT to be different to '{0}', but it is equal.", oldWxeFunctionToken));
    }

    private static void EnsureWindowContext (PageObjectContext context)
    {
      //  Without this line, the WxePostBackSequenceNumber cannot be found in some MultiWindow Scenarios.
      // Concrete Scenario: TestMultiWindowActions()
      // var options = Opt.ContinueWhenAll (Wxe.PostBackCompletedIn (home.Frame), Wxe.PostBackCompletedInContext (window2.Context.ParentContext));
      // closeAndRefreshMainAsWellButton.Click (options);
      // This line probably activates some kind of cache, so the WxePostBackSequenceNumber can be found correctly.
      var notNeeded = context.Window.Title;
    }

    private static string GetPageTitle ([NotNull] PageObjectContext context)
    {
      // Note: do not use page.GetTitle() instead, it may be specifically overloaded for a certain type of page which is not yet fully loaded!

      ArgumentUtility.CheckNotNull("context", context);

      return context.Scope.FindCss("title").InnerHTML.Trim();
    }
  }
}
