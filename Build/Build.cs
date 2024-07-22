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
using System.Collections.Immutable;
using System.Linq;
using Customizations;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Remotion.BuildScript;
using Remotion.BuildScript.Components;
using Remotion.BuildScript.Test;
using Remotion.BuildScript.Test.Dimensions;
using static Customizations.Browsers;
using static Customizations.Databases;
using static Remotion.BuildScript.Test.Dimensions.Configurations;
using static Remotion.BuildScript.Test.Dimensions.ExecutionRuntimes;
using static Customizations.EnforcedLocalMachineExecutionRuntimes;
using static Remotion.BuildScript.Test.Dimensions.Platforms;
using static Remotion.BuildScript.Test.Dimensions.TargetFrameworks;

// ReSharper disable RedundantTypeArgumentsOfMethod

class Build : RemotionBuild
{
  [Parameter(ValueProviderMember = nameof(SupportedTestBrowsers), Separator = "+")]
  public string[] TestBrowsers { get; set; } = [];

  [Parameter(ValueProviderMember = nameof(SupportedTestSqlServers), Separator = "+")]
  public string[] TestSqlServers { get; set; } = [];


  public static int Main () => Execute<Build>();

  [UsedImplicitly]
  public Target AddRemotionPackagingArtefacts => _ => _
      .TriggeredBy<IPack>()
      .Executes(() =>
      {
        // Copy GlobalFacetManifest.xml to output folder
        var globalFacetManifestPath = BuildProjectDirectory / "GlobalFacetManifest.xml";
        Assert.FileExists(globalFacetManifestPath);

        FileSystemTasks.CopyFile(globalFacetManifestPath, ((IBaseBuild)this).OutputFolder / globalFacetManifestPath.Name);

        // Copy GlobalFacetManifest.NetFramework.xml to output folder
        var globalFacetManifestNetFrameworkPath = BuildProjectDirectory / "GlobalFacetManifest.NetFramework.xml";
        Assert.FileExists(globalFacetManifestNetFrameworkPath);

        FileSystemTasks.CopyFile(globalFacetManifestNetFrameworkPath, ((IBaseBuild)this).OutputFolder / globalFacetManifestNetFrameworkPath.Name);

        // Create NPM package
        var packageJsonPath = ((IBaseBuild)this).Solution.Directory / "Remotion" / "Web" / "Dependencies.JavaScript" / "package.json";
        Assert.FileExists(packageJsonPath);

        var outputFolder = ((IBaseBuild)this).OutputFolder / "Npm" / "remotion.dependencies" / "package.json";
        var packageJsonContent = packageJsonPath.ReadAllText()
                .Replace("$version$", ((IBuildMetadata)this).BuildMetadataPerConfiguration.First().Value.Version);
        outputFolder.WriteAllText(packageJsonContent);
      });

  public override void ConfigureProjects (ProjectsBuilder projects)
  {
    var normalTestConfiguration = new TestConfiguration(
        DefaultTestExecutionRuntimeFactory.Instance,
        TestMatrices.Single(e => e.Name == "NormalTestMatrix"),
        ImmutableArray<ITestExecutionWrapper>.Empty);

    var webTestingTestConfiguration = new TestConfiguration(
        DefaultTestExecutionRuntimeFactory.Instance,
        TestMatrices.Single(e => e.Name == "WebTestingTestMatrix"),
        [new WebTestingTestSetup()]);

    var databaseTestConfiguration = new TestConfiguration(
        DefaultTestExecutionRuntimeFactory.Instance,
        TestMatrices.Single(e => e.Name == "DatabaseTestMatrix"),
        [new DatabaseTestSetup()]);

    projects.AddUnitTestProject("SharedSource.UnitTests", normalTestConfiguration);
    projects.AddReleaseProject("Core.Development.Analyzers");
    projects.AddReleaseProject("Core.Core");
    projects.AddReleaseProject("Core.Documentation");
    projects.AddReleaseProject("Core.Collections.Caching");
    projects.AddReleaseProject("Core.Collections.DataStore");
    projects.AddReleaseProject("Core.ExtensibleEnums");
    projects.AddReleaseProject("Core.Extensions");
    projects.AddReleaseProject("Core.Reflection");
    projects.AddReleaseProject("Core.Reflection.CodeGeneration");
    projects.AddReleaseProject("Core.Reflection.CodeGeneration.TypePipe");
    projects.AddReleaseProject("Core.Tools");
    projects.AddReleaseProject("Core.Xml");
    projects.AddUnitTestProject("Core.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Core.Collections.Caching.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Core.Collections.DataStore.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Core.ExtensibleEnums.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Core.Extensions.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Core.Reflection.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Core.Reflection.CodeGeneration.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Core.Reflection.CodeGeneration.TypePipe.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Core.Tools.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Core.Xml.UnitTests", normalTestConfiguration);
    projects.AddReleaseProject("Development.Core");
    projects.AddReleaseProject("Development.Data");
    projects.AddReleaseProject("Development.Mixins");
    projects.AddReleaseProject("Development.Web");
    projects.AddReleaseProject("Development.Sandboxing.NUnit2");
    projects.AddReleaseProject("Development.UnitTesting.Compilation");
    projects.AddReleaseProject("Development.UnitTesting.IsolatedCodeRunner");
    projects.AddUnitTestProject("Development.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Development.Moq.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Development.Sandboxing.NUnit2.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Development.UnitTesting.IsolatedCodeRunner.UnitTests", normalTestConfiguration);
    projects.AddReleaseProject("Mixins.Core");
    projects.AddReleaseProject("Mixins.CrossReferencer");
    projects.AddReleaseProject("Mixins.MixerTools");
    projects.AddUnitTestProject("Mixins.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Mixins.CrossReferencer.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Mixins.Samples", normalTestConfiguration);
    projects.AddUnitTestProject("Mixins.MixerTools.UnitTests", normalTestConfiguration);
    projects.AddReleaseProject("Globalization.Core");
    projects.AddReleaseProject("Globalization.ExtensibleEnums");
    projects.AddReleaseProject("Globalization.Mixins");
    projects.AddUnitTestProject("Globalization.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Globalization.ExtensibleEnums.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Globalization.Mixins.UnitTests", normalTestConfiguration);
    projects.AddReleaseProject("Validation.Core");
    projects.AddReleaseProject("Validation.Globalization");
    projects.AddReleaseProject("Validation.Mixins");
    projects.AddUnitTestProject("Validation.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Validation.IntegrationTests", normalTestConfiguration);
    projects.AddUnitTestProject("Validation.Globalization.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Validation.Mixins.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Validation.Mixins.IntegrationTests", normalTestConfiguration);
    projects.AddReleaseProject("Data.Core");
    projects.AddReleaseProject("Data.DomainObjects");
    projects.AddReleaseProject("Data.DomainObjects.ObjectBinding");
    projects.AddReleaseProject("Data.DomainObjects.RdbmsTools");
    projects.AddReleaseProject("Data.DomainObjects.Security");
    projects.AddReleaseProject("Data.DomainObjects.UberProfIntegration");
    projects.AddReleaseProject("Data.DomainObjects.Validation");
    projects.AddUnitTestProject("Data.DomainObjects.UnitTests", databaseTestConfiguration);
    projects.AddUnitTestProject("Data.DomainObjects.ObjectBinding.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Data.DomainObjects.ObjectBinding.IntegrationTests", databaseTestConfiguration);
    projects.AddUnitTestProject("Data.DomainObjects.RdbmsTools.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Data.DomainObjects.Security.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Data.DomainObjects.UberProfIntegration.UnitTests", databaseTestConfiguration);
    projects.AddUnitTestProject("Data.DomainObjects.Web.IntegrationTests", databaseTestConfiguration);
    projects.AddUnitTestProject("Data.DomainObjects.Validation.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Data.DomainObjects.Validation.IntegrationTests", normalTestConfiguration);
    projects.AddReleaseProject("ObjectBinding.Core");
    projects.AddReleaseProject("ObjectBinding.Security");
    projects.AddReleaseProject("ObjectBinding.Validation");
    projects.AddReleaseProject("ObjectBinding.Web");
    projects.AddReleaseProject("ObjectBinding.Web.ClientScript");
    projects.AddReleaseProject("ObjectBinding.Web.Compatibility");
    projects.AddReleaseProject("ObjectBinding.Web.Contracts.DiagnosticMetadata");
    projects.AddReleaseProject("ObjectBinding.Web.Development.WebTesting");
    projects.AddReleaseProject("ObjectBinding.Web.Preview");
    projects.AddReleaseProject("ObjectBinding.Web.Validation");
    projects.AddUnitTestProject("ObjectBinding.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("ObjectBinding.Security.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("ObjectBinding.Validation.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("ObjectBinding.Web.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("ObjectBinding.Web.Compatibility.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("ObjectBinding.Web.Validation.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("ObjectBinding.Web.Preview.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("ObjectBinding.Web.Development.WebTesting.IntegrationTests", webTestingTestConfiguration);
    projects.AddUnitTestProject("ObjectBinding.Web.IntegrationTests", webTestingTestConfiguration);
    projects.AddReleaseProject("Security.Core");
    projects.AddReleaseProject("Security.Metadata.Extractor");
    projects.AddReleaseProject("Security.MSBuild.Tasks");
    projects.AddUnitTestProject("Security.UnitTests", normalTestConfiguration);
    projects.AddReleaseProject("Web.Contracts.DiagnosticMetadata");
    projects.AddReleaseProject("Web.Core");
    projects.AddReleaseProject("Web.ClientScript");
    projects.AddReleaseProject("Web.Development.Analyzers");
    projects.AddUnitTestProject("Web.Development.Analyzers.IntegrationTests", normalTestConfiguration);
    projects.AddReleaseProject("Web.Development.WebTesting");
    projects.AddReleaseProject("Web.Development.WebTesting.ControlObjects");
    projects.AddReleaseProject("Web.Development.WebTesting.ExecutionEngine");
    projects.AddReleaseProject("Web.Development.WebTesting.WebFormsControlObjects");
    projects.AddReleaseProject("Web.Security");
    projects.AddUnitTestProject("Web.Development.WebTesting.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Web.UnitTests", normalTestConfiguration);
    projects.AddUnitTestProject("Web.Development.WebTesting.IntegrationTests", webTestingTestConfiguration);
    projects.AddUnitTestProject("Web.Development.WebTesting.IntegrationTests.RequireUI", webTestingTestConfiguration);
    projects.AddUnitTestProject("Web.IntegrationTests", webTestingTestConfiguration);
    projects.AddReleaseProject("Integration.Domain");
    projects.AddReleaseProject("Integration.Web");
    projects.AddReleaseProject("SecurityManager.AclTools.Expander");
    projects.AddReleaseProject("SecurityManager.Clients.Web");
    projects.AddUnitTestProject("SecurityManager.Clients.Web.UnitTests", normalTestConfiguration);
    projects.AddReleaseProject("SecurityManager.Core");
    projects.AddReleaseProject("SecurityManager.Metadata.Importer");
    projects.AddUnitTestProject("SecurityManager.Core.UnitTests", databaseTestConfiguration);
  }

  public override void ConfigureSupportedTestDimensions (SupportedTestDimensionsBuilder supportedTestDimensions)
  {
    supportedTestDimensions.AddSupportedDimension<ExecutionRuntimes>(
        LocalMachine, EnforcedLocalMachine(Docker_Win_NET6_0),
        Docker_Win_NET48, Docker_Win_NET481,
        Docker_Win_NET8_0);
    supportedTestDimensions.AddSupportedDimension<TargetFrameworks>(NET48, NET481, NET8_0);
    supportedTestDimensions.AddSupportedDimension<Configurations>(Debug, Release);
    supportedTestDimensions.AddSupportedDimension<Platforms>(x64, x86);

    supportedTestDimensions.AddSupportedDimension<Browsers>(
        NoBrowser,
        Chrome, Firefox, Edge);
    supportedTestDimensions.AddSupportedDimension<Databases>(
        NoDB, SqlServerDefault,
        SqlServer2016, SqlServer2017, SqlServer2019, SqlServer2022);
  }

  public override void ConfigureEnabledTestDimensions (EnabledTestDimensionsBuilder enabledTestDimensions)
  {
    base.ConfigureEnabledTestDimensions(enabledTestDimensions);

    if (SupportedTestDimensions.IsSupported<Browsers>())
    {
      var testBrowsers = SupportedTestDimensions.ParseTestDimensionValuesOrDefault<Browsers>(TestBrowsers)
                         ?? throw CreateConfigurationException<Browsers>();

      enabledTestDimensions.AddEnabledDimension(testBrowsers);
    }

    if (SupportedTestDimensions.IsSupported<Databases>())
    {
      var testSqlServers = SupportedTestDimensions.ParseTestDimensionValuesOrDefault<Databases>(TestSqlServers)
                           ?? throw CreateConfigurationException<Databases>();

      enabledTestDimensions.AddEnabledDimension(testSqlServers);
    }

    return;

    static InvalidOperationException CreateConfigurationException<T> ()
        where T : TestDimension
    {
      return new InvalidOperationException($"The configuration for test dimension '{typeof(T).Name}' cannot be empty.");
    }
  }

  public override void ConfigureTestMatrix (TestMatricesBuilder builder)
  {
    builder.AddTestMatrix(
        "WebTestingTestMatrix",
        new TestDimension[,] // todo docker images need to be wired to the config file
        {
            { Chrome, NET48, Debug, x86, NoDB, EnforcedLocalMachine(Docker_Win_NET48) },
            { Chrome, NET481, Release, x86, NoDB, EnforcedLocalMachine(Docker_Win_NET481) },
            { Firefox, NET48, Release, x64, NoDB, EnforcedLocalMachine(Docker_Win_NET481) },
            { Edge, NET48, Release, x64, NoDB, EnforcedLocalMachine(Docker_Win_NET48) },
            // { Edge, NET50, Release, x64, NoDB, Docker_Win_NET50, EnforcedLocalMachine },
        },
        allowEmpty: true);

    builder.AddTestMatrix(
        "DatabaseTestMatrix",
        new TestDimension[,]
        {
            { Docker_Win_NET48, NET48, NoBrowser, SqlServer2016, Debug, x86 },
            { Docker_Win_NET48, NET48, NoBrowser, SqlServer2016, Release, x86 },
            { Docker_Win_NET48, NET48, NoBrowser, SqlServer2016, Debug, x64 },
            { Docker_Win_NET48, NET48, NoBrowser, SqlServer2016, Release, x64 },

            // Local-->
            { LocalMachine, NET48, NoBrowser, SqlServerDefault, Debug, x86 },

            // Exercise compatibility between installed .NET version, target framework and SQL Server
            { Docker_Win_NET481, NET481, NoBrowser, SqlServer2022, Release, x64 },
            { Docker_Win_NET481, NET48, NoBrowser, SqlServer2019, Release, x64 },
            { Docker_Win_NET48, NET48, NoBrowser, SqlServer2017, Release, x64 },
            // { Docker_Win_NET48 , NET48, NoBrowser, SqlServer2016, Release, x64 },
            // { Docker_Win_NET50 , NET50, NoBrowser, SqlServer2022, Release, x64 },
            // { Docker_Win_NET50 , NET50, NoBrowser, SqlServer2019, Release, x64 },
            // { Docker_Win_NET50 , NET50, NoBrowser, SqlServer2017, Release, x64 },
            // { Docker_Win_NET50 , NET50, NoBrowser, SqlServer2016, Release, x64 },
        },
        allowEmpty: true);

    builder.AddTestMatrix(
        "NormalTestMatrix",
        new TestDimension[,]
        {
            { Docker_Win_NET48, NET48, NoBrowser, NoDB, Debug, x86 },
            { Docker_Win_NET48, NET48, NoBrowser, NoDB, Release, x86 },
            { Docker_Win_NET48, NET48, NoBrowser, NoDB, Debug, x64 },
            { Docker_Win_NET48, NET48, NoBrowser, NoDB, Release, x64 },
            { Docker_Win_NET481, NET481, NoBrowser, NoDB, Debug, x64 },
            { Docker_Win_NET481, NET481, NoBrowser, NoDB, Release, x64 },
            { Docker_Win_NET481, NET48, NoBrowser, NoDB, Release, x64 },

            //  Local-->
            { LocalMachine, NET48, NoBrowser, SqlServerDefault, Debug, x86 },

            //  Exercise compatibility between installed .NET version, target framework and SQL Server
            //  { Docker_Win_NET50, NET50, NoBrowser, NoDB, Debug, x64 },
            //  { Docker_Win_NET50, NET50, NoBrowser, NoDB, Release, x64 },
        },
        allowEmpty: true);
  }

  protected IEnumerable<string> SupportedTestBrowsers => GetTestDimensionValueList<Browsers>();

  protected IEnumerable<string> SupportedTestSqlServers => GetTestDimensionValueList<Databases>();
}
