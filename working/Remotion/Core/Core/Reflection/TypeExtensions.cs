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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Defines extension methods for working with <see cref="PropertyInfo"/>.
  /// </summary>
  public static class TypeExtensions
  {
    private static readonly ConcurrentDictionary<Tuple<Type, Type>, bool> s_canAscribeCache = 
        new ConcurrentDictionary<Tuple<Type, Type>, bool>();
    private static readonly ConcurrentDictionary<Tuple<Type, Type>, bool> s_canAscribeInternalCache =
        new ConcurrentDictionary<Tuple<Type, Type>, bool>();
    private static readonly ConcurrentDictionary<Tuple<Type, Type>, IReadOnlyList<Type>> s_ascribedGenericArgumentsCache =
        new ConcurrentDictionary<Tuple<Type, Type>, IReadOnlyList<Type>>();
    private static readonly ConcurrentDictionary<Tuple<Type, Type, Type>, bool> s_canDirectlyAscribeToGenericTypeInternalCache =
        new ConcurrentDictionary<Tuple<Type, Type, Type>, bool>();

    private static Func<Tuple<Type, Type>, bool> s_canAscribeCacheValueFactory;

    /// <summary>
    /// Evaluates whether the <paramref name="type"/> can be ascribed to the <paramref name="ascribeeType"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <param name="ascribeeType">The <see cref="Type"/> to check the <paramref name="type"/> against. Must not be <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="type"/> is the <paramref name="ascribeeType"/> or its instantiation, 
    /// its subclass or the implementation of an interface in case the <paramref name="ascribeeType"/> is an interface.
    /// </returns>
    public static bool CanAscribeTo (this Type type, Type ascribeeType)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("ascribeeType", ascribeeType);

      // C# compiler 7.2 does not provide caching for this anonymous method or local function.
      bool ValueFactory (Tuple<Type, Type> key)
      {
        var type1 = key.Item1;
        var ascribeeeType1 = key.Item2;
        if (ascribeeeType1.IsInterface)
        {
          if (type1.IsInterface && CanAscribeInternalFromCache (type1, ascribeeeType1))
            return true;

          return Array.Exists (type1.GetInterfaces(), current => CanAscribeInternalFromCache (current, ascribeeeType1));
        }
        else
        {
          return CanAscribeInternalFromCache (type1, ascribeeeType1);
        }
      }

      if (s_canAscribeCacheValueFactory == null)
        s_canAscribeCacheValueFactory = ValueFactory;

      return s_canAscribeCache.GetOrAdd (Tuple.Create (type, ascribeeType), s_canAscribeCacheValueFactory);
    }

    /// <summary>
    /// Returns the type arguments for the ascribed <paramref name="ascribeeType"/> as inherited or implemented by a given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which to return the type parameter. Must not be <see langword="null" />.</param>
    /// <param name="ascribeeType">The <see cref="Type"/> to check the <paramref name="type"/> against. Must not be <see langword="null" />.</param>
    /// <returns>A <see cref="Type"/> array containing the generic arguments of the <paramref name="ascribeeType"/> as it is inherited or implemented
    /// by <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the <paramref name="type"/> is not the <paramref name="ascribeeType"/> or its instantiation, its subclass or the implementation
    /// of an interface in case the <paramref name="ascribeeType"/> is an interface.
    /// </exception>
    /// <exception cref="AmbiguousMatchException">
    /// Thrown if the <paramref name="type"/> is an interface and implements the interface <paramref name="ascribeeType"/> or its instantiations
    /// more than once.
    /// </exception>
    public static IReadOnlyList<Type> GetAscribedGenericArguments (this Type type, Type ascribeeType)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("ascribeeType", ascribeeType);

      // C# compiler 7.2 already provides caching for anonymous method.
      return s_ascribedGenericArgumentsCache.GetOrAdd (
          Tuple.Create (type, ascribeeType), key => GetAscribedGenericArgumentsWithoutCache (key.Item1, key.Item2));
    }

    private static bool CanAscribeInternalFromCache (Type type, Type ascribeeType)
    {
      // C# compiler 7.2 already provides caching for anonymous method.
      return s_canAscribeInternalCache.GetOrAdd (Tuple.Create (type, ascribeeType), key => CanAscribeInternal (key.Item1, key.Item2));
    }

    private static bool CanAscribeInternal (Type type, Type ascribeeType)
    {
      if (!ascribeeType.IsGenericType)
        return ascribeeType.IsAssignableFrom (type);

      Type ascribeeGenericTypeDefinition = ascribeeType.GetGenericTypeDefinition();
      for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
      {
        if (CanDirectlyAscribeToGenericTypeInternalFromCache (currentType, ascribeeType, ascribeeGenericTypeDefinition))
          return true;
      }
      return false;
    }

    private static IReadOnlyList<Type> GetAscribedGenericArgumentsWithoutCache (Type type, Type ascribeeType)
    {
      if (!ascribeeType.IsGenericType)
      {
        if (ascribeeType.IsAssignableFrom (type))
          return Type.EmptyTypes;
        else
          throw ArgumentUtility.CreateArgumentTypeException ("type", type, ascribeeType);
      }
      else if (ascribeeType.IsInterface)
        return GetAscribedGenericInterfaceArgumentsInternal (type, ascribeeType);
      else
        return GetAscribedGenericClassArgumentsInternal (type, ascribeeType);
    }

    private static IReadOnlyList<Type> GetAscribedGenericInterfaceArgumentsInternal (Type type, Type ascribeeType)
    {
      Assertion.IsTrue (ascribeeType.IsGenericType);
      Assertion.IsTrue (ascribeeType.IsInterface);

      Type ascribeeGenericTypeDefinition = ascribeeType.GetGenericTypeDefinition ();

      Type conreteSpecialization; // concrete specialization of ascribeeType implemented by type
      // is type itself a specialization of ascribeeType?
      if (type.IsInterface && CanDirectlyAscribeToGenericTypeInternalFromCache (type, ascribeeType, ascribeeGenericTypeDefinition))
        conreteSpecialization = type;
      else
      {
        // Type.GetInterfaces will return all interfaces inherited by type. We will filter it to those that are directly ascribable
        // to ascribeeType. Since interfaces have no base types, these can only be closed or constructed specializations of ascribeeType.
        Type[] ascribableInterfaceTypes = Array.FindAll (
            type.GetInterfaces (),
            current => CanDirectlyAscribeToGenericTypeInternalFromCache (current, ascribeeType, ascribeeGenericTypeDefinition));

        if (ascribableInterfaceTypes.Length == 0)
          conreteSpecialization = null;
        else if (ascribableInterfaceTypes.Length == 1)
          conreteSpecialization = ascribableInterfaceTypes[0];
        else
        {
          string message = 
              String.Format ("The type {0} implements the given interface type {1} more than once.", type.FullName, ascribeeType.FullName);
          throw new AmbiguousMatchException (message);
        }
      }

      if (conreteSpecialization == null)
        throw ArgumentUtility.CreateArgumentTypeException ("type", type, ascribeeType);

      Assertion.IsTrue (conreteSpecialization.GetGenericTypeDefinition () == ascribeeType.GetGenericTypeDefinition ());
      return Array.AsReadOnly (conreteSpecialization.GetGenericArguments());
    }

    private static IReadOnlyList<Type> GetAscribedGenericClassArgumentsInternal (Type type, Type ascribeeType)
    {
      Assertion.IsTrue (ascribeeType.IsGenericType);
      Assertion.IsTrue (!ascribeeType.IsInterface);

      Type ascribeeGenericTypeDefinition = ascribeeType.GetGenericTypeDefinition ();

      // Search via base type until we find a type that is directly ascribable to the base type. That's the type whose generic arguments we want
      Type currentType = type;
      while (currentType != null && !CanDirectlyAscribeToGenericTypeInternalFromCache (currentType, ascribeeType, ascribeeGenericTypeDefinition))
        currentType = currentType.BaseType;

      if (currentType != null)
        return Array.AsReadOnly (currentType.GetGenericArguments());
      else
        throw ArgumentUtility.CreateArgumentTypeException ("type", type, ascribeeType);
    }

    private static bool CanDirectlyAscribeToGenericTypeInternalFromCache (Type type, Type ascribeeType, Type ascribeeGenericTypeDefinition)
    {
      // C# compiler 7.2 already provides caching for anonymous method.
      return s_canDirectlyAscribeToGenericTypeInternalCache.GetOrAdd (
        Tuple.Create (type, ascribeeType, ascribeeGenericTypeDefinition),
        key => CanDirectlyAscribeToGenericTypeInternal (key.Item1, key.Item2, key.Item3));
    }

    private static bool CanDirectlyAscribeToGenericTypeInternal (Type type, Type ascribeeType, Type ascribeeGenericTypeDefinition)
    {
      Assertion.IsNotNull (ascribeeType);

      if (!type.IsGenericType || type.GetGenericTypeDefinition () != ascribeeGenericTypeDefinition)
        return false;

      if (ascribeeType != ascribeeGenericTypeDefinition)
        return ascribeeType.IsAssignableFrom (type);
      else
        return ascribeeType.IsAssignableFrom (type.GetGenericTypeDefinition ());
    }
  }
}
