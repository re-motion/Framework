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
using System.Xml;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Queries.ConfigurationLoader;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.ConfigurationLoader
{
  [TestFixture]
  public class LoaderUtilityTest
  {
    private FakeConfigurationWrapper _configurationWrapper;

    [SetUp]
    public void SetUp ()
    {
      _configurationWrapper = new FakeConfigurationWrapper();
      _configurationWrapper.SetUpConnectionString("Rdbms", "ConnectionString", null);
      ConfigurationWrapper.SetCurrent(_configurationWrapper);
    }

    [TearDown]
    public void TearDown ()
    {
      ConfigurationWrapper.SetCurrent(null);
    }

    [Test]
    public void GetConfigurationFileName ()
    {
      _configurationWrapper.SetUpAppSetting("ConfigurationFileThatDoesNotExist", @"C:\NonExistingConfigurationFile.xml");

      Assert.That(LoaderUtility.GetConfigurationFileName("ConfigurationFileThatDoesNotExist", "Mapping.xml"), Is.EqualTo(@"C:\NonExistingConfigurationFile.xml"));
    }

    [Test]
    public void GetEmptyConfigurationFileName ()
    {
      _configurationWrapper.SetUpAppSetting("EmptyConfigurationFileName", string.Empty);

      Assert.That(LoaderUtility.GetConfigurationFileName("EmptyConfigurationFileName", "Mapping.xml"), Is.EqualTo(string.Empty));
    }

    [Test]
    public void GetConfigurationFileNameForNonExistingAppSettingsKey ()
    {
      Assert.That(
          LoaderUtility.GetConfigurationFileName("AppSettingKeyDoesNotExist", "Mapping.xml"),
          Is.EqualTo(Path.Combine(ReflectionUtility.GetConfigFileDirectory(), "Mapping.xml")));
    }

    [Test]
    public void GetTypeWithTypeUtilityNotation ()
    {
      Assert.That(LoaderUtility.GetType("Remotion.Data.DomainObjects::Queries.ConfigurationLoader.LoaderUtility"), Is.EqualTo(typeof(LoaderUtility)));
    }

    [Test]
    public void GetType_XPathNotFound ()
    {
      var namespaceManager = new XmlNamespaceManager(new NameTable());
      var node = new XmlDocument(namespaceManager.NameTable).CreateElement("documentRoot");
      var xPath = "missing/root/node";
      Assert.That(
          () => LoaderUtility.GetType(node, xPath, namespaceManager),
          Throws.Exception.TypeOf<ConfigurationException>().With.Message.EqualTo("XPath 'missing/root/node' does not exist on node 'documentRoot'."));
    }
  }
}
