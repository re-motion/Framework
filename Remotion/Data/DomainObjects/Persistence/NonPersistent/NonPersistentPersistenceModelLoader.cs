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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Validation;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.NonPersistent
{
  /// <summary>
  /// The <see cref="NonPersistentPersistenceModelLoader"/> is responsible for stubbing the persistence model for a non-persistent storage provider.
  /// </summary>
  public class NonPersistentPersistenceModelLoader : IPersistenceModelLoader
  {
    public StorageProviderDefinition StorageProviderDefinition { get; }

    public NonPersistentPersistenceModelLoader (StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      StorageProviderDefinition = storageProviderDefinition;
    }

    public IPersistenceMappingValidator CreatePersistenceMappingValidator (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      return new PersistenceMappingValidator(
          new PropertyStorageClassIsSupportedByStorageProviderValidationRule(),
          new RelationPropertyStorageClassMatchesReferencedTypeDefinitionStorageClassValidationRule());
    }

    public void ApplyPersistenceModelToHierarchy (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);


      var allTypeDefinitions = typeDefinition.GetTypeHierarchy();

      // ReSharper disable PossibleMultipleEnumeration - multiple enumeration is okay here.
      EnsureAllStoragePropertiesCreated(allTypeDefinitions);
      EnsureAllStorageEntitiesCreated(allTypeDefinitions);
      // ReSharper restore PossibleMultipleEnumeration
    }

    private void EnsureAllStorageEntitiesCreated (IEnumerable<TypeDefinition> typeDefinitions)
    {
      foreach (var typeDefinition in typeDefinitions)
        EnsureStorageEntitiesCreated(typeDefinition);
    }

    private void EnsureStorageEntitiesCreated (TypeDefinition typeDefinition)
    {
      if (!typeDefinition.HasStorageEntityDefinitionBeenSet)
      {
        var storageEntity = CreateEntityDefinition(typeDefinition);
        typeDefinition.SetStorageEntity(storageEntity);
      }
      else if (!typeDefinition.IsNonPersistent())
      {
        throw new InvalidOperationException(
            string.Format(
                "The storage entity definition of type '{0}' is not of type '{1}'.",
                typeDefinition.Type.GetFullNameSafe(),
                typeof(NonPersistentStorageEntity).Name));
      }

      Assertion.DebugIsNotNull(typeDefinition.StorageEntityDefinition, "typeDefinition.StorageEntityDefinition != null");
      Assertion.DebugAssert(typeDefinition.StorageEntityDefinition is NonPersistentStorageEntity, "classDefinition.StorageEntityDefinition is NonPersistentStorageEntity");
    }

    private void EnsureAllStoragePropertiesCreated (IEnumerable<TypeDefinition> typeDefinitions)
    {
      foreach (var typeDefinition in typeDefinitions)
        EnsureStoragePropertiesCreated(typeDefinition);
    }

    private void EnsureStoragePropertiesCreated (TypeDefinition typeDefinition)
    {
      foreach (var propertyDefinition in typeDefinition.MyPropertyDefinitions.Where(pd => pd.StorageClass == StorageClass.Persistent))
      {
        if (!propertyDefinition.HasStoragePropertyDefinitionBeenSet)
        {
          propertyDefinition.SetStorageProperty(NonPersistentStorageProperty.Instance);
        }
        else if (!(propertyDefinition.StoragePropertyDefinition is NonPersistentStorageProperty))
        {
          throw new InvalidOperationException(
              string.Format(
                  "The property definition '{0}' of type '{1}' has a storage property type of '{2}' when only '{3}' is supported.",
                  propertyDefinition.PropertyName,
                  typeDefinition.Type.GetFullNameSafe(),
                  propertyDefinition.StoragePropertyDefinition.GetType(),
                  typeof(NonPersistentStorageProperty).Name));
        }

        Assertion.DebugIsNotNull(propertyDefinition.StoragePropertyDefinition, "propertyDefinition.StoragePropertyDefinition != null");
      }
    }

    private IStorageEntityDefinition CreateEntityDefinition (TypeDefinition typeDefinition)
    {
      return new NonPersistentStorageEntity(StorageProviderDefinition);
    }
  }
}
