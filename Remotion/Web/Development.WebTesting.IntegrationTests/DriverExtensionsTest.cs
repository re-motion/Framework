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
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class DriverExtensionsTest : IntegrationTest
  {
    private const string c_versionPattern = @"(\d+\.){1,3}(\d+)";
    private const string c_unknown = "unknown";

    [Test]
    public void GetBrowserName_ReturnsCorrectName ()
    {
      Assert.That (Driver.GetBrowserName(), Is.EqualTo (Helper.BrowserConfiguration.BrowserName));
    }

    [Test]
    public void GetBrowserVersion_ReturnsBrowserVersion ()
    {
      if (Helper.BrowserConfiguration.IsInternetExplorer())
      {
        Assert.That (Driver.GetBrowserVersion(), Is.EqualTo (c_unknown));
      }
      else
      {
        Assert.That (Driver.GetBrowserVersion(), Is.StringMatching (c_versionPattern));
      }
    }

    [Test]
    public void GetWebDriverVersion_ReturnsWebDriverVersion ()
    {
      if (Helper.BrowserConfiguration.IsInternetExplorer())
      {
        Assert.That (Driver.GetWebDriverVersion(), Is.EqualTo (c_unknown));
      }
      else
      {
        Assert.That (Driver.GetWebDriverVersion(), Is.StringMatching (c_versionPattern));
      }
    }
  }
}