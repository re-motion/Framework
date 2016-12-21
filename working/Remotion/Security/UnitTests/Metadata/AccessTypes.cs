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
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Metadata
{

  public static class AccessTypes
  {
    public static readonly EnumValueInfo Read = new EnumValueInfo ("Security.GeneralAccessTypes, Remotion", "Read", 0);
    public static readonly EnumValueInfo Write = new EnumValueInfo ("Security.Security.GeneralAccessTypes, Remotion", "Write", 1);
    public static readonly EnumValueInfo Journalize = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain", "Journalize", 0);
    public static readonly EnumValueInfo Archive = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain", "Archive", 1);
  }
}
