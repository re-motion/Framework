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
  /// Wrapper around a <see cref="MethodInfo"/>.<see cref="MetadataToken"/>.
  /// </summary>
  [Obsolete ("Remotion.Scripting will be removed in the next major release of re-motion. (Version 2.27.2)")]
  public class MethodMetadataToken
  {
    private readonly int _token;
    private readonly MethodInfo _method;

    public MethodMetadataToken (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      _token = method.GetBaseDefinition ().MetadataToken;
      _method = method;
    }

    public int Token { 
      get { return _token; } 
    }

    public MethodInfo MethodInfo
    {
      get { return _method; }
    }

    public override bool Equals (object obj)
    {
      return Equals (obj as MethodMetadataToken);
    }

    public bool Equals (MethodMetadataToken other)
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