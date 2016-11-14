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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome
{
  /// <summary>
  /// Contains the information required for customizing a Chrome instance.
  /// </summary>
  public sealed class ChromeExecutable
  {
    public static ChromeExecutable CreateForCustomInstance ([NotNull] string binaryPath, [NotNull] string userDirectory)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("binaryPath", binaryPath);
      ArgumentUtility.CheckNotNullOrEmpty ("userDirectory", userDirectory);

      return new ChromeExecutable (binaryPath, userDirectory);
    }

    public static ChromeExecutable CreateForDefaultInstance ([NotNull] string userDirectory)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userDirectory", userDirectory);

      return new ChromeExecutable (null, userDirectory);
    }

    [CanBeNull]
    private readonly string _binaryPath;

    [NotNull]
    private readonly string _userDirectory;

    private ChromeExecutable ([CanBeNull] string binaryPath, [NotNull] string userDirectory)
    {
      _binaryPath = binaryPath;
      _userDirectory = userDirectory;
    }

    [CanBeNull]
    public string BinaryPath
    {
      get { return _binaryPath; }
    }

    [NotNull]
    public string UserDirectory
    {
      get { return _userDirectory; }
    }
  }
}