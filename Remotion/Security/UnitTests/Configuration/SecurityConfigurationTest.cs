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
using Remotion.Security.Configuration;

namespace Remotion.Security.UnitTests.Configuration
{
  [TestFixture]
  public class SecurityConfigurationTest
  {
    [Test]
    public void GetSecurityConfigurationWithoutConfigurationSection ()
    {
      ResetCurrentSecurityConfiguration();
      try
      {
        var configuration = SecurityConfiguration.Current;

        Assert.That (configuration, Is.Not.Null);
        Assert.That (configuration.DisableAccessChecks, Is.False);
      }
      finally
      {
        ResetCurrentSecurityConfiguration();
      }
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithNamespace ()
    {
      string xmlFragment = @"<remotion.security xmlns=""http://www.re-motion.org/Security/Configuration/3.0"" />";
      var configuration = new SecurityConfiguration();
      Assert.That (() => ConfigurationHelper.DeserializeSection (configuration, xmlFragment), Throws.Nothing);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithDisableAccessChecksSetTrue ()
    {
      string xmlFragment = @"<remotion.security xmlns=""http://www.re-motion.org/Security/Configuration/3.0"" disableAccessChecks=""true"" />";
      var configuration = new SecurityConfiguration();
      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);
      Assert.That (configuration.DisableAccessChecks, Is.True);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithDisableAccessChecksSetFalse ()
    {
      string xmlFragment = @"<remotion.security xmlns=""http://www.re-motion.org/Security/Configuration/3.0"" disableAccessChecks=""false"" />";
      var configuration = new SecurityConfiguration();
      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);
      Assert.That (configuration.DisableAccessChecks, Is.False);
    }

    private void ResetCurrentSecurityConfiguration ()
    {
      PrivateInvoke.SetNonPublicStaticField (
          typeof (SecurityConfiguration),
          "s_current",
          new Lazy<SecurityConfiguration> (() => new SecurityConfiguration()));
    }
  }
}