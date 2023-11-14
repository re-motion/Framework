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
using System.Linq;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Loads root assemblies by assembly names.
  /// </summary>
  public class NamedRootAssemblyFinder : IRootAssemblyFinder
  {
    private readonly IReadOnlyList<AssemblyNameSpecification> _specifications;
    private readonly IAssemblyLoader _assemblyLoader;

    public NamedRootAssemblyFinder (IEnumerable<AssemblyNameSpecification> specifications, IAssemblyLoader assemblyLoader)
    {
      ArgumentUtility.CheckNotNull("specifications", specifications);
      ArgumentUtility.CheckNotNull("assemblyLoader", assemblyLoader);

      _specifications = specifications.ToList().AsReadOnly();
      _assemblyLoader = assemblyLoader;
    }

    public IReadOnlyList<AssemblyNameSpecification> Specifications
    {
      get { return _specifications; }
    }

    public IAssemblyLoader AssemblyLoader
    {
      get { return _assemblyLoader; }
    }

    public IEnumerable<RootAssembly> FindRootAssemblies ()
    {
      var rootAssemblies = from specification in _specifications
                           let assembly = _assemblyLoader.TryLoadAssembly(specification.AssemblyName, specification.ToString())
                           where assembly != null
                           select new RootAssembly(assembly, specification.FollowReferences);
      return rootAssemblies.Distinct();
    }
  }
}
