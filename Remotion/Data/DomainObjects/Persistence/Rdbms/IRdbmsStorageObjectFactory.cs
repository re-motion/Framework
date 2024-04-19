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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// <see cref="IStorageObjectFactory"/> defines the API for all relational database management system storage object factories.
  /// </summary>
  public interface IRdbmsStorageObjectFactory : IStorageObjectFactory
  {
    ISqlDialect CreateSqlDialect (RdbmsProviderDefinition storageProviderDefinition);

    IStorageTypeInformationProvider CreateStorageTypeInformationProvider (RdbmsProviderDefinition rdmsStorageProviderDefinition);
    IStorageNameProvider CreateStorageNameProvider (RdbmsProviderDefinition storageProviderDefiniton);
    IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider (RdbmsProviderDefinition storageProviderDefinition);

    ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry);
    IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (
        RdbmsProviderDefinition storageProviderDefinition);
    IDbCommandBuilderFactory CreateDbCommandBuilderFactory (RdbmsProviderDefinition storageProviderDefinition);

    IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition);
    IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionProvider (
        RdbmsProviderDefinition storageProviderDefinition);
    IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition);
    IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition);
    IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition);
    IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (RdbmsProviderDefinition storageProviderDefinition);
    IDataParameterDefinitionFactory CreateDataParameterDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition);


    IStorageProviderSerializer CreateStorageProviderSerializer (IEnumSerializer enumSerializer);

    IScriptBuilder CreateSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition);
    IScriptBuilder CreateTableBuilder (RdbmsProviderDefinition storageProviderDefinition);
    IScriptBuilder CreateViewBuilder (RdbmsProviderDefinition storageProviderDefinition);
    IScriptBuilder CreateConstraintBuilder (RdbmsProviderDefinition storageProviderDefinition);
    IScriptBuilder CreateIndexBuilder (RdbmsProviderDefinition storageProviderDefinition);
    IScriptBuilder CreateSynonymBuilder (RdbmsProviderDefinition storageProviderDefinition);
    IEnumSerializer CreateEnumSerializer ();
  }
}
