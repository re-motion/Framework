﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer
{
  public class InvolvedTypeMember : IVisitableInvolved
  {
    private static readonly IEqualityComparer<MemberInfo> EqualityComparer = new MemberDefinitionEqualityComparer();

    public InvolvedTypeMember (
        MemberInfo memberInfo,
        MemberDefinitionBase? targetMemberDefinition,
        IReadOnlyCollection<MemberDefinitionBase>? mixinMemberDefinitions)
    {
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);

      MemberInfo = new OverridingMemberInfo(memberInfo);
      AddSubMemberInfos(memberInfo);

      SetMixinOverride(targetMemberDefinition);
      AddTargetOverrides(mixinMemberDefinitions);

      MixinMemberDefinitions = mixinMemberDefinitions ?? Array.Empty<MemberDefinitionBase>();
      TargetMemberDefinition = targetMemberDefinition;

      OverridingMixinTypes = GetOverridingMixinTypes();
      OverridingTargetTypes = GetOverridingTargetTypes();
    }

    private void AddOverriddenMember (MemberInfo overriddenMember, OverridingMemberInfo.OverrideType type)
    {
      MemberInfo.AddOverriddenMember(overriddenMember, type);

      if (overriddenMember.MemberType == MemberTypes.Property)
      {
        var overriddenProperty = (PropertyInfo)overriddenMember;

        var overriddenMethodGetAccessor = overriddenProperty.GetGetMethod();
        if (overriddenMethodGetAccessor != null && _subMemberInfos.ContainsKey(SubMemberType.PropertyGet))
          _subMemberInfos[SubMemberType.PropertyGet].AddOverriddenMember(overriddenMethodGetAccessor, type);

        var overriddenMethodSetAccessor = overriddenProperty.GetSetMethod();
        if (overriddenMethodSetAccessor != null && _subMemberInfos.ContainsKey(SubMemberType.PropertySet))
          _subMemberInfos[SubMemberType.PropertyGet].AddOverriddenMember(overriddenMethodSetAccessor, type);
      }

      if (overriddenMember.MemberType == MemberTypes.Event)
      {
        var overriddenEvent = (EventInfo)overriddenMember;

        if (_subMemberInfos.ContainsKey(SubMemberType.EventAdd))
          _subMemberInfos[SubMemberType.PropertyGet].AddOverriddenMember(Assertion.IsNotNull(overriddenEvent.GetAddMethod(), "overriddenEvent.GetAddMethod() != null"), type);

        if (_subMemberInfos.ContainsKey(SubMemberType.EventRemove))
          _subMemberInfos[SubMemberType.PropertyGet].AddOverriddenMember(Assertion.IsNotNull(overriddenEvent.GetRemoveMethod(), "overriddenEvent.GetRemoveMethod() != null"), type);
      }
    }

    private void AddTargetOverrides (IReadOnlyCollection<MemberDefinitionBase>? mixinMemberDefinitions)
    {
      if (mixinMemberDefinitions == null || !mixinMemberDefinitions.Any())
        return;

      var overriddenMembers = mixinMemberDefinitions
          .Select(m => m.BaseAsMember)
          .Where(m => m != null)
          .Select(m => m!.MemberInfo)
          .Distinct();

      foreach (var overriddenMember in overriddenMembers)
        AddOverriddenMember(overriddenMember, OverridingMemberInfo.OverrideType.Target);
    }

    private void SetMixinOverride (MemberDefinitionBase? targetMemberDefinition)
    {
      if (targetMemberDefinition == null)
        return;

      var baseMember = targetMemberDefinition.BaseAsMember;

      if (baseMember == null)
        return;

      AddOverriddenMember(baseMember.MemberInfo, OverridingMemberInfo.OverrideType.Mixin);
    }

    private void AddSubMemberInfos (MemberInfo memberInfo)
    {
      if (memberInfo.MemberType == MemberTypes.Property)
      {
        var propInfo = ((PropertyInfo)memberInfo);

        var getMethod = propInfo.GetGetMethod(nonPublic: true);
        if (getMethod != null)
          _subMemberInfos.Add(SubMemberType.PropertyGet, new OverridingMemberInfo(getMethod));

        var setMethod = propInfo.GetSetMethod(nonPublic: true);
        if (setMethod != null)
          _subMemberInfos.Add(SubMemberType.PropertySet, new OverridingMemberInfo(setMethod));
      }

      if (memberInfo.MemberType == MemberTypes.Event)
      {
        var eventInfo = ((EventInfo)memberInfo);

        var addMethod = eventInfo.GetAddMethod(nonPublic: true);
        if (addMethod != null)
          _subMemberInfos.Add(SubMemberType.EventAdd, new OverridingMemberInfo(addMethod));

        var removeMethod = eventInfo.GetRemoveMethod(nonPublic: true);
        if (removeMethod != null)
          _subMemberInfos.Add(SubMemberType.EventRemove, new OverridingMemberInfo(removeMethod));
      }
    }

    private IEnumerable<Type> GetOverridingMixinTypes ()
    {
      if (TargetMemberDefinition == null)
        return Enumerable.Empty<Type>();

      return
          TargetMemberDefinition.Overrides.Select(
              o => o.DeclaringClass.Type);
    }

    private IEnumerable<Type> GetOverridingTargetTypes ()
    {
      if (!MixinMemberDefinitions.Any())
        return Enumerable.Empty<Type>();

      return
          MixinMemberDefinitions.SelectMany(m => m.Overrides).Select(
              o => o.DeclaringClass.Type);
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

    public IEnumerable<OverridingMemberInfo> SubMemberInfos => _subMemberInfos.Values;

    public MemberDefinitionBase? TargetMemberDefinition { get; private set; }
    public IReadOnlyCollection<MemberDefinitionBase> MixinMemberDefinitions { get; private set; }

    public IEnumerable<Type> OverriddenMembersDeclaringTypes
    {
      get
      {
        return
            MemberInfo.OverriddenMixinMembers.Select(m => Assertion.IsNotNull(m.DeclaringType, "OverriddenMixinMembers: m.DeclaringType != null"))
                .Concat(MemberInfo.OverriddenTargetMembers.Select(m => Assertion.IsNotNull(m.DeclaringType, "OverriddenTargetMembers: m.DeclaringType != null")));
      }
    }

    public IEnumerable<Type> OverridingMixinTypes { get; private set; }
    public IEnumerable<Type> OverridingTargetTypes { get; private set; }

    public void Accept (IInvolvedVisitor involvedVisitor)
    {
      involvedVisitor.Visit(this);
    }

    public override bool Equals (object? obj)
    {
      var other = obj as InvolvedTypeMember;
      return other != null && EqualityComparer.Equals(MemberInfo, other.MemberInfo);
    }

    public override int GetHashCode ()
    {
      return MemberInfo.GetHashCode();
    }

    public override string ToString ()
    {
      return string.Format("{0}: {1}", typeof(InvolvedTypeMember).FullName, MemberInfo);
    }
  }
}
