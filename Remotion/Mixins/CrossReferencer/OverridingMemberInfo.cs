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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer
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

    public IEnumerable<MemberInfo> OverriddenTargetMembers => _overriddenTargetMembers;

    private readonly IList<MemberInfo> _overriddenMixinMembers = new List<MemberInfo>();

    public IEnumerable<MemberInfo> OverriddenMixinMembers => _overriddenMixinMembers;

    public OverridingMemberInfo (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);
      _memberInfo = memberInfo;
    }

    public void AddOverriddenMember (MemberInfo memberInfo, OverrideType type)
    {
      switch (type)
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

    public static implicit operator MemberInfo (OverridingMemberInfo o)
    {
      return o._memberInfo;
    }

    public static implicit operator OverridingMemberInfo (MemberInfo m)
    {
      return new OverridingMemberInfo(m);
    }
  }
}
