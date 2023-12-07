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
using System.ComponentModel.Design;
using System.Configuration;
using Remotion.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Configures ContextAwareTypeUtilityormed by <see cref="ContextAwareTypeUtility.GetTypeDiscoveryService"/>.
  /// </summary>
  public sealed class TypeDiscoveryConfiguration : ConfigurationSection
  {
    // TODO RM-7788: The Type property of CustomRootAssemblyFinder & CustomTypeDiscoveryService should be constrained to reference types.

    private static readonly DoubleCheckedLockingContainer<TypeDiscoveryConfiguration> s_current =
        new DoubleCheckedLockingContainer<TypeDiscoveryConfiguration>(GetTypeDiscoveryConfiguration);

    /// <summary>
    /// Gets the current <see cref="TypeDiscoveryConfiguration"/> instance. This is used by 
    /// <see cref="ContextAwareTypeUtility.GetTypeDiscoveryService"/> to retrieve a <see cref="ITypeDiscoveryService"/> instance
    /// if no <see cref="IDesignerHost"/> is available.
    /// </summary>
    /// <value>The current <see cref="TypeDiscoveryConfiguration"/>.</value>
    public static TypeDiscoveryConfiguration Current
    {
      get { return s_current.Value; }
    }

    /// <summary>
    /// Sets the <see cref="Current"/> <see cref="TypeDiscoveryConfiguration"/> instance.
    /// </summary>
    /// <param name="configuration">The new configuration to set as the <see cref="Current"/> configuration.</param>
    public static void SetCurrent (TypeDiscoveryConfiguration configuration)
    {
      s_current.Value = configuration;
    }

    private static TypeDiscoveryConfiguration GetTypeDiscoveryConfiguration ()
    {
      return (TypeDiscoveryConfiguration)(ConfigurationWrapper.Current.GetSection("remotion.typeDiscovery", false) ?? new TypeDiscoveryConfiguration());
    }

    /// <summary>
    /// Initializes a new default instance of the <see cref="TypeDiscoveryConfiguration"/> class. To load the configuration from a config file,
    /// use <see cref="ConfigurationWrapper.GetSection(string)"/> instead.
    /// </summary>
    public TypeDiscoveryConfiguration ()
    {
      var xmlnsProperty = new ConfigurationProperty("xmlns", typeof(string), null, ConfigurationPropertyOptions.None);
      Properties.Add(xmlnsProperty);
    }

    /// <summary>
    /// Gets or sets the <see cref="TypeDiscoveryMode"/> to be used for type discovery.
    /// </summary>
    /// <value>The <see cref="TypeDiscoveryMode"/> to be used for type discovery.</value>
    [ConfigurationProperty("mode", DefaultValue = TypeDiscoveryMode.Automatic, IsRequired = false)]
    public TypeDiscoveryMode Mode
    {
      get { return (TypeDiscoveryMode)this["mode"]; }
      set { this["mode"] = value; }
    }

    /// <summary>
    /// Gets a <see cref="RootAssembliesElement"/> describing specific root assemblies to be used. This is only relevant
    /// if <see cref="Mode"/> is set to <see cref="TypeDiscoveryMode.SpecificRootAssemblies"/>. In this mode, an 
    /// <see cref="AssemblyFinderTypeDiscoveryService"/> is created, and the given root assemblies are employed for type discovery.
    /// Note that even if an assembly is specified as a root assembly, the default filtering rules (<see cref="ApplicationAssemblyLoaderFilter"/>)
    /// still apply even for that assembly.
    /// </summary>
    /// <value>A <see cref="RootAssembliesElement"/> describing specific root assemblies to be used.</value>
    [ConfigurationProperty("specificRootAssemblies", IsRequired = false)]
    public RootAssembliesElement SpecificRootAssemblies
    {
      get { return (RootAssembliesElement)this["specificRootAssemblies"]; }
    }

    /// <summary>
    /// Creates an <see cref="ITypeDiscoveryService"/> instance as indicated by <see cref="Mode"/>.
    /// </summary>
    /// <returns>A new <see cref="ITypeDiscoveryService"/> that discovers types as indicated by <see cref="Mode"/>.</returns>
    public ITypeDiscoveryService CreateTypeDiscoveryService ()
    {
      switch (Mode)
      {
        case TypeDiscoveryMode.SpecificRootAssemblies:
          return CreateServiceWithSpecificRootAssemblies();
        default:
          return CreateServiceWithAutomaticDiscovery();
      }
    }

    private ITypeDiscoveryService CreateServiceWithSpecificRootAssemblies ()
    {
      var assemblyLoader = CreateAllAssemblyLoader();
      var rootAssemblyFinder = SpecificRootAssemblies.CreateRootAssemblyFinder(assemblyLoader);
      return CreateServiceWithAssemblyFinder(rootAssemblyFinder);
    }

    private ITypeDiscoveryService CreateServiceWithAutomaticDiscovery ()
    {
      var assemblyLoader = CreateApplicationAssemblyLoader();
      var searchPathRootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(false, assemblyLoader);
      return CreateServiceWithAssemblyFinder(searchPathRootAssemblyFinder);
    }

    private ITypeDiscoveryService CreateServiceWithAssemblyFinder (IRootAssemblyFinder customRootAssemblyFinder)
    {
      var filteringAssemblyLoader = CreateApplicationAssemblyLoader();
      var assemblyFinder = new CachingAssemblyFinderDecorator(new AssemblyFinder(customRootAssemblyFinder, filteringAssemblyLoader));
      return new AssemblyFinderTypeDiscoveryService(assemblyFinder);
    }

    private IAssemblyLoader CreateApplicationAssemblyLoader ()
    {
      return new FilteringAssemblyLoader(ApplicationAssemblyLoaderFilter.Instance);
    }

    private IAssemblyLoader CreateAllAssemblyLoader ()
    {
      return new FilteringAssemblyLoader(new LoadAllAssemblyLoaderFilter());
    }
  }
}
