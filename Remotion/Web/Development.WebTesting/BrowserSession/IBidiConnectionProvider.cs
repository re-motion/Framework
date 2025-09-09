// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Diagnostics.CodeAnalysis;
using OpenQA.Selenium.BiDi;

namespace Remotion.Web.Development.WebTesting.BrowserSession;
/// <summary>
/// Provides access to the bidirectional webdriver connection to a browser 
/// </summary>
public interface IBidiConnectionProvider: IDisposable
{
  /// <summary>
  /// Opens a Bidi Connection <see href="https://www.selenium.dev/documentation/webdriver/bidi/"/>.
  /// Note: In order to use Bidi you need to enable UseWebSocketUrl in the browser options for this session.
  /// </summary>
  [MemberNotNull(nameof(BiDiConnection))]
  void OpenBidiConnection ();

  /// <summary>
  /// Gets the open Bidi connection. Trying to access this before calling <see cref="OpenBidiConnection"/>
  /// should result in a <see cref="InvalidOperationException"/>
  /// </summary>
  BiDi BiDiConnection { get; }

  /// <summary>
  /// Used for timeouts for opening connections and interactions with the browser
  /// </summary>
  TimeSpan DefaultBidiTimeout { get; }
}
