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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.Linq.ExecutableQueries;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.FunctionalProgramming;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Generates <see cref="IQuery"/> objects from LINQ queries (parsed by re-linq into <see cref="QueryModel"/> instances).
  /// </summary>
  public class DomainObjectQueryGenerator : IDomainObjectQueryGenerator
  {
    private abstract class GenericCallHelper
    {
      private static readonly ConcurrentDictionary<Type, GenericCallHelper> s_cache = new ConcurrentDictionary<Type, GenericCallHelper>();

      public static GenericCallHelper Create (Type classType)
      {
        return s_cache.GetOrAdd(
            classType,
            key => (GenericCallHelper)Activator.CreateInstance(typeof(GenericCallHelper<>).MakeGenericType(key))!);
      }

      protected GenericCallHelper ()
      {
      }

      public abstract IQuery CreateSequenceQuery (
          DomainObjectQueryGenerator domainObjectQueryGenerator,
          string id,
          StorageProviderDefinition storageProviderDefinition,
          QueryModel queryModel,
          IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders);
    }

    private class GenericCallHelper<T> : GenericCallHelper
    {
      public GenericCallHelper ()
      {
      }

      public override IQuery CreateSequenceQuery (
          DomainObjectQueryGenerator domainObjectQueryGenerator,
          string id,
          StorageProviderDefinition storageProviderDefinition,
          QueryModel queryModel,
          IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders)
      {
        return domainObjectQueryGenerator.CreateSequenceQuery<T>(id, storageProviderDefinition, queryModel, fetchQueryModelBuilders);
      }
    }

    private static readonly ConcurrentDictionary<Type, Type> s_collectionTypeCache = new ConcurrentDictionary<Type, Type>();

    private readonly ISqlQueryGenerator _sqlQueryGenerator;
    private readonly ITypeConversionProvider _typeConversionProvider;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;
    private readonly IMappingConfiguration _mappingConfiguration;

    public DomainObjectQueryGenerator (
        ISqlQueryGenerator sqlQueryGenerator,
        ITypeConversionProvider typeConversionProvider,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IMappingConfiguration mappingConfiguration)
    {
      ArgumentUtility.CheckNotNull("sqlQueryGenerator", sqlQueryGenerator);
      ArgumentUtility.CheckNotNull("typeConversionProvider", typeConversionProvider);
      ArgumentUtility.CheckNotNull("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull("mappingConfiguration", mappingConfiguration);

      _sqlQueryGenerator = sqlQueryGenerator;
      _typeConversionProvider = typeConversionProvider;
      _storageTypeInformationProvider = storageTypeInformationProvider;
      _mappingConfiguration = mappingConfiguration;
    }

    public ISqlQueryGenerator SqlQueryGenerator
    {
      get { return _sqlQueryGenerator; }
    }

    public ITypeConversionProvider TypeConversionProvider
    {
      get { return _typeConversionProvider; }
    }

    public IStorageTypeInformationProvider StorageTypeInformationProvider
    {
      get { return _storageTypeInformationProvider; }
    }

    public IMappingConfiguration MappingConfiguration
    {
      get { return _mappingConfiguration; }
    }

    public virtual IExecutableQuery<T> CreateScalarQuery<T> (string id, StorageProviderDefinition storageProviderDefinition, QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull("id", id);
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("queryModel", queryModel);

      var sqlQuery = _sqlQueryGenerator.CreateSqlQuery(queryModel);
      var sqlCommand = sqlQuery.SqlCommand;

      var query = CreateQuery(id, storageProviderDefinition, sqlCommand.CommandText, sqlCommand.Parameters, QueryType.ScalarReadOnly, selectedEntityType: null);

      var projection = sqlCommand.GetInMemoryProjection<T>().Compile();
      return new ScalarQueryAdapter<T>(query, o => projection(new ScalarResultRowAdapter(o, _storageTypeInformationProvider)));
    }

    public virtual IExecutableQuery<IEnumerable<TItem>> CreateSequenceQuery<TItem> (
        string id,
        StorageProviderDefinition storageProviderDefinition,
        QueryModel queryModel,
        IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders)
    {
      ArgumentUtility.CheckNotNullOrEmpty("id", id);
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("queryModel", queryModel);
      ArgumentUtility.CheckNotNull("fetchQueryModelBuilders", fetchQueryModelBuilders);

      var sqlQuery = _sqlQueryGenerator.CreateSqlQuery(queryModel);
      var command = sqlQuery.SqlCommand;

      var queryType = sqlQuery.SelectedEntityType != null ? QueryType.CollectionReadOnly : QueryType.CustomReadOnly;
      var query = CreateQuery(id, storageProviderDefinition, command.CommandText, command.Parameters, queryType, sqlQuery.SelectedEntityType);

      if (queryType == QueryType.CollectionReadOnly)
      {
        Assertion.DebugIsNotNull(sqlQuery.SelectedEntityType, "sqlQuery.SelectedEntityType != null");
        var selectedEntityClassDefinition = _mappingConfiguration.GetTypeDefinition(sqlQuery.SelectedEntityType);
        Assertion.IsNotNull(selectedEntityClassDefinition, "We assume that in a re-store LINQ query, entities always have a mapping.");

        var fetchQueries = CreateEagerFetchQueries(selectedEntityClassDefinition, fetchQueryModelBuilders);
        foreach (var fetchQuery in fetchQueries)
          query.EagerFetchQueries.Add(fetchQuery.Item1, fetchQuery.Item2);

        return new DomainObjectSequenceQueryAdapter<TItem>(query);
      }
      else
      {
        if (fetchQueryModelBuilders.Any())
          throw new NotSupportedException("Only queries returning DomainObjects can perform eager fetching.");

        var projection = sqlQuery.SqlCommand.GetInMemoryProjection<TItem>().Compile();
        return new CustomSequenceQueryAdapter<TItem>(query, qrr => projection(new QueryResultRowAdapter(qrr)));
      }
    }

    protected virtual IQuery CreateQuery (
        string id,
        StorageProviderDefinition storageProviderDefinition,
        string statement,
        CommandParameter[] commandParameters,
        QueryType queryType,
        Type? selectedEntityType)
    {
      ArgumentUtility.CheckNotNullOrEmpty("id", id);
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("statement", statement);
      ArgumentUtility.CheckNotNull("commandParameters", commandParameters);

      var queryParameters = new QueryParameterCollection();
      foreach (var commandParameter in commandParameters)
        queryParameters.Add(commandParameter.Name, commandParameter.Value, QueryParameterType.Value);

      return queryType switch
      {
        QueryType.ScalarReadOnly => QueryFactory.CreateScalarQuery(id, storageProviderDefinition, statement, queryParameters),
        QueryType.CollectionReadOnly => QueryFactory.CreateCollectionQuery(id, storageProviderDefinition, statement, queryParameters, GetCollectionType(selectedEntityType)),
        QueryType.CustomReadOnly => QueryFactory.CreateCustomQuery(id, storageProviderDefinition, statement, queryParameters),
        _ => throw new ArgumentException("The requested query type '{0}' cannot be used with LiNQ. Only read-only query types are supported.", "queryType")
      };
    }

    private IEnumerable<Tuple<IRelationEndPointDefinition, IQuery>> CreateEagerFetchQueries (
        ClassDefinition previousClassDefinition,
        IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders)
    {
      foreach (var fetchQueryModelBuilder in fetchQueryModelBuilders)
      {
        var relationEndPointDefinition = GetEagerFetchRelationEndPointDefinition(fetchQueryModelBuilder.FetchRequest, previousClassDefinition);

        // clone the fetch query model because we don't want to modify the source model of all inner requests
        var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel().Clone();

        // for re-store, fetch queries must always be distinct even when the query would return duplicates
        // e.g., for (from o in Orders select o).FetchOne (o => o.Customer)
        // when two orders have the same customer, re-store gives an error, unless we add a DISTINCT clause
        fetchQueryModel.ResultOperators.Add(new DistinctResultOperator());

        var sortExpression = GetSortExpressionForRelation(relationEndPointDefinition);
        if (sortExpression != null)
        {
          // If we have a SortExpression, we need to add the ORDER BY clause _after_ the DISTINCT (because re-linq strips out ORDER BY clauses when
          // seeing a DISTINCT operator); therefore, we put the DISTINCT query into a sub-query, then append the ORDER BY clause
          fetchQueryModel = fetchQueryModel.ConvertToSubQuery("#fetch");

          var orderByClause = new OrderByClause();
          foreach (var sortedPropertySpecification in sortExpression.SortedProperties)
            orderByClause.Orderings.Add(GetOrdering(fetchQueryModel, sortedPropertySpecification));

          fetchQueryModel.BodyClauses.Add(orderByClause);
        }

        var fetchedClassType = relationEndPointDefinition.GetOppositeClassDefinition().ClassType;
        var genericCallHelper = GenericCallHelper.Create(fetchedClassType);
        var fetchQuery = genericCallHelper.CreateSequenceQuery(
            this,
            "<fetch query for " + fetchQueryModelBuilder.FetchRequest.RelationMember.Name + ">",
            previousClassDefinition.StorageEntityDefinition.StorageProviderDefinition,
            fetchQueryModel,
            fetchQueryModelBuilder.CreateInnerBuilders());

        yield return Tuple.Create(relationEndPointDefinition, fetchQuery);
      }
    }

    private SortExpressionDefinition? GetSortExpressionForRelation (IRelationEndPointDefinition relationEndPointDefinition)
    {
      switch (relationEndPointDefinition)
      {
        case DomainObjectCollectionRelationEndPointDefinition domainObjectCollectionRelationEndPointDefinition:
          return domainObjectCollectionRelationEndPointDefinition.GetSortExpression();
        default:
          return null;
      }
    }

    private IRelationEndPointDefinition GetEagerFetchRelationEndPointDefinition (FetchRequestBase fetchRequest, ClassDefinition classDefinition)
    {
      var propertyInfo = fetchRequest.RelationMember as PropertyInfo;
      if (propertyInfo == null)
      {
        var message = string.Format(
            "The member '{0}' is a '{1}', which cannot be fetched by this LINQ provider. Only properties can be fetched.",
            fetchRequest.RelationMember.Name,
            fetchRequest.RelationMember.MemberType);
        throw new NotSupportedException(message);
      }

      var propertyInfoAdapter = PropertyInfoAdapter.Create(propertyInfo);
      var endPoint = classDefinition.ResolveRelationEndPoint(propertyInfoAdapter)
                     ?? classDefinition.GetAllDerivedClasses()
                         .Select(cd => cd.ResolveRelationEndPoint(propertyInfoAdapter))
                         .FirstOrDefault(ep => ep != null);

      if (endPoint == null)
      {
        Assertion.IsNotNull(propertyInfo.DeclaringType);
        var message = string.Format(
            "The property '{0}.{1}' is not a relation end point. Fetching it is not supported by this LINQ provider.",
            propertyInfo.DeclaringType.GetFullNameSafe(),
            propertyInfo.Name);
        throw new NotSupportedException(message);
      }

      return endPoint;
    }

    private Ordering GetOrdering (QueryModel queryModel, SortedPropertySpecification sortedPropertySpecification)
    {
      var instanceExpression = queryModel.SelectClause.Selector;

      var normalizedProperty = GetNormalizedProperty(
          sortedPropertySpecification.PropertyDefinition.PropertyInfo,
          TypeAdapter.Create(instanceExpression.Type));

      var memberExpression = Expression.MakeMemberAccess(
            Expression.Convert(instanceExpression, normalizedProperty.DeclaringType!.ConvertToRuntimeType()),
            normalizedProperty.ConvertToRuntimePropertyInfo());

      var orderingDirection = sortedPropertySpecification.Order == SortOrder.Ascending ? OrderingDirection.Asc : OrderingDirection.Desc;
      return new Ordering(memberExpression, orderingDirection);
    }

    private IPropertyInformation GetNormalizedProperty (IPropertyInformation propertyInformation, ITypeInformation instanceTypeInformation)
    {
      var originalDeclaringType = propertyInformation.GetOriginalDeclaringType()!;
      // Support for properties declared on instance type and base types
      if (originalDeclaringType.IsAssignableFrom(instanceTypeInformation))
        return propertyInformation;

      // Support for properties declared on derived types
      if (instanceTypeInformation.IsAssignableFrom(originalDeclaringType))
        return propertyInformation;

      // Support for properties declared on mixin
      if (Mixins.Utilities.ReflectionUtility.IsMixinType(originalDeclaringType.ConvertToRuntimeType()))
      {
        var instanceRuntimeType = instanceTypeInformation.ConvertToRuntimeType();
        var interfacePropertyInformation = propertyInformation.FindInterfaceDeclarations()
            .Where(p => MixinTypeUtility.IsAssignableFrom(p.GetOriginalDeclaringType()!.ConvertToRuntimeType(), instanceRuntimeType))
            .First(
                () =>
                    new NotSupportedException(
                        string.Format(
                            "The member '{0}.{1}' is not part of any interface introduced onto the target class '{2}'. "
                            + "Only mixed properties that are part of an introduced interface can be used within the sort-expression of a collection property.",
                            propertyInformation.DeclaringType!.GetFullNameSafe(),
                            propertyInformation.Name,
                            instanceTypeInformation.GetFullNameSafe()
                            )));

        return interfacePropertyInformation;
      }

      // Unreachable due to mapping validation
      throw new NotSupportedException(
          string.Format(
              "The member '{0}.{1}' is not part of inheritance hierarchy of class '{2}'. "
              + "Only properties that are part of the inheritance hierarhcy can be used within the sort-expression of a collection property.",
              propertyInformation.DeclaringType!.GetFullNameSafe(),
              propertyInformation.Name,
              instanceTypeInformation.GetFullNameSafe()
              ));
    }

    private Type GetCollectionType (Type? itemType)
    {
      if (itemType == null)
        return typeof(DomainObjectCollection);

      if (!typeof(DomainObject).IsAssignableFrom(itemType))
        return typeof(DomainObjectCollection);

      return s_collectionTypeCache.GetOrAdd(itemType, key => typeof(ObjectList<>).MakeGenericType(itemType));
    }
  }
}
