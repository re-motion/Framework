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
using System.Data.SqlClient;
using System.Linq;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  public static class RdbmsProviderObjectMother
  {
    public static RdbmsProvider CreateForIntegrationTest (
        IStorageSettings storageSettings,
        RdbmsProviderDefinition storageProviderDefinition,
        Func<RdbmsProviderDefinition, IPersistenceExtension, IRdbmsProviderCommandFactory, RdbmsProvider> ctorCall = null)
    {
      if (!storageSettings.GetStorageProviderDefinitions().Contains(storageProviderDefinition))
        throw new ArgumentException($"RdbmsProviderDefinition '{storageProviderDefinition.Name}' is not part of the storage settings.", nameof(storageProviderDefinition));

      var storageTypeInformationProvider = new SqlStorageTypeInformationProvider();
      var dbCommandBuilderFactory = new SqlDbCommandBuilderFactory(new SingleScalarSqlTableTypeDefinitionProvider(storageTypeInformationProvider), new SqlDialect());
      var storageNameProvider = new ReflectionBasedStorageNameProvider();
      var rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();
      var infrastructureStoragePropertyDefinitionProvider = new InfrastructureStoragePropertyDefinitionProvider(
          storageTypeInformationProvider, storageNameProvider);
      var dataStoragePropertyDefinitionFactory = new DataStoragePropertyDefinitionFactory(
          new ValueStoragePropertyDefinitionFactory(storageTypeInformationProvider, storageNameProvider),
          new RelationStoragePropertyDefinitionFactory(
              storageProviderDefinition,
              false,
              storageNameProvider,
              storageTypeInformationProvider,
              storageSettings));
      var dataContainerValidator = new CompoundDataContainerValidator(Enumerable.Empty<IDataContainerValidator>());
      var objectReaderFactory = new ObjectReaderFactory(
          rdbmsPersistenceModelProvider,
          infrastructureStoragePropertyDefinitionProvider,
          storageTypeInformationProvider,
          dataContainerValidator);
      var dataParameterDefinitionFactoryChain =
          new ObjectIDDataParameterDefinitionFactory(storageProviderDefinition, storageTypeInformationProvider, storageSettings,
          new SimpleDataParameterDefinitionFactory(storageTypeInformationProvider));
      var commandFactory = new RdbmsProviderCommandFactory(
          storageProviderDefinition,
          dbCommandBuilderFactory,
          rdbmsPersistenceModelProvider,
          objectReaderFactory,
          new TableDefinitionFinder(rdbmsPersistenceModelProvider),
          dataStoragePropertyDefinitionFactory,
          dataParameterDefinitionFactoryChain);

      if (ctorCall == null)
        ctorCall = (def, ext, factory) => new RdbmsProvider(def, def.ConnectionString, ext, factory, () => new SqlConnection());

      return ctorCall(storageProviderDefinition, NullPersistenceExtension.Instance, commandFactory);
    }
  }
}
