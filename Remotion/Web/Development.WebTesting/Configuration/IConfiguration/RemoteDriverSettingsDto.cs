// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
namespace Remotion.Web.Development.WebTesting.Configuration.IConfiguration;

/// <summary>
/// DTO object for <see cref="IWebTestRemoteDriverSettings"/>, which is deserialized by the .NET configuration infrastructure.
/// </summary>
public class RemoteDriverSettingsDto : IWebTestRemoteDriverSettings
{
  /// <inheritdoc cref="IWebTestRemoteDriverSettings.Enabled" />
  public bool Enabled { get; init; } = false;

  /// <inheritdoc cref="IWebTestRemoteDriverSettings.Url" />
  public string Url { get; init; } = "";
}
