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
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Mixins;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Persistence
{
  [ImplementationFor(typeof(SecurityManagerSqlStorageObjectFactory), Lifetime = LifetimeKind.Singleton)]
  public class SecurityManagerSqlStorageObjectFactory : SqlStorageObjectFactory
  {
    public SecurityManagerSqlStorageObjectFactory (IStorageSettings storageSettings, ITypeConversionProvider typeConversionProvider, IDataContainerValidator dataContainerValidator, IDomainModelConstraintProvider domainModelConstraintProvider)
        : base(storageSettings, typeConversionProvider, dataContainerValidator, domainModelConstraintProvider)
    {
    }

    protected override IStorageProvider CreateStorageProvider (
        IPersistenceExtension persistenceExtension,
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsProviderCommandFactory commandFactory)
    {
      ArgumentNullException.ThrowIfNull(persistenceExtension);
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(commandFactory);

      return ObjectFactory.Create<SecurityManagerRdbmsProvider>(
          ParamList.Create(
              storageProviderDefinition,
              storageProviderDefinition.ConnectionString,
              persistenceExtension,
              commandFactory,
              (Func<DbConnection>)(() => new SqlConnection())));
    }

    protected override IReadOnlyStorageProvider CreateReadOnlyStorageProvider (
        IPersistenceExtension persistenceExtension,
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsProviderCommandFactory commandFactory)
    {
      ArgumentNullException.ThrowIfNull(persistenceExtension);
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(commandFactory);

      return ObjectFactory.Create<SecurityManagerRdbmsProvider>(
          ParamList.Create(
              storageProviderDefinition,
              storageProviderDefinition.ReadOnlyConnectionString,
              persistenceExtension,
              commandFactory,
              (Func<DbConnection>)(() => new SqlConnection())));
    }
  }
}
