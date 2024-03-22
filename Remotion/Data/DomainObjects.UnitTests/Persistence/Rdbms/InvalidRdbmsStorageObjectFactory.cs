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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [ImplementationFor(typeof(InvalidRdbmsStorageObjectFactory))]
  internal class InvalidRdbmsStorageObjectFactory : IStorageObjectFactory
  {
    public InvalidRdbmsStorageObjectFactory ()
    {
    }

    public StorageProvider CreateStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      throw new NotImplementedException();
    }

    public IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionFactory ()
    {
      throw new NotImplementedException();
    }

    public IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (StorageProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
        StorageProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IMappingConfiguration mappingConfiguration)
    {
      throw new NotImplementedException();
    }

    public IStoragePropertyDefinitionResolver CreateStoragePropertyDefinitionResolver ()
    {
      throw new NotImplementedException();
    }

    public IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (
        StorageProviderDefinition storageProviderDefinition,
        IStorageSettings storageSettings)
    {
      throw new NotImplementedException();
    }

    public IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory ()
    {
      throw new NotImplementedException();
    }

    public IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (
        StorageProviderDefinition storageProviderDefinition,
        IStorageSettings storageSettings)
    {
      throw new NotImplementedException();
    }

    public IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (StorageProviderDefinition storageProviderDefinition)
    {
      throw new NotImplementedException();
    }

    public IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory ()
    {
      throw new NotImplementedException();
    }

    public IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider ()
    {
      throw new NotImplementedException();
    }

    public IStorageNameProvider CreateStorageNameProvider ()
    {
      throw new NotImplementedException();
    }

    public IDbCommandBuilderFactory CreateDbCommandBuilderFactory ()
    {
      throw new NotImplementedException();
    }

    public IStorageTypeInformationProvider CreateStorageTypeInformationProvider ()
    {
      throw new NotImplementedException();
    }

    public IStorageSettings CreateStorageProviderDefinitionFinder ()
    {
      throw new NotImplementedException();
    }

    public ISqlQueryGenerator CreateSqlQueryGenerator (
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      throw new NotImplementedException();
    }

    public TableScriptBuilder CreateTableBuilder ()
    {
      throw new NotImplementedException();
    }

    public ViewScriptBuilder CreateViewBuilder ()
    {
      throw new NotImplementedException();
    }

    public ForeignKeyConstraintScriptBuilder CreateConstraintBuilder ()
    {
      throw new NotImplementedException();
    }

    public IndexScriptBuilder CreateIndexBuilder ()
    {
      throw new NotImplementedException();
    }

    public SynonymScriptBuilder CreateSynonymBuilder ()
    {
      throw new NotImplementedException();
    }
  }
}
