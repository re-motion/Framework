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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class EnumerationReflector : IEnumerationReflector
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public EnumerationReflector ()
    {
    }

    // methods and properties

    public Dictionary<Enum, EnumValueInfo> GetValues (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull("type", type);
      if (!type.IsEnum)
        throw new ArgumentException(string.Format("The type '{0}' is not an enumerated type.", type.GetFullNameSafe()), "type");
      ArgumentUtility.CheckNotNull("cache", cache);

      IList values = Enum.GetValues(type);

      Dictionary<Enum, EnumValueInfo> enumValueInfos = new Dictionary<Enum, EnumValueInfo>();
      for (int i = 0; i < values.Count; i++)
      {
        Enum value = (Enum)values[i]!;
        enumValueInfos.Add(value, GetValue(value, cache));
      }

      return enumValueInfos;
    }

    public EnumValueInfo GetValue (Enum value, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull("value", value);
      ArgumentUtility.CheckNotNull("cache", cache);

      EnumValueInfo? info = cache.GetEnumValueInfo(value);
      if (info == null)
      {
        string name = value.ToString();
        info = new EnumValueInfo(TypeUtility.GetPartialAssemblyQualifiedName(value.GetType()), name, Convert.ToInt32(value));
        FieldInfo? fieldInfo = value.GetType().GetField(name, BindingFlags.Static | BindingFlags.Public);
        Assertion.DebugIsNotNull(fieldInfo, "Field '{0}' was not found on type '{1}'.", name, value.GetType());
        PermanentGuidAttribute? attribute = (PermanentGuidAttribute?)Attribute.GetCustomAttribute(fieldInfo, typeof(PermanentGuidAttribute), false);
        if (attribute != null)
          info.ID = attribute.Value.ToString();

        cache.AddEnumValueInfo(value, info);
      }

      return info;
    }
  }

}
