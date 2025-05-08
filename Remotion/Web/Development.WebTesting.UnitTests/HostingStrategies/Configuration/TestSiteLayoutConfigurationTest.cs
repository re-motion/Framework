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
using System.IO;
using Moq;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies.Configuration;

namespace Remotion.Web.Development.WebTesting.UnitTests.HostingStrategies.Configuration
{
  [TestFixture]
  public class TestSiteLayoutConfigurationTest
  {
    [Test]
    public void CreateFromWebTestConfigurationSection_WithRelativePaths ()
    {
      var currentBasePath = AppContext.BaseDirectory;

      var relativeRootPath = OperatingSystem.IsWindows() ? @".\Some\Path" : "./Some/Path";
      var relativeResourcePath = OperatingSystem.IsWindows() ? @".\Some\Resource" : "./Some/Resource";
      var binaryPath = OperatingSystem.IsWindows() ? @".\BinFolder\Executable.exe" : "./BinFolder/Executable.exe";

      var webTestSettingsStub = new Mock<IWebTestSettings>();
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.RootPath)
          .Returns(relativeRootPath);
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.Resources)
          .Returns(new List<string>() { relativeResourcePath });
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.ProcessPath)
          .Returns(binaryPath);

      var testSiteLayoutConfiguration = new TestSiteLayoutConfiguration(webTestSettingsStub.Object);

      Assert.That(testSiteLayoutConfiguration.RootPath, Is.EqualTo(Path.Combine(currentBasePath, "Some", "Path")));
      Assert.That(testSiteLayoutConfiguration.Resources.Count, Is.EqualTo(1));
      Assert.That(testSiteLayoutConfiguration.Resources[0].Path, Is.EqualTo(Path.Combine(currentBasePath, "Some", "Path", "Some", "Resource")));
      Assert.That(testSiteLayoutConfiguration.ProcessPath, Is.EqualTo(Path.Combine(currentBasePath, "Some", "Path", "BinFolder", "Executable.exe")));
    }

    [Test]
    public void CreateFromWebTestConfigurationSection_WithAbsolutePaths ()
    {
      var somePath = OperatingSystem.IsWindows() ? @"C:\Some\" : "/var/some/";
      var someRelativePath = OperatingSystem.IsWindows() ? @"Some\Other\" : "some/other/";
      var binPath = OperatingSystem.IsWindows() ? @"C:\BinFolder" : "/bin/";

      var rootPath = Path.Combine(somePath, "Path");
      var resourcePathAbsolute = Path.Combine(somePath, "Resource");
      var resourcePathRelative = Path.Combine(someRelativePath, "Resource");
      var processPath = Path.Combine(binPath, "Executable.exe");

      var webTestSettingsStub = new Mock<IWebTestSettings>();
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.RootPath)
          .Returns(rootPath);
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.Resources)
          .Returns(new List<string>() { resourcePathAbsolute, resourcePathRelative });
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.ProcessPath)
          .Returns(processPath);
      var testSiteLayoutConfiguration = new TestSiteLayoutConfiguration(webTestSettingsStub.Object);

      Assert.That(testSiteLayoutConfiguration.RootPath, Is.EqualTo(rootPath));
      Assert.That(testSiteLayoutConfiguration.Resources.Count, Is.EqualTo(2));
      Assert.That(testSiteLayoutConfiguration.Resources[0].Path, Is.EqualTo(resourcePathAbsolute));
      Assert.That(testSiteLayoutConfiguration.Resources[1].Path, Is.EqualTo(Path.Combine(rootPath, resourcePathRelative)));
      Assert.That(testSiteLayoutConfiguration.ProcessPath, Is.EqualTo(processPath));
    }
  }
}
