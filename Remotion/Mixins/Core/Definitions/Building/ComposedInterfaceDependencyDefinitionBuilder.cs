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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  /// <summary>
  /// Builds <see cref="ComposedInterfaceDependencyDefinition"/> objects, for when a target class has a composed interface.
  /// </summary>
  public class ComposedInterfaceDependencyDefinitionBuilder : DependencyDefinitionBuilderBase
  {
    private readonly TargetClassDefinition _targetClass;
    private readonly Type _composedInterface;

    public ComposedInterfaceDependencyDefinitionBuilder (TargetClassDefinition targetClass, Type composedInterface)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);
      ArgumentUtility.CheckNotNull ("composedInterface", composedInterface);

      _targetClass = targetClass;
      _composedInterface = composedInterface;
    }

    protected override RequirementDefinitionBase GetRequirement (Type type)
    {
      return _targetClass.RequiredTargetCallTypes[type];
    }

    protected override RequirementDefinitionBase CreateRequirement (Type type)
    {
      return new RequiredTargetCallTypeDefinition (_targetClass, type);
    }

    protected override void AddRequirement (RequirementDefinitionBase requirement)
    {
      _targetClass.RequiredTargetCallTypes.Add ((RequiredTargetCallTypeDefinition) requirement);
    }

    protected override DependencyDefinitionBase CreateDependency (RequirementDefinitionBase requirement, DependencyDefinitionBase aggregator)
    {
      return new ComposedInterfaceDependencyDefinition ((RequiredTargetCallTypeDefinition) requirement, _composedInterface, aggregator);
    }

    protected override void AddDependency (DependencyDefinitionBase dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);
      _targetClass.ComposedInterfaceDependencies.Add ((ComposedInterfaceDependencyDefinition) dependency);
    }
  }
}