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
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.RequestErrorDetectionStrategies;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Default implementation of <see cref="ITestInfrastructureConfiguration"/>.
  /// </summary>
  public class TestInfrastructureConfiguration : ITestInfrastructureConfiguration
  {
    private static readonly Dictionary<string, Type> s_wellKnownRequestErrorDetectionStrategyTypes =
        new Dictionary<string, Type>
        {
            { "AspNet", typeof(AspNetRequestErrorDetectionStrategy) },
            { "None", typeof(NullRequestErrorDetectionStrategy) }
        };

    private readonly string _webApplicationRoot;
    private readonly string _screenshotDirectory;
    private readonly IRequestErrorDetectionStrategy _requestErrorDetectionStrategy;
    private readonly bool _closeBrowserWindowsOnSetUpAndTearDown;

    public TestInfrastructureConfiguration ([NotNull] IWebTestConfiguration webTestConfigurationSection)
    {
      ArgumentUtility.CheckNotNull("webTestConfigurationSection", webTestConfigurationSection);

      _webApplicationRoot = webTestConfigurationSection.WebApplicationRoot;
      _screenshotDirectory = webTestConfigurationSection.ScreenshotDirectory;

      _closeBrowserWindowsOnSetUpAndTearDown = webTestConfigurationSection.CloseBrowserWindowsOnSetUpAndTearDown;
      _requestErrorDetectionStrategy = GetRequestErrorDetectionConfiguration(webTestConfigurationSection.RequestErrorDetectionStrategy);
    }

    public string WebApplicationRoot
    {
      get { return _webApplicationRoot; }
    }

    public string ScreenshotDirectory
    {
      get { return _screenshotDirectory; }
    }

    public bool CloseBrowserWindowsOnSetUpAndTearDown
    {
      get { return _closeBrowserWindowsOnSetUpAndTearDown; }
    }

    public IRequestErrorDetectionStrategy RequestErrorDetectionStrategy
    {
      get { return _requestErrorDetectionStrategy; }
    }

    private IRequestErrorDetectionStrategy GetRequestErrorDetectionConfiguration (string requestErrorDetectionStrategyName)
    {
      var requestErrorStrategyType = GetRequestErrorDetectionStrategyType(requestErrorDetectionStrategyName);
      Assertion.IsNotNull(
          requestErrorStrategyType,
          string.Format("Request Error Detection strategy '{0}' could not be loaded.", requestErrorDetectionStrategyName));

      return (IRequestErrorDetectionStrategy)Activator.CreateInstance(requestErrorStrategyType)!;
    }

    [CanBeNull]
    private Type? GetRequestErrorDetectionStrategyType (string requestErrorDetectionStrategyTypeName)
    {
      if (s_wellKnownRequestErrorDetectionStrategyTypes.ContainsKey(requestErrorDetectionStrategyTypeName))
        return s_wellKnownRequestErrorDetectionStrategyTypes [requestErrorDetectionStrategyTypeName];

      return Type.GetType(requestErrorDetectionStrategyTypeName, throwOnError: false, ignoreCase: false);
    }
  }
}
