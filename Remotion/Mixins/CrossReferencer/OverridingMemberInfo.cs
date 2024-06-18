// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class OverridingMemberInfo
  {
    private readonly MemberInfo _memberInfo;

    public enum OverrideType
    {
      Target,
      Mixin
    }

    private readonly IList<MemberInfo> _overriddenTargetMembers = new List<MemberInfo>();
    public IEnumerable<MemberInfo> OverriddenTargetMembers { get { return _overriddenTargetMembers; } }
    private readonly IList<MemberInfo> _overriddenMixinMembers = new List<MemberInfo> ();
    public IEnumerable<MemberInfo> OverriddenMixinMembers { get { return _overriddenMixinMembers; } }
    public OverridingMemberInfo(MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
      _memberInfo = memberInfo;
    }

    public void AddOverriddenMember(MemberInfo memberInfo, OverrideType type)
    {
      switch(type)
      {
        case OverrideType.Target:
          _overriddenTargetMembers.Add(memberInfo);
          break;
        case OverrideType.Mixin:
          _overriddenMixinMembers.Add(memberInfo);
          break;
        default:
          throw new ArgumentOutOfRangeException("type");
      }
    }

    public static implicit operator MemberInfo(OverridingMemberInfo o)
    {
      return o._memberInfo;
    }

    public static implicit operator OverridingMemberInfo(MemberInfo m)
    {
      return new OverridingMemberInfo(m);
    }
  }
}