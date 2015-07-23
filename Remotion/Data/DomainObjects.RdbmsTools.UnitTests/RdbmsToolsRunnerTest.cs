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

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests
{
  [TestFixture]
  public class RdbmsToolsRunnerTest
  {
    [Test]
    public void CreateAppDomainSetup ()
    {
      var parameter = new RdbmsToolsParameters ();
      parameter.BaseDirectory = @"c:\foobar";
      AppDomainSetup setup = RdbmsToolsRunner.CreateAppDomainSetup (parameter);
      Assert.That (setup.ApplicationName, Is.EqualTo ("RdbmsTools"));
      Assert.That (setup.ApplicationBase, Is.EqualTo (@"c:\foobar"));
      Assert.That (setup.ConfigurationFile, Is.Null);
    }

    [Test]
    public void CreateAppDomainSetup_WithAbsoluteConfigFilePath ()
    {
      var parameter = new RdbmsToolsParameters ();
      parameter.BaseDirectory = @"c:\foobar";
      string configPath = GetConfigPath ("Test.config");
      parameter.ConfigFile = configPath;

      Assert.That (Path.IsPathRooted (configPath), configPath);
      Assert.That (File.Exists (configPath), configPath);
      
      AppDomainSetup setup = RdbmsToolsRunner.CreateAppDomainSetup (parameter);
      Assert.That (setup.ConfigurationFile, Is.EqualTo (configPath));
    }

    [Test]
    [ExpectedException (typeof (FileNotFoundException), 
        ExpectedMessage = "The configuration file supplied by the 'config' parameter was not found.", 
        MatchType = MessageMatch.Contains)]
    public void CreateAppDomainSetup_WithAbsoluteConfigFilePath_FileNotFound ()
    {
      var parameter = new RdbmsToolsParameters ();
      parameter.BaseDirectory = @"c:\foobar";
      string configPath = GetConfigPath ("Test12313.config");
      parameter.ConfigFile = configPath;

      Assert.That (Path.IsPathRooted (configPath), configPath);
      Assert.That (File.Exists (configPath), Is.False, configPath);

      RdbmsToolsRunner.CreateAppDomainSetup (parameter);
    }

    [Test]
    public void CreateAppDomainSetup_WithPathRelativeToCurrentDirectory ()
    {
      var parameter = new RdbmsToolsParameters ();
      parameter.BaseDirectory = @"c:\foobar";

      string configPath = GetConfigPath ("Test.config");
      Environment.CurrentDirectory = Path.GetDirectoryName (configPath);
      parameter.ConfigFile = Path.GetFileName (configPath);

      Assert.That (Path.IsPathRooted (configPath), configPath);
      Assert.That (Path.IsPathRooted (parameter.ConfigFile), Is.False, parameter.ConfigFile);
      Assert.That (File.Exists (configPath), configPath);
      Assert.That (File.Exists (Path.Combine (Environment.CurrentDirectory, parameter.ConfigFile)));

      AppDomainSetup setup = RdbmsToolsRunner.CreateAppDomainSetup (parameter);
      Assert.That (setup.ConfigurationFile, Is.EqualTo (configPath));
    }

    [Test]
    [ExpectedException (typeof (FileNotFoundException), 
        ExpectedMessage = "The configuration file supplied by the 'config' parameter was not found.", 
        MatchType = MessageMatch.Contains)]
    public void CreateAppDomainSetup_WithPathRelativeToCurrentDirectory_FileNotFound ()
    {
      var parameter = new RdbmsToolsParameters ();
      parameter.BaseDirectory = @"c:\foobar";
      string configPath = GetConfigPath ("Test123.config");
      Environment.CurrentDirectory = Path.GetDirectoryName (configPath);
      parameter.ConfigFile = Path.GetFileName (configPath);

      Assert.That (Path.IsPathRooted (configPath), configPath);
      Assert.That (Path.IsPathRooted (parameter.ConfigFile), Is.False, parameter.ConfigFile);
      Assert.That (File.Exists (Path.Combine (Environment.CurrentDirectory, parameter.ConfigFile)), Is.False);

      RdbmsToolsRunner.CreateAppDomainSetup (parameter);
    }

    private string GetConfigPath (string configFileName)
    {
      return Path.Combine (AppDomain.CurrentDomain.BaseDirectory, configFileName);
    }
  }
}
