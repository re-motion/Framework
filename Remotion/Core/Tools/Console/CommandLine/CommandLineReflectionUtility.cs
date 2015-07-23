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
using System.Reflection;

namespace Remotion.Tools.Console.CommandLine
{
  public class CommandLineReflectionUtility
  {
    public static void SetFieldOrPropertyValue (object obj, MemberInfo fieldOrProperty, object value)
    {
      if (obj == null)
        throw new ArgumentNullException ("obj");
      if (fieldOrProperty == null)
        throw new ArgumentNullException ("fieldOrProperty");

      if (fieldOrProperty is FieldInfo)
        ((FieldInfo) fieldOrProperty).SetValue (obj, value);
      else if (fieldOrProperty is PropertyInfo)
        ((PropertyInfo) fieldOrProperty).SetValue (obj, value, new object[0]);
      else
        throw new ArgumentException (String.Format ("Argument must be either FieldInfo or PropertyInfo but is {0}.", fieldOrProperty.GetType ().FullName), "fieldOrProperty");
    }

    public static Type GetFieldOrPropertyType (MemberInfo fieldOrProperty)
    {
      if (fieldOrProperty is FieldInfo)
        return ((FieldInfo) fieldOrProperty).FieldType;
      else if (fieldOrProperty is PropertyInfo)
        return ((PropertyInfo) fieldOrProperty).PropertyType;
      else
        throw new ArgumentException ("Argument must be FieldInfo or PropertyInfo.", "fieldOrProperty");
    }
  }
}