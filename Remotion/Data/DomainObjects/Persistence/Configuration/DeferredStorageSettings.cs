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
using System.Runtime.Serialization;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Enables configuration of the storage definition providers through IoC in a lazy manner.
  /// </summary>
  /// <remarks>
  /// Creates an instance of <see cref="Configuration.StorageSettings"/> at the first access to one of its methods
  /// based on the supplied <see cref="IStorageSettingsFactory"/> and <see cref="IStorageObjectFactoryFactory"/>.
  /// This <see cref="Configuration.StorageSettings"/> object is then used in every method call afterwards as well,
  /// supplying the actual functionality.
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  [UsedImplicitly]
  [Serializable]
  [ImplementationFor(typeof(IStorageSettings), Lifetime = LifetimeKind.Singleton)]
  public sealed class DeferredStorageSettings : IStorageSettings,
#pragma warning disable SYSLIB0050
      IObjectReference
#pragma warning restore SYSLIB0050
  {
    [NonSerialized]
    private readonly Lazy<IStorageSettings> _storageSettings;

    /// <summary>
    /// Creates an instance of this class. When one of the methods on this object is called, it
    /// initializes a privately held  <see cref="IStorageSettings"/> object.
    /// </summary>
    /// <param name="storageObjectFactoryFactory">
    /// The object factory used by the <see cref="IStorageSettingsFactory"/> to create the internal <see cref="IStorageSettings"/>.
    /// </param>
    /// <param name="storageSettingsFactoryResolver">Resolves the <see cref="IStorageSettingsFactory"/> to be used when initializing this object.</param>
    public DeferredStorageSettings (IStorageObjectFactoryFactory storageObjectFactoryFactory, IStorageSettingsFactoryResolver storageSettingsFactoryResolver)
    {
      ArgumentUtility.CheckNotNull("storageObjectFactoryFactory", storageObjectFactoryFactory);
      ArgumentUtility.CheckNotNull("storageSettingsFactoryResolver", storageSettingsFactoryResolver);

      _storageSettings = new Lazy<IStorageSettings>(
          () => storageSettingsFactoryResolver.Resolve().Create(storageObjectFactoryFactory),
          LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      return _storageSettings.Value.GetStorageProviderDefinition(classDefinition);
    }

    public StorageProviderDefinition GetStorageProviderDefinition (Type? storageGroupTypeOrNull)
    {
      return _storageSettings.Value.GetStorageProviderDefinition(storageGroupTypeOrNull);
    }

    public StorageProviderDefinition GetStorageProviderDefinition (string storageProviderName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("storageProviderName", storageProviderName);

      return _storageSettings.Value.GetStorageProviderDefinition(storageProviderName);
    }

    public StorageProviderDefinition? GetDefaultStorageProviderDefinition () => _storageSettings.Value.GetDefaultStorageProviderDefinition();

    public IReadOnlyCollection<StorageProviderDefinition> GetStorageProviderDefinitions () => _storageSettings.Value.GetStorageProviderDefinitions();

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();

      if (storageSettings is not DeferredStorageSettings)
      {
        throw new SerializationException(
            "The instance cannot be deserialized because the instance of the IStorageSettings resolved via SafeServiceLocator is not of type 'DeferredStorageSettings'.");
      }

      return storageSettings;
    }
  }
}
