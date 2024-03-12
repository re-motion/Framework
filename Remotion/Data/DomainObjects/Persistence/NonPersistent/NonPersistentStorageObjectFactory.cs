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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.NonPersistent
{
  /// <summary>
  /// The <see cref="NonPersistentStorageObjectFactory"/> is responsible to create storage provider instances that operate only within the
  /// <see cref="ClientTransaction"/>'s memory.
  /// </summary>
  public class NonPersistentStorageObjectFactory : INonPersistentStorageObjectFactory
  {
    public NonPersistentStorageObjectFactory ()
    {
    }

    public StorageProvider CreateStorageProvider (
        StorageProviderDefinition storageProviderDefinition,
        IPersistenceExtension persistenceExtension)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);

      return new NonPersistentProvider(storageProviderDefinition, persistenceExtension);
    }

    public StorageProvider CreateReadOnlyStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("persistenceExtension", persistenceExtension);

      return new NonPersistentProvider(storageProviderDefinition, persistenceExtension);
    }

    public IPersistenceModelLoader CreatePersistenceModelLoader (
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      return new NonPersistentPersistenceModelLoader(storageProviderDefinition);
    }

    public IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
        StorageProviderDefinition storageProviderDefinition,
        IMethodCallTransformerProvider methodCallTransformerProvider,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
        IMappingConfiguration mappingConfiguration)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);
      ArgumentUtility.CheckNotNull("methodCallTransformerProvider", methodCallTransformerProvider);
      ArgumentUtility.CheckNotNull("mappingConfiguration", mappingConfiguration);

      throw new NotSupportedException("Non-persistent DomainObjects do not support querying.");
    }
  }
}
