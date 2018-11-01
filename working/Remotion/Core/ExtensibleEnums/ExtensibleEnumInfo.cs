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
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Holds information about an extensible enum value, including the <see cref="Value"/> itself and meta-info such as the <see cref="DefiningMethod"/>.
  /// </summary>
  /// <typeparam name="T">The extensible enum type.</typeparam>
  /// <remarks>
  /// Instances of this class are immutable, i.e. they will not change once initialized.
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  public sealed class ExtensibleEnumInfo<T> : IExtensibleEnumInfo
      where T : ExtensibleEnum<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensibleEnumInfo{T}"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="declaringMethod">The declaring method of the value.</param>
    /// <param name="positionalKey">The positional key of the value.</param>
    public ExtensibleEnumInfo (T value, MethodInfo declaringMethod, double positionalKey)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("declaringMethod", declaringMethod);

      Value = value;
      DefiningMethod = declaringMethod;
      PositionalKey = positionalKey;
    }
    
    /// <summary>
    /// Gets the <see cref="ExtensibleEnum{T}"/> value described by this instance.
    /// </summary>
    /// <value>The value.</value>
    public T Value { get; private set; }

    /// <inheritdoc cref="IExtensibleEnumInfo.Value" />
    IExtensibleEnum IExtensibleEnumInfo.Value
    {
      get { return Value; }
    }

    /// <summary>
    /// Gets the method defining the <see cref="Value"/> described by this instance.
    /// </summary>
    /// <value>The defining method of the <see cref="Value"/>.</value>
    public MethodInfo DefiningMethod { get; private set; }

    /// <summary>
    /// Gets the positional key associated with the <see cref="Value"/>. The positional key determines the position of the <see cref="Value"/>
    /// in the list of all values of the extensible enum type.
    /// </summary>
    /// <value>The positional key.</value>
    public double PositionalKey { get; private set; }

    public override string ToString ()
    {
      return string.Format ("ExtensibleEnumInfo: {0} ({1})", Value.ValueName, Value.GetEnumType ());
    }
  }
}