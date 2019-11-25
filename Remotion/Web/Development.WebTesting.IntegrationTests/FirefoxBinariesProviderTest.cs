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
using System.Text.RegularExpressions;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class FirefoxBinariesProviderTest
  {
    [Test]
    public void GetInstalledExecutable ()
    {
      var provider = new FirefoxBinariesProvider();

      var result = provider.GetInstalledExecutable();

      Assert.That (result.BrowserBinaryPath, Is.EqualTo (@"C:\Program Files\Mozilla Firefox\firefox.exe"));
      Assert.That (
          result.DriverBinaryPath,
          Is.StringMatching (
              Regex.Escape (Path.GetTempPath()) + @"Remotion.Web.Development.WebTesting.WebDriver\\geckodriver\\geckodriver_v\d+\.\d+\.\d+\\geckodriver.exe"));
    }
  }
}