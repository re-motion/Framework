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

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Deserializes the data serialized by a <see cref="ArrayClassContextSerializer"/>.
  /// </summary>
  public abstract class ArrayClassContextDeserializer : ArrayContextDeserializerBase, IClassContextDeserializer
  {
    protected abstract ArrayMixinContextDeserializer CreateMixinContextDeserializer (object[] values);

    protected ArrayClassContextDeserializer (object[] values) : base (values, 3)
    {
    }

    public Type GetClassType ()
    {
      return GetValue<Type> (0);
    }

    public IEnumerable<MixinContext> GetMixins()
    {
      var mixins = GetValue<object[]> (1);
      return mixins.Select (oa => MixinContext.Deserialize (CreateMixinContextDeserializer ((object[]) oa)));
    }

    public IEnumerable<Type> GetComposedInterfaces()
    {
      return GetValue<Type[]> (2);
    }
  }
}
