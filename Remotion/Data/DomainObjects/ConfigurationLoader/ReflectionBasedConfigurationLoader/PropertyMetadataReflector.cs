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
using System.Collections.Concurrent;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  [ImplementationFor(typeof(IPropertyMetadataProvider), Lifetime = LifetimeKind.Singleton)]
  public sealed class PropertyMetadataReflector : IPropertyMetadataProvider
  {
    private readonly ConcurrentDictionary<IPropertyInformation, StorageClass?> _storageClassCache
        = new ConcurrentDictionary<IPropertyInformation, StorageClass?>();

    public PropertyMetadataReflector ()
    {
    }

    public StorageClass? GetStorageClass (IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull("propertyInformation", propertyInformation);

      // C# compiler 7.2 already provides caching for anonymous method.
      return _storageClassCache.GetOrAdd(propertyInformation, key => key.GetCustomAttribute<StorageClassAttribute>(true)?.StorageClass);
    }
  }
}
