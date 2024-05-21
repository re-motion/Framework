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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public class RelationEndPointDefinitionChecker
  {
    public void Check (
        RelationEndPointDefinitionCollection expectedDefinitions, RelationEndPointDefinitionCollection actualDefinitions, bool checkRelationDefinition)
    {
      Assert.That(actualDefinitions.Count, Is.EqualTo(expectedDefinitions.Count), "Number of relation end points does not match.");

      foreach (var expectedDefinition in expectedDefinitions)
      {
        var actualDefinition = actualDefinitions[expectedDefinition.PropertyName];
        Assert.That(actualDefinition, Is.Not.Null, $"Relation end point '{expectedDefinition.PropertyName}' was not found.");
        Check(expectedDefinition, actualDefinition, checkRelationDefinition);
      }
    }

    public void Check (
        IRelationEndPointDefinition expectedEndPointDefinition, IRelationEndPointDefinition actualEndPointDefinition, bool checkRelationDefinition)
    {
      Assert.That(
          actualEndPointDefinition.GetType(),
          Is.EqualTo(expectedEndPointDefinition.GetType()),
          $"End point definitions (property name: '{expectedEndPointDefinition.PropertyName}') are not of same type.");

      Assert.That(
          actualEndPointDefinition.ClassDefinition.ID,
          Is.EqualTo(expectedEndPointDefinition.ClassDefinition.ID),
          $"ClassDefinition of end point definitions (property name: '{expectedEndPointDefinition.PropertyName}') does not match.");

      if (checkRelationDefinition)
      {
        Assert.That(
            actualEndPointDefinition.RelationDefinition.ID,
            Is.EqualTo(expectedEndPointDefinition.RelationDefinition.ID),
            $"RelationDefinition of end point definitions (property name: '{expectedEndPointDefinition.PropertyName}') does not match.");
      }

      Assert.That(
          actualEndPointDefinition.PropertyName,
          Is.EqualTo(expectedEndPointDefinition.PropertyName),
          $"PropertyName of end point definitions (property name: '{expectedEndPointDefinition.PropertyName}') does not match.");

      if (!expectedEndPointDefinition.IsAnonymous)
      {
        Assert.That(
            actualEndPointDefinition.PropertyInfo.PropertyType,
            Is.EqualTo(expectedEndPointDefinition.PropertyInfo.PropertyType),
            $"PropertyType of end point definitions (property name: '{expectedEndPointDefinition.PropertyName}') does not match.");
      }

      Assert.That(
          actualEndPointDefinition.IsMandatory,
          Is.EqualTo(expectedEndPointDefinition.IsMandatory),
          $"IsMandatory of end point definitions (property name: '{expectedEndPointDefinition.PropertyName}') does not match. ");

      Assert.That(
          actualEndPointDefinition.IsAnonymous,
          Is.EqualTo(expectedEndPointDefinition.IsAnonymous),
          $"IsAnonymous of end point definitions (property name: '{expectedEndPointDefinition.PropertyName}') does not match.");

      Assert.That(
          actualEndPointDefinition.Cardinality,
          Is.EqualTo(expectedEndPointDefinition.Cardinality),
          $"Cardinality of end point definitions (property name: '{expectedEndPointDefinition.PropertyName}') does not match.");


      if (expectedEndPointDefinition is DomainObjectCollectionRelationEndPointDefinition)
      {
        var expectedCollectionRelationEndPointDefinition = (DomainObjectCollectionRelationEndPointDefinition)expectedEndPointDefinition;
        var actualCollectionEndPointDefinition = (DomainObjectCollectionRelationEndPointDefinition)actualEndPointDefinition;

        var expectedSortExpressionDefinition = expectedCollectionRelationEndPointDefinition.GetSortExpression();
        var actualSortExpressionDefinition = actualCollectionEndPointDefinition.GetSortExpression();
        Assert.That(
            actualSortExpressionDefinition?.ToString(),
            Is.EqualTo(expectedSortExpressionDefinition?.ToString()),
            $"SortExpression of end point definitions (property name: '{expectedCollectionRelationEndPointDefinition.PropertyName}') does not match.");
      }

      if (expectedEndPointDefinition is VirtualCollectionRelationEndPointDefinition)
      {
        var expectedCollectionRelationEndPointDefinition = (VirtualCollectionRelationEndPointDefinition)expectedEndPointDefinition;
        var actualCollectionEndPointDefinition = (VirtualCollectionRelationEndPointDefinition)actualEndPointDefinition;

        var expectedSortExpressionDefinition = expectedCollectionRelationEndPointDefinition.GetSortExpression();
        var actualSortExpressionDefinition = actualCollectionEndPointDefinition.GetSortExpression();
        Assert.That(
            actualSortExpressionDefinition?.ToString(),
            Is.EqualTo(expectedSortExpressionDefinition?.ToString()),
            $"SortExpression of end point definitions (property name: '{expectedCollectionRelationEndPointDefinition.PropertyName}') does not match.");
      }
    }
  }
}
