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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="ForeignKeyConstraintDefinitionFactory"/> is responsible to create all <see cref="ForeignKeyConstraintDefinition"/>s for a 
  /// <see cref="ClassDefinition"/>.
  /// </summary>
  public class ForeignKeyConstraintDefinitionFactory : IForeignKeyConstraintDefinitionFactory
  {
    private readonly IRdbmsPersistenceModelProvider _persistenceModelProvider;
    private readonly IStorageNameProvider _storageNameProvider;
    private readonly IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;

    public ForeignKeyConstraintDefinitionFactory (
        IStorageNameProvider storageNameProvider,
        IRdbmsPersistenceModelProvider persistenceModelProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider)
    {
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull("persistenceModelProvider", persistenceModelProvider);
      ArgumentUtility.CheckNotNull("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);

      _storageNameProvider = storageNameProvider;
      _persistenceModelProvider = persistenceModelProvider;
      _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
    }

    public IRdbmsPersistenceModelProvider PersistenceModelProvider
    {
      get { return _persistenceModelProvider; }
    }

    public IStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public IInfrastructureStoragePropertyDefinitionProvider InfrastructureStoragePropertyDefinitionProvider
    {
      get { return _infrastructureStoragePropertyDefinitionProvider; }
    }

    public IEnumerable<ForeignKeyConstraintDefinition> CreateForeignKeyConstraints (ClassDefinition classDefinition)
    {
      var allClassDefinitionsInHierarchy = classDefinition
          .CreateSequence(cd => cd.BaseClass)
          .Concat(classDefinition.GetAllDerivedClasses());

      return (from classDefinitionInHierarchy in allClassDefinitionsInHierarchy
              from endPointDefinition in classDefinitionInHierarchy.MyRelationEndPointDefinitions
              where !endPointDefinition.IsVirtual
              let referencedClassDefinition = endPointDefinition.TypeDefinition
                  .GetMandatoryRelationEndPointDefinition(
                      Assertion.IsNotNull(endPointDefinition.PropertyName, "endPointDefinition.PropertyName != null when endPointDefinition.IsVirtual == false"))
                  .GetOppositeTypeDefinition()
              let propertyDefinition = ((RelationEndPointDefinition)endPointDefinition).PropertyDefinition
              where propertyDefinition.StorageClass == StorageClass.Persistent
              let referencingStorageProperty =
                  (IObjectIDStoragePropertyDefinition)_persistenceModelProvider.GetStoragePropertyDefinition(propertyDefinition)
              where referencingStorageProperty.CanCreateForeignKeyConstraint
              let referencedTableName = FindTableName(referencedClassDefinition)
              where referencedTableName != null
              let referencedStoragePropertyDefinition = _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition()
              select referencingStorageProperty.CreateForeignKeyConstraint(
                  referencingColumns => _storageNameProvider.GetForeignKeyConstraintName(classDefinition, referencingColumns),
                  referencedTableName,
                  referencedStoragePropertyDefinition)
             ).ToList();
    }

    private EntityNameDefinition? FindTableName (TypeDefinition typeDefinition)
    {
      if (typeDefinition is not ClassDefinition classDefinition) // TODO R2I Persistence: check if this makes sense after refactoring GetTableName
        return null;

      var tableName = classDefinition
          .CreateSequence(cd => cd.BaseClass)
          .Select(cd => _storageNameProvider.GetTableName(cd))
          .FirstOrDefault(name => name != null);
      return tableName;
    }
  }
}
