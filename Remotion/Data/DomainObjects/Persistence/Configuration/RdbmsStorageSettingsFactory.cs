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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Creates an <see cref="IStorageSettings"/> object with an <see cref="RdbmsProviderDefinition"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public sealed class RdbmsStorageSettingsFactory : IStorageSettingsFactory
  {
    public string ProviderName { get; }
    public string ConnectionString { get; }
    public string ReadOnlyConnectionString { get; }
    public Type StorageObjectFactoryType { get; }

    public RdbmsStorageSettingsFactory (string providerName, Type storageObjectFactoryType, string connectionString, string readOnlyConnectionString)
    {
      ArgumentUtility.CheckNotNullOrEmpty("providerName", providerName);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("storageObjectFactoryType", storageObjectFactoryType, typeof(IRdbmsStorageObjectFactory));
      ArgumentUtility.CheckNotNullOrEmpty("connectionString", connectionString);
      ArgumentUtility.CheckNotNullOrEmpty("readOnlyConnectionString", readOnlyConnectionString);

      ProviderName = providerName;
      ConnectionString = connectionString;
      ReadOnlyConnectionString = readOnlyConnectionString;
      StorageObjectFactoryType = storageObjectFactoryType;
    }

    /// <summary>
    /// Creates an <see cref="IStorageSettings"/> object.
    /// </summary>
    /// <returns>A new <see cref="StorageSettings"/> object with a single default <see cref="StorageProviderDefinition" /> and no storage groups.</returns>
    public IStorageSettings Create (IStorageObjectFactoryFactory storageObjectFactoryFactory)
    {
      ArgumentUtility.CheckNotNull("storageObjectFactoryFactory", storageObjectFactoryFactory);

      var storageObjectFactory = storageObjectFactoryFactory.Create(StorageObjectFactoryType);
      Assertion.IsTrue(
          StorageObjectFactoryType.IsInstanceOfType(storageObjectFactory),
          "The factory-created instance '{0}' does not match the expected type '{1}'.",
          storageObjectFactory.GetType().GetFullNameSafe(),
          StorageObjectFactoryType.GetFullNameSafe());
      Assertion.DebugAssert(
          typeof(IRdbmsStorageObjectFactory).IsAssignableFrom(StorageObjectFactoryType),
          "typeof(IRdbmsStorageObjectFactory).IsAssignableFrom(StorageObjectFactoryType)");

      var rdbmsStorageObjectFactory = (IRdbmsStorageObjectFactory)storageObjectFactory;
      var providerDefinition = new RdbmsProviderDefinition(ProviderName, rdbmsStorageObjectFactory, ConnectionString, ReadOnlyConnectionString);

      return new StorageSettings(providerDefinition, new [] { providerDefinition });
    }
  }
}
