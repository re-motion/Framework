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
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyLoading
{
  /// <summary>
  /// Filters the assemblies loaded during type discovery by custom attributes, including only those which have a given attribute defined. 
  /// Note that the assemblies have to be loaded in memory in order to check whether the attribute is present. This will also lock those assembly 
  /// files that are rejected.
  /// </summary>
  public class AttributeAssemblyLoaderFilter : IAssemblyLoaderFilter
  {
    private readonly Type _attributeType;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeAssemblyLoaderFilter"/> class.
    /// </summary>
    /// <param name="attributeType">The attribute type to filter assemblies with.</param>
    public AttributeAssemblyLoaderFilter (Type attributeType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("attributeType", attributeType, typeof(Attribute));
      _attributeType = attributeType;
    }

    /// <summary>
    /// Gets the attribute type used for filtering. Only assemblies that have this attribute defined are included.
    /// </summary>
    /// <value>The attribute type used for filtering.</value>
    public Type AttributeType
    {
      get { return _attributeType; }
    }

    /// <summary>
    /// Always returns <see langword="true" />; all filtering is performed by <see cref="ShouldIncludeAssembly"/>.
    /// </summary>
    public bool ShouldConsiderAssembly (AssemblyName assemblyName)
    {
      ArgumentUtility.CheckNotNull("assemblyName", assemblyName);
      return true;
    }

    /// <summary>
    /// Determines whether the assembly of the given name should be considered for inclusion by the <see cref="FilteringAssemblyLoader"/>.
    /// Assemblies are considered only if the assembly has the <see cref="AttributeType"/> defined.
    /// </summary>
    /// <param name="assembly">The assembly to be checked.</param>
    /// <returns>
    /// <see langword="false" /> unless the <paramref name="assembly"/> has the <see cref="AttributeType"/> defined.
    /// </returns>
    public bool ShouldIncludeAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);
      return assembly.IsDefined(_attributeType, false);
    }
  }
}
