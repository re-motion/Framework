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
using JetBrains.Annotations;
using Remotion.Mixins;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Resolves or creates an <see cref="IStorageObjectFactory"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [UsedImplicitly]
  [ImplementationFor(typeof(IStorageObjectFactoryFactory), Lifetime = LifetimeKind.Singleton)]
  public sealed class StorageObjectFactoryFactory : IStorageObjectFactoryFactory
  {
    public StorageObjectFactoryFactory ()
    {
    }

    /// <summary>
    /// Resolves or creates an <see cref="IStorageObjectFactory"/> object based on the supplied <see cref="Type"/>.
    /// If the <see cref="IStorageObjectFactory"/> of the requested type cannot be resolved by the service locator, a
    /// new instance is created.
    /// </summary>
    public IStorageObjectFactory Create (Type storageObjectFactoryType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("storageObjectFactoryType", storageObjectFactoryType, typeof(IStorageObjectFactory));

      try
      {
        var registeredService = (IStorageObjectFactory?)SafeServiceLocator.Current.GetService(storageObjectFactoryType);
        if (registeredService != null)
          return registeredService;
      }
      catch (ActivationException ex)
      {
        var message = string.Format(
            "The factory type '{0}' cannot be resolved: {1}",
            storageObjectFactoryType,
            ex.Message);
        throw new ConfigurationException(message, ex);
      }

      if (storageObjectFactoryType.IsAbstract)
      {
        var message = string.Format(
            "The factory type '{0}' cannot be instantiated because it is abstract. "
            + "Either register an implementation of '{1}' in the configured service locator, or specify a non-abstract type.",
            storageObjectFactoryType,
            storageObjectFactoryType.Name);
        throw new ConfigurationException(message);
      }

      if (storageObjectFactoryType.GetConstructor(Type.EmptyTypes) == null)
      {
        throw new ConfigurationException(
            $"The factory type '{storageObjectFactoryType}' does not contain a default constructor required for instantiation via the StorageObjectFactoryFactory.\n"
            + "Register an instance of the factory type with the service locator instead:\n\n"
            + "var serviceLocator = DefaultServiceLocator.Create();\n"
            + "serviceLocator.RegisterSingle<FactoryType>(() => new FactoryType());\n"
            + "var serviceLocatorScope = new ServiceLocatorScope(serviceLocator);");
      }

      try
      {

        return (IStorageObjectFactory)ObjectFactory.Create(storageObjectFactoryType);
      }
      catch (Exception ex)
      {
        var message = string.Format(
            "The factory type '{0}' cannot be instantiated: {1}",
            storageObjectFactoryType,
            ex.Message);
        throw new ConfigurationException(message, ex);
      }
    }
  }
}
