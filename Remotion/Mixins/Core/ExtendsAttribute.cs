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
  /// Indicates that a mixin extends a specific class, providing some part of its functionality or public interface.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This attribute is effective for the declarative mixin configuration, which is in effect by default when an application is started.
  /// </para>
  /// <para> 
  /// Although the attribute itself is not inherited, its semantics in mixin configuration are: If a base class is configured to be mixed with a
  /// mixin type M by means of the <see cref="ExtendsAttribute"/>, this configuration setting is inherited by each of its (direct and indirect) subclasses.
  /// The subclasses will therefore also be mixed with the same mixin type M unless a second mixin M2 derived from M is applied to the subclass, thus
  /// overriding the inherited configuration. If M is configured for both base class and subclass, the base class configuration is ignored.
  /// </para>
  /// <para>
  /// This attribute can be applied to the same mixin class multiple times if it extends multiple target classes. It should not however be used to
  /// apply the same mixin multiple times to the same target class.
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class ExtendsAttribute : MixinRelationshipAttribute, IMixinConfigurationAttribute<Type>
  {
    private readonly Type _targetType;

    private Type[] _mixinTypeArguments = Type.EmptyTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendsAttribute"/> class.
    /// </summary>
    /// <param name="targetType">The target type extended by this mixin.</param>
    public ExtendsAttribute (Type targetType)
    {
      _targetType = ArgumentUtility.CheckNotNull ("targetType", targetType);
    }

    /// <summary>
    /// Gets the target type the mixin class applies to.
    /// </summary>
    /// <value>The target type the mixin class applies to.</value>
    public Type TargetType
    {
      get { return _targetType; }
    }

    /// <summary>
    /// Gets or sets the generic type arguments to be used when applying a generic mixin to the given target type. This is useful when the
    /// <see cref="ExtendsAttribute"/> is to be applied to a generic mixin class, but the default generic type specialization algorithm of the
    /// mixin engine does not give the desired results.
    /// </summary>
    /// <value>The generic type arguments to close the generic mixin type with.</value>
    /// <remarks>If this attribute is applied to a non-generic mixin class or if the types supplied don't match the mixin's generic parameters,
    /// an exception is thrown when the mixin configuration is analyzed.</remarks>
    public Type[] MixinTypeArguments
    {
      get { return _mixinTypeArguments; }
      set
      {
        _mixinTypeArguments = ArgumentUtility.CheckNotNull ("value", value);
      }
    }

    public bool IgnoresDuplicates
    {
      get { return false; }
    }

    public void Apply (MixinConfigurationBuilder configurationBuilder, Type attributeTarget)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      ArgumentUtility.CheckNotNull ("attributeTarget", attributeTarget);

      Type mixinType = CloseOverMixinTypeArguments (attributeTarget);
      var origin = MixinContextOrigin.CreateForCustomAttribute (this, attributeTarget);
      Apply (configurationBuilder, MixinKind.Extending, TargetType, mixinType, origin);
    }

    private Type CloseOverMixinTypeArguments (Type mixinType)
    {
      if (MixinTypeArguments.Length == 0)
        return mixinType;

      CheckNumberOfTypeArguments (mixinType);
      CheckMixinIsOpenGeneric (mixinType);

      try
      {
        return mixinType.MakeGenericType (MixinTypeArguments);
      }
      catch (ArgumentException ex)
      {
        string message = string.Format (
            "The ExtendsAttribute for target class '{0}' applied to mixin type '{1}' specified invalid generic type arguments: {2}",
            TargetType,
            mixinType,
            ex.Message);
        throw new ConfigurationException (message, ex);
      }
    }

    private void CheckMixinIsOpenGeneric (Type mixinType)
    {
      if (!mixinType.IsGenericTypeDefinition)
      {
        string message = string.Format (
            "The ExtendsAttribute for target class '{0}' applied to mixin type '{1}' specified generic type arguments, but the mixin type already has "
            + "type arguments specified.",
            TargetType,
            mixinType);
        throw new ConfigurationException (message);
      }
    }

    private void CheckNumberOfTypeArguments (Type mixinType)
    {
      int expectedTypeArgumentLength = mixinType.IsGenericType ? mixinType.GetGenericArguments ().Length : 0;
      if (MixinTypeArguments.Length != expectedTypeArgumentLength)
      {
        string message = string.Format (
            "The ExtendsAttribute for target class {0} applied to mixin type {1} specified {2} generic type argument(s) when {3} argument(s) were expected.",
            TargetType.FullName,
            mixinType.FullName,
            MixinTypeArguments.Length,
            expectedTypeArgumentLength);
        throw new ConfigurationException (message);
      }
    }
  }
}
