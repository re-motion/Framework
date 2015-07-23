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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Mixins;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Sql2005
{
  [TestFixture]
  public class SqlStorageObjectFactoryTest
  {
    private IRdbmsStorageObjectFactory _sqlProviderFactory;
    private RdbmsProviderDefinition _rdbmsProviderDefinition;
    private IPersistenceExtension _persistenceExtensionStub;
    private StorageGroupBasedStorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    private TableScriptBuilder _tableBuilderStub;
    private ViewScriptBuilder _viewBuilderStub;
    private ForeignKeyConstraintScriptBuilder _constraintBuilderStub;
    private SqlIndexScriptElementFactory _indexScriptElementFactoryStub;
    private IndexScriptBuilder _indexBuilderStub;
    private SynonymScriptBuilder _synonymBuilderStub;
    private IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProviderStub;
    private IStorageTypeInformationProvider _storageTypeInformationProviderStub;
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStub;
    private IStorageNameProvider _storageNameProviderStub;
    private IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProviderStub;
    private IDataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactoryStub;
    private IValueStoragePropertyDefinitionFactory _valueStoragePropertyDefinitonFactoryStub;
    private IRelationStoragePropertyDefinitionFactory _relationStoragePropertyDefiniitonFactoryStub;
    private IMethodCallTransformerProvider _methodCallTransformerProviderStub;
    private ResultOperatorHandlerRegistry _resultOpertatorHandlerRegistryStub;
    private ISqlQueryGenerator _sqlQueryGeneratorStub;
    private IForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactoryStub;
    private IStoragePropertyDefinitionResolver _storagePropertyDefinitionResolverStub;

    [SetUp]
    public void SetUp ()
    {
      _rdbmsProviderDefinition = new RdbmsProviderDefinition ("TestDomain", new SqlStorageObjectFactory(), "ConnectionString");
      _sqlProviderFactory = new SqlStorageObjectFactory();
      _persistenceExtensionStub = MockRepository.GenerateStub<IPersistenceExtension>();
      _storageProviderDefinitionFinder = new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);

      _tableBuilderStub = MockRepository.GenerateStub<TableScriptBuilder> (
          MockRepository.GenerateStub<ITableScriptElementFactory>(), new SqlCommentScriptElementFactory());
      _viewBuilderStub = MockRepository.GenerateStub<ViewScriptBuilder> (
          MockRepository.GenerateStub<IViewScriptElementFactory<TableDefinition>>(),
          MockRepository.GenerateStub<IViewScriptElementFactory<UnionViewDefinition>>(),
          MockRepository.GenerateStub<IViewScriptElementFactory<FilterViewDefinition>>(),
          MockRepository.GenerateStub<IViewScriptElementFactory<EmptyViewDefinition>>(),
          new SqlCommentScriptElementFactory());
      _constraintBuilderStub =
          MockRepository.GenerateStub<ForeignKeyConstraintScriptBuilder> (
              MockRepository.GenerateStub<IForeignKeyConstraintScriptElementFactory>(), new SqlCommentScriptElementFactory());
      _indexScriptElementFactoryStub = MockRepository.GenerateStub<SqlIndexScriptElementFactory> (
          MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlIndexDefinition>>(),
          MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlPrimaryXmlIndexDefinition>>(),
          MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlSecondaryXmlIndexDefinition>>());
      _indexBuilderStub = MockRepository.GenerateStub<IndexScriptBuilder> (_indexScriptElementFactoryStub, new SqlCommentScriptElementFactory());
      _synonymBuilderStub =
          MockRepository.GenerateStub<SynonymScriptBuilder> (
              MockRepository.GenerateStub<ISynonymScriptElementFactory<TableDefinition>>(),
              MockRepository.GenerateStub<ISynonymScriptElementFactory<UnionViewDefinition>>(),
              MockRepository.GenerateStub<ISynonymScriptElementFactory<FilterViewDefinition>>(),
              MockRepository.GenerateStub<ISynonymScriptElementFactory<EmptyViewDefinition>>(),
              new SqlCommentScriptElementFactory());
      _rdbmsPersistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();
      _storageTypeInformationProviderStub = MockRepository.GenerateStub<IStorageTypeInformationProvider>();
      _dbCommandBuilderFactoryStub = MockRepository.GenerateStub<IDbCommandBuilderFactory>();
      MockRepository.GeneratePartialMock<SqlSynonymScriptElementFactory>();
      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();
      _infrastructureStoragePropertyDefinitionProviderStub = MockRepository.GenerateStub<IInfrastructureStoragePropertyDefinitionProvider>();
      _dataStoragePropertyDefinitionFactoryStub = MockRepository.GenerateStub<IDataStoragePropertyDefinitionFactory>();
      _valueStoragePropertyDefinitonFactoryStub = MockRepository.GenerateStub<IValueStoragePropertyDefinitionFactory>();
      _relationStoragePropertyDefiniitonFactoryStub = MockRepository.GenerateStub<IRelationStoragePropertyDefinitionFactory>();
      _methodCallTransformerProviderStub = MockRepository.GenerateStub<IMethodCallTransformerProvider>();
      _resultOpertatorHandlerRegistryStub = MockRepository.GeneratePartialMock<ResultOperatorHandlerRegistry>();
      _sqlQueryGeneratorStub = MockRepository.GenerateStub<ISqlQueryGenerator>();
      _foreignKeyConstraintDefinitionFactoryStub = MockRepository.GenerateStub<IForeignKeyConstraintDefinitionFactory>();
      _storagePropertyDefinitionResolverStub = MockRepository.GenerateStub<IStoragePropertyDefinitionResolver>();
    }

    [Test]
    public void CreateStorageProvider ()
    {
      var result = _sqlProviderFactory.CreateStorageProvider (_rdbmsProviderDefinition, _persistenceExtensionStub);

      Assert.That (result, Is.TypeOf (typeof (RdbmsProvider)));
      Assert.That (result.PersistenceExtension, Is.SameAs (_persistenceExtensionStub));
      Assert.That (result.StorageProviderDefinition, Is.SameAs (_rdbmsProviderDefinition));

      var commandFactory = (RdbmsProviderCommandFactory) PrivateInvoke.GetNonPublicProperty (result, "StorageProviderCommandFactory");
      Assert.That (commandFactory.DbCommandBuilderFactory, Is.TypeOf (typeof (SqlDbCommandBuilderFactory)));
      Assert.That (commandFactory.RdbmsPersistenceModelProvider, Is.TypeOf (typeof (RdbmsPersistenceModelProvider)));
      Assert.That (commandFactory.ObjectReaderFactory, Is.TypeOf (typeof (ObjectReaderFactory)));
      Assert.That (commandFactory.DataStoragePropertyDefinitionFactory, Is.TypeOf (typeof (DataStoragePropertyDefinitionFactory)));
      var dataStoragePropertyDefinitionFactory = (DataStoragePropertyDefinitionFactory) commandFactory.DataStoragePropertyDefinitionFactory;
      var relationStoragePropertyDefinitionFactory =
          (RelationStoragePropertyDefinitionFactory) dataStoragePropertyDefinitionFactory.RelationStoragePropertyDefinitionFactory;
      Assert.That (relationStoragePropertyDefinitionFactory.ForceClassIDColumnInForeignKeyProperties, Is.False);
    }

    [Test]
    public void CreateStorageProviderWithMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (RdbmsProvider)).Clear().AddMixins (typeof (SqlProviderTestMixin)).EnterScope())
      {
        var result = _sqlProviderFactory.CreateStorageProvider (_rdbmsProviderDefinition, _persistenceExtensionStub);

        Assert.That (Mixin.Get<SqlProviderTestMixin> (result), Is.Not.Null);
      }
    }

    [Test]
    public void CreatePersistenceModelLoader ()
    {
      var result = _sqlProviderFactory.CreatePersistenceModelLoader (_rdbmsProviderDefinition, _storageProviderDefinitionFinder);

      Assert.That (result, Is.TypeOf (typeof (RdbmsPersistenceModelLoader)));
      var rdbmsPersistenceModelLoader = (RdbmsPersistenceModelLoader) result;

      Assert.That (rdbmsPersistenceModelLoader.DataStoragePropertyDefinitionFactory, Is.TypeOf (typeof (DataStoragePropertyDefinitionFactory)));
      var dataStoragePropertyDefinitionFactory =
          (DataStoragePropertyDefinitionFactory) rdbmsPersistenceModelLoader.DataStoragePropertyDefinitionFactory;
      var relationStoragePropertyDefinitionFactory =
          (RelationStoragePropertyDefinitionFactory) dataStoragePropertyDefinitionFactory.RelationStoragePropertyDefinitionFactory;
      Assert.That (relationStoragePropertyDefinitionFactory.ForceClassIDColumnInForeignKeyProperties, Is.False);

      Assert.That (rdbmsPersistenceModelLoader.EntityDefinitionFactory, Is.TypeOf (typeof (RdbmsStorageEntityDefinitionFactory)));
      Assert.That (rdbmsPersistenceModelLoader.RdbmsPersistenceModelProvider, Is.TypeOf (typeof (RdbmsPersistenceModelProvider)));
      Assert.That (rdbmsPersistenceModelLoader.StorageNameProvider, Is.TypeOf (typeof (ReflectionBasedStorageNameProvider)));
    }

    [Test]
    public void CreateInfrastructureStoragePropertyDefinitionProvider ()
    {
      var result = _sqlProviderFactory.CreateInfrastructureStoragePropertyDefinitionProvider (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (InfrastructureStoragePropertyDefinitionProvider)));
    }

    [Test]
    public void CreateEntityDefinitionFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory (
          null,
          null,
          null,
          _storageNameProviderStub,
          _infrastructureStoragePropertyDefinitionProviderStub,
          null,
          null,
          null,
          null,
          _foreignKeyConstraintDefinitionFactoryStub,
          _storagePropertyDefinitionResolverStub);

      var result = testableSqlProviderFactory.CreateEntityDefinitionFactory (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (RdbmsStorageEntityDefinitionFactory)));
      var resultAsRdmsStorageEntityDefinitionFactory = (RdbmsStorageEntityDefinitionFactory) result;
      Assert.That (resultAsRdmsStorageEntityDefinitionFactory.StorageProviderDefinition, Is.SameAs (_rdbmsProviderDefinition));
      Assert.That (
          resultAsRdmsStorageEntityDefinitionFactory.InfrastructureStoragePropertyDefinitionProvider,
          Is.SameAs (_infrastructureStoragePropertyDefinitionProviderStub));
      Assert.That (resultAsRdmsStorageEntityDefinitionFactory.StorageNameProvider, Is.SameAs (_storageNameProviderStub));
      Assert.That (
          resultAsRdmsStorageEntityDefinitionFactory.ForeignKeyConstraintDefinitionFactory, Is.SameAs (_foreignKeyConstraintDefinitionFactoryStub));
      Assert.That (resultAsRdmsStorageEntityDefinitionFactory.StoragePropertyDefinitionResolver, Is.SameAs (_storagePropertyDefinitionResolverStub));
    }

    [Test]
    public void CreateDomainObjectQueryGenerator ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory (
          null, _storageTypeInformationProviderStub, null, null, null, null, null, null, _sqlQueryGeneratorStub, null, null);
      var mappingConfiguration = MockRepository.GenerateStub<IMappingConfiguration>();

      var result = testableSqlProviderFactory.CreateDomainObjectQueryGenerator (
          _rdbmsProviderDefinition, 
          _methodCallTransformerProviderStub, 
          _resultOpertatorHandlerRegistryStub,
          mappingConfiguration);

      Assert.That (result, Is.InstanceOf<DomainObjectQueryGenerator>());
      var resultAsDomainObjectQueryGenerator = (DomainObjectQueryGenerator) result;
      Assert.That (resultAsDomainObjectQueryGenerator.SqlQueryGenerator, Is.SameAs (_sqlQueryGeneratorStub));
      Assert.That (resultAsDomainObjectQueryGenerator.StorageTypeInformationProvider, Is.SameAs (_storageTypeInformationProviderStub));
      Assert.That (resultAsDomainObjectQueryGenerator.MappingConfiguration, Is.SameAs (mappingConfiguration));
    }

    [Test]
    public void CreateSqlQueryGenerator ()
    {
      var result = _sqlProviderFactory.CreateSqlQueryGenerator (
          _rdbmsProviderDefinition, _methodCallTransformerProviderStub, _resultOpertatorHandlerRegistryStub);

      Assert.That (result, Is.TypeOf<SqlQueryGenerator>());
      
      var sqlQueryGenerator = (SqlQueryGenerator) result;
      Assert.That (sqlQueryGenerator.PreparationStage, Is.TypeOf (typeof (DefaultSqlPreparationStage)));
      
      var defaultSqlPreparationStage = ((DefaultSqlPreparationStage) sqlQueryGenerator.PreparationStage);
      Assert.That (defaultSqlPreparationStage.MethodCallTransformerProvider, Is.SameAs (_methodCallTransformerProviderStub));
      Assert.That (defaultSqlPreparationStage.ResultOperatorHandlerRegistry, Is.SameAs (_resultOpertatorHandlerRegistryStub));
      Assert.That (defaultSqlPreparationStage.UniqueIdentifierGenerator, Is.TypeOf<UniqueIdentifierGenerator>());

      Assert.That (sqlQueryGenerator.ResolutionStage, Is.TypeOf<DefaultMappingResolutionStage>());
      var defaultMappingResolutionStage = ((DefaultMappingResolutionStage) sqlQueryGenerator.ResolutionStage);
      Assert.That (defaultMappingResolutionStage.Resolver, Is.TypeOf<MappingResolver>());
      
      var mappingResolver = ((MappingResolver) defaultMappingResolutionStage.Resolver);
      Assert.That (mappingResolver.StorageSpecificExpressionResolver, Is.TypeOf<StorageSpecificExpressionResolver>());
      Assert.That (
          ((StorageSpecificExpressionResolver) mappingResolver.StorageSpecificExpressionResolver).RdbmsPersistenceModelProvider,
          Is.TypeOf<RdbmsPersistenceModelProvider>());
      
      Assert.That (defaultMappingResolutionStage.UniqueIdentifierGenerator, Is.SameAs (defaultSqlPreparationStage.UniqueIdentifierGenerator));
      Assert.That (sqlQueryGenerator.GenerationStage, Is.TypeOf<ExtendedSqlGenerationStage>());
    }

    [Test]
    public void CreateDataStoragePropertyDefinitionFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory (
          null,
          null,
          null,
          null,
          null,
          null,
          _valueStoragePropertyDefinitonFactoryStub,
          _relationStoragePropertyDefiniitonFactoryStub,
          null,
          null,
          null);

      var result = testableSqlProviderFactory.CreateDataStoragePropertyDefinitionFactory (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (DataStoragePropertyDefinitionFactory)));
      var resultAsDataStoragePropertyDefinitionFactory = (DataStoragePropertyDefinitionFactory) result;
      Assert.That (
          resultAsDataStoragePropertyDefinitionFactory.RelationStoragePropertyDefinitionFactory,
          Is.SameAs (_relationStoragePropertyDefiniitonFactoryStub));
      Assert.That (
          resultAsDataStoragePropertyDefinitionFactory.ValueStoragePropertyDefinitionFactory, Is.SameAs (_valueStoragePropertyDefinitonFactoryStub));
    }

    [Test]
    public void CreateStorageProviderCommandFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory (
          _rdbmsPersistenceModelProviderStub,
          null,
          _dbCommandBuilderFactoryStub,
          null,
          null,
          _dataStoragePropertyDefinitionFactoryStub,
          null,
          null,
          null,
          null,
          null);

      var result = testableSqlProviderFactory.CreateStorageProviderCommandFactory (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (RdbmsProviderCommandFactory)));
      var resultAsRdbmsProviderCommandFactory = (RdbmsProviderCommandFactory) result;
      Assert.That (resultAsRdbmsProviderCommandFactory.StorageProviderDefinition, Is.SameAs (_rdbmsProviderDefinition));
      Assert.That (resultAsRdbmsProviderCommandFactory.DbCommandBuilderFactory, Is.SameAs (_dbCommandBuilderFactoryStub));
      Assert.That (resultAsRdbmsProviderCommandFactory.RdbmsPersistenceModelProvider, Is.SameAs (_rdbmsPersistenceModelProviderStub));
      Assert.That (resultAsRdbmsProviderCommandFactory.DataStoragePropertyDefinitionFactory, Is.SameAs (_dataStoragePropertyDefinitionFactoryStub));
      var objectReader = resultAsRdbmsProviderCommandFactory.ObjectReaderFactory.CreateDataContainerReader();
      Assert.That (objectReader, Is.TypeOf (typeof (DataContainerReader)));
      var expectedDataContainerValidator = SafeServiceLocator.Current.GetInstance<IDataContainerValidator>();
      Assert.That (((DataContainerReader)objectReader).DataContainerValidator, Is.SameAs (expectedDataContainerValidator));
    }

    [Test]
    public void CreateRelationStoragePropertyDefinitionFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory (
          null, _storageTypeInformationProviderStub, null, _storageNameProviderStub, null, null, null, null, null, null, null);

      var result = testableSqlProviderFactory.CreateRelationStoragePropertyDefinitionFactory (
          _rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (RelationStoragePropertyDefinitionFactory)));
      var resultAsRelationStoragePropertyDefinitionFactory = (RelationStoragePropertyDefinitionFactory) result;
      Assert.That (resultAsRelationStoragePropertyDefinitionFactory.StorageProviderDefinition, Is.SameAs (_rdbmsProviderDefinition));
      Assert.That (resultAsRelationStoragePropertyDefinitionFactory.StorageNameProvider, Is.SameAs (_storageNameProviderStub));
      Assert.That (resultAsRelationStoragePropertyDefinitionFactory.StorageTypeInformationProvider, Is.SameAs (_storageTypeInformationProviderStub));
    }

    [Test]
    public void CreateValueStoragePropertyDefinitionFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory (
          null, _storageTypeInformationProviderStub, null, _storageNameProviderStub, null, null, null, null, null, null, null);

      var result = testableSqlProviderFactory.CreateValueStoragePropertyDefinitionFactory (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (ValueStoragePropertyDefinitionFactory)));
      var resultAsValueStoragePropertyDefinitionFactory = (ValueStoragePropertyDefinitionFactory) result;
      Assert.That (resultAsValueStoragePropertyDefinitionFactory.StorageTypeInformationProvider, Is.SameAs (_storageTypeInformationProviderStub));
      Assert.That (resultAsValueStoragePropertyDefinitionFactory.StorageNameProvider, Is.SameAs (_storageNameProviderStub));
    }

    [Test]
    public void CreateForeignKeyConstraintDefinitionsFactory ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory (
          _rdbmsPersistenceModelProviderStub,
          null,
          null,
          _storageNameProviderStub,
          _infrastructureStoragePropertyDefinitionProviderStub,
          null,
          null,
          null,
          null,
          null,
          null);

      var result = testableSqlProviderFactory.CreateForeignKeyConstraintDefinitionsFactory (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (ForeignKeyConstraintDefinitionFactory)));
      var resultAsForeignKeyConstraintDefinitionFactory = (ForeignKeyConstraintDefinitionFactory) result;
      Assert.That (resultAsForeignKeyConstraintDefinitionFactory.StorageNameProvider, Is.SameAs (_storageNameProviderStub));
      Assert.That (
          resultAsForeignKeyConstraintDefinitionFactory.InfrastructureStoragePropertyDefinitionProvider,
          Is.SameAs (_infrastructureStoragePropertyDefinitionProviderStub));
      Assert.That (resultAsForeignKeyConstraintDefinitionFactory.PersistenceModelProvider, Is.SameAs (_rdbmsPersistenceModelProviderStub));
    }

    [Test]
    public void CreateStorageNameProvider ()
    {
      var result = _sqlProviderFactory.CreateStorageNameProvider (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (ReflectionBasedStorageNameProvider)));
    }

    [Test]
    public void CreateDbCommandBuilderFactory ()
    {
      var result = _sqlProviderFactory.CreateDbCommandBuilderFactory (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (SqlDbCommandBuilderFactory)));
    }

    [Test]
    public void CreateStorageTypeInformationProvider ()
    {
      var result = _sqlProviderFactory.CreateStorageTypeInformationProvider (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (SqlStorageTypeInformationProvider)));
    }

    [Test]
    public void CreateRdbmsPersistenceModelProvider ()
    {
      var result = _sqlProviderFactory.CreateRdbmsPersistenceModelProvider (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (RdbmsPersistenceModelProvider)));
    }

    [Test]
    public void CreateSqlDialect ()
    {
      var result = _sqlProviderFactory.CreateSqlDialect (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (SqlDialect)));
    }

    [Test]
    public void CreateTableBuilder ()
    {
      var result = _sqlProviderFactory.CreateTableBuilder (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf<TableScriptBuilder>());

      var tableScriptBuilder = (TableScriptBuilder) result;
      Assert.That (tableScriptBuilder.ElementFactory, Is.TypeOf (typeof (SqlTableScriptElementFactory)));
    }

    [Test]
    public void CreateViewBuilder ()
    {
      var result = _sqlProviderFactory.CreateViewBuilder (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf<ViewScriptBuilder>());

      var viewScriptBuilder = (ViewScriptBuilder) result;
      Assert.That (viewScriptBuilder.TableViewElementFactory, Is.TypeOf (typeof (SqlTableViewScriptElementFactory)));
      Assert.That (viewScriptBuilder.UnionViewElementFactory, Is.TypeOf (typeof (SqlUnionViewScriptElementFactory)));
      Assert.That (viewScriptBuilder.FilterViewElementFactory, Is.TypeOf (typeof (SqlFilterViewScriptElementFactory)));
      Assert.That (viewScriptBuilder.EmptyViewElementFactory, Is.TypeOf (typeof (SqlEmptyViewScriptElementFactory)));
    }

    [Test]
    public void CreateConstraintBuilder ()
    {
      var result = _sqlProviderFactory.CreateConstraintBuilder (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf<ForeignKeyConstraintScriptBuilder>());

      var foreignKeyConstraintScriptBuilder = (ForeignKeyConstraintScriptBuilder) result;
      Assert.That (
          foreignKeyConstraintScriptBuilder.ForeignKeyConstraintElementFactory,
          Is.TypeOf (typeof (SqlForeignKeyConstraintScriptElementFactory)));
    }

    [Test]
    public void CreateIndexBuilder ()
    {
      var result = _sqlProviderFactory.CreateIndexBuilder (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf<IndexScriptBuilder>());

      var indexScriptBuilder = (IndexScriptBuilder) result;
      Assert.That (indexScriptBuilder.IndexScriptElementFactory, Is.TypeOf (typeof (SqlIndexScriptElementFactory)));
    }

    [Test]
    public void CreateSynonymBuilder ()
    {
      var result = _sqlProviderFactory.CreateSynonymBuilder (_rdbmsProviderDefinition);

      var synonymScriptBuilder = (SynonymScriptBuilder) result;
      Assert.That (synonymScriptBuilder.TableViewElementFactory, Is.TypeOf (typeof (SqlSynonymScriptElementFactory)));
      Assert.That (synonymScriptBuilder.UnionViewElementFactory, Is.TypeOf (typeof (SqlSynonymScriptElementFactory)));
      Assert.That (synonymScriptBuilder.FilterViewElementFactory, Is.TypeOf (typeof (SqlSynonymScriptElementFactory)));
      Assert.That (synonymScriptBuilder.EmptyViewElementFactory, Is.TypeOf (typeof (SqlSynonymScriptElementFactory)));
    }

    [Test]
    public void CreateSchemaScriptBuilder ()
    {
      IRdbmsStorageObjectFactory testableSqlProviderFactory = new TestableSqlStorageObjectFactory (
          _tableBuilderStub, _viewBuilderStub, _constraintBuilderStub, _indexBuilderStub, _synonymBuilderStub);

      var result = testableSqlProviderFactory.CreateSchemaScriptBuilder (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (SqlDatabaseSelectionScriptElementBuilder)));
      Assert.That (((SqlDatabaseSelectionScriptElementBuilder) result).InnerScriptBuilder, Is.TypeOf (typeof (CompositeScriptBuilder)));
      Assert.That (
          ((CompositeScriptBuilder) ((SqlDatabaseSelectionScriptElementBuilder) result).InnerScriptBuilder).RdbmsProviderDefinition,
          Is.SameAs (_rdbmsProviderDefinition));
      Assert.That (
          ((CompositeScriptBuilder) ((SqlDatabaseSelectionScriptElementBuilder) result).InnerScriptBuilder).ScriptBuilders,
          Is.EqualTo (
              new IScriptBuilder[]
              {
                  _tableBuilderStub,
                  _constraintBuilderStub,
                  _viewBuilderStub,
                  _indexBuilderStub,
                  _synonymBuilderStub
              }));
    }

    [Test]
    public void CreateStorageProviderSerializer ()
    {
      var enumSerializerStub = MockRepository.GenerateStub<IEnumSerializer>();
      var testableSqlProviderFactory = new TestableSqlStorageObjectFactory (null,
          enumSerializerStub);

      var storageProviderSerializer = testableSqlProviderFactory.CreateStorageProviderSerializer (enumSerializerStub);
      Assert.That(storageProviderSerializer, Is.InstanceOf<StorageProviderSerializer>());
      Assert.That (
          ((StorageProviderSerializer) storageProviderSerializer).ClassSerializer,
          Is.InstanceOf<ClassSerializer> ());
    }

    [Test]
    public void CreateEnumSerializer ()
    {
      var testableSqlProviderFactory = new TestableSqlStorageObjectFactory (null, null);

      var enumSerializer = testableSqlProviderFactory.CreateEnumSerializer ();
      Assert.That(enumSerializer, Is.InstanceOf<ExtensibleEnumSerializerDecorator>());
    }
  }
}