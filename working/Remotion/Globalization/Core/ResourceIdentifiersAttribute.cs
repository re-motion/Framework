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

namespace Remotion.Globalization
{

[AttributeUsage (AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public class ResourceIdentifiersAttribute: Attribute
{
  public static string GetResourceIdentifier (Enum enumValue)
  {
    ArgumentUtility.CheckNotNull ("enumValue", enumValue);
    Type type = enumValue.GetType();
    if (type.DeclaringType != null && IsEnumTypeNameSuppressed (type)) // if the enum is a nested type, suppress enum name
      type = type.DeclaringType;
    return type.FullName + "." + enumValue.ToString();

//    string typePath = type.FullName.Substring (0, type.FullName.Length - type.Name.Length);
//    if (typePath.EndsWith ("+"))
//      return typePath.Substring (0, typePath.Length - 1) + "." + enumValue.ToString(); // nested enum type: exclude enum type name
//    else
//      return type.FullName + "." + enumValue.ToString();
  }

  public static ResourceIdentifiersAttribute GetAttribute (Type type)
  {
    object[] attributes = type.GetCustomAttributes (typeof (ResourceIdentifiersAttribute), false);
    if (attributes == null || attributes.Length == 0)
      return null;
    else
      return (ResourceIdentifiersAttribute) attributes[0];
  }

  private static bool IsEnumTypeNameSuppressed (Type type)
  {
    ResourceIdentifiersAttribute attrib = GetAttribute (type);
    if (attrib == null)
      return false;
    else
      return attrib.SuppressTypeName;
  }

  bool _suppressTypeName;

  /// <summary> Initializes a new instance. </summary>
  /// <param name="suppressTypeName"> If true, the name of the enum type is not included in the resource identifier. Default is true. </param>
  public ResourceIdentifiersAttribute (bool suppressTypeName)
  {
    _suppressTypeName = suppressTypeName;
  }

  /// <summary> Initializes a new instance. </summary>
  public ResourceIdentifiersAttribute ()
    : this (true)
  {
  }

  public bool SuppressTypeName
  {
    get { return _suppressTypeName; }
  }
}

}
