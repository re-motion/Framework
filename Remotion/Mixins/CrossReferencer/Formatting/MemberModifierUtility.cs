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

namespace Remotion.Mixins.CrossReferencer.Formatting
{
  public class MemberModifierUtility
  {
    private readonly TypeModifierUtility _typeModifierUtility = new();

    public bool IsOverriddenMember (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);

      var methodInfo = memberInfo as MethodInfo;
      if (methodInfo != null)
        return IsOverriddenMethod(methodInfo);

      var propertyInfo = memberInfo as PropertyInfo;
      if (propertyInfo != null)
      {
        return IsOverriddenMethod(propertyInfo.GetGetMethod(true))
               || IsOverriddenMethod(propertyInfo.GetSetMethod(true));
      }

      var eventInfo = memberInfo as EventInfo;
      if (eventInfo != null)
      {
        return IsOverriddenMethod(eventInfo.GetAddMethod(true))
               || IsOverriddenMethod(eventInfo.GetRaiseMethod(true))
               || IsOverriddenMethod(eventInfo.GetRemoveMethod(true));
      }

      return false;
    }

    public string GetMemberModifiers (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);

      switch (memberInfo.MemberType)
      {
        case MemberTypes.Method:
        case MemberTypes.Constructor:
          return GetMethodModifiers(memberInfo, memberInfo);
        case MemberTypes.Field:
          return GetFieldModifiers((FieldInfo)memberInfo);

        case MemberTypes.Property:
          var propertyInfo = (PropertyInfo)memberInfo;
          var propertyAccessorMethod = Assertion.IsNotNull(propertyInfo.GetGetMethod(true) ?? propertyInfo.GetSetMethod(true), "propertyInfo has no accessor");
          return GetMethodModifiers(propertyAccessorMethod, memberInfo);

        case MemberTypes.Event:
          var eventInfo = (EventInfo)memberInfo;
          var eventAccessorMethod =Assertion.IsNotNull(eventInfo.GetAddMethod(true), "eventInfo.GetAddMethod(true) != null");
          return GetMethodModifiers( eventAccessorMethod, memberInfo);

        case MemberTypes.NestedType:
          return _typeModifierUtility.GetTypeModifiers((Type)memberInfo);

        case MemberTypes.Custom:
        case MemberTypes.TypeInfo:
          return "TODO special MemberTypes";

        default:
          throw new Exception("unknown member type");
      }
    }


    private string GetMethodModifiers (MemberInfo methodFieldOrConstructor, MemberInfo memberInfoForOverride)
    {
      var methodInfo = (MethodBase)methodFieldOrConstructor;
      var modifiers = "";

      if (methodInfo.IsPublic)
        modifiers = "public";
      else if (methodInfo.IsFamily)
        modifiers = "protected";
      else if (methodInfo.IsFamilyOrAssembly)
        modifiers = "protected internal";
      else if (methodInfo.IsAssembly)
        modifiers = "internal";
      else if (methodInfo.IsPrivate)
        modifiers = "private";

      if (methodFieldOrConstructor is MethodInfo)
      {
        var isOverriddenMember = IsOverriddenMember(memberInfoForOverride);

        if (methodInfo.IsAbstract)
          modifiers += " abstract";
        else if (methodInfo.IsFinal && (!methodInfo.IsVirtual || isOverriddenMember))
          modifiers += " sealed";
        if (isOverriddenMember)
          modifiers += " override";
        if (!isOverriddenMember
            && !methodInfo.IsAbstract
            && !methodInfo.IsFinal
            && methodInfo.IsVirtual)
          modifiers += " virtual";

        // explicit interface implementation
        if (methodInfo.IsHideBySig
            && methodInfo.IsPrivate
            && methodInfo.IsFinal
            && methodInfo.IsVirtual)
          return "";
      }

      if (methodInfo.IsStatic)
        modifiers += " static";

      return modifiers;
    }

    private string GetFieldModifiers (FieldInfo methodInfo)
    {
      var modifiers = "";

      if (methodInfo.IsPublic)
        modifiers = "public";
      else if (methodInfo.IsFamily)
        modifiers = "protected";
      else if (methodInfo.IsFamilyOrAssembly)
        modifiers = "protected internal";
      else if (methodInfo.IsAssembly)
        modifiers = "internal";
      else if (methodInfo.IsPrivate)
        modifiers = "private";

      if (methodInfo.IsStatic)
        modifiers += " static";

      if (methodInfo.IsInitOnly)
        modifiers += " readonly";

      return modifiers;
    }

    private bool IsOverriddenMethod (MethodInfo? methodInfo)
    {
      return methodInfo != null && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType;
    }
  }
}
