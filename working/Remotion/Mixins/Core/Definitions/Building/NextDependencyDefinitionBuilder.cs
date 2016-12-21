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
  /// Builds <see cref="NextCallDependencyDefinition"/> objects, for when a mixin has a dependency on a type to be used for 
  /// <see cref="Mixin{TTarget,TNext}.Next"/> calls.
  /// </summary>
  public class NextCallDependencyDefinitionBuilder : DependencyDefinitionBuilderBase
  {
    private readonly MixinDefinition _mixin;

    public NextCallDependencyDefinitionBuilder (MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      _mixin = mixin;
    }

    protected override RequirementDefinitionBase GetRequirement (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _mixin.TargetClass.RequiredNextCallTypes[type];
    }

    protected override RequirementDefinitionBase CreateRequirement (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      Assertion.IsTrue (type != typeof (object), "This method will not be called for typeof (object).");

      if (!type.IsInterface)
      {
        string message = string.Format ("Next call dependencies must be interfaces (or System.Object), but mixin {0} (on class {1} has a dependency "
            + "on a class: {2}.", _mixin.FullName, _mixin.TargetClass.FullName, type.FullName);
        throw new ConfigurationException (message);
      }

      return new RequiredNextCallTypeDefinition (_mixin.TargetClass, type);
    }

    protected override void AddRequirement (RequirementDefinitionBase requirement)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);

      _mixin.TargetClass.RequiredNextCallTypes.Add ((RequiredNextCallTypeDefinition) requirement);
    }

    protected override DependencyDefinitionBase CreateDependency (RequirementDefinitionBase requirement, DependencyDefinitionBase aggregator)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);

      return new NextCallDependencyDefinition ((RequiredNextCallTypeDefinition) requirement, _mixin, (NextCallDependencyDefinition)aggregator);
    }

    protected override void AddDependency (DependencyDefinitionBase dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);
      if (!_mixin.NextCallDependencies.ContainsKey (dependency.RequiredType.Type))
        _mixin.NextCallDependencies.Add ((NextCallDependencyDefinition) dependency);
    }
  }
}
