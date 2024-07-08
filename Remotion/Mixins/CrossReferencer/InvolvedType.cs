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
using Remotion.Mixins.CrossReferencer.Reflectors;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer
{
  public class InvolvedType : IVisitableInvolved
  {
    private readonly Type _realType;
    private IEnumerable<InvolvedTypeMember> _members;
    private ReflectedObject /* ClassContext */ _classContext;
    private ReflectedObject /* TargetClassDefinition */ _targetClassDefintion;
    private readonly IDictionary<InvolvedType, ReflectedObject> _targetTypes = new Dictionary<InvolvedType, ReflectedObject>();

    public InvolvedType (Type realType)
    {
      ArgumentUtility.CheckNotNull("realType", realType);

      _realType = realType;
    }

    public Type Type => _realType;

    public IEnumerable<InvolvedTypeMember> Members
    {
      get
      {
        if (_members == null)
            // TODO remove constructors
          _members = _realType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
              .Where(m => !HasSpecialName(m))
              .OrderBy(m => m.Name)
              .Select(
                  m =>
                  {
                    ReflectedObject targetMemberDefinition;
                    List<ReflectedObject> mixinMemberDefinitions;
                    TargetMemberDefinitions.TryGetValue(m, out targetMemberDefinition);
                    MixinMemberDefinitions.TryGetValue(m, out mixinMemberDefinitions);
                    return new InvolvedTypeMember(m, targetMemberDefinition, mixinMemberDefinitions);
                  });
        return _members;
      }
    }

    public bool IsTarget => _classContext != null;

    public bool IsMixin => _targetTypes.Count > 0;

    public ReflectedObject /* ClassContext */ ClassContext
    {
      get { return _classContext; }
      set { _classContext = value; }
    }

    public bool HasTargetClassDefintion => _targetClassDefintion != null;

    public ReflectedObject /* TargetClassDefinition */ TargetClassDefinition
    {
      get { return _targetClassDefintion; }
      set { _targetClassDefintion = value; }
    }

    public IDictionary<InvolvedType, ReflectedObject> TargetTypes
    {
      get { return _targetTypes; }
    }

    private IDictionary<MemberInfo, ReflectedObject> _targetMemberDefinitions;

    public IDictionary<MemberInfo, ReflectedObject> TargetMemberDefinitions
    {
      get
      {
        if (_targetMemberDefinitions == null)
          _targetMemberDefinitions = TargetClassDefinition == null
              ? new Dictionary<MemberInfo, ReflectedObject>()
              : TargetClassDefinition.CallMethod("GetAllMembers").ToDictionary(m => m.GetProperty("MemberInfo").To<MemberInfo>(), m => m, new MemberDefinitionEqualityComparer());

        return _targetMemberDefinitions;
      }
    }

    private IDictionary<MemberInfo, List<ReflectedObject>> _mixinMemberDefinitions;

    public IDictionary<MemberInfo, List<ReflectedObject>> MixinMemberDefinitions
    {
      get
      {
        if (_mixinMemberDefinitions == null)
          _mixinMemberDefinitions = TargetTypes.Values.Where(t => t != null).SelectMany(t => t.CallMethod("GetAllMembers"))
              .GroupBy(m => m.GetProperty("MemberInfo").To<MemberInfo>()).ToDictionary(m => m.Key, m => m.ToList(), new MemberDefinitionEqualityComparer());

        return _mixinMemberDefinitions;
      }
    }

    public void Accept (IInvolvedVisitor involvedVisitor)
    {
      foreach (var member in Members)
        member.Accept(involvedVisitor);
    }

    private static bool HasSpecialName (MemberInfo memberInfo)
    {
      if (memberInfo.MemberType == MemberTypes.Method)
      {
        var methodInfo = memberInfo as MethodInfo;
        if (methodInfo == null)
          return false;

        var methodName = methodInfo.Name;
        // only explicit interface implementations contain a '.'
        if (methodName.Contains('.'))
        {
          var parts = methodName.Split('.');
          var partCount = parts.Length;
          methodName = parts[partCount - 1];
        }

        return
            (methodInfo.IsSpecialName
             && (methodName.StartsWith("add_")
                 || methodName.StartsWith("remove_")
                 || methodName.StartsWith("get_")
                 || methodName.StartsWith("set_")
             )
            );
      }

      return false;
    }

    public override bool Equals (object obj)
    {
      var other = obj as InvolvedType;
      return other != null
             && Equals(other._realType, _realType)
             && Equals(other._classContext, _classContext)
             && Equals(other._targetClassDefintion, _targetClassDefintion)
             && other._targetTypes.SequenceEqual(_targetTypes);
    }

    public override int GetHashCode ()
    {
      int hashCode = _realType.GetHashCode();
      Rotate(ref hashCode);
      hashCode ^= _classContext == null ? 0 : _classContext.GetHashCode();
      Rotate(ref hashCode);
      hashCode ^= _targetClassDefintion == null ? 0 : _targetClassDefintion.GetHashCode();
      Rotate(ref hashCode);
      hashCode ^= _targetTypes.Aggregate(0, (current, typeAndMixinDefintionPair) => current ^ typeAndMixinDefintionPair.GetHashCode());

      return hashCode;
    }

    public override string ToString ()
    {
      return String.Format("{0}, isTarget: {1}, isMixin: {2}, # of targets: {3}", _realType, IsTarget, IsMixin, _targetTypes.Count);
    }

    private static void Rotate (ref int value)
    {
      const int rotateBy = 11;
      value = (value << rotateBy) ^ (value >> (32 - rotateBy));
    }
  }
}