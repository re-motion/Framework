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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using Coypu;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various general extension methods for Coypu's <see cref="ElementScope"/> class.
  /// </summary>
  public static class CoypuElementScopeExtensions
  {
    private static readonly Lazy<FieldInfo> s_driverFieldInfo = new Lazy<FieldInfo>(
        () => Assertion.IsNotNull(
            typeof(ElementScope).GetField("_driver", BindingFlags.NonPublic | BindingFlags.Instance),
            "Coypu has changed, update CoypuElementScopeExtensions.GetDriver() method."),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// This method ensures that the given <paramref name="scope"/> is existent. Coypu would automatically check for existence whenever we access the
    /// scope, however, we explicitly check the existence when creating new control objects. This ensures that any <see cref="MissingHtmlException"/>
    /// or <see cref="AmbiguousException"/> are thrown when the control object's corresponding <see cref="WebTestObjectContext"/> is created, which
    /// is always near the corresponding <c>parentScope.Find*()</c> method call. Otherwise, exceptions would be thrown when the context's
    /// <see cref="Scope"/> is actually used for the first time, which may be quite some time later and the exception would provide a stack trace
    /// where the <c>parentScope.Find*()</c> call could not be found.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> which is asserted to exist.</param>
    /// <exception cref="WebTestException">
    /// If the control cannot be found.
    /// <para>- or -</para>
    /// If multiple matching controls are found.
    /// </exception>
    public static void EnsureExistence ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      try
      {
        scope.Now();
      }
      catch (MissingHtmlException exception)
      {
        throw AssertionExceptionUtility.CreateControlMissingException(scope.GetDriver(), exception.Message);
      }
      catch (AmbiguousException exception)
      {
        throw AssertionExceptionUtility.CreateControlAmbiguousException(scope.GetDriver(), exception.Message);
      }
    }

    /// <summary>
    /// Ensures that the given <see cref="Scope"/> exists, much like <see cref="EnsureExistence"/>. However, it ensures that Coypu uses the
    /// <see cref="Match.Single"/> matching strategy.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> which is asserted to match only a single DOM element.</param>
    /// <exception cref="WebTestException">
    /// If the control cannot be found.
    /// <para>- or -</para>
    /// If multiple matching controls are found.
    /// </exception>
    public static void EnsureSingle ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      var matchBackup = scope.ElementFinder.Options.Match;

      try
      {
        scope.ElementFinder.Options.Match = Match.Single;
        scope.Now();
      }
      catch (MissingHtmlException exception)
      {
        throw AssertionExceptionUtility.CreateControlMissingException(scope.GetDriver(), exception.Message);
      }
      catch (AmbiguousException exception)
      {
        throw AssertionExceptionUtility.CreateControlAmbiguousException(scope.GetDriver(), exception.Message);
      }
      finally
      {
        scope.ElementFinder.Options.Match = matchBackup;
      }
    }

    /// <summary>
    /// Coypu`s <paramref name="scope"/>.<see cref="ElementScope.Exists"/> does not work correctly when working with iframes (see https://www.re-motion.org/jira/projects/RM/issues/RM-6770).
    /// </summary>
    /// <remarks>
    /// As a workaround, we call <paramref name="scope"/>.<see cref="DriverScope.Now"/> and return <see langword="false" /> if an <see cref="MissingHtmlException"/>, <see cref="MissingWindowException"/> or <see cref="StaleElementException"/> is thrown.
    /// These are the same exceptions that Coypu is catching in its <paramref name="scope"/>.<see cref="ElementScope.Exists"/> call.
    /// Should be removed when the issue is fixed in coypu. See RM-6773.
    /// </remarks>
    /// <param name="scope">The <see cref="ElementScope"/> which is asserted to exist.</param>
    /// <exception cref="WebTestException">If multiple matching controls are found.</exception>
    public static bool ExistsWorkaround ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      var scopeTimeoutBackup = scope.ElementFinder.Options.Timeout;
      try
      {
        scope.ElementFinder.Options.Timeout = TimeSpan.Zero;

        try
        {
          scope.Exists(Options.NoWait);
        }
        catch (Exception)
        {
          // Sometimes, the first scope.Exists() call fails after a switch to an IFrame. So we swallow that first exception and just try again.
        }
        var exists = scope.Exists(Options.NoWait);

        // scope.Exists (...) does not work correctly in some circumstances, whereby we do the exist check via this workaround. See RM-6773 for details.
        if (!exists)
        {
          try
          {
            scope.Now();
          }
          catch (Exception)
          {
            // Sometimes the first scope.Now() call fails although the element exist. So we swallow that first exception and just try again.
          }

          scope.Now();
        }

        return true;
      }
      catch (MissingHtmlException)
      {
        return false;
      }
      catch (MissingWindowException)
      {
        return false;
      }
      catch (StaleElementException)
      {
        return false;
      }
      catch (StaleElementReferenceException)
      {
        return false;
      }
      catch (AmbiguousException exception)
      {
        throw AssertionExceptionUtility.CreateControlAmbiguousException(scope.GetDriver(), exception.Message);
      }
      finally
      {
        scope.ElementFinder.Options.Timeout = scopeTimeoutBackup;
      }
    }

    /// <summary>
    /// Exists-workaround which also ensures that the element is single (e.g. throws an <see cref="WebTestException"/> if the element is not single).
    /// </summary>
    /// <remarks>
    /// This has to be done in its own function, as calling <see cref="EnsureSingle"/> after <see cref="ExistsWorkaround"/> does not throw the expected exception.
    /// This is due to caching of the <paramref name="scope"/>.<see cref="DriverScope.Now"/> call. <see cref="ExistsWorkaround"/> calls <paramref name="scope"/>.<see cref="DriverScope.Now"/> without <see cref="Match.Single"/> matching strategy ->
    /// <see cref="EnsureSingle"/> calls <paramref name="scope"/>.<see cref="DriverScope.Now"/> with <see cref="Match.Single"/> matching strategy.
    /// If .<see cref="ExistsWorkaround"/> is called before <see cref="EnsureSingle"/>, the scope is cached and no evaluation takes place.
    /// Should be removed when the issue is fixed in coypu https://www.re-motion.org/jira/browse/RM-6773.
    /// </remarks>
    /// <param name="scope">The <see cref="ElementScope"/> which is asserted to match only a single DOM element.</param>
    public static bool ExistsWithEnsureSingleWorkaround ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      var matchBackup = scope.ElementFinder.Options.Match;
      scope.ElementFinder.Options.Match = Match.Single;

      var scopeExists = scope.ExistsWorkaround();

      scope.ElementFinder.Options.Match = matchBackup;

      return scopeExists;
    }

    /// <summary>
    /// Returns the computed background color of the control. This method ignores background images as well as transparencies - the first
    /// non-transparent color set in the node's hierarchy is returned. The returned color's alpha value is always 255 (opaque).
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> for which the background color is computed.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    /// <returns>The background color or <see cref="WebColor.Transparent"/> if no background color is set (not even on any parent node).</returns>
    public static WebColor GetComputedBackgroundColor ([NotNull] this ElementScope scope, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("logger", logger);

      var computedBackgroundColor = (string)scope.GetDriver().ExecuteScript(CommonJavaScripts.GetComputedBackgroundColor, scope, scope.Native);

      if (IsTransparent(computedBackgroundColor))
        return WebColor.Transparent;

      return ParseColorFromBrowserReturnedString(computedBackgroundColor);
    }

    /// <summary>
    /// Returns the computed text color of the control. This method ignores transparencies - the first non-transparent color set in the node's
    /// DOM hierarchy is returned. The returned color's alpha value is always 255 (opaque).
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> for which the text color is computed.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    /// <returns>The text color or <see cref="WebColor.Transparent"/> if no text color is set (not even on any parent node).</returns>
    public static WebColor GetComputedTextColor ([NotNull] this ElementScope scope, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("logger", logger);

      var computedTextColor = (string)scope.GetDriver().ExecuteScript(CommonJavaScripts.GetComputedTextColor, scope, scope.Native);

      if (IsTransparent(computedTextColor))
        return WebColor.Transparent;

      return ParseColorFromBrowserReturnedString(computedTextColor);
    }

    /// <summary>
    /// Returns the <see cref="IDriver"/> object of an <see cref="ElementScope"/> instance.
    /// </summary>
    /// <returns>The <see cref="IDriver"/> instance held by <paramref name="scope"/>.</returns>
    internal static IDriver GetDriver ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      return (IDriver)s_driverFieldInfo.Value.GetValue(scope)!;
    }

    private static bool IsTransparent ([NotNull] string color)
    {
      ArgumentUtility.CheckNotNullOrEmpty("color", color);

      if (color == "rgba(0, 0, 0, 0)")
        return true;

      if (color == "transparent")
        return true;

      return false;
    }

    private static WebColor ParseColorFromBrowserReturnedString ([NotNull] string color)
    {
      var rgbArgs = color.Split(new[] { '(', ',', ')' });
      var rgb = rgbArgs.Skip(1).Take(3).Select(byte.Parse).ToArray();
      return WebColor.FromRgb(rgb[0], rgb[1], rgb[2]);
    }

    /// <summary>
    /// Returns whether the given <paramref name="scope"/>, which must represent a suitable HTML element (e.g. a checkbox), is currently selected.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> of the element for which the HTML selection state is checked.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    /// <returns>True if the HTML element is selected, otherwise false.</returns>
    public static bool IsSelected ([NotNull] this ElementScope scope, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("logger", logger);

      return RetryUntilTimeout.Run(
          logger,
          () =>
          {
            var webElement = (IWebElement)scope.Native;
            return webElement.Selected;
          });
    }

    /// <summary>
    /// Returns whether the given <paramref name="scope"/> is currently displayed (visible). The given <paramref name="scope"/> must exist, otherwise
    /// this method will throw an <see cref="MissingHtmlException"/>.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> of the element for which the visibility is checked.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    /// <returns>True if the given <paramref name="scope"/> is visible, otherwise false.</returns>
    public static bool IsVisible ([NotNull] this ElementScope scope, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("logger", logger);

      return RetryUntilTimeout.Run(
          logger,
          () =>
          {
            var webElement = (IWebElement)scope.Native;
            return webElement.Displayed;
          });
    }

    /// <summary>
    /// Ensures unhovering of the given <paramref name="scope"/> by placing the cursor back at 0/0 in the top-left corner.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> of the element against which the unhover operation is performed.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    public static void Unhover ([NotNull] this ElementScope scope, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      Cursor.Position = new Point(0, 0);
    }

    /// <summary>
    /// Gets the specified HTML attribute's value from the <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> where the HTML attribute is defined.</param>
    /// <param name="attributeName">The name of the HTML attribute for which the value should be retrieved.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    /// <remarks>
    /// Accessing HTML attributes through <see cref="ElementScope.this"/> returns <see langword="null"/> if the attribute does not exist.
    /// By using <see cref="GetAttribute"/> instead, a <see cref="MissingHtmlException"/> is thrown if the HTML attribute does not exist on the scope.
    /// </remarks>
    /// <exception cref="MissingHtmlException">If the attribute cannot be found.</exception>
    [NotNull]
    public static string GetAttribute ([NotNull] this ElementScope scope, [NotNull] string attributeName, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty("attributeName", attributeName);
      ArgumentUtility.CheckNotNull("logger", logger);

      var result = scope[attributeName];

      if (result == null)
        throw new MissingHtmlException($"Cannot find the attribute '{attributeName}' on the current scope.");

      return result;
    }
  }
}
