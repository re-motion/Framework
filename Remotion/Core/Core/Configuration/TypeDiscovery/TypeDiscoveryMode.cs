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

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Defines how type discovery should work.
  /// </summary>
  public enum TypeDiscoveryMode
  {
    /// <summary>
    /// Chooses automatic type discovery - the application's bin directory is searched for assemblies. The types are discovered from those assemblies
    /// and their referenced assemblies.
    /// </summary>
    Automatic,

    [Obsolete("Register the custom implementation of IRootAssemblyFinder via the SafeServiceLocator.BootstrapConfiguration. (Version 6.0.0)", true)]
    CustomRootAssemblyFinder,

    /// <summary>
    /// Chooses a number of specific root assemblies. The types are discovered from those assemblies. Whether types from referenced assemblies are 
    /// also included is defined by the user.
    /// See <see cref="TypeDiscoveryConfiguration.SpecificRootAssemblies"/>.
    /// </summary>
    SpecificRootAssemblies,

    [Obsolete("Register the custom implementation of ITypeDiscoveryService via the SafeServiceLocator.BootstrapConfiguration. (Version 6.0.0)", true)]
    CustomTypeDiscoveryService
  }
}
