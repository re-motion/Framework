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
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// The <see cref="PersistenceModelLoader"/> applied the persistence model to a class hierarchy.
  /// </summary>
  public class PersistenceModelLoader : IPersistenceModelLoader
  {
    private readonly IStorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    public PersistenceModelLoader (IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      _storageProviderDefinitionFinder = storageProviderDefinitionFinder;
    }

    public void ApplyPersistenceModelToHierarchy (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      var persistenceModelLoader = GetProviderSpecificPersistenceModelLoader(typeDefinition);
      persistenceModelLoader.ApplyPersistenceModelToHierarchy(typeDefinition);
    }

    public IPersistenceMappingValidator CreatePersistenceMappingValidator (TypeDefinition typeDefinition)
    {
      var persistenceModelLoader = GetProviderSpecificPersistenceModelLoader(typeDefinition);
      return persistenceModelLoader.CreatePersistenceMappingValidator(typeDefinition);
    }

    private IPersistenceModelLoader GetProviderSpecificPersistenceModelLoader (TypeDefinition typeDefinition)
    {
      var storageProviderDefinition = _storageProviderDefinitionFinder.GetStorageProviderDefinition(typeDefinition, null);
      return storageProviderDefinition.Factory.CreatePersistenceModelLoader(storageProviderDefinition, _storageProviderDefinitionFinder);
    }
  }
}
