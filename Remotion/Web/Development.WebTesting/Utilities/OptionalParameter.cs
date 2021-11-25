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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// A <see cref="Nullable{T}"/> for reference and value types.
  /// </summary>
  public struct OptionalParameter<T>
  {
    public static implicit operator OptionalParameter<T> (T value)
    {
      return new OptionalParameter<T>(value);
    }

    public static explicit operator T (OptionalParameter<T> optionalParameter)
    {
      return optionalParameter.Value;
    }

    private readonly T _value;
    private readonly bool _hasValue;

    private OptionalParameter (T value)
    {
      _value = value;
      _hasValue = true;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the <see cref="OptionalParameter{T}"/> contains a value, <see langword="false" /> if otherwise.
    /// </summary>
    public bool HasValue
    {
      get { return _hasValue; }
    }

    /// <summary>
    /// Returns the value <typeparamref name="T"/> of the <see cref="OptionalParameter{T}"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException"><see cref="HasValue"/> is <see langword="false" />.</exception>
    public T Value
    {
      get
      {
        if (!_hasValue)
          throw new InvalidOperationException("Optional parameter has no value.");
        return _value;
      }
    }

    /// <summary>
    /// Returns <see cref="Value"/> if <see cref="HasValue"/> == <see langword="true" />, otherwise <paramref name="defaultValue"/>.
    /// </summary>
    [CanBeNull]
    [return: NotNullIfNotNull ("defaultValue")]
    public T? GetValueOrDefault (T? defaultValue)
    {
      if (_hasValue)
        return _value;
      return defaultValue;
    }

    /// <inheritdoc />
    public override bool Equals (object? other)
    {
      if (!_hasValue)
        return other == null;
      if (other == null)
        return false;
      return _value!.Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode ()
    {
      if (_hasValue)
        return _value!.GetHashCode();
      return 0;
    }

    /// <inheritdoc />
    public override string? ToString ()
    {
      if (_hasValue)
        return _value!.ToString();
      return string.Empty;
    }
  }
}
