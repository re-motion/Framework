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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  public class ReferenceTypeDefinitionCollectionBuilder
  {
    private readonly Dictionary<Type, IReferenceTypeDefinitionBuilder> _builders = new();
    private readonly List<IReferenceTypeDefinitionRelationBuilder> _relationBuilders = new();

    public ReferenceClassDefinitionBuilder<T> ClassDefinitionFor<T> ()
        where T : DomainObject
    {
      if (_builders.ContainsKey(typeof(T)))
        throw new InvalidOperationException($"A type definition for the specified type '{typeof(T).GetFullNameSafe()}' is already registered.");

      var builder = new ReferenceClassDefinitionBuilder<T>();
      _builders.Add(typeof(T), builder);

      return builder;
    }

    public ReferenceInterfaceDefinitionBuilder<T> InterfaceDefinitionFor<T> ()
        where T : IDomainObject
    {
      if (_builders.ContainsKey(typeof(T)))
        throw new InvalidOperationException($"A type definition for the specified type '{typeof(T).GetFullNameSafe()}' is already registered.");

      var builder = new ReferenceInterfaceDefinitionBuilder<T>();
      _builders.Add(typeof(T), builder);

      return builder;
    }

    public ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> RelationBetween<TFirst, TSecond> (
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TFirst>> configureFirst,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TSecond>> configureSecond)
        where TFirst : IDomainObject
        where TSecond : IDomainObject
    {
      ArgumentUtility.CheckNotNull("configureFirst", configureFirst);
      ArgumentUtility.CheckNotNull("configureSecond", configureSecond);

      var relationBuilder = new ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond>();
      _relationBuilders.Add(relationBuilder);

      return relationBuilder.WithFirst(configureFirst).WithSecond(configureSecond);
    }

    public ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> UnidirectionalRelationBetween<TFirst, TSecond> (
        string propertyName,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TFirst>> configure = null)
        where TFirst : IDomainObject
        where TSecond : IDomainObject
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);
      configure ??= _ => { };

      return RelationBetween<TFirst, TSecond>(
          first => configure(first.WithProperty(propertyName)),
          second => second.AsAnonymousRelation());
    }

    public ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> UnidirectionalRelationBetween<TFirst, TSecond> (
        Expression<Func<TFirst, TSecond>> propertySelector,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TFirst>> configure = null)
        where TFirst : IDomainObject
        where TSecond : IDomainObject
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);

      var propertyName = MemberInfoFromExpressionUtility.GetProperty(propertySelector).Name;
      return UnidirectionalRelationBetween<TFirst, TSecond>(propertyName, configure);
    }

    public ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> BidirectionalVirtualObjectRelationBetween<TFirst, TSecond> (
        string firstPropertyName,
        string secondPropertyName,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TFirst>> configureFirst = null,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TSecond>> configureSecond = null)
        where TFirst : IDomainObject
        where TSecond : IDomainObject
    {
      ArgumentUtility.CheckNotNull("firstPropertyName", firstPropertyName);
      ArgumentUtility.CheckNotNull("secondPropertyName", secondPropertyName);
      configureFirst ??= _ => { };
      configureSecond ??= _ => { };

      return RelationBetween<TFirst, TSecond>(
          first => configureFirst(first.WithProperty(firstPropertyName)),
          second => configureSecond(second.WithProperty(secondPropertyName).AsVirtualObjectRelation()));
    }

    public ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> BidirectionalVirtualObjectRelationBetween<TFirst, TSecond> (
        Expression<Func<TFirst, TSecond>> firstPropertySelector,
        Expression<Func<TSecond, TFirst>> secondPropertySelector,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TFirst>> configureFirst = null,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TSecond>> configureSecond = null)
        where TFirst : IDomainObject
        where TSecond : IDomainObject
    {
      ArgumentUtility.CheckNotNull("firstPropertySelector", firstPropertySelector);
      ArgumentUtility.CheckNotNull("secondPropertySelector", secondPropertySelector);

      var firstPropertyName = MemberInfoFromExpressionUtility.GetProperty(firstPropertySelector).Name;
      var secondPropertyName = MemberInfoFromExpressionUtility.GetProperty(secondPropertySelector).Name;
      return BidirectionalVirtualObjectRelationBetween(
          firstPropertyName,
          secondPropertyName,
          configureFirst,
          configureSecond);
    }

    public ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> BidirectionalVirtualCollectionRelationBetween<TFirst, TSecond> (
        string firstPropertyName,
        string secondPropertyName,
        Action<ReferenceSortExpressionDefinitionBuilder> configureSortExpression,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TFirst>> configureFirst = null,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TSecond>> configureSecond = null)
        where TFirst : IDomainObject
        where TSecond : IDomainObject
    {
      ArgumentUtility.CheckNotNull("firstPropertyName", firstPropertyName);
      ArgumentUtility.CheckNotNull("secondPropertyName", secondPropertyName);
      ArgumentUtility.CheckNotNull("configureSortExpression", configureSortExpression);
      configureFirst ??= _ => { };
      configureSecond ??= _ => { };

      return RelationBetween<TFirst, TSecond>(
          first => configureFirst(first.WithProperty(firstPropertyName).AsVirtualCollectionRelation(configureSortExpression)),
          second => configureSecond(second.WithProperty(secondPropertyName)));
    }

    public ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> BidirectionalVirtualCollectionRelationBetween<TFirst, TSecond> (
        Expression<Func<TFirst, IObjectList<TSecond>>> firstPropertySelector,
        Expression<Func<TSecond, TFirst>> secondPropertySelector,
        Action<ReferenceSortExpressionDefinitionBuilder> configureSortExpression,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TFirst>> configureFirst = null,
        Action<ReferenceTypeDefinitionRelationEndPointBuilder<TSecond>> configureSecond = null)
        where TFirst : IDomainObject
        where TSecond : IDomainObject
    {
      ArgumentUtility.CheckNotNull("firstPropertySelector", firstPropertySelector);
      ArgumentUtility.CheckNotNull("secondPropertySelector", secondPropertySelector);
      ArgumentUtility.CheckNotNull("configureSortExpression", configureSortExpression);

      var firstPropertyName = MemberInfoFromExpressionUtility.GetProperty(firstPropertySelector).Name;
      var secondPropertyName = MemberInfoFromExpressionUtility.GetProperty(secondPropertySelector).Name;
      return BidirectionalVirtualCollectionRelationBetween(
          firstPropertyName,
          secondPropertyName,
          configureSortExpression,
          configureFirst,
          configureSecond);
    }

    public TypeDefinition[] BuildTypeDefinitions ()
    {
      var context = new ReferenceTypeDefinitionBuilderContext(_builders);
      var typeDefinitions = _builders.Values
          .Select(e => context.ResolveTypeDefinition(e.Type))
          .ToArray();

      foreach (var typeDefinition in typeDefinitions)
      {
        if (typeDefinition is ClassDefinition classDefinition)
        {
          classDefinition.SetDerivedClasses(
              typeDefinitions.OfType<ClassDefinition>().Where(e => e.BaseClass == classDefinition));
        }
        else if (typeDefinition is InterfaceDefinition interfaceDefinition)
        {
          interfaceDefinition.SetImplementingClasses(
              typeDefinitions.OfType<ClassDefinition>().Where(e => e.ImplementedInterfaces.Contains(interfaceDefinition)));

          interfaceDefinition.SetExtendingInterfaces(
              typeDefinitions.OfType<InterfaceDefinition>().Where(e => e.ExtendedInterfaces.Contains(interfaceDefinition)));
        }
        else
        {
          throw new InvalidOperationException("The specified type definition is not supported.");
        }
      }

      var relationDefinitions = _relationBuilders
          .Select(e => e.BuildRelationDefinition(context))
          .ToArray();

      var groupedRelationEndPoints = relationDefinitions.SelectMany(e => e.EndPointDefinitions)
          .Where(e => !e.IsAnonymous)
          .GroupBy(e => e.TypeDefinition);
      foreach (var groupedRelationEndPoint in groupedRelationEndPoints)
        groupedRelationEndPoint.Key.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(groupedRelationEndPoint, true));

      foreach (var typeDefinition in typeDefinitions)
        typeDefinition.SetReadOnly();

      return typeDefinitions;
    }
  }
}
