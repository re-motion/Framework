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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  public class ReferenceSortExpressionDefinitionBuilder
  {
    private readonly List<Func<TypeDefinition, SortedPropertySpecification>> _sortedPropertySpecifications = new();

    public ReferenceSortExpressionDefinitionBuilder ()
    {
    }

    public ReferenceSortExpressionDefinitionBuilder Ascending (string propertyName)
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);

      AddSortedPropertySpecification(propertyName, SortOrder.Ascending);

      return this;
    }

    public ReferenceSortExpressionDefinitionBuilder Descending (string propertyName)
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);

      AddSortedPropertySpecification(propertyName, SortOrder.Descending);

      return this;
    }

    public Lazy<SortExpressionDefinition> Build (ReferenceDeferredRelationEndPointDefinition deferredRelationEndPointDefinition)
    {
      return new Lazy<SortExpressionDefinition>(
          () =>
          {
            var relationEndPointDefinition = deferredRelationEndPointDefinition.RelationEndPointDefinition;
            Assertion.IsNotNull(relationEndPointDefinition, "RelationEndPointDefinition was not initialized.");

            var oppositeTypeDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition().TypeDefinition;
            return new SortExpressionDefinition(_sortedPropertySpecifications.Select(e => e(oppositeTypeDefinition)));
          });
    }

    private void AddSortedPropertySpecification (string propertyName, SortOrder sortOrder)
    {
      _sortedPropertySpecifications.Add(
          typeDefinition =>
          {
            var fullPropertyName = ReferenceTypeDefinitionUtility.GetPropertyName(typeDefinition.Type, propertyName);
            var propertyDefinition = typeDefinition.GetMandatoryPropertyDefinition(fullPropertyName);
            return new SortedPropertySpecification(propertyDefinition, sortOrder);
          });
    }
  }
}
