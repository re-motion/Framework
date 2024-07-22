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
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides reflective access to the mixins integrated with a target class.
  /// </summary>
  public static class Mixin
  {
    /// <summary>
    /// Gets the instance of the specified mixin type <typeparamref name="TMixin"/> that was mixed into the given <paramref name="mixinTarget"/>.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type to get an instance of.</typeparam>
    /// <param name="mixinTarget">The mixin target to get the mixin instance from.</param>
    /// <returns>The instance of the specified mixin type that was mixed into the given mixin target, or <see langword="null"/> if the target does not
    /// include a mixin of that type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="mixinTarget"/> parameter is <see langword="null"/>.</exception>
    /// <remarks>
    /// This method cannot be used with mixins that have been configured as open generic type definitions. Use the <see cref="Get(Type, object)">
    /// non-generic</see> variant instead.
    /// </remarks>
    public static TMixin? Get<TMixin> (object mixinTarget) where TMixin : class
    {
      ArgumentUtility.CheckNotNull("mixinTarget", mixinTarget);
      return (TMixin?)Get(typeof(TMixin), mixinTarget);
    }

    /// <summary>
    /// Gets the instance of the specified <paramref name="mixinType"/> that was mixed into the given <paramref name="mixinTarget"/>.
    /// </summary>
    /// <param name="mixinType">The mixin type to get an instance of.</param>
    /// <param name="mixinTarget">The mixin target to get the mixin instance from.</param>
    /// <returns>The instance of the specified mixin type that was mixed into the given mixin target, or <see langword="null"/> if the target does not
    /// include a mixin of that type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="mixinType"/> or the <paramref name="mixinTarget"/> parameter is
    /// <see langword="null"/>.</exception>
    /// <remarks>
    /// This method can also be used with mixins that have been configured as open generic type definitions. Use the open generic type definition
    /// to retrieve them, but be prepared to get an instance of a specialized (closed) generic type back.
    /// </remarks>
    public static object? Get (Type mixinType, object mixinTarget)
    {
      ArgumentUtility.CheckNotNull("mixinType", mixinType);
      ArgumentUtility.CheckNotNull("mixinTarget", mixinTarget);

      var castMixinTarget = mixinTarget as IMixinTarget;
      if (castMixinTarget != null)
      {
        return FindMixin(castMixinTarget, mixinType);
      }
      return null;
    }

    private static object? FindMixin (IMixinTarget mixinTarget, Type mixinType)
    {
      object? mixin = null;
      foreach (var potentialMixin in mixinTarget.Mixins)
      {
        if (IsTypeMatch(potentialMixin.GetType(), mixinType))
        {
          if (mixin != null)
          {
            string message = string.Format(
                "Both mixins '{0}' and '{1}' match the given type '{2}'.",
                mixin.GetType().GetFullNameSafe(),
                potentialMixin.GetType().GetFullNameSafe(),
                mixinType.Name);
            throw new AmbiguousMatchException(message);
          }

          mixin = potentialMixin;
        }
      }

      return mixin;
    }

    private static bool IsTypeMatch (Type potentialMixinType, Type searchedMixinType)
    {
      return searchedMixinType.IsAssignableFrom(potentialMixinType)
          || (searchedMixinType.IsGenericTypeDefinition
              && potentialMixinType.IsGenericType
              && potentialMixinType.GetGenericTypeDefinition() == searchedMixinType);
    }
  }

  /// <summary>
  /// Acts as a base class for mixins that require a reference to their target object (<see cref="Mixin{TTarget}.Target"/>) and a reference for calling 
  /// the base implementations of overridden methods (<see cref="Next"/>).
  /// </summary>
  /// <typeparam name="TTarget">The minimum type required for calling methods on the target object (<see cref="Mixin{TTarget}.Target"/>). The
  /// mixin can only be applied to types that fulfill this type constraint either themselves or via other mixins.
  /// This type needn't actually be implemented by the target class. See Remarks section for details.
  /// </typeparam>
  /// <typeparam name="TNext">The minimum type required for making base calls (<see cref="Next"/>) when overriding a method of the target class. 
  /// This type needn't actually be implemented by the target class. See Remarks section for details.</typeparam>
  /// <remarks>
  /// <para>
  /// Typically, this base class will be used whenever a mixin overrides a method of a target class and it needs to call the overridden base implementation.
  /// Derive from the <see cref="Mixin{TTarget}"/> class if you only need the target object reference but are not making any base calls, or use any
  /// base class if not even the target object reference is required.
  /// </para>
  /// <para>
  /// <typeparamref name="TTarget"/> is called the target-call dependency of the mixin, and can be assigned a class or interface (or a type parameter with 
  /// class or interface constraints). It defines the type of the <see cref="Mixin{TTarget}.Target"/> property, which is used to access the target
  /// instance of this mixin as well as members of other mixins applied to the same target instance. The mixin engine will ensure that 
  /// the target-call dependency is fulfilled either by the target class of the mixin, or by any mixin applied to the target class. 
  /// Accessing a member via the <see cref="Mixin{TTarget}.Target"/> property will actually access the corresponding member of the implementing target 
  /// class or mixin.
  /// </para>
  /// <para>
  /// <typeparamref name="TNext"/> is called the next-call dependency of the mixin and can be assigned an interface or
  /// the type <see cref="System.Object"/> (or a type parameter with interface or <see cref="System.Object"/> constraints). It defines the type
  /// of the <see cref="Next"/> property, which is used when a mixin overrides a member of the target class and needs to call the next implementation 
  /// in the chain of overrides (or the base implementation on the target class, when there is no other mixin in the chain).
  /// The next-call dependencies of a mixin also define the order in which method overrides are executed when multiple mixins override the same target 
  /// method: when mixin A has a next-call dependency on an interface IB, its override will be executed before any mixin implementing the interface IB.
  /// </para>
  /// <para>If <typeparamref name="TTarget"/> or <typeparamref name="TNext"/> are interfaces, that dependency need not actually be implemented on the 
  /// target class as an interface is usually implemented. There are two additional possibilities:
  /// <list type="bullet">
  /// <item>
  /// The interface can be implemented on a mixin applied to the same target class.
  /// </item>
  /// <item>
  /// The members of the interface can be implemented on the target class with the same name and signature as defined by the interface (duck typing).
  /// </item>
  /// </list>
  /// The latter enables a mixin to define its dependencies even if the target class and any interfaces it implements are unknown.
  /// </para>
  /// <para>
  /// If a concrete mixin derived from this class is a generic type, it can be configured as a mixin in its open generic type definition form 
  /// (<c>typeof (C&lt;,&gt;)</c>). In such a case, the mixin engine will try to close it at the time of configuration analysis; for this, 
  /// binding information in the form of the <see cref="BindToTargetTypeAttribute"/>, <see cref="BindToConstraintsAttribute"/>, and 
  /// <see cref="BindToGenericTargetParameterAttribute"/> is used.
  /// </para>
  /// </remarks>
  [Serializable]
  public class Mixin<TTarget, TNext> : Mixin<TTarget>, IInitializableMixin
      where TTarget: class
      where TNext: class
  {
    [NonSerialized]
    private TNext? _next;

    /// <summary>
    /// Provides a way to call the next or base implementation from member overrides.
    /// </summary>
    /// <value>The next implementation to be called.</value>
    /// <exception cref="InvalidOperationException">The mixin has not been initialized yet, probably because the property is accessed from the mixin's
    /// constructor.</exception>
    /// <remarks>
    /// <para>
    /// This property must not be accessed from the mixin's constructor; if you need to initialize the mixin by accessing the <see cref="Next"/>
    /// property (which is unusual because the <see cref="Next"/> property should only be used from member overrides), override the 
    /// <see cref="Mixin{TTarget}.OnInitialized"/> method.
    /// </para>
    /// <para>
    /// This property is only for calling the next implementation from overridden methods. It should not be used to access arbitrary methods of the 
    /// mixin's target object, use the <see cref="Mixin{TTarget}.Target"/> property instead. If the <see cref="Next"/> property is used other than to 
    /// perform base calls from member overrides and more than one mixin is applied to the target object, unexpected behavior may arise because the 
    /// <see cref="Next"/> property will represent the target object together with a subset (but not all) of its mixins.
    /// </para>
    /// <para>
    /// When a member is accessed via the <see cref="Next"/> property, the member is actually called on either the next mixin in the override chain
    /// for the corresponding member, or on the target class if there are no more mixins in the chain. The order in which member overrides are
    /// chained is defined by mixin ordering, e.g. via the dependencies expressed via the <typeparamref name="TNext"/> parameter.
    /// </para>
    /// </remarks>
    protected TNext Next
    {
      [DebuggerStepThrough]
      get
      {
        if (_next == null)
          throw new InvalidOperationException("Mixin has not been initialized yet.");
        return _next;
      }
    }

    void IInitializableMixin.Initialize (object target, object? next)
    {
      _target = (TTarget)target;
      _next = (TNext?)next;
      OnInitialized();
    }
  }

  /// <summary>
  /// Acts as a base class for mixins that require a reference to their target object (<see cref="Target"/>).
  /// </summary>
  /// <typeparam name="TTarget">The minimum type required for calling methods on the target object (<see cref="Mixin{TTarget}.Target"/>). The
  /// mixin can only be applied to types that fulfill this type constraint either themselves or via other mixins.
  /// This type needn't actually be implemented by the target class. See Remarks section for details.
  /// </typeparam>
  /// <remarks>
  /// <para>
  /// Typically, this base class will be used for those mixins which do require a reference to their target object, but which do not overrride
  /// any methods. 
  /// Derive from the <see cref="Mixin{TTarget, TNext}"/> class if you need to override target methods, or use any
  /// base class if not even the target object reference is required.
  /// </para>
  /// <para>
  /// <typeparamref name="TTarget"/> is called the target-call dependency of the mixin, and can be assigned a class or interface (or a type parameter with 
  /// class or interface constraints). It defines the type of the <see cref="Mixin{TTarget}.Target"/> property, which is used to access the target
  /// instance of this mixin as well as members of other mixins applied to the same target instance. The mixin engine will ensure that 
  /// the target-call dependency is fulfilled either by the target class of the mixin, or by any mixin applied to the target class. 
  /// Accessing a member via the <see cref="Mixin{TTarget}.Target"/> property will actually access the corresponding member of the implementing target 
  /// class or mixin.
  /// </para>
  /// <para>
  /// If <typeparamref name="TTarget"/> is an interface, that dependency need not actually be implemented on the 
  /// target class as an interface is usually implemented. There are two additional possibilities:
  /// <list type="bullet">
  /// <item>
  /// The interface can be implemented on a mixin applied to the same target class.
  /// </item>
  /// <item>
  /// The members of the interface can be implemented on the target class with the same name and signature as defined by the interface (duck typing).
  /// </item>
  /// </list>
  /// The latter enables a mixin to define its dependencies even if the target class and any interfaces it implements are unknown.
  /// </para>
  /// <para>
  /// If a concrete mixin derived from this class is a generic type, it can be configured as a mixin in its open generic type definition form 
  /// (<c>typeof (C&lt;&gt;)</c>). In such a case, the mixin engine will try to close it at the time of configuration analysis; for this, 
  /// binding information in the form of the <see cref="BindToTargetTypeAttribute"/>, <see cref="BindToConstraintsAttribute"/>, and 
  /// <see cref="BindToGenericTargetParameterAttribute"/> is used.
  /// </para>
  /// </remarks>
  [Serializable]
  public class Mixin<TTarget> : IInitializableMixin
      where TTarget: class
  {
    // TODO RM-7688 Should be private
    [NonSerialized]
    internal TTarget? _target;

    /// <summary>
    /// Gets a reference to the concrete mixed object.
    /// </summary>
    /// <value>The target object reference.</value>
    /// <exception cref="InvalidOperationException">The mixin has not been initialized yet, probably because the property is accessed from the mixin's
    /// constructor.</exception>
    /// <remarks>
    /// This property must not be accessed from the mixin's constructor; if you need to initialize the mixin by accessing the <see cref="Target"/>
    /// property, override the <see cref="Mixin{TTarget}.OnInitialized"/> method.
    /// <note type="warning">
    /// Be careful when calling members that this mixin overrides via the <see cref="Target"/> property, this can easily throw a
    /// <see cref="StackOverflowException"/> because the <see cref="Target"/> property includes all mixins defined on the target object. Use 
    /// <see cref="Mixin{TTarget,TNext}.Next"/> instead to call the base implementations of overridden members.
    /// </note>
    /// </remarks>
    protected TTarget Target
    {
      [DebuggerStepThrough]
      get
      {
        if (_target == null)
          throw new InvalidOperationException("Mixin has not been initialized yet.");
        return _target;
      }
    }

    /// <summary>
    /// Called when the mixin has been initialized and its properties can be safely accessed.
    /// </summary>
    protected virtual void OnInitialized ()
    {
      // nothing
    }

    void IInitializableMixin.Initialize (object target, object? next)
    {
      _target = (TTarget)target;
      OnInitialized();
    }
  }
}
