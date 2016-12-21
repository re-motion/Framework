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
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class AccessTypeReflector : IAccessTypeReflector
  {
    // types

    // static members

    // member fields
    private readonly IEnumerationReflector _enumerationReflector;
    private readonly PermissionReflector _permissionReflector = new PermissionReflector();

    // construction and disposing

    public AccessTypeReflector ()
      : this (new EnumerationReflector ())
    {
    }

    public AccessTypeReflector (IEnumerationReflector enumerationReflector)
    {
      ArgumentUtility.CheckNotNull ("enumerationReflector", enumerationReflector);
      _enumerationReflector = enumerationReflector;
    }

    // methods and properties

    public IEnumerationReflector EnumerationTypeReflector
    {
      get { return _enumerationReflector; }
    }

    public List<EnumValueInfo> GetAccessTypesFromAssembly (Assembly assembly, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("cache", cache);

      List<EnumValueInfo> accessTypes = new List<EnumValueInfo> ();
      foreach (var type in AssemblyTypeCache.GetTypes (assembly))
      {
        if (type.IsEnum && Attribute.IsDefined (type, typeof (AccessTypeAttribute), false))
        {
          Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues (type, cache);
          foreach (KeyValuePair<Enum, EnumValueInfo> entry in values)
          {
            if (!cache.ContainsAccessType (entry.Key))
              cache.AddAccessType (entry.Key, entry.Value);
            accessTypes.Add (entry.Value);
          }
        }
      }

      return accessTypes;
    }

    public List<EnumValueInfo> GetAccessTypesFromType (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("cache", cache);

      Dictionary<Enum, EnumValueInfo> accessTypes = _enumerationReflector.GetValues (typeof (GeneralAccessTypes), cache);
      foreach (KeyValuePair<Enum, EnumValueInfo> entry in accessTypes)
      {
        if (!cache.ContainsAccessType (entry.Key))
          cache.AddAccessType (entry.Key, entry.Value);
      }

      AddAccessTypes (type, accessTypes, cache);

      return new List<EnumValueInfo> (accessTypes.Values);
    }
    
    private void AddAccessTypes (Type type, Dictionary<Enum, EnumValueInfo> accessTypes, MetadataCache cache)
    {
      var instanceMethods = GetInstanceMethods (type);
      var staticMethods = GetStaticMethods (type);

      var methodInformations = instanceMethods.Concat (staticMethods);

      AddAccessTypesFromAttribute (methodInformations, accessTypes, cache);
    }

    private IEnumerable<MethodInfo> GetStaticMethods (Type type)
    {
      MemberInfo[] staticMethods = type.FindMembers (
          MemberTypes.Method,
          BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy,
          FindSecuredMembersFilter,
          null);
      return staticMethods.Cast<MethodInfo>();
    }

    private IEnumerable<MethodInfo> GetInstanceMethods (Type type)
    {
      MemberInfo[] instanceMethods = type.FindMembers (
          MemberTypes.Method,
          BindingFlags.Instance | BindingFlags.Public,
          FindSecuredMembersFilter,
          null);
      return instanceMethods.Cast<MethodInfo>();
    }

    private bool FindSecuredMembersFilter (MemberInfo member, object filterCriteria)
    {
      return Attribute.IsDefined (member, typeof (DemandPermissionAttribute), true);
    }

    private void AddAccessTypesFromAttribute (IEnumerable<MethodInfo> methodInfos, Dictionary<Enum, EnumValueInfo> accessTypes, MetadataCache cache)
    {
      foreach (var methodInfo in methodInfos)
      {
        var values = _permissionReflector.GetRequiredMethodPermissions (methodInfo.DeclaringType, MethodInfoAdapter.Create(methodInfo));
        foreach (Enum value in values)
        {
          EnumValueInfo accessType = _enumerationReflector.GetValue (value, cache);

          if (!cache.ContainsAccessType (value))
            cache.AddAccessType (value, accessType);

          if (!accessTypes.ContainsKey (value))
            accessTypes.Add (value, accessType);
        }
      }
    }
  }
}
