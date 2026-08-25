// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using Microsoft.Playwright;

namespace Remotion.Web.Development.WebTesting;

public class BrowserWindow
{
  private readonly IPage _page;

  public BrowserWindow (IPage page)
  {
    _page = page;
  }

  public string Title => _page.TitleAsync().GetAwaiter().GetResult();

  public void AcceptModalDialog ()
  {
    throw new NotImplementedException();
  }

  public void CancelModalDialog ()
  {
    throw new NotImplementedException();
  }

  public void ExecuteJavaScript (string script, params object?[] args)
  {
    // TODO: this should probably support passing elements
    var wrappedScript = $"(() => {{ {script} }})();";
    _page.EvaluateAsync(wrappedScript, args).GetAwaiter().GetResult();
  }

  public T? ExecuteJavaScript<T> (string script, params object?[] args)
  {
    // TODO: this should probably support passing elements
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

  public T Query<T> (Func<T> func, T expectedResult, TimeSpan? timeout = null)
  {
    var actualTimeout = timeout ?? TimeSpan.FromSeconds(30);

    var start = Stopwatch.GetTimestamp();
    do
    {
      var result = func();
      if (Equals(result, expectedResult))
        return result;

      Thread.Sleep(500);
    } while (Stopwatch.GetElapsedTime(start) < actualTimeout);

    throw new InvalidOperationException("Query did not result in something.");
  }
}
