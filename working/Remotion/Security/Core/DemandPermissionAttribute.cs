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

namespace Remotion.Security
{
  [AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
  public class DemandPermissionAttribute : Attribute
  {
    // HACK: Cannot store an Enum[] because that causes CustomAttributeData to throw an undocumented and unexpected exception 
    // (http://connect.microsoft.com/VisualStudio/feedback/details/296032/customattributedata-throws-when-attribute-has-a-public-enum-property)
    // Probably fixed in .NET 4.0
    private readonly object[] _accessTypes; 

    public DemandPermissionAttribute (object accessType1)
        : this (new [] { accessType1 })
    {
    }

    public DemandPermissionAttribute (object accessType1, object accessType2)
        : this (new [] { accessType1, accessType2 })
    {
    }

    public DemandPermissionAttribute (object accessType1, object accessType2, object accessType3)
        : this (new [] { accessType1, accessType2, accessType3 })
    {
    }

    public DemandPermissionAttribute (object accessType1, object accessType2, object accessType3, object accessType4)
        : this (new [] { accessType1, accessType2, accessType3, accessType4 })
    {
    }

    public DemandPermissionAttribute (object accessType1, object accessType2, object accessType3, object accessType4, object accessType5)
        : this (new [] { accessType1, accessType2, accessType3, accessType4, accessType5 })
    {
    }

    private DemandPermissionAttribute (object[] accessTypes)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("accessTypes", accessTypes);
      ArgumentUtility.CheckItemsType ("accessTypes", accessTypes, typeof (Enum));

      Enum[] accessTypeEnums = new Enum[accessTypes.Length];

      for (int i = 0; i < accessTypes.Length; i++)
        accessTypeEnums[i] = GetAccessType (accessTypes[i]);

      _accessTypes = accessTypeEnums;
    }

    public Enum[] GetAccessTypes ()
    {
      return (Enum[]) _accessTypes;
    }

    private Enum GetAccessType (object accessType)
    {
      Type permissionType = accessType.GetType ();
      if (!permissionType.IsDefined (typeof (AccessTypeAttribute), false))
      {
        string message = string.Format (string.Format ("Enumerated Type '{0}' cannot be used as an access type. Valid access types must have the "
                + "Remotion.Security.AccessTypeAttribute applied.", permissionType.FullName));

        throw new ArgumentException (message, "accessType");
      }

      return (Enum) accessType;
    }
  }
}
