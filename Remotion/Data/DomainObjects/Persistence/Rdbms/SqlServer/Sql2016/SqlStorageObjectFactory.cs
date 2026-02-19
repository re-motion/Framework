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
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Mixins;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016
{
  /// <summary>
  /// The <see cref="SqlStorageObjectFactory"/> is responsible to create SQL Server-specific storage provider instances.
  /// </summary>
  [ImplementationFor(typeof(SqlStorageObjectFactory), Lifetime = LifetimeKind.Singleton)]
  public class SqlStorageObjectFactory : IRdbmsStorageObjectFactory
  {
    public IStorageSettings StorageSettings { get; }
    public ITypeConversionProvider TypeConversionProvider { get; }
    public IDataContainerValidator DataContainerValidator { get; }
    public IDomainModelConstraintProvider DomainModelConstraintProvider { get; }

    public SqlStorageObjectFactory (IStorageSettings storageSettings, ITypeConversionProvider typeConversionProvider, IDataContainerValidator dataContainerValidator, IDomainModelConstraintProvider domainModelConstraintProvider)
    {
      ArgumentNullException.ThrowIfNull(storageSettings);
      ArgumentNullException.ThrowIfNull(typeConversionProvider);
      ArgumentNullException.ThrowIfNull(dataContainerValidator);
      ArgumentNullException.ThrowIfNull(domainModelConstraintProvider);

      StorageSettings = storageSettings;
      TypeConversionProvider = typeConversionProvider;
      DataContainerValidator = dataContainerValidator;
      DomainModelConstraintProvider = domainModelConstraintProvider;
    }

    public IStorageProvider CreateStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      ArgumentNullException.ThrowIfNull(persistenceExtension);
      var rdbmsProviderDefinition =
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition>(nameof(storageProviderDefinition), storageProviderDefinition);

      var commandFactory = CreateStorageProviderCommandFactory(rdbmsProviderDefinition);
      return CreateStorageProvider(persistenceExtension, rdbmsProviderDefinition, commandFactory);
    }

    public IReadOnlyStorageProvider CreateReadOnlyStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      ArgumentNullException.ThrowIfNull(persistenceExtension);
      var rdbmsProviderDefinition =
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition>(nameof(storageProviderDefinition), storageProviderDefinition);

      var commandFactory = CreateStorageProviderCommandFactory(rdbmsProviderDefinition);
      return new ReadOnlyStorageProviderDecorator(CreateReadOnlyStorageProvider(persistenceExtension, rdbmsProviderDefinition, commandFactory));
    }

    public virtual IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition)
    {
      var rdmsStorageProviderDefinition =
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition>(nameof(storageProviderDefinition), storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(rdmsStorageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider(rdmsStorageProviderDefinition);
      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider(rdmsStorageProviderDefinition);
      var storagePropertyDefinitionResolver = CreateStoragePropertyDefinitionResolver(rdmsStorageProviderDefinition, persistenceModelProvider);

      var dataStoragePropertyDefinitionFactory = CreateDataStoragePropertyDefinitionFactory(
          rdmsStorageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);

      var infrastructureStoragePropertyDefinitionFactory = CreateInfrastructureStoragePropertyDefinitionProvider(
          rdmsStorageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);

      var foreignKeyConstraintDefinitionFactory = CreateForeignKeyConstraintDefinitionsFactory(
          rdmsStorageProviderDefinition,
          storageNameProvider,
          persistenceModelProvider,
          infrastructureStoragePropertyDefinitionFactory);

      var entityDefinitionFactory = CreateEntityDefinitionFactory(
          rdmsStorageProviderDefinition,
          storageNameProvider,
          infrastructureStoragePropertyDefinitionFactory,
          foreignKeyConstraintDefinitionFactory,
          storagePropertyDefinitionResolver);

      return new RdbmsPersistenceModelLoader(
          entityDefinitionFactory,
          dataStoragePropertyDefinitionFactory,
          storageNameProvider,
          persistenceModelProvider);
    }

    public virtual IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
        StorageProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IMappingConfiguration mappingConfiguration)
    {
      var rdmsStorageProviderDefinition =
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition>(nameof(storageProviderDefinition), storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(methodCallTransformerProvider);
      ArgumentNullException.ThrowIfNull(resultOperatorHandlerRegistry);
      ArgumentNullException.ThrowIfNull(mappingConfiguration);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(rdmsStorageProviderDefinition);
      var sqlQueryGenerator = CreateSqlQueryGenerator(rdmsStorageProviderDefinition, methodCallTransformerProvider, resultOperatorHandlerRegistry);

      return ObjectFactory.Create<DomainObjectQueryGenerator>(
          ParamList.Create(sqlQueryGenerator, TypeConversionProvider, storageTypeInformationProvider, mappingConfiguration));
    }


    public virtual ISqlDialect CreateSqlDialect (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      return new SqlDialect();
    }


    public virtual IStorageTypeInformationProvider CreateStorageTypeInformationProvider (RdbmsProviderDefinition rdmsStorageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(rdmsStorageProviderDefinition);

      var dateTimeDefaultStorageTypeProvider = CreateDateTimeDefaultStorageTypeProvider(rdmsStorageProviderDefinition);
      return new SqlStorageTypeInformationProvider(dateTimeDefaultStorageTypeProvider);
    }

    protected virtual IDateTimeDefaultStorageTypeProvider CreateDateTimeDefaultStorageTypeProvider (RdbmsProviderDefinition rdbmsStorageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(rdbmsStorageProviderDefinition);

      return new DateTime2DefaultStorageTypeProvider();
    }

    public virtual IStorageNameProvider CreateStorageNameProvider (RdbmsProviderDefinition storageProviderDefiniton)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefiniton);

      return new ReflectionBasedStorageNameProvider();
    }

    public virtual IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      return new RdbmsPersistenceModelProvider();
    }

    public ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(methodCallTransformerProvider);
      ArgumentNullException.ThrowIfNull(resultOperatorHandlerRegistry);

      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider(storageProviderDefinition);

      return CreateSqlQueryGenerator(
          storageProviderDefinition,
          methodCallTransformerProvider,
          resultOperatorHandlerRegistry,
          persistenceModelProvider);
    }

    public IRdbmsProviderCommandFactory CreateStorageProviderCommandFactory (
        RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);
      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider(storageProviderDefinition);

      var infrastructureStoragePropertyDefinitionProvider = CreateInfrastructureStoragePropertyDefinitionProvider(
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);

      var dataStoragePropertyDefinitionFactory = CreateDataStoragePropertyDefinitionFactory(
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);

      var simpleStructuredTypeDefinitionRepository = CreateSingleScalarStructuredTypeDefinitionProvider(storageProviderDefinition);
      var structuredTypeDefinitionFinder = CreateStructuredTypeDefinitionFinder(simpleStructuredTypeDefinitionRepository);
      var queryParameterRecordDefinitionFactory = CreateQueryParameterRecordDefinitionFinder(structuredTypeDefinitionFinder);

      var dataParameterDefinitionFactory = CreateDataParameterDefinitionFactory(storageProviderDefinition, queryParameterRecordDefinitionFactory, storageTypeInformationProvider);

      return CreateStorageProviderCommandFactory(
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider,
          persistenceModelProvider,
          infrastructureStoragePropertyDefinitionProvider,
          dataStoragePropertyDefinitionFactory,
          dataParameterDefinitionFactory);
    }

    public virtual IDbCommandBuilderFactory CreateDbCommandBuilderFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var singleScalarStructuredTypeDefinitionProvider = CreateSingleScalarStructuredTypeDefinitionProvider(storageProviderDefinition);
      var sqlDialect = CreateSqlDialect(storageProviderDefinition);
      return new SqlDbCommandBuilderFactory(singleScalarStructuredTypeDefinitionProvider, sqlDialect);
    }


    public IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider(storageProviderDefinition);
      var infrastructureStoragePropertyDefinitionProvider = CreateInfrastructureStoragePropertyDefinitionProvider(storageProviderDefinition);
      var foreignKeyConstraintDefinitionFactory = CreateForeignKeyConstraintDefinitionsFactory(storageProviderDefinition);
      var storagePropertyDefinitionResolver = CreateStoragePropertyDefinitionResolver(storageProviderDefinition, persistenceModelProvider);
      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);

      return CreateEntityDefinitionFactory(
          storageProviderDefinition,
          storageNameProvider,
          infrastructureStoragePropertyDefinitionProvider,
          foreignKeyConstraintDefinitionFactory,
          storagePropertyDefinitionResolver);
    }

    public IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionProvider (
        RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);

      return CreateInfrastructureStoragePropertyDefinitionProvider(storageProviderDefinition, storageTypeInformationProvider, storageNameProvider);
    }

    public IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);

      return CreateDataStoragePropertyDefinitionFactory(
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);
    }

    public virtual ISingleScalarStructuredTypeDefinitionProvider CreateSingleScalarStructuredTypeDefinitionProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);
      return CreateSingleScalarStructuredTypeDefinitionProvider(storageTypeInformationProvider);
    }

    public ITableManipulationRecordDefinitionProvider CreateTableManipulationRecordDefinitionProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);
      var infrastructureStoragePropertyDefinitionProvider = CreateInfrastructureStoragePropertyDefinitionProvider(storageProviderDefinition);
      var rdbmsPersistenceModelProvider = CreateRdbmsPersistenceModelProvider(storageProviderDefinition);
      return CreateTableManipulationRecordDefinitionProvider(storageTypeInformationProvider, infrastructureStoragePropertyDefinitionProvider, rdbmsPersistenceModelProvider);
    }

    public IDataParameterDefinitionFactory CreateDataParameterDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);
      var simpleStructuredTypeDefinitionRepository = CreateSingleScalarStructuredTypeDefinitionProvider(storageProviderDefinition);
      var structuredTypeDefinitionFinder = CreateStructuredTypeDefinitionFinder(simpleStructuredTypeDefinitionRepository);
      var queryParameterRecordDefinitionFactory = CreateQueryParameterRecordDefinitionFinder(structuredTypeDefinitionFinder);

      return CreateDataParameterDefinitionFactory(storageProviderDefinition, queryParameterRecordDefinitionFactory, storageTypeInformationProvider);
    }

    public IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);
      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);

      return CreateRelationStoragePropertyDefinitionFactory(
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);
    }

    public IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);

      return CreateValueStoragePropertyDefinitionFactory(storageProviderDefinition, storageTypeInformationProvider, storageNameProvider);
    }

    public IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);
      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider(storageProviderDefinition);
      var infrastructureStoragePropertyDefinitionProvider = CreateInfrastructureStoragePropertyDefinitionProvider(storageProviderDefinition);

      return CreateForeignKeyConstraintDefinitionsFactory(
          storageProviderDefinition,
          storageNameProvider,
          persistenceModelProvider,
          infrastructureStoragePropertyDefinitionProvider);
    }

    public virtual IEnumSerializer CreateEnumSerializer ()
    {
      return new ExtensibleEnumSerializerDecorator(new EnumSerializer());
    }

    public virtual IStorageProviderSerializer CreateStorageProviderSerializer (IEnumSerializer enumSerializer)
    {
      ArgumentNullException.ThrowIfNull(enumSerializer);
      return new StorageProviderSerializer(CreateClassSerializer(enumSerializer));
    }

    public virtual IClassSerializer CreateClassSerializer (IEnumSerializer enumSerializer)
    {
      ArgumentNullException.ThrowIfNull(enumSerializer);
      return new ClassSerializer(CreateTableSerializer(enumSerializer));
    }

    public virtual ITableSerializer CreateTableSerializer (IEnumSerializer enumSerializer)
    {
      ArgumentNullException.ThrowIfNull(enumSerializer);
      var propertySerializer = CreatePropertySerializer();
      var decoratedPropertySerializer = new EnumPropertySerializerDecorator(enumSerializer, propertySerializer);
      return new TableSerializer(decoratedPropertySerializer);
    }

    public virtual IPropertySerializer CreatePropertySerializer ()
    {
      return new PropertySerializer(CreateColumnSerializer());
    }

    public virtual IColumnSerializer CreateColumnSerializer ()
    {
      return new ColumnSerializer();
    }

    public virtual IScriptBuilder CreateSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var compositeScriptBuilder = new CompositeScriptBuilder(
          storageProviderDefinition,
          CreateScriptBuildersForSchemaScriptBuilder(storageProviderDefinition));

      return new SqlDatabaseSelectionScriptElementBuilder(compositeScriptBuilder, storageProviderDefinition.ConnectionString);
    }

    protected virtual IEnumerable<IScriptBuilder> CreateScriptBuildersForSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      yield return CreateTableBuilder(storageProviderDefinition);
      yield return CreateConstraintBuilder(storageProviderDefinition);
      yield return CreateViewBuilder(storageProviderDefinition);
      yield return CreateIndexBuilder(storageProviderDefinition);
      yield return CreateSynonymBuilder(storageProviderDefinition);
      yield return CreateTableTypeBuilder();
    }

    public virtual IScriptBuilder CreateTableBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      return new TableScriptBuilder(new SqlTableScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    public virtual IScriptBuilder CreateViewBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      return new ViewScriptBuilder(
          new SqlTableViewScriptElementFactory(),
          new SqlUnionViewScriptElementFactory(),
          new SqlFilterViewScriptElementFactory(),
          new SqlEmptyViewScriptElementFactory(),
          new SqlCommentScriptElementFactory());
    }

    public virtual IScriptBuilder CreateConstraintBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      return new ForeignKeyConstraintScriptBuilder(new SqlForeignKeyConstraintScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    public virtual IScriptBuilder CreateIndexBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      return new IndexScriptBuilder(
          new SqlIndexScriptElementFactory(
              new SqlIndexDefinitionScriptElementFactory(),
              new SqlPrimaryXmlIndexDefinitionScriptElementFactory(),
              new SqlSecondaryXmlIndexDefinitionScriptElementFactory()),
          new SqlCommentScriptElementFactory());
    }

    public virtual IScriptBuilder CreateSynonymBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      var sqlSynonymScriptElementFactory = new SqlSynonymScriptElementFactory();
      return new SynonymScriptBuilder(
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          new SqlCommentScriptElementFactory());
    }

    public virtual IScriptBuilder CreateTableTypeBuilder ()
    {
      return new TableTypeScriptBuilder(new SqlTableTypeScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    protected virtual IStorageProvider CreateStorageProvider (
        IPersistenceExtension persistenceExtension,
        RdbmsProviderDefinition rdbmsProviderDefinition,
        IRdbmsProviderCommandFactory commandFactory)
    {
      ArgumentNullException.ThrowIfNull(persistenceExtension);
      ArgumentNullException.ThrowIfNull(commandFactory);
      ArgumentNullException.ThrowIfNull(rdbmsProviderDefinition);

      return ObjectFactory.Create<RdbmsProvider>(
          ParamList.Create(
              rdbmsProviderDefinition,
              rdbmsProviderDefinition.ConnectionString,
              persistenceExtension,
              commandFactory,
              (Func<DbConnection>)(() => new SqlConnection())));
    }

    protected virtual IReadOnlyStorageProvider CreateReadOnlyStorageProvider (
        IPersistenceExtension persistenceExtension,
        RdbmsProviderDefinition rdbmsProviderDefinition,
        IRdbmsProviderCommandFactory commandFactory)
    {
      ArgumentNullException.ThrowIfNull(persistenceExtension);
      ArgumentNullException.ThrowIfNull(commandFactory);
      ArgumentNullException.ThrowIfNull(rdbmsProviderDefinition);

      return ObjectFactory.Create<RdbmsProvider>(
          ParamList.Create(
              rdbmsProviderDefinition,
              rdbmsProviderDefinition.ReadOnlyConnectionString,
              persistenceExtension,
              commandFactory,
              (Func<DbConnection>)(() => new SqlConnection())));
    }


    protected virtual IStoragePropertyDefinitionResolver CreateStoragePropertyDefinitionResolver (
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(persistenceModelProvider);

      return new StoragePropertyDefinitionResolver(persistenceModelProvider);
    }


    protected virtual IRdbmsProviderCommandFactory CreateStorageProviderCommandFactory (
      RdbmsProviderDefinition storageProviderDefinition,
      IStorageTypeInformationProvider storageTypeInformationProvider,
      IStorageNameProvider storageNameProvider,
      IRdbmsPersistenceModelProvider persistenceModelProvider,
      IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
      IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory,
      IDataParameterDefinitionFactory dataParameterDefinitionFactory)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(storageNameProvider);
      ArgumentNullException.ThrowIfNull(persistenceModelProvider);
      ArgumentNullException.ThrowIfNull(infrastructureStoragePropertyDefinitionProvider);
      ArgumentNullException.ThrowIfNull(storageTypeInformationProvider);
      ArgumentNullException.ThrowIfNull(dataStoragePropertyDefinitionFactory);
      ArgumentNullException.ThrowIfNull(dataParameterDefinitionFactory);

      var dataContainerValidator = CreateDataContainerValidator(storageProviderDefinition);

      var objectReaderFactory = new ObjectReaderFactory(
          persistenceModelProvider,
          infrastructureStoragePropertyDefinitionProvider,
          storageTypeInformationProvider,
          dataContainerValidator);

      var dbCommandBuilderFactory = CreateDbCommandBuilderFactory(storageProviderDefinition);

      return new RdbmsProviderCommandFactory(
          storageProviderDefinition,
          dbCommandBuilderFactory,
          persistenceModelProvider,
          objectReaderFactory,
          new TableDefinitionFinder(persistenceModelProvider),
          dataStoragePropertyDefinitionFactory,
          dataParameterDefinitionFactory);
    }

    protected virtual IDataContainerValidator CreateDataContainerValidator (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);

      return DataContainerValidator;
    }

    protected virtual ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(methodCallTransformerProvider);
      ArgumentNullException.ThrowIfNull(resultOperatorHandlerRegistry);
      ArgumentNullException.ThrowIfNull(persistenceModelProvider);

      var generator = new UniqueIdentifierGenerator();
      var resolver = CreateMappingResolver(storageProviderDefinition, persistenceModelProvider);
      var sqlPreparationStage = ObjectFactory.Create<DefaultSqlPreparationStage>(
          ParamList.Create(methodCallTransformerProvider, resultOperatorHandlerRegistry, generator));
      var mappingResolutionStage = ObjectFactory.Create<DefaultMappingResolutionStage>(ParamList.Create(resolver, generator));
      var sqlGenerationStage = ObjectFactory.Create<ExtendedSqlGenerationStage>(ParamList.Empty);
      var tableValuedParameterThreshold = GetTableValuedParameterThreshold();

      return new SqlQueryGenerator(sqlPreparationStage, mappingResolutionStage, sqlGenerationStage, tableValuedParameterThreshold);
    }

    /// <summary>
    /// Gets the number of elements in a collection that triggers the use of a table-valued parameter instead of single-element parameters in
    /// <see cref="TableValuedParameterSqlCommandBuilder"/>.
    /// </summary>
    protected virtual int GetTableValuedParameterThreshold ()
    {
      return 25;
    }

    protected virtual IMappingResolver CreateMappingResolver (
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(persistenceModelProvider);

      return new MappingResolver(new StorageSpecificExpressionResolver(persistenceModelProvider));
    }

    protected virtual IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionFactory,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactory,
        IStoragePropertyDefinitionResolver storagePropertyDefinitionResolver)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(infrastructureStoragePropertyDefinitionFactory);
      ArgumentNullException.ThrowIfNull(foreignKeyConstraintDefinitionFactory);
      ArgumentNullException.ThrowIfNull(storagePropertyDefinitionResolver);
      ArgumentNullException.ThrowIfNull(storageNameProvider);

      return new RdbmsStorageEntityDefinitionFactory(
          infrastructureStoragePropertyDefinitionFactory,
          foreignKeyConstraintDefinitionFactory,
          storagePropertyDefinitionResolver,
          storageNameProvider,
          storageProviderDefinition);
    }

    protected virtual IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionProvider (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(storageTypeInformationProvider);
      ArgumentNullException.ThrowIfNull(storageNameProvider);

      return new InfrastructureStoragePropertyDefinitionProvider(storageTypeInformationProvider, storageNameProvider);
    }

    protected virtual IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(storageNameProvider);
      ArgumentNullException.ThrowIfNull(storageTypeInformationProvider);

      var valueStoragePropertyDefinitionFactory = CreateValueStoragePropertyDefinitionFactory(
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);

      var relationStoragePropertyDefinitionFactory = CreateRelationStoragePropertyDefinitionFactory(
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);

      return new DataStoragePropertyDefinitionFactory(valueStoragePropertyDefinitionFactory, relationStoragePropertyDefinitionFactory);
    }

    protected virtual IRdbmsStructuredTypeDefinitionFinder CreateStructuredTypeDefinitionFinder (ISingleScalarStructuredTypeDefinitionProvider simpleStructuredTypeDefinitionProvider)
    {
      ArgumentNullException.ThrowIfNull(simpleStructuredTypeDefinitionProvider);

      return new StructuredTypeDefinitionFinderForCollectionOfScalars(simpleStructuredTypeDefinitionProvider);
    }

    protected virtual IQueryParameterRecordDefinitionFinder CreateQueryParameterRecordDefinitionFinder (IRdbmsStructuredTypeDefinitionFinder structuredTypeDefinitionFinder)
    {
      ArgumentNullException.ThrowIfNull(structuredTypeDefinitionFinder);

      return new SimpleTypeQueryParameterRecordDefinitionFinder(structuredTypeDefinitionFinder);
    }

    protected virtual IDataParameterDefinitionFactory CreateDataParameterDefinitionFactory (
        StorageProviderDefinition storageProviderDefinition,
        IQueryParameterRecordDefinitionFinder queryParameterRecordDefinitionFinder,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(queryParameterRecordDefinitionFinder);
      ArgumentNullException.ThrowIfNull(storageTypeInformationProvider);

      return new SqlTableValuedDataParameterDefinitionFactory(
          queryParameterRecordDefinitionFinder,
          new SqlFulltextDataParameterDefinitionFactory(
              new ObjectIDDataParameterDefinitionFactory(
                  storageProviderDefinition,
                  storageTypeInformationProvider,
                  StorageSettings,
                  new SimpleDataParameterDefinitionFactory(storageTypeInformationProvider)
              )
          )
      );
    }

    protected virtual ISingleScalarStructuredTypeDefinitionProvider CreateSingleScalarStructuredTypeDefinitionProvider (
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentNullException.ThrowIfNull(storageTypeInformationProvider);

      return new SingleScalarSqlTableTypeDefinitionProvider(storageTypeInformationProvider);
    }

    protected virtual ITableManipulationRecordDefinitionProvider CreateTableManipulationRecordDefinitionProvider (
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider)
    {
      ArgumentNullException.ThrowIfNull(storageTypeInformationProvider);
      ArgumentNullException.ThrowIfNull(infrastructureStoragePropertyDefinitionProvider);
      ArgumentNullException.ThrowIfNull(rdbmsPersistenceModelProvider);

      return new TableManipulationRecordDefinitionProvider(
          storageTypeInformationProvider,
          infrastructureStoragePropertyDefinitionProvider,
          rdbmsPersistenceModelProvider);
    }

    protected virtual IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(storageNameProvider);
      ArgumentNullException.ThrowIfNull(storageTypeInformationProvider);

      return new ValueStoragePropertyDefinitionFactory(storageTypeInformationProvider, storageNameProvider);
    }

    protected virtual IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(storageNameProvider);
      ArgumentNullException.ThrowIfNull(storageTypeInformationProvider);

      return new RelationStoragePropertyDefinitionFactory(
          storageProviderDefinition,
          false,
          storageNameProvider,
          storageTypeInformationProvider,
          StorageSettings);
    }

    protected virtual IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IRdbmsPersistenceModelProvider persistenceModelProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(storageNameProvider);
      ArgumentNullException.ThrowIfNull(persistenceModelProvider);
      ArgumentNullException.ThrowIfNull(infrastructureStoragePropertyDefinitionProvider);

      return new ForeignKeyConstraintDefinitionFactory(
          storageNameProvider,
          persistenceModelProvider,
          infrastructureStoragePropertyDefinitionProvider,
          DomainModelConstraintProvider);
    }
  }
}
