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
using System.Xml.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport
{
  /// <summary>
  /// Default implementation of the <see cref="IPropertySerializer"/> interface.
  /// </summary>
  public class PropertySerializer : IPropertySerializer
  {
    private readonly IColumnSerializer _columnSerializer;

    public PropertySerializer (IColumnSerializer columnSerializer)
    {
      ArgumentUtility.CheckNotNull ("columnSerializer", columnSerializer);

      _columnSerializer = columnSerializer;
    }

    public XElement Serialize (PropertyDefinition propertyDefinition, IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);

      var propertyType = GetPropertyType (propertyDefinition);

      return new XElement (
          Constants.Namespace + "property",
          new XAttribute ("name", propertyDefinition.PropertyName),
          new XAttribute ("displayName", propertyDefinition.PropertyInfo.Name),
          new XAttribute ("type", GetTypeName (propertyType)),
          new XAttribute ("isNullable", propertyDefinition.IsNullable),
          GetMaxLenghtAttribute (propertyDefinition),
          _columnSerializer.Serialize (propertyDefinition, persistenceModelProvider)
          );
    }

    private XAttribute GetMaxLenghtAttribute (PropertyDefinition propertyDefinition)
    {
      return propertyDefinition.MaxLength.HasValue ? new XAttribute ("maxLength", propertyDefinition.MaxLength.Value) : null;
    }

    private string GetTypeName (Type propertyType)
    {
      if (propertyType.Assembly == typeof (int).Assembly)
        return propertyType.FullName;

      return TypeUtility.GetPartialAssemblyQualifiedName (propertyType);
    }

    private Type GetPropertyType (PropertyDefinition propertyDefinition)
    {
      var propertyType = propertyDefinition.IsObjectID ? propertyDefinition.PropertyInfo.PropertyType : propertyDefinition.PropertyType;

      if (NullableTypeUtility.IsNullableType (propertyType))
        propertyType = NullableTypeUtility.GetBasicType (propertyType);
      return propertyType;
    }
  }
}