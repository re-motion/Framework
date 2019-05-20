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
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      StorageProviderDefinition = storageProviderDefinition;
    }

    public IPersistenceMappingValidator CreatePersistenceMappingValidator (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return new PersistenceMappingValidator (
          new PropertyStorageClassIsSupportedByStorageProviderValidationRule(),
          new RelationPropertyStorageClassMatchesReferencedClassDefinitionStorageClassValidationRule());
    }

    public void ApplyPersistenceModelToHierarchy (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);


      ClassDefinition[] derivedClasses = classDefinition.GetAllDerivedClasses();
      var allClassDefinitions = new[] { classDefinition }.Concat (derivedClasses);

      // ReSharper disable PossibleMultipleEnumeration - multiple enumeration is okay here.
      EnsureAllStoragePropertiesCreated (allClassDefinitions);
      EnsureAllStorageEntitiesCreated (allClassDefinitions);
      // ReSharper restore PossibleMultipleEnumeration
    }

    private void EnsureAllStorageEntitiesCreated (IEnumerable<ClassDefinition> classDefinitions)
    {
      foreach (var classDefinition in classDefinitions)
        EnsureStorageEntitiesCreated (classDefinition);
    }

    private void EnsureStorageEntitiesCreated (ClassDefinition classDefinition)
    {
      if (classDefinition.StorageEntityDefinition == null)
      {
        var storageEntity = CreateEntityDefinition (classDefinition);
        classDefinition.SetStorageEntity (storageEntity);
      }
      else if (!(classDefinition.StorageEntityDefinition is NonPersistentStorageEntity))
      {
        throw new InvalidOperationException (
            string.Format (
                "The storage entity definition of class '{0}' is not of type '{1}'.",
                classDefinition.ID,
                typeof (NonPersistentStorageEntity).Name));
      }

      Assertion.IsNotNull (classDefinition.StorageEntityDefinition);
    }

    private void EnsureAllStoragePropertiesCreated (IEnumerable<ClassDefinition> classDefinitions)
    {
      foreach (var classDefinition in classDefinitions)
        EnsureStoragePropertiesCreated (classDefinition);
    }

    private void EnsureStoragePropertiesCreated (ClassDefinition classDefinition)
    {
      foreach (var propertyDefinition in classDefinition.MyPropertyDefinitions.Where (pd => pd.StorageClass == StorageClass.Persistent))
      {
        if (propertyDefinition.StoragePropertyDefinition == null)
        {
          propertyDefinition.SetStorageProperty (NonPersistentStorageProperty.Instance);
        }
        else if (!(propertyDefinition.StoragePropertyDefinition is NonPersistentStorageProperty))
        {
          throw new InvalidOperationException (
              string.Format (
                  "The property definition '{0}' of class '{1}' has a storage property type of '{2}' when only '{3}' is supported.",
                  propertyDefinition.PropertyName,
                  classDefinition.ID,
                  propertyDefinition.StoragePropertyDefinition.GetType(),
                  typeof (NonPersistentStorageProperty).Name));
        }

        Assertion.IsNotNull (propertyDefinition.StoragePropertyDefinition);
      }
    }

    private IStorageEntityDefinition CreateEntityDefinition (ClassDefinition classDefinition)
    {
      return new NonPersistentStorageEntity (StorageProviderDefinition);
    }
  }
}