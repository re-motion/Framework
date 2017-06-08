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
using System;
using JetBrains.Annotations;
using log4net.Config;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.HostingStrategies;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Helper class for web test set up fixtures. Initializes log4net and hosts the configured web application under test.
  /// </summary>
  /// <remarks>
  /// Use the factory methods <see cref="CreateFromConfiguration"/> or <see cref="CreateFromConfiguration{TFactory}"/> 
  /// for instantiating an instance of type <see cref="WebTestSetUpFixtureHelper"/>.
  /// </remarks>
  public class WebTestSetUpFixtureHelper
  {
    /// <summary>
    /// Creates a new <see cref="WebTestSetUpFixtureHelper"/> with configuration based on <see cref="WebTestConfigurationFactory"/>.
    /// </summary>
    [PublicAPI]
    public static WebTestSetUpFixtureHelper CreateFromConfiguration ()
    {
      return new WebTestSetUpFixtureHelper(new WebTestConfigurationFactory());
    }

    /// <summary>
    /// Creates a new <see cref="WebTestSetUpFixtureHelper"/> with configuration based on <typeparamref name="TFactory"/>.
    /// </summary>
    /// <remarks>
    /// Use this overload when you have to provide test-project specific configuration settings (e.g. custom <see cref="IHostingStrategy"/>) 
    /// via custom <see cref="WebTestConfigurationFactory"/>.
    /// </remarks>
    [PublicAPI]
    public static WebTestSetUpFixtureHelper CreateFromConfiguration<TFactory> () where TFactory : WebTestConfigurationFactory, new()
    {
      return new WebTestSetUpFixtureHelper (new TFactory());
    }

    private readonly IHostingStrategy _hostingStrategy;

    [PublicAPI]
    protected WebTestSetUpFixtureHelper ([NotNull] WebTestConfigurationFactory webTestConfigurationFactory)
    {
      ArgumentUtility.CheckNotNull ("webTestConfigurationFactory", webTestConfigurationFactory);

      var configuration = webTestConfigurationFactory.CreateHostingConfiguration();

      _hostingStrategy = configuration.GetHostingStrategy();
    }

    /// <summary>
    /// One-time SetUp method for all web tests.
    /// </summary>
    public void OnSetUp ()
    {
      SetUpLog4net();
      HostWebApplication();
    }

    /// <summary>
    /// One-time TearDown method for all web tests.
    /// </summary>
    public void OnTearDown ()
    {
      UnhostWebApplication();
    }

    private void SetUpLog4net ()
    {
      XmlConfigurator.Configure();
    }

    private void HostWebApplication ()
    {
      _hostingStrategy.DeployAndStartWebApplication();
    }

    private void UnhostWebApplication ()
    {
      _hostingStrategy.StopAndUndeployWebApplication();
    }
  }
}