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
using System.Reflection;

namespace Remotion.ExtensibleEnums.UnitTests.TestDomain
{
  public static class EnumWithDifferentCtorsExtensions
  {
    public static EnumWithDifferentCtors IDOnly (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors ("ValueName");
    }

    public static EnumWithDifferentCtors DeclarationSpaceAndName (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors ("DeclarationSpace", "ValueName");
    }

    public static EnumWithDifferentCtors NameAndNullDeclarationSpace (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors ((string) null, "ValueName");
    }

    public static EnumWithDifferentCtors NameAndEmptyDeclarationSpace (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors ("", "ValueName");
    }

    public static EnumWithDifferentCtors DeclaringTypeAndName (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors (typeof (EnumWithDifferentCtorsExtensions), "ValueName");
    }

    public static EnumWithDifferentCtors CurrentMethod (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors (MethodBase.GetCurrentMethod());
    }
  }
}
