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
  public class TypeNameDefinition
  {
    public string? SchemaName { get; }
    public string TypeName { get; }

    public TypeNameDefinition (string? schemaName, string typeName)
    {
      ArgumentUtility.CheckNotEmpty(nameof(schemaName), schemaName);
      ArgumentUtility.CheckNotNullOrEmpty(nameof(typeName), typeName);

      SchemaName = schemaName;
      TypeName = typeName;
    }

    public override bool Equals (object? obj)
    {
      if (ReferenceEquals(obj, null))
        return false;

      if (obj.GetType() != GetType())
        return false;

      var other = (TypeNameDefinition)obj;
      return other.SchemaName == SchemaName && other.TypeName == TypeName;
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode(SchemaName, TypeName);
    }
  }
}
