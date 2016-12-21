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

namespace Remotion.UnitTests.Utilities.CustomAttributeUtilityTestDomain
{
  public class AttributeWithPropertyAndFieldParams : Attribute
  {
    public AttributeWithPropertyAndFieldParams (int i, string s, object o, Type t, int[] iArray, string[] sArray, object[] oArray, Type[] tArray)
    {
    }

    public int INamed
    {
      get { return 0; }
      set { }
    }

    public string SNamed
    {
      get { return null; }
      set { }
    }

    public object ONamed
    {
      get { return null; }
      set { }
    }

    public Type TNamed
    {
      get { return null; }
      set { }
    }

    public int[] INamedArray
    {
      get { return null; }
      set { }
    }

    public string[] SNamedArray
    {
      get { return null; }
      set { }
    }

    public object[] ONamedArray
    {
      get { return null; }
      set { }
    }

    public Type[] TNamedArray
    {
      get { return null; }
      set { }
    }

    public int INamedF;
    public string SNamedF;
    public object ONamedF;
    public Type TNamedF;

    public int[] INamedArrayF;
    public string[] SNamedArrayF;
    public object[] ONamedArrayF;
    public Type[] TNamedArrayF;
  }
}
