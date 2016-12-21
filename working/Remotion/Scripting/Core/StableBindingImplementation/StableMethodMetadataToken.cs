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

namespace Remotion.Scripting.StableBindingImplementation
{
  /// <summary>
  /// Wrapper around a <see cref="MethodInfo"/>.<see cref="MetadataToken"/> which is the same 
  /// between <see cref="MethodInfo"/>|s coming from related types.
  /// </summary>
  /// <remarks>
  /// <see cref="MemberInfo.MetadataToken"/>|s referring to the the same method  but coming from different,
  /// related types are not the same if the method was overridden in a child type.
  /// </remarks>
  public class StableMethodMetadataToken : StableMetadataToken
  {
    private readonly int _token;
    private readonly MethodInfo _method;

    public StableMethodMetadataToken (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      _token = method.GetBaseDefinition().MetadataToken;
      _method = method;
    }

    public override bool Equals (object obj)
    {
      return Equals (obj as StableMethodMetadataToken);
    }

    public bool Equals (StableMethodMetadataToken other)
    {
      if (ReferenceEquals (null, other))
        return false;
      return other._token == _token;
    }

    public override int GetHashCode ()
    {
      return _token;
    }

    public override string ToString ()
    {
      return String.Format ("({0},{1}({2}))", _token, _method, _method.MetadataToken);
    }
  }
}
