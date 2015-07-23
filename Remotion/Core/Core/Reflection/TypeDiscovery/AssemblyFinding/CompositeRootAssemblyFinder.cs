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
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Composes several <see cref="IRootAssemblyFinder"/> instances into one, combining all results and eliminating duplicates in the process.
  /// </summary>
  public class CompositeRootAssemblyFinder : IRootAssemblyFinder
  {
    private readonly IReadOnlyList<IRootAssemblyFinder> _innerFinders;

    public CompositeRootAssemblyFinder (IEnumerable<IRootAssemblyFinder> finders)
    {
      ArgumentUtility.CheckNotNull ("finders", finders);
      _innerFinders = finders.ToList().AsReadOnly();
    }

    public IReadOnlyList<IRootAssemblyFinder> InnerFinders
    {
      get { return _innerFinders; }
    }

    public IEnumerable<RootAssembly> FindRootAssemblies ()
    {
      return _innerFinders.AsParallel().SelectMany (finder => finder.FindRootAssemblies());
    }
  }
}
