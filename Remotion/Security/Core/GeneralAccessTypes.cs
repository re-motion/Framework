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
  [AccessType]
  public enum GeneralAccessTypes
  {
    [PermanentGuid ("1D6D25BC-4E85-43ab-A42D-FB5A829C30D5")]
    Create = 0,
    [PermanentGuid ("62DFCD92-A480-4d57-95F1-28C0F5996B3A")]
    Read = 1,
    [PermanentGuid ("11186122-6DE0-4194-B434-9979230C41FD")]
    Edit = 2,
    [PermanentGuid ("305FBB40-75C8-423a-84B2-B26EA9E7CAE7")]
    Delete = 3,
    [PermanentGuid ("67CEA479-0BE7-4e2f-B2E0-BB1FCC9EA1D6")]
    Search = 4,
    [PermanentGuid ("203B7478-96F1-4bf1-B4EA-5BDD1206252C")]
    Find = 5
  }
}
