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
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeResolution
{
  /// <summary>Runtime implementation of the <see cref="ITypeResolutionService"/> interface.</summary>
  /// <remarks>This service should be aligned with <see cref="AssemblyFinderTypeDiscoveryService"/>. See https://www.re-motion.org/jira/browse/RM-6413</remarks>
  /// <threadsafety static="true" instance="false"/>
  public sealed class DefaultTypeResolutionService : ITypeResolutionService
  {
    public DefaultTypeResolutionService ()
    {
    }

    public Assembly? GetAssembly (AssemblyName name)
    {
      ArgumentUtility.CheckNotNull("name", name);

      return GetAssembly(name, false);
    }

    public Assembly? GetAssembly (AssemblyName name, bool throwOnError)
    {
      ArgumentUtility.CheckNotNull("name", name);

      try
      {
        return Assembly.Load(name);
      }
      catch (FileNotFoundException)
      {
        if (!throwOnError)
          return null;

        throw;
      }
    }

    public Type? GetType (string name)
    {
      ArgumentUtility.DebugCheckNotNull("name", name);

      return GetType(name, throwOnError: false, ignoreCase: false);
    }

    public Type? GetType (string name, bool throwOnError)
    {
      ArgumentUtility.DebugCheckNotNull("name", name);

      return GetType(name, throwOnError, ignoreCase: false);
    }

    public Type? GetType (string name, bool throwOnError, bool ignoreCase)
    {
      ArgumentUtility.DebugCheckNotNull("name", name);

      return Type.GetType(name, throwOnError, ignoreCase);
    }

    public void ReferenceAssembly (AssemblyName name)
    {
      throw new NotSupportedException("The ReferenceAssembly method is not supported. See https://www.re-motion.org/jira/browse/RM-6412");
    }

    public string? GetPathOfAssembly (AssemblyName name)
    {
      ArgumentUtility.CheckNotNull("name", name);

      var assembly = GetAssembly(name, throwOnError: false);
      if (assembly == null)
        return null;

      return assembly.Location;
    }
  }
}
