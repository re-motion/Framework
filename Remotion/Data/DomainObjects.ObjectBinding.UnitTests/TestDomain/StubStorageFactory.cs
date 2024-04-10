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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2014;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
  public class StubStorageFactory : IRdbmsStorageObjectFactory
  {
    public IStorageProvider CreateStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return new StubStorageProvider(storageProviderDefinition, persistenceExtension);
    }

    public IReadOnlyStorageProvider CreateReadOnlyStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return new StubStorageProvider(storageProviderDefinition, persistenceExtension);
    }

    public IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition,
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return new SqlStorageObjectFactory().CreatePersistenceModelLoader(storageProviderDefinition, storageProviderDefinitionFinder);
    }

    public IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
        StorageProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IMappingConfiguration mappingConfiguration)
    {
      return new StubDomainObjectQueryGenerator();
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

    public IScriptBuilder CreateTableBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return new TableScriptBuilder(new SqlTableScriptElementFactory(), new SqlCommentScriptElementFactory());
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

    public IRdbmsProviderCommandFactory CreateStorageProviderCommandFactory (RdbmsProviderDefinition storageProviderDefinition)
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
