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
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.IO;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Configuration
{
  /// <summary>Abstract base class for <see cref="ProviderHelperBase{T}"/>.</summary>
  public abstract class ProviderHelperBase
  {
    /// <summary>Initializes properties and adds them to the given <see cref="ConfigurationPropertyCollection"/>.</summary>
    public abstract void InitializeProperties (ConfigurationPropertyCollection properties);

    public abstract void PostDeserialze ();
  }

  /// <summary>Base for helper classes that load specific providers from the <see cref="System.Configuration.ConfigurationSection"/> section.</summary>
  /// <remarks>
  ///   <see cref="ProviderHelperBase{T}"/> is designed to work with providers deriving from <see cref="ExtendedProviderBase"/> and
  ///   having a constructor with the following signature: <c>public ctor (<see cref="string"/>, <see cref="NameValueCollection"/>)</c>.
  /// </remarks>
  public abstract class ProviderHelperBase<TProvider> : ProviderHelperBase where TProvider: class
  {
    private readonly ExtendedConfigurationSection _configurationSection;
    private readonly DoubleCheckedLockingContainer<TProvider> _provider;
    private readonly DoubleCheckedLockingContainer<ProviderCollection<TProvider>> _providers;
    private ConfigurationProperty _providerSettingsProperty;
    private ConfigurationProperty _defaultProviderNameProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderHelperBase{TProvider}"/> class. 
    /// </summary>
    /// <param name="configurationSection">
    /// The <see cref="System.Configuration.ConfigurationSection"/> holding the <see cref="ProviderSettings"/> 
    /// loaded from the configuration section in the xml file
    /// </param>
    protected ProviderHelperBase (ExtendedConfigurationSection configurationSection)
    {
      ArgumentUtility.CheckNotNull ("configurationSection", configurationSection);

      _configurationSection = configurationSection;
      _provider = new DoubleCheckedLockingContainer<TProvider> (GetProviderFromConfiguration);
      _providers = new DoubleCheckedLockingContainer<ProviderCollection<TProvider>> (GetProvidersFromConfiguration);
    }

    protected abstract ConfigurationProperty CreateDefaultProviderNameProperty ();

    protected abstract ConfigurationProperty CreateProviderSettingsProperty ();

    /// <summary>Initializes properties and adds them to the given <see cref="ConfigurationPropertyCollection"/>.</summary>
    public override void InitializeProperties (ConfigurationPropertyCollection properties)
    {
      ArgumentUtility.CheckNotNull ("properties", properties);

      _providerSettingsProperty = CreateProviderSettingsProperty();
      _defaultProviderNameProperty = CreateDefaultProviderNameProperty();

      properties.Add (_providerSettingsProperty);
      properties.Add (_defaultProviderNameProperty);
    }

    public override void PostDeserialze ()
    {
    }

    /// <summary>Get and set the provider.</summary>
    public TProvider Provider
    {
      get { return _provider.Value; }
      set { _provider.Value = value; }
    }

    public ProviderCollection<TProvider> Providers
    {
      get { return _providers.Value; }
      set { _providers.Value = value; }
    }

    protected ExtendedConfigurationSection ConfigurationSection
    {
      get { return _configurationSection; }
    }

    protected ProviderSettingsCollection ProviderSettings
    {
      get { return (ProviderSettingsCollection) _configurationSection[_providerSettingsProperty]; }
    }

    protected string DefaultProviderName
    {
      get { return (string) _configurationSection[DefaultProviderNameProperty]; }
    }

    protected ConfigurationProperty DefaultProviderNameProperty
    {
      get { return _defaultProviderNameProperty; }
    }

    protected ConfigurationProperty CreateDefaultProviderNameProperty (string name, string defaultValue)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      return new ConfigurationProperty (name, typeof (string), defaultValue, null, new StringValidator (1), ConfigurationPropertyOptions.None);
    }

    protected ConfigurationProperty CreateProviderSettingsProperty (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      return new ConfigurationProperty (name, typeof (ProviderSettingsCollection), null, ConfigurationPropertyOptions.None);
    }

    protected virtual void EnsureWellKownProviders (ProviderCollection collection)
    {
    }

    protected void CheckForDuplicateWellKownProviderName (string wellKnownName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("wellKnownName", wellKnownName);

      if (ProviderSettings[wellKnownName] != null)
      {
        throw CreateConfigurationErrorsException (
            null,
            ProviderSettings[wellKnownName].ElementInformation.Properties["name"],
            "The name of the entry '{0}' identifies a well known provider and cannot be reused for custom providers.",
            wellKnownName);
      }
    }

    protected Type GetTypeWithMatchingVersionNumber (ConfigurationProperty property, string assemblyName, string typeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("assemblyName", assemblyName);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      ArgumentUtility.CheckNotNull ("property", property);

      AssemblyName frameworkAssemblyName = GetType().Assembly.GetName();
      AssemblyName realAssemblyName = new AssemblyName (frameworkAssemblyName.FullName);
      realAssemblyName.Name = assemblyName;

      return GetType (property, realAssemblyName, typeName);
    }

    protected Type GetType (ConfigurationProperty property, AssemblyName assemblyName, string typeName)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      ArgumentUtility.CheckNotNull ("assemblyName", assemblyName);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

      try
      {
        Assembly.Load (assemblyName);
      }
      catch (FileNotFoundException e)
      {
        throw CreateConfigurationErrorsException (
            e,
            _configurationSection.ElementInformation.Properties[property.Name],
            "The current value of property '{0}' requires that the assembly '{1}' is placed within the CLR's probing path for this application.",
            property.Name,
            assemblyName.FullName);
      }

      return TypeUtility.GetType (Assembly.CreateQualifiedName (assemblyName.FullName, typeName), true);
    }

    /// <summary>Initializes a collection of providers of the given type using the supplied settings.</summary>
    /// <param name="providerSettingsCollection">A collection of settings to be passed to the provider upon initialization.</param>
    /// <param name="providerCollection">The collection used to contain the initialized providers after the method returns.</param>
    /// <param name="providerType">The <see cref="Type"/> of the providers to be initialized.</param>
    /// <param name="providerInterfaces">The list of interfaces each provider must implement.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="providerSettingsCollection" /> is null.<para>- or -</para>
    /// <paramref name="providerCollection" /> is null.<para>- or -</para>
    /// <paramref name="providerType"/> is null.
    /// </exception>
    protected void InstantiateProviders (
        ProviderSettingsCollection providerSettingsCollection,
        ProviderCollection providerCollection,
        Type providerType,
        params Type[] providerInterfaces)
    {
      ArgumentUtility.CheckNotNull ("providerSettingsCollection", providerSettingsCollection);
      ArgumentUtility.CheckNotNull ("providerCollection", providerCollection);
      ArgumentUtility.CheckNotNull ("providerType", providerType);

      foreach (ProviderSettings providerSettings in providerSettingsCollection)
        providerCollection.Add (InstantiateProvider (providerSettings, providerType, providerInterfaces));
    }

    /// <summary>Initializes and returns a single provider of the given type using the supplied settings.</summary>
    /// <param name="providerSettings">The settings to be passed to the provider upon initialization.</param>
    /// <param name="providerType">The <see cref="Type"/> of the providers to be initialized.</param>
    /// <param name="providerInterfaces">The list of interfaces each provider must implement.</param>
    /// <returns>A new provider of the given type using the supplied settings.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="providerSettings" /> is null.<para>- or -</para>
    /// <paramref name="providerType"/> is null.
    /// </exception>
    /// <exception cref="ConfigurationErrorsException">
    /// The provider threw an exception while it was being initialized.<para>- or -</para>
    /// An error occurred while attempting to resolve a <see cref="Type"/> instance for the provider specified in <paramref name="providerSettings"/>.
    /// </exception>    
    protected ExtendedProviderBase InstantiateProvider (ProviderSettings providerSettings, Type providerType, params Type[] providerInterfaces)
    {
      ArgumentUtility.CheckNotNull ("providerSettings", providerSettings);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("providerType", providerType, typeof (ExtendedProviderBase));
      ArgumentUtility.CheckNotNullOrItemsNull ("providerInterfaces", providerInterfaces);

      try
      {
        return CreateProvider (
            providerSettings,
            providerType,
            providerInterfaces,
            providerSettings.Name,
            NameValueCollectionUtility.Clone (providerSettings.Parameters));
      }
      catch (ConfigurationException)
      {
        throw;
      }
      catch (Exception e)
      {
        string message;
        if (e is TargetInvocationException)
          message = e.InnerException.Message;
        else
          message = e.Message;

        throw new ConfigurationErrorsException (
            message,
            e,
            providerSettings.ElementInformation.Properties["type"].Source,
            providerSettings.ElementInformation.Properties["type"].LineNumber);
      }
    }

    private static ExtendedProviderBase CreateProvider (
        ProviderSettings providerSettings,
        Type providerType,
        Type[] providerInterfaces,
        string name,
        NameValueCollection collection)
    {
      if (string.IsNullOrEmpty (providerSettings.Type))
        throw new ArgumentException ("Type name must be specified for this provider.");

      Type actualType = TypeUtility.GetType (providerSettings.Type, true);

      if (!providerType.IsAssignableFrom (actualType))
        throw new ArgumentException (string.Format ("Provider must implement the class '{0}'.", providerType.FullName));

      foreach (Type interfaceType in providerInterfaces)
      {
        if (!interfaceType.IsAssignableFrom (actualType))
          throw new ArgumentException (string.Format ("Provider must implement the interface '{0}'.", interfaceType.FullName));
      }

      return (ExtendedProviderBase) Activator.CreateInstance (actualType, new object[] {name, collection});
    }

    private TProvider GetProviderFromConfiguration ()
    {
      if (DefaultProviderName == null)
        return null;

      if (Providers[DefaultProviderName] == null)
      {
        throw CreateConfigurationErrorsException (
            null,
            _configurationSection.ElementInformation.Properties[DefaultProviderNameProperty.Name],
            "The provider '{0}' specified for the {1} does not exist in the providers collection.",
            DefaultProviderName,
            DefaultProviderNameProperty.Name);
      }

      return (TProvider) (object) Providers[DefaultProviderName];
    }

    private ProviderCollection<TProvider> GetProvidersFromConfiguration ()
    {
      ProviderCollection<TProvider> collection = new ProviderCollection<TProvider>();
      EnsureWellKownProviders (collection);
      InstantiateProviders (ProviderSettings, collection, typeof (ExtendedProviderBase), typeof (TProvider));
      collection.SetReadOnly();
      
      return collection;
    }

    private ConfigurationErrorsException CreateConfigurationErrorsException (
        FileNotFoundException e, PropertyInformation propertyInformation, string message, params object[] args)
    {
      return new ConfigurationErrorsException (string.Format (message, args), e, propertyInformation.Source, propertyInformation.LineNumber);
    }
  }
}
