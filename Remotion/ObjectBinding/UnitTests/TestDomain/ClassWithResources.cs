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

namespace Remotion.ObjectBinding.UnitTests.TestDomain
{
  [BindableObject]
  [MultiLingualResources("Remotion.ObjectBinding.UnitTests.Globalization.ClassWithResources")]
  public class ClassWithResources
  {
    private string _value1;
    private string _value2;
    private string _valueWithoutResource;

    public ClassWithResources ()
    {
    }

    public string Value1
    {
      get { return _value1; }
      set { _value1 = value; }
    }

    public string Value2
    {
      get { return _value2; }
      set { _value2 = value; }
    }

    public string ValueWithoutResource
    {
      get { return _valueWithoutResource; }
      set { _valueWithoutResource = value; }
    }
  }
}
