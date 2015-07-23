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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{RequiredType.Type}, Depender = {Depender}")]
  public abstract class DependencyDefinitionBase : IVisitableDefinition
  {
    private readonly UniqueDefinitionCollection<Type, DependencyDefinitionBase> _aggregatedDependencies;

    private readonly RequirementDefinitionBase _requirement; // the required face or base interface
    private readonly DependencyDefinitionBase _aggregator; // the outer dependency containing this dependency, if defined indirectly

    public DependencyDefinitionBase (RequirementDefinitionBase requirement, DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckType ("aggregator", aggregator, GetType ());

      _requirement = requirement;
      _aggregator = aggregator;

      _aggregatedDependencies = new UniqueDefinitionCollection<Type, DependencyDefinitionBase> (
          delegate (DependencyDefinitionBase d) { return d.RequiredType.Type; },
          HasSameDepender);
    }

    public abstract IVisitableDefinition Depender { get; }

    public abstract void Accept (IDefinitionVisitor visitor);
    public abstract string GetDependencyDescription ();

    private bool HasSameDepender (DependencyDefinitionBase dependencyToCheck)
    {
      ArgumentUtility.CheckNotNull ("dependencyToCheck", dependencyToCheck);
      return dependencyToCheck.Depender == Depender;
    }

    public TargetClassDefinition TargetClass
    {
      get { return RequiredType.TargetClass; }
    }

    public RequirementDefinitionBase RequiredType
    {
      get { return _requirement; }
    }

    public DependencyDefinitionBase Aggregator
    {
      get { return _aggregator; }
    }

    public string FullName
    {
      get { return RequiredType.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get
      {
        if (Aggregator != null)
        {
          return Aggregator;
        }
        else
        {
          return Depender;
        }
      }
    }

    // aggregates hold nested dependencies
    public bool IsAggregate
    {
      get { return _aggregatedDependencies.Count > 0; }
    }

    public UniqueDefinitionCollection<Type, DependencyDefinitionBase> AggregatedDependencies
    {
      get { return _aggregatedDependencies; }
    }

    public virtual ClassDefinitionBase GetImplementer()
    {
      if (RequiredType.Type.IsAssignableFrom (TargetClass.Type))
        return TargetClass;
      else if (TargetClass.ReceivedInterfaces.ContainsKey (RequiredType.Type))
        return TargetClass.ReceivedInterfaces[RequiredType.Type].Implementer;
      else if (!RequiredType.IsEmptyInterface) // duck interface
        return TargetClass; 
      else
        return null; // empty interface that is neither introduced nor implemented
    }
  }
}
