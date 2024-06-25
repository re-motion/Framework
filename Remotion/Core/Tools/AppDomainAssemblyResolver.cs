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
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Tools
{
  /// <summary>
  /// Helper class for resolving assemblies when executing code in a separate <see cref="AppDomain"/>. Create an instance of this class in the
  /// target app domain, then use <see cref="Register"/> to register the resolver with the <see cref="AppDomain"/>. The class derives from
  /// <see cref="MarshalByRefObject"/> in order to allow <see cref="Register"/> to be called from the parent <see cref="AppDomain"/>. After 
  /// <see cref="Register"/> was called, the resolver tries to resolve all assembly references from the given assembly directory.
  /// </summary>
  public sealed class AppDomainAssemblyResolver : MarshalByRefObject
  {
    public static AppDomainAssemblyResolver CreateInAppDomain (AppDomain appDomain, string applicationBase)
    {
      ArgumentUtility.CheckNotNull("appDomain", appDomain);
      ArgumentUtility.CheckNotNullOrEmpty("applicationBase", applicationBase);

      // TODO RM-7761: null guard should be added.
      return (AppDomainAssemblyResolver)appDomain.CreateInstanceFromAndUnwrap(
                                             typeof(AppDomainAssemblyResolver).Assembly.Location,
                                             typeof(AppDomainAssemblyResolver).GetFullNameChecked(),
                                             false,
                                             BindingFlags.Public | BindingFlags.Instance,
                                             null,
                                             new[] { applicationBase },
                                             null,
                                             null)!;
    }

    private readonly string _assemblyDirectory;

    public AppDomainAssemblyResolver (string assemblyDirectory)
    {
      // Must not access any other assemblies before AssemblyResolve is registered - perform argument check manually
      if (assemblyDirectory == null)
        throw new ArgumentNullException("assemblyDirectory");
      if (assemblyDirectory == string.Empty)
        throw new ArgumentException("Assembly directory must not be empty.", "assemblyDirectory");

      _assemblyDirectory = assemblyDirectory;
    }

    public string AssemblyDirectory
    {
      get { return _assemblyDirectory; }
    }

    public void Register (AppDomain appDomain)
    {
      // Must not access any other assemblies before AssemblyResolve is registered - perform argument check manually
      if (appDomain == null)
        throw new ArgumentNullException("appDomain");

      appDomain.AssemblyResolve += ResolveAssembly;
    }

    public Assembly? ResolveAssembly (object? sender, ResolveEventArgs args)
    {
      ArgumentUtility.CheckNotNull("sender", sender!);
      ArgumentUtility.CheckNotNull("args", args);

      var reference = new AssemblyName(args.Name);
      var assemblyLocation = GetAssemblyLocation(reference);

      if (assemblyLocation == null)
        return null;

      var assemblyName = AssemblyName.GetAssemblyName(assemblyLocation);
      if (!AssemblyName.ReferenceMatchesDefinition(reference, assemblyName))
        throw CreateFileLoadException(args.Name);

      return Assembly.LoadFile(assemblyLocation);
    }

    private string? GetAssemblyLocation (AssemblyName assemblyName)
    {
      var dllLocation = Path.Combine(_assemblyDirectory, assemblyName.GetNameChecked() + ".dll");
      if (File.Exists(dllLocation))
        return dllLocation;

      var exeLocation = Path.Combine(_assemblyDirectory, assemblyName.GetNameChecked() + ".exe");
      if (File.Exists(exeLocation))
        return exeLocation;

      return null;
    }

    private FileLoadException CreateFileLoadException (string assemblyName)
    {
      return new FileLoadException(
          String.Format(
              "Could not load file or assembly '{0}'. The located assembly's manifest definition does not match the assembly reference.",
              assemblyName));
    }
  }
}
