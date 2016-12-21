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
using Castle.DynamicProxy;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples.DynamicMixinBuilding.Core
{
  internal class BaseRequirements
  {
    private readonly Type _requirementsType;

    private readonly IDictionary<MethodInfo, MethodInfo> _methodToInterfaceMap;

    private BaseRequirements (Type requirementsType, IDictionary<MethodInfo, MethodInfo> methodToInterfaceMap)
    {
      ArgumentUtility.CheckNotNull ("requirementsType", requirementsType);
      ArgumentUtility.CheckNotNull ("methodToInterfaceMap", methodToInterfaceMap);

      _requirementsType = requirementsType;
      _methodToInterfaceMap = methodToInterfaceMap;
    }

    public Type RequirementsType
    {
      get { return _requirementsType; }
    }

    public MethodInfo GetBaseCallMethod (MethodInfo targetMethod)
    {
      return _methodToInterfaceMap[targetMethod];
    }

    public static BaseRequirements BuildBaseRequirements (IEnumerable<MethodInfo> methodsToOverride, string typeName, ModuleScope scope)
    {
      ArgumentUtility.CheckNotNull ("methodsToOverride", methodsToOverride);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      ArgumentUtility.CheckNotNull ("scope", scope);

      CustomClassEmitter requirementsInterface = new CustomClassEmitter (new InterfaceEmitter (scope, typeName));
      
      Dictionary<MethodInfo, MethodInfo> methodToInterfaceMap = new Dictionary<MethodInfo, MethodInfo> ();
      foreach (MethodInfo method in methodsToOverride)
      {
        MethodInfo interfaceMethod = DefineEquivalentInterfaceMethod (requirementsInterface, method);
        methodToInterfaceMap.Add (method, interfaceMethod);
      }

      BaseRequirements result = new BaseRequirements (requirementsInterface.BuildType (), methodToInterfaceMap);
      return result;
    }

    private static MethodInfo DefineEquivalentInterfaceMethod (CustomClassEmitter emitter, MethodInfo method)
    {
      var interfaceMethod = emitter.CreateMethod (
          method.Name,
          MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual,
          method);
      return interfaceMethod.MethodBuilder;
    }
  }
}
