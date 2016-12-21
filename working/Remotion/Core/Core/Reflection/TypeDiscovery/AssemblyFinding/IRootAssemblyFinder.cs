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
using System.Reflection;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Defines an interface for classes finding the root assemblies for type discovery.
  /// </summary>
  public interface IRootAssemblyFinder
  {
    /// <summary>
    /// Finds root assemblies as defined by the concrete implementation. Root assemblies are used by <see cref="AssemblyFinder"/> as an entry
    /// point into assembly discovery.
    /// </summary>
    /// <returns>A sequence of <see cref="RootAssembly"/> instances holding distinct, non-<see langword="null" /> <see cref="Assembly"/> objects
    /// as well as flags indicating whether to follow the references of the respective assembly or not.</returns>
    IEnumerable<RootAssembly> FindRootAssemblies ();
  }
}
