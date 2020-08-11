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

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  public static class CustomUserDirectory
  {
    private static bool s_firstRun = true;
    private static string s_customUserDirectory;

    public static string GetCustomUserDirectory ()
    {
      if (s_firstRun)
      {
        s_customUserDirectory = GetTemporaryDirectory();
        EnsureDirectoryDoesNotExist (s_customUserDirectory);
        s_firstRun = false;
      }

      return s_customUserDirectory;
    }

    private static string GetTemporaryDirectory ()
    {
      string tempDirectory = Path.Combine (Path.GetTempPath(), Path.GetRandomFileName());
      Directory.CreateDirectory (tempDirectory);
      return tempDirectory;
    }

    private static void EnsureDirectoryDoesNotExist (string customUserDirectoryPath)
    {
      Directory.Delete (customUserDirectoryPath);
    }
  }
}