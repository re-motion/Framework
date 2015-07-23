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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Serialization
{
  /// <summary>
  /// Serializes a <see cref="MixinContextOrigin"/> into an array.
  /// </summary>
  public abstract class ArrayMixinContextOriginSerializer : ArrayContextSerializerBase, IMixinContextOriginSerializer
  {
    protected ArrayMixinContextOriginSerializer()
        : base(3)
    {
    }

    public void AddKind (string kind)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("kind", kind);
      SetValue (0, kind);
    }

    public void AddAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      SetValue (1, assembly);
    }

    public void AddLocation (string location)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("location", location);
      SetValue (2, location);
    }
  }
}