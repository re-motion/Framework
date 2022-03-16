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
using System.Reflection;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  public class ReferenceTypeDefinitionRelationEndPointBuilder<TType>
      where TType : IDomainObject
  {
    private abstract record RelationEndPointKind
    {
      public abstract IRelationEndPointDefinition Build (TypeDefinition typeDefinition, IPropertyInformation propertyInfo, string propertyName, bool isMandatory);
    }

    private record NormalRelationEndPointKind : RelationEndPointKind
    {
      /// <inheritdoc />
      public override IRelationEndPointDefinition Build (TypeDefinition typeDefinition, IPropertyInformation propertyInfo, string propertyName, bool isMandatory)
      {
        var propertyDefinition = typeDefinition.GetMandatoryPropertyDefinition(propertyName);
        return new RelationEndPointDefinition(propertyDefinition, isMandatory);
      }
    }

    private record AnonymousRelationEndPointKind : RelationEndPointKind
    {
      /// <inheritdoc />
      public override IRelationEndPointDefinition Build (TypeDefinition typeDefinition, IPropertyInformation propertyInfo, string propertyName, bool isMandatory)
      {
        throw new InvalidOperationException("Anonymous relation end points cannot build themselves.");
      }
    }

    private record VirtualObjectRelationEndPointKind : RelationEndPointKind
    {
      /// <inheritdoc />
      public override IRelationEndPointDefinition Build (TypeDefinition typeDefinition, IPropertyInformation propertyInfo, string propertyName, bool isMandatory)
      {
        return new VirtualObjectRelationEndPointDefinition(
            typeDefinition,
            propertyName,
            isMandatory,
            propertyInfo);
      }
    }

    private record DomainObjectCollectionRelationEndPointKind : RelationEndPointKind
    {
      private readonly ReferenceSortExpressionDefinitionBuilder _builder;

      public DomainObjectCollectionRelationEndPointKind (ReferenceSortExpressionDefinitionBuilder builder)
      {
        ArgumentUtility.CheckNotNull(nameof(builder), builder);

        _builder = builder;
      }

      /// <inheritdoc />
      public override IRelationEndPointDefinition Build (TypeDefinition typeDefinition, IPropertyInformation propertyInfo, string propertyName, bool isMandatory)
      {
        var deferredRelationEndPointDefinition = new ReferenceDeferredRelationEndPointDefinition();
        var relationEndPoint = new DomainObjectCollectionRelationEndPointDefinition(
            typeDefinition,
            propertyName,
            isMandatory,
            _builder.Build(deferredRelationEndPointDefinition),
            propertyInfo);
        deferredRelationEndPointDefinition.RelationEndPointDefinition = relationEndPoint;

        return relationEndPoint;
      }
    }

    private record VirtualCollectionRelationEndPointKind : RelationEndPointKind
    {
      private readonly ReferenceSortExpressionDefinitionBuilder _builder;

      public VirtualCollectionRelationEndPointKind (ReferenceSortExpressionDefinitionBuilder builder)
      {
        ArgumentUtility.CheckNotNull(nameof(builder), builder);

        _builder = builder;
      }

      /// <inheritdoc />
      public override IRelationEndPointDefinition Build (TypeDefinition typeDefinition, IPropertyInformation propertyInfo, string propertyName, bool isMandatory)
      {
        var deferredRelationEndPointDefinition = new ReferenceDeferredRelationEndPointDefinition();
        var relationEndPointDefinition = new VirtualCollectionRelationEndPointDefinition(
            typeDefinition,
            propertyName,
            isMandatory,
            _builder.Build(deferredRelationEndPointDefinition),
            propertyInfo);
        deferredRelationEndPointDefinition.RelationEndPointDefinition = relationEndPointDefinition;

        return relationEndPointDefinition;
      }
    }

    [CanBeNull]
    public string PropertyName { get; private set; }

    public bool IsMandatory { get; private set; }

    private RelationEndPointKind Kind { get; set; }

    public ReferenceTypeDefinitionRelationEndPointBuilder ()
    {
      Kind = new NormalRelationEndPointKind();
    }

    public bool IsAnonymous => Kind is AnonymousRelationEndPointKind;

    public ReferenceTypeDefinitionRelationEndPointBuilder<TType> AsAnonymousRelation ()
    {
      Kind = new AnonymousRelationEndPointKind();

      return this;
    }

    public ReferenceTypeDefinitionRelationEndPointBuilder<TType> AsVirtualObjectRelation ()
    {
      Kind = new VirtualObjectRelationEndPointKind();

      return this;
    }

    public ReferenceTypeDefinitionRelationEndPointBuilder<TType> AsDomainObjectCollectionRelation (Action<ReferenceSortExpressionDefinitionBuilder> configureSortExpression)
    {
      var sortExpressionDefinitionBuilder = new ReferenceSortExpressionDefinitionBuilder();
      configureSortExpression(sortExpressionDefinitionBuilder);

      Kind = new DomainObjectCollectionRelationEndPointKind(sortExpressionDefinitionBuilder);

      return this;
    }

    public ReferenceTypeDefinitionRelationEndPointBuilder<TType> AsVirtualCollectionRelation (Action<ReferenceSortExpressionDefinitionBuilder> configureSortExpression)
    {
      var sortExpressionDefinitionBuilder = new ReferenceSortExpressionDefinitionBuilder();
      configureSortExpression(sortExpressionDefinitionBuilder);

      Kind = new VirtualCollectionRelationEndPointKind(sortExpressionDefinitionBuilder);

      return this;
    }

    public ReferenceTypeDefinitionRelationEndPointBuilder<TType> WithProperty (string propertyName)
    {
      ArgumentUtility.CheckNotNull(nameof(propertyName), propertyName);

      PropertyName = propertyName;

      return this;
    }

    public ReferenceTypeDefinitionRelationEndPointBuilder<TType> SetIsMandatory (bool isMandatory = true)
    {
      IsMandatory = isMandatory;

      return this;
    }

    public IRelationEndPointDefinition BuildRelationEndPointDefinition (TypeDefinition typeDefinition)
    {
      if (Kind is AnonymousRelationEndPointKind)
        throw new InvalidOperationException("An anonymous relation end point cannot be built.");

      var hasProperty = PropertyName != null;
      if (!hasProperty)
        throw new InvalidOperationException("A non-anonymous relation end point requires a property.");

      var property = typeDefinition.Type.GetProperty(
          PropertyName,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      if (property == null)
        throw new InvalidOperationException($"Cannot find the specified relation end point property '{PropertyName}'.");

      var propertyInfo = PropertyInfoAdapter.Create(property);
      var propertyName = ReferenceTypeDefinitionUtility.GetPropertyName(typeof(TType), PropertyName);
      return Kind.Build(typeDefinition, propertyInfo, propertyName, IsMandatory);
    }
  }
}
