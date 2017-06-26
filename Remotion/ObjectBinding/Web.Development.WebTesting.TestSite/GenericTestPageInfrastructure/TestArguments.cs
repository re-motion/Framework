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
using System.Collections;
using System.Collections.Generic;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.GenericTestPageInfrastructure
{
  public class TestArguments : IEnumerable<string>
  {
    private readonly string[] _values;

    public TestArguments (params string[] values)
    {
      _values = values;
    }

    public int Length
    {
      get { return _values.Length; }
    }

    public IEnumerator<string> GetEnumerator ()
    {
      return ((IEnumerable<string>) _values).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public string this [int index]
    {
      get { return _values[index]; }
    }
  }
}