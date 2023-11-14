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
using System.Diagnostics.CodeAnalysis;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Serialization
{
  public class FlattenedSerializationInfo
  {
    private readonly FlattenedSerializationWriter<object?> _objectWriter = new FlattenedSerializationWriter<object?>();
    private readonly FlattenedSerializationWriter<int> _intWriter = new FlattenedSerializationWriter<int>();
    private readonly FlattenedSerializationWriter<bool> _boolWriter = new FlattenedSerializationWriter<bool>();
    private readonly Dictionary<object, int> _handleMap = new Dictionary<object, int>();

    public FlattenedSerializationInfo ()
    {
    }

    public object[] GetData ()
    {
      var objects = _objectWriter.GetData();
      var ints = _intWriter.GetData();
      var bools = _boolWriter.GetData();

      return new object[] { objects, ints, bools };
    }

    public void AddIntValue (int value)
    {
      _intWriter.AddSimpleValue(value);
    }

    public void AddBoolValue (bool value)
    {
      _boolWriter.AddSimpleValue(value);
    }

    /// <remarks>Note: This will use T to decide whether to recurse into an IFlattenedSerializable or not.</remarks>
    public void AddValue<T> (T? value)
    {
      if (typeof(T) == typeof(Type))
        AddHandle(GetTypeNameFromTypeOrNull((Type?)(object?)value));
      else if (typeof(IFlattenedSerializable).IsAssignableFrom(typeof(T)))
        AddFlattenedSerializable((IFlattenedSerializable?)value);
      else
        _objectWriter.AddSimpleValue(value);
    }

    private void AddFlattenedSerializable (IFlattenedSerializable? serializable)
    {
      if (serializable != null)
      {
        var type = serializable.GetType();
        var typeName = GetTypeNameFromTypeOrNull(type);
        AddHandle(typeName);
        serializable.SerializeIntoFlatStructure(this);
      }
      else
      {
        AddHandle<Type?>(null);
      }
    }

    public void AddArray<T> (T[] valueArray)
    {
      ArgumentUtility.CheckNotNull("valueArray", valueArray);
      AddCollection(valueArray);
    }

    public void AddCollection<T> (ICollection<T> valueCollection)
    {
      ArgumentUtility.CheckNotNull("valueCollection", valueCollection);
      AddIntValue(valueCollection.Count);
      foreach (T t in valueCollection)
        AddValue(t);
    }

    public void AddHandle<T> (T? value)
    {
      if (value == null)
        AddIntValue(-1);
      else
      {
        int handle;
        if (!_handleMap.TryGetValue(value, out handle))
        {
          handle = _handleMap.Count;
          _handleMap.Add(value, handle);
          AddIntValue(handle);
          AddValue(value);
        }
        else
          AddIntValue(handle);
      }
    }

    [return: NotNullIfNotNull("type")]
    private string? GetTypeNameFromTypeOrNull (Type? type) => type?.AssemblyQualifiedName;
  }
}
