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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq.SqlBackend.SqlPreparation;

namespace Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.TestDomain
{
  public class StorageObjectFactoryStub : IRdbmsStorageObjectFactory
  {
    public StorageObjectFactoryStub ()
    {
    }

    public StorageProvider CreateStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      throw new System.NotImplementedException();
    }

    public StorageProvider CreateReadOnlyStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      throw new System.NotImplementedException();
    }

    public IPersistenceModelLoader CreatePersistenceModelLoader (StorageProviderDefinition storageProviderDefinition, IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      throw new System.NotImplementedException();
    }

    public IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
        StorageProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IMappingConfiguration mappingConfiguration)
    {
      throw new System.NotImplementedException();
    }

    public ISqlDialect CreateSqlDialect (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IStorageTypeInformationProvider CreateStorageTypeInformationProvider (RdbmsProviderDefinition rdmsStorageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IStorageNameProvider CreateStorageNameProvider (RdbmsProviderDefinition storageProviderDefiniton)
    {
      throw new System.NotImplementedException();
    }

    public IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      throw new System.NotImplementedException();
    }

    public IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IDbCommandBuilderFactory CreateDbCommandBuilderFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IStorageProviderSerializer CreateStorageProviderSerializer (IEnumSerializer enumSerializer)
    {
      throw new System.NotImplementedException();
    }

    public IScriptBuilder CreateSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IScriptBuilder CreateTableBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IScriptBuilder CreateViewBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IScriptBuilder CreateConstraintBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IScriptBuilder CreateIndexBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IScriptBuilder CreateSynonymBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      throw new System.NotImplementedException();
    }

    public IEnumSerializer CreateEnumSerializer ()
    {
      throw new System.NotImplementedException();
    }
  }
}
