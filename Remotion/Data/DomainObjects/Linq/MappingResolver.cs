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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Implements <see cref="IMappingResolver"/> to supply information from re-store to the re-linq SQL backend.
  /// </summary>
  public class MappingResolver : IMappingResolver
  {
    private readonly IStorageSpecificExpressionResolver _storageSpecificExpressionResolver;

    private static readonly PropertyInfoAdapter s_classIDProperty =
        PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((ObjectID obj) => obj.ClassID));

    private static readonly PropertyInfoAdapter s_idProperty =
        PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((DomainObject obj) => obj.ID));

    public MappingResolver (IStorageSpecificExpressionResolver storageSpecificExpressionResolver)
    {
      ArgumentNullException.ThrowIfNull(storageSpecificExpressionResolver);

      _storageSpecificExpressionResolver = storageSpecificExpressionResolver;
    }

    public IStorageSpecificExpressionResolver StorageSpecificExpressionResolver
    {
      get { return _storageSpecificExpressionResolver; }
    }

    public IResolvedTableInfo ResolveTableInfo (UnresolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentNullException.ThrowIfNull(tableInfo);
      ArgumentNullException.ThrowIfNull(generator);

      var classDefinition = GetClassDefinition(tableInfo.ItemType);
      return _storageSpecificExpressionResolver.ResolveTable(classDefinition, generator.GetUniqueIdentifier("t"));
    }

    public ResolvedJoinInfo ResolveJoinInfo (UnresolvedJoinInfo joinInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentNullException.ThrowIfNull(joinInfo);
      ArgumentNullException.ThrowIfNull(generator);

      var leftEndPointDefinition = GetEndPointDefinition(joinInfo.OriginatingEntity, joinInfo.MemberInfo);

      return _storageSpecificExpressionResolver.ResolveJoin(
          joinInfo.OriginatingEntity,
          leftEndPointDefinition,
          leftEndPointDefinition.GetOppositeEndPointDefinition(),
          generator.GetUniqueIdentifier("t"));
    }

    public SqlEntityDefinitionExpression ResolveSimpleTableInfo (IResolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentNullException.ThrowIfNull(tableInfo);
      ArgumentNullException.ThrowIfNull(generator);

      var classDefinition = GetClassDefinition(tableInfo.ItemType);
      return _storageSpecificExpressionResolver.ResolveEntity(classDefinition, tableInfo.TableAlias);
    }

    public Expression ResolveMemberExpression (SqlEntityExpression originatingEntity, MemberInfo memberInfo)
    {
      ArgumentNullException.ThrowIfNull(originatingEntity);
      ArgumentNullException.ThrowIfNull(memberInfo);

      var property = GetMemberAsProperty(originatingEntity, memberInfo);
      var entityClassDefinition = GetClassDefinition(originatingEntity.Type);

      if (property.Equals(s_idProperty))
        return _storageSpecificExpressionResolver.ResolveIDProperty(originatingEntity, entityClassDefinition);

      var allClassDefinitions = new[] { entityClassDefinition }.Concat(entityClassDefinition.GetAllDerivedClasses());
      var resolvedMember = allClassDefinitions
          .Select(cd => ResolveMemberInClassDefinition(originatingEntity, property, cd))
          .FirstOrDefault(e => e != null);

      if (resolvedMember == null)
      {
        Assertion.IsNotNull(property.DeclaringType);
        string message = string.Format(
            "The member '{0}.{1}' does not have a queryable database mapping.", property.DeclaringType.Name, property.Name);
        throw new UnmappedItemException(message);
      }

      return resolvedMember;
    }

    public Expression ResolveMemberExpression (SqlColumnExpression sqlColumnExpression, MemberInfo memberInfo)
    {
      ArgumentNullException.ThrowIfNull(sqlColumnExpression);
      ArgumentNullException.ThrowIfNull(memberInfo);

      throw new UnmappedItemException(
          string.Format("The member '{0}.{1}' does not identify a mapped property.", memberInfo.ReflectedType!.Name, memberInfo.Name));
    }

    public Expression ResolveConstantExpression (ConstantExpression constantExpression)
    {
      ArgumentNullException.ThrowIfNull(constantExpression);

      var domainObject = constantExpression.Value as DomainObject;
      if (domainObject != null)
      {
        var primaryKeyExpression = Expression.Constant(domainObject.ID);
        return new SqlEntityConstantExpression(constantExpression.Type, constantExpression.Value, primaryKeyExpression);
      }

      return constantExpression;
    }

    public Expression ResolveTypeCheck (Expression checkedExpression, Type desiredType)
    {
      ArgumentNullException.ThrowIfNull(checkedExpression);
      ArgumentNullException.ThrowIfNull(desiredType);

      if (desiredType.IsAssignableFrom(checkedExpression.Type))
      {
        return Expression.Constant(true);
      }
      else if (checkedExpression.Type.IsAssignableFrom(desiredType))
      {
        if (!ReflectionUtility.IsDomainObject(checkedExpression.Type))
        {
          var message = string.Format(
              "No database-level type check can be added for the expression '{0}'. Only the types of DomainObjects can be checked in database queries.",
              checkedExpression);
          throw new UnmappedItemException(message);
        }

        var classDefinition = GetClassDefinition(desiredType);
        var idExpression = Expression.MakeMemberAccess(checkedExpression, s_idProperty.PropertyInfo);
        var classIDExpression = Expression.MakeMemberAccess(idExpression, s_classIDProperty.PropertyInfo);
        var allClassDefinitions = EnumerableUtility.Singleton(classDefinition).Concat(classDefinition.GetAllDerivedClasses().Select(cd => cd));
        var allClassIDs = allClassDefinitions.Select(cd => cd.ID).ToArray();

        return new SqlInExpression(classIDExpression, new ConstantCollectionExpression(allClassIDs));
      }
      else
      {
        return Expression.Constant(false);
      }
    }

    public Expression? TryResolveOptimizedIdentity (SqlEntityRefMemberExpression entityRefMemberExpression)
    {
      ArgumentNullException.ThrowIfNull(entityRefMemberExpression);

      var endPointDefinition = GetEndPointDefinition(entityRefMemberExpression.OriginatingEntity, entityRefMemberExpression.MemberInfo);
      var foreignKeyEndPoint = endPointDefinition as RelationEndPointDefinition;
      if (foreignKeyEndPoint == null)
        return null;

      return _storageSpecificExpressionResolver.ResolveEntityIdentityViaForeignKey(entityRefMemberExpression.OriginatingEntity, foreignKeyEndPoint);
    }

    public Expression? TryResolveOptimizedMemberExpression (SqlEntityRefMemberExpression entityRefMemberExpression, MemberInfo memberInfo)
    {
      ArgumentNullException.ThrowIfNull(entityRefMemberExpression);
      ArgumentNullException.ThrowIfNull(memberInfo);

      if (memberInfo.DeclaringType != typeof(DomainObject) || memberInfo.Name != "ID")
        return null;

      var endPointDefinition = GetEndPointDefinition(entityRefMemberExpression.OriginatingEntity, entityRefMemberExpression.MemberInfo);
      var foreignKeyEndPoint = endPointDefinition as RelationEndPointDefinition;
      if (foreignKeyEndPoint == null)
        return null;

      return _storageSpecificExpressionResolver.ResolveIDPropertyViaForeignKey(entityRefMemberExpression.OriginatingEntity, foreignKeyEndPoint);
    }

    public bool TryResolveSetOperationReconciliationContext (Expression[] projections, [NotNullWhen(true)] out ISetOperationReconciliationContext? reconciliationContext)
    {
      ArgumentUtility.CheckNotNull(nameof(projections), projections);
      reconciliationContext = null;

      // For reconciliation, every projection needs to come from an entity.
      // We don't support mixing entities with, for example, constants.
      // If we can't find all the entities, we don't do reconciliation.
      // We don't throw an exception to continue to support existing queries that work even if they are not really supported.
      var entities = new SqlEntityExpression[projections.Length];
      for (var i = 0; i < projections.Length; i++)
      {
        var entity = UnwrapSqlEntityExpression(projections[i]);
        if (entity == null)
          return false;

        entities[i] = entity;
      }

      var builder = DefaultSetOperationReconciliationContext.CreateBuilder();
      foreach (var entity in entities)
      {
        foreach (var column in entity.Columns)
          builder.AddSqlColumn(entity, column);
      }

      reconciliationContext = builder.Build();
      return true;
    }

    private static SqlEntityExpression? UnwrapSqlEntityExpression (Expression projectionExpression)
    {
      // A projection expression might be wrapped in unary expressions like convert to change the resulting type.
      // We unwrap those as they have no effect on the selected SQL columns.
      while (projectionExpression is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unaryExpression)
        projectionExpression = unaryExpression.Operand;

      if (projectionExpression is SqlEntityExpression sqlEntityExpression)
        return sqlEntityExpression;

      return null;
    }

    private ClassDefinition GetClassDefinition (Type type)
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(
          type,
          t => new UnmappedItemException(string.Format("The type '{0}' does not identify a queryable table.", t)));
      return classDefinition;
    }

    private static PropertyInfoAdapter GetMemberAsProperty (SqlEntityExpression originatingEntity, MemberInfo memberInfo)
    {
      var property = memberInfo as PropertyInfo;
      if (property == null)
      {
        throw new UnmappedItemException(
            string.Format(
                "Field '{0}.{1}' cannot be used in a query because it is not a mapped member.",
                originatingEntity.Type.Name,
                memberInfo.Name));
      }
      return PropertyInfoAdapter.Create(property);
    }

    private Expression? ResolveMemberInClassDefinition (
        SqlEntityExpression originatingEntity, PropertyInfoAdapter propertyInfoAdapter, ClassDefinition classDefinition)
    {
      var endPointDefinition = classDefinition.ResolveRelationEndPoint(propertyInfoAdapter);
      if (endPointDefinition != null)
      {
        if (endPointDefinition.Cardinality != CardinalityType.One)
        {
          var message = string.Format(
              "Cannot resolve a collection-valued end-point definition. ('{0}.{1}')", originatingEntity.Type, propertyInfoAdapter.Name);
          throw new NotSupportedException(message);
        }
        return new SqlEntityRefMemberExpression(originatingEntity, propertyInfoAdapter.PropertyInfo);
      }

      var propertyDefinition = classDefinition.ResolveProperty(propertyInfoAdapter);
      if (propertyDefinition != null)
        return _storageSpecificExpressionResolver.ResolveProperty(originatingEntity, propertyDefinition);

      return null;
    }

    private IRelationEndPointDefinition GetEndPointDefinition (SqlEntityExpression originatingEntity, MemberInfo memberInfo)
    {
      var property = GetMemberAsProperty(originatingEntity, memberInfo);
      var entityClassDefinition = GetClassDefinition(originatingEntity.Type);

      var allClassDefinitions = new[] { entityClassDefinition }.Concat(entityClassDefinition.GetAllDerivedClasses());
      var leftEndPointDefinition = allClassDefinitions
          .Select(cd => cd.ResolveRelationEndPoint(property))
          .FirstOrDefault(e => e != null);

      if (leftEndPointDefinition == null)
      {
        Assertion.IsNotNull(memberInfo.DeclaringType);
        string message =
            string.Format("The member '{0}.{1}' does not identify a relation.", memberInfo.DeclaringType.Name, memberInfo.Name);
        throw new UnmappedItemException(message);
      }
      return leftEndPointDefinition;
    }
  }
}
