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

namespace Remotion.SecurityManager.Domain
{
  [PermanentGuid(c_permanentGuid)]
  public sealed class UserNamesRevisionKey : IRevisionKey
  {
    public static readonly UserNamesRevisionKey Global = new UserNamesRevisionKey();

    private const string c_permanentGuid = "{40024F99-76E0-4DDE-BFB5-3E7CB9A50E85}";
    private static readonly Guid s_globalKey = new Guid(c_permanentGuid);

    private UserNamesRevisionKey ()
    {
    }

    public Guid GlobalKey
    {
      get { return s_globalKey; }
    }

    public string? LocalKey
    {
      get { return null; }
    }

    public override bool Equals (object? obj)
    {
      return obj is UserNamesRevisionKey;
    }

    public override int GetHashCode ()
    {
      return s_globalKey.GetHashCode();
    }
  }
}
