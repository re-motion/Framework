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
using System.Data;
using System.Data.SqlClient;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
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

    public SqlStorageObjectFactory (IStorageSettings storageSettings, ITypeConversionProvider typeConversionProvider, IDataContainerValidator dataContainerValidator)
    {
      ArgumentUtility.CheckNotNull("storageSettings", storageSettings);
      ArgumentUtility.CheckNotNull("typeConversionProvider", typeConversionProvider);
      ArgumentUtility.CheckNotNull("dataContainerValidator", dataContainerValidator);

      StorageSettings = storageSettings;
      TypeConversionProvider = typeConversionProvider;
      DataContainerValidator = dataContainerValidator;
    }

    public StorageProvider CreateStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);
      var rdbmsProviderDefinition =
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition>("storageProviderDefinition", storageProviderDefinition);

      var commandFactory = CreateStorageProviderCommandFactory(rdbmsProviderDefinition);
      return CreateStorageProvider(persistenceExtension, rdbmsProviderDefinition, commandFactory);
    }

    public virtual IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition)
    {
      var rdmsStorageProviderDefinition =
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition>("storageProviderDefinition", storageProviderDefinition);

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
          ArgumentUtility.CheckNotNullAndType<RdbmsProviderDefinition>("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);
      ArgumentUtility.CheckNotNull("mappingConfiguration", mappingConfiguration);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(rdmsStorageProviderDefinition);
      var sqlQueryGenerator = CreateSqlQueryGenerator(rdmsStorageProviderDefinition, methodCallTransformerProvider, resultOperatorHandlerRegistry);

      return ObjectFactory.Create<DomainObjectQueryGenerator>(
          ParamList.Create(sqlQueryGenerator, TypeConversionProvider, storageTypeInformationProvider, mappingConfiguration));
    }


    public virtual ISqlDialect CreateSqlDialect (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return new SqlDialect();
    }


    public virtual IStorageTypeInformationProvider CreateStorageTypeInformationProvider (RdbmsProviderDefinition rdmsStorageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("rdmsStorageProviderDefinition", rdmsStorageProviderDefinition);

      return new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator(new SqlStorageTypeInformationProvider());
    }

    public virtual IStorageNameProvider CreateStorageNameProvider (RdbmsProviderDefinition storageProviderDefiniton)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefiniton", storageProviderDefiniton);

      return new ReflectionBasedStorageNameProvider();
    }

    public virtual IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return new RdbmsPersistenceModelProvider();
    }

    public ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      var persistenceModelProvider = CreateRdbmsPersistenceModelProvider(storageProviderDefinition);

      return CreateSqlQueryGenerator(
          storageProviderDefinition,
          methodCallTransformerProvider,
          resultOperatorHandlerRegistry,
          persistenceModelProvider);
    }

    public IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (
        RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

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

      var dataParameterDefinitionFactory = CreateDataParameterDefinitionFactory(storageProviderDefinition, storageTypeInformationProvider);

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
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      var sqlDialect = CreateSqlDialect(storageProviderDefinition);
      return new SqlDbCommandBuilderFactory(sqlDialect);
    }


    public IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

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
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);

      return CreateInfrastructureStoragePropertyDefinitionProvider(storageProviderDefinition, storageTypeInformationProvider, storageNameProvider);
    }

    public IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);

      return CreateDataStoragePropertyDefinitionFactory(
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);
    }

    public IDataParameterDefinitionFactory CreateDataParameterDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);

      return CreateDataParameterDefinitionFactory(storageProviderDefinition, storageTypeInformationProvider);
    }

    public IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);
      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);

      return CreateRelationStoragePropertyDefinitionFactory(
          storageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider);
    }

    public IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      var storageTypeInformationProvider = CreateStorageTypeInformationProvider(storageProviderDefinition);
      var storageNameProvider = CreateStorageNameProvider(storageProviderDefinition);

      return CreateValueStoragePropertyDefinitionFactory(storageProviderDefinition, storageTypeInformationProvider, storageNameProvider);
    }

    public IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

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
      ArgumentUtility.CheckNotNull("enumSerializer", enumSerializer);
      return new StorageProviderSerializer(CreateClassSerializer(enumSerializer));
    }

    public virtual IClassSerializer CreateClassSerializer (IEnumSerializer enumSerializer)
    {
      ArgumentUtility.CheckNotNull("enumSerializer", enumSerializer);
      return new ClassSerializer(CreateTableSerializer(enumSerializer));
    }

    public virtual ITableSerializer CreateTableSerializer (IEnumSerializer enumSerializer)
    {
      ArgumentUtility.CheckNotNull("enumSerializer", enumSerializer);
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
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      var compositeScriptBuilder = new CompositeScriptBuilder(
          storageProviderDefinition,
          CreateScriptBuildersForSchemaScriptBuilder(storageProviderDefinition));

      return new SqlDatabaseSelectionScriptElementBuilder(compositeScriptBuilder, storageProviderDefinition.ConnectionString);
    }

    protected virtual IEnumerable<IScriptBuilder> CreateScriptBuildersForSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      yield return CreateTableBuilder(storageProviderDefinition);
      yield return CreateConstraintBuilder(storageProviderDefinition);
      yield return CreateViewBuilder(storageProviderDefinition);
      yield return CreateIndexBuilder(storageProviderDefinition);
      yield return CreateSynonymBuilder(storageProviderDefinition);
    }

    public virtual IScriptBuilder CreateTableBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return new TableScriptBuilder(new SqlTableScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    public virtual IScriptBuilder CreateViewBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return new ViewScriptBuilder(
          new SqlTableViewScriptElementFactory(),
          new SqlUnionViewScriptElementFactory(),
          new SqlFilterViewScriptElementFactory(),
          new SqlEmptyViewScriptElementFactory(),
          new SqlCommentScriptElementFactory());
    }

    public virtual IScriptBuilder CreateConstraintBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return new ForeignKeyConstraintScriptBuilder(new SqlForeignKeyConstraintScriptElementFactory(), new SqlCommentScriptElementFactory());
    }

    public virtual IScriptBuilder CreateIndexBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return new IndexScriptBuilder(
          new SqlIndexScriptElementFactory(
              new SqlIndexDefinitionScriptElementFactory(),
              new SqlPrimaryXmlIndexDefinitionScriptElementFactory(),
              new SqlSecondaryXmlIndexDefinitionScriptElementFactory()),
          new SqlCommentScriptElementFactory());
    }

    public virtual IScriptBuilder CreateSynonymBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      var sqlSynonymScriptElementFactory = new SqlSynonymScriptElementFactory();
      return new SynonymScriptBuilder(
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          sqlSynonymScriptElementFactory,
          new SqlCommentScriptElementFactory());
    }


    protected virtual StorageProvider CreateStorageProvider (
        IPersistenceExtension persistenceExtension,
        RdbmsProviderDefinition rdbmsProviderDefinition,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory)
    {
      ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);
      ArgumentUtility.CheckNotNull("commandFactory", commandFactory);
      ArgumentUtility.CheckNotNull("rdbmsProviderDefinition", rdbmsProviderDefinition);

      return ObjectFactory.Create<RdbmsProvider>(
          ParamList.Create(
              rdbmsProviderDefinition,
              persistenceExtension,
              commandFactory,
              (Func<IDbConnection>)(() => new SqlConnection())));
    }


    protected virtual IStoragePropertyDefinitionResolver CreateStoragePropertyDefinitionResolver (
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("persistenceModelProvider", persistenceModelProvider);

      return new StoragePropertyDefinitionResolver(persistenceModelProvider);
    }


    protected virtual IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> CreateStorageProviderCommandFactory (
      RdbmsProviderDefinition storageProviderDefinition,
      IStorageTypeInformationProvider storageTypeInformationProvider,
      IStorageNameProvider storageNameProvider,
      IRdbmsPersistenceModelProvider persistenceModelProvider,
      IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
      IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory,
      IDataParameterDefinitionFactory dataParameterDefinitionFactory)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull("persistenceModelProvider", persistenceModelProvider);
      ArgumentUtility.CheckNotNull("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull("dataStoragePropertyDefinitionFactory", dataStoragePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull("dataParameterDefinitionFactory", dataParameterDefinitionFactory);

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
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return DataContainerValidator;
    }

    protected virtual ISqlQueryGenerator CreateSqlQueryGenerator (
        RdbmsProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);
      ArgumentUtility.CheckNotNull("persistenceModelProvider", persistenceModelProvider);

      var generator = new UniqueIdentifierGenerator();
      var resolver = CreateMappingResolver(storageProviderDefinition, persistenceModelProvider);
      var sqlPreparationStage = ObjectFactory.Create<DefaultSqlPreparationStage>(
          ParamList.Create(methodCallTransformerProvider, resultOperatorHandlerRegistry, generator));
      var mappingResolutionStage = ObjectFactory.Create<DefaultMappingResolutionStage>(ParamList.Create(resolver, generator));
      var sqlGenerationStage = ObjectFactory.Create<ExtendedSqlGenerationStage>(ParamList.Empty);

      return new SqlQueryGenerator(sqlPreparationStage, mappingResolutionStage, sqlGenerationStage);
    }

    protected virtual IMappingResolver CreateMappingResolver (
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("persistenceModelProvider", persistenceModelProvider);

      return new MappingResolver(new StorageSpecificExpressionResolver(persistenceModelProvider));
    }

    protected virtual IRdbmsStorageEntityDefinitionFactory CreateEntityDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionFactory,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactory,
        IStoragePropertyDefinitionResolver storagePropertyDefinitionResolver)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("infrastructureStoragePropertyDefinitionFactory", infrastructureStoragePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull("foreignKeyConstraintDefinitionFactory", foreignKeyConstraintDefinitionFactory);
      ArgumentUtility.CheckNotNull("storagePropertyDefinitionResolver", storagePropertyDefinitionResolver);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);

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
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);

      return new InfrastructureStoragePropertyDefinitionProvider(storageTypeInformationProvider, storageNameProvider);
    }

    protected virtual IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull("storageTypeInformationProvider", storageTypeInformationProvider);

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

    protected virtual IDataParameterDefinitionFactory CreateDataParameterDefinitionFactory (
        StorageProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("storageTypeInformationProvider", storageTypeInformationProvider);

      return new SqlFulltextDataParameterDefinitionFactory(
          new ObjectIDDataParameterDefinitionFactory(
          storageProviderDefinition,
          storageTypeInformationProvider,
          StorageSettings,
          new SimpleDataParameterDefinitionFactory(storageTypeInformationProvider)
          )
      );
    }

    protected virtual IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull("storageTypeInformationProvider", storageTypeInformationProvider);

      return new ValueStoragePropertyDefinitionFactory(storageTypeInformationProvider, storageNameProvider);
    }

    protected virtual IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull("storageTypeInformationProvider", storageTypeInformationProvider);

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
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull("persistenceModelProvider", persistenceModelProvider);
      ArgumentUtility.CheckNotNull("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);

      return new ForeignKeyConstraintDefinitionFactory(
          storageNameProvider,
          persistenceModelProvider,
          infrastructureStoragePropertyDefinitionProvider);
    }
  }
}
