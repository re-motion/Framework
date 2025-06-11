// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
namespace Remotion.Web.Development.WebTesting.Configuration.IConfiguration;

public class RemotingSettingsDto : IWebTestRemotingSettings
{
  /// <inheritdoc cref="IWebTestRemotingSettings.Enabled" />
  public bool Enabled { get; init; } = false;

  /// <inheritdoc cref="IWebTestRemotingSettings.Url" />
  public string Url { get; init; } = "";
}
