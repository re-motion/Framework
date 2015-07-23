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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.Serialization
{
  /// <summary>
  /// Deserializes instances of <see cref="ConcreteMixinTypeIdentifier"/> serialized with 
  /// <see cref="SerializationInfoConcreteMixinTypeIdentifierSerializer"/>.
  /// </summary>
  public class SerializationInfoConcreteMixinTypeIdentifierDeserializer : IConcreteMixinTypeIdentifierDeserializer
  {
    private readonly SerializationInfo _serializationInfo;
    private readonly string _key;

    public SerializationInfoConcreteMixinTypeIdentifierDeserializer (SerializationInfo serializationInfo, string key)
    {
      ArgumentUtility.CheckNotNull ("serializationInfo", serializationInfo);
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      _serializationInfo = serializationInfo;
      _key = key;
    }

    public Type GetMixinType ()
    {
      return Type.GetType (_serializationInfo.GetString (_key + ".MixinType"));
    }

    public HashSet<MethodInfo> GetOverriders ()
    {
      return DeserializeMethods (_key + ".Overriders");
    }

    public HashSet<MethodInfo> GetOverridden ()
    {
      return DeserializeMethods (_key + ".Overridden");
    }

    private HashSet<MethodInfo> DeserializeMethods (string collectionKey)
    {
      var methods = new HashSet<MethodInfo> ();
      var count = _serializationInfo.GetInt32 (collectionKey + ".Count");

      for (int i = 0; i < count; ++i)
      {
        var methodDeclaringType = Type.GetType (_serializationInfo.GetString (collectionKey + "[" + i + "].DeclaringType"));

        var name = _serializationInfo.GetString (collectionKey + "[" + i + "].Name");
        var signature = _serializationInfo.GetString (collectionKey + "[" + i + "].Signature");

        var method = MethodResolver.ResolveMethod (methodDeclaringType, name, signature);
        methods.Add (method);
      }
      return methods;
    }
  }
}
