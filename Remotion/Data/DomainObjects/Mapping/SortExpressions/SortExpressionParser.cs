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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.SortExpressions
{
  /// <summary>
  /// Parses a <see cref="BidirectionalRelationAttribute.SortExpression"/> into a <see cref="SortExpressionDefinition"/>.
  /// </summary>
  public class SortExpressionParser
  {
    private readonly TypeDefinition _typeDefinition;

    public SortExpressionParser (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      _typeDefinition = typeDefinition;
    }

    public SortExpressionDefinition? Parse (string sortExpression)
    {
      ArgumentUtility.CheckNotNull("sortExpression", sortExpression);

      try
      {
        var sortedProperties = (from s in sortExpression.Split(',')
                                let specs = s.Trim()
                                where !string.IsNullOrEmpty(specs)
                                select ParseSortedPropertySpecification(specs)).ToList();
        if (sortedProperties.Count == 0)
          return null;
        else
          return new SortExpressionDefinition(sortedProperties);
      }
      catch (MappingException ex)
      {
        var message = string.Format("SortExpression '{0}' cannot be parsed: {1}", sortExpression, ex.Message);
        throw new MappingException(message);
      }

    }

    private SortedPropertySpecification ParseSortedPropertySpecification (string sortedPropertySpecification)
    {
      var splitSpecification = SplitSortedPropertySpecification(sortedPropertySpecification);

      var propertyDefinition = ParsePropertyName(splitSpecification.Item1);
      var sortOrder = ParseOrderSpecification(splitSpecification.Item2);

      return new SortedPropertySpecification(propertyDefinition, sortOrder);
    }

    private Tuple<string, string?> SplitSortedPropertySpecification (string sortedPropertySpecification)
    {
      var parts = sortedPropertySpecification.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();

      if (parts.Length > 2)
      {
        var message = string.Format("Expected 1 or 2 parts (a property name and an optional identifier), found {0} parts instead.", parts.Length);
        throw new MappingException(message);
      }

      return Tuple.Create(parts[0], parts.Length > 1 ? parts[1] : null);
    }

    private SortOrder ParseOrderSpecification (string? orderSpecification)
    {
      if (orderSpecification == null)
        return SortOrder.Ascending;

      switch (orderSpecification.ToLower())
      {
        case "asc":
          return SortOrder.Ascending;
        case "desc":
          return SortOrder.Descending;
        default:
          var message = string.Format("'{0}' is not a valid sort order. Expected 'asc' or 'desc'.", orderSpecification);
          throw new MappingException(message);
      }
    }

    private PropertyDefinition ParsePropertyName (string fullOrShortPropertyName)
    {
      // Try as full name first, only if that doesn't match, assume it must be a short name
      // If that still doesn't produce a match, search all derived classes for a full name match. Derived classes are not searched for short names.
      var propertyAccessorData =
          _typeDefinition.PropertyAccessorDataCache.GetPropertyAccessorData(fullOrShortPropertyName)
          ?? _typeDefinition.PropertyAccessorDataCache.FindPropertyAccessorData(_typeDefinition.Type, fullOrShortPropertyName)
          ?? InlineTypeDefinitionWalker.WalkDescendants(
              _typeDefinition,
              classDefinition => classDefinition.PropertyAccessorDataCache.GetPropertyAccessorData(fullOrShortPropertyName),
              interfaceDefinition => interfaceDefinition.PropertyAccessorDataCache.GetPropertyAccessorData(fullOrShortPropertyName),
              match: propertyAccessorData => propertyAccessorData != null);

      if (propertyAccessorData == null)
      {
        var message = string.Format(
            "'{0}' is not a valid mapped property name. Expected the .NET property name of a property declared by the '{1}' class or its base classes. "
            + "Alternatively, to resolve ambiguities or to use a property declared by a mixin or a derived class of '{1}', the full unique re-store "
            + "property identifier can be specified.",
            fullOrShortPropertyName,
            _typeDefinition.Type.Name);
        throw new MappingException(message);
      }

      if (propertyAccessorData.PropertyDefinition == null)
      {
        Assertion.IsNotNull(propertyAccessorData.RelationEndPointDefinition);

        var message = string.Format(
            "The property '{0}' is a virtual relation end point. SortExpressions can only contain relation end points if the object to be sorted "
            + "contains the foreign key.",
            propertyAccessorData.PropertyIdentifier);
        throw new MappingException(message);
      }

      return propertyAccessorData.PropertyDefinition;
    }
  }
}
