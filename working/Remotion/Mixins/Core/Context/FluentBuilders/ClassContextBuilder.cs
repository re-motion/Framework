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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Remotion.Collections;
using Remotion.Mixins.Context.Suppression;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.FluentBuilders
{
  /// <summary>
  /// Assists <see cref="MixinConfigurationBuilder"/> by providing a fluent interface for building <see cref="ClassContext"/> objects.
  /// </summary>
  /// <remarks>
  /// This class gathers configuration data to be associated with a specific <see cref="TargetType"/>. That data is combined with inherited
  /// contexts (from base classes or parent configuration, for example) and pushed into a <see cref="ClassContext"/> by the 
  /// <see cref="BuildClassContext(System.Collections.Generic.IEnumerable{Remotion.Mixins.Context.ClassContext})"/> method. That method is called
  /// by <see cref="MixinConfigurationBuilder.BuildConfiguration"/>.
  /// </remarks>
  public class ClassContextBuilder
  {
    private readonly MixinConfigurationBuilder _parent;
    private readonly Type _targetType;
    private readonly Dictionary<Type, MixinContextBuilder> _mixinContextBuilders = new Dictionary<Type, MixinContextBuilder> ();
    private readonly HashSet<Type> _composedInterfaces = new HashSet<Type> ();
    private readonly List<IMixinSuppressionRule> _suppressedMixins = new List<IMixinSuppressionRule> ();
    private readonly MultiDictionary<Type, Type> _mixinDependencies = new MultiDictionary<Type, Type>();
    private bool _suppressInheritance = false;

    public ClassContextBuilder (Type targetType) : this (new MixinConfigurationBuilder (null), targetType)
    {
    }

    public ClassContextBuilder (MixinConfigurationBuilder parent, Type targetType)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      _parent = parent;
      _targetType = targetType;
    }

    /// <summary>
    /// Gets the <see cref="MixinConfigurationBuilder"/> used for creating this <see cref="ClassContextBuilder"/>.
    /// </summary>
    /// <value>This object's <see cref="MixinConfigurationBuilder"/>.</value>
    public MixinConfigurationBuilder Parent
    {
      get { return _parent; }
    }

    /// <summary>
    /// Gets the type configured by this <see cref="ClassContextBuilder"/>.
    /// </summary>
    /// <value>The target type configured by this object.</value>
    public Type TargetType
    {
      get { return _targetType; }
    }

    /// <summary>
    /// Gets the mixin context builders collected so far.
    /// </summary>
    /// <value>The mixin context builders collected so far by this object.</value>
    public IEnumerable<MixinContextBuilder> MixinContextBuilders
    {
      get { return _mixinContextBuilders.Values; }
    }

    /// <summary>
    /// Gets the composed interfaces collected so far.
    /// </summary>
    /// <value>The composed interfaces collected so far by this object.</value>
    public IEnumerable<Type> ComposedInterfaces
    {
      get { return _composedInterfaces; }
    }

    /// <summary>
    /// Gets the suppressed mixins collected so far.
    /// </summary>
    /// <value>The suppressed mixins collected so far by this object.</value>
    public IEnumerable<IMixinSuppressionRule> SuppressedMixins
    {
      get { return _suppressedMixins; }
    }

    /// <summary>
    /// Gets the independent mixin dependencies collected so far.
    /// </summary>
    /// <value>The independent mixin dependencies collected so far.</value>
    public IEnumerable<MixinDependencySpecification> MixinDependencies
    {
      get { return _mixinDependencies.Select (kvp => new MixinDependencySpecification (kvp.Key, kvp.Value)); }
    }

    public bool SuppressInheritance
    {
      get { return _suppressInheritance; }
    }

    /// <summary>
    /// Clears all mixin configuration for the <see cref="TargetType"/>. This causes the target type to ignore all mixin configuration data from its
    /// parent context and also resets all information collected so far for the class by this object.
    /// </summary>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder Clear ()
    {
      _mixinContextBuilders.Clear();
      _composedInterfaces.Clear();
      _suppressInheritance = true;
      return this;
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="TargetType"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>
    /// A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.
    /// </returns>
    public virtual MixinContextBuilder AddMixin (Type mixinType, MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("origin", origin);

      if (AlreadyAppliedSame (mixinType))
      {
        Type mixinTypeForException = mixinType.IsGenericType ? mixinType.GetGenericTypeDefinition() : mixinType;
        Assertion.IsNotNull (mixinTypeForException);
        throw new ArgumentException (
            string.Format ("{0} is already configured as a mixin for type {1}.", mixinTypeForException.FullName, TargetType.FullName), "mixinType");
      }

      var mixinContextBuilder = new MixinContextBuilder (this, mixinType, origin);
      _mixinContextBuilders.Add (mixinType, mixinContextBuilder);
      return mixinContextBuilder;
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="TargetType"/>, using the calling method as <see cref="MixinContextBuilder.Origin"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <returns>
    /// A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.
    /// </returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public MixinContextBuilder AddMixin (Type mixinType)
    {
      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return AddMixin (mixinType, origin);
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.</returns>
    public virtual MixinContextBuilder AddMixin<TMixin> (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);
      return AddMixin (typeof (TMixin), origin);
    }

    /// <summary>
    /// Collects the given type as a mixin for the <see cref="TargetType"/>, using the calling method as <see cref="MixinContextBuilder.Origin"/>.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <returns>A <see cref="MixinContextBuilder"/> object for further configuration of the mixin.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public MixinContextBuilder AddMixin<TMixin> ()
    {
      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return AddMixin<TMixin> (origin);
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>.
    /// </summary>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins (MixinContextOrigin origin, params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);
      ArgumentUtility.CheckNotNull ("origin", origin);

      foreach (Type mixinType in mixinTypes)
        AddMixin (mixinType, origin);
      return this;
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>, using the calling method as <see cref="MixinContextBuilder.Origin"/>.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public ClassContextBuilder AddMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);

      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return AddMixins (origin, mixinTypes);
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2> (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);

      return AddMixins (origin, typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>, using the calling method as <see cref="MixinContextBuilder.Origin"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public ClassContextBuilder AddMixins<TMixin1, TMixin2> ()
    {
      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return AddMixins<TMixin1, TMixin2> (origin);
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2, TMixin3> (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);
      return AddMixins (origin, typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/>, using the calling method as <see cref="MixinContextBuilder.Origin"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public ClassContextBuilder AddMixins<TMixin1, TMixin2, TMixin3> ()
    {
      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return AddMixins<TMixin1, TMixin2, TMixin3> (origin);
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect with dependencies.</param>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins (MixinContextOrigin origin, params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);
      
      Type lastMixinType = null;
      foreach (Type mixinType in mixinTypes)
      {
        MixinContextBuilder mixinContextBuilder = AddMixin (mixinType, origin);
        if (lastMixinType != null)
          mixinContextBuilder.WithDependency (lastMixinType);
        lastMixinType = mixinType;
      }
      return this;
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order, using the calling method as <see cref="MixinContextBuilder.Origin"/>.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect with dependencies.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public ClassContextBuilder AddOrderedMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);

      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return AddOrderedMixins (origin, mixinTypes);
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect with dependencies.</typeparam>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2> (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);
      return AddOrderedMixins (origin, typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order, using the calling method as <see cref="MixinContextBuilder.Origin"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect with dependencies.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2> ()
    {
      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return AddOrderedMixins<TMixin1, TMixin2> (origin);
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect with dependencies.</typeparam>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2, TMixin3> (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);
      return AddOrderedMixins (origin, typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Collects the given types as mixins for the <see cref="TargetType"/> and adds dependencies between the mixins to ensure a proper base call
    /// order, using the calling method as <see cref="MixinContextBuilder.Origin"/>.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect with dependencies.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect with dependencies.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2, TMixin3> ()
    {
      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return AddOrderedMixins<TMixin1, TMixin2, TMixin3> (origin);
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="TargetType"/>, adding it if necessary. The mixin will not be
    /// added if it has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, it will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual MixinContextBuilder EnsureMixin (Type mixinType, MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("origin", origin);

      MixinContextBuilder builder;
      if (!_mixinContextBuilders.TryGetValue (mixinType, out builder))
        builder = AddMixin (mixinType, origin);
      return builder;
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="TargetType"/>, adding it if necessary, 
    /// using the calling method as <see cref="MixinContextBuilder.Origin"/>. The mixin will not be
    /// added if it has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, it will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <param name="mixinType">The mixin type to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public MixinContextBuilder EnsureMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return EnsureMixin (mixinType, origin);
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="TargetType"/>, adding it if necessary. The mixin will not be
    /// added if it has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, it will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual MixinContextBuilder EnsureMixin<TMixin>  (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);
      return EnsureMixin (typeof (TMixin), origin);
    }

    /// <summary>
    /// Ensures that the given type is configured as a mixin for the <see cref="TargetType"/>, adding it if necessary, 
    /// using the calling method as <see cref="MixinContextBuilder.Origin"/>. The mixin will not be
    /// added if it has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, it will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public MixinContextBuilder EnsureMixin<TMixin> ()
    {
      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return EnsureMixin<TMixin> (origin);
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins (MixinContextOrigin origin, params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mixinTypes", mixinTypes);
      ArgumentUtility.CheckNotNull ("origin", origin);

      foreach (Type mixinType in mixinTypes)
        EnsureMixin (mixinType, origin);
      return this;
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary, 
    /// using the calling method as <see cref="MixinContextBuilder.Origin"/>. The mixins will not be
    /// added if they has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <param name="mixinTypes">The mixin types to collect.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public ClassContextBuilder EnsureMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mixinTypes", mixinTypes);

      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return EnsureMixins (origin, mixinTypes);
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins<TMixin1, TMixin2> (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);
      return EnsureMixins (origin, typeof (TMixin1), typeof (TMixin2));
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary, 
    /// using the calling method as <see cref="MixinContextBuilder.Origin"/>. The mixins will not be
    /// added if they has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public ClassContextBuilder EnsureMixins<TMixin1, TMixin2> ()
    {
      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return EnsureMixins<TMixin1, TMixin2> (origin);
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary. The mixins will not be
    /// added if they has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <param name="origin">A <see cref="MixinContextOrigin"/> object describing where the mixin configuration originates from.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder EnsureMixins<TMixin1, TMixin2, TMixin3> (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);
      return EnsureMixins (origin, typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    /// <summary>
    /// Ensures that the given types are configured as mixins for the <see cref="TargetType"/>, adding them if necessary, 
    /// using the calling method as <see cref="MixinContextBuilder.Origin"/>. The mixins will not be
    /// added if they has been taken over from the parent context (unless <see cref="Clear"/> was called); if added, they will override corresponding
    /// mixins inherited from a base type.
    /// </summary>
    /// <typeparam name="TMixin1">The first mixin type to collect.</typeparam>
    /// <typeparam name="TMixin2">The second mixin type to collect.</typeparam>
    /// <typeparam name="TMixin3">The third mixin type to collect.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    [MethodImpl (MethodImplOptions.NoInlining)]
    public ClassContextBuilder EnsureMixins<TMixin1, TMixin2, TMixin3> ()
    {
      var origin = MixinContextOrigin.CreateForStackFrame (new StackFrame (1));
      return EnsureMixins<TMixin1, TMixin2, TMixin3> (origin);
    }

    /// <summary>
    /// Adds the given type as a composed interface to the <see cref="TargetType"/>. A composed interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <param name="interfaceType">The type to collect as a composed interface.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddComposedInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      if (_composedInterfaces.Contains (interfaceType))
      {
        string message = string.Format ("{0} is already configured as a composed interface for type {1}.",
            interfaceType.FullName, TargetType.FullName);
        throw new ArgumentException (message, "interfaceType");
      }
      _composedInterfaces.Add (interfaceType);
      return this;
    }

    /// <summary>
    /// Adds the given type as a composed interface to the <see cref="TargetType"/>. A composed interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface">The type to collect as a composed interface.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddComposedInterface<TInterface> ()
    {
      return AddComposedInterface (typeof (TInterface));
    }

    /// <summary>
    /// Adds the given types as composed interfaces to the <see cref="TargetType"/>. A composed interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <param name="interfaceTypes">The types to collect as composed interfaces.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddComposedInterfaces (params Type[] interfaceTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("interfaceTypes", interfaceTypes);
      foreach (Type interfaceType in interfaceTypes)
        AddComposedInterface (interfaceType);
      return this;
    }

    /// <summary>
    /// Adds the given types as composed interfaces to the <see cref="TargetType"/>. A composed interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface1">The types to collect as composed interfaces.</typeparam>
    /// <typeparam name="TInterface2">The types to collect as composed interfaces.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddComposedInterfaces<TInterface1, TInterface2> ()
    {
      return AddComposedInterfaces (typeof (TInterface1), typeof (TInterface2));
    }

    /// <summary>
    /// Adds the given types as composed interfaces to the <see cref="TargetType"/>. A composed interface can contain both members defined by the
    /// target class itself and by mixins applied to the class, making it easier to invoke methods and properties on a mixed object without casting.
    /// </summary>
    /// <typeparam name="TInterface1">The types to collect as composed interfaces.</typeparam>
    /// <typeparam name="TInterface2">The types to collect as composed interfaces.</typeparam>
    /// <typeparam name="TInterface3">The types to collect as composed interfaces.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddComposedInterfaces<TInterface1, TInterface2, TInterface3> ()
    {
      return AddComposedInterfaces (typeof (TInterface1), typeof (TInterface2), typeof (TInterface3));
    }

    /// <summary>
    /// Denotes that specific mixin types should be ignored in the context of this class. Suppression is helpful when a target class should take 
    /// over most of its mixins from the parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <param name="rule">A <see cref="IMixinSuppressionRule"/> denoting mixin types to be suppressed.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixin (IMixinSuppressionRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      _suppressedMixins.Add (rule);
      return this;
    }

    /// <summary>
    /// Denotes that a specific mixin type, and all mixin types that can be ascribed to it (see <see cref="Reflection.TypeExtensions.CanAscribeTo"/>), should be
    /// ignored in the context of this class. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <param name="mixinType">The mixin type, base type, or generic type definition denoting mixin types to be suppressed.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      _suppressedMixins.Add (new MixinTreeSuppressionRule (mixinType));
      return this;
    }

    /// <summary>
    /// Denotes that a specific mixin type, and all mixin types that can be ascribed to it (see <see cref="Reflection.TypeExtensions.CanAscribeTo"/>), should be
    /// ignored in the context of this class. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <typeparam name="TMixinType">The mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixin<TMixinType> ()
    {
      return SuppressMixin (typeof (TMixinType));
    }

    /// <summary>
    /// Denotes that a number of mixin types, and all mixin types that can be ascribed to it (see <see cref="Reflection.TypeExtensions.CanAscribeTo"/>), should be
    /// ignored in the context of this class. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <param name="mixinTypes">The mixin types, base types, or generic type definitions denoting mixin types to be suppressed.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNull ("mixinTypes", mixinTypes);
      foreach (Type mixinType in mixinTypes)
        SuppressMixin (mixinType);
      return this;
    }

    /// <summary>
    /// Denotes that a number of mixin types, and all mixin types that can be ascribed to it (see <see cref="Reflection.TypeExtensions.CanAscribeTo"/>), should be
    /// ignored in the context of this class. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <typeparam name="TMixinType1">The first mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <typeparam name="TMixinType2">The second mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixins<TMixinType1, TMixinType2> ()
    {
      return SuppressMixins (typeof (TMixinType1), typeof (TMixinType2));
    }

    /// <summary>
    /// Denotes that a number of mixin types, and all mixin types that can be ascribed to it (see <see cref="Reflection.TypeExtensions.CanAscribeTo"/>), should be
    /// ignored in the context of this class. Suppression is helpful when a target class should take over most of its mixins from the
    /// parent context or inherit mixins from another type, but a specific mixin should be ignored in that process.
    /// </summary>
    /// <typeparam name="TMixinType1">The first mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <typeparam name="TMixinType2">The second mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <typeparam name="TMixinType3">The third mixin type, base type, or generic type definition denoting mixin types to be suppressed.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder SuppressMixins<TMixinType1, TMixinType2, TMixinType3> ()
    {
      return SuppressMixins (typeof (TMixinType1), typeof (TMixinType2), typeof (TMixinType3));
    }

    /// <summary>
    /// Denotes that mixin <paramref name="dependentMixin"/> should have a dependency on <paramref name="requiredMixin"/> in the context 
    /// of the <see cref="TargetType"/>. This dependency can be added independently from the actual mixin, but when the <see cref="ClassContext"/> is 
    /// built, an exception is found if <paramref name="dependentMixin"/> is not configured for the <see cref="TargetType"/>.
    /// </summary>
    /// <param name="dependentMixin">The mixin for which the dependency is to be added.</param>
    /// <param name="requiredMixin">The type on which <paramref name="dependentMixin"/> should have a dependency.</param>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixinDependency (Type dependentMixin, Type requiredMixin)
    {
      ArgumentUtility.CheckNotNull ("dependentMixin", dependentMixin);
      ArgumentUtility.CheckNotNull ("requiredMixin", requiredMixin);

      _mixinDependencies.Add (dependentMixin, requiredMixin);
      return this;
    }

    /// <summary>
    /// Denotes that mixin <typeparamref name="TDependentMixin"/> should have a dependency on <typeparamref name="TRequiredMixin"/> in the context 
    /// of the <see cref="TargetType"/>. This dependency can be added independently from the actual mixin, but when the <see cref="ClassContext"/> is 
    /// built, an exception is found if <typeparamref name="TDependentMixin"/> is not configured for the <see cref="TargetType"/>.
    /// </summary>
    /// <typeparam name="TDependentMixin">The mixin for which the dependency is to be added.</typeparam>
    /// <typeparam name="TRequiredMixin">The type on which <typeparamref name="TDependentMixin"/> should have a dependency.</typeparam>
    /// <returns>This object for further configuration of the <see cref="TargetType"/>.</returns>
    public virtual ClassContextBuilder AddMixinDependency<TDependentMixin, TRequiredMixin> ()
    {
      return AddMixinDependency (typeof (TDependentMixin), typeof (TRequiredMixin));
    }

    /// <summary>
    /// Builds a class context with the data collected so far for the <see cref="TargetType"/> that inherits from other contexts.
    /// </summary>
    /// <param name="inheritedContexts">A collection of <see cref="ClassContext"/> instances the newly built context should inherit mixin data from.</param>
    /// <returns>A <see cref="ClassContext"/> for the <see cref="TargetType"/> holding all mixin configuration data collected so far.</returns>
    public virtual ClassContext BuildClassContext (IEnumerable<ClassContext> inheritedContexts)
    {
      var mixinContexts = MixinContextBuilders.Select (mixinContextBuilder => mixinContextBuilder.BuildMixinContext());
      var classContext = new ClassContext (_targetType, mixinContexts, ComposedInterfaces);
      classContext = ApplyInheritance (classContext, inheritedContexts);
      classContext = classContext.SuppressMixins (SuppressedMixins);
      try
      {
        classContext = classContext.ApplyMixinDependencies (_mixinDependencies.Select (kvp => new MixinDependencySpecification (kvp.Key, kvp.Value)));
      }
      catch (InvalidOperationException ex)
      {
        var message = string.Format ("The mixin dependencies configured for type '{0}' could not be processed: {1}", TargetType, ex.Message);
        throw new ConfigurationException (message, ex);
      }
      return classContext;
    }

    /// <summary>
    /// Builds a class context with the data collected so far for the <see cref="TargetType"/> without inheriting from other contexts.
    /// </summary>
    /// <returns>A <see cref="ClassContext"/> for the <see cref="TargetType"/> holding all mixin configuration data collected so far.</returns>
    public virtual ClassContext BuildClassContext ()
    {
      return BuildClassContext (new ClassContext[0]);
    }

    private ClassContext ApplyInheritance (ClassContext classContext, IEnumerable<ClassContext> inheritedContexts)
    {
      if (SuppressInheritance)
        return classContext;
      else
        return classContext.InheritFrom (inheritedContexts);
    }

    private bool AlreadyAppliedSame (Type mixinType)
    {
      if (_mixinContextBuilders.ContainsKey (mixinType))
        return true;

      if (!mixinType.IsGenericType)
        return false;

      Type typeDefinition = mixinType.GetGenericTypeDefinition ();

      return MixinContextBuilders
          .Any (mixinContextBuilder => mixinContextBuilder.MixinType.IsGenericType 
              && mixinContextBuilder.MixinType.GetGenericTypeDefinition() == typeDefinition);
    }

    #region Parent members

    /// <summary>
    /// Begins configuration of another target class.
    /// </summary>
    /// <param name="targetType">The class to be configured.</param>
    /// <returns>A fluent interface object for configuring the given <paramref name="targetType"/>.</returns>
    public virtual ClassContextBuilder ForClass (Type targetType)
    {
      return _parent.ForClass (targetType);
    }

    /// <summary>
    /// Begins configuration of another target class.
    /// </summary>
    /// <typeparam name="TTargetType">The class to be configured.</typeparam>
    /// <returns>A fluent interface object for configuring the given <typeparamref name="TTargetType"/>.</returns>
    public virtual ClassContextBuilder ForClass<TTargetType> ()
    {
      return _parent.ForClass<TTargetType>();
    }

    /// <summary>
    /// Builds a configuration object with the data gathered so far.
    /// </summary>
    /// <returns>A new <see cref="MixinConfiguration"/> instance incorporating all the data acquired so far.</returns>
    public virtual MixinConfiguration BuildConfiguration ()
    {
      return _parent.BuildConfiguration();
    }

    /// <summary>
    /// Builds a configuration object and calls the <see cref="EnterScope"/> method on it, thus activating the configuration for the current
    /// thread. The previous configuration is restored when the returned object's <see cref="IDisposable.Dispose"/> method is called (e.g. by a
    /// using statement).
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> object for restoring the original configuration.</returns>
    public virtual IDisposable EnterScope ()
    {
      return _parent.EnterScope();
    }
    #endregion

  }
}
