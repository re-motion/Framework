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
#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Wraps the initialization of <see cref="SearchPathRootAssemblyFinder"/> based on <see cref="AppDomain"/>.<see cref="AppDomain.CurrentDomain"/>.
  /// </summary>
  public class CurrentAppDomainBasedRootAssemblyFinder : IRootAssemblyFinder
  {
    public IAssemblyLoader AssemblyLoader { get; }

    public CurrentAppDomainBasedRootAssemblyFinder (IAssemblyLoader assemblyLoader)
    {
      ArgumentUtility.CheckNotNull("assemblyLoader", assemblyLoader);
      AssemblyLoader = assemblyLoader;
    }

    public IEnumerable<RootAssembly> FindRootAssemblies ()
    {
      var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(false, AssemblyLoader);
      return rootAssemblyFinder.FindRootAssemblies();
    }
  }
}

#endif
