﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Reflection;

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Provides utility functionality for classes serializing Context objects into an array.
  /// </summary>
  public abstract class ArrayContextSerializerBase
  {
    private readonly object[] _values;

    protected ArrayContextSerializerBase (int valueCount)
    {
      _values = new object[valueCount];
    }

    public object[] Values
    {
      get { return _values; }
    }

    protected void SetValue<T> (int index, T value)
        where T : notnull
    {
      Values[index] = ConvertToStorageFormat(value);
    }

    protected virtual object ConvertToStorageFormat<T> (T value)
        where T : notnull
    {
      if (typeof(T) == typeof(Assembly))
        return ConvertToStorageFormat(((Assembly)(object)value).GetFullNameChecked());

      return value;
    }
  }
}
