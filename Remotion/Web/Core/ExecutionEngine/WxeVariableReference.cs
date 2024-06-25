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

namespace Remotion.Web.ExecutionEngine
{
  public class WxeVariableReference
  {
    private readonly string _name;

    public WxeVariableReference (string variableName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("variableName", variableName);
      if (! System.Text.RegularExpressions.Regex.IsMatch(variableName, @"^([a-zA-Z_][a-zA-Z0-9_]*)$"))
        throw new ArgumentException(string.Format("The variable name '{0}' is not valid.", variableName), "variableName");
      _name = variableName;
    }

    public string Name
    {
      get { return _name; }
    }

    public override bool Equals (object? obj)
    {
      WxeVariableReference? other = obj as WxeVariableReference;
      if (other == null)
        return false;

      return this._name == other._name;
    }

    public override int GetHashCode ()
    {
      return _name.GetHashCode();
    }
  }
}
