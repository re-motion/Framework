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
using System.Linq;

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Deserializes the data serialized by a <see cref="FlatMixinContextSerializer"/>.
  /// </summary>
  public class FlatMixinContextDeserializer : ArrayMixinContextDeserializer
  {
    public FlatMixinContextDeserializer (object[] values)
        : base(values)
    {
    }

    protected override T ConvertFromStorageFormat<T> (object value, int index)
    {
      if (typeof(T) == typeof(Type[]))
      {
        var convertedTypes = ConvertFromStorageFormat<string[]>(value, index);
        return (T)(object)Enumerable.ToArray(convertedTypes.Select(ConvertFromStorageFormat<Type>));
      }

      if (typeof(T) == typeof(Type))
      {
        var typeName = ConvertFromStorageFormat<string>(value, index);
        // TODO RM-7810: A meaningful exception should be thrown if no type can be found.
        return (T)(object)Type.GetType(typeName)!;
      }

      return base.ConvertFromStorageFormat<T>(value, index);
    }

    protected override ArrayMixinContextOriginDeserializer CreateMixinContextOriginDeserializer (object[] values)
    {
      return new FlatMixinContextOriginDeserializer(values);
    }
  }
}
