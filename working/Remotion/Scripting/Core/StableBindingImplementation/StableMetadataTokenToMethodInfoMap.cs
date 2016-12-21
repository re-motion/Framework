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
using Remotion.Utilities;

namespace Remotion.Scripting.StableBindingImplementation
{
  /// <summary>
  /// Map between <see cref="StableMethodMetadataToken"/> and <see cref="MethodInfo"/>.
  /// </summary>
  public class StableMetadataTokenToMethodInfoMap
  {
    private readonly Dictionary<StableMethodMetadataToken, MethodInfo> _map = new Dictionary<StableMethodMetadataToken, MethodInfo>();

    public StableMetadataTokenToMethodInfoMap (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _map = type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToDictionary (
        mi => new StableMethodMetadataToken(mi), mi => mi);
    }


    public MethodInfo GetMethod (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      return GetMethod (new StableMethodMetadataToken (method));
    }

    public MethodInfo GetMethod (StableMethodMetadataToken stableMethodMetadataToken)
    {
      ArgumentUtility.CheckNotNull ("stableMethodMetadataToken", stableMethodMetadataToken);
      MethodInfo correspondingMethod;
      _map.TryGetValue (stableMethodMetadataToken, out correspondingMethod);
      return correspondingMethod;
    }
  }
}
