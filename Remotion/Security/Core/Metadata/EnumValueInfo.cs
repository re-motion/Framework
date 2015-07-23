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
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class EnumValueInfo : MetadataInfo
  {
    // types

    // static members

    // member fields

    private int _value;
    private string _typeName;

    // construction and disposing

    public EnumValueInfo (string typeName, string name, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      _value = value;
      _typeName = typeName;
      Name = name;
    }

    // methods and properties

    public int Value
    {
      get { return _value; }
      set { _value = value; }
    }

    public string TypeName
    {
      get
      {
        return _typeName;
      }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("TypeName", value);
        _typeName = value;
      }
    }

    public override string Description
    {
      get { return TypeName; }
    }
  }
}
