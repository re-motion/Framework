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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Implementation infrastructure for resolving the localized name of the <typeparamref name="TReflectionObject"/> 
  /// based on the applied <see cref="MultiLingualNameAttribute"/>s.
  /// </summary>
  /// <typeparam name="TReflectionObject">An <see cref="IMemberInformation"/> or <see cref="MemberInfo"/> to retrieve the localized name for.</typeparam>
  public abstract class LocalizedNameProviderBase<TReflectionObject>
      where TReflectionObject : class
  {
    // ReSharper disable StaticMemberInGenericType

    private static readonly IReadOnlyDictionary<CultureInfo, string> s_emptyDictionary = 
        new ReadOnlyDictionary<CultureInfo, string> (new Dictionary<CultureInfo, string>());

    private static readonly Lazy<CultureInfo> s_invariantCulture = new Lazy<CultureInfo> (
        () => CultureInfo.InvariantCulture,
        LazyThreadSafetyMode.ExecutionAndPublication);

    // ReSharper restore StaticMemberInGenericType

    private readonly ConcurrentDictionary<TReflectionObject, Lazy<IReadOnlyDictionary<CultureInfo, string>>> _localizedTypeNamesForTypeInformation =
        new ConcurrentDictionary<TReflectionObject, Lazy<IReadOnlyDictionary<CultureInfo, string>>>();

    private readonly ConcurrentDictionary<Assembly, Lazy<CultureInfo>> _neutralResourcesCultureForAssembly =
        new ConcurrentDictionary<Assembly, Lazy<CultureInfo>>();

    protected LocalizedNameProviderBase ()
    {
    }

    [NotNull]
    protected abstract IEnumerable<MultiLingualNameAttribute> GetCustomAttributes ([NotNull] TReflectionObject reflectionObject);

    [CanBeNull]
    protected abstract Assembly GetAssembly ([NotNull] TReflectionObject reflectionObject);

    [NotNull]
    protected abstract string GetContextForExceptionMessage ([NotNull] TReflectionObject reflectionObject);

    public bool TryGetLocalizedNameForCurrentUICulture ([NotNull] TReflectionObject reflectionObject, [CanBeNull] out string result)
    {
      ArgumentUtility.CheckNotNull ("reflectionObject", reflectionObject);

      var localizedNames = GetLocalizedNamesFromCache (reflectionObject);
      if (!localizedNames.Any())
      {
        result = null;
        return false;
      }

      var currentUICulture = CultureInfo.CurrentUICulture;
      foreach (var cultureInfo in currentUICulture.GetCultureHierarchy())
      {
        if (localizedNames.TryGetValue (cultureInfo, out result))
          return true;
      }

      throw new InvalidOperationException (
          string.Format ("{0} has no localization defined for the invariant culture.", GetContextForExceptionMessage (reflectionObject)));
    }

    private IReadOnlyDictionary<CultureInfo, string> GetLocalizedNamesFromCache (TReflectionObject reflectionObject)
    {
      var lazyAttributes = _localizedTypeNamesForTypeInformation.GetOrAdd (
          reflectionObject,
          new Lazy<IReadOnlyDictionary<CultureInfo, string>> (
              () => GetLocalizedNames (reflectionObject),
              LazyThreadSafetyMode.ExecutionAndPublication));

      return lazyAttributes.Value;
    }

    private IReadOnlyDictionary<CultureInfo, string> GetLocalizedNames (TReflectionObject reflectionObject)
    {
      var attributes = GetLocalizedNamesForReflectionObject (reflectionObject);

      if (!attributes.Any())
        return s_emptyDictionary;

      UpdateLocalizedNamesForAssemblyNeutralResourceCulture (reflectionObject, attributes);

      return attributes;
    }

    private Dictionary<CultureInfo, string> GetLocalizedNamesForReflectionObject (TReflectionObject reflectionObject)
    {
      var attributes = new Dictionary<CultureInfo, string>();
      foreach (var attribute in GetCustomAttributes (reflectionObject))
      {
        if (attributes.ContainsKey (attribute.Culture))
        {
          throw new InvalidOperationException (
              string.Format (
                  "{0} has more than one MultiLingualNameAttribute for the culture '{1}' applied. "
                  + "The used cultures must be unique within the set of MultiLingualNameAttributes.",
                  GetContextForExceptionMessage (reflectionObject),
                  attribute.Culture));
        }
        attributes.Add (attribute.Culture, attribute.LocalizedName);
      }
      return attributes;
    }

    private void UpdateLocalizedNamesForAssemblyNeutralResourceCulture (
        TReflectionObject reflectionObject,
        Dictionary<CultureInfo, string> attributes)
    {
      var assemblyNeutralResourcesCulture = GetAssemblyNeutralResourcesCulture (reflectionObject);
      string neutralLocalizedName;
      if (!attributes.TryGetValue (assemblyNeutralResourcesCulture, out neutralLocalizedName))
      {
        throw new InvalidOperationException (
            string.Format (
                "{0} has no MultiLingualNameAttribute for the assembly's neutral resource language ('{1}') applied. "
                + "The neutral resource language can be specified the NeutralResourcesLanguageAttribute; "
                + "the invariant culture is used as fallback if no attribute has been applied to the assembly.",
                GetContextForExceptionMessage (reflectionObject),
                assemblyNeutralResourcesCulture));
      }

      if (!attributes.ContainsKey (CultureInfo.InvariantCulture))
        attributes.Add (CultureInfo.InvariantCulture, neutralLocalizedName);
    }

    [NotNull]
    private CultureInfo GetAssemblyNeutralResourcesCulture (TReflectionObject reflectionObject)
    {
      var assembly = GetAssembly (reflectionObject);
      if (assembly == null)
        return CultureInfo.InvariantCulture;

      var cachedCulture = _neutralResourcesCultureForAssembly.GetOrAdd (assembly, GetAssemblyNeutralResourcesCulture);
      return cachedCulture.Value;
    }

    private Lazy<CultureInfo> GetAssemblyNeutralResourcesCulture (Assembly assembly)
    {
      var attribute = assembly.GetCustomAttribute<NeutralResourcesLanguageAttribute>();
      if (attribute == null)
        return s_invariantCulture;
      return new Lazy<CultureInfo> (() => CultureInfo.GetCultureInfo (attribute.CultureName), LazyThreadSafetyMode.ExecutionAndPublication);
    }
  }
}