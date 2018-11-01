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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="RdbmsPersistenceModelLoader"/> is responsible to load a persistence model for a relational database.
  /// </summary>
  public class RdbmsPersistenceModelLoader : IPersistenceModelLoader
  {
    private readonly IRdbmsStorageEntityDefinitionFactory _entityDefinitionFactory;
    private readonly IDataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactory;
    private readonly IStorageNameProvider _storageNameProvider;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;

    public RdbmsPersistenceModelLoader (
        IRdbmsStorageEntityDefinitionFactory entityDefinitionFactory,
        IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory,
        IStorageNameProvider storageNameProvider,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("entityDefinitionFactory", entityDefinitionFactory);
      ArgumentUtility.CheckNotNull ("dataStoragePropertyDefinitionFactory", dataStoragePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);

      _entityDefinitionFactory = entityDefinitionFactory;
      _dataStoragePropertyDefinitionFactory = dataStoragePropertyDefinitionFactory;
      _storageNameProvider = storageNameProvider;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
    }

    public IRdbmsStorageEntityDefinitionFactory EntityDefinitionFactory
    {
      get { return _entityDefinitionFactory; }
    }

    public IDataStoragePropertyDefinitionFactory DataStoragePropertyDefinitionFactory
    {
      get { return _dataStoragePropertyDefinitionFactory; }
    }

    public IStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public IRdbmsPersistenceModelProvider RdbmsPersistenceModelProvider
    {
      get { return _rdbmsPersistenceModelProvider; }
    }

    public IPersistenceMappingValidator CreatePersistenceMappingValidator (ClassDefinition classDefinition)
    {
      return new PersistenceMappingValidator (
          new OnlyOneTablePerHierarchyValidationRule(),
          new TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule(),
          new ClassAboveTableIsAbstractValidationRule(),
          new ColumnNamesAreUniqueWithinInheritanceTreeValidationRule(_rdbmsPersistenceModelProvider),
          new PropertyTypeIsSupportedByStorageProviderValidationRule());
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
      else if (!(classDefinition.StorageEntityDefinition is IRdbmsStorageEntityDefinition))
      {
        throw new InvalidOperationException (
            string.Format (
                "The storage entity definition of class '{0}' does not implement interface '{1}'.",
                classDefinition.ID,
                typeof (IRdbmsStorageEntityDefinition).Name));
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
          var storagePropertyDefinition = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);
          propertyDefinition.SetStorageProperty (storagePropertyDefinition);
        }
        else if (!(propertyDefinition.StoragePropertyDefinition is IRdbmsStoragePropertyDefinition))
        {
          throw new InvalidOperationException (
            string.Format (
                "The property definition '{0}' of class '{1}' does not implement interface '{2}'.",
                propertyDefinition.PropertyName,
                classDefinition.ID,
                typeof (IRdbmsStoragePropertyDefinition).Name));
        }
        
        Assertion.IsNotNull (propertyDefinition.StoragePropertyDefinition);
      }
    }

    private IStorageEntityDefinition CreateEntityDefinition (ClassDefinition classDefinition)
    {
      if (_storageNameProvider.GetTableName (classDefinition) != null)
        return _entityDefinitionFactory.CreateTableDefinition (classDefinition);

      var baseClasses = classDefinition.BaseClass.CreateSequence (cd => cd.BaseClass);
      if (baseClasses.Any (cd => _storageNameProvider.GetTableName (cd) != null))
        return CreateEntityDefinitionForClassBelowTable (classDefinition);
      else
        return CreateEntityDefinitionForClassAboveTable (classDefinition);
    }

    private IStorageEntityDefinition CreateEntityDefinitionForClassBelowTable (ClassDefinition classDefinition)
    {
      // The following call is potentially recursive (GetEntityDefinition -> EnsureStorageEntitiesCreated -> CreateEntityDefinitionForClassBelowTable), but this is
      // guaranteed to terminate because we know at this point that there is a class in the classDefinition's base hierarchy that will get a 
      // TableDefinition
      var baseStorageEntityDefinition = GetEntityDefinition (classDefinition.BaseClass);

      return _entityDefinitionFactory.CreateFilterViewDefinition (classDefinition, baseStorageEntityDefinition);
    }

    private IStorageEntityDefinition CreateEntityDefinitionForClassAboveTable (ClassDefinition classDefinition)
    {
      var derivedStorageEntityDefinitions =
          (from ClassDefinition derivedClass in classDefinition.DerivedClasses
           let entityDefinition = GetEntityDefinition (derivedClass)
           where !(entityDefinition is EmptyViewDefinition)
           select entityDefinition).ToList();

      if (!derivedStorageEntityDefinitions.Any())
        return _entityDefinitionFactory.CreateEmptyViewDefinition (classDefinition);
      else

      return _entityDefinitionFactory.CreateUnionViewDefinition (classDefinition, derivedStorageEntityDefinitions);
    }

    private IRdbmsStorageEntityDefinition GetEntityDefinition (ClassDefinition classDefinition)
    {
      EnsureStorageEntitiesCreated (classDefinition);

      return _rdbmsPersistenceModelProvider.GetEntityDefinition(classDefinition);
    }
  }
}