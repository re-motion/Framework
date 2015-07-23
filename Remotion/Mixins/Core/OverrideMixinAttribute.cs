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

namespace Remotion.Mixins
{
  /// <summary>
  /// Indicates that a target class member overrides a virtual or abstract member of one of the mixins combined with the class.
  /// </summary>
  /// <remarks>
  /// <para>
  /// An overriding member and its base member must both be public or protected, and they must have the same name and signature. If an overriding
  /// member would apply to multiple mixin members, this is regarded as a configuration error, unless the mixin type is explicitly specified in
  /// this attribute's constructor.
  /// </para>
  /// <para>
  /// This attribute is inherited (i.e. if the overriding member is replaced in a subclass, the subclass' member is now the overriding member) and
  /// can only be applied once per member.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
  public class OverrideMixinAttribute : Attribute, IOverrideAttribute
  {
    private readonly Type _mixinType;

    /// <summary>
    /// Indicates that this member overrides a virtual or abstract member of one of the mixins combined with the class. The overridden member
    /// has the same name and signature as this member. If more than one mixin defines such a member, an exception is thrown.
    /// </summary>
    public OverrideMixinAttribute ()
    {
    }

    /// <summary>
    /// Indicates that this member overrides a virtual or abstract member of the given mixin type. The overridden member
    /// has the same name and signature as this member. If the given mixin type is not part of the current configuration, an exception is thrown.
    /// </summary>
    /// <param name="mixinType">The type (or base type or interface) of the mixin whose member to override. For generic mixins, you can specify the
    /// open type (with unbound generic parameters) even if the configuration contains a closed type (with bound parameters).</param>
    public OverrideMixinAttribute (Type mixinType)
    {
      _mixinType = ArgumentUtility.CheckNotNull ("mixinType", mixinType);
    }

    public Type MixinType
    {
      get { return _mixinType; }
    }

    Type IOverrideAttribute.OverriddenType
    {
      get { return MixinType; }
    }
  }
}
