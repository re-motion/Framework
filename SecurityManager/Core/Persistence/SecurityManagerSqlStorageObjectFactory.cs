// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Data;
using System.Data.SqlClient;
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
    public SecurityManagerSqlStorageObjectFactory (IStorageSettings storageSettings, ITypeConversionProvider typeConversionProvider, IDataContainerValidator dataContainerValidator)
        : base(storageSettings, typeConversionProvider, dataContainerValidator)
    {
    }

    protected override IStorageProvider CreateStorageProvider (
        IPersistenceExtension persistenceExtension,
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsProviderCommandFactory commandFactory)
    {
      ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("commandFactory", commandFactory);

      return ObjectFactory.Create<SecurityManagerRdbmsProvider>(
          ParamList.Create(
              storageProviderDefinition,
              storageProviderDefinition.ConnectionString,
              persistenceExtension,
              commandFactory,
              (Func<IDbConnection>)(() => new SqlConnection())));
    }

    protected override IReadOnlyStorageProvider CreateReadOnlyStorageProvider (
        IPersistenceExtension persistenceExtension,
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsProviderCommandFactory commandFactory)
    {
      ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("commandFactory", commandFactory);

      return ObjectFactory.Create<SecurityManagerRdbmsProvider>(
          ParamList.Create(
              storageProviderDefinition,
              storageProviderDefinition.ReadOnlyConnectionString,
              persistenceExtension,
              commandFactory,
              (Func<IDbConnection>)(() => new SqlConnection())));
    }
  }
}
