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
using System.Collections.Concurrent;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyLoading
{
  /// <summary>
  /// Caches the results of <see cref="AssemblyName"/> operations.
  /// </summary>
  public static class AssemblyNameCache
  {
    private static readonly ConcurrentDictionary<string, AssemblyName> s_cache = new ConcurrentDictionary<string, AssemblyName>();

    public static AssemblyName GetAssemblyName (string filePath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("filePath", filePath);

      // C# compiler 7.2 does not provide caching for delegate but calls are only during application start so no caching is needed.
      return s_cache.GetOrAdd(filePath, AssemblyName.GetAssemblyName);
    }
  }
}
