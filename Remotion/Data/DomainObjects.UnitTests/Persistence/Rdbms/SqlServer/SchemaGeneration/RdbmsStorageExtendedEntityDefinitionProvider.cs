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
using System.ComponentModel;
using System.Data;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class RdbmsStorageExtendedEntityDefinitionProvider : IRdbmsStorageEntityDefinitionProvider
  {
    public IEnumerable<IRdbmsStorageEntityDefinition> GetEntityDefinitions (IEnumerable<TypeDefinition> typeDefinitions)
    {
      var entityDefinitions = new RdbmsStorageEntityDefinitionProvider().GetEntityDefinitions(typeDefinitions).ToList();

      var tableDefinitions = entityDefinitions.OfType<TableDefinition>().ToList();
      if (tableDefinitions.Count() > 0)
      {
        var firstTableDefinition = tableDefinitions[0];
        var newTableDefinition = new TableDefinition(
            firstTableDefinition.StorageProviderDefinition,
            firstTableDefinition.TableName,
            new EntityNameDefinition(firstTableDefinition.ViewName.SchemaName, "NewViewName"),
            firstTableDefinition.ObjectIDProperty,
            firstTableDefinition.TimestampProperty,
            firstTableDefinition.DataProperties,
            firstTableDefinition.Constraints,
            new IIndexDefinition[0],
            new EntityNameDefinition[0]);
        entityDefinitions.Remove(firstTableDefinition);
        entityDefinitions.Add(newTableDefinition);

        entityDefinitions.Add(CreateNewFilterViewDefinition(newTableDefinition));
        entityDefinitions.Add(CreateNewTableDefinitionWithIndexes(newTableDefinition.StorageProviderDefinition));
        entityDefinitions.Add(CreateNewTableDefinitionWithNonClusteredPrimaryKey(newTableDefinition.StorageProviderDefinition));
      }

      return entityDefinitions;
    }

    private FilterViewDefinition CreateNewFilterViewDefinition (TableDefinition tableDefinition)
    {
      return new FilterViewDefinition(
          tableDefinition.StorageProviderDefinition,
          new EntityNameDefinition(tableDefinition.TableName.SchemaName, "AddedView"),
          tableDefinition,
          new[] { "ClassID" },
          tableDefinition.ObjectIDProperty,
          tableDefinition.TimestampProperty,
          tableDefinition.DataProperties,
          new IIndexDefinition[0],
          new[] { new EntityNameDefinition(tableDefinition.ViewName.SchemaName, "AddedViewSynonym") });
    }

    private TableDefinition CreateNewTableDefinitionWithIndexes (StorageProviderDefinition storageProviderDefinition)
    {
      var storageProperty1 =
          new SimpleStoragePropertyDefinition(
              typeof(Guid),
              new ColumnDefinition(
                  "ID",
                  new StorageTypeInformation(typeof(Guid), "uniqueidentifier", DbType.Guid, false, null, typeof(Guid), new GuidConverter()),
                  true));
      var storageProperty2 =
          new SimpleStoragePropertyDefinition(
              typeof(string),
              new ColumnDefinition(
                  "FirstName",
                  new StorageTypeInformation(typeof(string), "varchar(100)", DbType.String, false, 100, typeof(string), new StringConverter()),
                  false));
      var storageProperty3 =
          new SimpleStoragePropertyDefinition(
              typeof(string),
              new ColumnDefinition(
                  "LastName",
                  new StorageTypeInformation(typeof(string), "varchar(100)", DbType.String, false, 100, typeof(string), new StringConverter()),
                  false));
      var storageProperty4 =
          new SimpleStoragePropertyDefinition(
              typeof(string),
              new ColumnDefinition(
                  "XmlColumn1",
                  new StorageTypeInformation(typeof(string), "xml", DbType.Xml, false, null, typeof(string), new StringConverter()),
                  false));

      var tableName = new EntityNameDefinition(null, "IndexTestTable");
      var viewName = new EntityNameDefinition(null, "IndexTestView");

      var nonClusteredUniqueIndex = new SqlIndexDefinition(
          "IDX_NonClusteredUniqueIndex", new[] { new SqlIndexedColumnDefinition(storageProperty1.ColumnDefinition) }, null, false, true, true, false);
      var nonClusteredNonUniqueIndex = new SqlIndexDefinition(
          "IDX_NonClusteredNonUniqueIndex",
          new[]
          { new SqlIndexedColumnDefinition(storageProperty2.ColumnDefinition), new SqlIndexedColumnDefinition(storageProperty3.ColumnDefinition) },
          new[] { storageProperty1.ColumnDefinition },
          false,
          false,
          false,
          false);
      var indexWithOptionsSet = new SqlIndexDefinition(
          "IDX_IndexWithSeveralOptions",
          new[] { new SqlIndexedColumnDefinition(storageProperty2.ColumnDefinition, IndexOrder.Desc) },
          null,
          false,
          true,
          true,
          false,
          true,
          5,
          true,
          true,
          false,
          true,
          true,
          2);
      var primaryXmlIndex = new SqlPrimaryXmlIndexDefinition(
          "IDX_PrimaryXmlIndex", storageProperty4.ColumnDefinition, true, 3, true, true, false, true, true, 2);
      var secondaryXmlIndex1 = new SqlSecondaryXmlIndexDefinition(
          "IDX_SecondaryXmlIndex1",
          storageProperty4.ColumnDefinition,
          "IDX_PrimaryXmlIndex",
          SqlSecondaryXmlIndexKind.Path,
          true,
          null,
          false,
          false,
          false,
          false,
          false);
      var secondaryXmlIndex2 = new SqlSecondaryXmlIndexDefinition(
          "IDX_SecondaryXmlIndex2", storageProperty4.ColumnDefinition, "IDX_PrimaryXmlIndex", SqlSecondaryXmlIndexKind.Value, false, 8, true);
      var secondaryXmlIndex3 = new SqlSecondaryXmlIndexDefinition(
          "IDX_SecondaryXmlIndex3",
          storageProperty4.ColumnDefinition,
          "IDX_PrimaryXmlIndex",
          SqlSecondaryXmlIndexKind.Property,
          null,
          null,
          null,
          true,
          null,
          true,
          false);

      return CreateCustomTable(
          storageProviderDefinition,
          tableName,
          viewName,
          new[] { storageProperty1, storageProperty2, storageProperty3, storageProperty4 },
          new[] { new PrimaryKeyConstraintDefinition("PK_IndexTestTable_ID", true, new[] { storageProperty1.ColumnDefinition }) },
          new IIndexDefinition[]
          {
              nonClusteredUniqueIndex,
              nonClusteredNonUniqueIndex,
              indexWithOptionsSet,
              primaryXmlIndex,
              secondaryXmlIndex1,
              secondaryXmlIndex2,
              secondaryXmlIndex3
          });
    }

    private TableDefinition CreateNewTableDefinitionWithNonClusteredPrimaryKey (StorageProviderDefinition storageProviderDefinition)
    {
      var tableName = new EntityNameDefinition(null, "PKTestTable");
      var viewName = new EntityNameDefinition(null, "PKTestView");

      var column1 = new ColumnDefinition(
          "ID",
          new StorageTypeInformation(typeof(Guid), "uniqueidentifier", DbType.Guid, false, null, typeof(Guid), new GuidConverter()),
          true);
      var column2 = new ColumnDefinition(
          "Name",
          new StorageTypeInformation(typeof(string), "varchar(100)", DbType.String, false, 100, typeof(string), new StringConverter()),
          false);

      var property1 = new SimpleStoragePropertyDefinition(typeof(Guid), column1);
      var property2 = new SimpleStoragePropertyDefinition(typeof(string), column2);

      var nonClusteredUniqueIndex = new SqlIndexDefinition(
          "IDX_ClusteredUniqueIndex", new[] { new SqlIndexedColumnDefinition(column2) }, null, true, true, true, false);

      return CreateCustomTable(
          storageProviderDefinition,
          tableName,
          viewName,
          new[] { property1, property2 },
          new[] { new PrimaryKeyConstraintDefinition("PK_PKTestTable_ID", false, new[] { column1 }) },
          new IIndexDefinition[] { nonClusteredUniqueIndex });
    }

    private TableDefinition CreateCustomTable (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition tableName,
        EntityNameDefinition viewName,
        SimpleStoragePropertyDefinition[] dataProperties,
        PrimaryKeyConstraintDefinition[] constraints,
        IEnumerable<IIndexDefinition> indexes)
    {
      var idColumn = new ColumnDefinition(
          "ObjectID",
          new StorageTypeInformation(typeof(Int32), "integer", DbType.Int32, false, null, typeof(int), new Int32Converter()),
          true);
      var classIDColumn = new ColumnDefinition(
          "ClassID",
          new StorageTypeInformation(typeof(string), "varchar", DbType.String, false, -1, typeof(string), new StringConverter()),
          false);
      var timestampColumn = new ColumnDefinition(
          "Timestamp",
          new StorageTypeInformation(typeof(DateTime), "datetime", DbType.DateTime, true, null, typeof(DateTime), new DateTimeConverter()),
          false);

      var objectIDProperty = new ObjectIDStoragePropertyDefinition(
          new SimpleStoragePropertyDefinition(typeof(object), idColumn),
          new SimpleStoragePropertyDefinition(typeof(string), classIDColumn));
      var timestampProperty = new SimpleStoragePropertyDefinition(typeof(object), timestampColumn);

      return new TableDefinition(
          storageProviderDefinition,
          tableName,
          viewName,
          objectIDProperty,
          timestampProperty,
          dataProperties,
          constraints,
          indexes,
          new EntityNameDefinition[0]);
    }
  }
}
