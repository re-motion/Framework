// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Web.UI;

namespace Remotion.Web.ContentSecurityPolicy;

/// <summary>
/// Adapter for <see cref="ICspClientScriptManager"/> to work with <see cref="ISmartPage"/>.
/// Registers scripts using smartPage.ClientScript.RegisterStartupScriptBlock.
/// </summary>
public class SmartPageCspClientScriptManagerAdapter : ICspClientScriptManager
{
  private readonly ISmartPage _smartPage;

  public SmartPageCspClientScriptManagerAdapter (ISmartPage smartPage)
  {
    ArgumentNullException.ThrowIfNull(smartPage);

    _smartPage = smartPage;
  }

  /// <inheritdoc />
  public void RegisterScript (string key, string script)
  {
    ArgumentException.ThrowIfNullOrEmpty(key);
    ArgumentException.ThrowIfNullOrEmpty(script);

    _smartPage.ClientScript.RegisterStartupScriptBlock(_smartPage, typeof(CspEnabledHtmlTextWriter), key, script);
  }
}
