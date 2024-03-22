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
using Remotion.ServiceLocation;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Implementation of the <see cref="IWxeLifetimeManagementSettings"/> interface which allows programmatic configuration.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IWxeLifetimeManagementSettings), Lifetime = LifetimeKind.Singleton, Position = Position)]
  public class WxeLifetimeManagementSettings : IWxeLifetimeManagementSettings
  {
    public const int Position = 0;

    private const int c_defaultFunctionTimeout = 20;

    private const bool c_defaultEnableSessionManagement = true;

    private const int c_defaultRefreshInterval = 10;

    /// <summary>
    /// Factory method to create a <see cref="WxeLifetimeManagementSettings"/> object.
    /// </summary>
    /// <param name="functionTimeout">
    ///   Specifies the <see cref="FunctionTimeout"/>. If not specified, 20 will be set as the default value.
    /// </param>
    /// <param name="enableSessionManagement">
    ///   Specifies the <see cref="EnableSessionManagement"/>. If not specified, true will be set as the default value.
    /// </param>
    /// <param name="refreshInterval">
    ///   Specifies the <see cref="RefreshInterval"/>. If not specified, 10 will be set as the default value.
    /// </param>
    /// <returns>A new <see cref="WxeUrlSettings"/> object.</returns>
    public static WxeLifetimeManagementSettings Create (int? functionTimeout = null, bool? enableSessionManagement = null, int? refreshInterval = null)
    {
      var constructedFunctionTimeout = functionTimeout ?? c_defaultFunctionTimeout;
      var constructedEnableSessionTimeout = enableSessionManagement ?? c_defaultEnableSessionManagement;
      var constructedRefreshInterval = refreshInterval ?? c_defaultRefreshInterval;

      return new WxeLifetimeManagementSettings(constructedFunctionTimeout, constructedEnableSessionTimeout, constructedRefreshInterval);
    }

    /// <summary> Gets the timeout for individual functions within one session.</summary>
    /// <value> The timeout in minutes. Defaults to 20.</value>
    public int FunctionTimeout { get; }

    /// <summary> Gets a flag that determines whether session management is employed.</summary>
    /// <value> <see langword="true"/> to enable session management. Defaults to <see langword="true"/>.</value>
    public bool EnableSessionManagement { get; }

    /// <summary> Gets the refresh interval for a function.</summary>
    /// <value> The refresh interval in minutes. Defaults to 10.</value>
    public int RefreshInterval { get; }

    public WxeLifetimeManagementSettings ()
      : this(c_defaultFunctionTimeout, c_defaultEnableSessionManagement, c_defaultRefreshInterval)
    {
    }

    private WxeLifetimeManagementSettings (int functionTimeout, bool enableSessionManagement, int refreshInterval)
    {
      FunctionTimeout = functionTimeout;
      EnableSessionManagement = enableSessionManagement;
      RefreshInterval = refreshInterval;
    }
  }
}
