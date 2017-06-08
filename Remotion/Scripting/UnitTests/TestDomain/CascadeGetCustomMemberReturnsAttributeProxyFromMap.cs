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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Remotion.Scripting.UnitTests.TestDomain
{
  public class CascadeGetCustomMemberReturnsAttributeProxyFromMap : Cascade
  {
    protected readonly Dictionary<Tuple<Type, string>, object> _attributeProxyMap = new Dictionary<Tuple<Type, string>, object> ();

    public CascadeGetCustomMemberReturnsAttributeProxyFromMap (int nrChildren)
    {
      --nrChildren;
      Name = "C" + nrChildren;
      if (nrChildren > 0)
      {
        Child = new CascadeGetCustomMemberReturnsAttributeProxyFromMap (nrChildren);
      }
    }

    public void AddAttributeProxy (string name, object proxied, ScriptContext scriptContext)
    {
      var type = this.GetType ();
      
      var attributeNameProxy = scriptContext.GetAttributeProxy (proxied, name);
      _attributeProxyMap[new Tuple<Type, string> (type,name)] = attributeNameProxy;
    }

    [SpecialName]
    public object GetCustomMember (string name)
    {
      var type = this.GetType();
      return _attributeProxyMap[new Tuple<Type, string> (type, name)];
    }
  }
}