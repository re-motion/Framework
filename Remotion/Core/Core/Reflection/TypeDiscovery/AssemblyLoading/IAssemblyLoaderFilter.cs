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
using System.Reflection;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;

namespace Remotion.Reflection.TypeDiscovery.AssemblyLoading
{
  /// <summary>
  /// Provides an interface for filtering the assemblies loaded by the <see cref="FilteringAssemblyLoader"/>.
  /// </summary>
  /// <remarks>The filtering interface provides a two-step model: first, the <see cref="AssemblyFinder"/> checks whether the assembly name fits the 
  /// criteria of the filter implementation, then, it checks whether the assembly itself fits the criteria. If the <see cref="FilteringAssemblyLoader"/> 
  /// locates an assembly on disk, it will only load it if the assembly's name passes the first step. After loading, the second step can still reject 
  /// the assembly based on more detailed investigation.</remarks>
  public interface IAssemblyLoaderFilter
  {
    /// <summary>
    /// Determines whether the assembly of the given name should be considered for inclusion by the <see cref="FilteringAssemblyLoader"/>.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to be checked.</param>
    /// <returns><see langword="true" /> if the <see cref="AssemblyFinder"/> should consider this assembly; otherwise <see langword="false" />.</returns>
    /// <remarks>This is the first step of a two-step filtering protocol. Assemblies rejected by this method will not be explicitly loaded by the
    /// <see cref="FilteringAssemblyLoader"/>.</remarks>
    bool ShouldConsiderAssembly (AssemblyName assemblyName);
    
    /// <summary>
    /// Determines whether the given assembly should be included in the list of assemblies returned by the <see cref="FilteringAssemblyLoader"/>.
    /// </summary>
    /// <param name="assembly">The assembly to be checked.</param>
    /// <returns><see langword="true" /> if the <see cref="FilteringAssemblyLoader"/> should return this assembly; otherwise <see langword="false" />.</returns>
    /// <remarks>This is the second step of a two-step filtering protocol. Only assemblies not rejected by <see cref="ShouldConsiderAssembly"/> are
    /// passed on to this step.</remarks>
    bool ShouldIncludeAssembly (Assembly assembly);
  }
}
