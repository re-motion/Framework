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
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.Serialization
{
  /// <summary>
  /// Serializes instances of <see cref="ConcreteMixinTypeIdentifier"/> into a <see cref="SerializationInfo"/> object. The serialization is
  /// completely flat, using only primitive types, so the returned object is always guaranteed to be complete even in the face of the order of 
  /// deserialization of objects not being deterministic.
  /// </summary>
  public class SerializationInfoConcreteMixinTypeIdentifierSerializer : IConcreteMixinTypeIdentifierSerializer
  {
    private readonly SerializationInfo _serializationInfo;
    private readonly string _key;

    public SerializationInfoConcreteMixinTypeIdentifierSerializer (SerializationInfo serializationInfo, string key)
    {
      ArgumentUtility.CheckNotNull ("serializationInfo", serializationInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      _serializationInfo = serializationInfo;
      _key = key;
    }

    public void AddMixinType (Type mixinType)
    {
      _serializationInfo.AddValue (_key + ".MixinType", mixinType.AssemblyQualifiedName);
    }

    public void AddOverriders (HashSet<MethodInfo> overriders)
    {
      SerializeMethods (_key + ".Overriders", overriders);
    }

    public void AddOverridden (HashSet<MethodInfo> overridden)
    {
      SerializeMethods (_key + ".Overridden", overridden);
    }

    private void SerializeMethods (string collectionKey, ICollection<MethodInfo> collection)
    {
      _serializationInfo.AddValue (collectionKey + ".Count", collection.Count);

      var index = 0;
      foreach (var methodInfo in collection)
      {
        if (methodInfo.IsGenericMethod && !methodInfo.IsGenericMethodDefinition)
          throw new NotSupportedException ("Cannot serialize closed generic methods. This is not supported.");

        _serializationInfo.AddValue (collectionKey + "[" + index + "].DeclaringType", methodInfo.DeclaringType.AssemblyQualifiedName);

        _serializationInfo.AddValue (collectionKey + "[" + index + "].Name", methodInfo.Name);
        _serializationInfo.AddValue (collectionKey + "[" + index + "].Signature", methodInfo.ToString());

        ++index;
      }
    }
  }
}
