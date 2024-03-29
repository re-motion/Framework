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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  public static class FlattenedSerializer
  {
    public static object[] Serialize (IFlattenedSerializable serializable)
    {
      FlattenedSerializationInfo info = new FlattenedSerializationInfo();
      info.AddValue(serializable);
      return info.GetData();
    }

    public static T Deserialize<T> (object[] data) where T : IFlattenedSerializable
    {
      FlattenedDeserializationInfo info = new FlattenedDeserializationInfo(data);
      var value = info.GetValue<T>();
      info.SignalDeserializationFinished();
      return value;
    }

    public static T SerializeAndDeserialize<T> (T serializable) where T : IFlattenedSerializable
    {
      var flattenedData = Serialize(serializable);
      var deserializedFlattenedData = Serializer.SerializeAndDeserialize(flattenedData);
      return Deserialize<T>(deserializedFlattenedData);
    }
  }
}
