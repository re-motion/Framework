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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// <see cref="IStorageNameProvider"/> defines the API for classes providing names for RDBMS items. There is a default implementation
  /// called <see cref="ReflectionBasedStorageNameProvider"/> that retrieves the item names via reflection and custom attributes. Storage providers with
  /// specific naming constraints (eg., length limits, forbidden characters, etc.) can decorate (or reimplement) this interface to fulfill those
  /// constraints.
  /// </summary>
  public interface IStorageNameProvider
  {
    string GetIDColumnName ();
    string GetClassIDColumnName ();
    string GetTimestampColumnName ();

    EntityNameDefinition? GetTableName (ClassDefinition classDefinition); // TODO R2I Persistence: check usages for required changes
    EntityNameDefinition GetViewName (ClassDefinition classDefinition);
    string GetColumnName (PropertyDefinition propertyDefinition);
    string GetRelationColumnName (RelationEndPointDefinition relationEndPointDefinition);
    string GetRelationClassIDColumnName (RelationEndPointDefinition relationEndPointDefinition);

    string GetPrimaryKeyConstraintName (ClassDefinition classDefinition);
    string GetForeignKeyConstraintName (ClassDefinition classDefinition, IEnumerable<ColumnDefinition> foreignKeyColumns);
  }
}
