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
using Remotion.Globalization;
using Remotion.ObjectBinding;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [MultiLingualResources("Remotion.SecurityManager.Globalization.Domain.AccessControl.GroupHierarchyCondition")]
  [UndefinedEnumValue(Undefined)]
  //[Flags] //Must not be official flags enum since business objects interface does not support this.
  [DisableEnumValues(Parent, Children)]
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
