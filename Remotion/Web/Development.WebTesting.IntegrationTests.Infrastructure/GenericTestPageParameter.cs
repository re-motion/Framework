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

// ReSharper disable once CheckNamespace

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Represents a parameter that is passed to the test client via generic test page.
  /// </summary>
  public class GenericTestPageParameter : IEnumerable<string>
  {
    private readonly string _name;
    private readonly string[] _arguments;

    public GenericTestPageParameter (string name)
    {
      _name = name;
      _arguments = new string[0];
    }

    public GenericTestPageParameter (string name, params string[] arguments)
    {
      _name = name;
      _arguments = arguments;
    }

    public int ArgumentCount
    {
      get { return _arguments.Length; }
    }

    public string Name
    {
      get { return _name; }
    }

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator ()
    {
      return ((IEnumerable<string>) _arguments).GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public string this [int index]
    {
      get
      {
        if (index < 0 || index >= _arguments.Length)
          throw new ArgumentOutOfRangeException ("index");
        return _arguments[index];
      }
    }
  }
}