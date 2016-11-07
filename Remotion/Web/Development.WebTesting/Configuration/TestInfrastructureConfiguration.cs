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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Default implementation of <see cref="ITestInfrastructureConfiguration"/>.
  /// </summary>
  public class TestInfrastructureConfiguration : ITestInfrastructureConfiguration
  {
    private readonly string _webApplicationRoot;
    private readonly string _screenshotDirectory;
    private readonly TimeSpan _searchTimeout;
    private readonly TimeSpan _retryInterval;

    private readonly bool _closeBrowserWindowsOnSetUpAndTearDown;

    public TestInfrastructureConfiguration ([NotNull] WebTestConfigurationSection webTestConfigurationSection)
    {
      ArgumentUtility.CheckNotNull ("webTestConfigurationSection", webTestConfigurationSection);

      _webApplicationRoot = webTestConfigurationSection.WebApplicationRoot;
      _screenshotDirectory = webTestConfigurationSection.ScreenshotDirectory;
      _searchTimeout = webTestConfigurationSection.SearchTimeout;
      _retryInterval = webTestConfigurationSection.RetryInterval;

      _closeBrowserWindowsOnSetUpAndTearDown = webTestConfigurationSection.CloseBrowserWindowsOnSetUpAndTearDown;
    }

    public string WebApplicationRoot
    {
      get { return _webApplicationRoot; }
    }

    public string ScreenshotDirectory
    {
      get { return _screenshotDirectory; }
    }

    public TimeSpan SearchTimeout
    {
      get { return _searchTimeout; }
    }

    public TimeSpan RetryInterval
    {
      get { return _retryInterval; }
    }

    public bool CloseBrowserWindowsOnSetUpAndTearDown
    {
      get { return _closeBrowserWindowsOnSetUpAndTearDown; }
    }
  }
}