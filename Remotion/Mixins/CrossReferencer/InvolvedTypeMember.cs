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
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class InvolvedTypeMember : IVisitableInvolved
  {
    private static readonly IEqualityComparer<MemberInfo> EqualityComparer = new MemberDefinitionEqualityComparer();

    public InvolvedTypeMember(MemberInfo memberInfo, ReflectedObject targetMemberDefinition,
                              IEnumerable<ReflectedObject> mixinMemberDefinitions)
    {
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);

      MemberInfo = new OverridingMemberInfo(memberInfo);
      AddSubMemberInfos(memberInfo);

      SetMixinOverride(targetMemberDefinition);
      AddTargetOverrides(mixinMemberDefinitions);

      MixinMemberDefinitions = mixinMemberDefinitions ?? Enumerable.Empty<ReflectedObject>();
      TargetMemberDefinition = targetMemberDefinition;

      OverridingMixinTypes = GetOverridingMixinTypes();
      OverridingTargetTypes = GetOverridingTargetTypes();
    }

    private void AddOverriddenMember(MemberInfo overriddenMember, OverridingMemberInfo.OverrideType type)
    {
      MemberInfo.AddOverriddenMember (overriddenMember, type);

      if (overriddenMember.MemberType == MemberTypes.Property)
      {
        var overriddenProperty = (PropertyInfo) overriddenMember;

        if (overriddenProperty.GetGetMethod () != null && _subMemberInfos.ContainsKey (SubMemberType.PropertyGet))
          _subMemberInfos[SubMemberType.PropertyGet].AddOverriddenMember (overriddenProperty.GetGetMethod (), type);

        if (overriddenProperty.GetSetMethod () != null && _subMemberInfos.ContainsKey (SubMemberType.PropertySet))
          _subMemberInfos[SubMemberType.PropertyGet].AddOverriddenMember (overriddenProperty.GetSetMethod (), type);
      }

      if (overriddenMember.MemberType == MemberTypes.Event)
      {
        var overriddenEvent = (EventInfo) overriddenMember;

        if (_subMemberInfos.ContainsKey (SubMemberType.EventAdd))
          _subMemberInfos[SubMemberType.PropertyGet].AddOverriddenMember(overriddenEvent.GetAddMethod(), type);

        if (_subMemberInfos.ContainsKey (SubMemberType.EventRemove))
          _subMemberInfos[SubMemberType.PropertyGet].AddOverriddenMember(overriddenEvent.GetRemoveMethod(), type);
      }
    }

    private void AddTargetOverrides(IEnumerable<ReflectedObject> mixinMemberDefinitions)
    {
      if (mixinMemberDefinitions == null || !mixinMemberDefinitions.Any ())
        return;

      var overriddenMembers = mixinMemberDefinitions
        .Select(m => m.GetProperty("BaseAsMember"))
        .Where(m => m != null)
        .Select(m => m.GetProperty("MemberInfo").To<MemberInfo>())
        .Distinct();

      foreach (var overriddenMember in overriddenMembers)
        AddOverriddenMember(overriddenMember, OverridingMemberInfo.OverrideType.Target);
    }

    private void SetMixinOverride(ReflectedObject targetMemberDefinition)
    {
      if (targetMemberDefinition == null)
        return;

      var baseMember = targetMemberDefinition.GetProperty ("BaseAsMember");

      if (baseMember == null)
        return;

      AddOverriddenMember(baseMember.GetProperty ("MemberInfo").To<MemberInfo> (), OverridingMemberInfo.OverrideType.Mixin);
    }

    private void AddSubMemberInfos (MemberInfo memberInfo)
    {
      if (memberInfo.MemberType == MemberTypes.Property)
      {
        var propInfo = ((PropertyInfo) memberInfo);

        var getMethod = propInfo.GetGetMethod (nonPublic: true);
        if (getMethod != null)
          _subMemberInfos.Add (SubMemberType.PropertyGet, new OverridingMemberInfo (getMethod));

        var setMethod = propInfo.GetSetMethod (nonPublic: true);
        if (setMethod != null)
          _subMemberInfos.Add (SubMemberType.PropertySet, new OverridingMemberInfo (setMethod));
      }

      if (memberInfo.MemberType == MemberTypes.Event)
      {
        var eventInfo = ((EventInfo) memberInfo);

        var addMethod = eventInfo.GetAddMethod (nonPublic: true);
        if (addMethod != null)
          _subMemberInfos.Add (SubMemberType.EventAdd, new OverridingMemberInfo (addMethod));

        var removeMethod = eventInfo.GetRemoveMethod (nonPublic: true);
        if (removeMethod != null)
          _subMemberInfos.Add (SubMemberType.EventRemove, new OverridingMemberInfo (removeMethod));
      }
    }

    private IEnumerable<Type> GetOverridingMixinTypes()
    {
      if (TargetMemberDefinition == null)
        return Enumerable.Empty<Type>();

      return
        TargetMemberDefinition.GetProperty("Overrides").Select(
          o => o.GetProperty("DeclaringClass").GetProperty("Type").To<Type>());
    }

    private IEnumerable<Type> GetOverridingTargetTypes()
    {
      if (!MixinMemberDefinitions.Any())
        return Enumerable.Empty<Type>();

      return
        MixinMemberDefinitions.SelectMany(m => m.GetProperty("Overrides")).Select(
          o => o.GetProperty("DeclaringClass").GetProperty("Type").To<Type>());
    }

    private enum SubMemberType
    {
      PropertyGet,
      PropertySet,
      EventAdd,
      EventRemove
    }

    private readonly IDictionary<SubMemberType, OverridingMemberInfo> _subMemberInfos =
      new Dictionary<SubMemberType, OverridingMemberInfo>(); 

    public OverridingMemberInfo MemberInfo { get; private set; }

    public IEnumerable<OverridingMemberInfo> SubMemberInfos
    {
      get { return _subMemberInfos.Values; }
    }

    public ReflectedObject TargetMemberDefinition { get; private set; }
    public IEnumerable<ReflectedObject> MixinMemberDefinitions { get; private set; }

    public IEnumerable<Type> OverriddenMembersDeclaringTypes
    {
      get
      {
        return
          MemberInfo.OverriddenMixinMembers.Select(m => m.DeclaringType).Concat(
            MemberInfo.OverriddenTargetMembers.Select(m => m.DeclaringType));
      }
    } 

    public IEnumerable<Type> OverridingMixinTypes { get; private set; }
    public IEnumerable<Type> OverridingTargetTypes { get; private set; }

    public void Accept(IInvolvedVisitor involvedVisitor)
    {
      involvedVisitor.Visit(this);
    }

    public override bool Equals(object obj)
    {
      var other = obj as InvolvedTypeMember;
      return other != null && EqualityComparer.Equals(MemberInfo, other.MemberInfo);
    }

    public override int GetHashCode()
    {
      return MemberInfo.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format("{0}: {1}", typeof (InvolvedTypeMember).FullName, MemberInfo);
    }
  }
}