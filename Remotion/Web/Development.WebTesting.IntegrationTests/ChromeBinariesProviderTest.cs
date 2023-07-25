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
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ChromeBinariesProviderTest
  {
    private const string c_guidRegex = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";

    [Test]
    [Retry(3)]
    public void GetInstalledExecutable ()
    {
      var provider = new ChromeBinariesProvider();

      var result = provider.GetInstalledExecutable();

      Assert.That(result.BrowserBinaryPath,
          Is.EqualTo(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe").
          Or.EqualTo(@"C:\Program Files\Google\Chrome\Application\chrome.exe"));

      Assert.That(File.Exists(result.DriverBinaryPath), Is.True);
      Assert.That(
          result.DriverBinaryPath,
          Does.Match(
              Regex.Escape(Path.GetTempPath()) + @"Remotion.Web.Development.WebTesting.WebDriver\\chromedriver\\chromedriver_v\d+\.\d+\.\d+(\.\d+)?(\\chromedriver-win32)?\\chromedriver.exe"));
      Assert.That(
          result.UserDirectory,
          Does.Match(Regex.Escape(Path.GetTempPath()) + c_guidRegex));
    }
  }
}
