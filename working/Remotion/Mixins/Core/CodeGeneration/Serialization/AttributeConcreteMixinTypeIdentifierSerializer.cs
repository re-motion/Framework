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
using System.Linq;
using System.Reflection;

namespace Remotion.Mixins.CodeGeneration.Serialization
{
  /// <summary>
  /// Deserializes instances of <see cref="ConcreteMixinTypeIdentifier"/> serialized with <see cref="AttributeConcreteMixinTypeIdentifierSerializer"/>.
  /// </summary>
  public class AttributeConcreteMixinTypeIdentifierSerializer : IConcreteMixinTypeIdentifierSerializer
  {
    private readonly object[] _values = new object[3];

    public object[] Values
    {
      get { return _values; }
    }

    public void AddMixinType (Type mixinType)
    {
      _values[0] = mixinType;
    }

    public void AddOverriders (HashSet<MethodInfo> overriders)
    {
      _values[1] = GetStorableMethodTriplets (overriders);
    }

    public void AddOverridden (HashSet<MethodInfo> overridden)
    {
      _values[2] = GetStorableMethodTriplets (overridden);
    }

    private object[] GetStorableMethodTriplets (HashSet<MethodInfo> methodInfos)
    {
      var triples = from methodInfo in methodInfos
                    select (object) new object[] { CheckNotClosedGeneric (methodInfo).DeclaringType, methodInfo.Name, methodInfo.ToString () };
      return triples.ToArray ();
    }

    private MethodInfo CheckNotClosedGeneric (MethodInfo methodInfo)
    {
      if (methodInfo.IsGenericMethod && !methodInfo.IsGenericMethodDefinition)
        throw new NotSupportedException ("Cannot create an attribute representation of a closed generic method. This is not supported.");

      return methodInfo;
    }
  }
}
