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
  // Note: No properties and methods of this class are inheritance-aware!
  public class RelationDefinition
  {
    private readonly string _id;
    private readonly IRelationEndPointDefinition _endPointDefinition1;
    private readonly IRelationEndPointDefinition _endPointDefinition2;

    public RelationDefinition (
        string id,
        IRelationEndPointDefinition endPointDefinition1,
        IRelationEndPointDefinition endPointDefinition2)
    {
      ArgumentUtility.CheckNotNullOrEmpty("id", id);
      ArgumentUtility.CheckNotNull("endPointDefinition1", endPointDefinition1);
      ArgumentUtility.CheckNotNull("endPointDefinition2", endPointDefinition2);

      _id = id;

      _endPointDefinition1 = endPointDefinition1;
      _endPointDefinition2 = endPointDefinition2;
    }

    public IRelationEndPointDefinition GetMandatoryOppositeRelationEndPointDefinition (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull("endPointDefinition", endPointDefinition);

      IRelationEndPointDefinition? oppositeEndPointDefinition = GetOppositeEndPointDefinition(endPointDefinition);
      if (oppositeEndPointDefinition == null)
      {
        throw CreateMappingException(
            "Relation '{0}' has no association with type '{1}' and property '{2}'.",
            ID,
            endPointDefinition.TypeDefinition.Type.GetFullNameSafe(),
            endPointDefinition.PropertyName);
      }

      return oppositeEndPointDefinition;
    }

    public string ID
    {
      get { return _id; }
    }

    public RelationKindType RelationKind
    {
      get
      {
        if (_endPointDefinition1 is AnonymousRelationEndPointDefinition || _endPointDefinition2 is AnonymousRelationEndPointDefinition)
          return RelationKindType.Unidirectional;
        else if (_endPointDefinition1.Cardinality == CardinalityType.Many || _endPointDefinition2.Cardinality == CardinalityType.Many)
          return RelationKindType.OneToMany;
        else
          return RelationKindType.OneToOne;
      }
    }

    public IRelationEndPointDefinition[] EndPointDefinitions
    {
      get { return new[] { _endPointDefinition1, _endPointDefinition2 }; }
    }

    public IRelationEndPointDefinition? GetEndPointDefinition (Type type, string propertyName)
    {
      ArgumentUtility.CheckNotNull("type", type);

      if (_endPointDefinition1.TypeDefinition.Type == type && _endPointDefinition1.PropertyName == propertyName)
        return _endPointDefinition1;

      if (_endPointDefinition2.TypeDefinition.Type == type && _endPointDefinition2.PropertyName == propertyName)
        return _endPointDefinition2;

      return null;
    }

    public IRelationEndPointDefinition? GetOppositeEndPointDefinition (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull("endPointDefinition", endPointDefinition);

      if (endPointDefinition == _endPointDefinition1)
        return _endPointDefinition2;
      else if (endPointDefinition == _endPointDefinition2)
        return _endPointDefinition1;
      else
        return null;
    }

    public IRelationEndPointDefinition? GetOppositeEndPointDefinition (Type type, string propertyName)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var matchingEndPointDefinition = GetEndPointDefinition(type, propertyName);
      if (matchingEndPointDefinition == null)
        return null;

      return GetOppositeEndPointDefinition(matchingEndPointDefinition);
    }

    public bool IsEndPoint (Type type, string propertyName)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return GetEndPointDefinition(type, propertyName) != null;
    }

    public bool Contains (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull("endPointDefinition", endPointDefinition);

      return endPointDefinition == _endPointDefinition1 || endPointDefinition == _endPointDefinition2;
    }

    public override string ToString ()
    {
      return GetType().Name + ": " + _id;
    }

    private MappingException CreateMappingException (string message, params object?[] args)
    {
      return new MappingException(String.Format(message, args));
    }
  }
}
