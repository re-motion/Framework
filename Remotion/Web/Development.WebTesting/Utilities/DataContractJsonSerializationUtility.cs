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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Utility class for serializing/deserializing data contract annotated object.  
  /// </summary>
  public class DataContractJsonSerializationUtility
  {
    /// <summary>
    /// Serializes the specified data contract annotated object and returns it as JSON.
    /// </summary>
    public static string Serialize<T> ([NotNull] T data)
        where T : class
    {
      ArgumentUtility.CheckNotNull("data", data);

      var serializer = new DataContractJsonSerializer(typeof (T));
      using (var dataStream = new MemoryStream())
      {
        serializer.WriteObject(dataStream, data);

        return Encoding.UTF8.GetString(dataStream.ToArray());
      }
    }

    /// <summary>
    /// Deserializes the specified JSON string into the specified data contract type.
    /// </summary>
    public static T? Deserialize<T> ([CanBeNull] string? serializedData)
        where T : class
    {
      if (string.IsNullOrEmpty(serializedData))
        return default(T);

      using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedData)))
      {
        var serializer = new DataContractJsonSerializer(typeof (T));
        return (T?) serializer.ReadObject(dataStream);
      }
    }
  }
}