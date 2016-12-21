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

namespace Remotion.Mixins.Definitions.Building.RequiredMethodDefinitionBuilding
{
  public class ImplementedInterfaceRequiredMethodDefinitionCollector : IRequiredMethodDefinitionCollector
  {
    private readonly TargetClassDefinition _targetClassDefinition;
    private readonly Dictionary<MethodInfo, MethodDefinition> _allTargetClassMethods;

    public ImplementedInterfaceRequiredMethodDefinitionCollector (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      _targetClassDefinition = targetClassDefinition;
      _allTargetClassMethods = new Dictionary<MethodInfo, MethodDefinition> ();
      
      foreach (MethodDefinition methodDefinition in targetClassDefinition.GetAllMethods ())
        _allTargetClassMethods.Add (methodDefinition.MethodInfo, methodDefinition);
    }

    public IEnumerable<RequiredMethodDefinition> CreateRequiredMethodDefinitions (RequirementDefinitionBase requirement)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);

      Assertion.IsTrue (requirement.Type.IsInterface);
      Assertion.IsTrue (requirement.TargetClass == _targetClassDefinition);
      Assertion.IsTrue (requirement.TargetClass.ImplementedInterfaces.Contains (requirement.Type));

      InterfaceMapping interfaceMapping = _targetClassDefinition.GetAdjustedInterfaceMap (requirement.Type);
      for (int i = 0; i < interfaceMapping.InterfaceMethods.Length; ++i)
      {
        var interfaceMethod = interfaceMapping.InterfaceMethods[i];
        var implementingMethod = _allTargetClassMethods[interfaceMapping.TargetMethods[i]];

        yield return new RequiredMethodDefinition (requirement, interfaceMethod, implementingMethod);
      }
    }
  }
}
