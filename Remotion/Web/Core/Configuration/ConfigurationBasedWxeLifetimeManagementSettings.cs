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
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Configuration
{
  /// <summary>
  /// Implements the <see cref="IWxeLifetimeManagementSettings" /> interface based on <see cref="ExecutionEngineConfiguration"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IWxeLifetimeManagementSettings), Lifetime = LifetimeKind.Singleton, Position = Position)]
  public class ConfigurationBasedWxeLifetimeManagementSettings : IWxeLifetimeManagementSettings
  {
    public const int Position = WxeLifetimeManagementSettings.Position - 1;

    public int FunctionTimeout => Configuration.FunctionTimeout;

    public bool EnableSessionManagement => Configuration.EnableSessionManagement;

    public int RefreshInterval => Configuration.RefreshInterval;

    private ExecutionEngineConfiguration Configuration => WebConfiguration.Current.ExecutionEngine;
  }
}
