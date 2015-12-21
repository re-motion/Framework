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
using System.Collections;
using System.Web.Compilation;
using JetBrains.Annotations;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// Represents a <see cref="BuildManager"/> compatible type.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public interface IBuildManager
  {
    /// <summary>
    /// Finds a type in the top-level assemblies, or in assemblies that are defined in configuration, by using a case-insensitive search and optionally throwing an exception on failure.
    /// </summary>
    /// <param name="typeName">The name of the type.</param>
    /// <param name="throwOnError"><see langword="true" /> to throw an exception if a Type cannot be generated for the type name; otherwise, <see langword="false" />.</param>
    /// <param name="ignoreCase"><see langword="true" /> if typeName is case-sensitive; otherwise, <see langword="false" />.</param>
    /// <returns>A Type object that represents the requested typeName parameter, or <see langword="null" />.</returns>
    [CanBeNull]
    Type GetType (string typeName, bool throwOnError, bool ignoreCase);

    /// <summary>
    /// Compiles a file, given its virtual path, and returns the compiled type.
    /// </summary>
    /// <param name="virtualPath">The virtual path to build into a type.</param>
    /// <returns>A <see cref="Type"/> object that represents the type generated from compiling the virtual path, or <see langword="null" />.</returns>
    [CanBeNull]
    Type GetCompiledType (string virtualPath);

    /// <summary>
    /// Gets a list of assemblies built from the App_Code directory.
    /// </summary>
    [CanBeNull]
    IList CodeAssemblies { get; }
  }
}