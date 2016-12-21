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
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Provides functionality for classes deserializing a Context from an array.
  /// </summary>
  public abstract class ArrayContextDeserializerBase
  {
    private readonly object[] _values;

    protected ArrayContextDeserializerBase (object[] values, int expectedNumberOfValues)
    {
      ArgumentUtility.CheckNotNull ("values", values);

      if (values.Length != expectedNumberOfValues)
        throw new ArgumentException (string.Format ("Expected an array with {0} elements.", expectedNumberOfValues), "values");

      _values = values;
    }

    protected T GetValue<T> (int index)
    {
      var value = _values[index];

      return ConvertFromStorageFormat<T> (value, index);
    }

    protected virtual T ConvertFromStorageFormat<T> (object value, int index)
    {
      if (!(value is T))
      {
        var message = string.Format (
            "Expected value of type '{0}' at index {1} in the values array, but found '{2}'.",
            typeof (T).FullName,
            index,
            value != null ? value.GetType().FullName : "null");
        throw new SerializationException (message);
      }

      return (T) value;
    }
  }
}