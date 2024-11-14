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
using Remotion.Security;

namespace Remotion.SecurityManager.Domain
{
  [AccessType]
  [MultiLingualResources("Remotion.SecurityManager.Globalization.Domain.SecurityManagerAccessTypes")]
  public enum SecurityManagerAccessTypes
  {
    [PermanentGuid("0348BE71-CFAF-4184-A3BF-C621B2611A29")]
    AssignRole = 0,

    [PermanentGuid("4564E3BD-7E1D-4afc-9715-9C698B46A037")]
    AssignSubstitute = 1,
  }
}
