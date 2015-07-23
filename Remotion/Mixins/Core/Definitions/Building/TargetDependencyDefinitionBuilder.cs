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
  /// Builds <see cref="TargetCallDependencyDefinition"/> objects, for when a mixin has a dependency on a type to be used for 
  /// <see cref="Mixin{TTarget}.Target"/> calls.
  /// </summary>
  public class TargetCallDependencyDefinitionBuilder : DependencyDefinitionBuilderBase
  {
    private readonly MixinDefinition _mixin;

    public TargetCallDependencyDefinitionBuilder (MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      _mixin = mixin;
    }

    protected override RequirementDefinitionBase GetRequirement (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _mixin.TargetClass.RequiredTargetCallTypes[type];
    }

    protected override RequirementDefinitionBase CreateRequirement (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return new RequiredTargetCallTypeDefinition (_mixin.TargetClass, type);
    }

    protected override void AddRequirement (RequirementDefinitionBase requirement)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);

      _mixin.TargetClass.RequiredTargetCallTypes.Add ((RequiredTargetCallTypeDefinition) requirement);
    }

    protected override DependencyDefinitionBase CreateDependency (RequirementDefinitionBase requirement, DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);

      return new TargetCallDependencyDefinition ((RequiredTargetCallTypeDefinition) requirement, _mixin, (TargetCallDependencyDefinition) aggregator);
    }

    protected override void AddDependency (DependencyDefinitionBase dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);

      if (!_mixin.TargetCallDependencies.ContainsKey (dependency.RequiredType.Type))
        _mixin.TargetCallDependencies.Add ((TargetCallDependencyDefinition) dependency);
    }
  }
}
