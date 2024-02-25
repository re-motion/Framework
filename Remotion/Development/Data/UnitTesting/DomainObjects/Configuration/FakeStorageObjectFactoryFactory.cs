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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Development.Data.UnitTesting.DomainObjects.Configuration
{
  /// <summary>
  /// Custom implementation of <see cref="IStorageObjectFactoryFactory"/> mainly used for development purposes.
  /// </summary>
  public sealed class FakeStorageObjectFactoryFactory : IStorageObjectFactoryFactory
  {
    /// <summary>
    /// Returns the currently configured <see cref="IStorageObjectFactory"/> object.
    /// </summary>
    public IStorageObjectFactory? StorageObjectFactory { get; private set; }

    public FakeStorageObjectFactoryFactory ()
    {
    }

    /// <summary>
    /// Configures the factory to be resolved in the <see cref="FakeStorageObjectFactoryFactory"/>.<see cref="Create"/> method.
    /// </summary>
    public void SetUp (IStorageObjectFactory factory)
    {
      ArgumentUtility.CheckNotNull("factory", factory);

      StorageObjectFactory = factory;
    }

    /// <summary>
    /// Returns the factory configured in the <see cref="FakeStorageObjectFactoryFactory"/>.<see cref="SetUp"/> method.
    /// </summary>
    /// <param name="storageObjectFactoryType">Must match the type of the configured factory.</param>
    public IStorageObjectFactory Create (Type storageObjectFactoryType)
    {
      ArgumentUtility.CheckNotNull("storageObjectFactoryType", storageObjectFactoryType);

      if (StorageObjectFactory == null)
        throw new InvalidOperationException($"{nameof(FakeStorageObjectFactoryFactory)}.{nameof(SetUp)}(...) must be called before performing the current operation.");

      if (!storageObjectFactoryType.IsInstanceOfType(StorageObjectFactory))
      {
        throw new InvalidOperationException(
            $"This {nameof(FakeStorageObjectFactoryFactory)} is set up to return an instance of type '{StorageObjectFactory.GetType().GetFullNameSafe()}', "
            + $"which is not compatible with the requested type '{storageObjectFactoryType.GetFullNameSafe()}'.");
      }

      return StorageObjectFactory;
    }
  }
}
