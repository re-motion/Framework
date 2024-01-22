﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Represents settings for Chromium specific browsers.
  /// </summary>
  public interface IWebTestChromiumSettings
  {
    /// <summary>
    /// Specifies the wanted behavior related to the <c>CommandLineFlagSecurityWarningsEnabled</c> registry flag responsible for hiding infobars
    /// showing "Chrome is being controlled by automated test software". Default is <see cref="ChromiumDisableSecurityWarningsBehavior"/>.<see cref="ChromiumDisableSecurityWarningsBehavior.Ignore"/>.
    /// </summary>
    ChromiumDisableSecurityWarningsBehavior DisableSecurityWarningsBehavior { get; }
  }
}
