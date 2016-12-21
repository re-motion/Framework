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
using System.Reflection;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Defines a dependency from a mixin to another mixin or interface type in the context of a given target class, 
  /// as if that dependency was specified via an <see cref="ExtendsAttribute"/>'s, <see cref="UsesAttribute"/>'s, or <see cref="MixAttribute"/>'s 
  /// <see cref="MixinRelationshipAttribute.AdditionalDependencies"/> property. If the type depended upon is not present within the target type's 
  /// mixin configuration, an error is raised.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This can be used to augment an existing mixin declaration with a dependency in situations where the mixin declaration cannot be changed. It 
  /// implements the following use cases:
  /// <list type="bullet">
  /// <item>
  ///   <description>
  ///     Resolving an ordering conflict between mixins added to a single class by different components oblivious to each other. 
  ///     An application or additional component that combines the components with conflicting mixins can resolve that conflict by adding 
  ///     dependencies using the <see cref="AdditionalMixinDependencyAttribute"/>.
  ///   </description>
  /// </item>
  /// <item>
  ///   <description>
  ///     Adding a mixin in the middle of two existing mixins.
  ///   </description>
  /// </item>
  /// <item>
  ///   <description>
  ///     Overriding the alphabetic ordering (see <see cref="AcceptsAlphabeticOrderingAttribute"/>) between existing mixins.
  ///   </description>
  /// </item>
  /// </list>
  /// </para>
  /// <para>
  /// Assembly-level mixin dependencies influence the mixin configuration of the class specified via the <see cref="TargetType"/>parameter. 
  /// If a derived class inherits a mixin with a dependency, the dependency is inherited together with the mixin.
  /// </para>
  /// <para>
  /// When an assembly-level mixin dependency is specified for a class that does not have the given dependent mixin, an error of type 
  /// <see cref="ConfigurationException"/> is raised at attribute analysis time.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true)]
  public class AdditionalMixinDependencyAttribute : Attribute, IMixinConfigurationAttribute<Assembly>
  {
    private readonly Type _targetType;
    private readonly Type _dependentMixin;
    private readonly Type _dependency;

    public AdditionalMixinDependencyAttribute (Type targetType, Type dependentMixin, Type dependency)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("dependentMixin", dependentMixin);
      ArgumentUtility.CheckNotNull ("dependency", dependency);

      _targetType = targetType;
      _dependentMixin = dependentMixin;
      _dependency = dependency;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public Type DependentMixin
    {
      get { return _dependentMixin; }
    }

    public Type Dependency
    {
      get { return _dependency; }
    }

    public bool IgnoresDuplicates
    {
      get { return false; }
    }

    public void Apply (MixinConfigurationBuilder configurationBuilder, Assembly attributeTarget)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      ArgumentUtility.CheckNotNull ("attributeTarget", attributeTarget);

      configurationBuilder.ForClass (TargetType).AddMixinDependency (DependentMixin, Dependency);
    }
  }
}