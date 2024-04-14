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
using System.Diagnostics.CodeAnalysis;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  public abstract class StorageProviderManagerBase<TStorageProvider> : IDisposable
      where TStorageProvider : IReadOnlyStorageProvider
  {
    public IStorageSettings StorageSettings { get; }
    public IPersistenceExtension PersistenceExtension { get; }
    private readonly Dictionary<string, TStorageProvider> _storageProviders;
    private bool _disposed;

    protected StorageProviderManagerBase (IPersistenceExtension persistenceExtension, IStorageSettings storageSettings)
    {
      ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull("storageSettings", storageSettings);

      _storageProviders = new Dictionary<string, TStorageProvider>();
      PersistenceExtension = persistenceExtension;
      StorageSettings = storageSettings;
    }

    protected abstract TStorageProvider CreateStorageProvider (StorageProviderDefinition providerDefinition);

    public IReadOnlyDictionary<string, TStorageProvider> StorageProviders
    {
      get
      {
        CheckDisposed();
        return _storageProviders;
      }
    }

    [Obsolete("Use GetMandatory(StorageProviderDefinition) instead. (Version 7.0.0)")]
    public TStorageProvider GetMandatory (string storageProviderID)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNullOrEmpty("storageProviderID", storageProviderID);

      if (_storageProviders.TryGetValue(storageProviderID, out var storageProvider))
        return storageProvider;

      var providerDefinition = StorageSettings.GetStorageProviderDefinition(storageProviderID);
      var provider = CreateStorageProvider(providerDefinition);

      if (provider == null)
        throw CreatePersistenceException("Storage provider '{0}' could not be created.", storageProviderID);

      _storageProviders.Add(storageProviderID, provider);

      return provider;
    }

    public TStorageProvider GetMandatory (StorageProviderDefinition providerDefinition)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("providerDefinition", providerDefinition);

#if DEBUG
      if (providerDefinition != StorageSettings.GetStorageProviderDefinition(providerDefinition.Name))
      {
        throw new InvalidOperationException(
            $"Supplied provider definition '{providerDefinition.Name}' does not match the provider definition with the same name in the IStorageSettings object.");
      }
#endif

      if (_storageProviders.TryGetValue(providerDefinition.Name, out var storageProvider))
        return (TStorageProvider)storageProvider;

      var provider = CreateStorageProvider(providerDefinition);

      if (provider == null)
        throw CreatePersistenceException("Storage provider '{0}' could not be created.", providerDefinition.Name);

      _storageProviders.Add(providerDefinition.Name, provider);

      return provider;
    }

    public void Dispose ()
    {
      if (!_disposed)
      {
        foreach (var storageProvider in _storageProviders.Values)
        {
          storageProvider.Dispose();
        }

        _storageProviders.Clear();
        _disposed = true;
      }
    }

    private PersistenceException CreatePersistenceException (string message, params object[] args)
    {
      return new PersistenceException(string.Format(message, args));
    }

    [MemberNotNull(nameof(_storageProviders))]
    private void CheckDisposed ()
    {
      if (_disposed)
      {
        var typeName = GetType().Name;
        throw new ObjectDisposedException(typeName, $"A disposed {typeName} cannot be accessed.");
      }
#pragma warning disable 8774 // Disable LocalStorageProviders-not-initialized warning
    }
#pragma warning restore 8774
  }
}
