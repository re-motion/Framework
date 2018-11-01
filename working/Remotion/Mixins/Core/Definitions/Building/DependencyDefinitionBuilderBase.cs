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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public abstract class DependencyDefinitionBuilderBase
  {
    protected abstract RequirementDefinitionBase GetRequirement (Type type);
    protected abstract RequirementDefinitionBase CreateRequirement (Type type);
    protected abstract void AddRequirement (RequirementDefinitionBase requirement);
    protected abstract DependencyDefinitionBase CreateDependency (RequirementDefinitionBase requirement, DependencyDefinitionBase aggregator);
    protected abstract void AddDependency (DependencyDefinitionBase dependency);

    public void Apply (IEnumerable<Type> dependencyTypes)
    {
      ArgumentUtility.CheckNotNull ("dependencyTypes", dependencyTypes);

      foreach (Type type in dependencyTypes)
      {
        if (!type.Equals (typeof (object))) // dependencies to System.Object are always fulfilled and not explicitly added to the configuration
        {
          DependencyDefinitionBase dependency = BuildDependency (type, null);
          AddDependency (dependency);
        }
      }
    }

    private DependencyDefinitionBase BuildDependency(Type type, DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      RequirementDefinitionBase requirement = GetRequirement (type);
      if (requirement == null)
      {
        requirement = CreateRequirement (type);
        AddRequirement(requirement);
      }
      DependencyDefinitionBase dependency = CreateDependency (requirement, aggregator);
      requirement.RequiringDependencies.Add (dependency);
      CheckForAggregate (dependency);
      return dependency;
    }

    private void CheckForAggregate (DependencyDefinitionBase dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);

      if (dependency.RequiredType.IsAggregatorInterface)
      {
        foreach (Type type in dependency.RequiredType.Type.GetInterfaces ())
        {
          DependencyDefinitionBase innerDependency = BuildDependency (type, dependency);
          dependency.AggregatedDependencies.Add (innerDependency);
        }
      }
    }
  }
}
