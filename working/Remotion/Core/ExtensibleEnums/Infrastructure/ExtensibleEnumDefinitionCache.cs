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
using Remotion.Collections;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums.Infrastructure
{
  /// <summary>
  /// Caches <see cref="ExtensibleEnumDefinition{T}"/> instances for non-generic, reflective access.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof(ExtensibleEnumDefinitionCache), Lifetime = LifetimeKind.Singleton)]
  public sealed class ExtensibleEnumDefinitionCache
  {
    private readonly LockingCacheDecorator<Type, IExtensibleEnumDefinition> _cache = CacheFactory.CreateWithLocking<Type, IExtensibleEnumDefinition>();
    private readonly IExtensibleEnumValueDiscoveryService _valueDiscoveryService;

    public ExtensibleEnumDefinitionCache (IExtensibleEnumValueDiscoveryService valueDiscoveryService)
    {
      ArgumentUtility.CheckNotNull ("valueDiscoveryService", valueDiscoveryService);
      
      _valueDiscoveryService = valueDiscoveryService;
    }

    /// <summary>
    /// Gets the <see cref="IExtensibleEnumValueDiscoveryService"/> used to discover values for <see cref="ExtensibleEnumDefinition{T}"/> instances
    /// created by this <see cref="ExtensibleEnumDefinitionCache"/>.
    /// </summary>
    /// <value>The value discovery service.</value>
    public IExtensibleEnumValueDiscoveryService ValueDiscoveryService
    {
      get { return _valueDiscoveryService; }
    }

    /// <summary>
    /// Gets the <see cref="ExtensibleEnumDefinition{T}"/> for the given <paramref name="extensibleEnumType"/> from the cache creating a new
    /// one if necessary. If a new instance is created, the <see cref="ValueDiscoveryService"/> is used to discover the values of the enum type.
    /// </summary>
    /// <param name="extensibleEnumType">The type of the extensible enum for which to retrieve an <see cref="ExtensibleEnumDefinition{T}"/>.</param>
    /// <returns>The <see cref="ExtensibleEnumDefinition{T}"/> for the given type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="extensibleEnumType"/> parameter is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="extensibleEnumType"/> parameter is not derived from 
    /// <see cref="ExtensibleEnumInfo{T}"/>.</exception>
    public IExtensibleEnumDefinition GetDefinition (Type extensibleEnumType)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumType", extensibleEnumType);

      return _cache.GetOrCreateValue (extensibleEnumType, CreateDefinition);
    }

    private IExtensibleEnumDefinition CreateDefinition (Type extensibleEnumType)
    {
      Type definitionType;
      try
      {
        definitionType = typeof (ExtensibleEnumDefinition<>).MakeGenericType (extensibleEnumType);
      }
      catch (ArgumentException ex) // constraint violation
      {
        var message = string.Format ("Type '{0}' is not an extensible enum type derived from ExtensibleEnum<T>.", extensibleEnumType);
        throw new ArgumentException (message, "extensibleEnumType", ex);
      }
      return (IExtensibleEnumDefinition) Activator.CreateInstance (definitionType, new[] { ValueDiscoveryService });
    }
  }
}