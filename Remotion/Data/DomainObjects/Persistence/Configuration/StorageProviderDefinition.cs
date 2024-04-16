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
using System.Linq;
using CommonServiceLocator;
using Remotion.Configuration;
using Remotion.Mixins;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Defines the configuration for a specific <see cref="IReadOnlyStorageProvider"/> or <see cref="IStorageProvider"/>. Subclasses of <see cref="StorageProviderDefinition"/> can be 
  /// instantiated from a config file entry.
  /// </summary>
  public abstract class StorageProviderDefinition: ExtendedProviderBase
  {
    private readonly IStorageObjectFactory _factory;

    protected StorageProviderDefinition (string name, NameValueCollection config)
        : base(name, config)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);
      ArgumentUtility.CheckNotNull("config", config);

      var factoryTypeName = GetAndRemoveNonEmptyStringAttribute(config, "factoryType", name, required: true)!;
      var configuredFactoryType = TypeUtility.GetType(factoryTypeName, throwOnError: true)!;
      _factory = CreateStorageObjectFactory(configuredFactoryType);
    }

    protected StorageProviderDefinition (string name, IStorageObjectFactory factory)
        : base(name, new NameValueCollection())
    {
      ArgumentUtility.CheckNotNull("factory", factory);

      _factory = factory;
    }

    public abstract bool IsIdentityTypeSupported (Type identityType);

    public void CheckIdentityType (Type identityType)
    {
      if (!IsIdentityTypeSupported(identityType))
        throw new IdentityTypeNotSupportedException(GetType(), identityType);
    }

    public IStorageObjectFactory Factory
    {
      get { return _factory; }
    }

    public override string ToString ()
    {
      return string.Format("{0}: '{1}'", GetType().Name, Name);
    }

    private IStorageObjectFactory CreateStorageObjectFactory (Type configuredFactoryType)
    {
      try
      {
        var registeredService = (IStorageObjectFactory?)SafeServiceLocator.Current.GetService(configuredFactoryType);
        if (registeredService != null)
          return registeredService;
      }
      catch (ActivationException ex)
      {
        var message = string.Format(
            "The factory type '{0}' specified in the configuration of the '{1}' StorageProvider definition cannot be resolved: {2}",
            configuredFactoryType,
            Name,
            ex.Message);
        throw new ConfigurationErrorsException(message, ex);
      }

      if (configuredFactoryType.IsAbstract)
      {
        var message = string.Format(
            "The factory type '{0}' specified in the configuration of the '{1}' StorageProvider definition cannot be instantiated because it is "
            + "abstract. Either register an implementation of '{2}' in the configured service locator, or specify a non-abstract type.",
            configuredFactoryType,
            Name,
            configuredFactoryType.Name);
        throw new ConfigurationErrorsException(message);
      }

      try
      {
        return (IStorageObjectFactory)ObjectFactory.Create(configuredFactoryType);
      }
      catch (Exception ex)
      {
        var message = string.Format(
            "The factory type '{0}' specified in the configuration of the '{1}' StorageProvider definition cannot be instantiated: {2}",
            configuredFactoryType,
            Name,
            ex.Message);
        throw new ConfigurationErrorsException(message, ex);
      }
    }
  }
}
