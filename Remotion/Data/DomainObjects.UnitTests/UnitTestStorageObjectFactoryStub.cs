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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public class UnitTestStorageObjectFactoryStub : IRdbmsStorageObjectFactory
  {
  public StorageProvider CreateStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      var providerDefiniton = ArgumentUtility.CheckNotNullAndType<UnitTestStorageProviderStubDefinition>(
          "storageProviderDefinition",
          storageProviderDefinition);
      return new UnitTestStorageProviderStub(providerDefiniton, persistenceExtension);
    }

    public IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      var typeConversionProvider = SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>();
      var dataContainerValidator = SafeServiceLocator.Current.GetInstance<IDataContainerValidator>();
      var storageSettings = new StorageSettings(storageProviderDefinition, new[] { storageProviderDefinition });

      var storageObjectFactory = new SqlStorageObjectFactory(storageSettings, typeConversionProvider, dataContainerValidator);
      return storageObjectFactory.CreatePersistenceModelLoader(storageProviderDefinition);
    }

    public IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
        StorageProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IMappingConfiguration mappingConfiguration)
    {
      throw new NotImplementedException();
    }

    public IStorageProviderSerializer CreateStorageProviderSerializer (IEnumSerializer enumSerializer)
    {
      throw new NotImplementedException();
    }

    public IEnumSerializer CreateEnumSerializer ()
    {
      throw new NotImplementedException();
    }

    public IScriptBuilder CreateSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IStorageNameProvider CreateStorageNameProvider (RdbmsProviderDefinition storageProviderDefiniton)
    {
      throw new NotImplementedException();
    }

    public IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IStoragePropertyDefinitionResolver CreateStoragePropertyDefinitionResolver (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IDataParameterDefinitionFactory CreateDataParameterDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IScriptBuilder CreateTableBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IScriptBuilder CreateViewBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IScriptBuilder CreateConstraintBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IScriptBuilder CreateIndexBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IScriptBuilder CreateSynonymBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IDbCommandBuilderFactory CreateDbCommandBuilderFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IStorageTypeInformationProvider CreateStorageTypeInformationProvider (RdbmsProviderDefinition rdmsStorageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      throw new NotImplementedException();
    }

    public ISqlDialect CreateSqlDialect (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }
  }
}
