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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public class RelationDefinitionChecker
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public RelationDefinitionChecker ()
    {
    }

    // methods and properties

    public void Check (
        IReadOnlyCollection<RelationDefinition> expectedDefinitions,
        IDictionary<string, RelationDefinition> actualDefinitions,
        bool ignoreUnknown)
    {
      ArgumentUtility.CheckNotNull("expectedDefinitions", expectedDefinitions);
      ArgumentUtility.CheckNotNull("actualDefinitions", actualDefinitions);

      if (!ignoreUnknown)
        Assert.That(actualDefinitions.Count, Is.EqualTo(expectedDefinitions.Count()), "Number of relation definitions does not match.");

      foreach (RelationDefinition expectedDefinition in expectedDefinitions)
      {
        RelationDefinition actualDefinition = actualDefinitions.GetValueOrDefault(expectedDefinition.ID);
        Assert.That(actualDefinition, Is.Not.Null, $"Relation '{expectedDefinition.ID}' was not found.");
        Check(expectedDefinition, actualDefinition);
      }
    }

    public void Check (RelationDefinition expectedDefinition, RelationDefinition actualDefinition)
    {
      ArgumentUtility.CheckNotNull("expectedDefinition", expectedDefinition);
      ArgumentUtility.CheckNotNull("actualDefinition", actualDefinition);

      Assert.That(actualDefinition.ID, Is.EqualTo(expectedDefinition.ID), "IDs of relation definitions do not match.");

      CheckEndPointDefinitions(expectedDefinition, actualDefinition);
    }

    private void CheckEndPointDefinitions (RelationDefinition expectedRelationDefinition, RelationDefinition actualRelationDefinition)
    {
      foreach (IRelationEndPointDefinition expectedEndPointDefinition in expectedRelationDefinition.EndPointDefinitions)
      {
        IRelationEndPointDefinition actualEndPointDefinition = actualRelationDefinition.GetEndPointDefinition(
          expectedEndPointDefinition.ClassDefinition.ID, expectedEndPointDefinition.PropertyName);

        Assert.That(
            actualEndPointDefinition,
            Is.Not.Null,
            $"End point definition was not found ("
            + $"relation definition: '{expectedRelationDefinition.ID}', "
            + $"class: '{expectedEndPointDefinition.ClassDefinition.ID}', "
            + $"property name: '{expectedEndPointDefinition.PropertyName}').");

        var endPointDefinitionChecker = new RelationEndPointDefinitionChecker();
        endPointDefinitionChecker.Check(expectedEndPointDefinition, actualEndPointDefinition, true);

        Assert.That(
            actualEndPointDefinition.RelationDefinition,
            Is.SameAs(actualRelationDefinition),
            $"End point definition does not reference the correct relation definition ("
            + $"relation definition: '{actualRelationDefinition.ID}', "
            + $"class: '{actualEndPointDefinition.ClassDefinition.ID}', "
            + $"property name: '{actualEndPointDefinition.PropertyName}').");
      }
    }
  }
}
