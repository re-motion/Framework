// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Text.Json;
using Microsoft.Playwright;

namespace Remotion.Web.Development.WebTesting;

public class BrowserWindow
{
  private readonly IPage _page;

  public BrowserWindow (IPage page)
  {
    _page = page;
  }

  public void AcceptModalDialog ()
  {
    throw new NotImplementedException();
  }

  public void CancelModalDialog ()
  {
    throw new NotImplementedException();
  }

  public T? ExecuteJavaScript<T> (string script, params object?[] args)
  {
    var wrappedScript = $"(() => {{ {script} }})();";
    var jsonElement = _page.EvaluateAsync(wrappedScript, args).GetAwaiter().GetResult();

    object? result;
    if (!jsonElement.HasValue)
    {
      result = null;
    }
    else
    {
      var jsonValue = jsonElement.Value;
      if (jsonValue.ValueKind == JsonValueKind.String)
      {
        result = jsonValue.GetString();
      }
      else
      {
        throw new InvalidOperationException($"Unsupported JSON node returned by eval '{jsonValue.ValueKind}'.");
      }
    }

    return (T?)result;
  }

  public ElementScope GetRootScope ()
  {
    return new ElementScope(this, _page.Locator(":root"));
  }

  public void MaximiseWindow ()
  {
    throw new NotImplementedException();
  }

  public void Close ()
  {
    throw new NotImplementedException();
  }
}
