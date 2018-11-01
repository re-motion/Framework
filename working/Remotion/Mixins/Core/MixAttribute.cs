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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Configures that a class and a mixin should be mixed together.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This attribute is effective for the declarative mixin configuration, which is in effect by default when an application is started.
  /// </para>
  /// <para> 
  /// The <see cref="MixAttribute"/> is an alternative to <see cref="UsesAttribute"/> and <see cref="ExtendsAttribute"/> allowing assembly-level mixin
  /// configuration. Therefore, it is suitable for transparently putting mixins and classes together, with neither mixin nor target class explicitly
  /// referencing the other side of the relationship.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
  public class MixAttribute : MixinRelationshipAttribute, IMixinConfigurationAttribute<Assembly>
  {
    private readonly Type _targetType;
    private readonly Type _mixinType;

    /// <summary>
    /// Initializes a new instance of the <see cref="MixAttribute"/> class.
    /// </summary>
    /// <param name="targetType">The target type to be mixed.</param>
    /// <param name="mixinType">The mixin type to be mixed with the target type.</param>
    public MixAttribute (Type targetType, Type mixinType)
    {
      _targetType = ArgumentUtility.CheckNotNull ("targetType", targetType);
      _mixinType = ArgumentUtility.CheckNotNull ("mixinType", mixinType);
    }

    /// <summary>
    /// Gets or sets the kind of relationship between the mixin and its target class. For more information see <see cref="Mixins.MixinKind"/>. If not
    /// explicitly specified, <see cref="Mixins.MixinKind.Extending"/> is assumed.
    /// </summary>
    /// <value>The mixin kind.</value>
    public MixinKind MixinKind { get; set; }


    /// <summary>
    /// Gets the target type to be mixed.
    /// </summary>
    /// <value>The mixed type.</value>
    public Type TargetType
    {
      get { return _targetType; }
    }

    /// <summary>
    /// Gets the mixin type mixed with the target class.
    /// </summary>
    /// <value>The mixin type.</value>
    public Type MixinType
    {
      get { return _mixinType; }
    }

    public override bool Equals (object obj)
    {
      MixAttribute other = obj as MixAttribute;
      return !object.ReferenceEquals (other, null)
          && TargetType == other.TargetType
          && MixinType == other.MixinType
          && MixinKind == other.MixinKind
          && base.Equals (other);
    }

    public override int GetHashCode ()
    {
      return TargetType.GetHashCode()
          ^ MixinType.GetHashCode()
          ^ MixinKind.GetHashCode() 
          ^ base.GetHashCode ();
    }

    public bool IgnoresDuplicates
    {
      get { return true; }
    }

    public void Apply (MixinConfigurationBuilder configurationBuilder, Assembly attributeTarget)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      ArgumentUtility.CheckNotNull ("attributeTarget", attributeTarget);

      var origin = MixinContextOrigin.CreateForCustomAttribute (this, attributeTarget);
      Apply (configurationBuilder, MixinKind, TargetType, MixinType, origin);
    }
  }
}
