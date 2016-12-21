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
using System.Xml.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport
{
  /// <summary>
  /// Adds support for extensible enums to the <see cref="IEnumSerializer" /> interface.
  /// </summary>
  public class ExtensibleEnumSerializerDecorator : IEnumSerializer
  {
    private readonly IEnumSerializer _enumSerializer;
    private readonly HashSet<Type> _enumTypes = new HashSet<Type>();

    public ExtensibleEnumSerializerDecorator (IEnumSerializer enumSerializer)
    {
      ArgumentUtility.CheckNotNull ("enumSerializer", enumSerializer);
      _enumSerializer = enumSerializer;
    }

    public HashSet<Type> EnumTypes
    {
      get { return _enumTypes; }
    }

    public void CollectPropertyType (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var propertyType = propertyDefinition.PropertyType;
      if (ExtensibleEnumUtility.IsExtensibleEnumType(propertyType))
        _enumTypes.Add (propertyType);
      else
        _enumSerializer.CollectPropertyType (propertyDefinition);
    }

    public IEnumerable<XElement> Serialize ()
    {
      var elements = _enumTypes.Select (
          t => new XElement (
              Constants.Namespace + "enumType",
              new XAttribute ("type", TypeUtility.GetPartialAssemblyQualifiedName (t)),
              GetValues (t))).ToList();

      elements.AddRange (_enumSerializer.Serialize());
      return elements;
    }

    private IEnumerable<XElement> GetValues (Type type)
    {
      return ExtensibleEnumUtility.GetDefinition (type).GetValueInfos().Select (
          info => new XElement (
              Constants.Namespace + "value",
              new XAttribute ("name", info.Value.ValueName),
              new XAttribute ("columnValue", info.Value.ID)));
    }
  }
}