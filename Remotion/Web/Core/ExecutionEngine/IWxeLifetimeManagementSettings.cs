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

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Declares relevant settings for the lifetime of the web execution engine. Resolved via the application's IoC infrastructure.
  /// </summary>
  /// <seealso cref="WxeLifetimeManagementSettings"/>
  /// <seealso cref="Remotion.Web.Configuration.ConfigurationBasedWxeLifetimeManagementSettings"/>
  /// <threadsafety static="true" instance="true" />
  public interface IWxeLifetimeManagementSettings
  {
    /// <summary> Gets the timeout for individual functions within one session.</summary>
    /// <value> The timeout in minutes.</value>
    int FunctionTimeout { get; }

    /// <summary> Gets a flag that determines whether session management is employed.</summary>
    /// <value> <see langword="true"/> to enable session management.</value>
    bool EnableSessionManagement { get; }

    /// <summary> Gets the refresh interval for a function.</summary>
    /// <value> The refresh interval in minutes.</value>
    int RefreshInterval { get; }
  }
}
