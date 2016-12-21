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
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Serializes a <see cref="ClassContext"/> into an array.
  /// </summary>
  public abstract class ArrayClassContextSerializer : ArrayContextSerializerBase, IClassContextSerializer
  {
    protected ArrayClassContextSerializer ()
        : base(3)
    {
    }

    protected abstract ArrayMixinContextSerializer CreateMixinContextSerializer ();

    public void AddClassType(Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      SetValue (0, type);
    }

    public void AddMixins(IEnumerable<MixinContext> mixinContexts)
    {
      ArgumentUtility.CheckNotNull ("mixinContexts", mixinContexts);
      SetValue (1, mixinContexts.Select (mc => (object) SerializeMixinContext (mc)).ToArray ());
    }

    public void AddComposedInterfaces(IEnumerable<Type> composedInterfaces)
    {
      ArgumentUtility.CheckNotNull ("composedInterfaces", composedInterfaces);
      SetValue (2, composedInterfaces.ToArray ());
    }

    private object[] SerializeMixinContext (MixinContext m)
    {
      var serializer = CreateMixinContextSerializer();
      m.Serialize (serializer);
      return serializer.Values;
    }
  }
}
