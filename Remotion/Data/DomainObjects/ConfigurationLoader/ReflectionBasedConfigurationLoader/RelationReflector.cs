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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Used to create the <see cref="RelationDefinition"/> from a <see cref="IPropertyInformation"/>.</summary>
  public class RelationReflector : RelationReflectorBase<BidirectionalRelationAttribute>
  {
    public RelationReflector (
        TypeDefinition typeDefinition,
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider)
        : base(typeDefinition, propertyInfo, nameResolver, propertyMetadataProvider)
    {
    }

    public RelationDefinition GetMetadata (IDictionary<Type, TypeDefinition> typeDefinitions)
    {
      ArgumentUtility.CheckNotNull("typeDefinitions", typeDefinitions);

      var firstEndPoint = GetEndPointDefinition(TypeDefinition, PropertyInfo);
      var secondEndPoint = GetOppositeEndPointDefinition(typeDefinitions);

      var relationID = GetRelationID(firstEndPoint, secondEndPoint);
      return new RelationDefinition(relationID, firstEndPoint, secondEndPoint);
    }

    private string GetRelationID (IRelationEndPointDefinition first, IRelationEndPointDefinition second)
    {
      bool isFirstEndPointReal = !first.IsVirtual && !first.IsAnonymous;
      var endPoints = isFirstEndPointReal ? new { Left = first, Right = second } : new { Left = second, Right = first };

      Assertion.DebugAssert(endPoints.Left.IsAnonymous == false, "At least one relation endpoint must be a real endpoint.");
      var leftPropertyName = NameResolver.GetPropertyName(endPoints.Left.PropertyInfo);

      if (endPoints.Right.IsAnonymous)
      {
        return string.Format("{0}:{1}", endPoints.Left.TypeDefinition.Type.GetFullNameChecked(), leftPropertyName);
      }
      else
      {
        var rightPropertyName = NameResolver.GetPropertyName(endPoints.Right.PropertyInfo);
        return string.Format("{0}:{1}->{2}", endPoints.Left.TypeDefinition.Type.GetFullNameChecked(), leftPropertyName, rightPropertyName);
      }
    }

    private IRelationEndPointDefinition GetOppositeEndPointDefinition (IDictionary<Type, TypeDefinition> typeDefinitions)
    {
      if (!IsBidirectionalRelation)
        return CreateOppositeAnonymousRelationEndPointDefinition(typeDefinitions);
      Assertion.DebugIsNotNull(BidirectionalRelationAttribute, "BidirectionalRelationAttribute != null");

      var oppositeTypeDefinition = GetOppositeTypeDefinition(typeDefinitions);
      var oppositePropertyInfo = GetOppositePropertyInfo();
      if (oppositePropertyInfo == null)
        return new PropertyNotFoundRelationEndPointDefinition(oppositeTypeDefinition, BidirectionalRelationAttribute.OppositeProperty, typeof(void));
      else
        return GetEndPointDefinition(oppositeTypeDefinition, oppositePropertyInfo);
    }

    private AnonymousRelationEndPointDefinition CreateOppositeAnonymousRelationEndPointDefinition (IDictionary<Type, TypeDefinition> typeDefinitions)
    {
      var oppositeTypeDefinition = GetOppositeTypeDefinition(typeDefinitions);
      return new AnonymousRelationEndPointDefinition(oppositeTypeDefinition);
    }

    private IRelationEndPointDefinition GetEndPointDefinition (TypeDefinition typeDefinition, IPropertyInformation propertyInfo)
    {
      var endPointDefinition = typeDefinition.GetRelationEndPointDefinition(NameResolver.GetPropertyName(propertyInfo));
      if (endPointDefinition != null)
        return endPointDefinition;

      return new PropertyNotFoundRelationEndPointDefinition(typeDefinition, propertyInfo.Name, propertyInfo.PropertyType);
    }

    private TypeDefinition GetOppositeTypeDefinition (IDictionary<Type, TypeDefinition> typeDefinitions)
    {
      var type = ReflectionUtility.GetRelatedObjectTypeFromRelationProperty(PropertyInfo);
      if (type == null)
      {
        var notFoundTypeDefinition = new TypeDefinitionForUnresolvedRelationPropertyType(typeof(DomainObject), PropertyInfo);
        notFoundTypeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
        notFoundTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());
        return notFoundTypeDefinition;
      }

      var oppositeTypeDefinition = typeDefinitions.GetValueOrDefault(type);
      if (oppositeTypeDefinition == null)
      {
        var notFoundTypeDefinition = new TypeDefinitionForUnresolvedRelationPropertyType(type, PropertyInfo);
        notFoundTypeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
        notFoundTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());
        return notFoundTypeDefinition;
      }

      return oppositeTypeDefinition;
    }
  }
}
