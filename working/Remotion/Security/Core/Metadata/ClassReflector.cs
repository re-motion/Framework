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

  public class ClassReflector : IClassReflector
  {
    // types

    // static members

    // member fields

    private IStatePropertyReflector _statePropertyReflector;
    private IAccessTypeReflector _accessTypeReflector;

    // construction and disposing

    public ClassReflector ()
      : this (new StatePropertyReflector (), new AccessTypeReflector ())
    {
    }

    public ClassReflector (IStatePropertyReflector statePropertyReflector, IAccessTypeReflector accessTypeReflector)
    {
      ArgumentUtility.CheckNotNull ("statePropertyReflector", statePropertyReflector);
      ArgumentUtility.CheckNotNull ("accessTypeReflector", accessTypeReflector);

      _statePropertyReflector = statePropertyReflector;
      _accessTypeReflector = accessTypeReflector;
    }

    // methods and properties

    public IStatePropertyReflector StatePropertyReflector
    {
      get { return _statePropertyReflector; }
    }

    public IAccessTypeReflector AccessTypeReflector
    {
      get { return _accessTypeReflector; }
    }

    public SecurableClassInfo GetMetadata (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));
      if (type.IsValueType)
        throw new ArgumentException ("Value types are not supported.", "type");
      ArgumentUtility.CheckNotNull ("cache", cache);

      SecurableClassInfo info = cache.GetSecurableClassInfo (type);
      if (info == null)
      {
        info = new SecurableClassInfo ();
        info.Name = TypeUtility.GetPartialAssemblyQualifiedName (type);
        PermanentGuidAttribute guidAttribute = (PermanentGuidAttribute) Attribute.GetCustomAttribute (type, typeof (PermanentGuidAttribute), true);
        if (guidAttribute != null)
          info.ID = guidAttribute.Value.ToString ();
        info.Properties.AddRange (GetProperties (type, cache));
        info.AccessTypes.AddRange (_accessTypeReflector.GetAccessTypesFromType (type, cache));

        cache.AddSecurableClassInfo (type, info);

        if (typeof (ISecurableObject).IsAssignableFrom (type.BaseType))
        {
          info.BaseClass = GetMetadata (type.BaseType, cache);
          info.BaseClass.DerivedClasses.Add (info);
        }
      }

      return info;
    }

    protected virtual List<StatePropertyInfo> GetProperties (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableObject));
      ArgumentUtility.CheckNotNull ("cache", cache);

      MemberInfo[] propertyInfos = type.FindMembers (
          MemberTypes.Property,
          BindingFlags.Instance | BindingFlags.Public,
          FindStatePropertiesFilter,
          null);

      List<StatePropertyInfo> statePropertyInfos = new List<StatePropertyInfo> ();
      foreach (PropertyInfo propertyInfo in propertyInfos)
        statePropertyInfos.Add (_statePropertyReflector.GetMetadata (propertyInfo, cache));

      return statePropertyInfos;
    }

    protected bool FindStatePropertiesFilter (MemberInfo member, object filterCriteria)
    {
      PropertyInfo property = ArgumentUtility.CheckNotNullAndType<PropertyInfo> ("member", member);
      return property.PropertyType.IsEnum && Attribute.IsDefined (property.PropertyType, typeof (SecurityStateAttribute), false);
    }
  }
}
