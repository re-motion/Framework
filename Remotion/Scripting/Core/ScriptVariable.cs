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

namespace Remotion.Scripting
{
  /// <summary>
  /// Contains the value of a variable retrieved from a <see cref="ScriptEnvironment"/>, together with the information 
  /// whether the value is valid (i.e. the variable existed).
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public struct ScriptVariable<T>
  {
    private readonly T _value;
    private readonly bool _isValid;

    public ScriptVariable (T value, bool isValid)
    {
      _value = value;
      _isValid = isValid;
    }

    /// <summary>
    /// Value of the variable. Defined only if <see cref="IsValid"/> is <see langword="true"/>.
    /// </summary>
    public T Value
    {
      get { return _value; }
    }

    /// <summary>
    /// <see langword="true"/> if the variable existed, <see langword="false"/> otherwise.
    /// </summary>
    public bool IsValid
    {
      get { return _isValid; }
    }
  }
}