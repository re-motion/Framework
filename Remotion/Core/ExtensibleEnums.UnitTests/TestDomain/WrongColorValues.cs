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

namespace Remotion.ExtensibleEnums.UnitTests.TestDomain
{
  public static class WrongColorValues
  {
    public static int WrongReturnType (this ExtensibleEnumDefinition<Color> definition)
    {
      throw new NotImplementedException ();
    }

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
    private static Color WrongVisibility1 (this ExtensibleEnumDefinition<Color> definition)
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local
    {
      throw new NotImplementedException ();
    }

    internal static Color WrongVisibility2 (this ExtensibleEnumDefinition<Color> definition)
    {
      throw new NotImplementedException ();
    }

    public static Color NonExtensionMethod (ExtensibleEnumDefinition<Color> definition)
    {
      throw new NotImplementedException ();
    }

    public static Color WrongParameterCount (this ExtensibleEnumDefinition<Color> definition, int index)
    {
      throw new NotImplementedException ();
    }

    public static Color NotDerivedFromValuesClass (this Color values)
    {
      throw new NotImplementedException ();
    }

// ReSharper disable UnusedTypeParameter
    public static Color Generic<T> (this ExtensibleEnumDefinition<Color> definition)
// ReSharper restore UnusedTypeParameter
    {
      throw new NotImplementedException ();
    }
  }
}
