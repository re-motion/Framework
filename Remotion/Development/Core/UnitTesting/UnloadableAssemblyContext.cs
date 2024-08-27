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
using System.Runtime.Loader;
using System.Threading;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// An wrapper for <see cref="AssemblyLoadContext"/> that ensures that the assembly-unload has
  /// completed before control is returned to the caller when the object is disposed.
  /// </summary>
  public class UnloadableAssemblyContext : IDisposable
  {
    private readonly WeakReference _weakAssemblyLoadContextReference;

    private AssemblyLoadContext? _assemblyLoadContext;

    public UnloadableAssemblyContext ()
    {
      _assemblyLoadContext = new AssemblyLoadContext(null, true);
      _weakAssemblyLoadContextReference = new WeakReference(_assemblyLoadContext);
    }

    private AssemblyLoadContext Context => _assemblyLoadContext ?? throw new InvalidOperationException("AssemblyLoadContext has already been unloaded.");

    public void RunWithAssemblyLoadContext (Action<AssemblyLoadContext> action)
    {
      ArgumentUtility.CheckNotNull("action", action);

      var context = Context;
      action(context);
    }

    public Assembly LoadFromAssemblyPath (string assemblyPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("assemblyPath", assemblyPath);

      return Context.LoadFromAssemblyPath(assemblyPath);
    }

    public Assembly LoadFromNativeImagePath (string nativeImagePath, string? assemblyPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("nativeImagePath", nativeImagePath);

      return Context.LoadFromNativeImagePath(nativeImagePath, assemblyPath);
    }

    public Assembly LoadFromStream (Stream assembly)
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);

      return Context.LoadFromStream(assembly);
    }

    public Assembly LoadFromStream (Stream assembly, Stream? assemblySymbols)
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);

      return Context.LoadFromStream(assembly, assemblySymbols);
    }

    public void Unload ()
    {
      const int maxRetries = 10;

      TriggerUnload();

      for (var i = 0; _weakAssemblyLoadContextReference.IsAlive && i <= maxRetries; i++)
      {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        if (i == maxRetries)
          throw new InvalidOperationException("Could not unload AssemblyLoadContext.");
      }
    }

    void IDisposable.Dispose ()
    {
      Unload();
    }

    private void TriggerUnload ()
    {
      _assemblyLoadContext?.Unload();
      _assemblyLoadContext = null;
    }
  }
}
