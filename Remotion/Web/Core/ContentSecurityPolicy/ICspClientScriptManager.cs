// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
namespace Remotion.Web.ContentSecurityPolicy;

/// <summary>
/// Abstraction used by <see cref="CspEnabledHtmlTextWriter"/> to allow different ways to register CSP client scripts.
/// </summary>
public interface ICspClientScriptManager
{
  /// <summary>
  /// Registers the specified <paramref name="script"/> using the specified <paramref name="key"/>.
  /// If the key already exists, a previously registered script will be overriden.
  /// </summary>
  void RegisterScript (string key, string script);
}
