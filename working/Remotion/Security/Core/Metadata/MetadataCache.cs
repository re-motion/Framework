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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class MetadataCache
  {
    // types

    // static members

    // member fields

    private Dictionary<Type, SecurableClassInfo> _classes = new Dictionary<Type, SecurableClassInfo> ();
    private Dictionary<PropertyInfo, StatePropertyInfo> _stateProperties = new Dictionary<PropertyInfo, StatePropertyInfo> ();
    private Dictionary<Enum, EnumValueInfo> _enumValues = new Dictionary<Enum, EnumValueInfo> ();
    private Dictionary<Enum, EnumValueInfo> _accessTypes = new Dictionary<Enum, EnumValueInfo> ();
    private Dictionary<Enum, EnumValueInfo> _abstractRoles = new Dictionary<Enum, EnumValueInfo> ();

    // construction and disposing

    public MetadataCache ()
    {
    }

    // methods and properties

    public SecurableClassInfo GetSecurableClassInfo (Type key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (_classes.ContainsKey (key))
        return _classes[key];
      else
        return null;
    }

    public void AddSecurableClassInfo (Type key, SecurableClassInfo value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _classes.Add (key, value);
    }

    public bool ContainsSecurableClassInfo (Type key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _classes.ContainsKey (key);
    }

    public StatePropertyInfo GetStatePropertyInfo (PropertyInfo key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      key = NormalizeProperty (key);
      if (_stateProperties.ContainsKey (key))
        return _stateProperties[key];
      else
        return null;
    }

    public void AddStatePropertyInfo (PropertyInfo key, StatePropertyInfo value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _stateProperties.Add (NormalizeProperty (key), value);
    }

    public bool ContainsStatePropertyInfo (PropertyInfo key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _stateProperties.ContainsKey (NormalizeProperty (key));
    }

    private PropertyInfo NormalizeProperty (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      if (property.DeclaringType == property.ReflectedType)
        return property;
      else
        return property.DeclaringType.GetProperty (property.Name);
    }

    public EnumValueInfo GetEnumValueInfo (Enum key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (_enumValues.ContainsKey (key))
        return _enumValues[key];
      else
        return null;
    }

    public void AddEnumValueInfo (Enum key, EnumValueInfo value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _enumValues.Add (key, value);
    }

    public bool ContainsEnumValueInfo (Enum key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _enumValues.ContainsKey (key);
    }

    public EnumValueInfo GetAccessType (Enum key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (_accessTypes.ContainsKey (key))
        return _accessTypes[key];
      else
        return null;
    }

    public void AddAccessType (Enum key, EnumValueInfo value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _accessTypes.Add (key, value);
    }

    public bool ContainsAccessType (Enum key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _accessTypes.ContainsKey (key);
    }

    public EnumValueInfo GetAbstractRole (Enum key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (_abstractRoles.ContainsKey (key))
        return _abstractRoles[key];
      else
        return null;
    }

    public void AddAbstractRole (Enum key, EnumValueInfo value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _abstractRoles.Add (key, value);
    }

    public bool ContainsAbstractRole (Enum key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _abstractRoles.ContainsKey (key);
    }

    public List<SecurableClassInfo> GetSecurableClassInfos ()
    { 
      return new List<SecurableClassInfo> (_classes.Values);
    }

    public List<StatePropertyInfo> GetStatePropertyInfos ()
    {
      return new List<StatePropertyInfo> (_stateProperties.Values);
    }

    public List<EnumValueInfo> GetAccessTypes ()
    {
      return new List<EnumValueInfo> (_accessTypes.Values);
    }

    public List<EnumValueInfo> GetAbstractRoles ()
    {
      return new List<EnumValueInfo> (_abstractRoles.Values);
    }
  }
}
