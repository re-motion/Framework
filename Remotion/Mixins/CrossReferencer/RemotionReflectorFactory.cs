// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef
{
  public static class RemotionReflectorFactory
  {
    public static IRemotionReflector Create (string assemblyDirectory, string reflectorSource)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);
      ArgumentUtility.CheckNotNull ("reflectorSource", reflectorSource);

      var assemblies = GetReflectorAssemblies (reflectorSource);

      if (!assemblies.Any ())
        throw new ArgumentException ("There are no assemblies matching the given reflector source", "reflectorSource");

      return new RemotionReflector ("Remotion", DetectVersion (assemblyDirectory), assemblies, assemblyDirectory);
    }

    public static IRemotionReflector Create (string assemblyDirectory, Type customReflector)
    {
      return ((IRemotionReflector) Activator.CreateInstance(customReflector, assemblyDirectory)).Initialize(assemblyDirectory);
    }

    private static Version DetectVersion (string assemblyDirectory)
    {
      // Assumption: There is always a 'Remotion.dll'
      return AssemblyName.GetAssemblyName (Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll"))).Version;
    }

    private static IEnumerable<_Assembly> GetReflectorAssemblies (string reflectorPath)
    {
      var path = Path.GetDirectoryName (reflectorPath);
      path = string.IsNullOrEmpty (path) ? "." : path;
      var file = Path.GetFileName (reflectorPath) ?? "*.dll";

      return Directory.GetFiles (Path.GetFullPath (path), file).Select (f => (_Assembly) Assembly.LoadFile (f));
    }
  }
}