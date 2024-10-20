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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer
{
  public class AssemblyResolver
  {
    public static AssemblyResolver Create ()
    {
      return new AssemblyResolver(GetAssembliesInPrivateBinPath());
    }

    private readonly Dictionary<string, AssemblyName> _assembliesInPrivateBinPath;

    private AssemblyResolver (Dictionary<string, AssemblyName> assembliesInPrivateBinPath)
    {
      _assembliesInPrivateBinPath = assembliesInPrivateBinPath;
    }

    public Assembly? HandleAssemblyResolve (object? sender, ResolveEventArgs args)
    {
      ArgumentUtility.CheckNotNull("args", args);

      if (_assembliesInPrivateBinPath.TryGetValue(args.Name, out var privateAssemblyName))
        return Assembly.Load(privateAssemblyName);

      // Simulate assembly binding redirects to the latest version.
      // This is only a fallback if the requested assembly of a different version is already loaded into the AppDomain.
      var assemblyName = new AssemblyName(args.Name);
      return AppDomain.CurrentDomain.GetAssemblies()
          .Where(a => AssemblyName.ReferenceMatchesDefinition(assemblyName, a.GetName()))
          .OrderByDescending(a => a.GetName().Version)
          .FirstOrDefault();
    }

    private static Dictionary<string, AssemblyName> GetAssembliesInPrivateBinPath ()
    {
      var privateBinPaths = (AppDomain.CurrentDomain.RelativeSearchPath ?? "")
          .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
          .Select(p => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, p))
          .Where(Directory.Exists)
          .Select(p => new DirectoryInfo(p))
          .ToArray();

      return privateBinPaths
          .SelectMany(d => d.EnumerateFileSystemInfos("*.dll").Concat(d.EnumerateFileSystemInfos("*.exe")))
          .Select(GetAssemblyNameOrNull)
          .Where(a => a != null)
          .ToDictionary(a => a.FullName, a => a);
    }

    private static AssemblyName GetAssemblyNameOrNull (FileSystemInfo file)
    {
      return AssemblyName.GetAssemblyName(file.FullName);
    }
  }
}
