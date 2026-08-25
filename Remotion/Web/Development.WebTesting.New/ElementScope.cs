// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Playwright;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting;

public class ElementScope
{
  private readonly BrowserWindow _window;
  private readonly ILocator _locator;

  public ElementScope (BrowserWindow window, ILocator locator)
  {
    _window = window;
    _locator = locator;
  }

  public string Id => _locator.GetAttributeAsync("id").GetAwaiter().GetResult() ?? throw new InvalidOperationException("The element has no ID.");

  public string InnerHTML => _locator.InnerHTMLAsync().GetAwaiter().GetResult() ?? throw new InvalidOperationException("bla");

  public bool Selected => throw new NotImplementedException();

  public string Text => _locator.TextContentAsync().GetAwaiter().GetResult() ?? "";

  public BrowserWindow Window => _window;

  public string Value => _locator.InputValueAsync().GetAwaiter().GetResult();
  public bool Disabled => _locator.IsDisabledAsync().GetAwaiter().GetResult();

  public void Click ()
  {
    _locator.ClickAsync().GetAwaiter().GetResult();
  }

  public void EnsureExistence ()
  {
    var count = _locator.CountAsync().GetAwaiter().GetResult();
    if (count == 0)
      throw new InvalidOperationException($"The locator '{_locator}' does not exist.");
  }

  public ElementScope FindCss (string css)
  {
    return new ElementScope(_window, _locator.Locator($"css={css}"));
  }

  public bool ExistsWorkaround ()
  {
    return Exists();
  }

  public ElementScope FindId (string htmlID)
  {
    return new ElementScope(_window, _locator.Locator($"#{htmlID}"));
  }

  public ElementScope FindIdEndingWith (string localID)
  {
    return FindCss($"*[id$=\"{localID}\"]");
  }

  public void Press (string key)
  {
    _locator.PressAsync(key).GetAwaiter().GetResult();
  }

  public void Focus ()
  {
    _locator.FocusAsync().GetAwaiter().GetResult();
  }

  public string this [string attributeName]
  {
    get => _locator.GetAttributeAsync(attributeName).GetAwaiter().GetResult()!;
  }

  public void EnsureSingle ()
  {
    var count = _locator.CountAsync().GetAwaiter().GetResult();
    if (count == 0)
      throw new InvalidOperationException($"The locator '{_locator}' does not exist.");
    if (count > 0)
      throw new InvalidOperationException($"The locator '{_locator}' selects more than one element.");
  }

  public bool Exists ()
  {
    return _locator.CountAsync().GetAwaiter().GetResult() > 0;
  }

  public ElementScope FindXPath (string format)
  {
    return new ElementScope(_window, _locator.Locator($"xpath={format}"));
  }

  public ElementScope FindTagWithAttribute (string tagSelector, string attributeName, string attributeValue)
  {
    var cssSelector = string.Format("{0}[{1}={2}]", tagSelector, attributeName, DomSelectorUtility.CreateMatchValueForCssSelector(attributeValue));
    return FindCss(cssSelector);
  }

  public ElementScope FindTagWithAttributes (string tagSelector, Dictionary<string, string> attributes)
  {
    const string dmaCheckPattern = "[{0}={1}]";
    var dmaCheck = string.Concat(
        attributes.Select(dm => string.Format(dmaCheckPattern, dm.Key, DomSelectorUtility.CreateMatchValueForCssSelector(dm.Value))));
    var cssSelector = tagSelector + dmaCheck;
    return FindCss(cssSelector);
  }

  public ElementScope FindChild (string idSuffix)
  {
    var fullId = $"{Id}_{idSuffix}";
    return FindId(fullId);
  }

  public string GetAttribute (string attributeName)
  {
    var value = _locator.GetAttributeAsync(attributeName).GetAwaiter().GetResult();

    return value ?? throw new InvalidOperationException("NOPE");
  }
}
