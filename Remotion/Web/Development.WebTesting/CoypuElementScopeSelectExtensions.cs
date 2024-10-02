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
using System.Linq;
using Coypu;
using Coypu.Drivers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Extension methods for Coypu's <see cref="ElementScope"/> class regarding <see cref="ElementScope.SelectOption"/>.
  /// </summary>
  public static class CoypuElementScopeSelectExtensions
  {
    private static readonly Html s_html = new Html();
    private static readonly XPath s_xpath = new XPath();

    /// <summary>
    /// Returns the text of the currently selected option. If more than one option is selected, this method returns the first selected item's text.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    /// <returns>The text of the currently selected option.</returns>
    public static OptionDefinition GetSelectedOption ([NotNull] this ElementScope scope, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("logger", logger);

      var selectedOptions = scope.FindAllCss("option[selected]").ToList();

      if (selectedOptions.Count() == 1)
        return new OptionDefinition(selectedOptions.First().Value, -1, scope.SelectedOption, true);

      // If we cant uniquely find an item per selected attribute, we have to use selenium directly
      return RetryUntilTimeout.Run(
          logger,
          () =>
          {
            var webElement = (IWebElement)scope.Native;

            var select = new SelectElement(webElement);
            var selectedOption = select.SelectedOption;
            return new OptionDefinition(selectedOption.GetAttribute("value"), -1, selectedOption.Text, selectedOption.Selected);
          });
    }

    /// <summary>
    /// Selects an option of a &lt;select&gt; element by <paramref name="oneBasedIndex"/>.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="oneBasedIndex">The one-based index of the option to select.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    public static void SelectOptionByIndex ([NotNull] this ElementScope scope, int oneBasedIndex, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("logger", logger);

      var targetOption = scope.FindXPath(string.Format("({0})[{1}]", s_html.Child("option"), oneBasedIndex));
      targetOption.Click();
    }

    /// <summary>
    /// Selects an option of a &lt;select&gt; element by <paramref name="displayText"/>.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="displayText">The display text to select.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    public static void SelectOptionByDisplayText ([NotNull] this ElementScope scope, [NotNull] string displayText, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("displayText", displayText);
      ArgumentUtility.CheckNotNull("logger", logger);

      var targetOption = scope.FindXPath(s_html.Child("option") + XPath.Where(s_xpath.IsText(displayText, Options.Exact)));
      targetOption.Click();
    }

    /// <summary>
    /// Selects an option of a &lt;select&gt; element by <paramref name="value"/>.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="value">The value to select.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    public static void SelectOptionByValue ([NotNull] this ElementScope scope, [NotNull] string value, [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNull("value", value);
      ArgumentUtility.CheckNotNull("logger", logger);

      var targetOption = scope.FindXPath(s_html.Child("option") + XPath.Where(s_xpath.Is("@value", value, Options.Exact)));
      targetOption.Click();
    }

    /// <summary>
    /// Selects an option of a &lt;select&gt; element by <see cref="DiagnosticMetadataAttributes"/> given by
    /// <paramref name="diagnosticMetadataAttributeName"/> and <paramref name="diagnosticMetadataAttributeValue"/>.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="diagnosticMetadataAttributeName">The diagnostic metadata attribute name.</param>
    /// <param name="diagnosticMetadataAttributeValue">The diagnostic metadata attribute value.</param>
    /// <param name="logger">
    /// The <see cref="ILogger"/> used by the web testing infrastructure for diagnostic output. The <paramref name="logger"/> can be retrieved from
    /// <see cref="WebTestObject{TWebTestObjectContext}"/>.<see cref="WebTestObject{TWebTestObjectContext}.Logger"/>.
    /// </param>
    public static void SelectOptionByDMA (
        [NotNull] this ElementScope scope,
        [NotNull] string diagnosticMetadataAttributeName,
        [NotNull] string diagnosticMetadataAttributeValue,
        [NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNull("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty("diagnosticMetadataAttributeName", diagnosticMetadataAttributeName);
      ArgumentUtility.CheckNotNullOrEmpty("diagnosticMetadataAttributeValue", diagnosticMetadataAttributeValue);
      ArgumentUtility.CheckNotNull("logger", logger);

      var targetOption =
          scope.FindXPath(
              s_html.Child("option") + XPath.Where(s_xpath.Attr(diagnosticMetadataAttributeName, diagnosticMetadataAttributeValue, Options.Exact)));
      targetOption.Click();
    }
  }
}
