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
using Remotion.Mixins.Definitions.Building.RequiredMethodDefinitionBuilding;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class RequiredMethodDefinitionBuilder
  {
    private readonly ImplementedInterfaceRequiredMethodDefinitionCollector _implementedInterfaceMethodCollector;
    private readonly IntroducedInterfaceRequiredMethodDefinitionCollector _introducedInterfaceMethodCollector;
    private readonly DuckTypingRequiredMethodDefinitionCollector _duckTypingMethodCollector;

    public RequiredMethodDefinitionBuilder (TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      _implementedInterfaceMethodCollector = new ImplementedInterfaceRequiredMethodDefinitionCollector (targetClassDefinition);
      _introducedInterfaceMethodCollector = new IntroducedInterfaceRequiredMethodDefinitionCollector ();
      _duckTypingMethodCollector = new DuckTypingRequiredMethodDefinitionCollector (targetClassDefinition);
    }

    public void Apply (RequirementDefinitionBase requirement)
    {
      if (requirement.IsEmptyInterface || !requirement.Type.IsInterface)
        return;

      var methodCollector = GetMethodCollector (requirement);
      foreach (var requiredMethodDefinition in methodCollector.CreateRequiredMethodDefinitions (requirement))
        requirement.Methods.Add (requiredMethodDefinition);
    }

    private IRequiredMethodDefinitionCollector GetMethodCollector (RequirementDefinitionBase requirement)
    {
      if (requirement.TargetClass.ImplementedInterfaces.Contains (requirement.Type))
        return _implementedInterfaceMethodCollector;
      else if (requirement.TargetClass.ReceivedInterfaces.ContainsKey (requirement.Type))
        return _introducedInterfaceMethodCollector;
      else
        return _duckTypingMethodCollector;
    }
  }
}

