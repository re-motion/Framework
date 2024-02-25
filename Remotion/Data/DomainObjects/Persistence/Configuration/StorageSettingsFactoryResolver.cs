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
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Default implementation of the <see cref="IStorageSettingsFactoryResolver"/> interface.
  /// Resolves an <see cref="IStorageSettingsFactory"/> from the <see cref="SafeServiceLocator"/>.
  /// </summary>
  [UsedImplicitly]
  [ImplementationFor(typeof(IStorageSettingsFactoryResolver), Lifetime = LifetimeKind.Singleton)]
  public sealed class StorageSettingsFactoryResolver : IStorageSettingsFactoryResolver
  {
    public StorageSettingsFactoryResolver ()
    {
    }

    public IStorageSettingsFactory Resolve ()
    {
      try
      {
        return SafeServiceLocator.Current.GetInstance<IStorageSettingsFactory>();
      }
      catch (Exception ex)
      {
        throw new ConfigurationException(
            "Could not locate implementation of IStorageSettingsFactory in the IoC container. Example implementation on how to set up a basic implementation:"
            + "Example that registers a default instance of the RdbmsStorageSettingsFactory:\n"
            + "var serviceLocator = DefaultServiceLocator.Create();\n"
            + "serviceLocator.RegisterSingle(() => StorageSettingsFactory.CreateForSqlServer(\"\"connectionString\"\"));\n"
            + "var serviceLocatorScope = new ServiceLocatorScope(serviceLocator);", ex);
      }
    }
  }
}
