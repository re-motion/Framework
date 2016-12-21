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

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Apply to an extension method defining an extensible enum value to specify the position of that value in the list of all values of that
  /// extensible enum type.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class ExtensibleEnumPositionAttribute : Attribute
  {
    private readonly double _positionalKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensibleEnumPositionAttribute"/> attribute.
    /// </summary>
    /// <param name="positionalKey">The positional key to assign the extensible enum value. The default is 0.0. Values with greater keys occur
    /// after values with lower keys in the list of all values of the corresponding extensible enum type.</param>
    public ExtensibleEnumPositionAttribute (double positionalKey)
    {
      _positionalKey = positionalKey;
    }

    /// <summary>
    /// Gets the positional key of the extensible enum value. Values with greater keys occur
    /// after values with lower keys in the list of all values of the corresponding extensible enum type.
    /// </summary>
    /// <value>The positional key.</value>
    public double PositionalKey
    {
      get { return _positionalKey; }
    }
  }
}