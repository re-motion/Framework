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
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  [PermanentGuid(c_permanentGuid)]
  public sealed class UserRevisionKey : IRevisionKey
  {
    public static readonly UserRevisionKey Global = new UserRevisionKey();

    private const string c_permanentGuid = "{7ABCDBE8-B3F8-41FB-826B-990DC3D4CB51}";
    private static readonly Guid s_globalKey = new Guid(c_permanentGuid);
    private readonly string? _localKey;

    public UserRevisionKey (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("userName", userName);

      _localKey = userName;
    }

    private UserRevisionKey ()
    {
      _localKey = null;
    }

    public Guid GlobalKey
    {
      get { return s_globalKey; }
    }

    public string? LocalKey
    {
      get { return _localKey; }
    }

    public override bool Equals (object? obj)
    {
      var otherRevisionKey = obj as UserRevisionKey;
      if (otherRevisionKey == null)
        return false;
      return string.Equals(_localKey, otherRevisionKey._localKey);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode(s_globalKey, _localKey);
    }
  }
}
