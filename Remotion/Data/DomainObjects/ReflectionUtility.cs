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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
    private static readonly Type s_stringPropertyValueType = typeof (string);
    private static readonly Type s_binaryPropertyValueType = typeof (byte[]);
    private static readonly Type s_typePropertyValueType = typeof (Type);
    private static readonly Type s_objectIDPropertyValueType = typeof (ObjectID);

    /// <summary>
    /// Returns the directory of the current executing assembly.
    /// </summary>
    /// <returns>The path of the current executing assembly</returns>
    public static string GetConfigFileDirectory ()
    {
      return GetAssemblyDirectory (typeof (DomainObject).Assembly);
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
      return GetAssemblyDirectory ((_Assembly) assembly);
    }

    /// <summary>
    /// Gets the directory containing the given assembly.
    /// </summary>
    /// <param name="assembly">The assembly whose directory to retrieve.</param>
    /// <returns>The directory holding the given assembly as a local path. If the assembly has been shadow-copied, this returns the directory before the
    /// shadow-copying.</returns>
    /// <exception cref="InvalidOperationException">The assembly's code base is not a local path.</exception>
    [CLSCompliant (false)]
    public static string GetAssemblyDirectory (_Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      Uri codeBaseUri = new Uri (assembly.EscapedCodeBase);
      if (!codeBaseUri.IsFile)
        throw new InvalidOperationException (String.Format ("The assembly's code base '{0}' is not a local path.", codeBaseUri.OriginalString));
      return Path.GetDirectoryName (codeBaseUri.LocalPath);
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
      ArgumentUtility.CheckNotNull ("type", type);

      Type[] constructorParameterTypes = new Type[constructorParameters.Length];
      for (int i = 0; i < constructorParameterTypes.Length; i++)
        constructorParameterTypes[i] = constructorParameters[i].GetType();

      ConstructorInfo constructor = type.GetConstructor (
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
          null,
          constructorParameterTypes,
          null);

      if (constructor != null)
        return constructor.Invoke (constructorParameters);
      else
      {
        throw new ArgumentException (
            String.Format (
                "Type '{0}' has no suitable constructor. Parameter types: ({1})",
                type,
                GetTypeListAsString (constructorParameterTypes)));
      }
    }

    internal static string GetTypeListAsString (Type[] types)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      string result = String.Empty;
      foreach (Type type in types)
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

    public static string GetSignatureForArguments (object[] args)
    {
      Type[] argumentTypes = GetTypesForArgs (args);
      return GetTypeListAsString (argumentTypes);
    }

    public static Type[] GetTypesForArgs (object[] args)
    {
      Type[] types = new Type[args.Length];
      for (int i = 0; i < args.Length; i++)
      {
        if (args[i] == null)
          types[i] = null;
        else
          types[i] = args[i].GetType();
      }
      return types;
    }

    /// <summary>
    /// Returns the reflection based property identifier for a given property member.
    /// </summary>
    /// <param name="propertyInfo">The property whose identifier should be returned. Must not be <see langword="null" />.</param>
    /// <returns>The property identifier for the given property.</returns>
    /// <remarks>
    /// Currently, the identifier is defined to be the full name of the property's declaring type, suffixed with a dot (".") and the
    /// property's name (e.g. MyNamespace.MyType.MyProperty). However, this might change in the future, so this API should be used whenever the
    /// identifier must be retrieved programmatically.
    /// </remarks>
    [Obsolete ("Use MappingConfiguration.NameResolver.GetPropertyName.")]
    public static string GetPropertyName (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      Type originalDeclaringType = PropertyInfoExtensions.GetOriginalDeclaringType (propertyInfo);
      if (originalDeclaringType.IsGenericType)
        return GetPropertyName (originalDeclaringType.GetGenericTypeDefinition(), propertyInfo.Name);
      else
        return GetPropertyName (originalDeclaringType, propertyInfo.Name);
    }

    /// <summary>Returns the property name scoped for a specific <paramref name="originalDeclaringType"/>.</summary>
    [Obsolete ("Use MappingConfiguration.NameResolver.GetPropertyName.")]
    public static string GetPropertyName (Type originalDeclaringType, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("type", originalDeclaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return originalDeclaringType.FullName + "." + propertyName;
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
      return TypeExtensions.CanAscribeTo (type, typeof (ObjectList<>));
    }

    /// <summary>
    /// Checks if a property type is a domain object.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the given type is a domain object.</returns>
    public static bool IsDomainObject (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return (typeof (DomainObject).IsAssignableFrom (type));
    }

    /// <summary>
    /// Checks if a property type is a relation property.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the given type is a relation property.</returns>
    public static bool IsRelationType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return IsDomainObject (type) || IsObjectList (type);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsBinaryPropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      return s_binaryPropertyValueType.Equals(propertyType);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsStringPropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      return s_stringPropertyValueType.Equals (propertyType);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsExtensibleEnumPropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      return ExtensibleEnumUtility.IsExtensibleEnumType (propertyType);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsObjectIDPropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      return s_objectIDPropertyValueType.Equals (propertyType);
    }

    /// <remarks>Only temporary solution until type resulition is refactored.</remarks>
    internal static bool IsTypePropertyValueType (Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      return s_typePropertyValueType.Equals (propertyType);
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
    public static Type GetObjectListTypeParameter (Type type)
    {
      Type[] typeParameters = TypeExtensions.GetAscribedGenericArguments (type, typeof (ObjectList<>));
      if (typeParameters == null)
        return null;
      return typeParameters[0];
    }

    /// <summary>
    /// Gets the type of the related object (the type of the <see cref="DomainObject"/>) for a relation <paramref name="propertyInfo"/>.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="IPropertyInformation"/> to analyze.</param>
    /// <returns>the domain object type of the given property.</returns>
    public static Type GetRelatedObjectTypeFromRelationProperty (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (IsObjectList (propertyInfo.PropertyType))
        return GetObjectListTypeParameter (propertyInfo.PropertyType);
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
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (IsMixedProperty (propertyInfo, classDefinition))
      {
        var originalMixinTarget =
            classDefinition.PersistentMixinFinder.FindOriginalMixinTarget (classDefinition.GetPersistentMixin (propertyInfo.DeclaringType.ConvertToRuntimeType ()));
        if (originalMixinTarget == null)
          throw new InvalidOperationException (
              String.Format ("IPersistentMixinFinder.FindOriginalMixinTarget (DeclaringMixin) evaluated and returned null."));
        return originalMixinTarget;
      }
      else
        return propertyInfo.DeclaringType.ConvertToRuntimeType();
    }

    /// <summary>
    /// Checks if the given <see cref="PropertyInfo"/> on the given <see cref="ClassDefinition"/> is a mixed property.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="IPropertyInformation"/> to analyze.</param>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> of the given <see cref="IPropertyInformation"/></param>
    /// <returns><see langword="true" /> if the given <see cref="PropertyInfo"/> is a mixed property.</returns>
    public static bool IsMixedProperty (IPropertyInformation propertyInfo, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return classDefinition.GetPersistentMixin (propertyInfo.DeclaringType.ConvertToRuntimeType()) != null;
    }

    /// <summary>
    /// Checks if the given type is the inheritance root. A type is the inheritance root if it is either the domain object base or if the
    /// type has the <see cref="StorageGroupAttribute"/> applied.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to be analyzed</param>
    /// <returns><see langword="true" /> if the given type is the inheritance root.</returns>
    public static bool IsInheritanceRoot (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));

      if (IsTypeIgnoredForMappingConfiguration (type))
        return false;

      if (AttributeUtility.IsDefined<StorageGroupAttribute> (type, false))
        return true;

      return type.BaseType.CreateSequence (t => t.BaseType, IsDomainObject).All (IsTypeIgnoredForMappingConfiguration);
    }

    public static bool IsTypeIgnoredForMappingConfiguration (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));

      return AttributeUtility.IsDefined<IgnoreForMappingConfigurationAttribute> (type, false);
    }
  }
}