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
using Remotion.Data.DomainObjects.Persistence.Configuration;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Defines an API for resolving <see cref="IReadOnlyStorageProvider"/>s.
  /// </summary>
  /// <remarks>
  /// The instantiated storage providers will be disposed when the <see cref="IReadOnlyStorageProviderManager" /> is disposed.
  /// </remarks>
  public interface IReadOnlyStorageProviderManager : IDisposable
  {
    /// <summary>
    /// Resolves an <see cref="IReadOnlyStorageProvider"/> based on the supplied <paramref name="providerDefinition"/>.
    /// </summary>
    /// <exception cref="PersistenceException">If no <see cref="IStorageProvider"/> could be created based on the <paramref name="providerDefinition"/>.</exception>
    IReadOnlyStorageProvider GetMandatory (StorageProviderDefinition providerDefinition);
  }
}
