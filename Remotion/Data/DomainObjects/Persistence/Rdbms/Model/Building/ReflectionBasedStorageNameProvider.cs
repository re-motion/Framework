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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="ReflectionBasedStorageNameProvider"/> provides methods to obtain names for RDBMS items (tables, columns, ...) using default names for
  /// system items ("ID", "ClassID", "Timestamp") and custom attributes (<see cref="DBTableAttribute"/>, <see cref="DBColumnAttribute"/>) for 
  /// user-defined names.
  /// </summary>
  public class ReflectionBasedStorageNameProvider : IStorageNameProvider
  {
    public string GetIDColumnName ()
    {
      return "ID";
    }

    public string GetClassIDColumnName ()
    {
      return "ClassID";
    }

    public string GetTimestampColumnName ()
    {
      return "Timestamp";
    }

    public EntityNameDefinition? GetTableName (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      var tableAttribute = AttributeUtility.GetCustomAttribute<DBTableAttribute>(classDefinition.Type, false);
      if (tableAttribute == null)
        return null;

      return new EntityNameDefinition(null, String.IsNullOrEmpty(tableAttribute.Name) ? classDefinition.ID : tableAttribute.Name);
    }

    public EntityNameDefinition GetViewName (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      return new EntityNameDefinition(null, classDefinition.ID + "View");
    }

    public string GetColumnName (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      var name = GetColumnNameFromAttribute(propertyDefinition);
      if (name != null)
        return name;

      return propertyDefinition.PropertyInfo.Name;
    }

    public string GetRelationColumnName (RelationEndPointDefinition relationEndPointDefinition)
    {
      var name = GetColumnNameFromAttribute(relationEndPointDefinition.PropertyDefinition);
      if (name != null)
        return name;

      return relationEndPointDefinition.PropertyInfo.Name + "ID";
    }

    public string GetRelationClassIDColumnName (RelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      return GetRelationColumnName(relationEndPointDefinition) + "ClassID";
    }

    public string GetPrimaryKeyConstraintName (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      var entityNameDefinition = GetTableName(classDefinition);
      if (entityNameDefinition == null)
      {
        throw new MappingException(
            string.Format("Class '{0}' cannot not define a primary key constraint because no table name has been defined.", classDefinition.ID));
      }

      var tableName = entityNameDefinition.EntityName;

      return String.Format("PK_{0}", tableName);
    }

    public string GetForeignKeyConstraintName (ClassDefinition classDefinition, IEnumerable<ColumnDefinition> foreignKeyColumns)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull("foreignKeyColumns", foreignKeyColumns);

      var entityNameDefinition = GetTableName(classDefinition);
      if (entityNameDefinition == null)
      {
        throw new MappingException(
            string.Format("Class '{0}' cannot not define a foreign key constraint because no table name has been defined.", classDefinition.ID));
      }

      var tableName = entityNameDefinition.EntityName;

      return String.Format("FK_{0}_{1}", tableName, String.Join((string)"_", (IEnumerable<string>)foreignKeyColumns.Select(cd => cd.Name)));
    }

    private string? GetColumnNameFromAttribute (PropertyDefinition propertyDefinition)
    {
      var attribute = propertyDefinition.PropertyInfo.GetCustomAttribute<IStorageSpecificIdentifierAttribute>(false);
      return attribute != null ? attribute.Identifier : null;
    }
  }
}
