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
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="BrowserWindow"/> class regaring modal dialog handling.
  /// </summary>
  public static class CoypuBrowserWindowModalDialogExtensions
  {
    /// <summary>
    /// IE-compatible version for Coypu's <see cref="BrowserWindow.AcceptModalDialog"/> method.
    /// </summary>
    /// <param name="window">The <see cref="BrowserWindow"/> on which the action is performed.</param>
    /// <param name="browser">The corresponding <see cref="BrowserSession"/> (internally required for IE-fixes).</param>
    public static void AcceptModalDialogFixed ([NotNull] this BrowserWindow window, [NotNull] BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("window", window);
      ArgumentUtility.CheckNotNull ("browser", browser);

      // Note: currently we have no IE-specific compatibility requirements.
      window.AcceptModalDialog();
    }

    /// <summary>
    /// See <see cref="AcceptModalDialogFixed"/>, however, the <see cref="WebTestingConfiguration.SearchTimeout"/> and
    /// <see cref="WebTestingConfiguration.RetryInterval"/> do not apply.
    /// </summary>
    public static void AcceptModalDialogImmediatelyFixed ([NotNull] this BrowserWindow window, [NotNull] BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("window", window);
      ArgumentUtility.CheckNotNull ("browser", browser);

      // Note: currently we have no IE-specific compatibility requirements.
      window.AcceptModalDialog (Options.NoWait);
    }

    /// <summary>
    /// IE-compatible version for Coypu's <see cref="BrowserWindow.CancelModalDialog"/> method.
    /// </summary>
    /// <param name="window">The <see cref="BrowserWindow"/> on which the action is performed.</param>
    /// <param name="browser">The corresponding <see cref="BrowserSession"/> (internally required for IE-fixes).</param>
    public static void CancelModalDialogFixed ([NotNull] this BrowserWindow window, [NotNull] BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("window", window);
      ArgumentUtility.CheckNotNull ("browser", browser);

      // Note: currently we have no IE-specific compatibility requirements.
      window.CancelModalDialog();
    }
  }
}