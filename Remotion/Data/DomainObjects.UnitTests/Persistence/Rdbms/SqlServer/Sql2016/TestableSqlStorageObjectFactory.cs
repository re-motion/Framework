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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Linq.SqlBackend.SqlPreparation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Sql2016
{
  public class TestableSqlStorageObjectFactory : SqlStorageObjectFactory
  {
    private readonly IStorageProviderSerializer _storageProviderSerializer;
    private readonly IEnumSerializer _enumSerializer;
    private readonly TableScriptBuilder _tableBuilder;
    private readonly ViewScriptBuilder _viewBuilder;
    private readonly ForeignKeyConstraintScriptBuilder _constraintBuilder;
    private readonly IndexScriptBuilder _indexBuilder;
    private readonly SynonymScriptBuilder _synonymBuilder;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IStorageNameProvider _storageNameProvider;
    private readonly IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;
    private readonly IDataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactory;
    private readonly IValueStoragePropertyDefinitionFactory _valueStoragePropertyDefinitionFactory;
    private readonly IRelationStoragePropertyDefinitionFactory _relationStoragePropertyDefinitionFactory;
    private readonly ISqlQueryGenerator _sqlQueryGenerator;
    private readonly IForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactory;
    private readonly IStoragePropertyDefinitionResolver _storagePropertyDefinitionResolver;

    public TestableSqlStorageObjectFactory (
        TableScriptBuilder tableBuilder,
        ViewScriptBuilder viewBuilder,
        ForeignKeyConstraintScriptBuilder constraintBuilder,
        IndexScriptBuilder indexBuilder,
        SynonymScriptBuilder synonymBuilder)
    {
      _indexBuilder = indexBuilder;
      _constraintBuilder = constraintBuilder;
      _viewBuilder = viewBuilder;
      _tableBuilder = tableBuilder;
      _synonymBuilder = synonymBuilder;
    }

    public TestableSqlStorageObjectFactory (
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IStorageNameProvider storageNameProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory,
        IValueStoragePropertyDefinitionFactory valueStoragePropertyDefinitionFactory,
        IRelationStoragePropertyDefinitionFactory relationStoragePropertyDefinitionFactory,
        ISqlQueryGenerator sqlQueryGenerator,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactoryFactory,
        IStoragePropertyDefinitionResolver storagePropertyDefinitionResolver)
    {
      _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
      _storageNameProvider = storageNameProvider;
      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _storageTypeInformationProvider = storageTypeInformationProvider;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
      _dataStoragePropertyDefinitionFactory = dataStoragePropertyDefinitionFactory;
      _valueStoragePropertyDefinitionFactory = valueStoragePropertyDefinitionFactory;
      _relationStoragePropertyDefinitionFactory = relationStoragePropertyDefinitionFactory;
      _sqlQueryGenerator = sqlQueryGenerator;
      _foreignKeyConstraintDefinitionFactory = foreignKeyConstraintDefinitionFactoryFactory;
      _storagePropertyDefinitionResolver = storagePropertyDefinitionResolver;
    }

    public TestableSqlStorageObjectFactory (IStorageProviderSerializer storageProviderSerializer, IEnumSerializer enumSerializer)
    {
      _storageProviderSerializer = storageProviderSerializer;
      _enumSerializer = enumSerializer;
    }

    public override IStorageProviderSerializer CreateStorageProviderSerializer (IEnumSerializer enumSerializer)
    {
      return _storageProviderSerializer ?? base.CreateStorageProviderSerializer(enumSerializer);
    }

    public override IEnumSerializer CreateEnumSerializer ()
    {
      return _enumSerializer ?? base.CreateEnumSerializer();
    }

    public override IScriptBuilder CreateTableBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _tableBuilder ?? base.CreateTableBuilder(storageProviderDefinition);
    }

    public override IScriptBuilder CreateViewBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _viewBuilder ?? base.CreateViewBuilder(storageProviderDefinition);
    }

    public override IScriptBuilder CreateConstraintBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _constraintBuilder ?? base.CreateConstraintBuilder(storageProviderDefinition);
    }

    public override IScriptBuilder CreateIndexBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _indexBuilder ?? base.CreateIndexBuilder(storageProviderDefinition);
    }

    public override IScriptBuilder CreateSynonymBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _synonymBuilder ?? base.CreateSynonymBuilder(storageProviderDefinition);
    }

    public override IStorageNameProvider CreateStorageNameProvider (RdbmsProviderDefinition storageProviderDefiniton)
    {
      return _storageNameProvider ?? base.CreateStorageNameProvider(storageProviderDefiniton);
    }

    protected override IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionProvider (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      return _infrastructureStoragePropertyDefinitionProvider
             ?? base.CreateInfrastructureStoragePropertyDefinitionProvider(
                 storageProviderDefinition,
                 storageTypeInformationProvider,
                 storageNameProvider);
    }

    public override IDbCommandBuilderFactory CreateDbCommandBuilderFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _dbCommandBuilderFactory ?? base.CreateDbCommandBuilderFactory(storageProviderDefinition);
    }

    public override IStorageTypeInformationProvider CreateStorageTypeInformationProvider (RdbmsProviderDefinition rdmsStorageProviderDefinition)
    {
      return _storageTypeInformationProvider ?? base.CreateStorageTypeInformationProvider(rdmsStorageProviderDefinition);
    }

    public override IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _rdbmsPersistenceModelProvider ?? base.CreateRdbmsPersistenceModelProvider(storageProviderDefinition);
    }

    protected override IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider,
        IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      return _dataStoragePropertyDefinitionFactory
             ?? base.CreateDataStoragePropertyDefinitionFactory(storageProviderDefinition, storageTypeInformationProvider, storageNameProvider, providerDefinitionFinder);
    }

    protected override IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      return _valueStoragePropertyDefinitionFactory
             ?? base.CreateValueStoragePropertyDefinitionFactory(storageProviderDefinition, storageTypeInformationProvider, storageNameProvider);
    }

    protected override IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider,
        IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      return _relationStoragePropertyDefinitionFactory
             ?? base.CreateRelationStoragePropertyDefinitionFactory(
                 storageProviderDefinition,
                 storageTypeInformationProvider,
                 storageNameProvider,
                 providerDefinitionFinder);
    }

    protected override ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      return _sqlQueryGenerator
             ?? base.CreateSqlQueryGenerator(storageProviderDefinition, methodCallTransformerProvider, resultOperatorHandlerRegistry, persistenceModelProvider);
    }

    protected override IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IRdbmsPersistenceModelProvider persistenceModelProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider)
    {
      return _foreignKeyConstraintDefinitionFactory
             ?? base.CreateForeignKeyConstraintDefinitionsFactory(
                 storageProviderDefinition,
                 storageNameProvider,
                 persistenceModelProvider,
                 infrastructureStoragePropertyDefinitionProvider);
    }

    protected override IStoragePropertyDefinitionResolver CreateStoragePropertyDefinitionResolver (
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      return _storagePropertyDefinitionResolver
             ?? base.CreateStoragePropertyDefinitionResolver(storageProviderDefinition, persistenceModelProvider);
    }
  }
}
