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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.TypePipe.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Holds a flattened serialized <see cref="ClassContext"/>.
  /// </summary>
  public class FlatClassContext : IFlatValue
  {
    public static FlatClassContext Create (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull("classContext", classContext);

      var serializer = new FlatClassContextSerializer();
      classContext.Serialize(serializer);

      return new FlatClassContext(serializer.Values);
    }

    private readonly object[] _serializedValues;

    private FlatClassContext (object[] serializedValues)
    {
      _serializedValues = serializedValues;
    }

    public object GetRealValue ()
    {
      return ClassContext.Deserialize(new FlatClassContextDeserializer(_serializedValues));
    }
  }
}
