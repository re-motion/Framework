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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class InterfaceIntroductionDefinitionBuilder
  {
    private readonly MixinDefinition _mixin;
    private readonly MemberVisibility _defaultVisibility;
    private readonly HashSet<Type> _nonIntroducedInterfaces;

    public InterfaceIntroductionDefinitionBuilder (MixinDefinition mixin, MemberVisibility defaultVisibility)
    {
      _mixin = mixin;
      _defaultVisibility = defaultVisibility;
      //We intentionally keep the interfaces in the types list to prevent existing code from accidentally introducing serialization stuff on the target.
      _nonIntroducedInterfaces = new HashSet<Type> { typeof(ISerializable), typeof(IDeserializationCallback), typeof(IInitializableMixin) };
      AnalyzeNonIntroducedInterfaces();
    }

    private void AnalyzeNonIntroducedInterfaces ()
    {
      foreach (NonIntroducedAttribute notIntroducedAttribute in _mixin.Type.GetCustomAttributes(typeof(NonIntroducedAttribute), true))
        _nonIntroducedInterfaces.Add(notIntroducedAttribute.NonIntroducedType);
    }

    public void Apply ()
    {
      foreach (Type implementedInterface in _mixin.ImplementedInterfaces)
      {
        if (_nonIntroducedInterfaces.Contains(implementedInterface))
          ApplyNonIntroduced(implementedInterface, true);
        else if (_mixin.TargetClass.ImplementedInterfaces.Contains(implementedInterface))
          ApplyNonIntroduced(implementedInterface, false);
        else
          Apply(implementedInterface);
      }
    }

    public void Apply (Type implementedInterface)
    {
      if (_mixin.TargetClass.ReceivedInterfaces.ContainsKey(implementedInterface))
      {
        MixinDefinition otherIntroducer = _mixin.TargetClass.ReceivedInterfaces[implementedInterface].Implementer;
        string message = string.Format(
            "Two mixins introduce the same interface {0} to base class {1}: {2} and {3}.",
            implementedInterface.GetFullNameSafe(),
            _mixin.TargetClass.FullName,
            otherIntroducer.FullName,
            _mixin.FullName);
        throw new ConfigurationException(message);
      }

      InterfaceIntroductionDefinition introducedInterface = new InterfaceIntroductionDefinition(implementedInterface, _mixin);
      _mixin.InterfaceIntroductions.Add(introducedInterface);
      _mixin.TargetClass.ReceivedInterfaces.Add(introducedInterface);

      AnalyzeIntroducedMembers(introducedInterface);
    }

    public void ApplyNonIntroduced (Type implementedInterface, bool explicitSuppression)
    {
      NonInterfaceIntroductionDefinition nonIntroducedInterface =
          new NonInterfaceIntroductionDefinition(implementedInterface, _mixin, explicitSuppression);
      _mixin.NonInterfaceIntroductions.Add(nonIntroducedInterface);
    }


    private void AnalyzeIntroducedMembers (InterfaceIntroductionDefinition introducedInterface)
    {
      MemberImplementationFinder memberFinder = new MemberImplementationFinder(introducedInterface.InterfaceType, _mixin);
      var specialMethods = new HashSet<MethodInfo>();

      AnalyzeProperties(introducedInterface, memberFinder, specialMethods);
      AnalyzeEvents(introducedInterface, memberFinder, specialMethods);
      AnalyzeMethods(introducedInterface, memberFinder, specialMethods);
    }

    private void AnalyzeProperties (InterfaceIntroductionDefinition introducedInterface, MemberImplementationFinder memberFinder,
         HashSet<MethodInfo> specialMethods)
    {
      foreach (PropertyInfo interfaceProperty in introducedInterface.InterfaceType.GetProperties())
      {
        PropertyDefinition? implementer = memberFinder.FindPropertyImplementation(interfaceProperty);
        CheckMemberImplementationFound(implementer, interfaceProperty);
        MemberVisibility visibility = GetVisibility(implementer.MemberInfo);
        introducedInterface.IntroducedProperties.Add(new PropertyIntroductionDefinition(introducedInterface, interfaceProperty, implementer, visibility));

        MethodInfo? getMethod = interfaceProperty.GetGetMethod();
        if (getMethod != null)
          specialMethods.Add(getMethod);

        MethodInfo? setMethod = interfaceProperty.GetSetMethod();
        if (setMethod != null)
          specialMethods.Add(setMethod);
      }
    }

    private void AnalyzeEvents (InterfaceIntroductionDefinition introducedInterface, MemberImplementationFinder memberFinder,
        HashSet<MethodInfo> specialMethods)
    {
      foreach (EventInfo interfaceEvent in introducedInterface.InterfaceType.GetEvents())
      {
        EventDefinition? implementer = memberFinder.FindEventImplementation(interfaceEvent);
        CheckMemberImplementationFound(implementer, interfaceEvent);
        MemberVisibility visibility = GetVisibility(implementer.MemberInfo);
        introducedInterface.IntroducedEvents.Add(new EventIntroductionDefinition(introducedInterface, interfaceEvent, implementer, visibility));

        // TODO: Assert notnull
        specialMethods.Add(interfaceEvent.GetAddMethod()!);
        specialMethods.Add(interfaceEvent.GetRemoveMethod()!);
      }
    }

    private void AnalyzeMethods (InterfaceIntroductionDefinition introducedInterface, MemberImplementationFinder memberFinder,
        HashSet<MethodInfo> specialMethods)
    {
      foreach (MethodInfo interfaceMethod in introducedInterface.InterfaceType.GetMethods())
      {
        if (!specialMethods.Contains(interfaceMethod))
        {
          MethodDefinition? implementer = memberFinder.FindMethodImplementation(interfaceMethod);
          CheckMemberImplementationFound(implementer, interfaceMethod);
          MemberVisibility visibility = GetVisibility(implementer.MemberInfo);
          introducedInterface.IntroducedMethods.Add(new MethodIntroductionDefinition(introducedInterface, interfaceMethod, implementer, visibility));
        }
      }
    }

    private MemberVisibility GetVisibility (MemberInfo implementingMemberInfo)
    {
      MemberVisibilityAttribute? visibilityAttribute = AttributeUtility.GetCustomAttribute<MemberVisibilityAttribute>(implementingMemberInfo, false);
      if (visibilityAttribute != null)
        return visibilityAttribute.Visibility;
      else
        return _defaultVisibility;
    }

    private void CheckMemberImplementationFound ([NotNull] object? implementation, MemberInfo interfaceMember)
    {
      if (implementation == null)
      {
        string message = string.Format(
            "An implementation for interface member {0}.{1} could not be found in mixin {2}.",
            interfaceMember.DeclaringType,
            interfaceMember.Name,
            _mixin.FullName);
        throw new ConfigurationException(message);
      }
    }
  }
}
