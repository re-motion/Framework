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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Defines the name of an entity in a relational database.
  /// </summary>
  public class EntityNameDefinition
  {
    private readonly string _schemaName;
    private readonly string _entityName;

    public EntityNameDefinition (string schemaName, string entityName)
    {
      ArgumentUtility.CheckNotEmpty ("schemaName", schemaName);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);

      _schemaName = schemaName;
      _entityName = entityName;
    }

    public string SchemaName
    {
      get { return _schemaName; }
    }

    public string EntityName
    {
      get { return _entityName; }
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals (obj, null))
        return false;

      if (obj.GetType () != GetType ())
        return false;

      var other = (EntityNameDefinition) obj;
      return other.SchemaName == SchemaName && other.EntityName == EntityName;
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (SchemaName, EntityName);
    }
  }
}