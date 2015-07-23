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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport
{
  /// <summary>
  /// Default implementation of the <see cref="IEnumSerializer"/> interface.
  /// </summary>
  public class EnumSerializer : IEnumSerializer
  {
    private readonly HashSet<Type> _enumTypes = new HashSet<Type>();
 
    public EnumSerializer ()
    {
    }

    public HashSet<Type> EnumTypes
    {
      get { return _enumTypes; }
    }

    public void CollectPropertyType (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var propertyType = propertyDefinition.PropertyType;
      if (NullableTypeUtility.IsNullableType (propertyType))
        propertyType = NullableTypeUtility.GetBasicType (propertyType);

      if (propertyType.IsEnum)
        _enumTypes.Add (propertyType);
    }

    public IEnumerable<XElement> Serialize ()
    {
      return _enumTypes.Select (
          t => new XElement (Constants.Namespace + "enumType",
              new XAttribute ("type", TypeUtility.GetPartialAssemblyQualifiedName (t)),
              GetValues (t)));
    }

    private IEnumerable<XElement> GetValues (Type type)
    {
     return Enum.GetValues (type).Cast<object> ().Select (
          value => new XElement (Constants.Namespace + "value",
              new XAttribute ("name", Enum.GetName (type, value)),
              new XAttribute ("columnValue", Convert.ChangeType (value, Enum.GetUnderlyingType (type)))));
    }
  }
}