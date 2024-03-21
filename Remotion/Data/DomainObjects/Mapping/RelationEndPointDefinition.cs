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
using System.Diagnostics;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Represents the non-anonymous, foreign-key side of a bidirectional or unidirectional relationship.
  /// </summary>
  [DebuggerDisplay("{GetType().Name}: {PropertyName}, Cardinality: {Cardinality}")]
  public class RelationEndPointDefinition : IRelationEndPointDefinition, IRelationEndPointDefinitionSetter
  {
    private RelationDefinition? _relationDefinition;
    private readonly TypeDefinition _typeDefinition;
    private readonly PropertyDefinition _propertyDefinition;
    private readonly bool _isMandatory;

    public RelationEndPointDefinition (PropertyDefinition propertyDefinition, bool isMandatory)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      if (!propertyDefinition.IsObjectID)
      {
        throw CreateMappingException(
            "Relation definition error: Property '{0}' of type '{1}' is of type '{2}', but non-virtual properties must be of type '{3}'.",
            propertyDefinition.PropertyName,
            propertyDefinition.TypeDefinition.Type.GetFullNameSafe(),
            propertyDefinition.PropertyType,
            typeof(ObjectID));
      }

      _typeDefinition = propertyDefinition.TypeDefinition;
      _isMandatory = isMandatory;
      _propertyDefinition = propertyDefinition;
    }

    public void SetRelationDefinition (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull("relationDefinition", relationDefinition);

      _relationDefinition = relationDefinition;
    }

    public bool HasRelationDefinitionBeenSet
    {
      get { return _relationDefinition != null; }
    }

    public RelationDefinition RelationDefinition
    {
      get
      {
        Assertion.IsNotNull(_relationDefinition, "RelationDefinition has not been set for this relation end point.");
        return _relationDefinition;
      }
    }

    public TypeDefinition TypeDefinition
    {
      get { return _typeDefinition; }
    }

    public string PropertyName
    {
      get { return _propertyDefinition.PropertyName; }
    }

    public bool IsMandatory
    {
      get { return _isMandatory; }
    }

    public CardinalityType Cardinality
    {
      get { return CardinalityType.One; }
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyDefinition.PropertyInfo; }
    }

    public bool IsVirtual
    {
      get { return false; }
    }

    public bool IsAnonymous
    {
      get { return false; }
    }

    public PropertyDefinition PropertyDefinition
    {
      get { return _propertyDefinition; }
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException(string.Format(message, args));
    }
  }
}
