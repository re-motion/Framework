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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides a central point for reflectively working with mixin targets and generated concrete types.
  /// </summary>
  public static class MixinTypeUtility
  {
    private static readonly ConcurrentDictionary<Type, ClassContext?> s_classContextForConcreteTypesCache =
        new ConcurrentDictionary<Type, ClassContext?>();

    private static readonly ConcurrentDictionary<Type, ReadOnlyCollection<Type>> s_exactMixinTypesCache =
        new ConcurrentDictionary<Type, ReadOnlyCollection<Type>>();

    private static readonly ConcurrentDictionary<(Type ConfiguredMixinType, Type MixinType), bool> s_mixinTypeCache = new();

    /// <summary>
    /// Determines whether the given <paramref name="type"/> is a concrete, mixed type generated by the mixin infrastructure.
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>
    /// True if <paramref name="type"/> or one of its base types was generated by the mixin infrastructure as a concrete, mixed type; otherwise, false.
    /// </returns>
    public static bool IsGeneratedConcreteMixedType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);
      return typeof(IMixinTarget).IsAssignableFrom(type) && !type.IsInterface;
    }
    
    /// <summary>
    /// Determines whether the given <paramref name="type"/> was generated by the mixin infrastructure.
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>
    /// True if <paramref name="type"/> or one of its base types was generated by the mixin infrastructure. It might be a concrete, mixed type,
    /// a derived mixin type, or any other type needed by the mixin infrastructure.
    /// </returns>
    public static bool IsGeneratedByMixinEngine (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);
      return IsGeneratedConcreteMixedType(type)
          || typeof(IGeneratedMixinType).IsAssignableFrom(type)
          || typeof(IGeneratedNextCallProxyType).IsAssignableFrom(type)
          || (type.IsNested && type.IsInterface && IsGeneratedByMixinEngine(type.DeclaringType!));
    }

    /// <summary>
    /// Gets the concrete type for a given <paramref name="targetOrConcreteType"/> which contains all mixins currently configured for the type.
    /// </summary>
    /// <param name="targetOrConcreteType">The target type for which to retrieve a concrete type, or a concrete type.</param>
    /// <returns>
    /// The <paramref name="targetOrConcreteType"/> itself if there are no mixins configured for the type or if the type itself is a generated type; 
    /// otherwise, a generated type containing all the mixins currently configured for <paramref name="targetOrConcreteType"/>.
    /// </returns>
    public static Type GetConcreteMixedType (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull("targetOrConcreteType", targetOrConcreteType);
      
      // Check if type is concrete type for performance reasons (this is faster than just going to the TypeFactory)
      if (IsGeneratedConcreteMixedType(targetOrConcreteType))
        return targetOrConcreteType;

      return TypeFactory.GetConcreteType(targetOrConcreteType);
    }

    /// <summary>
    /// Gets the underlying target type for a given <paramref name="targetOrConcreteType"/>.
    /// </summary>
    /// <param name="targetOrConcreteType">The type to get the underlying target type for.</param>
    /// <returns>The <paramref name="targetOrConcreteType"/> itself if it is not a generated type; otherwise, the type that was used as a target type when the
    /// given <paramref name="targetOrConcreteType"/> was generated.</returns>
    public static Type GetUnderlyingTargetType (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull("targetOrConcreteType", targetOrConcreteType);
      var classContextFromConcreteType = GetClassContextForConcreteType(targetOrConcreteType);
      if (classContextFromConcreteType == null)
        return targetOrConcreteType;
      return classContextFromConcreteType.Type;
    }

    /// <summary>
    /// Determines whether the given <paramref name="targetOrConcreteType"/> would be assignable to <paramref name="baseOrInterface"/> after all mixins
    /// currently configured for the type have been taken into account.
    /// </summary>
    /// <param name="baseOrInterface">The base or interface to assign to.</param>
    /// <param name="targetOrConcreteType">The type to check for assignment compatibility to <paramref name="baseOrInterface"/>. This must not be a generic
    /// type definition.</param>
    /// <returns>
    /// True if the type returned by <see cref="GetConcreteMixedType"/> for <paramref name="targetOrConcreteType"/> is the same as, derived from, or an
    /// implementation of <paramref name="baseOrInterface"/>; otherwise, false.
    /// </returns>
    /// <remarks>
    /// <note type="caution">
    /// This method requires the concrete mixed type, so it may invoke the code generation for <paramref name="targetOrConcreteType"/> 
    /// (if it is not already a concrete type) unless code has already been generated for that type.
    /// This could cause a performance problem when called in a tight loop with types for which no code has been generated so far.
    /// </note>
    /// </remarks>
    public static bool IsAssignableFrom (Type baseOrInterface, Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull("baseOrInterface", baseOrInterface);
      ArgumentUtility.CheckNotNull("targetOrConcreteType", targetOrConcreteType);

      return baseOrInterface.IsAssignableFrom(GetConcreteMixedType(targetOrConcreteType));
    }

    /// <summary>
    /// Determines whether the specified <paramref name="targetOrConcreteType"/> is associated with any mixins.
    /// </summary>
    /// <param name="targetOrConcreteType">The type to check for mixins.</param>
    /// <returns>
    /// True if the specified type is a generated type containing any mixins or a target type for which there are mixins currently configured;
    /// otherwise, false.
    /// </returns>
    public static bool HasMixins (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull("targetOrConcreteType", targetOrConcreteType);

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext(targetOrConcreteType);
      return classContext != null && classContext.Mixins.Count > 0;
    }

    /// <summary>
    /// Determines whether the specified <paramref name="targetOrConcreteType"/> is associated with a mixin of the given <paramref name="mixinType"/>.
    /// </summary>
    /// <param name="targetOrConcreteType">The type to check.</param>
    /// <param name="mixinType">The mixin type to check for.</param>
    /// <returns>
    /// True if the specified type is a generated type containing a mixin of the given <paramref name="mixinType"/> or a base type currently
    /// configured with such a mixin; otherwise, false.
    /// </returns>
    /// <remarks>
    /// This method checks for the exact mixin type, it does not take assignability or generic type instantiations into account. If the
    /// check should be broadened to include these properties, <see cref="GetAscribableMixinType"/> should be used.
    /// </remarks>
    public static bool HasMixin (Type targetOrConcreteType, Type mixinType)
    {
      ArgumentUtility.CheckNotNull("targetOrConcreteType", targetOrConcreteType);
      ArgumentUtility.CheckNotNull("mixinType", mixinType);

      ClassContext? classContext = MixinConfiguration.ActiveConfiguration.GetContext(targetOrConcreteType);
      return classContext != null && classContext.Mixins.ContainsKey(mixinType);
    }

    /// <summary>
    /// Determines whether the specified <paramref name="targetOrConcreteType"/> is associated with a mixin that can be ascribed to the given
    /// <paramref name="mixinType"/>, and returns the respective mixin type.
    /// </summary>
    /// <param name="targetOrConcreteType">The type to check.</param>
    /// <param name="mixinType">The mixin type to check for.</param>
    /// <returns>
    /// The mixin type if the specified type is a generated type containing a mixin that can be ascribed to <paramref name="mixinType"/> or a
    /// target type currently configured with such a mixin; otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// For performance reasons, if <paramref name="targetOrConcreteType"/> is a target type, this method returns only the configured mixin type 
    /// (as would be returned by <see cref="GetMixinTypes"/>), not the exact mixin type (as would be returned by <see cref="GetMixinTypesExact"/>).
    /// </para>
    /// </remarks>
    public static Type? GetAscribableMixinType (Type targetOrConcreteType, Type mixinType)
    {
      ArgumentUtility.CheckNotNull("targetOrConcreteType", targetOrConcreteType);
      ArgumentUtility.CheckNotNull("mixinType", mixinType);

      return GetMixinTypes(targetOrConcreteType)
          .FirstOrDefault(
              configuredMixinType => s_mixinTypeCache.GetOrAdd(
                  ValueTuple.Create(configuredMixinType, mixinType),
                  static tuple => Reflection.TypeExtensions.CanAscribeTo(tuple.ConfiguredMixinType, tuple.MixinType)));
    }

    /// <summary>
    /// Determines whether the specified <paramref name="targetOrConcreteType"/> is associated with a mixin that can be ascribed to the given
    /// <paramref name="mixinType"/>.
    /// </summary>
    /// <param name="targetOrConcreteType">The type to check.</param>
    /// <param name="mixinType">The mixin type to check for.</param>
    /// <returns>
    /// <see langword="true" />, if the specified type is a generated type containing a mixin that can be ascribed to <paramref name="mixinType"/> or 
    /// a base type currently configured with such a mixin; otherwise <see langword="false" />.
    /// </returns>
    public static bool HasAscribableMixin (Type targetOrConcreteType, Type mixinType)
    {
      return GetAscribableMixinType(targetOrConcreteType, mixinType) != null;
    }

    /// <summary>
    /// Gets the mixin types associated with the given <paramref name="targetOrConcreteType"/>.
    /// </summary>
    /// <param name="targetOrConcreteType">The type to check for mixin types.</param>
    /// <returns>The mixins included in <paramref name="targetOrConcreteType"/> if it is a generated type; otherwise the mixins currently configured for
    /// <paramref name="targetOrConcreteType"/>.</returns>
    /// <remarks>
    /// This method works on the mixin configuration rather than the concrete mixed type, so it can be used without triggering code generation.
    /// However, it returns the mixins without any defined ordering, and generic mixins are not closed before being returned. Use 
    /// <see cref="GetMixinTypesExact"/> when you require the exact mixin types as applied during code generation.
    /// </remarks>
    public static IEnumerable<Type> GetMixinTypes (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull("targetOrConcreteType", targetOrConcreteType);

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext(targetOrConcreteType);
      if (classContext == null)
        return Array.AsReadOnly(Type.EmptyTypes);

      return classContext.Mixins.Select(m => m.MixinType);
    }

    /// <summary>
    /// Gets the mixin types associated with the given <paramref name="targetOrConcreteType"/>, ordered and closed (if generic) exactly as they are 
    /// held by instances of the concrete type.
    /// </summary>
    /// <param name="targetOrConcreteType">The type to check for mixin types.</param>
    /// <returns>The mixins included in <paramref name="targetOrConcreteType"/> if it is a generated type; otherwise the mixins currently configured for
    /// <paramref name="targetOrConcreteType"/>.</returns>
    /// <remarks>
    /// <para>
    /// This method returns the mixin types exactly as they are held in the <see cref="IMixinTarget.Mixins"/> property by the concrete type 
    /// corresponding to <paramref name="targetOrConcreteType"/> (or <paramref name="targetOrConcreteType"/> itself if it is a generated concrete 
    /// type).
    /// </para>
    /// <para>
    /// <note type="caution">
    /// This method requires the concrete mixed type, so it may invoke the code generation for <paramref name="targetOrConcreteType"/> 
    /// (if it is not already a concrete type) unless code has already been generated for that type.
    /// This could cause a performance problem when called in a tight loop with types for which no code has been generated so far.
    /// Consider using <see cref="GetMixinTypes"/> for a faster variant.
    /// </note>
    /// </para>
    /// <para>
    /// The results of this method are cached.
    /// </para>
    /// </remarks>
    public static ReadOnlyCollection<Type> GetMixinTypesExact (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull("targetOrConcreteType", targetOrConcreteType);

      var concreteType = GetConcreteMixedType(targetOrConcreteType);
      return s_exactMixinTypesCache.GetOrAdd(
          concreteType,
          t =>
          {
            var types = MixinReflector.GetOrderedMixinTypesFromConcreteType(concreteType);
            return Array.AsReadOnly(types ?? Type.EmptyTypes);
          });
    }

    /// <summary>
    /// Creates an instance of the type returned by <see cref="GetConcreteMixedType"/> for the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type for whose concrete type to create an instance.</param>
    /// <param name="args">The arguments to be passed to the constructor.</param>
    /// <returns>An instance of the type returned by <see cref="GetConcreteMixedType"/> for <paramref name="type"/> created via a constructor taking the
    /// specified <paramref name="args"/>.</returns>
    /// <remarks>
    /// This is just a wrapper around 
    /// <see cref="ObjectFactory.Create(bool,System.Type,Remotion.TypePipe.ParamList,object[])"/>
    /// with a Reflection-like interface.
    /// </remarks>
    public static object CreateInstance (Type type, params object[] args)
    {
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNull("args", args);

      return ObjectFactory.Create(false, type, ParamList.CreateDynamic(args));
    }

    /// <summary>
    /// Returns the <see cref="ClassContext"/> that was used as the mixin configuration when the given <paramref name="concreteMixedType"/>
    /// was created by the <see cref="TypeFactory"/>.
    /// </summary>
    /// <param name="concreteMixedType">The type whose mixin configuration is to be retrieved.</param>
    /// <returns>The <see cref="ClassContext"/> used when the given <paramref name="concreteMixedType"/> was created, or <see langword="null"/>
    /// if <paramref name="concreteMixedType"/> is no mixed type.</returns>
    /// <remarks>
    /// <para>
    /// The results of this method are cached.
    /// </para>
    /// </remarks>
    public static ClassContext? GetClassContextForConcreteType (Type concreteMixedType)
    {
      ArgumentUtility.CheckNotNull("concreteMixedType", concreteMixedType);

      // C# compiler 7.2 already provides caching for anonymous method.
      return s_classContextForConcreteTypesCache.GetOrAdd(
          concreteMixedType,
          t =>
          {
            var attribute = AttributeUtility.GetCustomAttribute<ConcreteMixedTypeAttribute>(t, true);
            if (attribute == null)
              return null;
            else
              return attribute.GetClassContext();
          });
    }
  }
}
