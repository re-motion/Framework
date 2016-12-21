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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Indicates that a class integrates a mixin to implement some part of its functionality or public interface.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This attribute is effective for the declarative mixin configuration, which is in effect by default when an application is started.
  /// </para>
  /// <para> 
  /// Although the attribute itself is not inherited, its semantics in mixin configuration are: If a base class is configured to be mixed with a
  /// mixin type M by means of the <see cref="UsesAttribute"/>, this configuration setting is inherited by each of its (direct and indirect) subclasses.
  /// The subclasses will therefore also be mixed with the same mixin type M unless a second mixin M2 derived from M is applied to the subclass, thus
  /// overriding the inherited configuration. If M is configured for both base class and subclass, the base class configuration is ignored.
  /// </para>
  /// <para>
  /// This attribute can be applied to the same target class multiple times if a class depends on multiple mixins, but it should not be used to
  /// apply the same mixin multiple times to the same target class.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
  public class UsesAttribute : MixinRelationshipAttribute, IMixinConfigurationAttribute<Type>
  {
    private readonly Type _mixinType;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsesAttribute"/> class.
    /// </summary>
    /// <param name="mixinType">The mixin type the class depends on.</param>
    public UsesAttribute (Type mixinType)
    {
      _mixinType = ArgumentUtility.CheckNotNull ("mixinType", mixinType);
    }

    /// <summary>
    /// Gets the mixin type the class depends on.
    /// </summary>
    /// <value>The mixin type the class depends on.</value>
    public Type MixinType
    {
      get { return _mixinType; }
    }

    public bool IgnoresDuplicates
    {
      get { return false; }
    }

    public void Apply (MixinConfigurationBuilder configurationBuilder, Type attributeTarget)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      ArgumentUtility.CheckNotNull ("attributeTarget", attributeTarget);

      var origin = MixinContextOrigin.CreateForCustomAttribute (this, attributeTarget);
      Apply (configurationBuilder, MixinKind.Used, attributeTarget, MixinType, origin);
    }
  }
}
