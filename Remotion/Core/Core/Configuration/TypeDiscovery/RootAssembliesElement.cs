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
using System.Configuration;
using System.Reflection;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Configures the root assemblies to be used for type discovery.
  /// </summary>
  public sealed class RootAssembliesElement : ConfigurationElement
  {
    /// <summary>
    /// Gets a <see cref="ByNameRootAssemblyElementCollection"/> allowing to specify assemblies by <see cref="AssemblyName"/>.
    /// </summary>
    /// <value>A <see cref="ByNameRootAssemblyElementCollection"/> allowing to specify assemblies by <see cref="AssemblyName"/>.</value>
    [ConfigurationProperty ("byName")]
    public ByNameRootAssemblyElementCollection ByName
    {
      get { return (ByNameRootAssemblyElementCollection) this["byName"]; }
    }

    /// <summary>
    /// Gets a <see cref="ByFileRootAssemblyElementCollection"/> allowing to specify assemblies by file name patterns.
    /// </summary>
    /// <value>A <see cref="ByFileRootAssemblyElementCollection"/> allowing to specify assemblies by file name patterns.</value>
    [ConfigurationProperty ("byFile")]
    public ByFileRootAssemblyElementCollection ByFile
    {
      get { return (ByFileRootAssemblyElementCollection) this["byFile"]; }
    }

    /// <summary>
    /// Creates a <see cref="CompositeRootAssemblyFinder"/> representing the assembly specifications given by <see cref="ByName"/> and
    /// <see cref="ByFile"/>.
    /// </summary>
    /// <returns>A <see cref="CompositeRootAssemblyFinder"/> for the assembly specifications.</returns>
    public CompositeRootAssemblyFinder CreateRootAssemblyFinder (IAssemblyLoader assemblyLoader)
    {
      var namedFinder = ByName.CreateRootAssemblyFinder (assemblyLoader);
      var filePatternFinder = ByFile.CreateRootAssemblyFinder (assemblyLoader);

      return new CompositeRootAssemblyFinder (new IRootAssemblyFinder[] { namedFinder, filePatternFinder });
    }
  }
}
