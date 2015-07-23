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

namespace Remotion.Web.ExecutionEngine.Obsolete
{
  /// <summary>
  /// Specifies that a WXE function should automatically be created by the WXE function generator (wxegen.exe).
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class WxeFunctionPageAttribute : Attribute
  {
    private string _axpxPageName;
    private Type _baseClass;

    public WxeFunctionPageAttribute (string aspxPageName)
      : this (aspxPageName, typeof (WxeFunction))
    {
    }

    public WxeFunctionPageAttribute (string aspxPageName, Type baseClass)
    {
      _axpxPageName = aspxPageName;
      _baseClass = baseClass;
    }

    public string AspxPageName
    {
      get { return _axpxPageName; }
    }

    public Type BaseClass
    {
      get { return _baseClass; }
    }
  }

  /// <summary>
  /// Specifies a WXE function parameter that should automatically be created by the WXE function generator (wxegen.exe). 
  /// Requires <see cref="WxeFunctionPageAttribute"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class WxePageParameterAttribute: Attribute
  {
    private int _index;
    private string _name;
    private Type _type;
    private bool _required;
    private WxeParameterDirection _direction;

    public WxePageParameterAttribute (int index, string name, Type type, bool required, WxeParameterDirection direction)
    {
      _index = index;
      _name = name;
      _type = type;
      _required = required;
      _direction = direction;
    }

    public WxePageParameterAttribute (int index, string name, Type type)
      : this (index, name, type, false, WxeParameterDirection.In)
    {
    }

    public WxePageParameterAttribute (int index, string name, Type type, WxeParameterDirection direction)
      : this (index, name, type, false, direction)
    {
    }

    public WxePageParameterAttribute (int index, string name, Type type, bool required)
      : this (index, name, type, required, WxeParameterDirection.In)
    {
    }

    public int Index
    {
      get { return _index; }
    }

    public string Name
    {
      get { return _name; }
    }

    public Type Type
    {
      get { return _type; }
    }

    public bool Required
    {
      get { return _required; }
    }

    public WxeParameterDirection Direction
    {
      get { return _direction; }
    }
  }
}
