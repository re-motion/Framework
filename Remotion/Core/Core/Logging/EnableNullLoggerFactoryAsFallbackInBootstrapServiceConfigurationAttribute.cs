// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Microsoft.Extensions.Logging.Abstractions;
using Remotion.ServiceLocation;

namespace Remotion.Logging;

/// <summary>
/// Apply this attribute to your application's main assembly (i.e. the <c>Program</c> or the testing assembly) use implicityly use the <see cref="NullLoggerFactory"/>
/// as your application's default logging provider.
/// </summary>
/// <remarks>
/// Note that when the <see cref="SafeServiceLocator"/> discovers the <see cref="EnableNullLoggerFactoryAsFallbackInBootstrapServiceConfigurationAttribute"/> within its callstack,
/// it will no longer require the use of <see cref="BootstrapServiceConfiguration"/>.<see cref="BootstrapServiceConfiguration.SetLoggerFactory"/> during application startup.
/// Instead, the <see cref="NullLoggerFactory"/> will be used, effectivly disabling logging in the application.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class EnableNullLoggerFactoryAsFallbackInBootstrapServiceConfigurationAttribute : Attribute
{
  public EnableNullLoggerFactoryAsFallbackInBootstrapServiceConfigurationAttribute ()
  {
  }
}
