﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Rhino.Mocks;

namespace Remotion.Web.Development.WebTesting.UnitTests.WebDriver.Configuration.Chrome
{
  [TestFixture]
  public class ChromeUserDirectoryCleanUpStrategyTest
  {
    [Test]
    public void Cleanup_NoRootDirectoryCleanUp ()
    {
      var userDirectoryPath = Path.Combine (Path.GetTempPath(), Guid.NewGuid().ToString ("N"));
      var chromeConfigurationStub = MockRepository.GenerateStub<IChromeConfiguration>();
      Directory.CreateDirectory (userDirectoryPath);
      chromeConfigurationStub.Stub (_ => _.EnableUserDirectoryRootCleanup).Return (false);
      var cleanUpStrategy = new ChromeUserDirectoryCleanUpStrategy (chromeConfigurationStub, userDirectoryPath);

      try
      {
        cleanUpStrategy.CleanUp();

        Assert.That (Directory.Exists (userDirectoryPath), Is.False);
      }
      finally
      {
        if (Directory.Exists (userDirectoryPath))
          Directory.Delete (userDirectoryPath, true);
      }
    }

    [Test]
    public void Cleanup_WithRootDirectoryCleanUp ()
    {
      var userDirectoryRootPath = Path.Combine (Path.GetTempPath(), Guid.NewGuid().ToString ("N"));
      var userDirectoryPath = Path.Combine (userDirectoryRootPath, "0");
      Directory.CreateDirectory (userDirectoryPath);
      var chromeConfigurationStub = MockRepository.GenerateStub<IChromeConfiguration>();
      chromeConfigurationStub.Stub (_ => _.EnableUserDirectoryRootCleanup).Return (true);
      chromeConfigurationStub.Stub (_ => _.UserDirectoryRoot).Return (userDirectoryRootPath);
      var cleanUpStrategy = new ChromeUserDirectoryCleanUpStrategy (chromeConfigurationStub, userDirectoryPath);

      try
      {
        cleanUpStrategy.CleanUp();

        Assert.That (Directory.Exists (userDirectoryRootPath), Is.False);
      }
      finally
      {
        if (Directory.Exists (userDirectoryRootPath))
          Directory.Delete (userDirectoryRootPath, true);
      }
    }
  }
}