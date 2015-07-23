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

namespace Remotion.Reflection.TypeDiscovery.AssemblyLoading
{
  /// <summary>
  /// Defines an interface for classes that can load assemblies from a file path.
  /// </summary>
  public interface IAssemblyLoader
  {
    /// <summary>
    /// Tries to load an assembly from the given <paramref name="filePath"/>, returning <see langword="null"/> if the file exists but is no assembly.
    /// </summary>
    /// <param name="filePath">The file path to load the assembly from.</param>
    /// <returns>The loaded assembly, or <see langword="null"/> if the assembly can't be loaded.</returns>
    /// <exception cref="AssemblyLoaderException">Thrown when the file cannot be found or an unexpected exception occurs while loading it.</exception>
    Assembly TryLoadAssembly (string filePath);

    /// <summary>
    /// Tries the load an assembly from the given <paramref name="assemblyName"/>, returning <see langword="null"/> if the file exists but is no 
    /// assembly.
    /// </summary>
    /// <param name="assemblyName">The assembly name to load the assembly from.</param>
    /// <param name="context">Context information to be included with the exception message when the assembly cannot be found or an unexpected 
    /// exception occurs while loading it.</param>
    /// <returns>The loaded assembly, or <see langword="null"/> if the assembly can't be loaded.</returns>
    /// <exception cref="AssemblyLoaderException">Thrown when the assembly cannot be found or an unexpected exception occurs while loading it.</exception>
    Assembly TryLoadAssembly (AssemblyName assemblyName, string context);
  }
}
