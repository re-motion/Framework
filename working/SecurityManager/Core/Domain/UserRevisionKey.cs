// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  [PermanentGuid (c_permanentGuid)]
  public sealed class UserRevisionKey : IRevisionKey
  {
    public static readonly UserRevisionKey Global = new UserRevisionKey();

    private const string c_permanentGuid = "{7ABCDBE8-B3F8-41FB-826B-990DC3D4CB51}";
    private static readonly Guid s_globalKey = new Guid (c_permanentGuid);
    private readonly string _localKey;

    public UserRevisionKey (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);

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

    public string LocalKey
    {
      get { return _localKey; }
    }

    public override bool Equals (object obj)
    {
      var otherRevisionKey = obj as UserRevisionKey;
      if (otherRevisionKey == null)
        return false;
      return string.Equals (_localKey, otherRevisionKey._localKey);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (s_globalKey, _localKey);
    }
  }
}