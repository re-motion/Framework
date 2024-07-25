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

      var webTestSettingsStub = new Mock<IWebTestSettings>();
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.RootPath)
          .Returns(@".\Some\Path");
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.Resources)
          .Returns(new List<string>() { @".\Some\Resource" });
      webTestSettingsStub

      var testSiteLayoutConfiguration = new TestSiteLayoutConfiguration(webTestSettingsStub.Object);

      Assert.That(testSiteLayoutConfiguration.RootPath, Is.EqualTo(Path.Combine(currentBasePath, @"Some\Path")));
      Assert.That(testSiteLayoutConfiguration.Resources.Count, Is.EqualTo(1));
      Assert.That(testSiteLayoutConfiguration.Resources[0].Path, Is.EqualTo(Path.Combine(currentBasePath, @"Some\Path\Some\Resource")));
      Assert.That(testSiteLayoutConfiguration.ProcessPath, Is.EqualTo(Path.Combine(currentBasePath, @"Some\Path\BinFolder\Executable.exe")));
    }

    [Test]
    public void CreateFromWebTestConfigurationSection_WithAbsolutePaths ()
    {
      var webTestSettingsStub = new Mock<IWebTestSettings>();
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.RootPath)
          .Returns(@"C:\Some\Path");
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.Resources)
          .Returns(new List<string>() { @"C:\Some\Resource", @"Some\Other\Resource" });
      webTestSettingsStub
          .Setup(m => m.TestSiteLayout.ProcessPath)
          .Returns(@"C:\BinFolder\Executable.exe");
      var testSiteLayoutConfiguration = new TestSiteLayoutConfiguration(webTestSettingsStub.Object);

      Assert.That(testSiteLayoutConfiguration.RootPath, Is.EqualTo(@"C:\Some\Path"));
      Assert.That(testSiteLayoutConfiguration.Resources.Count, Is.EqualTo(2));
      Assert.That(testSiteLayoutConfiguration.Resources[0].Path, Is.EqualTo(@"C:\Some\Resource"));
      Assert.That(testSiteLayoutConfiguration.Resources[1].Path, Is.EqualTo(@"C:\Some\Path\Some\Other\Resource"));
      Assert.That(testSiteLayoutConfiguration.ProcessPath, Is.EqualTo(@"C:\BinFolder\Executable.exe"));
    }
  }
}
