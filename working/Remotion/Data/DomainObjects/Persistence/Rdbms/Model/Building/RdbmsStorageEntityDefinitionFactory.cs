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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="RdbmsStorageEntityDefinitionFactory"/> provides factory methods to create <see cref="IRdbmsStorageEntityDefinition"/>s.
  /// </summary>
  public class RdbmsStorageEntityDefinitionFactory : IRdbmsStorageEntityDefinitionFactory
  {
    private readonly IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly IStoragePropertyDefinitionResolver _storagePropertyDefinitionResolver;
    private readonly IForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactory;
    private readonly IStorageNameProvider _storageNameProvider;

    public RdbmsStorageEntityDefinitionFactory (
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactory,
        IStoragePropertyDefinitionResolver storagePropertyDefinitionResolver,
        IStorageNameProvider storageNameProvider,
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintDefinitionFactory", foreignKeyConstraintDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storagePropertyDefinitionResolver", storagePropertyDefinitionResolver);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
      _foreignKeyConstraintDefinitionFactory = foreignKeyConstraintDefinitionFactory;
      _storagePropertyDefinitionResolver = storagePropertyDefinitionResolver;
      _storageNameProvider = storageNameProvider;
      _storageProviderDefinition = storageProviderDefinition;
    }

    public IInfrastructureStoragePropertyDefinitionProvider InfrastructureStoragePropertyDefinitionProvider
    {
      get { return _infrastructureStoragePropertyDefinitionProvider; }
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public IStoragePropertyDefinitionResolver StoragePropertyDefinitionResolver
    {
      get { return _storagePropertyDefinitionResolver; }
    }

    public IForeignKeyConstraintDefinitionFactory ForeignKeyConstraintDefinitionFactory
    {
      get { return _foreignKeyConstraintDefinitionFactory; }
    }

    public IStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public virtual IRdbmsStorageEntityDefinition CreateTableDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var tableName = _storageNameProvider.GetTableName (classDefinition);
      if (tableName == null)
        throw new MappingException (string.Format ("Class '{0}' has no table name defined.", classDefinition.ID));

      var objectIDProperty = _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition ();
      var timestampProperty = _infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition ();
      var dataProperties = _storagePropertyDefinitionResolver.GetStoragePropertiesForHierarchy (classDefinition).ToList();
      var allProperties = new[] { objectIDProperty, timestampProperty }.Concat (dataProperties).ToList().AsReadOnly();

      var primaryKeyConstraints = CreatePrimaryKeyConstraints (classDefinition, allProperties);
      var foreignKeyConstraints = CreateForeignKeyConstraintsForTableDefinition (classDefinition, allProperties);

      return new TableDefinition (
          _storageProviderDefinition,
          tableName,
          _storageNameProvider.GetViewName (classDefinition),
          objectIDProperty,
          timestampProperty,
          dataProperties,
          primaryKeyConstraints.Concat (foreignKeyConstraints),
          CreateIndexesForTableDefinition (classDefinition, allProperties),
          CreateSynonymsForTableDefinition (classDefinition));
    }

    public virtual IRdbmsStorageEntityDefinition CreateFilterViewDefinition (
        ClassDefinition classDefinition,
        IRdbmsStorageEntityDefinition baseEntity)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("baseEntity", baseEntity);

      var objectIDProperty = _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition();
      var timestampProperty = _infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition();
      var dataProperties = _storagePropertyDefinitionResolver.GetStoragePropertiesForHierarchy (classDefinition).ToList();
      var allProperties = new[] { objectIDProperty, timestampProperty }.Concat (dataProperties).ToList().AsReadOnly();

      return new FilterViewDefinition (
          _storageProviderDefinition,
          _storageNameProvider.GetViewName (classDefinition),
          baseEntity,
          GetClassIDsForBranch (classDefinition),
          objectIDProperty,
          timestampProperty,
          dataProperties,
          CreateIndexesForFilterViewDefinition (classDefinition, baseEntity, allProperties),
          CreateSynonymsForFilterViewDefinition (classDefinition, baseEntity));
    }

    public virtual IRdbmsStorageEntityDefinition CreateUnionViewDefinition (
        ClassDefinition classDefinition,
        IEnumerable<IRdbmsStorageEntityDefinition> unionedEntities)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("unionedEntities", unionedEntities);

      var objectIDProperty = _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition();
      var timestampProperty = _infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition();
      var dataProperties = _storagePropertyDefinitionResolver.GetStoragePropertiesForHierarchy (classDefinition).ToList();
      var allProperties = new[] { objectIDProperty, timestampProperty }.Concat (dataProperties).ToList().AsReadOnly();

      var unionedEntitiesList = unionedEntities.ToList().AsReadOnly();

      return new UnionViewDefinition (
          _storageProviderDefinition,
          _storageNameProvider.GetViewName (classDefinition),
          unionedEntitiesList,
          objectIDProperty,
          timestampProperty,
          dataProperties,
          CreateIndexesForUnionViewDefinition (classDefinition, unionedEntitiesList, allProperties),
          CreateSynonymsForUnionViewDefinition (classDefinition, unionedEntitiesList));
    }

    public IRdbmsStorageEntityDefinition CreateEmptyViewDefinition (ClassDefinition classDefinition)
    {
      var dataProperties = _storagePropertyDefinitionResolver.GetStoragePropertiesForHierarchy (classDefinition);
      return new EmptyViewDefinition (
          _storageProviderDefinition,
          _storageNameProvider.GetViewName (classDefinition),
          _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition(),
          _infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition(),
          dataProperties,
          new EntityNameDefinition[0]);
    }

    protected IEnumerable<string> GetClassIDsForBranch (ClassDefinition classDefinition)
    {
      return new[] { classDefinition }.Concat (classDefinition.GetAllDerivedClasses()).Select (cd => cd.ID);
    }

    private IEnumerable<ITableConstraintDefinition> CreatePrimaryKeyConstraints (
       ClassDefinition classDefinition,
       IReadOnlyList<IRdbmsStoragePropertyDefinition> allProperties)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("allProperties", allProperties);

      var primaryKeyColumns =
          (from p in allProperties
            from c in p.GetColumns()
            where c.IsPartOfPrimaryKey
            select c).ToList().AsReadOnly();

      if (primaryKeyColumns.Any())
        yield return CreatePrimaryKeyConstraint (classDefinition, primaryKeyColumns);
    }

    protected virtual PrimaryKeyConstraintDefinition CreatePrimaryKeyConstraint (
        ClassDefinition classDefinition,
        IReadOnlyList<ColumnDefinition> primaryKeyColumns)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("primaryKeyColumns", primaryKeyColumns);

      return new PrimaryKeyConstraintDefinition (
          _storageNameProvider.GetPrimaryKeyConstraintName (classDefinition),
          true,
          primaryKeyColumns);
    }

    protected virtual IEnumerable<ITableConstraintDefinition> CreateForeignKeyConstraintsForTableDefinition (
        ClassDefinition classDefinition,
        IReadOnlyCollection<IRdbmsStoragePropertyDefinition> allProperties)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("allProperties", allProperties);

      return _foreignKeyConstraintDefinitionFactory.CreateForeignKeyConstraints (classDefinition);
    }

    protected virtual IEnumerable<IIndexDefinition> CreateIndexesForTableDefinition (
        ClassDefinition classDefinition,
        IReadOnlyList<IRdbmsStoragePropertyDefinition> allProperties)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("allProperties", allProperties);

      return Enumerable.Empty<IIndexDefinition>();
    }

    protected virtual IEnumerable<IIndexDefinition> CreateIndexesForFilterViewDefinition (
        ClassDefinition classDefinition,
        IRdbmsStorageEntityDefinition baseEntity,
        IReadOnlyList<IRdbmsStoragePropertyDefinition> allProperties)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("baseEntity", baseEntity);
      ArgumentUtility.CheckNotNull ("allProperties", allProperties);

      return Enumerable.Empty<IIndexDefinition>();
    }

    protected virtual IEnumerable<IIndexDefinition> CreateIndexesForUnionViewDefinition (
        ClassDefinition classDefinition,
        IReadOnlyList<IRdbmsStorageEntityDefinition> unionedEntitiesList,
        IReadOnlyList<IRdbmsStoragePropertyDefinition> allProperties)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("unionedEntitiesList", unionedEntitiesList);
      ArgumentUtility.CheckNotNull ("allProperties", allProperties);

      return Enumerable.Empty<IIndexDefinition>();
    }

    protected virtual IEnumerable<EntityNameDefinition> CreateSynonymsForTableDefinition (
        ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return Enumerable.Empty<EntityNameDefinition>();
    }

    private IEnumerable<EntityNameDefinition> CreateSynonymsForFilterViewDefinition (
        ClassDefinition classDefinition,
        IRdbmsStorageEntityDefinition baseEntity)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("baseEntity", baseEntity);

      return Enumerable.Empty<EntityNameDefinition>();
    }

    private IEnumerable<EntityNameDefinition> CreateSynonymsForUnionViewDefinition (
        ClassDefinition classDefinition,
        IReadOnlyList<IRdbmsStorageEntityDefinition> unionedEntitiesList)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("unionedEntitiesList", unionedEntitiesList);

      return Enumerable.Empty<EntityNameDefinition>();
    }
  }
}