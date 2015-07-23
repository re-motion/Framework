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
using System.Reflection;
using System.Threading;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Decorates an <see cref="IAssemblyFinder"/>, caching its results.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class CachingAssemblyFinderDecorator : IAssemblyFinder
  {
    private readonly IAssemblyFinder _innerFinder;
    private readonly Lazy<IReadOnlyCollection<Assembly>> _resultCache;

    public CachingAssemblyFinderDecorator (IAssemblyFinder innerFinder)
    {
      ArgumentUtility.CheckNotNull ("innerFinder", innerFinder);

      _innerFinder = innerFinder;
      _resultCache = new Lazy<IReadOnlyCollection<Assembly>> (
          () => _innerFinder.FindAssemblies().ToList().AsReadOnly(),
          LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public IAssemblyFinder InnerFinder
    {
      get { return _innerFinder; }
    }

    public IEnumerable<Assembly> FindAssemblies ()
    {
      return _resultCache.Value;
    }
  }
}