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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Reflection.CodeGeneration.TypePipe.Configuration;

namespace Remotion.Reflection.CodeGeneration.TypePipe.UnitTests.Configuration
{
  [TestFixture]
  public class AppConfigBasedSettingsProviderTest
  {
    private AppConfigBasedSettingsProvider _provider;
    private TypePipeConfigurationSection _section;

    [SetUp]
    public void SetUp ()
    {
      _section = new TypePipeConfigurationSection();
      _provider = new AppConfigBasedSettingsProvider();
      PrivateInvoke.SetNonPublicField (_provider, "_section", _section);
    }

    [Test]
    public void ForceStrongNaming_False ()
    {
      var xmlFragment = "<typePipe/>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      Assert.That (_provider.ForceStrongNaming, Is.False);
    }

    [Test]
    public void ForceStrongNaming_True ()
    {
      var xmlFragment = "<typePipe><forceStrongNaming/></typePipe>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      Assert.That (_provider.ForceStrongNaming, Is.True);
      Assert.That (_provider.KeyFilePath, Is.Empty);
    }

    [Test]
    public void KeyFilePath ()
    {
      var xmlFragment = @"<typePipe><forceStrongNaming keyFilePath=""C:\key.snk""/></typePipe>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      Assert.That (_provider.KeyFilePath, Is.EqualTo (@"C:\key.snk"));
    }

    [Test]
    public void EnableComplexSerialization_False ()
    {
      var xmlFragment = "<typePipe/>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      Assert.That (_provider.EnableSerializationWithoutAssemblySaving, Is.False);
    }

    [Test]
    public void EnableComplexSerialization_True ()
    {
      var xmlFragment = "<typePipe><enableSerializationWithoutAssemblySaving/></typePipe>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      Assert.That (_provider.EnableSerializationWithoutAssemblySaving, Is.True);
    }

    [Test]
    public void GetSettings_Defaults ()
    {
      var xmlFragment = "<typePipe/>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      var settings = _provider.GetSettings();

      Assert.That (settings.ForceStrongNaming, Is.False);
      Assert.That (settings.KeyFilePath, Is.Empty);
      Assert.That (settings.EnableSerializationWithoutAssemblySaving, Is.False);
    }

    [Test]
    public void GetSettings_CustomValues ()
    {
      var xmlFragment = @"<typePipe><forceStrongNaming keyFilePath=""C:\key.snk""/><enableSerializationWithoutAssemblySaving/></typePipe>";
      ConfigurationHelper.DeserializeSection (_section, xmlFragment);

      var settings = _provider.GetSettings();

      Assert.That (settings.ForceStrongNaming, Is.True);
      Assert.That (settings.KeyFilePath, Is.EqualTo (@"C:\key.snk"));
      Assert.That (settings.EnableSerializationWithoutAssemblySaving, Is.True);
    }
  }
}