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
using Remotion.Mixins;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.Web.UnitTests.Domain
{
  [BindableObject]
  public class TypeWithString
  {
    public static TypeWithString Create ()
    {
      return ObjectFactory.Create<TypeWithString> (true, ParamList.Empty);
    }

    public static TypeWithString Create (string firstValue, string secondValue)
    {
      return ObjectFactory.Create<TypeWithString> (true, ParamList.Create (firstValue, secondValue));
    }

    private string _stringValue;
    private string[] _stringArray;
    private string _firstValue;
    private string _secondValue;

    protected TypeWithString ()
    {
    }

    protected TypeWithString (string firstValue, string secondValue)
    {
      _firstValue = firstValue;
      _secondValue = secondValue;
    }

    public string StringValue
    {
      get { return _stringValue; }
      set { _stringValue = value; }
    }

    public string[] StringArray
    {
      get { return _stringArray; }
      set { _stringArray = value; }
    }

    public string FirstValue
    {
      get { return _firstValue; }
      set { _firstValue = value; }
    }

    public string SecondValue
    {
      get { return _secondValue; }
      set { _secondValue = value; }
    }
  }
}
