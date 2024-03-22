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
using System.Diagnostics.CodeAnalysis;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
public class StorageProviderManager : IDisposable
{
  private bool _disposed;
  private StorageProviderCollection? _storageProviders;
  private readonly IPersistenceExtension _persistenceExtension;
  private readonly IStorageSettings _storageSettings;

  public StorageProviderManager (IPersistenceExtension persistenceExtension, IStorageSettings storageSettings)
  {
    ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);
    ArgumentUtility.CheckNotNull("storageSettings", storageSettings);

    _storageProviders = new StorageProviderCollection();
    _persistenceExtension = persistenceExtension;
    _storageSettings = storageSettings;
  }

  #region IDisposable Members

  public void Dispose ()
  {
    if (!_disposed)
    {
      if (_storageProviders != null)
        _storageProviders.Dispose();

      _storageProviders = null;

      _disposed = true;
      GC.SuppressFinalize(this);
    }
  }

  #endregion

  public StorageProvider GetMandatory (string storageProviderID)
  {
    CheckDisposed();
    ArgumentUtility.CheckNotNullOrEmpty("storageProviderID", storageProviderID);

    if (_storageProviders.Contains(storageProviderID))
      return _storageProviders[storageProviderID]!;

    var providerDefinition = _storageSettings.GetStorageProviderDefinition(storageProviderID);
    var provider = providerDefinition.Factory.CreateStorageProvider(providerDefinition, _persistenceExtension);

    if (provider == null)
      throw CreatePersistenceException("Storage provider '{0}' could not be created.", storageProviderID);

    _storageProviders.Add(provider);

    return provider;
  }

  public StorageProvider GetMandatory (StorageProviderDefinition providerDefinition)
  {
    CheckDisposed();
    ArgumentUtility.CheckNotNull("providerDefinition", providerDefinition);

#if DEBUG
    if (providerDefinition != _storageSettings.GetStorageProviderDefinition(providerDefinition.Name))
    {
      throw new InvalidOperationException(
          $"Supplied provider definition '{providerDefinition.Name}' does not match the provider definition with the same name in the IStorageSettings object.");
    }
#endif

    if (_storageProviders.Contains(providerDefinition.Name))
      return _storageProviders[providerDefinition.Name]!;

    var provider = providerDefinition.Factory.CreateStorageProvider(providerDefinition, _persistenceExtension);

    if (provider == null)
      throw CreatePersistenceException("Storage provider '{0}' could not be created.", providerDefinition.Name);

    _storageProviders.Add(provider);

    return provider;
  }

  public StorageProviderCollection StorageProviders
  {
    get
    {
      CheckDisposed();
      return _storageProviders;
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
      throw new ObjectDisposedException("StorageProviderManager", "A disposed StorageProviderManager cannot be accessed.");
#pragma warning disable 8774 // Disable _storageProviders-not-initialized warning
  }
#pragma warning restore 8774
}
}
