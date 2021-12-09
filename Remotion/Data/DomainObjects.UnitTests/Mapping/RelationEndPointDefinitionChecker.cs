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
      Assert.AreEqual(expectedDefinitions.Count, actualDefinitions.Count, "Number of relation end points does not match.");

      foreach (var expectedDefinition in expectedDefinitions)
      {
        var actualDefinition = actualDefinitions[expectedDefinition.PropertyName];
        Assert.IsNotNull(actualDefinition, "Relation end point '{0}' was not found.", expectedDefinition.PropertyName);
        Check(expectedDefinition, actualDefinition, checkRelationDefinition);
      }
    }

    public void Check (
        IRelationEndPointDefinition expectedEndPointDefinition, IRelationEndPointDefinition actualEndPointDefinition, bool checkRelationDefinition)
    {
      Assert.AreEqual(
          expectedEndPointDefinition.GetType(),
          actualEndPointDefinition.GetType(),
          "End point definitions (property name: '{0}') are not of same type.",
          expectedEndPointDefinition.PropertyName);

      // TODO R2I Mapping: Do actually checking for TypeDefinition not only ClassDefinition
      Assert.AreEqual(
          ((ClassDefinition)expectedEndPointDefinition.TypeDefinition).ID,
          ((ClassDefinition)actualEndPointDefinition.TypeDefinition).ID,
          "ClassDefinition of end point definitions (property name: '{0}') does not match.",
          expectedEndPointDefinition.PropertyName);

      if (checkRelationDefinition)
      {
        Assert.AreEqual(
            expectedEndPointDefinition.RelationDefinition.ID,
            actualEndPointDefinition.RelationDefinition.ID,
            "RelationDefinition of end point definitions (property name: '{0}') does not match.",
            expectedEndPointDefinition.PropertyName);
      }

      Assert.AreEqual(
          expectedEndPointDefinition.PropertyName,
          actualEndPointDefinition.PropertyName,
          "PropertyName of end point definitions (property name: '{0}') does not match.",
          expectedEndPointDefinition.PropertyName);

      if (!expectedEndPointDefinition.IsAnonymous)
      {
        Assert.AreEqual(
            expectedEndPointDefinition.PropertyInfo.PropertyType,
            actualEndPointDefinition.PropertyInfo.PropertyType,
            "PropertyType of end point definitions (property name: '{0}') does not match.",
            expectedEndPointDefinition.PropertyName);
      }

      Assert.AreEqual(
          expectedEndPointDefinition.IsMandatory,
          actualEndPointDefinition.IsMandatory,
          "IsMandatory of end point definitions (property name: '{0}') does not match. ",
          expectedEndPointDefinition.PropertyName);

      Assert.AreEqual(
          expectedEndPointDefinition.IsAnonymous,
          actualEndPointDefinition.IsAnonymous,
          "IsAnonymous of end point definitions (property name: '{0}') does not match.",
          expectedEndPointDefinition.PropertyName);

      Assert.AreEqual(
          expectedEndPointDefinition.Cardinality,
          actualEndPointDefinition.Cardinality,
          "Cardinality of end point definitions (property name: '{0}') does not match.",
          expectedEndPointDefinition.PropertyName);


      if (expectedEndPointDefinition is DomainObjectCollectionRelationEndPointDefinition)
      {
        var expectedCollectionRelationEndPointDefinition = (DomainObjectCollectionRelationEndPointDefinition)expectedEndPointDefinition;
        var actualCollectionEndPointDefinition = (DomainObjectCollectionRelationEndPointDefinition)actualEndPointDefinition;

        var expectedSortExpressionDefinition = expectedCollectionRelationEndPointDefinition.GetSortExpression();
        var actualSortExpressionDefinition = actualCollectionEndPointDefinition.GetSortExpression();
        Assert.AreEqual(
            expectedSortExpressionDefinition?.ToString(),
            actualSortExpressionDefinition?.ToString(),
            "SortExpression of end point definitions (property name: '{0}') does not match.",
            expectedCollectionRelationEndPointDefinition.PropertyName);
      }

      if (expectedEndPointDefinition is VirtualCollectionRelationEndPointDefinition)
      {
        var expectedCollectionRelationEndPointDefinition = (VirtualCollectionRelationEndPointDefinition)expectedEndPointDefinition;
        var actualCollectionEndPointDefinition = (VirtualCollectionRelationEndPointDefinition)actualEndPointDefinition;

        var expectedSortExpressionDefinition = expectedCollectionRelationEndPointDefinition.GetSortExpression();
        var actualSortExpressionDefinition = actualCollectionEndPointDefinition.GetSortExpression();
        Assert.AreEqual(
            expectedSortExpressionDefinition?.ToString(),
            actualSortExpressionDefinition?.ToString(),
            "SortExpression of end point definitions (property name: '{0}') does not match.",
            expectedCollectionRelationEndPointDefinition.PropertyName);
      }
    }
  }
}
