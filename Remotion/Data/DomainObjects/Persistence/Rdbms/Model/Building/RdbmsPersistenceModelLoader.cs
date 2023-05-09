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
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Validation;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
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
      ArgumentUtility.CheckNotNull("entityDefinitionFactory", entityDefinitionFactory);
      ArgumentUtility.CheckNotNull("dataStoragePropertyDefinitionFactory", dataStoragePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);

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

    public IPersistenceMappingValidator CreatePersistenceMappingValidator (TypeDefinition typeDefinition)
    {
      return new PersistenceMappingValidator(
          new OnlyOneTablePerHierarchyValidationRule(),
          new TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule(),
          new ClassAboveTableIsAbstractValidationRule(),
          new ColumnNamesAreUniqueWithinInheritanceTreeValidationRule(_rdbmsPersistenceModelProvider),
          new PropertyTypeIsSupportedByStorageProviderValidationRule(),
          new RelationPropertyStorageClassMatchesReferencedTypeDefinitionStorageClassValidationRule());
    }

    public void ApplyPersistenceModel (IEnumerable<TypeDefinition> typeDefinitions)
    {
      var typeDefinitionsArray = typeDefinitions.ToArray();

      // Apply the storage properties first
      var classHierarchyRoots = TypeDefinitionHierarchy.GetClassHierarchyRoots(typeDefinitionsArray);
      foreach (var classHierarchyRoot in classHierarchyRoots)
      {
        InlineTypeDefinitionWalker.WalkDescendants(
            classHierarchyRoot,
            EnsureStoragePropertiesCreated,
            interfaceDefinition => throw new InvalidOperationException("Interfaces are not expected.")); // TODO R2I Linq: Add support for interfaces
      }
      foreach (var typeDefinition in typeDefinitionsArray)
        EnsureStoragePropertiesCreated(typeDefinition);

      // Apply the storage entities
      foreach (var classHierarchyRoot in classHierarchyRoots)
      {
        InlineTypeDefinitionWalker.WalkDescendants(
            classHierarchyRoot,
            EnsureStorageEntitiesCreated,
            interfaceDefinition => throw new InvalidOperationException("Interfaces are not expected.")); // TODO R2I Linq: Add support for interfaces
      }
      foreach (var typeDefinition in typeDefinitionsArray)
        EnsureStorageEntitiesCreated(typeDefinition);
    }

    private void EnsureStorageEntitiesCreated (TypeDefinition typeDefinition)
    {
      if (!typeDefinition.HasStorageEntityDefinitionBeenSet)
      {
        var storageEntity = CreateEntityDefinition(typeDefinition);
        typeDefinition.SetStorageEntity(storageEntity);
      }
      else if (!(typeDefinition.StorageEntityDefinition is IRdbmsStorageEntityDefinition))
      {
        throw new InvalidOperationException(
            string.Format(
                "The storage entity definition of type '{0}' does not implement interface '{1}'.",
                typeDefinition.Type.GetFullNameSafe(),
                typeof(IRdbmsStorageEntityDefinition).Name));
      }

      Assertion.DebugIsNotNull(typeDefinition.StorageEntityDefinition, "typeDefinition.StorageEntityDefinition != null");
    }

    private void EnsureStoragePropertiesCreated (TypeDefinition typeDefinition)
    {
      foreach (var propertyDefinition in typeDefinition.MyPropertyDefinitions.Where(pd => pd.StorageClass == StorageClass.Persistent))
      {
        if (!propertyDefinition.HasStoragePropertyDefinitionBeenSet)
        {
          var storagePropertyDefinition = _dataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition(propertyDefinition);
          propertyDefinition.SetStorageProperty(storagePropertyDefinition);
        }
        else if (!(propertyDefinition.StoragePropertyDefinition is IRdbmsStoragePropertyDefinition))
        {
          throw new InvalidOperationException(
            string.Format(
                "The property definition '{0}' of type '{1}' does not implement interface '{2}'.",
                propertyDefinition.PropertyName,
                typeDefinition.Type.GetFullNameSafe(),
                typeof(IRdbmsStoragePropertyDefinition).Name));
        }

        Assertion.DebugIsNotNull(propertyDefinition.StoragePropertyDefinition, "propertyDefinition.StoragePropertyDefinition != null");
      }
    }

    private IStorageEntityDefinition CreateEntityDefinition (TypeDefinition typeDefinition)
    {
      return typeDefinition switch
      {
          ClassDefinition classDefinition => CreateEntityDefinition(classDefinition),
          InterfaceDefinition interfaceDefinition => CreateEntityDefinition(interfaceDefinition),
          _ => throw new InvalidOperationException("Only class and interface definitions are supported.")
      };
    }

    private IStorageEntityDefinition CreateEntityDefinition (ClassDefinition classDefinition)
    {
      if (_storageNameProvider.GetTableName(classDefinition) != null)
        return _entityDefinitionFactory.CreateTableDefinition(classDefinition);

      var baseClasses = classDefinition.BaseClass.CreateSequence(cd => cd.BaseClass);
      if (baseClasses.Any(cd => _storageNameProvider.GetTableName(cd) != null))
        return CreateEntityDefinitionForClassBelowTable(classDefinition);
      else
        return CreateEntityDefinitionForClassAboveTable(classDefinition);
    }

    private IStorageEntityDefinition CreateEntityDefinition (InterfaceDefinition interfaceDefinition)
    {
      var derivedStorageEntityDefinitions =
          (from ClassDefinition implementingClass in interfaceDefinition.ImplementingClasses
           let entityDefinition = GetEntityDefinition(implementingClass)
           where !(entityDefinition is EmptyViewDefinition)
           select entityDefinition).ToList();

      if (!derivedStorageEntityDefinitions.Any())
        return _entityDefinitionFactory.CreateEmptyViewDefinition(interfaceDefinition);
      else
        return _entityDefinitionFactory.CreateUnionViewDefinition(interfaceDefinition, derivedStorageEntityDefinitions);
    }

    private IStorageEntityDefinition CreateEntityDefinitionForClassBelowTable (ClassDefinition classDefinition)
    {
      Assertion.DebugIsNotNull(classDefinition.BaseClass, "classDefinition.BaseClass != null");

      // The following call is potentially recursive (GetEntityDefinition -> EnsureStorageEntitiesCreated -> CreateEntityDefinitionForClassBelowTable), but this is
      // guaranteed to terminate because we know at this point that there is a class in the classDefinition's base hierarchy that will get a 
      // TableDefinition
      var baseStorageEntityDefinition = GetEntityDefinition(classDefinition.BaseClass);

      return _entityDefinitionFactory.CreateFilterViewDefinition(classDefinition, baseStorageEntityDefinition);
    }

    private IStorageEntityDefinition CreateEntityDefinitionForClassAboveTable (ClassDefinition classDefinition)
    {
      var derivedStorageEntityDefinitions =
          (from ClassDefinition derivedClass in classDefinition.DerivedClasses
           let entityDefinition = GetEntityDefinition(derivedClass)
           where !(entityDefinition is EmptyViewDefinition)
           select entityDefinition).ToList();

      if (!derivedStorageEntityDefinitions.Any())
        return _entityDefinitionFactory.CreateEmptyViewDefinition(classDefinition);
      else
        return _entityDefinitionFactory.CreateUnionViewDefinition(classDefinition, derivedStorageEntityDefinitions);
    }

    private IRdbmsStorageEntityDefinition GetEntityDefinition (ClassDefinition classDefinition)
    {
      EnsureStorageEntitiesCreated(classDefinition);

      return _rdbmsPersistenceModelProvider.GetEntityDefinition(classDefinition);
    }
  }
}
