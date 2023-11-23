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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// Creates <see cref="IRdbmsStoragePropertyDefinition"/> instances for relation properties.
  /// </summary>
  public class RelationStoragePropertyDefinitionFactory : IRelationStoragePropertyDefinitionFactory
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly bool _forceClassIDColumnInForeignKeyProperties;
    private readonly IStorageNameProvider _storageNameProvider;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;
    private readonly IStorageSettings _storageSettings;

    public RelationStoragePropertyDefinitionFactory (
        StorageProviderDefinition storageProviderDefinition,
        bool forceClassIDColumnInForeignKeyProperties,
        IStorageNameProvider storageNameProvider,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageSettings storageSettings)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull("storageSettings", storageSettings);

      _storageProviderDefinition = storageProviderDefinition;
      _forceClassIDColumnInForeignKeyProperties = forceClassIDColumnInForeignKeyProperties;
      _storageNameProvider = storageNameProvider;
      _storageTypeInformationProvider = storageTypeInformationProvider;
      _storageSettings = storageSettings;
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public bool ForceClassIDColumnInForeignKeyProperties
    {
      get { return _forceClassIDColumnInForeignKeyProperties; }
    }

    public IStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public IStorageTypeInformationProvider StorageTypeInformationProvider
    {
      get { return _storageTypeInformationProvider; }
    }

    public IStorageSettings StorageSettings
    {
      get { return _storageSettings; }
    }

    public IRdbmsStoragePropertyDefinition CreateStoragePropertyDefinition (RelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition();
      var relationColumnName = _storageNameProvider.GetRelationColumnName(relationEndPointDefinition);
      var relationClassIDColumnName = _storageNameProvider.GetRelationClassIDColumnName(relationEndPointDefinition);
      return CreateStoragePropertyDefinition(oppositeEndPointDefinition.ClassDefinition, relationColumnName, relationClassIDColumnName);
    }

    public virtual IRdbmsStoragePropertyDefinition CreateStoragePropertyDefinition (
        ClassDefinition relatedClassDefinition,
        string relationColumnName,
        string relationClassIDColumnName)
    {
      ArgumentUtility.CheckNotNull("relatedClassDefinition", relatedClassDefinition);
      ArgumentUtility.CheckNotNullOrEmpty("relationColumnName", relationColumnName);
      ArgumentUtility.CheckNotNullOrEmpty("relationClassIDColumnName", relationClassIDColumnName);

      var relatedStorageProviderDefinition = _storageSettings.GetStorageProviderDefinition(relatedClassDefinition);

      if (_storageProviderDefinition != relatedStorageProviderDefinition)
        return CreateCrossProviderRelationStoragePropertyDefinition(relationColumnName);
      else
        return CreateSameProviderRelationStoragePropertyDefinition(relatedClassDefinition, relationColumnName, relationClassIDColumnName);
    }

    private IRdbmsStoragePropertyDefinition CreateCrossProviderRelationStoragePropertyDefinition (string relationColumnName)
    {
      var storageTypeInfo = _storageTypeInformationProvider.GetStorageTypeForSerializedObjectID(true);
      var columnDefinition = new ColumnDefinition(relationColumnName, storageTypeInfo, false);
      return new SerializedObjectIDStoragePropertyDefinition(new SimpleStoragePropertyDefinition(typeof(ObjectID), columnDefinition));
    }

    private IRdbmsStoragePropertyDefinition CreateSameProviderRelationStoragePropertyDefinition (
        ClassDefinition relatedClassDefinition,
        string relationColumnName,
        string relationClassIDColumnName)
    {
      // Relation properties are always nullable within the same storage provider
      var storageTypeInfo = _storageTypeInformationProvider.GetStorageTypeForID(true);
      var valueColumnDefinition = new ColumnDefinition(relationColumnName, storageTypeInfo, false);

      if (relatedClassDefinition.IsPartOfInheritanceHierarchy || _forceClassIDColumnInForeignKeyProperties)
      {
        var storageTypeForClassID = _storageTypeInformationProvider.GetStorageTypeForClassID(true);
        var classIDColumnDefinition = new ColumnDefinition(relationClassIDColumnName, storageTypeForClassID, false);
        return new ObjectIDStoragePropertyDefinition(
            new SimpleStoragePropertyDefinition(typeof(object), valueColumnDefinition),
            new SimpleStoragePropertyDefinition(typeof(string), classIDColumnDefinition));
      }
      else
      {
        return new ObjectIDWithoutClassIDStoragePropertyDefinition(
            new SimpleStoragePropertyDefinition(typeof(object), valueColumnDefinition),
            relatedClassDefinition);
      }
    }
  }
}
