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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Provides all <see cref="IRdbmsStructuredTypeDefinition"/> for which to generate scripts.
  /// </summary>
  public class RdbmsStructuredTypeDefinitionProvider : IRdbmsStructuredTypeDefinitionProvider
  {
    public RdbmsStructuredTypeDefinitionProvider ()
    {
    }

    /// <summary>
    /// Gets all <see cref="IRdbmsStructuredTypeDefinition"/>s for which to generate CREATE TYPE and DROP TYPE scripts.
    /// </summary>
    /// <param name="storageProviderDefinition">The storage provider for which to generate scripts.</param>
    /// <remarks>
    /// In order to influence the returned collection, override or mix the <see cref="IRdbmsStorageObjectFactory.CreateSingleScalarStructuredTypeDefinitionProvider"/> method
    /// on the <paramref name="storageProviderDefinition"/>'s <see cref="RdbmsProviderDefinition.Factory"/>. 
    /// </remarks>
    public IReadOnlyCollection<IRdbmsStructuredTypeDefinition> GetTypeDefinitions (RdbmsProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(storageProviderDefinition), storageProviderDefinition);

      var factory = storageProviderDefinition.Factory;
      var simpleStructuredTypeDefinitionRepository = factory.CreateSingleScalarStructuredTypeDefinitionProvider(storageProviderDefinition);
      return simpleStructuredTypeDefinitionRepository.GetAllStructuredTypeDefinitions().ToArray();
    }
  }
}
