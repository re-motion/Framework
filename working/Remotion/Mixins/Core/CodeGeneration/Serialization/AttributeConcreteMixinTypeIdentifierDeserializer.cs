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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.Serialization
{
  /// <summary>
  /// Serializes instances of <see cref="ConcreteMixinTypeIdentifier"/> into a format that can be used as a custom attribute parameter.
  /// </summary>
  public class AttributeConcreteMixinTypeIdentifierDeserializer : IConcreteMixinTypeIdentifierDeserializer
  {
    private readonly object[] _values;

    public AttributeConcreteMixinTypeIdentifierDeserializer (object[] values)
    {
      ArgumentUtility.CheckNotNull ("values", values);
      _values = values;
    }

    public Type GetMixinType ()
    {
      return (Type) _values[0];
    }

    public HashSet<MethodInfo> GetOverriders ()
    {
      var overriderArray = (object[]) _values[1];
      return DeserializeTriplets (overriderArray);
    }

    public HashSet<MethodInfo> GetOverridden ()
    {
      var overriddenArray = (object[]) _values[2];
      return DeserializeTriplets (overriddenArray);
    }

    private HashSet<MethodInfo> DeserializeTriplets (object[] overriderArray)
    {
      return new HashSet<MethodInfo> (from object[] triplet in overriderArray
                                      let declaringType = (Type) triplet[0]
                                      let name = (string) triplet[1]
                                      let signature = (string) triplet[2]
                                      select MethodResolver.ResolveMethod (declaringType, name, signature));
    }
  }
}
