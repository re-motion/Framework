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
using System.Reflection;
using Remotion.ExtensibleEnums.Infrastructure;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Provides a non-generic interface for the <see cref="ExtensibleEnumInfo{T}"/> class.
  /// </summary>
  /// <remarks>
  /// Do not implement this interface yourself, use it only when working with extensible enums in a reflective context, e.g. via the 
  /// <see cref="ExtensibleEnumDefinitionCache"/> class.
  /// </remarks>
  public interface IExtensibleEnumInfo
  {
    /// <summary>
    /// Gets the <see cref="ExtensibleEnum{T}"/> value described by this instance.
    /// </summary>
    /// <value>The value.</value>
    IExtensibleEnum Value { get; }

    /// <summary>
    /// Gets the method defining the <see cref="Value"/> described by this instance.
    /// </summary>
    /// <value>The defining method of the <see cref="Value"/>.</value>
    MethodInfo DefiningMethod { get; }

    /// <summary>
    /// Gets the positional key associated with the <see cref="Value"/>. The positional key determines the position of the <see cref="Value"/>
    /// in the list of all values of the extensible enum type.
    /// </summary>
    /// <value>The positional key.</value>
    double PositionalKey { get; }
  }
}