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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Development.Data.UnitTesting.DomainObjects.Configuration
{
  /// <summary>
  /// Used to instantiate fake implementations of <see cref="IStorageObjectFactory"/>s.
  /// </summary>
  public static class FakeStorageSettings
  {
    /// <summary>
    /// Creates a <see cref="FakeStorageSettingsFactory"/> named <c>Default</c> and a <see cref="SqlStorageObjectFactory"/> with the given <paramref name="connectionString"/>.
    /// </summary>
    public static IStorageSettings CreateForSqlServer (string connectionString, string readOnlyConnectionString, Func<IStorageSettings, SqlStorageObjectFactory>? sqlStorageObjectFactoryFactory = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty("connectionString", connectionString);
      ArgumentUtility.CheckNotNullOrEmpty("readOnlyConnectionString", readOnlyConnectionString);

      var fakeStorageObjectFactoryFactory = new FakeStorageObjectFactoryFactory();
      var fakeStorageSettingsFactory = new FakeStorageSettingsFactory();
      var fakeStorageSettingsFactoryResolver = new FakeStorageSettingsFactoryResolver(fakeStorageSettingsFactory);

      var deferredStorageSettings = new DeferredStorageSettings(fakeStorageObjectFactoryFactory, fakeStorageSettingsFactoryResolver);

      var sqlStorageObjectFactory = sqlStorageObjectFactoryFactory != null
          ? sqlStorageObjectFactoryFactory(deferredStorageSettings)
          : new SqlStorageObjectFactory(
              deferredStorageSettings,
              SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>(),
              SafeServiceLocator.Current.GetInstance<IDataContainerValidator>());

      Assertion.IsNotNull(sqlStorageObjectFactory, "sqlStorageObjectFactoryFactory(...) was evaluated and returned null");

      var providerDefinition = new RdbmsProviderDefinition("Default", sqlStorageObjectFactory, connectionString, readOnlyConnectionString);

      var storageSettings = new StorageSettings(providerDefinition, new[] { providerDefinition });

      fakeStorageObjectFactoryFactory.SetUp(sqlStorageObjectFactory);
      fakeStorageSettingsFactory.SetUp(storageSettings);

      return deferredStorageSettings;
    }
  }
}
