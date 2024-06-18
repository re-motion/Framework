// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;

namespace MixinXRef.Formatting
{
  public class TypeModifierUtility
  {
    public string GetTypeModifiers (Type type)
    {
      var modifiers = "";

      if (type.IsPublic || type.IsNestedPublic)
        modifiers = "public";
      else if (type.IsNestedFamily)
        modifiers = "protected";
      else if (type.IsNestedFamORAssem)
        modifiers = "protected internal";
      else if (type.IsNestedAssembly)
        modifiers = "internal";
      else if (type.IsNestedPrivate)
        modifiers = "private";
          // non nested internal class - no own flag?
      else if (type.IsNotPublic)
        modifiers = "internal";

      if (type.IsAbstract)
        modifiers += " abstract";
      else if (type.IsSealed)
        modifiers += " sealed";

      return modifiers;
    }
  }
}