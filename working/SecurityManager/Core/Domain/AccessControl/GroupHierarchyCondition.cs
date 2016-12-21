// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Globalization;
using Remotion.ObjectBinding;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.AccessControl.GroupHierarchyCondition")]
  [UndefinedEnumValue (Undefined)]
  //[Flags] //Must not be official flags enum since business objects interface does not support this.
  [DisableEnumValues (Parent, Children)]
  public enum GroupHierarchyCondition
  {
    Undefined = 0,
    This = 1,
    Parent = 2,
    Children = 4,
    ThisAndParent = This | Parent, // 3
    ThisAndChildren = This | Children, // 5
    ThisAndParentAndChildren = This | Parent | Children, // 7
  }
}
