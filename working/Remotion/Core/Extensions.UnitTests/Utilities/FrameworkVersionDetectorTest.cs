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
using Microsoft.Win32;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.Extensions.UnitTests.Utilities
{
  [TestFixture]
  public class FrameworkVersionDetectorTest
  {
    [Test]
    public void IsMinimumVersion_Net_4_0 ()
    {
      Assert.That (FrameworkVersionDetector.IsVersionSupported (FrameworkVersion.Net_4_0), Is.True);
    }

    [Test]
    public void IsMinimumVersion_Net_4_5 ()
    {
      Assert.That (FrameworkVersionDetector.IsVersionSupported (FrameworkVersion.Net_4_5), Is.True);
    }

    [Test]
    public void IsMinimumVersion_Net_4_5_1 ()
    {
      Assert.That (FrameworkVersionDetector.IsVersionSupported (FrameworkVersion.Net_4_5_1), Is.EqualTo (IsNet_4_5_1_Installed()));
    }

    [Test]
    public void IsMinimumVersion_Net_4_5_2 ()
    {
      Assert.That (FrameworkVersionDetector.IsVersionSupported (FrameworkVersion.Net_4_5_2), Is.EqualTo (IsNet_4_5_2_Installed()));
    }

    [Test]
    public void IsMinimumVersion_Net_4_6 ()
    {
      Assert.That (FrameworkVersionDetector.IsVersionSupported (FrameworkVersion.Net_4_6), Is.EqualTo (IsNet_4_6_Installed()));
    }

    private bool IsNet_4_5_1_Installed ()
    {
      var net_4_5_1_ReleaseVersion = 378675;
      return IsNet_4_x_x_Installed (net_4_5_1_ReleaseVersion);
    }

    private bool IsNet_4_5_2_Installed ()
    {
      var net_4_5_2_ReleaseVersion = 379893;
      return IsNet_4_x_x_Installed (net_4_5_2_ReleaseVersion);
    }

    private bool IsNet_4_6_Installed ()
    {
      var net_4_6_ReleaseVersion = 393295;
      return IsNet_4_x_x_Installed (net_4_6_ReleaseVersion);
    }

    private static bool IsNet_4_x_x_Installed (int expectedReleaseVersion)
    {
      // http://msdn.microsoft.com/en-us/library/hh925568.aspx
      // http://blogs.msdn.com/b/astebner/archive/2013/11/11/10466402.aspx

      var registryKeyPath = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full";
      using (var key = RegistryKey.OpenBaseKey (RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey (registryKeyPath))
      {
        Assertion.IsNotNull (key, "Registry key '{0}' not found.", registryKeyPath);
        var release = (int?) key.GetValue ("Release");
        Assertion.IsNotNull (release, "Registry value 'Release' not found.");

        return release >= expectedReleaseVersion;
      }
    }
  }
}