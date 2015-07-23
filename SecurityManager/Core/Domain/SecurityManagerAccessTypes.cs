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
using Remotion.Security;

namespace Remotion.SecurityManager.Domain
{
  [AccessType]
  public enum SecurityManagerAccessTypes
  {
    [PermanentGuid ("0348BE71-CFAF-4184-A3BF-C621B2611A29")]
    AssignRole = 0,

    [PermanentGuid ("4564E3BD-7E1D-4afc-9715-9C698B46A037")]
    AssignSubstitute= 1,
}
}
