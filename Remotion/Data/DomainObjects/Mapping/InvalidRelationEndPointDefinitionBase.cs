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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Holds information about a relation end point that could not be resolved.
  /// </summary>
  public abstract class InvalidRelationEndPointDefinitionBase : IRelationEndPointDefinition, IRelationEndPointDefinitionSetter
  {
    private readonly TypeDefinition _typeDefinition;
    private readonly string _propertyName;
    private readonly Type _propertyType;
    private RelationDefinition? _relationDefinition;
    private readonly IPropertyInformation _propertyInformation;

    protected InvalidRelationEndPointDefinitionBase (TypeDefinition typeDefinition, string propertyName, Type propertyType)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);
      ArgumentUtility.CheckNotNull("propertyName", propertyName);
      ArgumentUtility.CheckNotNull("propertyType", propertyType);

      _typeDefinition = typeDefinition;
      _propertyName = propertyName;
      _propertyType = propertyType;
      _propertyInformation = new InvalidPropertyInformation(TypeAdapter.Create(_typeDefinition.Type), propertyName, propertyType);
    }

    public TypeDefinition TypeDefinition
    {
      get { return _typeDefinition; }
    }

    public string PropertyName
    {
      get { return _propertyName;  }
    }

    public RelationDefinition RelationDefinition
    {
      get
      {
        Assertion.IsNotNull(_relationDefinition, "RelationDefinition has not been set for this relation end point.");
        return _relationDefinition;
      }
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInformation; }
    }

    public bool IsMandatory
    {
      get { throw new NotImplementedException(); }
    }

    public CardinalityType Cardinality
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsVirtual
    {
      get { return false;  }
    }

    public bool IsAnonymous
    {
      get { return false;  }
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
  }
}
