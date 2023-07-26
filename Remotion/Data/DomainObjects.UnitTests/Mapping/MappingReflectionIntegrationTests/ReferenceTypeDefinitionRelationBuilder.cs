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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  public class ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> : IReferenceTypeDefinitionRelationBuilder
      where TFirst : IDomainObject
      where TSecond : IDomainObject
  {
    private readonly ReferenceTypeDefinitionRelationEndPointBuilder<TFirst> _firstEndpoint;
    private readonly ReferenceTypeDefinitionRelationEndPointBuilder<TSecond> _secondEndpoint;

    [CanBeNull]
    public string ID { get; set; }

    public ReferenceTypeDefinitionRelationBuilder ()
    {
      _firstEndpoint = new();
      _secondEndpoint = new();
    }

    /// <inheritdoc />
    public Type LeftType => typeof(TFirst);

    /// <inheritdoc />
    public Type RightType => typeof(TSecond);

    public ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> WithID (string id)
    {
      ID = id;

      return this;
    }

    public ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> WithFirst (Action<ReferenceTypeDefinitionRelationEndPointBuilder<TFirst>> configure)
    {
      configure(_firstEndpoint);

      return this;
    }

    public ReferenceTypeDefinitionRelationBuilder<TFirst, TSecond> WithSecond (Action<ReferenceTypeDefinitionRelationEndPointBuilder<TSecond>> configure)
    {
      configure(_secondEndpoint);

      return this;
    }

    /// <inheritdoc />
    public RelationDefinition BuildRelationDefinition (ReferenceTypeDefinitionBuilderContext context)
    {
      ArgumentUtility.CheckNotNull(nameof(context), context);

      var firstIsAnonymous = _firstEndpoint.IsAnonymous;
      var secondIsAnonymous = _secondEndpoint.IsAnonymous;
      if (firstIsAnonymous && secondIsAnonymous)
        throw new InvalidOperationException("Only one endpoint in a relation can be anonymous.");

      var firstTypeDefinition = context.ResolveTypeDefinition(typeof(TFirst));
      var secondTypeDefinition = context.ResolveTypeDefinition(typeof(TSecond));

      var firstRelationEndPointDefinition = !firstIsAnonymous
          ? _firstEndpoint.BuildRelationEndPointDefinition(firstTypeDefinition)
          : new AnonymousRelationEndPointDefinition(secondTypeDefinition);

      var secondRelationEndPointDefinition = !secondIsAnonymous
          ? _secondEndpoint.BuildRelationEndPointDefinition(secondTypeDefinition)
          : new AnonymousRelationEndPointDefinition(firstTypeDefinition);

      var relationDefinition = new RelationDefinition(
          ID ?? GenerateID(firstRelationEndPointDefinition, secondRelationEndPointDefinition),
          firstRelationEndPointDefinition,
          secondRelationEndPointDefinition);

      ((IRelationEndPointDefinitionSetter)firstRelationEndPointDefinition).SetRelationDefinition(relationDefinition);
      ((IRelationEndPointDefinitionSetter)secondRelationEndPointDefinition).SetRelationDefinition(relationDefinition);

      return relationDefinition;
    }

    private string GenerateID (IRelationEndPointDefinition first, IRelationEndPointDefinition second)
    {
      var isFirstEndPointReal = !first.IsVirtual && !first.IsAnonymous;
      var endPoints = isFirstEndPointReal ? new { Left = first, Right = second } : new { Left = second, Right = first };

      Assertion.DebugAssert(endPoints.Left.IsAnonymous == false, "At least one relation endpoint must be a real endpoint.");
      var firstPropertyName = ReferenceTypeDefinitionUtility.GetPropertyName(endPoints.Left.PropertyInfo);

      if (endPoints.Right.IsAnonymous)
      {
        return $"{endPoints.Left.TypeDefinition.Type.GetFullNameChecked()}:{firstPropertyName}";
      }
      else
      {
        var secondPropertyName = ReferenceTypeDefinitionUtility.GetPropertyName(endPoints.Right.PropertyInfo);
        return $"{endPoints.Left.TypeDefinition.Type.GetFullNameChecked()}:{firstPropertyName}->{secondPropertyName}";
      }
    }
  }
}
