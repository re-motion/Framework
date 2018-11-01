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
        ClassDefinition classDefinition,
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider)
        : base (classDefinition, propertyInfo, nameResolver, propertyMetadataProvider)
    {
    }

    public RelationDefinition GetMetadata (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      var firstEndPoint = GetEndPointDefinition (ClassDefinition, PropertyInfo);
      var secondEndPoint = GetOppositeEndPointDefinition (classDefinitions);

      var relationID = GetRelationID (firstEndPoint, secondEndPoint);
      return new RelationDefinition (relationID, firstEndPoint, secondEndPoint);
    }

    private string GetRelationID (IRelationEndPointDefinition first, IRelationEndPointDefinition second)
    {
      bool isFirstEndPointReal = !first.IsVirtual && !first.IsAnonymous;
      var endPoints = isFirstEndPointReal ? new { Left = first, Right = second } : new { Left = second, Right = first };

      var leftPropertyName = NameResolver.GetPropertyName (endPoints.Left.PropertyInfo);

      if (endPoints.Right.IsAnonymous)
      {
        return string.Format ("{0}:{1}", endPoints.Left.ClassDefinition.ClassType.FullName, leftPropertyName);
      }
      else
      {
        var rightPropertyName = NameResolver.GetPropertyName (endPoints.Right.PropertyInfo);
        return string.Format ("{0}:{1}->{2}", endPoints.Left.ClassDefinition.ClassType.FullName, leftPropertyName, rightPropertyName);
      }
    }

    private IRelationEndPointDefinition GetOppositeEndPointDefinition (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      if (!IsBidirectionalRelation)
        return CreateOppositeAnonymousRelationEndPointDefinition (classDefinitions);

      var oppositeClassDefinition = GetOppositeClassDefinition (classDefinitions);
      var oppositePropertyInfo = GetOppositePropertyInfo ();
      if (oppositePropertyInfo == null)
        return new PropertyNotFoundRelationEndPointDefinition (oppositeClassDefinition, BidirectionalRelationAttribute.OppositeProperty);
      else
        return GetEndPointDefinition (oppositeClassDefinition, oppositePropertyInfo);
    }

    private AnonymousRelationEndPointDefinition CreateOppositeAnonymousRelationEndPointDefinition (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      var oppositeClassDefinition = GetOppositeClassDefinition (classDefinitions);
      return new AnonymousRelationEndPointDefinition (oppositeClassDefinition);
    }

    private IRelationEndPointDefinition GetEndPointDefinition (ClassDefinition classDefinition, IPropertyInformation propertyInfo)
    {
      var endPointDefinition = classDefinition.GetRelationEndPointDefinition (NameResolver.GetPropertyName (propertyInfo));
      if (endPointDefinition != null)
        return endPointDefinition;

      return new PropertyNotFoundRelationEndPointDefinition (classDefinition, propertyInfo.Name);
    }

    private ClassDefinition GetOppositeClassDefinition (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      var type = ReflectionUtility.GetRelatedObjectTypeFromRelationProperty (PropertyInfo);
      var oppositeClassDefinition = classDefinitions.GetValueOrDefault (type);
      if (oppositeClassDefinition == null)
      {
        var notFoundClassDefinition = new ClassDefinitionForUnresolvedRelationPropertyType (type.Name, type, PropertyInfo);
        notFoundClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection ());
        notFoundClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
        return notFoundClassDefinition;
      }

      return oppositeClassDefinition;
    }
  }
}