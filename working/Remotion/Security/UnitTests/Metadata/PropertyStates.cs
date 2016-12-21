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
  public static class PropertyStates
  {
    public static readonly EnumValueInfo FileStateNew = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain", "New", 0);
    public static readonly EnumValueInfo FileStateNormal = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain", "Normal", 1);
    public static readonly EnumValueInfo FileStateArchived = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain", "Archived", 2);
    public static readonly EnumValueInfo ConfidentialityNormal = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain", "Normal", 0);
    public static readonly EnumValueInfo ConfidentialityConfidential = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain", "Confidential", 1);
    public static readonly EnumValueInfo ConfidentialityPrivate = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain", "Private", 2);
  }
}
