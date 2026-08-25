// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Microsoft.Playwright;

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

  public string InnerHTML => _locator.InnerHTMLAsync().GetAwaiter().GetResult();

  public BrowserWindow Window => _window;

  public void EnsureExistence ()
  {
    var count = _locator.CountAsync().GetAwaiter().GetResult();
    if (count == 0)
      throw new InvalidOperationException($"The locator '{_locator}' does not exist.");
  }

  public ElementScope FindCss (string css)
  {
    throw new NotImplementedException();
  }

  public bool ExistsWorkaround ()
  {
    throw new NotImplementedException();
  }

  public ElementScope FindId (string htmlID)
  {
    throw new NotImplementedException();
  }

  public ElementScope FindIdEndingWith (string localID)
  {
    throw new NotImplementedException();
  }
}
