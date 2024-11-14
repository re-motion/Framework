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
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Configuration.IConfiguration
{
  /// <summary>
  /// DTO object for <see cref="IWebTestSettings"/>, which is deserialized by the .NET configuration infrastructure.
  /// </summary>
  public record WebTestSettingsDto : IWebTestSettings
  {
    private ILoggerFactory? _loggerFactory;

    /// <inheritdoc />
    [Required]
    [RegularExpression("(Chrome|Edge|Firefox)")]
    public required string BrowserName { get; init; }

    /// <inheritdoc />
    [Required]
    public required TimeSpan SearchTimeout { get; init; }

    /// <inheritdoc />
    public TimeSpan CommandTimeout { get; init; } = TimeSpan.FromMinutes(1);

    /// <inheritdoc />
    public TimeSpan DownloadStartedTimeout { get; init; } = TimeSpan.FromSeconds(10);

    /// <inheritdoc />
    public TimeSpan DownloadUpdatedTimeout { get; init; } = TimeSpan.FromSeconds(10);

    /// <inheritdoc />
    public TimeSpan VerifyWebApplicationStartedTimeout { get; init; } = TimeSpan.FromMinutes(1);

    /// <inheritdoc />
    [Required]
    public required TimeSpan RetryInterval { get; init; }

    /// <inheritdoc />
    public TimeSpan AsyncJavaScriptTimeout { get; init; } = TimeSpan.FromSeconds(10);

    /// <inheritdoc />
    public bool Headless { get; init; } = false;

    /// <inheritdoc />
    [Required]
    [MinLength(1)]
    public required string WebApplicationRoot { get; init; }

    /// <inheritdoc />
    public string ScreenshotDirectory { get; init; } = "";

    /// <inheritdoc />
    public string LogsDirectory { get; init; } = ".";

    /// <inheritdoc />
    public bool CloseBrowserWindowsOnSetUpAndTearDown { get; init; } = false;

    /// <inheritdoc />
    public bool CleanUpUnmatchedDownloadedFiles { get; init; } = false;

    public string RequestErrorDetectionStrategy { get; init; } = "None";

    /// <inheritdoc />
    string IWebTestSettings.RequestErrorDetectionStrategyTypeName => RequestErrorDetectionStrategy;

    public WebTestChromiumSettingsDto Chrome { get; init; } = new();

    /// <inheritdoc />
    IWebTestChromiumSettings IWebTestSettings.Chrome => Chrome;

    public WebTestChromiumSettingsDto Edge { get; init; } = new();

    /// <inheritdoc />
    IWebTestChromiumSettings IWebTestSettings.Edge => Edge;

    [Required]
    public required WebTestHostingSettingsDto Hosting { get; init; }

    /// <inheritdoc />
    IWebTestHostingSettings IWebTestSettings.Hosting => Hosting;

    [Required]
    public required WebTestTestSiteLayoutDto TestSiteLayout { get; init; }

    /// <inheritdoc />
    IWebTestTestSiteLayoutSettings IWebTestSettings.TestSiteLayout => TestSiteLayout;

    ILoggerFactory IWebTestSettings.LoggerFactory =>
        _loggerFactory ?? throw new InvalidOperationException("The LoggerFactory was not initialized via WebTestSettingsDto.SetLoggerFactory(...).");

    /// <summary>
    /// Sets the <see cref="ILoggerFactory"/> used by the web test infrastructure.
    /// </summary>
    public void SetLoggerFactory (ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      _loggerFactory = loggerFactory;
    }
  }
}
