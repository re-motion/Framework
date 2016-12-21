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

namespace Remotion.Security
{
  // Make DemandPermissionAttribute sealed once this type is deleted.
  [Obsolete ("Use DemandPermissionAttribute instead. (Version 1.13.74)")]
  [AttributeUsage (AttributeTargets.Method, AllowMultiple=false)]
  public class DemandMethodPermissionAttribute : DemandPermissionAttribute
  {
    public DemandMethodPermissionAttribute (object accessType1)
        : base (new object[] { accessType1 })
    {
    }

    public DemandMethodPermissionAttribute (object accessType1, object accessType2)
        : base (new object[] { accessType1, accessType2 })
    {
    }

    public DemandMethodPermissionAttribute (object accessType1, object accessType2, object accessType3)
        : base (new object[] { accessType1, accessType2, accessType3 })
    {
    }

    public DemandMethodPermissionAttribute (object accessType1, object accessType2, object accessType3, object accessType4)
        : base (new object[] { accessType1, accessType2, accessType3, accessType4 })
    {
    }

    public DemandMethodPermissionAttribute (object accessType1, object accessType2, object accessType3, object accessType4, object accessType5)
        : base (new object[] { accessType1, accessType2, accessType3, accessType4, accessType5 })
    {
    }
  }
}
