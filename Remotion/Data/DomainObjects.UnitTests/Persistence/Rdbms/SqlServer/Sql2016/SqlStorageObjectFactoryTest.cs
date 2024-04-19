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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Mixins;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Sql2016
{
  [TestFixture]
  public class SqlStorageObjectFactoryTest
  {
    private IRdbmsStorageObjectFactory _sqlStorageObjectFactory;
    private RdbmsProviderDefinition _rdbmsProviderDefinition;
    private Mock<IPersistenceExtension> _persistenceExtensionStub;
    private StorageSettings _storageSettings;
    private Mock<TableScriptBuilder> _tableBuilderStub;
    private Mock<ViewScriptBuilder> _viewBuilderStub;
    private Mock<ForeignKeyConstraintScriptBuilder> _constraintBuilderStub;
    private Mock<SqlIndexScriptElementFactory> _indexScriptElementFactoryStub;
    private Mock<IndexScriptBuilder> _indexBuilderStub;
    private Mock<SynonymScriptBuilder> _synonymBuilderStub;
    private Mock<IRdbmsPersistenceModelProvider> _rdbmsPersistenceModelProviderStub;
    private Mock<IStorageTypeInformationProvider> _storageTypeInformationProviderStub;
    private Mock<IDbCommandBuilderFactory> _dbCommandBuilderFactoryStub;
    private Mock<IStorageNameProvider> _storageNameProviderStub;
    private Mock<IInfrastructureStoragePropertyDefinitionProvider> _infrastructureStoragePropertyDefinitionProviderStub;
    private Mock<IDataStoragePropertyDefinitionFactory> _dataStoragePropertyDefinitionFactoryStub;
    private Mock<IValueStoragePropertyDefinitionFactory> _valueStoragePropertyDefinitonFactoryStub;
    private Mock<IRelationStoragePropertyDefinitionFactory> _relationStoragePropertyDefiniitonFactoryStub;
    private Mock<IMethodCallTransformerProvider> _methodCallTransformerProviderStub;
    private Mock<ResultOperatorHandlerRegistry> _resultOpertatorHandlerRegistryStub;
    private Mock<ISqlQueryGenerator> _sqlQueryGeneratorStub;
    private Mock<IForeignKeyConstraintDefinitionFactory> _foreignKeyConstraintDefinitionFactoryStub;
    private Mock<IStoragePropertyDefinitionResolver> _storagePropertyDefinitionResolverStub;

    [SetUp]
    public void SetUp ()
    {
      _persistenceExtensionStub = new Mock<IPersistenceExtension>();
      _tableBuilderStub = new Mock<TableScriptBuilder>(
          new Mock<ITableScriptElementFactory>().Object,
          new SqlCommentScriptElementFactory());
      _viewBuilderStub = new Mock<ViewScriptBuilder>(
          new Mock<IViewScriptElementFactory<TableDefinition>>().Object,
          new Mock<IViewScriptElementFactory<UnionViewDefinition>>().Object,
          new Mock<IViewScriptElementFactory<FilterViewDefinition>>().Object,
          new Mock<IViewScriptElementFactory<EmptyViewDefinition>>().Object,
          new SqlCommentScriptElementFactory());
      _constraintBuilderStub =
          new Mock<ForeignKeyConstraintScriptBuilder>(
              new Mock<IForeignKeyConstraintScriptElementFactory>().Object,
              new SqlCommentScriptElementFactory());
      _indexScriptElementFactoryStub = new Mock<SqlIndexScriptElementFactory>(
          new Mock<ISqlIndexDefinitionScriptElementFactory<SqlIndexDefinition>>().Object,
          new Mock<ISqlIndexDefinitionScriptElementFactory<SqlPrimaryXmlIndexDefinition>>().Object,
          new Mock<ISqlIndexDefinitionScriptElementFactory<SqlSecondaryXmlIndexDefinition>>().Object);
      _indexBuilderStub = new Mock<IndexScriptBuilder>(_indexScriptElementFactoryStub.Object, new SqlCommentScriptElementFactory());
      _synonymBuilderStub =
          new Mock<SynonymScriptBuilder>(
              new Mock<ISynonymScriptElementFactory<TableDefinition>>().Object,
              new Mock<ISynonymScriptElementFactory<UnionViewDefinition>>().Object,
              new Mock<ISynonymScriptElementFactory<FilterViewDefinition>>().Object,
              new Mock<ISynonymScriptElementFactory<EmptyViewDefinition>>().Object,
              new SqlCommentScriptElementFactory());
      _rdbmsPersistenceModelProviderStub = new Mock<IRdbmsPersistenceModelProvider>();
      _storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
      _dbCommandBuilderFactoryStub = new Mock<IDbCommandBuilderFactory>();
      _storageNameProviderStub = new Mock<IStorageNameProvider>();
      _infrastructureStoragePropertyDefinitionProviderStub = new Mock<IInfrastructureStoragePropertyDefinitionProvider>();
      _dataStoragePropertyDefinitionFactoryStub = new Mock<IDataStoragePropertyDefinitionFactory>();
      _valueStoragePropertyDefinitonFactoryStub = new Mock<IValueStoragePropertyDefinitionFactory>();
      _relationStoragePropertyDefiniitonFactoryStub = new Mock<IRelationStoragePropertyDefinitionFactory>();
      _methodCallTransformerProviderStub = new Mock<IMethodCallTransformerProvider>();
      _resultOpertatorHandlerRegistryStub = new Mock<ResultOperatorHandlerRegistry>() { CallBase = true };
      _sqlQueryGeneratorStub = new Mock<ISqlQueryGenerator>();
      _foreignKeyConstraintDefinitionFactoryStub = new Mock<IForeignKeyConstraintDefinitionFactory>();
      _storagePropertyDefinitionResolverStub = new Mock<IStoragePropertyDefinitionResolver>();

      _rdbmsProviderDefinition = new RdbmsProviderDefinition("TestDomain", Mock.Of<IRdbmsStorageObjectFactory>(), "ConnectionString");
      _storageSettings = new StorageSettings(_rdbmsProviderDefinition, new[] { _rdbmsProviderDefinition });

      _sqlStorageObjectFactory = new SqlStorageObjectFactory(
          _storageSettings,
          SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>(),
          SafeServiceLocator.Current.GetInstance<IDataContainerValidator>());
    }

    [Test]
    public void CreateStorageProvider ()
    {
      var result = _sqlStorageObjectFactory.CreateStorageProvider(_rdbmsProviderDefinition, _persistenceExtensionStub.Object);

      Assert.That(result, Is.TypeOf(typeof(RdbmsProvider)));
      Assert.That(result.PersistenceExtension, Is.SameAs(_persistenceExtensionStub.Object));
      Assert.That(result.StorageProviderDefinition, Is.SameAs(_rdbmsProviderDefinition));

      var commandFactory = (RdbmsProviderCommandFactory)PrivateInvoke.GetNonPublicProperty(result, "StorageProviderCommandFactory");
      Assert.That(commandFactory.DbCommandBuilderFactory, Is.TypeOf(typeof(SqlDbCommandBuilderFactory)));
      Assert.That(commandFactory.RdbmsPersistenceModelProvider, Is.TypeOf(typeof(RdbmsPersistenceModelProvider)));
      Assert.That(commandFactory.ObjectReaderFactory, Is.TypeOf(typeof(ObjectReaderFactory)));
      Assert.That(commandFactory.DataStoragePropertyDefinitionFactory, Is.TypeOf(typeof(DataStoragePropertyDefinitionFactory)));
      var dataStoragePropertyDefinitionFactory = (DataStoragePropertyDefinitionFactory)commandFactory.DataStoragePropertyDefinitionFactory;
      var relationStoragePropertyDefinitionFactory = (RelationStoragePropertyDefinitionFactory)dataStoragePropertyDefinitionFactory.RelationStoragePropertyDefinitionFactory;
      Assert.That(relationStoragePropertyDefinitionFactory.ForceClassIDColumnInForeignKeyProperties, Is.False);
    }

    [Test]
    public void CreateStorageProviderWithMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass(typeof(RdbmsProvider)).Clear().AddMixins(typeof(SqlProviderTestMixin)).EnterScope())
      {
        var result = _sqlStorageObjectFactory.CreateStorageProvider(_rdbmsProviderDefinition, _persistenceExtensionStub.Object);

        Assert.That(Mixin.Get<SqlProviderTestMixin>(result), Is.Not.Null);
      }
    }

    [Test]
    public void CreatePersistenceModelLoader ()
    {
      var result = _sqlStorageObjectFactory.CreatePersistenceModelLoader(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(RdbmsPersistenceModelLoader)));
      var rdbmsPersistenceModelLoader = (RdbmsPersistenceModelLoader)result;

      Assert.That(rdbmsPersistenceModelLoader.DataStoragePropertyDefinitionFactory, Is.TypeOf(typeof(DataStoragePropertyDefinitionFactory)));
      var dataStoragePropertyDefinitionFactory = (DataStoragePropertyDefinitionFactory)rdbmsPersistenceModelLoader.DataStoragePropertyDefinitionFactory;
      var relationStoragePropertyDefinitionFactory = (RelationStoragePropertyDefinitionFactory)dataStoragePropertyDefinitionFactory.RelationStoragePropertyDefinitionFactory;
      Assert.That(relationStoragePropertyDefinitionFactory.ForceClassIDColumnInForeignKeyProperties, Is.False);

      Assert.That(rdbmsPersistenceModelLoader.EntityDefinitionFactory, Is.TypeOf(typeof(RdbmsStorageEntityDefinitionFactory)));
      Assert.That(rdbmsPersistenceModelLoader.RdbmsPersistenceModelProvider, Is.TypeOf(typeof(RdbmsPersistenceModelProvider)));
      Assert.That(rdbmsPersistenceModelLoader.StorageNameProvider, Is.TypeOf(typeof(ReflectionBasedStorageNameProvider)));
    }

    [Test]
    public void CreateInfrastructureStoragePropertyDefinitionProvider ()
    {
      var result = _sqlStorageObjectFactory.CreateInfrastructureStoragePropertyDefinitionProvider(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(InfrastructureStoragePropertyDefinitionProvider)));
    }

    [Test]
    public void CreateEntityDefinitionFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory(
          _storageSettings,
          null,
          null,
          null,
          _storageNameProviderStub.Object,
          _infrastructureStoragePropertyDefinitionProviderStub.Object,
          null,
          null,
          null,
          null,
          _foreignKeyConstraintDefinitionFactoryStub.Object,
          _storagePropertyDefinitionResolverStub.Object);

      var result = testableSqlProviderFactory.CreateEntityDefinitionFactory(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(RdbmsStorageEntityDefinitionFactory)));
      var resultAsRdmsStorageEntityDefinitionFactory = (RdbmsStorageEntityDefinitionFactory)result;
      Assert.That(resultAsRdmsStorageEntityDefinitionFactory.StorageProviderDefinition, Is.SameAs(_rdbmsProviderDefinition));
      Assert.That(
          resultAsRdmsStorageEntityDefinitionFactory.InfrastructureStoragePropertyDefinitionProvider,
          Is.SameAs(_infrastructureStoragePropertyDefinitionProviderStub.Object));
      Assert.That(resultAsRdmsStorageEntityDefinitionFactory.StorageNameProvider, Is.SameAs(_storageNameProviderStub.Object));
      Assert.That(
          resultAsRdmsStorageEntityDefinitionFactory.ForeignKeyConstraintDefinitionFactory,
          Is.SameAs(_foreignKeyConstraintDefinitionFactoryStub.Object));
      Assert.That(resultAsRdmsStorageEntityDefinitionFactory.StoragePropertyDefinitionResolver, Is.SameAs(_storagePropertyDefinitionResolverStub.Object));
    }

    [Test]
    public void CreateDomainObjectQueryGenerator ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory(
          _storageSettings,
          null,
          _storageTypeInformationProviderStub.Object,
          null,
          null,
          null,
          null,
          null,
          null,
          _sqlQueryGeneratorStub.Object,
          null,
          null);
      var mappingConfiguration = new Mock<IMappingConfiguration>();

      var result = testableSqlProviderFactory.CreateDomainObjectQueryGenerator(
          _rdbmsProviderDefinition,
          _methodCallTransformerProviderStub.Object,
          _resultOpertatorHandlerRegistryStub.Object,
          mappingConfiguration.Object);

      Assert.That(result, Is.InstanceOf<DomainObjectQueryGenerator>());
      var resultAsDomainObjectQueryGenerator = (DomainObjectQueryGenerator)result;
      Assert.That(resultAsDomainObjectQueryGenerator.SqlQueryGenerator, Is.SameAs(_sqlQueryGeneratorStub.Object));
      Assert.That(resultAsDomainObjectQueryGenerator.StorageTypeInformationProvider, Is.SameAs(_storageTypeInformationProviderStub.Object));
      Assert.That(resultAsDomainObjectQueryGenerator.MappingConfiguration, Is.SameAs(mappingConfiguration.Object));
    }

    [Test]
    public void CreateSqlQueryGenerator ()
    {
      var result = _sqlStorageObjectFactory.CreateSqlQueryGenerator(
          _rdbmsProviderDefinition,
          _methodCallTransformerProviderStub.Object,
          _resultOpertatorHandlerRegistryStub.Object);

      Assert.That(result, Is.TypeOf<SqlQueryGenerator>());

      var sqlQueryGenerator = (SqlQueryGenerator)result;
      Assert.That(sqlQueryGenerator.PreparationStage, Is.TypeOf(typeof(DefaultSqlPreparationStage)));

      var defaultSqlPreparationStage = ((DefaultSqlPreparationStage)sqlQueryGenerator.PreparationStage);
      Assert.That(defaultSqlPreparationStage.MethodCallTransformerProvider, Is.SameAs(_methodCallTransformerProviderStub.Object));
      Assert.That(defaultSqlPreparationStage.ResultOperatorHandlerRegistry, Is.SameAs(_resultOpertatorHandlerRegistryStub.Object));
      Assert.That(defaultSqlPreparationStage.UniqueIdentifierGenerator, Is.TypeOf<UniqueIdentifierGenerator>());

      Assert.That(sqlQueryGenerator.ResolutionStage, Is.TypeOf<DefaultMappingResolutionStage>());
      var defaultMappingResolutionStage = ((DefaultMappingResolutionStage)sqlQueryGenerator.ResolutionStage);
      Assert.That(defaultMappingResolutionStage.Resolver, Is.TypeOf<MappingResolver>());

      var mappingResolver = ((MappingResolver)defaultMappingResolutionStage.Resolver);
      Assert.That(mappingResolver.StorageSpecificExpressionResolver, Is.TypeOf<StorageSpecificExpressionResolver>());
      Assert.That(
          ((StorageSpecificExpressionResolver)mappingResolver.StorageSpecificExpressionResolver).RdbmsPersistenceModelProvider,
          Is.TypeOf<RdbmsPersistenceModelProvider>());

      Assert.That(defaultMappingResolutionStage.UniqueIdentifierGenerator, Is.SameAs(defaultSqlPreparationStage.UniqueIdentifierGenerator));
      Assert.That(sqlQueryGenerator.GenerationStage, Is.TypeOf<ExtendedSqlGenerationStage>());
    }

    [Test]
    public void CreateDataStoragePropertyDefinitionFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory(
          _storageSettings,
          null,
          null,
          null,
          null,
          null,
          null,
          _valueStoragePropertyDefinitonFactoryStub.Object,
          _relationStoragePropertyDefiniitonFactoryStub.Object,
          null,
          null,
          null);

      var result = testableSqlProviderFactory.CreateDataStoragePropertyDefinitionFactory(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(DataStoragePropertyDefinitionFactory)));
      var resultAsDataStoragePropertyDefinitionFactory = (DataStoragePropertyDefinitionFactory)result;
      Assert.That(
          resultAsDataStoragePropertyDefinitionFactory.RelationStoragePropertyDefinitionFactory,
          Is.SameAs(_relationStoragePropertyDefiniitonFactoryStub.Object));
      Assert.That(
          resultAsDataStoragePropertyDefinitionFactory.ValueStoragePropertyDefinitionFactory,
          Is.SameAs(_valueStoragePropertyDefinitonFactoryStub.Object));
    }

    [Test]
    public void CreateDataParameterDefinitionFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory(
          _storageSettings,
          null,
          _storageTypeInformationProviderStub.Object,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null,
          null);

      var result = testableSqlProviderFactory.CreateDataParameterDefinitionFactory(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(ObjectIDDataParameterDefinitionFactory)));
      var objectIDDataParameterDefinitionFactory = result.As<ObjectIDDataParameterDefinitionFactory>();
      Assert.That(objectIDDataParameterDefinitionFactory.StorageProviderDefinition, Is.SameAs(_rdbmsProviderDefinition));
      Assert.That(objectIDDataParameterDefinitionFactory.StorageSettings, Is.SameAs(_storageSettings));
      Assert.That(objectIDDataParameterDefinitionFactory.StorageTypeInformationProvider, Is.SameAs(_storageTypeInformationProviderStub.Object));

      Assert.That(objectIDDataParameterDefinitionFactory.NextDataParameterDefinitionFactory, Is.InstanceOf<SimpleDataParameterDefinitionFactory>());
      var simpleDataParameterDefinitionFactory = objectIDDataParameterDefinitionFactory.NextDataParameterDefinitionFactory.As<SimpleDataParameterDefinitionFactory>();
      Assert.That(simpleDataParameterDefinitionFactory.StorageTypeInformationProvider, Is.SameAs(_storageTypeInformationProviderStub.Object));
    }

    [Test]
    public void CreateStorageProviderCommandFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory
          = new TestableSqlStorageObjectFactory(
              _storageSettings,
              _rdbmsPersistenceModelProviderStub.Object,
              null,
              _dbCommandBuilderFactoryStub.Object,
              null,
              null,
              _dataStoragePropertyDefinitionFactoryStub.Object,
              null,
              null,
              null,
              null,
              null);

      var result = testableSqlProviderFactory.CreateStorageProviderCommandFactory(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(RdbmsProviderCommandFactory)));
      var resultAsRdbmsProviderCommandFactory = (RdbmsProviderCommandFactory)result;
      Assert.That(resultAsRdbmsProviderCommandFactory.StorageProviderDefinition, Is.SameAs(_rdbmsProviderDefinition));
      Assert.That(resultAsRdbmsProviderCommandFactory.DbCommandBuilderFactory, Is.SameAs(_dbCommandBuilderFactoryStub.Object));
      Assert.That(resultAsRdbmsProviderCommandFactory.RdbmsPersistenceModelProvider, Is.SameAs(_rdbmsPersistenceModelProviderStub.Object));
      Assert.That(resultAsRdbmsProviderCommandFactory.DataStoragePropertyDefinitionFactory, Is.SameAs(_dataStoragePropertyDefinitionFactoryStub.Object));
      var objectReader = resultAsRdbmsProviderCommandFactory.ObjectReaderFactory.CreateDataContainerReader();
      Assert.That(objectReader, Is.TypeOf(typeof(DataContainerReader)));
      var expectedDataContainerValidator = SafeServiceLocator.Current.GetInstance<IDataContainerValidator>();
      Assert.That(((DataContainerReader)objectReader).DataContainerValidator, Is.SameAs(expectedDataContainerValidator));
    }

    [Test]
    public void CreateRelationStoragePropertyDefinitionFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory(
          _storageSettings,
          null,
          _storageTypeInformationProviderStub.Object,
          null,
          _storageNameProviderStub.Object,
          null,
          null,
          null,
          null,
          null,
          null,
          null);

      var result = testableSqlProviderFactory.CreateRelationStoragePropertyDefinitionFactory(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(RelationStoragePropertyDefinitionFactory)));
      var resultAsRelationStoragePropertyDefinitionFactory = (RelationStoragePropertyDefinitionFactory)result;
      Assert.That(resultAsRelationStoragePropertyDefinitionFactory.StorageProviderDefinition, Is.SameAs(_rdbmsProviderDefinition));
      Assert.That(resultAsRelationStoragePropertyDefinitionFactory.StorageNameProvider, Is.SameAs(_storageNameProviderStub.Object));
      Assert.That(resultAsRelationStoragePropertyDefinitionFactory.StorageTypeInformationProvider, Is.SameAs(_storageTypeInformationProviderStub.Object));
    }

    [Test]
    public void CreateValueStoragePropertyDefinitionFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory(
          _storageSettings,
          null,
          _storageTypeInformationProviderStub.Object,
          null,
          _storageNameProviderStub.Object,
          null,
          null,
          null,
          null,
          null,
          null,
          null);

      var result = testableSqlProviderFactory.CreateValueStoragePropertyDefinitionFactory(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(ValueStoragePropertyDefinitionFactory)));
      var resultAsValueStoragePropertyDefinitionFactory = (ValueStoragePropertyDefinitionFactory)result;
      Assert.That(resultAsValueStoragePropertyDefinitionFactory.StorageTypeInformationProvider, Is.SameAs(_storageTypeInformationProviderStub.Object));
      Assert.That(resultAsValueStoragePropertyDefinitionFactory.StorageNameProvider, Is.SameAs(_storageNameProviderStub.Object));
    }

    [Test]
    public void CreateForeignKeyConstraintDefinitionsFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory(
          _storageSettings,
          _rdbmsPersistenceModelProviderStub.Object,
          null,
          null,
          _storageNameProviderStub.Object,
          _infrastructureStoragePropertyDefinitionProviderStub.Object,
          null,
          null,
          null,
          null,
          null,
          null);

      var result = testableSqlProviderFactory.CreateForeignKeyConstraintDefinitionsFactory(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(ForeignKeyConstraintDefinitionFactory)));
      var resultAsForeignKeyConstraintDefinitionFactory = (ForeignKeyConstraintDefinitionFactory)result;
      Assert.That(resultAsForeignKeyConstraintDefinitionFactory.StorageNameProvider, Is.SameAs(_storageNameProviderStub.Object));
      Assert.That(
          resultAsForeignKeyConstraintDefinitionFactory.InfrastructureStoragePropertyDefinitionProvider,
          Is.SameAs(_infrastructureStoragePropertyDefinitionProviderStub.Object));
      Assert.That(resultAsForeignKeyConstraintDefinitionFactory.PersistenceModelProvider, Is.SameAs(_rdbmsPersistenceModelProviderStub.Object));
    }

    [Test]
    public void CreateStorageNameProvider ()
    {
      var result = _sqlStorageObjectFactory.CreateStorageNameProvider(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(ReflectionBasedStorageNameProvider)));
    }

    [Test]
    public void CreateDbCommandBuilderFactory ()
    {
      var result = _sqlStorageObjectFactory.CreateDbCommandBuilderFactory(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(SqlDbCommandBuilderFactory)));
    }

    [Test]
    public void CreateStorageTypeInformationProvider ()
    {
      var result = _sqlStorageObjectFactory.CreateStorageTypeInformationProvider(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator)));
      var decoratedResult = (SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator)result;
      Assert.That(decoratedResult.InnerStorageTypeInformationProvider, Is.TypeOf(typeof(SqlStorageTypeInformationProvider)));
    }

    [Test]
    public void CreateRdbmsPersistenceModelProvider ()
    {
      var result = _sqlStorageObjectFactory.CreateRdbmsPersistenceModelProvider(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(RdbmsPersistenceModelProvider)));
    }

    [Test]
    public void CreateSqlDialect ()
    {
      var result = _sqlStorageObjectFactory.CreateSqlDialect(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(SqlDialect)));
    }

    [Test]
    public void CreateTableBuilder ()
    {
      var result = _sqlStorageObjectFactory.CreateTableBuilder(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf<TableScriptBuilder>());

      var tableScriptBuilder = (TableScriptBuilder)result;
      Assert.That(tableScriptBuilder.ElementFactory, Is.TypeOf(typeof(SqlTableScriptElementFactory)));
    }

    [Test]
    public void CreateViewBuilder ()
    {
      var result = _sqlStorageObjectFactory.CreateViewBuilder(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf<ViewScriptBuilder>());

      var viewScriptBuilder = (ViewScriptBuilder)result;
      Assert.That(viewScriptBuilder.TableViewElementFactory, Is.TypeOf(typeof(SqlTableViewScriptElementFactory)));
      Assert.That(viewScriptBuilder.UnionViewElementFactory, Is.TypeOf(typeof(SqlUnionViewScriptElementFactory)));
      Assert.That(viewScriptBuilder.FilterViewElementFactory, Is.TypeOf(typeof(SqlFilterViewScriptElementFactory)));
      Assert.That(viewScriptBuilder.EmptyViewElementFactory, Is.TypeOf(typeof(SqlEmptyViewScriptElementFactory)));
    }

    [Test]
    public void CreateConstraintBuilder ()
    {
      var result = _sqlStorageObjectFactory.CreateConstraintBuilder(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf<ForeignKeyConstraintScriptBuilder>());

      var foreignKeyConstraintScriptBuilder = (ForeignKeyConstraintScriptBuilder)result;
      Assert.That(
          foreignKeyConstraintScriptBuilder.ForeignKeyConstraintElementFactory,
          Is.TypeOf(typeof(SqlForeignKeyConstraintScriptElementFactory)));
    }

    [Test]
    public void CreateIndexBuilder ()
    {
      var result = _sqlStorageObjectFactory.CreateIndexBuilder(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf<IndexScriptBuilder>());

      var indexScriptBuilder = (IndexScriptBuilder)result;
      Assert.That(indexScriptBuilder.IndexScriptElementFactory, Is.TypeOf(typeof(SqlIndexScriptElementFactory)));
    }

    [Test]
    public void CreateSynonymBuilder ()
    {
      var result = _sqlStorageObjectFactory.CreateSynonymBuilder(_rdbmsProviderDefinition);

      var synonymScriptBuilder = (SynonymScriptBuilder)result;
      Assert.That(synonymScriptBuilder.TableViewElementFactory, Is.TypeOf(typeof(SqlSynonymScriptElementFactory)));
      Assert.That(synonymScriptBuilder.UnionViewElementFactory, Is.TypeOf(typeof(SqlSynonymScriptElementFactory)));
      Assert.That(synonymScriptBuilder.FilterViewElementFactory, Is.TypeOf(typeof(SqlSynonymScriptElementFactory)));
      Assert.That(synonymScriptBuilder.EmptyViewElementFactory, Is.TypeOf(typeof(SqlSynonymScriptElementFactory)));
    }

    [Test]
    public void CreateSchemaScriptBuilder ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory(
          _storageSettings,
          _tableBuilderStub.Object,
          _viewBuilderStub.Object,
          _constraintBuilderStub.Object,
          _indexBuilderStub.Object,
          _synonymBuilderStub.Object);

      var result = testableSqlProviderFactory.CreateSchemaScriptBuilder(_rdbmsProviderDefinition);

      Assert.That(result, Is.TypeOf(typeof(SqlDatabaseSelectionScriptElementBuilder)));
      Assert.That(((SqlDatabaseSelectionScriptElementBuilder)result).InnerScriptBuilder, Is.TypeOf(typeof(CompositeScriptBuilder)));
      Assert.That(
          ((CompositeScriptBuilder)((SqlDatabaseSelectionScriptElementBuilder)result).InnerScriptBuilder).RdbmsProviderDefinition,
          Is.SameAs(_rdbmsProviderDefinition));
      Assert.That(
          ((CompositeScriptBuilder)((SqlDatabaseSelectionScriptElementBuilder)result).InnerScriptBuilder).ScriptBuilders,
          Is.EqualTo(
              new IScriptBuilder[]
              {
                  _tableBuilderStub.Object,
                  _constraintBuilderStub.Object,
                  _viewBuilderStub.Object,
                  _indexBuilderStub.Object,
                  _synonymBuilderStub.Object
              }));
    }

    [Test]
    public void CreateStorageProviderSerializer ()
    {
      var enumSerializerStub = new Mock<IEnumSerializer>();
      var testableSqlProviderFactory = new TestableSqlStorageObjectFactory(
          _storageSettings,
          null,
          enumSerializerStub.Object);

      var storageProviderSerializer = testableSqlProviderFactory.CreateStorageProviderSerializer(enumSerializerStub.Object);
      Assert.That(storageProviderSerializer, Is.InstanceOf<StorageProviderSerializer>());
      Assert.That(
          ((StorageProviderSerializer)storageProviderSerializer).ClassSerializer,
          Is.InstanceOf<ClassSerializer>());
    }

    [Test]
    public void CreateEnumSerializer ()
    {
      var testableSqlProviderFactory = new TestableSqlStorageObjectFactory(_storageSettings, null, null);

      var enumSerializer = testableSqlProviderFactory.CreateEnumSerializer();
      Assert.That(enumSerializer, Is.InstanceOf<ExtensibleEnumSerializerDecorator>());
    }
  }
}
