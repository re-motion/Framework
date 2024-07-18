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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ExtensibleEnums;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Utility class for methods using reflection.
  /// </summary>
  public static class ReflectionUtility
  {
    private static readonly Type s_stringPropertyValueType = typeof(string);
    private static readonly Type s_binaryPropertyValueType = typeof(byte[]);
    private static readonly Type s_typePropertyValueType = typeof(Type);
    private static readonly Type s_objectIDPropertyValueType = typeof(ObjectID);
    private static readonly ConcurrentDictionary<Type, (bool CanAscribeTo, Type? ItemType)> s_objectListTypeCache = new();

    private static readonly Func<Type, ValueTuple<bool, Type?>> s_objectListTypeCacheValueFactory =
        static t =>
        {
          var canAscribeTo = typeof(IReadOnlyList<DomainObject>).IsAssignableFrom(t) && t.CanAscribeTo(typeof(ObjectList<>));
          return ValueTuple.Create(
              canAscribeTo,
              canAscribeTo
                  ? t.GetAscribedGenericArguments(typeof(ObjectList<>))[0]
                  : null);
        };

    private static readonly ConcurrentDictionary<Type, (bool IsMatchingType, Type? ItemType)> s_iObjectListTypeCache = new();

    private static readonly Func<Type, ValueTuple<bool, Type?>> s_iObjectListTypeCacheValueFactory =
        static t =>
        {
          var isIObjectList = IsGenericIObjectList(t);

          return ValueTuple.Create(
              isIObjectList,
              isIObjectList
                  ? t.GetAscribedGenericArguments(typeof(IObjectList<>))[0]
                  : null);

          static bool IsGenericIObjectList (Type type)
          {
            if (type == typeof(IObjectList<>))
              return true;

            if (!type.IsInterface)
              return false;

            if (!type.IsConstructedGenericType)
              return false;

            return type.GetGenericTypeDefinition() == typeof(IObjectList<>);
          }
        };

    /// <summary>
    /// Returns the directory of the current executing assembly.
    /// </summary>
    /// <returns>The path of the current executing assembly</returns>
    public static string GetConfigFileDirectory ()
    {
      return GetAssemblyDirectory(typeof(DomainObject).Assembly);
    }

    /// <summary>
    /// Gets the directory containing the given assembly.
    /// </summary>
    /// <param name="assembly">The assembly whose directory to retrieve.</param>
    /// <returns>The directory holding the given assembly as a local path. If the assembly has been shadow-copied, this returns the directory before the
    /// shadow-copying.</returns>
    /// <exception cref="InvalidOperationException">The assembly's code base is not a local path.</exception>
    public static string GetAssemblyDirectory (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);

      var assemblyLocation = assembly.Location;
      if (string.IsNullOrEmpty(assemblyLocation))
        throw new InvalidOperationException(string.Format("Assembly '{0}' does not have a location. It was likely loaded from a byte array.", assembly.FullName));

      // Guarding against URL-based paths isn't needed in .NET. Assemblies are never loaded from a URL anyway.

      var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
      Assertion.IsNotNull(assemblyDirectory, "Assembly location '{0}' does not contain a valid directory name.", assemblyLocation);
      return assemblyDirectory;
    }

    /// <summary>
    /// Creates an object of a given type.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> of the object to instantiate. Must not be <see langword="null"/>.</param>
    /// <param name="constructorParameters">The parameters for the constructor of the object.</param>
    /// <returns>The object that has been created.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException">Type <paramref name="type"/> has no suitable constructor for the given <paramref name="constructorParameters"/>.</exception>
    public static object CreateObject (Type type, params object[] constructorParameters)
    {
      ArgumentUtility.CheckNotNull("type", type);

      Type[] constructorParameterTypes = new Type[constructorParameters.Length];
      for (int i = 0; i < constructorParameterTypes.Length; i++)
        constructorParameterTypes[i] = constructorParameters[i].GetType();

      ConstructorInfo? constructor = type.GetConstructor(
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
          null,
          constructorParameterTypes,
          null);

      if (constructor != null)
        return constructor.Invoke(constructorParameters);
      else
      {
        throw new ArgumentException(
            String.Format(
                "Type '{0}' has no suitable constructor. Parameter types: ({1})",
                type,
                GetTypeListAsString(constructorParameterTypes)));
      }
    }

    internal static string GetTypeListAsString (Type?[] types)
    {
      ArgumentUtility.CheckNotNull("types", types);
      string result = String.Empty;
      foreach (Type? type in types)
      {
        if (result != String.Empty)
          result += ", ";

        if (type != null)
          result += type.ToString();
        else
          result += "<any reference type>";
      }

      return result;
    }

    public static string GetSignatureForArguments (object?[] args)
    {
      Type?[] argumentTypes = GetTypesForArgs(args);
      return GetTypeListAsString(argumentTypes);
    }

    public static Type?[] GetTypesForArgs (object?[] args)
    {
      Type?[] types = new Type?[args.Length];
      for (int i = 0; i < args.Length; i++)
      {
        object? argument = args[i];
        if (argument == null)
          types[i] = null;
        else
          types[i] = argument.GetType();
      }
      return types;
    }

    /// <summary>Returns the property name scoped for a specific <paramref name="originalDeclaringType"/>.</summary>
    [Obsolete("Use MappingConfiguration.Current.NameResolver.GetPropertyName(...).", true)]
    public static string GetPropertyName (Type originalDeclaringType, string propertyName)
    {
      throw new NotSupportedException("Use MappingConfiguration.Current.NameResolver.GetPropertyName(...).");
    }

    /// <summary>
    /// Evaluates whether the <paramref name="type"/> is an <see cref="ObjectList{T}"/> or derived from <see cref="ObjectList{T}"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="type"/> is an <see cref="ObjectList{T}"/> or derived from <see cref="ObjectList{T}"/>.
    /// </returns>
    public static bool IsObjectList (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return s_objectListTypeCache.GetOrAdd(type, s_objectListTypeCacheValueFactory).CanAscribeTo;
    }

    /// <summary>
    /// Evaluates whether the <paramref name="type"/> is an <see cref="IObjectList{T}"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="type"/> is an <see cref="IObjectList{T}"/>.
    /// </returns>
    public static bool IsIObjectList (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return s_iObjectListTypeCache.GetOrAdd(type, s_iObjectListTypeCacheValueFactory).IsMatchingType;
    }

    /// <summary>
    /// Checks if a property type is a domain object.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the given type is a domain object.</returns>
    public static bool IsDomainObject (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return (typeof(DomainObject).IsAssignableFrom(type));
    }

    /// <summary>
    /// Checks if a property type is a relation property.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the given type is a relation property.</returns>
    public static bool IsRelationType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return IsDomainObject(type) || IsObjectList(type) || IsIObjectList(type);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsBinaryPropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull("propertyType", propertyType);
      return s_binaryPropertyValueType.Equals(propertyType);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsStringPropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull("propertyType", propertyType);
      return s_stringPropertyValueType.Equals(propertyType);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsExtensibleEnumPropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull("propertyType", propertyType);
      return ExtensibleEnumUtility.IsExtensibleEnumType(propertyType);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsStructuralEquatablePropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull("propertyType", propertyType);
      return typeof(IStructuralEquatable).IsAssignableFrom(propertyType);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsObjectIDPropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull("propertyType", propertyType);
      return s_objectIDPropertyValueType.Equals(propertyType);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsTypePropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull("propertyType", propertyType);
      return s_typePropertyValueType.Equals(propertyType);
    }

    /// <summary>
    /// Returns the type parameter of the <see cref="ObjectList{T}"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which to return the type parameter. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="Type"/> if the <paramref name="type"/> is a closed <see cref="ObjectList{T}"/> or <see langword="null"/> if the generic 
    /// <see cref="ObjectList{T}"/> is open.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the type is not an <see cref="ObjectList{T}"/> or derived from <see cref="ObjectList{T}"/>.
    /// </exception>
    public static Type? GetObjectListTypeParameter (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var typeParameter = s_objectListTypeCache.GetOrAdd(type, s_objectListTypeCacheValueFactory).ItemType;

      if (typeParameter is null)
        throw ArgumentUtility.CreateArgumentTypeException("type", type, typeof(ObjectList<>));

      if (typeParameter.IsGenericParameter)
        return null;

      return typeParameter;
    }

    /// <summary>
    /// Returns the type parameter of the <see cref="IObjectList{T}"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which to return the type parameter. Must not be <see langword="null" />.</param>
    /// <returns>
    /// A <see cref="Type"/> if the <paramref name="type"/> is a closed <see cref="IObjectList{T}"/> or <see langword="null"/> if the generic 
    /// <see cref="IObjectList{T}"/> is open.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the type is not an <see cref="IObjectList{T}"/> or implements <see cref="IObjectList{T}"/>.
    /// </exception>
    public static Type? GetIObjectListTypeParameter (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var typeParameter = s_iObjectListTypeCache.GetOrAdd(type, s_iObjectListTypeCacheValueFactory).ItemType;

      if (typeParameter is null)
        throw ArgumentUtility.CreateArgumentTypeException("type", type, typeof(IObjectList<>));

      if (typeParameter.IsGenericParameter)
        return null;

      return typeParameter;
    }

    /// <summary>
    /// Gets the type of the related object (the type of the <see cref="DomainObject"/>) for a relation <paramref name="propertyInfo"/>.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="IPropertyInformation"/> to analyze.</param>
    /// <returns>the domain object type of the given property.</returns>
    public static Type? GetRelatedObjectTypeFromRelationProperty (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      if (IsObjectList(propertyInfo.PropertyType))
        return GetObjectListTypeParameter(propertyInfo.PropertyType);
      if (IsIObjectList(propertyInfo.PropertyType))
        return GetIObjectListTypeParameter(propertyInfo.PropertyType);
      else if (propertyInfo.PropertyType.IsGenericParameter)
        return null;
      else
        return propertyInfo.PropertyType;
    }

    /// <summary>
    /// Gets the declaring domain object type for the given property.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="IPropertyInformation"/> to analyze.</param>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> of the given <see cref="IPropertyInformation"/></param>
    /// <returns>the declaring domain object type for the given property.</returns>
    public static Type GetDeclaringDomainObjectTypeForProperty (IPropertyInformation propertyInfo, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      var persistentMixin = GetPersistentMixinTypeForProperty(propertyInfo, classDefinition);
      if (persistentMixin != null)
      {
        var originalMixinTarget =
            classDefinition.PersistentMixinFinder.FindOriginalMixinTarget(persistentMixin);
        if (originalMixinTarget == null)
          throw new InvalidOperationException(
              String.Format("IPersistentMixinFinder.FindOriginalMixinTarget (DeclaringMixin) evaluated and returned null."));
        return originalMixinTarget;
      }
      else
        return propertyInfo.DeclaringType!.ConvertToRuntimeType();
    }

    /// <summary>
    /// Checks if the given <see cref="PropertyInfo"/> on the given <see cref="ClassDefinition"/> is a mixed property.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="IPropertyInformation"/> to analyze.</param>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> of the given <see cref="IPropertyInformation"/></param>
    /// <returns><see langword="true" /> if the given <see cref="PropertyInfo"/> is a mixed property.</returns>
    public static bool IsMixedProperty (IPropertyInformation propertyInfo, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      return GetPersistentMixinTypeForProperty(propertyInfo, classDefinition) != null;
    }

    private static Type? GetPersistentMixinTypeForProperty (IPropertyInformation propertyInfo, ClassDefinition classDefinition)
    {
      return classDefinition.GetPersistentMixin(propertyInfo.DeclaringType!.ConvertToRuntimeType());
    }

    /// <summary>
    /// Checks if the given type is the inheritance root. A type is the inheritance root if it is either the domain object base or if the
    /// type has the <see cref="StorageGroupAttribute"/> applied.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to be analyzed</param>
    /// <returns><see langword="true" /> if the given type is the inheritance root.</returns>
    public static bool IsInheritanceRoot (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("type", type, typeof(DomainObject));

      if (IsTypeIgnoredForMappingConfiguration(type))
        return false;

      if (AttributeUtility.IsDefined<StorageGroupAttribute>(type, false))
        return true;

      return type.BaseType.CreateSequence(t => t.BaseType, IsDomainObject).All(IsTypeIgnoredForMappingConfiguration);
    }

    public static bool IsTypeIgnoredForMappingConfiguration (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("type", type, typeof(DomainObject));

      return AttributeUtility.IsDefined<IgnoreForMappingConfigurationAttribute>(type, false);
    }
  }
}
