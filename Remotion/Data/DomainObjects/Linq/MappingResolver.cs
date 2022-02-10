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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
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
      ArgumentUtility.CheckNotNull("storageSpecificExpressionResolver", storageSpecificExpressionResolver);

      _storageSpecificExpressionResolver = storageSpecificExpressionResolver;
    }

    public IStorageSpecificExpressionResolver StorageSpecificExpressionResolver
    {
      get { return _storageSpecificExpressionResolver; }
    }

    public IResolvedTableInfo ResolveTableInfo (UnresolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull("tableInfo", tableInfo);
      ArgumentUtility.CheckNotNull("generator", generator);

      var typeDefinition = GetTypeDefinition(tableInfo.ItemType);
      return _storageSpecificExpressionResolver.ResolveTable(typeDefinition, generator.GetUniqueIdentifier("t"));
    }

    public ResolvedJoinInfo ResolveJoinInfo (UnresolvedJoinInfo joinInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull("joinInfo", joinInfo);
      ArgumentUtility.CheckNotNull("generator", generator);

      var leftEndPointDefinition = GetEndPointDefinition(joinInfo.OriginatingEntity, joinInfo.MemberInfo);

      return _storageSpecificExpressionResolver.ResolveJoin(
          joinInfo.OriginatingEntity,
          leftEndPointDefinition,
          leftEndPointDefinition.GetOppositeEndPointDefinition(),
          generator.GetUniqueIdentifier("t"));
    }

    public SqlEntityDefinitionExpression ResolveSimpleTableInfo (IResolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull("tableInfo", tableInfo);
      ArgumentUtility.CheckNotNull("generator", generator);

      var typeDefinition = GetTypeDefinition(tableInfo.ItemType);
      return _storageSpecificExpressionResolver.ResolveEntity(typeDefinition, tableInfo.TableAlias);
    }

    public Expression ResolveMemberExpression (SqlEntityExpression originatingEntity, MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull("originatingEntity", originatingEntity);
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);

      var property = GetMemberAsProperty(originatingEntity, memberInfo);
      var entityTypeDefinition = GetTypeDefinition(originatingEntity.Type);

      if (property.Equals(s_idProperty))
        return _storageSpecificExpressionResolver.ResolveIDProperty(originatingEntity, entityTypeDefinition);

      var resolvedMember = InlineTypeDefinitionWalker.WalkDescendants(
          entityTypeDefinition,
          classDefinition => ResolveMemberInTypeDefinition(originatingEntity, property, classDefinition),
          interfaceDefinition => throw new NotImplementedException("Interfaces are not supported"),
          match: expression => expression != null); // TODO R2I Mapping: Add support for interfaces

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
      ArgumentUtility.CheckNotNull("sqlColumnExpression", sqlColumnExpression);
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);

      throw new UnmappedItemException(
          string.Format("The member '{0}.{1}' does not identify a mapped property.", memberInfo.ReflectedType!.Name, memberInfo.Name));
    }

    public Expression ResolveConstantExpression (ConstantExpression constantExpression)
    {
      ArgumentUtility.CheckNotNull("constantExpression", constantExpression);

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
      ArgumentUtility.CheckNotNull("checkedExpression", checkedExpression);
      ArgumentUtility.CheckNotNull("desiredType", desiredType);

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

        var typeDefinition = GetTypeDefinition(desiredType);
        var idExpression = Expression.MakeMemberAccess(checkedExpression, s_idProperty.PropertyInfo);
        var classIDExpression = Expression.MakeMemberAccess(idExpression, s_classIDProperty.PropertyInfo);

        var allClassIDExpressions = new List<Expression>();
        InlineTypeDefinitionWalker.WalkDescendants(
            typeDefinition,
            classDefinition => allClassIDExpressions.Add(new SqlLiteralExpression(classDefinition.ID)),
            interfaceDefinition => throw new NotImplementedException("Interfaces are not supported")); // TODO R2I Mapping: Add support for interfaces

        return new SqlInExpression(classIDExpression, new SqlCollectionExpression(typeof(string[]), allClassIDExpressions));
      }
      else
      {
        return Expression.Constant(false);
      }
    }

    public Expression? TryResolveOptimizedIdentity (SqlEntityRefMemberExpression entityRefMemberExpression)
    {
      ArgumentUtility.CheckNotNull("entityRefMemberExpression", entityRefMemberExpression);

      var endPointDefinition = GetEndPointDefinition(entityRefMemberExpression.OriginatingEntity, entityRefMemberExpression.MemberInfo);
      var foreignKeyEndPoint = endPointDefinition as RelationEndPointDefinition;
      if (foreignKeyEndPoint == null)
        return null;

      return _storageSpecificExpressionResolver.ResolveEntityIdentityViaForeignKey(entityRefMemberExpression.OriginatingEntity, foreignKeyEndPoint);
    }

    public Expression? TryResolveOptimizedMemberExpression (SqlEntityRefMemberExpression entityRefMemberExpression, MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull("entityRefMemberExpression", entityRefMemberExpression);
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);

      if (memberInfo.DeclaringType != typeof(DomainObject) || memberInfo.Name != "ID")
        return null;

      var endPointDefinition = GetEndPointDefinition(entityRefMemberExpression.OriginatingEntity, entityRefMemberExpression.MemberInfo);
      var foreignKeyEndPoint = endPointDefinition as RelationEndPointDefinition;
      if (foreignKeyEndPoint == null)
        return null;

      return _storageSpecificExpressionResolver.ResolveIDPropertyViaForeignKey(entityRefMemberExpression.OriginatingEntity, foreignKeyEndPoint);
    }

    private TypeDefinition GetTypeDefinition (Type type)
    {
      var typeDefinition = MappingConfiguration.Current.GetTypeDefinition(
          type,
          t => new UnmappedItemException(string.Format("The type '{0}' does not identify a queryable table.", t)));
      return typeDefinition;
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

    private Expression? ResolveMemberInTypeDefinition (
        SqlEntityExpression originatingEntity, PropertyInfoAdapter propertyInfoAdapter, TypeDefinition typeDefinition)
    {
      var endPointDefinition = typeDefinition.ResolveRelationEndPoint(propertyInfoAdapter);
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

      var propertyDefinition = typeDefinition.ResolveProperty(propertyInfoAdapter);
      if (propertyDefinition != null)
        return _storageSpecificExpressionResolver.ResolveProperty(originatingEntity, propertyDefinition);

      return null;
    }

    private IRelationEndPointDefinition GetEndPointDefinition (SqlEntityExpression originatingEntity, MemberInfo memberInfo)
    {
      var property = GetMemberAsProperty(originatingEntity, memberInfo);
      var entityTypeDefinition = GetTypeDefinition(originatingEntity.Type);

      var leftEndPointDefinition = InlineTypeDefinitionWalker.WalkDescendants(
          entityTypeDefinition,
          classDefinition => classDefinition.ResolveRelationEndPoint(property),
          interfaceDefinition => throw new NotImplementedException("Interfaces are not supported"),
          match: relationEndPoint => relationEndPoint != null); // TODO R2I Mapping: Add support for interfaces

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
