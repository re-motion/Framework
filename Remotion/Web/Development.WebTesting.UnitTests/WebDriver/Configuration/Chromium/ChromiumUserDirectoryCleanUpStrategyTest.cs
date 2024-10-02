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
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium;

namespace Remotion.Web.Development.WebTesting.UnitTests.WebDriver.Configuration.Chromium
{
  [TestFixture]
  public class ChromiumUserDirectoryCleanUpStrategyTest
  {
    [Test]
    public void Cleanup_UserDirectoryRootContainsMultipleFolders_DeletesOnlyUserDirectory ()
    {
      var userDirectoryRootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
      var userDirectoryPath = Path.Combine(userDirectoryRootPath, "0");
      var anotherUserDirectoryPath = Path.Combine(userDirectoryRootPath, "1");
      Directory.CreateDirectory(userDirectoryPath);
      Directory.CreateDirectory(anotherUserDirectoryPath);
      var cleanUpStrategy = new ChromiumUserDirectoryCleanUpStrategy(userDirectoryRootPath, userDirectoryPath, NullLogger.Instance);

      try
      {
        cleanUpStrategy.CleanUp();

        Assert.That(Directory.Exists(userDirectoryPath), Is.False);
        Assert.That(Directory.Exists(anotherUserDirectoryPath), Is.True);
      }
      finally
      {
        if (Directory.Exists(userDirectoryRootPath))
          Directory.Delete(userDirectoryRootPath, true);
      }
    }

    [Test]
    public void Cleanup_UserDirectoryRootContainsOnlyUserDirectory_DeletesUserDirectoryRoot ()
    {
      var userDirectoryRootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
      var userDirectoryPath = Path.Combine(userDirectoryRootPath, "0");
      Directory.CreateDirectory(userDirectoryPath);
      var cleanUpStrategy = new ChromiumUserDirectoryCleanUpStrategy(userDirectoryRootPath, userDirectoryPath, NullLogger.Instance);

      try
      {
        cleanUpStrategy.CleanUp();

        Assert.That(Directory.Exists(userDirectoryRootPath), Is.False);
      }
      finally
      {
        if (Directory.Exists(userDirectoryRootPath))
          Directory.Delete(userDirectoryRootPath, true);
      }
    }
  }
}
