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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  public static class TableDefinitionObjectMother
  {
    public static TableDefinition Create (StorageProviderDefinition storageProviderDefinition)
    {
      return Create (storageProviderDefinition, new EntityNameDefinition ("TestSchema", "TestTable"));
    }

    public static TableDefinition Create (StorageProviderDefinition storageProviderDefinition, EntityNameDefinition tableName)
    {
      return Create (
          storageProviderDefinition,
          tableName,
          null);
    }

    public static TableDefinition Create (
        StorageProviderDefinition storageProviderDefinition, EntityNameDefinition tableName, EntityNameDefinition viewName)
    {
      return Create (
          storageProviderDefinition,
          tableName,
          viewName,
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty);
    }

    public static TableDefinition Create (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition tableName,
        EntityNameDefinition viewName,
        IEnumerable<ITableConstraintDefinition> constraints)
    {
      return Create (
          storageProviderDefinition,
          tableName,
          viewName,
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new IRdbmsStoragePropertyDefinition[0],
          constraints);
    }

    public static TableDefinition Create (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition tableName,
        EntityNameDefinition viewName,
        ObjectIDStoragePropertyDefinition objectIDPropertyDefinition,
        SimpleStoragePropertyDefinition timestampPropertyDefinition,
        params IRdbmsStoragePropertyDefinition[] dataPropertyDefinitions)
    {
      return Create (
          storageProviderDefinition,
          tableName,
          viewName,
          objectIDPropertyDefinition,
          timestampPropertyDefinition,
          dataPropertyDefinitions,
          new ITableConstraintDefinition[0]);
    }

    public static TableDefinition Create (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition tableName,
        EntityNameDefinition viewName,
        ObjectIDStoragePropertyDefinition objectIDPropertyDefinition,
        SimpleStoragePropertyDefinition timestampPropertyDefinition,
        IEnumerable<IRdbmsStoragePropertyDefinition> dataPropertyDefinitions,
        IEnumerable<ITableConstraintDefinition> tableConstraintDefinitions)
    {
      return new TableDefinition (
                storageProviderDefinition,
                tableName,
                viewName,
                objectIDPropertyDefinition,
                timestampPropertyDefinition,
                dataPropertyDefinitions,
                tableConstraintDefinitions,
                new IIndexDefinition[0],
                new EntityNameDefinition[0]);
    }

    public static TableDefinition CreateWithIndexes (StorageProviderDefinition storageProviderDefinition, IEnumerable<IIndexDefinition> indexDefinitions)
    {
      return new TableDefinition (
          storageProviderDefinition,
          new EntityNameDefinition ("TestSchema", "TestTable"),
          null,
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new IRdbmsStoragePropertyDefinition[0],
          new ITableConstraintDefinition[0],
          indexDefinitions,
          new EntityNameDefinition[0]);
    }

    public static TableDefinition CreateWithSynonyms (StorageProviderDefinition storageProviderDefinition, IEnumerable<EntityNameDefinition> synonyms)
    {
      return new TableDefinition (
          storageProviderDefinition,
          new EntityNameDefinition ("TestSchema", "TestTable"),
          null,
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new IRdbmsStoragePropertyDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          synonyms);
    }

    public static TableDefinition Create (StorageProviderDefinition storageProviderDefinition, IRdbmsStoragePropertyDefinition[] dataPropertyDefinitions)
    {
      return Create (
          storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          null,
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          dataPropertyDefinitions);
    }
  }
}