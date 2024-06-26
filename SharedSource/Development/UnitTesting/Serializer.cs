// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Provides quick serialization and deserialization functionality for unit tests.
  /// </summary>
  /// <remarks>The methods of this class use a <see cref="BinaryFormatter"/> for serialization.</remarks>
  static partial class Serializer
  {
    public static T SerializeAndDeserialize<T> (T t) where T : notnull
    {
      if (t == null)
        throw new ArgumentNullException("t");

      return (T)Serializer.Deserialize(Serializer.Serialize((object)t));
    }

    public static byte[] Serialize (object? o)
    {
      if (o == null)
        throw new ArgumentNullException("o");

      using (MemoryStream stream = new MemoryStream())
      {
#pragma warning disable SYSLIB0011
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, o);
#pragma warning restore SYSLIB0011
        return stream.ToArray();
      }
    }

    public static object Deserialize (byte[] bytes)
    {
      if (bytes == null)
        throw new ArgumentNullException("bytes");

      using (MemoryStream stream = new MemoryStream(bytes))
      {
#pragma warning disable SYSLIB0011
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
      }
    }
  }
}
