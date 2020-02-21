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
using System.IO;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies.Configuration;

namespace Remotion.Web.Development.WebTesting.UnitTests.HostingStrategies.Configuration
{
  [TestFixture]
  public class TestSiteLayoutConfigurationTest
  {
    private const string c_configurationXmlWithRelativePaths = @"
<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0""
    browser=""Chrome""
    searchTimeout=""00:00:43""
    retryInterval=""00:00:00.042""
    webApplicationRoot=""http://some.url:1337/"">
  <hosting name=""IisExpress"" type=""IisExpress"" port=""60042"" />
  <testSiteLayout rootPath="".\Some\Path"">
    <resources>
      <add path="".\Some\Resource"" />
    </resources>
  </testSiteLayout>
</remotion.webTesting>";

    private const string c_configurationXmlWithAbsolutePaths = @"
<remotion.webTesting xmlns=""http://www.re-motion.org/WebTesting/Configuration/2.0""
    browser=""Chrome""
    searchTimeout=""00:00:43""
    retryInterval=""00:00:00.042""
    webApplicationRoot=""http://some.url:1337/"">
  <hosting name=""IisExpress"" type=""IisExpress"" port=""60042"" />
  <testSiteLayout rootPath=""C:\Some\Path"">
    <resources>
      <add path=""C:\Some\Resource"" />
      <add path=""Some\Other\Resource"" />
    </resources>
  </testSiteLayout>
</remotion.webTesting>";

    [Test]
    public void CreateFromWebTestConfigurationSection_WithRelativePaths ()
    {
      var currentBasePath = AppDomain.CurrentDomain.BaseDirectory;
      var configurationSection = (WebTestConfigurationSection) Activator.CreateInstance (typeof (WebTestConfigurationSection), true);
      ConfigurationHelper.DeserializeSection (configurationSection, c_configurationXmlWithRelativePaths);

      var testSiteLayoutConfiguration = new TestSiteLayoutConfiguration (configurationSection);

      Assert.That (testSiteLayoutConfiguration.RootPath, Is.EqualTo (Path.Combine (currentBasePath, @"Some\Path")));
      Assert.That (testSiteLayoutConfiguration.Resources.Count, Is.EqualTo (1));
      Assert.That (testSiteLayoutConfiguration.Resources[0].Path, Is.EqualTo (Path.Combine (currentBasePath, @"Some\Path\Some\Resource")));
    }

    [Test]
    public void CreateFromWebTestConfigurationSection_WithAbsolutePaths ()
    {
      var configurationSection = (WebTestConfigurationSection) Activator.CreateInstance (typeof (WebTestConfigurationSection), true);
      ConfigurationHelper.DeserializeSection (configurationSection, c_configurationXmlWithAbsolutePaths);

      var testSiteLayoutConfiguration = new TestSiteLayoutConfiguration (configurationSection);

      Assert.That (testSiteLayoutConfiguration.RootPath, Is.EqualTo (@"C:\Some\Path"));
      Assert.That (testSiteLayoutConfiguration.Resources.Count, Is.EqualTo (2));
      Assert.That (testSiteLayoutConfiguration.Resources[0].Path, Is.EqualTo (@"C:\Some\Resource"));
      Assert.That (testSiteLayoutConfiguration.Resources[1].Path, Is.EqualTo (@"C:\Some\Path\Some\Other\Resource"));
    }
  }
}