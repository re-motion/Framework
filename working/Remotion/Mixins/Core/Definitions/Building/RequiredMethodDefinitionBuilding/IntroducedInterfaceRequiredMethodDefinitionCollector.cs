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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building.RequiredMethodDefinitionBuilding
{
  public class IntroducedInterfaceRequiredMethodDefinitionCollector : IRequiredMethodDefinitionCollector
  {
    public IEnumerable<RequiredMethodDefinition> CreateRequiredMethodDefinitions (RequirementDefinitionBase requirement)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);

      Assertion.IsTrue (requirement.Type.IsInterface);
      
      InterfaceIntroductionDefinition introduction = requirement.TargetClass.ReceivedInterfaces[requirement.Type];
      foreach (EventIntroductionDefinition eventIntroduction in introduction.IntroducedEvents)
      {
        yield return new RequiredMethodDefinition (requirement, eventIntroduction.InterfaceMember.GetAddMethod(), eventIntroduction.ImplementingMember.AddMethod);
        yield return new RequiredMethodDefinition (requirement, eventIntroduction.InterfaceMember.GetRemoveMethod(), eventIntroduction.ImplementingMember.RemoveMethod);
      }

      foreach (PropertyIntroductionDefinition propertyIntroduction in introduction.IntroducedProperties)
      {
        var getMethod = propertyIntroduction.InterfaceMember.GetGetMethod();
        if (getMethod != null)
          yield return new RequiredMethodDefinition (requirement, getMethod, propertyIntroduction.ImplementingMember.GetMethod);

        var setMethod = propertyIntroduction.InterfaceMember.GetSetMethod();
        if (setMethod != null)
          yield return new RequiredMethodDefinition (requirement, setMethod, propertyIntroduction.ImplementingMember.SetMethod);
      }
      
      foreach (MethodIntroductionDefinition methodIntroduction in introduction.IntroducedMethods)
        yield return new RequiredMethodDefinition (requirement, methodIntroduction.InterfaceMember, methodIntroduction.ImplementingMember);
    }
  }
}
