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
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.Utilities
{
  /// <summary>
  /// Utility methods for handling types.
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  public static partial class TypeUtility
  {
    private static readonly ConcurrentDictionary<string, string> s_fullTypeNames = new ConcurrentDictionary<string, string>();
    private static readonly ConcurrentDictionary<Type, string> s_partialAssemblyQualifiedNameCache = new ConcurrentDictionary<Type, string>();

    /// <summary>The <see cref="Lazy{T}"/> protects the expensive regex-creation.</summary>
    private static readonly Lazy<AbbreviationParser> s_abbreviationParser = new Lazy<AbbreviationParser>(() => new AbbreviationParser());

    private static readonly AbbreviationBuilder s_abbreviationBuilder = new AbbreviationBuilder();

    ///<remarks>Optimized for memory allocations</remarks>
    private static readonly Func<string, string> s_parseAbbreviatedTypeNameWithoutCacheFunc = ParseAbbreviatedTypeNameWithoutCache;

    /// <summary>
    ///   Converts abbreviated qualified type names into standard qualified type names.
    /// </summary>
    /// <remarks>
    ///   Abbreviated type names use the format <c>assemblyname::subnamespace.type</c>. For instance, the
    ///   abbreviated type name <c>"Remotion.Web::Utilities.ControlHelper"</c> would result in the standard
    ///   type name <c>"Remotion.Web.Utilities.ControlHelper, Remotion.Web"</c>.
    /// </remarks>
    /// <param name="typeName"> A standard or abbreviated type name. </param>
    /// <returns> A standard type name as expected by <see cref="Type.GetType(string)"/>. </returns>
    [CanBeNull]
    [ContractAnnotation("typeName:notnull => notnull;typeName:null => null")]
    [return: NotNullIfNotNull("typeName")]
    public static string? ParseAbbreviatedTypeName ([CanBeNull]string? typeName)
    {
      if (typeName == null)
        return null;

      return s_fullTypeNames.GetOrAdd(typeName, s_parseAbbreviatedTypeNameWithoutCacheFunc);
    }

    private static string ParseAbbreviatedTypeNameWithoutCache ([JetBrains.Annotations.NotNull] string typeName)
    {
      // Optimization to prevent instantiating the AbbreviationParser unless necessary.
      if (!AbbreviationParser.IsAbbreviatedTypeName(typeName))
        return typeName;

      return s_abbreviationParser.Value.ParseAbbreviatedTypeName(typeName);
    }

    /// <summary>
    ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    /// <remarks>
    /// This method uses <see cref="ContextAwareTypeUtility"/>. By default, it will search all assemblies for the requested type.
    /// In the designer context, <see cref="IDesignerHost"/> is used for the lookup.
    /// </remarks>
    [CanBeNull]
    public static Type? GetType ([JetBrains.Annotations.NotNull]string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      return TypeResolutionService.GetType(ParseAbbreviatedTypeName(name), false);
    }

    /// <summary>
    ///   Loads a type by name, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
    /// </summary>
    /// <param name="name">
    /// The name of the type to get. This must follow the conventions of <see cref="Type.GetType(string,bool)"/>.
    /// </param>
    /// <param name="throwOnError">
    /// If <see langword="true" />, a <see cref="TypeLoadException"/> is thrown if the given type cannot be loaded. Otherwise, <see langword="null" /> is returned.
    /// </param>
    /// <returns>The type with the given name, retrieved either from the designer or via <see cref="Type.GetType(string,bool)"/>.</returns>
    /// <remarks>
    /// By default, it will search all assemblies for the requested type using <see cref="Type.GetType(string,bool)"/>. In the designer context, 
    /// the designer services (<see cref="IDesignerHost"/>) are used for the lookup.
    /// </remarks>
    [CanBeNull]
    [ContractAnnotation("throwOnError:true => notnull")]
    public static Type? GetType ([JetBrains.Annotations.NotNull]string name, bool throwOnError)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      return TypeResolutionService.GetType(ParseAbbreviatedTypeName(name), throwOnError);
    }

    /// <summary>
    /// Gets the type and assembly name without the version, culture, and public key token.
    /// </summary>
    [JetBrains.Annotations.NotNull]
    public static string GetPartialAssemblyQualifiedName ([JetBrains.Annotations.NotNull]Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      // C# compiler 7.2 already provides caching for anonymous method.
      return s_partialAssemblyQualifiedNameCache.GetOrAdd(type, key => key.GetFullNameChecked() + ", " + key.Assembly.GetName().GetNameChecked());
    }

    /// <summary>
    /// Gets the type name in abbreviated syntax (<see cref="ParseAbbreviatedTypeName"/>).
    /// </summary>
    [JetBrains.Annotations.NotNull]
    public static string GetAbbreviatedTypeName ([JetBrains.Annotations.NotNull]Type type, bool includeVersionAndCulture)
    {
      ArgumentUtility.CheckNotNull("type", type);
      return s_abbreviationBuilder.BuildAbbreviatedTypeName(type, includeVersionAndCulture);
    }

    private static ITypeResolutionService TypeResolutionService
    {
      get { return SafeServiceLocator.Current.GetInstance<ITypeResolutionService>(); }
    }
  }
}
