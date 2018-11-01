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

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Deserializes the data serialized by a <see cref="ArrayMixinContextSerializer"/>.
  /// </summary>
  public abstract class ArrayMixinContextDeserializer : ArrayContextDeserializerBase, IMixinContextDeserializer
  {
    protected abstract ArrayMixinContextOriginDeserializer CreateMixinContextOriginDeserializer (object[] values);

    protected ArrayMixinContextDeserializer (object[] values)
        : base (values, 5)
    {
    }

    public Type GetMixinType()
    {
      return GetValue<Type> (0);
    }

    public MixinKind GetMixinKind()
    {
      return GetValue<MixinKind> (1);
    }

    public MemberVisibility GetIntroducedMemberVisibility()
    {
      return GetValue<MemberVisibility> (2);
    }

    public IEnumerable<Type> GetExplicitDependencies()
    {
      return GetValue<Type[]> (3);
    }

    public MixinContextOrigin GetOrigin ()
    {
      var originDeserializer = CreateMixinContextOriginDeserializer (GetValue<object[]> (4));
      return MixinContextOrigin.Deserialize (originDeserializer);
    }
  }
}
