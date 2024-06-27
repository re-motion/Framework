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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Reflection;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  /// <summary>
  /// Provides support for building mixin configuration data from the declarative mixin configuration attributes
  /// (<see cref="UsesAttribute"/>, <see cref="ExtendsAttribute"/>, <see cref="ComposedInterfaceAttribute"/>,
  /// and <see cref="IgnoreForMixinConfigurationAttribute"/>).
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  public class DeclarativeConfigurationBuilder
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<DeclarativeConfigurationBuilder>();

    /// <summary>
    /// Builds a new <see cref="MixinConfiguration"/> from the declarative configuration information in the given assemblies without inheriting
    /// from a parent configuration.
    /// </summary>
    /// <param name="assemblies">The assemblies to be scanned for declarative mixin information.</param>
    /// <returns>A mixin configuration incorporating the configuration information held by the given assemblies.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static MixinConfiguration BuildConfigurationFromAssemblies (params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull("assemblies", assemblies);

      return BuildConfigurationFromAssemblies(null, (IEnumerable<Assembly>)assemblies);
    }

    /// <summary>
    /// Builds a new <see cref="MixinConfiguration"/> from the declarative configuration information in the given assemblies.
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration to derive the new configuration from (can be <see langword="null"/>).</param>
    /// <param name="assemblies">The assemblies to be scanned for declarative mixin information.</param>
    /// <returns>A mixin configuration inheriting from <paramref name="parentConfiguration"/> and incorporating the configuration information
    /// held by the given assemblies.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static MixinConfiguration BuildConfigurationFromAssemblies (MixinConfiguration parentConfiguration, params Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull("assemblies", assemblies);

      return BuildConfigurationFromAssemblies(parentConfiguration, (IEnumerable<Assembly>)assemblies);
    }

    /// <summary>
    /// Builds a new <see cref="MixinConfiguration"/> from the declarative configuration information in the given assemblies.
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration to derive the new configuration from (can be <see langword="null"/>).</param>
    /// <param name="assemblies">The assemblies to be scanned for declarative mixin information.</param>
    /// <returns>An mixin configuration inheriting from <paramref name="parentConfiguration"/> and incorporating the configuration information
    /// held by the given assemblies.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> parameter is <see langword="null"/>.</exception>
    public static MixinConfiguration BuildConfigurationFromAssemblies (MixinConfiguration? parentConfiguration, IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull("assemblies", assemblies);

      var builder = new DeclarativeConfigurationBuilder(parentConfiguration);
      foreach (Assembly assembly in assemblies)
        builder.AddAssembly(assembly);

      return builder.BuildConfiguration();
    }

    /// <summary>
    /// Builds a new <see cref="MixinConfiguration"/> from the declarative configuration information in the given types.
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration to derive the new configuration from (can be <see langword="null"/>).</param>
    /// <param name="types">The types to be scanned for declarative mixin information.</param>
    /// <returns>A mixin configuration inheriting from <paramref name="parentConfiguration"/> and incorporating the configuration information
    /// held by the given types.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="types"/> parameter is <see langword="null"/>.</exception>
    public static MixinConfiguration BuildConfigurationFromTypes (MixinConfiguration? parentConfiguration, IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull("types", types);

      var builder = new DeclarativeConfigurationBuilder(parentConfiguration);
      foreach (Type type in types)
      {
        if (!type.IsDefined(typeof(IgnoreForMixinConfigurationAttribute), false))
          builder.AddType(type);
      }

      return builder.BuildConfiguration();
    }

    /// <summary>
    /// Builds the default application configuration by analyzing all assemblies in the application bin directory and their (directly or indirectly)
    /// referenced assemblies for mixin configuration information. System assemblies are not scanned.
    /// </summary>
    /// <returns>A mixin configuration holding the default mixin configuration information for this application.</returns>
    /// <remarks>This method uses the <see cref="ContextAwareTypeUtility"/> to discover the types to be used in the mixin configuration.
    /// In design mode, this will use the types returned by the designer, but in ordinary application scenarios, the following steps are performed:
    /// <list type="number">
    /// <item>Retrieve all types assemblies from the current <see cref="AppDomain">AppDomain's</see> bin directory.</item>
    /// <item>Analyze each of them that is included by the <see cref="ApplicationAssemblyLoaderFilter"/> for mixin configuration information.</item>
    /// <item>Load the referenced assemblies of those assemblies if they aren't excluded by the <see cref="ApplicationAssemblyLoaderFilter"/>.</item>
    /// <item>If the loaded assemblies haven't already been analyzed, treat them according to steps 2-4.</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="ContextAwareTypeUtility"/>
    public static MixinConfiguration BuildDefaultConfiguration ()
    {
      ICollection types = GetTypeDiscoveryService().GetTypes(null, false);
      return BuildConfigurationFromTypes(null, types.Cast<Type>());
    }

    // Separate method because of tests
    private static ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      return ContextAwareTypeUtility.GetTypeDiscoveryService();
    }

    private readonly MixinConfiguration? _parentConfiguration;
    private readonly HashSet<Type> _allTypes = new HashSet<Type>();

    /// <summary>
    /// Initializes a new <see cref="DeclarativeConfigurationBuilder"/>, which can be used to collect assemblies and types with declarative
    /// mixin configuration attributes in order to build an <see cref="MixinConfiguration"/>.
    /// </summary>
    /// <param name="parentConfiguration">The parent configuration used when this instance builds a new <see cref="MixinConfiguration"/>.</param>
    public DeclarativeConfigurationBuilder (MixinConfiguration? parentConfiguration)
    {
      _parentConfiguration = parentConfiguration;
    }

    public IEnumerable<Type> AllTypes
    {
      get { return _allTypes; }
    }

    /// <summary>
    /// Scans the given assembly for declarative mixin configuration information and stores the information for a later call to <see cref="BuildConfiguration"/>.
    /// The mixin configuration information of types marked with the <see cref="IgnoreForMixinConfigurationAttribute"/> will be ignored.
    /// </summary>
    /// <param name="assembly">The assembly to be scanned.</param>
    /// <returns>A reference to this <see cref="DeclarativeConfigurationBuilder"/> object.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="assembly"/> parameter is <see langword="null"/>.</exception>
    public DeclarativeConfigurationBuilder AddAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);
      s_logger.LogDebug("Adding assembly {0} to DeclarativeConfigurationBuilder.", assembly);

      foreach (var t in AssemblyTypeCache.GetTypes(assembly))
      {
        if (!t.IsDefined(typeof(IgnoreForMixinConfigurationAttribute), false) && !MixinTypeUtility.IsGeneratedByMixinEngine(t))
          AddType(t);
      }
      return this;
    }

    /// <summary>
    /// Scans the given type for declarative mixin configuration information and stores the information for a later call to <see cref="BuildConfiguration"/>.
    /// The type will be scanned whether or not is is marked with the <see cref="IgnoreForMixinConfigurationAttribute"/>.
    /// </summary>
    /// <param name="type">The type to be scanned. This must be a non-generic type or a generic type definition. Closed generic types are not
    /// supported to be scanned.</param>
    /// <returns>A reference to this <see cref="DeclarativeConfigurationBuilder"/> object.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The given type is a closed generic type and not a generic type definition.</exception>
    public DeclarativeConfigurationBuilder AddType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      if (type.IsGenericType && !type.IsGenericTypeDefinition)
        throw new ArgumentException("Type must be non-generic or a generic type definition.", "type");

      _allTypes.Add(type);

      if (type.BaseType != null)
      {
        // When analyzing types for attributes, we want type definitions, not specializations
        if (type.BaseType.IsGenericType)
          AddType(type.BaseType.GetGenericTypeDefinition());
        else
          AddType(type.BaseType);
      }

      return this;
    }

    /// <summary>
    /// Analyzes the information added so far to this builder and creates a new <see cref="MixinConfiguration"/> from that data.
    /// </summary>
    /// <returns>An <see cref="MixinConfiguration"/> derived from the configuration specified in the builder's constructor containing
    /// <see cref="ClassContext"/> and <see cref="MixinContext"/> objects based on the information added so far.</returns>
    public MixinConfiguration BuildConfiguration ()
    {
      s_logger.LogInformation("Building mixin configuration from {0} types.", _allTypes.Count);

      using (StopwatchScope.CreateScope(s_logger, LogLevel.Information, "Time needed to build mixin configuration: {elapsed}."))
      {
        var typeAnalyzers = new IMixinDeclarationAnalyzer<Type>[] { CreateAttributeAnalyzer<Type>(), new HasComposedInterfaceMarkerAnalyzer() };
        var assemblyAnalyzers = new IMixinDeclarationAnalyzer<Assembly>[] { CreateAttributeAnalyzer<Assembly>() };

        var configurationAnalyzer = new DeclarativeConfigurationAnalyzer(typeAnalyzers, assemblyAnalyzers);

        var configurationBuilder = new MixinConfigurationBuilder(_parentConfiguration);
        configurationAnalyzer.Analyze(_allTypes, configurationBuilder);
        return configurationBuilder.BuildConfiguration();
      }
    }

    private static MixinConfigurationAttributeAnalyzer<T> CreateAttributeAnalyzer<T> ()
        where T : ICustomAttributeProvider
    {
      var handledAttributeContext = new HashSet<IMixinConfigurationAttribute<T>>();
      return new MixinConfigurationAttributeAnalyzer<T>(a => GetCustomAttributesWithDuplicateHandling(a, handledAttributeContext));
    }

    private static IEnumerable<IMixinConfigurationAttribute<T>> GetCustomAttributesWithDuplicateHandling<T> (
        T attributeProvider, HashSet<IMixinConfigurationAttribute<T>> handledAttributeContext)
        where T : ICustomAttributeProvider
    {
      var customAttributes =
          (IMixinConfigurationAttribute<T>[])attributeProvider.GetCustomAttributes(typeof(IMixinConfigurationAttribute<T>), false);

      foreach (var attribute in customAttributes)
      {
        if (!attribute.IgnoresDuplicates || !handledAttributeContext.Contains(attribute))
          yield return attribute;

        if (attribute.IgnoresDuplicates)
          handledAttributeContext.Add(attribute);
      }
    }
  }
}
