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
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Remotion.Logging;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery
{
  /// <summary>
  /// Provides an implementation of the <see cref="ITypeDiscoveryService"/> interface that uses an <see cref="AssemblyFinder"/> to
  /// retrieve types. This class is the default <see cref="ITypeDiscoveryService"/> provided by <see cref="ContextAwareTypeUtility.GetTypeDiscoveryService"/>
  /// in the standard context.
  /// </summary>
  public sealed class AssemblyFinderTypeDiscoveryService : ITypeDiscoveryService
  {
    private static readonly Lazy<ILog> s_log = new Lazy<ILog>(() => LogManager.GetLogger(typeof(AssemblyFinderTypeDiscoveryService)));

    private readonly IAssemblyFinder _assemblyFinder;
    private readonly Lazy<BaseTypeCache> _baseTypeCache;

#if FEATURE_GAC
    private readonly ConcurrentDictionary<Type, ReadOnlyCollection<Type>> _globalTypesCache =
        new ConcurrentDictionary<Type, ReadOnlyCollection<Type>>();
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyFinderTypeDiscoveryService"/> class with a specific <see cref="AssemblyFinder"/>
    /// instance.
    /// </summary>
    /// <param name="assemblyFinder">The assembly finder used by this service instance to retrieve types.</param>
    public AssemblyFinderTypeDiscoveryService (IAssemblyFinder assemblyFinder)
    {
      ArgumentUtility.CheckNotNull("assemblyFinder", assemblyFinder);
      _assemblyFinder = assemblyFinder;
      _baseTypeCache = new Lazy<BaseTypeCache>(CreateBaseTypeCache);
    }

    /// <summary>
    /// Gets the assembly finder used by this service to discover types. The service simply returns the types returned by the
    /// <see cref="Assembly.GetTypes"/> method for the assemblies found by this object.
    /// </summary>
    /// <value>The assembly finder used for type discovery.</value>
    public IAssemblyFinder AssemblyFinder
    {
      get { return _assemblyFinder; }
    }

    /// <summary>
    /// Retrieves the list of types available in the assemblies found by the <see cref="AssemblyFinder"/> specified in the constructor.
    /// </summary>
    /// <param name="baseType">The base type to match. Can be null.</param>
    /// <param name="excludeGlobalTypes">Indicates whether types from all referenced assemblies should be checked.</param>
    /// <returns>
    /// A collection of types that match the criteria specified by baseType and excludeGlobalTypes.
    /// </returns>
    public ICollection GetTypes (Type? baseType, bool excludeGlobalTypes)
    {
      var nonNullBaseType = baseType ?? typeof(object);

      if (nonNullBaseType.IsSealed) // ValueTypes are also sealed
        return new[] { nonNullBaseType };

      if (!excludeGlobalTypes && AssemblyTypeCache.IsGacAssembly(nonNullBaseType.Assembly))
      {
#if FEATURE_GAC
        // C# compiler 7.2 does not provide caching for anonymous method but calls are only during application start so no caching is needed.
        return _globalTypesCache.GetOrAdd(
            nonNullBaseType,
            key =>
            {
              s_log.Value.DebugFormat("Discovering types derived from '{0}', including GAC...", key);
              using (StopwatchScope.CreateScope(
                  s_log.Value,
                  LogLevel.Info,
                  string.Format("Discovered types derived from '{0}', including GAC. Time taken: {{elapsed}}", key)))
              {
                return GetTypesFromAllAssemblies(key, excludeGlobalTypes: false).ToList().AsReadOnly();
              }
            });
#else
        throw new PlatformNotSupportedException(
            $"{nameof(AssemblyTypeCache)} resolved type '{baseType}' as belonging to the Global Assembly Cache, but the Global Assembly Cache does not exist on this platform.");
#endif
      }

      var baseTypeCache = _baseTypeCache.Value;
      Assertion.IsTrue(_baseTypeCache.IsValueCreated);

      return baseTypeCache.GetTypes(nonNullBaseType);
    }

    private BaseTypeCache CreateBaseTypeCache ()
    {
      s_log.Value.DebugFormat("Creating cache for all types in application directory...");
      using (StopwatchScope.CreateScope(
          s_log.Value,
          LogLevel.Info,
          "Created cache for all types in application directory. Time taken: {elapsed}"))
      {
        return BaseTypeCache.Create(GetTypesFromAllAssemblies(null, true));
      }
    }

    private IEnumerable<Type> GetTypesFromAllAssemblies (Type? baseType, bool excludeGlobalTypes)
    {
#if !FEATURE_GAC
      Assertion.DebugAssert(baseType == null, "{0} parameter must be 'null'.", nameof(baseType));
      Assertion.DebugAssert(excludeGlobalTypes == true, "{0} parameter must be 'true'.", nameof(excludeGlobalTypes));
#endif

      return GetAssemblies(excludeGlobalTypes).AsParallel().SelectMany(a => GetTypesFromBaseType(a, baseType));
    }

    private IEnumerable<Assembly> GetAssemblies (bool excludeGlobalTypes)
    {
      var assemblies = _assemblyFinder.FindAssemblies();

#if FEATURE_GAC
      return assemblies.Where(assembly => !excludeGlobalTypes || !assembly.GlobalAssemblyCache);
#else
      Assertion.IsTrue(excludeGlobalTypes, "{0} parameter must be 'true'.", nameof(excludeGlobalTypes));
      return assemblies;
#endif
    }

    private IEnumerable<Type> GetTypesFromBaseType (Assembly assembly, Type? baseType)
    {
#if !FEATURE_GAC
      Assertion.DebugAssert(baseType == null, "{0} parameter must be 'null'.", nameof(baseType));
#endif

      IReadOnlyCollection<Type> allTypesInAssembly;

      try
      {
        allTypesInAssembly = AssemblyTypeCache.GetTypes(assembly);
      }
      catch (ReflectionTypeLoadException ex)
      {
        string message = string.Format(
            "The types from assembly '{0}' could not be loaded.{1}{2}",
            assembly.GetName(),
            Environment.NewLine,
            string.Join(Environment.NewLine, ex.LoaderExceptions.Select(e => e!.Message))); // TODO RM-7753: Message property should not be accessed for null items.
        throw new TypeLoadException(message, ex);
      }

      if (baseType == null)
        return allTypesInAssembly;

      return GetFilteredTypes(allTypesInAssembly, baseType);
    }

    private IEnumerable<Type> GetFilteredTypes (IEnumerable<Type> types, Type baseType)
    {
#if FEATURE_GAC
      var isBaseTypeAGenericTypeDefinition = baseType.IsGenericTypeDefinition;
      return types.Where(type => baseType.IsAssignableFrom(type) || (isBaseTypeAGenericTypeDefinition && type.CanAscribeTo(baseType)));
#else
      throw new PlatformNotSupportedException($"{nameof(GetFilteredTypes)} is only used with GAC lookups but the GAC is not supported on this this platform.");
#endif
    }
  }
}
