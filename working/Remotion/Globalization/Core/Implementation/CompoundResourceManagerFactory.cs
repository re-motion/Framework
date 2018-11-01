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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Implements the <see cref="IResourceManagerFactory"/> interface for attributes that implement the <see cref="IResourcesAttribute"/>.
  /// </summary>
  /// <seealso cref="MultiLingualResourcesAttribute"/>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IResourceManagerFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public class CompoundResourceManagerFactory : IResourceManagerFactory
  {    private readonly IReadOnlyList<IResourceManagerFactory> _resourceManagerFactories;

    /// <summary>
    ///   Combines several <see cref="IResourceManagerFactory"/>-instances to a single <see cref="CompoundResourceManagerFactory"/>.
    /// </summary>
    /// <param name="resourceManagerFactories"> The <see cref="IGlobalizationService"/>s, starting with the least specific.</param>
    public CompoundResourceManagerFactory (IEnumerable<IResourceManagerFactory> resourceManagerFactories)
    {
      ArgumentUtility.CheckNotNull ("resourceManagerFactories", resourceManagerFactories);

      _resourceManagerFactories = resourceManagerFactories.ToList().AsReadOnly();
    }

    public IReadOnlyList<IResourceManagerFactory> ResourceManagerFactories
    {
      get { return _resourceManagerFactories; }
    }

    public IResourceManager CreateResourceManager (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return new ResourceManagerSet (_resourceManagerFactories.Select (f => f.CreateResourceManager (type)));
    }
  }
}