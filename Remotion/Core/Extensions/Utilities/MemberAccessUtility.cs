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

namespace Remotion.Utilities
{
  public static class MemberAccessUtility
  {
    public delegate bool CompareValues (object propertyOrFieldValue, object compareToValue);

    public static object GetAttributeArrayMemberValue (
        MemberInfo reflectionObject,
        Type attributeType,
        bool inherit,
        MemberInfo fieldOrProperty,
        MemberInfo comparePropertyOrField,
        object compareToValue,
        CompareValues comparer)
    {
      object[] attributes = reflectionObject.GetCustomAttributes (attributeType, inherit);
      if (attributes == null || attributes.Length == 0)
        return null;
      foreach (Attribute attribute in attributes)
      {
        if (comparer (GetFieldOrPropertyValue (attribute, comparePropertyOrField), compareToValue))
          return GetFieldOrPropertyValue (attribute, fieldOrProperty);
      }
      return null;
    }


    public static object GetAttributeMemberValue (MemberInfo reflectionObject, Type attributeType, bool inherit, MemberInfo fieldOrProperty)
    {
      object[] attributes = reflectionObject.GetCustomAttributes (attributeType, inherit);
      if (attributes == null || attributes.Length == 0)
        return null;
      if (attributes.Length > 1)
        throw new NotSupportedException (String.Format ("Cannot get member value for multiple attributes. Reflection object {0} has {1} instances of attribute {2}", reflectionObject.Name, attributes.Length, attributeType.FullName));
      return GetFieldOrPropertyValue (attributes[0], fieldOrProperty);
    }

    public static MemberInfo GetFieldOrProperty (Type type, string fieldOrPropertyName, BindingFlags bindingFlags, bool throwExceptionIfNotFound)
    {
      MemberInfo member = type.GetField (fieldOrPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (member != null)
        return member;

      member = type.GetProperty (fieldOrPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (member != null)
        return member;

      if (throwExceptionIfNotFound)
        throw new ArgumentException (String.Format ("{0} is not an instance field or property of type {1}.", fieldOrPropertyName, type.FullName), "fieldOrPropertyName");
      return null;
    }


    public static object GetFieldOrPropertyValue (object obj, string fieldOrPropertyName)
    {
      return GetFieldOrPropertyValue (obj, fieldOrPropertyName, BindingFlags.Public);
    }

    public static object GetFieldOrPropertyValue (object obj, string fieldOrPropertyName, BindingFlags bindingFlags)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);
      MemberInfo fieldOrProperty = GetFieldOrProperty (obj.GetType (), fieldOrPropertyName, bindingFlags, true);
      return GetFieldOrPropertyValue (obj, fieldOrProperty);
    }

    public static object GetFieldOrPropertyValue (object obj, MemberInfo fieldOrProperty)
    {
      if (obj == null)
        throw new ArgumentNullException ("obj");
      if (fieldOrProperty == null)
        throw new ArgumentNullException ("fieldOrProperty");

      if (fieldOrProperty is FieldInfo)
        return ((FieldInfo) fieldOrProperty).GetValue (obj);
      else if (fieldOrProperty is PropertyInfo)
        return ((PropertyInfo) fieldOrProperty).GetValue (obj, new object[0]);
      else
        throw new ArgumentException (String.Format ("Argument must be either FieldInfo or PropertyInfo but is {0}.", fieldOrProperty.GetType ().FullName), "fieldOrProperty");
    }


    public static void SetFieldOrPropertyValue (object obj, string fieldOrPropertyName, object value)
    {
      SetFieldOrPropertyValue (obj, fieldOrPropertyName, BindingFlags.Public, value);
    }

    public static void SetFieldOrPropertyValue (object obj, string fieldOrPropertyName, BindingFlags bindingFlags, object value)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);
      MemberInfo fieldOrProperty = GetFieldOrProperty (obj.GetType (), fieldOrPropertyName, bindingFlags, true);
      SetFieldOrPropertyValue (obj, fieldOrProperty, value);
    }

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
